using NationalInstruments;
using NationalInstruments.ModularInstruments.NIScope;
using Newtonsoft.Json;
using ScopeLibrary;
using ScopeLibrary.ConnectionManagement;
using ScopeLibrary.ReportWriting;
using ScopeLibrary.SignalAnalysis;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using XtraViewScope.Models;
using XtraViewScope.Models.Dictionaries;
using XtraViewScope.ReportWriting;
using XtraViewScope.ScopeAnalysis;
using XtraViewScopeFormApplication;

namespace XtraViewScope.ConnectionManagement
{
    public class XtraViewScopeConnectionManager : IScopeConnectionManager
    {
        
        private IConfigManager ConfigManager;

        public XtraViewScopeConnectionManager()
        {
            ConfigManager = Program.configManager;
            Program.report = new Report();

            if (Program.configManager.getProperty("ReportContents") != null)
            {
                if (Program.configManager.getProperty("ReportContents").Equals("Xml"))
                {
                    Program.reportContents = new XmlReportContents();
                }
                else
                {
                    Program.reportContents = new JsonReportContents();
                }
            }
            else
            {
                Program.reportContents = new JsonReportContents();
            }
        }

        private string channelName = "0";
        public string ChannelName
        {
            get
            {
                return ConfigManager.getProperty("ChannelName");
            }
            set
            {
                channelName = value;
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
            set
            {
                resourceName = value;
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
            set
            {
                idQuery = value;
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
            set
            {
                resetDevice = value;
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
            set
            {
                timeout = value;
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
            set
            {
                numberOfMeasurentSamplesToFetch = value;
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
                Program.reportContents.Exceptions.Add(ex);
            }
        }

        void DriverOperation_Warning(object sender, ScopeWarningEventArgs e)
        {
            Program.reportContents.EventArgs.Add(e);
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
                    Program.reportContents.Exceptions.Add(ex);
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
            set
            {
                coupling = value;
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
            set
            {
                offset = value;
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
            set
            {
                probeAttenuation = value;
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
            set
            {
                verticalRange = value;
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
            set
            {
                enforceRealTime = value;
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
            set
            {
                numberOfRecords = value;
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
            set
            {
                numberOfPointsMin = value;
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
            set
            {
                referencePosition = value;
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
            set
            {
                sampleRateMin = value;
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

        void CalculateSliceTimes(PrecisionDateTime startTime, PrecisionDateTime endTime)
        {
            StringBuilder sb = new StringBuilder();

            //foreach (KeyValuePair<int, WaveformTiming> item in waveformTimingDictionary)
            //{
            PrecisionDateTime period = startTime + PrecisionTimeSpan.FromSeconds(waveformInfo[0].AbsoluteInitialX);

            int i = 0;
            AnalogWaveform<double> waveform = waveforms[0];
            //waveformTiming.XmpPacket = new XmpPacket();
            //waveformTiming.XmpPacket.Nibbles = new Collection<Nibble>();

            Collection<Add2Packet> xmpPackets = new Collection<Add2Packet>();
            Add2Packet currentXmpPacket = null;
            int currentXmpPacketIndex = 0;

            Nibble currentNibble = null;
            int currentNibbleIndex = 0;

            while (i < waveform.SampleCount)
            {
                //sb.Append(startTimeOld.ToString(FULL_DATE_AND_TIME)); Use this to print out of the full date and time of the period
                sb.Append(period.ToString("ss.FFFFFFF"));
                sb.Append("\t");
                sb.Append(waveform.Samples[i].Value.ToString());
                sb.Append(Environment.NewLine);

                //Lets make sure that we have the proper amplitude, direction doesn't matter
                double currentValue = Math.Abs(waveform.Samples[i].Value);
                if (currentValue < 0.5)
                {
                    if (currentXmpPacket == null)
                    {
                        i++;
                        continue;
                    }
                    else if (xmpPackets[currentXmpPacketIndex].Nibbles[currentNibbleIndex].PulseEndTime.FractionalSeconds == 0.0)
                    {
                        bool isNoise = false;
                        int countNoiseThreshold = 0;
                        //We need to make sure that just because we find a value below the threshold that we can still be sure that we are in a pulse.
                        //The finer the resolution, the larger this gap could potentially be
                        for (int j = 1; j <= 10; j++)
                        {
                            if (i + j < waveform.SampleCount)
                            {
                                double nextValue = Math.Abs(waveform.Samples[i + j].Value);
                                if (nextValue < 0.5)
                                {
                                    countNoiseThreshold++;
                                    continue;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        if (countNoiseThreshold > 8)
                        {
                            isNoise = true;
                        }
                        if (isNoise)
                        {
                            //We've found the start of the noise and the end of the pulse
                            xmpPackets[currentXmpPacketIndex].Nibbles[currentNibbleIndex].PulseEndTime = period;

                            xmpPackets[currentXmpPacketIndex].Nibbles[currentNibbleIndex].NoiseStartTime = period;
                        }
                    }
                }
                else if (currentValue >= 0.5)
                {
                    //We are at the start of a new XMP Packet, so lets make a new one
                    if (currentXmpPacket == null || xmpPackets.Count < currentXmpPacketIndex + 1)
                    {
                        currentXmpPacket = new Add2Packet(new Collection<Nibble>());
                        xmpPackets.Add(currentXmpPacket);
                    }

                    //The XMP Packet isn't finished transmitting, so lets get the next nibble
                    if (xmpPackets[currentXmpPacketIndex].Nibbles.Count < currentNibbleIndex + 1)
                    {
                        currentNibble = new Nibble();
                        if (xmpPackets[currentXmpPacketIndex].Nibbles.Count == 0)
                        {
                            //If this is the first nibble then we can use this startTime for the first peak value.
                            currentNibble.PulseStartTime = period;
                            //currentNibble.PulseDuration.Total = startTime.FractionalSeconds; //TODO: Change this total time so that its meaningful or gone
                        }
                        else
                        {
                            //Otherwise we should use the end of the noise from the previous nibble for the startTime
                            currentNibble.PulseStartTime = xmpPackets[currentXmpPacketIndex].Nibbles[currentNibbleIndex - 1].NoiseEndTime;
                            //currentNibble.pulseDuration.totalTime = startTime.FractionalSeconds; //TODO: same for this one
                        }

                        xmpPackets[currentXmpPacketIndex].Nibbles.Add(currentNibble);
                    }
                    //This is the end of the nibble, so calculate the end time and move to the next nibble
                    else if (xmpPackets[currentXmpPacketIndex].Nibbles[currentNibbleIndex].NoiseStartTime.FractionalSeconds != 0.0 &&
                        xmpPackets[currentXmpPacketIndex].Nibbles[currentNibbleIndex].NoiseEndTime.FractionalSeconds == 0.0)
                    {
                        xmpPackets[currentXmpPacketIndex].Nibbles[currentNibbleIndex].NoiseEndTime = period;

                        //Get the nibble's total time
                        //xmpPackets[currentXmpPacketIndex].Nibbles[currentNibbleIndex].TotalDuration =
                        //    xmpPackets[currentXmpPacketIndex].Nibbles[currentNibbleIndex].NoiseSlice.PeriodTotalSpan +
                        //    waveformTiming.XmpPackets[currentXmpPacketIndex].Nibbles[currentNibbleIndex].PulseSlice.PeriodTotalSpan;

                        //And its time deviation
                        //waveformTiming.XmpPackets[currentXmpPacketIndex].Nibbles[currentNibbleIndex].TimeDeviation =
                        //    NibbleDictionary.GetDeviation(waveformTiming.XmpPackets[currentXmpPacketIndex].Nibbles[currentNibbleIndex].TotalTimeSpan.FractionalSeconds * 1000000);
                        //And its decimal value
                        



                        //If the currentNibble is the ninth nibble then we've finished with this packet and we should move onto the next one
                        if (currentNibbleIndex == 8)
                        {
                            //This isn't actually a nibble, the last pulse is used to indicate this is the end of the XMP packet
                            xmpPackets[currentXmpPacketIndex].Nibbles.RemoveAt(currentNibbleIndex);
                            currentNibbleIndex = 0;
                            currentXmpPacketIndex++;
                        }
                        else
                        {
                            xmpPackets[currentXmpPacketIndex].Nibbles[currentNibbleIndex].BinaryValue =
                                Add2Dictionary.GetBinaryData(xmpPackets[currentXmpPacketIndex].Nibbles[currentNibbleIndex].TotalDuration.FractionalSeconds * 1000000);

                            //Once we've finished using this nibble, we move onto the next one.
                            currentNibbleIndex++;
                        }
                        

                        bool isNoise = false;
                        int countNoiseThreshold = 0;
                        //We need to make sure that just because we find a value below the threshold that we can still be sure that we are in a pulse.
                        //The finer the resolution, the larger this gap could potentially be
                        for (int j = 1; j <= 10; j++)
                        {
                            if (i + j < waveform.SampleCount)
                            {
                                double nextValue = Math.Abs(waveform.Samples[i + j].Value);
                                if (nextValue < 0.5)
                                {
                                    countNoiseThreshold++;
                                    continue;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        if (countNoiseThreshold > 8)
                        {
                            isNoise = true;
                        }
                        if (isNoise)
                        {
                            //If the pulse only has one sample then we need to use this data to start the next nibble because we won't be coming back into the currentValue >= 0.5
                            currentNibble = new Nibble();
                            currentNibble.PulseStartTime = period;

                            //currentXmpPacket = new XmpPacket(currentXmpPacketIndex, new Collection<Nibble>());
                            //waveformTiming.XmpPackets.Add(currentXmpPacket);
                            xmpPackets[currentXmpPacketIndex].Nibbles.Add(currentNibble);
                        }
                        else
                        {
                            //If we found samples below the threshold but it appears to be part of the pulse, then we need to skip over those samples and continue from the next peak.
                            //These samples are false noise, so we need to treat them differently.
                            i += countNoiseThreshold;
                        }
                    }
                }

                period += PrecisionTimeSpan.FromSeconds(waveformInfo[0].XIncrement);

                i++;
            }

            WriteFullTimeDataToFile(sb);

            System.Diagnostics.Debug.WriteLine(ToJson(xmpPackets));
        }

        void WriteFullTimeDataToFile(StringBuilder sb)
        {
            string directoryPath = @"C:\waveform";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            FileInfo filePath = new FileInfo(Path.Combine(directoryPath, @"dataPoints.txt"));
            if (!filePath.Exists)
            {
                filePath.Create().Close();
            }
            else
            {
                //File.WriteAllText(filePath.ToString(), String.Empty);
            }

            File.WriteAllText(filePath.ToString(), sb.ToString());
        }

        string ToJson(Object ClassObject)
        {
            return JsonConvert.SerializeObject(ClassObject, Formatting.None, new NibbleJsonConverter());
        }
    }
}
