using NationalInstruments;
using NationalInstruments.ModularInstruments.NIScope;
using ScopeLibrary;
using ScopeLibrary.ConnectionManagement;
using System;
using XtraViewScopeFormApplication;

namespace XtraViewScope.ConnectionManagement
{
    public class XtraViewScopeConnectionManager : IScopeConnectionManager
    {
        
        private IConfigManager ConfigManager;

        public XtraViewScopeConnectionManager()
        {
            ConfigManager = Program.configManager;

        }

        private string channelName = "0";
        public string ChannelName
        {
            get
            {
                if (ConfigManager.getProperty("ChannelName") != null)
                {
                    channelName = ConfigManager.getProperty("ChannelName");
                }

                return channelName;
            }
        }    

        private string resourceName = "Dev1";
        public string ResourceName
        {
            get
            {
                if(ConfigManager.getProperty("ResourceName") != null)
                {
                    resourceName = ConfigManager.getProperty("ResourceName");
                }

                return resourceName;
            }
        }

        private bool idQuery = false;
        public bool IdQuery
        {
            get
            {
                if(ConfigManager.getProperty("IdQuery") != null)
                {
                    try
                    {
                        idQuery = Convert.ToBoolean(ConfigManager.getProperty("IdQuery"));
                    }
                    catch(FormatException)
                    {
                    }
                }
                 
                return idQuery;
            }
        }

        private bool resetDevice = false;
        public bool ResetDevice
        {
            get
            {
                if (ConfigManager.getProperty("ResetDevice") != null)
                {
                    try
                    {
                        resetDevice = Convert.ToBoolean(ConfigManager.getProperty("ResetDevice"));
                    }
                    catch (FormatException)
                    {
                    }
                }

                return resetDevice;
            }
        }

        private PrecisionTimeSpan timeout = new PrecisionTimeSpan(-1);
        public PrecisionTimeSpan Timeout
        {
            get
            {
                if (ConfigManager.getProperty("Timeout") != null)
                {
                    try
                    {
                        timeout = new PrecisionTimeSpan(Convert.ToInt32(ConfigManager.getProperty("Timeout")));
                    }
                    catch (FormatException)
                    {
                    }
                }

                return timeout;
            }
        }

        private long numberOfMeasurentSamplesToFetch = 400;
        public long NumberOfMeasurementSamplesToFetch
        {
            get
            {
                if (ConfigManager.getProperty("NumberOfMeasurentSamplesToFetch") != null)
                {
                    try
                    {
                        numberOfMeasurentSamplesToFetch = Convert.ToInt64(ConfigManager.getProperty("NumberOfMeasurentSamplesToFetch"));
                    }
                    catch (FormatException)
                    {
                    }
                }

                return numberOfMeasurentSamplesToFetch;
            }
        }

        private AnalogWaveformCollection<double> waveforms;
        public AnalogWaveformCollection<double> Waveforms
        {
            get
            {
                return waveforms;
            }
            set
            {
                waveforms = value;
            }
        }

        private ScopeWaveformInfo[] waveformInfo;
        public ScopeWaveformInfo[] WaveformInfo
        {
            get
            {
                return waveformInfo;
            }
            set 
            {
                waveformInfo = value;
            }
        }

        public void InitialiseSession()
        {
            try
            {
                scopeSession = new NIScope(ResourceName, IdQuery, ResetDevice);
                scopeSession.DriverOperation.Warning += new EventHandler<ScopeWarningEventArgs>(DriverOperation_Warning);
            }
            catch(Ivi.Driver.IviCDriverException ex)
            {
                Program.log.Error(ex);
            }
        }

        void DriverOperation_Warning(object sender, ScopeWarningEventArgs e)
        {
            Program.reportWriter.Report.ReportContents.EventArgs.Add(e);
        }

        public NIScope scopeSession;
        public NIScope ScopeSession
        {
            get
            {
                return scopeSession;
            }
        }

