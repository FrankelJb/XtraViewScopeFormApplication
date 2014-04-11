using NationalInstruments;
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
            MnecSignalAnalysisResult mnecSignalAnalysisResult = new MnecSignalAnalysisResult();

            Collection<MnecPacket> mnecPackets = new Collection<MnecPacket>();
            MnecPacket currentMnecPacket = null;
            int currentMnecPacketIndex = 0;

            Nibble currentNibble = null;
            int currentNibbleIndex = 0;

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
                    else if (mnecPackets[currentMnecPacketIndex].Nibbles[currentNibbleIndex].PulseEndTime.FractionalSeconds == 0.0)
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
                            mnecPackets[currentMnecPacketIndex].Nibbles[currentNibbleIndex].PulseEndTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);
                            mnecPackets[currentMnecPacketIndex].Nibbles[currentNibbleIndex].NoiseStartTime = period;
                        }
                    }
                }
                else if (currentValue >= 0.5)
                {
                    //We are at the start of a new XMP Packet, so lets make a new one
                    if (currentMnecPacket == null || mnecPackets.Count < currentMnecPacketIndex + 1)
                    {
                        currentMnecPacket = new MnecPacket(new Collection<Nibble>());
                        mnecPackets.Add(currentMnecPacket);
                    }

                    //The XMP Packet isn't finished transmitting, so lets get the next nibble
                    if (mnecPackets[currentMnecPacketIndex].Nibbles.Count < currentNibbleIndex + 1)
                    {
                        currentNibble = new Nibble();
                        if (mnecPackets[currentMnecPacketIndex].Nibbles.Count == 0)
                        {
                            //If this is the first nibble then we can use this startTime for the first peak value.
                            currentNibble.PulseStartTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);
                        }
                        else
                        {
                            //Otherwise we should use the end of the noise from the previous nibble for the startTime plus the x-increment
                            currentNibble.PulseStartTime = mnecPackets[currentMnecPacketIndex].Nibbles[currentNibbleIndex - 1].NoiseEndTime + PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);
                            //currentNibble.pulseDuration.totalTime = startTime.FractionalSeconds; //TODO: same for this one
                        }

                        mnecPackets[currentMnecPacketIndex].Nibbles.Add(currentNibble);
                    }
                    //This is the end of the nibble, so calculate the end time and move to the next nibble
                    else if (mnecPackets[currentMnecPacketIndex].Nibbles[currentNibbleIndex].NoiseStartTime.FractionalSeconds != 0.0 &&
                        mnecPackets[currentMnecPacketIndex].Nibbles[currentNibbleIndex].NoiseEndTime.FractionalSeconds == 0.0)
                    {
                        //We've found the start of the next pulse so set the end of the noise to one x-increment before
                        mnecPackets[currentMnecPacketIndex].Nibbles[currentNibbleIndex].NoiseEndTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);

                        //If the currentNibble is the ninth nibble then we've finished with this packet and we should move onto the next one
                        //if (currentNibbleIndex == 30)
                        //{
                        //    //This isn't actually a nibble, the last pulse is used to indicate this is the end of the XMP packet
                        //    mnecPackets[currentMnecPacketIndex].Nibbles.RemoveAt(currentNibbleIndex);
                        //    currentNibbleIndex = 0;
                        //    currentMnecPacketIndex++;
                        //}
                        //else
                        //{
                            try
                            {
                                mnecPackets[currentMnecPacketIndex].Nibbles[currentNibbleIndex].BinaryValue =
                                    MnecDictionary.GetBinaryData(ScopeLibrary.Util.TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(mnecPackets[currentMnecPacketIndex].Nibbles[currentNibbleIndex].TotalDuration));
                            }
                            catch (KeyNotFoundException)
                            {
                                System.Diagnostics.Debug.WriteLine(ScopeLibrary.Util.TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(mnecPackets[currentMnecPacketIndex].Nibbles[currentNibbleIndex].TotalDuration));
                                string logMessage = "Nibble duration was not in an acceptable range: sequence number " + (currentMnecPacketIndex) +
                                                    ", nibble number " + (currentNibbleIndex) + ", duration found " +
                                                    ScopeLibrary.Util.TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(mnecPackets[currentMnecPacketIndex].Nibbles[currentNibbleIndex].TotalDuration);
                                Program.log.Error(logMessage);
                                mnecPackets[currentMnecPacketIndex].Nibbles[currentNibbleIndex].IsNotValid = true;
                                //break;
                            }

                            //Once we've finished using this nibble, we move onto the next one.
                            currentNibbleIndex++;
                        //}


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
                            mnecPackets[currentMnecPacketIndex].Nibbles.Add(currentNibble);
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
            foreach (Nibble nibble in mnecPackets[0].Nibbles)
            {
                if (c % 4 == 0)
                {
                    System.Diagnostics.Debug.Write("    ");
                }
                System.Diagnostics.Debug.Write(String.Format("{0:X}", nibble.DecimalValue));
                c++;

            }

            if (mnecPackets[0].Nibbles[0].DecimalValue == 2)
            {
                //There is one nibble that remains on the last packet because there is only noise at the end of the heartbeat
                if (mnecPackets.Count == 9 && mnecPackets[8].Nibbles.Count == 9)
                {
                    mnecPackets[8].Nibbles.RemoveAt(8);
                }

                mnecSignalAnalysisResult.MnecPacketTransmission = new XtraViewScopeFormApplication.Models.MnecTransmission.IrInbound();
                mnecSignalAnalysisResult.MnecPacketTransmission.MnecPackets = mnecPackets;
                mnecSignalAnalysisResult.MnecPacketTransmission.TimeCaptured = StartTime + PrecisionTimeSpan.FromSeconds(WaveformInfo[0].AbsoluteInitialX);

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
            Nibble nibble = null;
            int i = 0;

            while (i < waveform.SampleCount)
            {
                //Lets make sure that we have the proper amplitude, direction doesn't matter. Rounding to avoid 0.99999... != 1 (Which is false)
                double currentValue = Math.Round(Math.Abs(waveform.Samples[i].Value), 3);
                if (currentValue < 0.5)
                {
                    if (nibble == null)
                    {
                        i++;
                        continue;
                    }
                    else if (nibble.PulseEndTime.FractionalSeconds == 0.0)
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
                            nibble.PulseEndTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);
                            nibble.NoiseStartTime = period;
                        }
                    }
                }
                else if (currentValue >= 0.5)
                {
                    //We are at the start of a new XMP Packet, so lets make a new one
                    if (nibble == null)
                    {
                        nibble = new Nibble();
                        nibble.PulseStartTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);
                    }

                    if (nibble.NoiseStartTime.FractionalSeconds != 0.0 &&
                        nibble.NoiseEndTime.FractionalSeconds == 0.0)
                    {

                        nibble.NoiseEndTime = period - PrecisionTimeSpan.FromSeconds(WaveformInfo[0].XIncrement);

                        try
                        {
                            if (MnecDictionary.GetBinaryData(ScopeLibrary.Util.TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(nibble.TotalDuration)) == Models.Enums.BinaryValue.b0010)
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
