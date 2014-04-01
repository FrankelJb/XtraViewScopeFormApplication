using NationalInstruments;
using NationalInstruments.ModularInstruments.NIScope;
using ScopeLibrary.SignalAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using XtraViewScope.Models;
using XtraViewScope.Models.Dictionaries;
using XtraViewScope.Models.Enums;
using XtraViewScopeFormApplication;
using XtraViewScopeFormApplication.Models.Enums;
using XtraViewScopeFormApplication.Models.XmpPackets;
using XtraViewScopeFormApplication.ScopeAnalysis;

namespace XtraViewScope.ScopeAnalysis
{
    public class Add2SignalAnalyser : SignalAnalyser
    {
        public override SignalAnalysisResultContainer analyseScopeSignal()
        {
            PrecisionDateTime period = StartTime + PrecisionTimeSpan.FromSeconds(WaveformInfo[0].AbsoluteInitialX);

            //Always take the 0th element because the AnalogWaveformCollection contains all the channels returned. This can be changed to be more generic later
            AnalogWaveform<double> waveform = Waveforms[0];

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
            while (i < waveform.SampleCount)
            {
                //TODO: remove this sb stuff
                sb.Append(period.ToString("ss.FFFFFFF"));
                sb.Append("\t");
                sb.Append(waveform.Samples[i].Value.ToString());
                sb.Append(Environment.NewLine);
                //Lets make sure that we have the proper amplitude, direction doesn't matter. Rounding to avoid 0.99999... != 1 (Which is false)
                double currentValue = Math.Round(Math.Abs(waveform.Samples[i].Value), 3);
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
                            //We've found the start of the noise and the end of the pulse.
                            //The end time of the pulse will be one x-increment before the start of the noise
                            add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].PulseEndTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);
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
                            currentNibble.PulseStartTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);
                        }
                        else
                        {
                            //Otherwise we should use the end of the noise from the previous nibble for the startTime plus the x-increment
                            currentNibble.PulseStartTime = add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex - 1].NoiseEndTime + PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);
                            //currentNibble.pulseDuration.totalTime = startTime.FractionalSeconds; //TODO: same for this one
                        }

                        add2Packets[currentAdd2PacketIndex].Nibbles.Add(currentNibble);
                    }
                    //This is the end of the nibble, so calculate the end time and move to the next nibble
                    else if (add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].NoiseStartTime.FractionalSeconds != 0.0 &&
                        add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].NoiseEndTime.FractionalSeconds == 0.0)
                    {
                        //We've found the start of the next pulse so set the end of the noise to one x-increment before
                        add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].NoiseEndTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);

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
                                    Add2Dictionary.GetBinaryData(ScopeLibrary.Util.TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].TotalDuration));
                            }
                            catch (KeyNotFoundException)
                            {
                                string logMessage = "Nibble duration was not in an acceptable range: sequence number " + (currentAdd2PacketIndex) + 
                                                    ", nibble number " + (currentNibbleIndex) + ", duration found " + 
                                                    ScopeLibrary.Util.TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].TotalDuration);
                                Program.log.Error(logMessage); 
                                add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].IsNotValid = true;
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

                period += PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);

                i++;
            }

            //There is one nibble that remains on the last packet because there is only noise at the end of the heartbeat
            if (add2Packets.Count == 9 && add2Packets[8].Nibbles.Count == 9)
            {
                add2Packets[8].Nibbles.RemoveAt(8);
            }

            WriteFullTimeDataToFile(sb);

            //add2SignalAnalysisResult.Heartbeat.Add2Packets = add2Packets;
            signalAnalysisResultContainer.SignalAnalysisResult = add2SignalAnalysisResult;
            //TODO: VERY NB! Fix this method, commented code so that project builds
            return signalAnalysisResultContainer;
        }

        public override void analyseGenericSignal()
        {
            PrecisionDateTime period = StartTime + PrecisionTimeSpan.FromSeconds(WaveformInfo[0].AbsoluteInitialX);

            //Always take the 0th element because the AnalogWaveformCollection contains all the channels returned. This can be changed to be more generic later
            AnalogWaveform<double> waveform = Waveforms[0];

            SignalAnalysisResultContainer signalAnalysisResultContainer = new SignalAnalysisResultContainer();
            Add2SignalAnalysisResult add2SignalAnalysisResult = new Add2SignalAnalysisResult();

            Collection<Add2Packet> add2Packets = new Collection<Add2Packet>();
            Add2Packet currentAdd2Packet = null;
            int currentAdd2PacketIndex = 0;

            Nibble currentNibble = null;
            int currentNibbleIndex = 0;

            int i = 0;

            StringBuilder sb = new StringBuilder();
            while (i < waveform.SampleCount)
            {
                ////TODO: remove this sb stuff
                //sb.Append(period.ToString("ss.FFFFFFF"));
                //sb.Append("\t");
                //sb.Append(waveform.Samples[i].Value.ToString());
                //sb.Append(Environment.NewLine);
                //Lets make sure that we have the proper amplitude, direction doesn't matter. Rounding to avoid 0.99999... != 1 (Which is false)
                double currentValue = Math.Round(Math.Abs(waveform.Samples[i].Value), 3);
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
                            //We've found the start of the noise and the end of the pulse.
                            //The end time of the pulse will be one x-increment before the start of the noise
                            add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].PulseEndTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);
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
                            currentNibble.PulseStartTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);
                        }
                        else
                        {
                            //Otherwise we should use the end of the noise from the previous nibble for the startTime plus the x-increment
                            currentNibble.PulseStartTime = add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex - 1].NoiseEndTime + PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);
                            //currentNibble.pulseDuration.totalTime = startTime.FractionalSeconds; //TODO: same for this one
                        }

                        add2Packets[currentAdd2PacketIndex].Nibbles.Add(currentNibble);
                    }
                    //This is the end of the nibble, so calculate the end time and move to the next nibble
                    else if (add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].NoiseStartTime.FractionalSeconds != 0.0 &&
                        add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].NoiseEndTime.FractionalSeconds == 0.0)
                    {
                        //We've found the start of the next pulse so set the end of the noise to one x-increment before
                        add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].NoiseEndTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);

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
                                    Add2Dictionary.GetBinaryData(ScopeLibrary.Util.TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].TotalDuration));
                            }
                            catch (KeyNotFoundException)
                            {
                                string logMessage = "Nibble duration was not in an acceptable range: sequence number " + (currentAdd2PacketIndex) +
                                                    ", nibble number " + (currentNibbleIndex) + ", duration found " +
                                                    ScopeLibrary.Util.TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].TotalDuration);
                                Program.log.Error(logMessage);
                                add2Packets[currentAdd2PacketIndex].Nibbles[currentNibbleIndex].IsNotValid = true;
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

                period += PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);

                i++;
            }

            //There is one nibble that remains on the last packet because there is only noise at the end of the heartbeat
            if (add2Packets.Count == 9 && add2Packets[8].Nibbles.Count == 9)
            {
                add2Packets[8].Nibbles.RemoveAt(8);
                add2SignalAnalysisResult.XmpPacketTransmission = new Heartbeat();
                add2SignalAnalysisResult.XmpPacketTransmission.Add2Packets = add2Packets;
                add2SignalAnalysisResult.XmpPacketTransmission.XmpPacketTransmissionType = XmpPacketTransmissionType.Heartbeat;
            }
            else
            {
                if (add2Packets[add2Packets.Count - 1].Nibbles.Count == 9)
                {
                    add2Packets[add2Packets.Count - 1].Nibbles.RemoveAt(8);
                }
                else
                {
                    Program.log.Error("User long pressed button, truncating > 3 captured packets");
                    for (int j = 2; j < add2Packets.Count; j++)
                    {
                        add2Packets.RemoveAt(j);
                    }
                    System.Diagnostics.Debug.WriteLine(add2Packets[add2Packets.Count - 1].Nibbles.Count);
                }
                add2SignalAnalysisResult.XmpPacketTransmission = new IrInbound();
                add2SignalAnalysisResult.XmpPacketTransmission.Add2Packets = add2Packets;
                add2SignalAnalysisResult.XmpPacketTransmission.XmpPacketTransmissionType = XmpPacketTransmissionType.IrInbound;

                //foreach (Add2Packet add2Packet in add2Packets)
                //{
                //    foreach (Nibble nibble in add2Packet.Nibbles)
                //    {
                //        sb.Append(String.Format("{0:X}", nibble.DecimalValue));
                //    }
                //    if (sb.Length > 0)
                //        sb.Append(Environment.NewLine);
                //}

                //sb.Append("----" + Environment.NewLine);
                //System.Diagnostics.Debug.WriteLine(sb.ToString());
                //WriteFullTimeDataToFile(sb);
            }


            signalAnalysisResultContainer.SignalAnalysisResult = add2SignalAnalysisResult;

            Program.signalAnalysisResultBlockingQueue.Add(signalAnalysisResultContainer);
        }

        //TODO: Delete this method
        void WriteFullTimeDataToFile(StringBuilder sb)
        {
            string directoryPath = @"C:\waveform";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            FileInfo filePath = new FileInfo(Path.Combine(directoryPath, @"hexKeyPresses.txt"));
            if (!filePath.Exists)
            {
                filePath.Create().Close();
            }
            else
            {
                //File.WriteAllText(filePath.ToString(), String.Empty);
            }

            File.AppendAllText(filePath.ToString(), sb.ToString());
        }
    }
}
