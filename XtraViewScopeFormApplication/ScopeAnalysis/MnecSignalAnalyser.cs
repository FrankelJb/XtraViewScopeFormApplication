using NationalInstruments;
using ScopeLibrary;
using ScopeLibrary.SignalAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using XtraViewScopeFormApplication.Models.Dictionaries;
using XtraViewScopeFormApplication.Models.MnecTransmission;
using XtraViewScopeFormApplication.Models.XmpTransmission;

namespace XtraViewScopeFormApplication.ScopeAnalysis
{
    public class MnecSignalAnalyser : SignalAnalyser
    {
        public override void analyseScopeSignal()
        {
            PrecisionDateTime period = StartTime + PrecisionTimeSpan.FromSeconds(WaveformInfo[0].AbsoluteInitialX);

            //Always take the 0th element because the AnalogWaveformCollection contains all the channels returned. This can be changed to be more generic later
            AnalogWaveform<double> waveform = Waveforms[0];

            SignalAnalysisResultContainer signalAnalysisResultContainer = new SignalAnalysisResultContainer();
            ISignalAnalysisResult mnecSignalAnalysisResult = new MnecSignalAnalysisResult();

            Collection<AbstractIrPacket> mnecPackets = new Collection<AbstractIrPacket>();
            MnecPacket currentMnecPacket = null;
            int currentMnecPacketIndex = 0;

            Bit currentBit = null;
            int currentBitIndex = 0;

            int i = 0;

            StringBuilder sb = new StringBuilder();
            while (i < waveform.SampleCount)
            {
                //sb.Append(period);
                //sb.Append("\t");
                //sb.Append(waveform.Samples[i].Value.ToString());
                //sb.Append(Environment.NewLine);
                //Lets make sure that we have the proper amplitude, direction doesn't matter. Rounding to avoid 0.99999... != 1 (Which is false)
                double currentValue = Math.Round(Math.Abs(waveform.Samples[i].Value), 3);
                if (currentValue < 0.5)
                {
                    if (currentMnecPacket == null)
                    {
                        i++;
                        continue;
                    }
                    else if (mnecPackets[currentMnecPacketIndex].InformationUnits[currentBitIndex].PulseEndTime.FractionalSeconds == 0.0)
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
                            mnecPackets[currentMnecPacketIndex].InformationUnits[currentBitIndex].PulseEndTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);
                            mnecPackets[currentMnecPacketIndex].InformationUnits[currentBitIndex].NoiseStartTime = period;
                        }
                    }
                }
                else if (currentValue >= 0.5)
                {
                    //We are at the start of a new XMP Packet, so lets make a new one
                    if (currentMnecPacket == null || mnecPackets.Count < currentMnecPacketIndex + 1)
                    {
                        currentMnecPacket = new MnecPacket(new Collection<AbstractInfromationUnit>());
                        mnecPackets.Add(currentMnecPacket);
                    }

                    //The XMP Packet isn't finished transmitting, so lets get the next nibble
                    if (mnecPackets[currentMnecPacketIndex].InformationUnits.Count < currentBitIndex + 1)
                    {
                        currentBit = new Bit();
                        if (mnecPackets[currentMnecPacketIndex].InformationUnits.Count == 0)
                        {
                            //If this is the first nibble then we can use this startTime for the first peak value.
                            currentBit.PulseStartTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);
                        }
                        else
                        {
                            //Otherwise we should use the end of the noise from the previous nibble for the startTime plus the x-increment
                            currentBit.PulseStartTime = mnecPackets[currentMnecPacketIndex].InformationUnits[currentBitIndex - 1].NoiseEndTime + PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);
                            //currentNibble.pulseDuration.totalTime = startTime.FractionalSeconds; //TODO: same for this one
                        }