        private PrecisionDateTime startTime;
        public PrecisionDateTime StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                startTime = value;
            }
        }

        public void StartAcquisition()
        {
            //Initialise the scope
            InitialiseSession();

            //Set up horizontal and vertical parameters
            SetHorizontalParameters();
            SetVerticalParameters();

            //Set the trigger type. Either Edge or Immediate
            TriggerType();

            scopeSession.Measurement.Initiate();

            //This is configured as a horizontal parameter
            long actualRecordLength = scopeSession.Acquisition.RecordLength;
            scopeSession.Acquisition.NumberOfMeasurementSamplesToFetch = NumberOfMeasurementSamplesToFetch;

            StartTime = PrecisionDateTime.Now;
            waveforms = scopeSession.Channels[ScopeTriggerSource.Channel0].Measurement.FetchDouble(Timeout, actualRecordLength, waveforms, out waveformInfo);
        }

        public void CloseSession()
        {
            if (scopeSession != null)
            {
                try
                {
                    scopeSession.Close();
                    scopeSession = null;
                }
                catch (Exception ex)
                {
                    Program.log.Error(ex);
                }
            }
        }

        public void TriggerType()
        {
            if (ScopeSession == null)
            {
                throw new ScopeNotInitialisedException("Scope must be initialised before trigger can be configured");
            }

            string scopeTriggerString = "Edge";

            if (ConfigManager.getProperty("ScopeTriggerType") != null)
            {
                try
                {
                    scopeTriggerString = ConfigManager.getProperty("NumberOfMeasurentSamplesToFetch");
                }
                catch (FormatException)
                {
                }
            }

            switch (scopeTriggerString)
            {
                case "Immediate":
                    {
                        ScopeSession.Trigger.ConfigureTriggerImmediate();
                        break;
                    }
                case "Edge":
                    {
                        double triggerLevel = 0.5;
                        ScopeTriggerSlope triggerSlope = ScopeTriggerSlope.Positive;
                        ScopeTriggerCoupling triggerCoupling = ScopeTriggerCoupling.DC;
                        ScopeTriggerSource triggerSource = ScopeTriggerSource.Channel0;
                        PrecisionTimeSpan triggerHoldoff = PrecisionTimeSpan.Zero;
                        PrecisionTimeSpan triggerDelay = PrecisionTimeSpan.Zero;
                        ScopeSession.Trigger.EdgeTrigger.Configure(triggerSource, triggerLevel, triggerSlope, triggerCoupling, triggerHoldoff, triggerDelay);

                        break;
                    }
            }
        }

        private ScopeVerticalCoupling coupling = ScopeVerticalCoupling.DC;
        public ScopeVerticalCoupling Coupling
        {
            get
            {
                string couplingProperty = null;
                if(ConfigManager.getProperty("Coupling") != null)
                {
                    couplingProperty = ConfigManager.getProperty("Coupling");
                }
                switch (couplingProperty)
                {
                    case "AC":
                        {
                            coupling = ScopeVerticalCoupling.AC;
                            break;
                        }
                    case "ground":
                        {
                            coupling = ScopeVerticalCoupling.Ground;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                return coupling;
            }
        }

        private double offset = 0.0;
        public double Offset
        {
            get
            {
                if(ConfigManager.getProperty("Offset") != null)
                {
                    try
                    {
                        offset = Convert.ToDouble(ConfigManager.getProperty("Offset"));
                    }
                    catch (FormatException)
                    {
                    }
                }
                return offset;
            }
        }

        private double probeAttenuation = 1.0;
        public double ProbeAttenuation
        {
            get
            {
                if (ConfigManager.getProperty("ProbeAttenuation") != null)
                {
                    try
                    {
                        probeAttenuation = Convert.ToDouble(ConfigManager.getProperty("ProbeAttenuation"));
                    }
                    catch (FormatException)
                    {
                    }
                }
                
                return probeAttenuation;
            }
        }

        private double verticalRange = 4.0;
        public double VerticalRange
        {
            get
            {
                if (ConfigManager.getProperty("VerticalRange") != null)
                {
                    try
                    {
                        verticalRange = Convert.ToDouble(ConfigManager.getProperty("VerticalRange"));
                    }
                    catch (FormatException)
                    {
                    }
                }

                return verticalRange;
            }
        }

        public void SetVerticalParameters()
        {
            if (scopeSession == null)
            {
                throw new ScopeNotInitialisedException("Scope must be initialised before vertical paramters can be set");
            }
            bool enabled = true; //This can be false but that would disable the channel. Which, under these conditions, would be quite silly.
            scopeSession.Channels[ScopeTriggerSource.Channel0].Configure(VerticalRange, Offset, Coupling, ProbeAttenuation, enabled);
        }

        private bool enforceRealTime = false;
        public bool EnforceRealTime
        {
            get
            {
                if (ConfigManager.getProperty("VerticalRange") != null)
                {
                    try
                    {
                        enforceRealTime = Convert.ToBoolean(ConfigManager.getProperty("EnforceRealTime"));
                    }
                    catch (FormatException)
                    {
                    }
                }

                return enforceRealTime;
            }
        }

        //This value can only be 1 with the NI USB-5133 scope.
        private int numberOfRecords = 1;
        public int NumberOfRecords
        {
            get
            {
                if (ConfigManager.getProperty("NumberOfRecords") != null)
                {
                    try
                    {
                        numberOfRecords = Convert.ToInt32(ConfigManager.getProperty("NumberOfRecords"));
                    }
                    catch (FormatException)
                    {
                    }
                }

                return numberOfRecords;
            }
        }

        private int numberOfPointsMin = 70000;
        public int NumberOfPointsMin
        {
            get
            {
                if (ConfigManager.getProperty("NumberOfPointsMin") != null)
                {
                    try
                    {
                        numberOfPointsMin = Convert.ToInt32(ConfigManager.getProperty("NumberOfPointsMin"));
                    }
                    catch (FormatException)
                    {
                    }
                }

                return numberOfPointsMin;
            }
        }

        private double referencePosition = 0.10;
        public double ReferencePosition
        {
            get
            {
                if (ConfigManager.getProperty("ReferencePosition") != null)
                {
                    try
                    {
                        referencePosition = Convert.ToDouble(ConfigManager.getProperty("ReferencePosition"));
                    }
                    catch (FormatException)
                    {
                    }
                }

                return referencePosition;
            }
        }

        private double sampleRateMin = 200000;
        public double SampleRateMin
        {
            get
            {
                if (ConfigManager.getProperty("SampleRateMin") != null)
                {
                    try
                    {
                        sampleRateMin = Convert.ToDouble(ConfigManager.getProperty("SampleRateMin"));
                    }
                    catch (FormatException)
                    {
                    }
                }
                return sampleRateMin;
            }
        }

        public void SetHorizontalParameters()
        {
            if (scopeSession == null)
            {
                throw new ScopeNotInitialisedException("Scope must be initialised before horizontal parameters can be set");
            }
            scopeSession.Timing.ConfigureTiming(SampleRateMin, NumberOfPointsMin, ReferencePosition, NumberOfRecords, EnforceRealTime);
        }
    }
}
