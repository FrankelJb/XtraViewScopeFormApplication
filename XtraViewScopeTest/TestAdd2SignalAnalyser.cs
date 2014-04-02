using Microsoft.VisualStudio.TestTools.UnitTesting;
using NationalInstruments;
using Newtonsoft.Json;
using ScopeLibrary.SignalAnalysis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using XtraViewScope.Models;
using XtraViewScope.Models.Dictionaries;
using XtraViewScope.Models.Enums;
using XtraViewScope.ScopeAnalysis;
using XtraViewScopeFormApplication.Models.Enums;
using XtraViewScopeFormApplication.Models.XmpPackets;

namespace XtraViewScopeTest
{
    /// <summary>
    /// Summary description for Add2SignalAnalyserTest
    /// </summary>
    [TestClass]
    public class TestAdd2SignalAnalyser
    {
        public TestAdd2SignalAnalyser()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestNibbleCreationSucessful()
        {
            Nibble nibble = null;
            for (int i = 0; i < 16; i++)
            {
                nibble = createNibble(i);
                Assert.AreEqual(nibble.BinaryValue, (BinaryValue)i, "Nibble should have a binary value of " + (BinaryValue)i);
                Assert.AreEqual(nibble.DecimalValue, i, "Nibble should have a decimal value of " + i);
            }
        }

        [TestMethod]
        public void TestNibbleCreationFailure()
        {
            Nibble nibble = null;
            for (int i = 0; i <= 16; i++)
            {
                nibble = createFailedNibble(i);
                Assert.IsTrue(nibble.TimeDeviation > 50 || nibble.TimeDeviation < -50);
            }
        }

        [TestMethod]
        public void TestAdd2PacketCreationSucessful()
        {
            Add2Packet add2Packet = new Add2Packet();
            add2Packet.Nibbles = new Collection<Nibble>();

            for (int i = 0; i < 8; i++)
            {
                add2Packet.Nibbles.Add(createAdd2Nibble(i));

            }

            Assert.AreEqual(add2Packet.Nibbles[0].DecimalValue, 14, "The first nibble's decimal value should be 14");
            Assert.AreEqual(add2Packet.Checksum, 0, "The checksum of the add2packet must be 0");
            Assert.AreEqual(add2Packet.MessageType, (MessageType) 0, "This is the first add2packet and should have a message type of " + (MessageType) 0);
            Assert.AreEqual(add2Packet.SequenceNumber, 0, "This is the first add2packet in the sequence and should have a sequence number of 0");

        }