                        mnecPackets[currentMnecPacketIndex].InformationUnits.Add(currentBit);
                    }
                    //This is the end of the nibble, so calculate the end time and move to the next nibble
                    else if (mnecPackets[currentMnecPacketIndex].InformationUnits[currentBitIndex].NoiseStartTime.FractionalSeconds != 0.0 &&
                        mnecPackets[currentMnecPacketIndex].InformationUnits[currentBitIndex].NoiseEndTime.FractionalSeconds == 0.0)
                    {
                        //We've found the start of the next pulse so set the end of the noise to one x-increment before
                        mnecPackets[currentMnecPacketIndex].InformationUnits[currentBitIndex].NoiseEndTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);

                        //If the currentNibble is the ninth nibble then we've finished with this packet and we should move onto the next one
                        if (currentBitIndex == 34)
                        {
                            //This isn't actually a nibble, the last pulse is used to indicate this is the end of the XMP packet
                            mnecPackets[currentMnecPacketIndex].InformationUnits.RemoveAt(currentBitIndex);
                            currentBitIndex = 0;
                            currentMnecPacketIndex++;
                        }
                        else
                        {
                            try
                            {
                                mnecPackets[currentMnecPacketIndex].InformationUnits[currentBitIndex].BinaryValue =
                                    MnecDictionary.GetBinaryData(ScopeLibrary.Util.TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(mnecPackets[currentMnecPacketIndex].InformationUnits[currentBitIndex].TotalDuration));
                            }
                            catch (KeyNotFoundException)
                            {
                                //System.Diagnostics.Debug.WriteLine(ScopeLibrary.Util.TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(mnecPackets[currentMnecPacketIndex].InformationUnits[currentBitIndex].TotalDuration));
                                string logMessage = "Nibble duration was not in an acceptable range: sequence number " + (currentMnecPacketIndex) +
                                                    ", nibble number " + (currentBitIndex) + ", duration found " +
                                                    ScopeLibrary.Util.TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(mnecPackets[currentMnecPacketIndex].InformationUnits[currentBitIndex].TotalDuration);
                                Program.log.Error(logMessage);
                                mnecPackets[currentMnecPacketIndex].InformationUnits[currentBitIndex].IsNotValid = true;
                                //break;
                            }

                            //Once we've finished using this nibble, we move onto the next one.
                            currentBitIndex++;
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
                            currentBit = new Bit();
                            currentBit.PulseStartTime = period;

                            //currentXmpPacket = new XmpPacket(currentXmpPacketIndex, new Collection<Nibble>());
                            //waveformTiming.XmpPackets.Add(currentXmpPacket);
                            mnecPackets[currentMnecPacketIndex].InformationUnits.Add(currentBit);
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

            //WriteData(sb);
            int c = 0;
            StringBuilder hexBuilder = new StringBuilder();
            for (int j = 1; j < mnecPackets[0].InformationUnits.Count; j++)
            {
                hexBuilder.Append(mnecPackets[0].InformationUnits[j].DecimalValue);
                if (hexBuilder.Length == 4)
                {
                    System.Diagnostics.Debug.Write(String.Format("{0:X}", Convert.ToByte(hexBuilder.ToString(), 2)));
                    hexBuilder.Clear();
                }
            }
            System.Diagnostics.Debug.WriteLine("");

            if (mnecPackets[0].InformationUnits[0].DecimalValue == 2)
            {
                //    //IR button presses are sent multiple times if the user long-presses a button, truncate the message to be 2 packets long
                for (int j = mnecPackets.Count - 1; j > 0; j--)
                {
                    mnecPackets.RemoveAt(j);
                }
                //There is one nibble that remains on the last packet because there is only noise at the end of the heartbeat
                if (mnecPackets.Count == 9 && mnecPackets[8].InformationUnits.Count == 9)
                {
                    mnecPackets[8].InformationUnits.RemoveAt(8);
                }

                mnecSignalAnalysisResult.PacketTransmission = new XtraViewScopeFormApplication.Models.MnecTransmission.IrInbound();
                mnecSignalAnalysisResult.PacketTransmission.IrPackets = mnecPackets;
                mnecSignalAnalysisResult.PacketTransmission.TimeCaptured = StartTime + PrecisionTimeSpan.FromSeconds(WaveformInfo[0].AbsoluteInitialX);

                signalAnalysisResultContainer.SignalAnalysisResult = mnecSignalAnalysisResult;
                TransmissionDelegates.raiseIrInboundAnalysed(signalAnalysisResultContainer);
            }
            //else if(add2Packets[0].Nibbles[0].DecimalValue == 1)
            //{
            //    //IR button presses are sent multiple times if the user long-presses a button, truncate the message to be 2 packets long
            //    for (int j = add2Packets.Count - 1; j > 1 ; j--)
            //    {
            //        add2Packets.RemoveAt(j);
            //    }

            //    //Remove the noise at the end
            //    if (add2Packets[add2Packets.Count - 1].Nibbles.Count == 9)
            //    {
            //        add2Packets[add2Packets.Count - 1].Nibbles.RemoveAt(8);
            //    }

            //    add2SignalAnalysisResult.XmpPacketTransmission = new IrInbound();
            //    add2SignalAnalysisResult.XmpPacketTransmission.Add2Packets = add2Packets;

            //    signalAnalysisResultContainer.SignalAnalysisResult = add2SignalAnalysisResult;
            //    XmpTransmissionDelegates.raiseIrInboundAnalysed(signalAnalysisResultContainer);
            //}
        }

        public override void determineSignalType(object sender, EventArgs e)
        {
            PrecisionDateTime period = StartTime + PrecisionTimeSpan.FromSeconds(WaveformInfo[0].AbsoluteInitialX);
            AnalogWaveform<double> waveform = Waveforms[0];
            Bit bit = null;
            int i = 0;

            while (i < waveform.SampleCount)
            {
                //Lets make sure that we have the proper amplitude, direction doesn't matter. Rounding to avoid 0.99999... != 1 (Which is false)
                double currentValue = Math.Round(Math.Abs(waveform.Samples[i].Value), 3);
                if (currentValue < 0.5)
                {
                    if (bit == null)
                    {
                        i++;
                        continue;
                    }
                    else if (bit.PulseEndTime.FractionalSeconds == 0.0)
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
                            bit.PulseEndTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);
                            bit.NoiseStartTime = period;
                        }
                    }
                }
                else if (currentValue >= 0.5)
                {
                    //We are at the start of a new XMP Packet, so lets make a new one
                    if (bit == null)
                    {
                        bit = new Bit();
                        bit.PulseStartTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);
                    }

                    if (bit.NoiseStartTime.FractionalSeconds != 0.0 &&
                        bit.NoiseEndTime.FractionalSeconds == 0.0)
                    {

                        bit.NoiseEndTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);

                        try
                        {
                            if (MnecDictionary.GetBinaryData(ScopeLibrary.Util.TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(bit.TotalDuration)) == BinaryValue.b0010)
                            {
                                analyseScopeSignal();
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch
                        {
                            break;
                        }
                        

                    }
                }

                period += PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);

                i++;
            }
        }
    }
}