        #region Nibble creation
        public Nibble createNibble(int nibbleNum)
        {
            Nibble nibble = new Nibble();
            nibble.PulseStartTime = PrecisionDateTime.Now;

            switch (nibbleNum)
            {
                case 0:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000915);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 1:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001051);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 2:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001188);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 3:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001325);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 4:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001461);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 5:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001598);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 6:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001735);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 7:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001872);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 8:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002008);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 9:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002145);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 10:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002282);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 11:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002418);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 12:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002555);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 13:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002692);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 14:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002829);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 15:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002965);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
            }

            nibble.BinaryValue = Add2Dictionary.GetBinaryData(nibble.TotalDuration.FractionalSeconds * 1000000);

            return nibble;
        }

        public Nibble createFailedNibble(int nibbleNum)
        {
            Nibble nibble = new Nibble();
            nibble.PulseStartTime = PrecisionDateTime.Now;

            switch (nibbleNum)
            {
                case 0:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000915);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 1:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001051);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 2:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001188);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 3:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001325);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 4:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001461);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 5:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001598);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 6:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001735);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 7:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001872);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 8:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002008);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 9:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002145);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 10:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002282);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 11:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002418);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 12:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002555);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 13:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002692);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 14:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002829);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 15:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002965);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 16:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.003076);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000010);
                        break;
                    }
            }

            return nibble;
        }

        public Nibble createAdd2Nibble(int nibbleNum)
        {
            Nibble nibble = new Nibble();
            nibble.PulseStartTime = PrecisionDateTime.Now;

            switch (nibbleNum)
            {
                case 0:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.001884);
                        break;
                    }
                case 1:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000230);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.001560);
                        break;
                    }
                case 2:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000220);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000750);
                        break;
                    }
                case 3:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000225);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000745);
                        break;
                    }
                case 4:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000225);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000745);
                        break;
                    }
                case 5:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000225);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000745);
                        break;
                    }
                case 6:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000220);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.002390);
                        break;
                    }
                case 7:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000225);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000745);
                        break;
                    }
            }

            nibble.BinaryValue = Add2Dictionary.GetBinaryData(nibble.TotalDuration.FractionalSeconds * 1000000);

            return nibble;
        }
        #endregion

        [TestMethod]
        public void testAlgorithm()
        {
            List<double> waveform = new List<double>();

            using (StreamReader r = new StreamReader(@"C:\Projects\XtraViewScopeFormApplication\XtraViewScopeFormApplication\Resources\dataPoints.txt"))
            {
                // 3
                // Use while != null pattern for loop
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    waveform.Add(Convert.ToDouble(line));
                }
            }

            PrecisionDateTime period = PrecisionDateTime.Now + PrecisionTimeSpan.FromSeconds(44.4154143);

            SignalAnalysisResultContainer signalAnalysisResultContainer = new SignalAnalysisResultContainer();
            Add2SignalAnalysisResult add2SignalAnalysisResult = new Add2SignalAnalysisResult();
            //add2SignalAnalysisResult.Heartbeat = new Heartbeat();

            Collection<Add2Packet> add2Packets = new Collection<Add2Packet>();
            Add2Packet currentAdd2Packet = null;
            int currentAdd2PacketIndex = 0;

            Nibble currentNibble = null;
            int currentNibbleIndex = 0;

            int i = 0;

            StringBuilder sb = new StringBuilder();
            while (i < waveform.Count)
            {
                //TODO: remove this sb stuff
                sb.Append(period.ToString("ss.FFFFFFF"));
                sb.Append("\t");
                sb.Append(waveform[i].ToString());
                sb.Append(Environment.NewLine);
                //Lets make sure that we have the proper amplitude, direction doesn't matter. Rounding to avoid 0.99999... != 1 (Which is false)
                double currentValue = Math.Round(Math.Abs(waveform[i]), 3);
                if (currentValue < 0.5)
                {
                    if (currentAdd2Packet == null)
                    {
                        i++;
                        continue;
                    }
                    else if (add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].PulseEndTime.FractionalSeconds == 0.0)
                    {
                        bool isNoise = false;
                        int countNoiseThreshold = 0;
                        //We need to make sure that just because we find a value below the threshold that we can still be sure that we are in a pulse.
                        //The finer the resolution, the larger this gap could potentially be
                        for (int j = 1; j <= 10; j++)
                        {
                            if (i + j < waveform.Count)
                            {
                                double nextValue = Math.Abs(waveform[i + j]);
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
                            add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].PulseEndTime = period - PrecisionTimeSpan.FromSeconds(0.000005);
                            add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].NoiseStartTime = period;
                        }
                    }
                }
                else if (currentValue >= 0.5)
                {
                    //We are at the start of a new XMP Packet, so lets make a new one
                    if (currentAdd2Packet == null || add2Packets.Count < currentAdd2PacketIndex + 1)
                    {
                        currentAdd2Packet = new Add2Packet(new Collection<Nibble>());
                        add2Packets.Add(currentAdd2Packet);
                    }

                    //The XMP Packet isn't finished transmitting, so lets get the next nibble
                    if (add2Packets[currentAdd2PacketIndex].Nibbles.Count < currentNibbleIndex + 1)
                    {
                        currentNibble = new Nibble();
                        if (add2Packets[currentAdd2PacketIndex].Nibbles.Count == 0)
                        {
                            //If this is the first nibble then we can use this startTime for the first peak value.
                            currentNibble.PulseStartTime = period - PrecisionTimeSpan.FromSeconds(0.000005);
                        }
                        else
                        {
                            //Otherwise we should use the end of the noise from the previous nibble for the startTime
                            currentNibble.PulseStartTime = add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex - 1].NoiseEndTime + PrecisionTimeSpan.FromSeconds(0.000005);
                            //currentNibble.pulseDuration.totalTime = startTime.FractionalSeconds; //TODO: same for this one
                        }

                        add2Packets[currentAdd2PacketIndex].Nibbles.Add(currentNibble);
                    }
                    //This is the end of the nibble, so calculate the end time and move to the next nibble
                    else if (add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].NoiseStartTime.FractionalSeconds != 0.0 &&
                        add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].NoiseEndTime.FractionalSeconds == 0.0)
                    {
                        add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].NoiseEndTime = period - PrecisionTimeSpan.FromSeconds(0.000005);

                        //If the currentNibble is the ninth nibble then we've finished with this packet and we should move onto the next one
                        if (currentNibbleIndex == 8)
                        {
                            //This isn't actually a nibble, the last pulse is used to indicate this is the end of the XMP packet
                            add2Packets[currentAdd2PacketIndex].Nibbles.RemoveAt(currentNibbleIndex);
                            currentNibbleIndex = 0;
                            currentAdd2PacketIndex++;
                        }
                        else
                        {
                            try
                            {
                                add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].BinaryValue =
                                    Add2Dictionary.GetBinaryData(add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].TotalDuration.FractionalSeconds * 1000000);
                            }
                            catch (KeyNotFoundException)
                            {
                                string logMessage = "Nibble duration was not in an acceptable range: sequence number " + (currentAdd2PacketIndex) +
                                                    ", nibble number " + (currentNibbleIndex) + ", duration found " +
                                                    add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].TotalDuration.FractionalSeconds * 1000000;
                                //Program.log.Error(logMessage);
                                WriteFullTimeDataToFile(sb);
                                add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].IsNotValid = true;
                                System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(add2Packets, new NibbleJsonConverter()));
                                //break;
                            }

                            //Once we've finished using this nibble, we move onto the next one.
                            currentNibbleIndex++;
                        }


                        bool isNoise = false;
                        int countNoiseThreshold = 0;
                        //We need to make sure that just because we find a value below the threshold that we can still be sure that we are in a pulse.
                        //The finer the resolution, the larger this gap could potentially be
                        for (int j = 1; j <= 10; j++)
                        {
                            if (i + j < waveform.Count)
                            {
                                double nextValue = Math.Abs(waveform[i + j]);
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
                            add2Packets[currentAdd2PacketIndex].Nibbles.Add(currentNibble);
                        }
                        else
                        {
                            //If we found samples below the threshold but it appears to be part of the pulse, then we need to skip over those samples and continue from the next peak.
                            //These samples are false noise, so we need to treat them differently.
                            i += countNoiseThreshold;
                        }
                    }
                }

                period += PrecisionTimeSpan.FromSeconds(0.000005);

                i++;
            }
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(add2Packets, new NibbleJsonConverter()));

            WriteFullTimeDataToFile(sb);

            //There is one nibble that remains on the last packet because there is only noise at the end of the heartbeat
            if (add2Packets.Count == 9 && add2Packets[8].Nibbles.Count == 9)
            {
                add2Packets[8].Nibbles.RemoveAt(8);
            }
        }


        //TODO: Delete this method
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
    }
}
