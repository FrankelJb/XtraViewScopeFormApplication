using NationalInstruments;

namespace XtraViewScopeFormApplication.Models.XmpTransmission
{
    public class HeartbeatTiming
    {
        public int numberOfHeartbeats { get; set; }
        public PrecisionTimeSpan? Total { get; set; }
        public double Average { get; set; }
        public PrecisionTimeSpan? Shortest { get; set; }
        public PrecisionTimeSpan? Longest { get; set; }
        private PrecisionDateTime? latestHeartbeatDateTime;
        public PrecisionDateTime? LatestHeartbeatDateTime
        {
            get
            {
                return latestHeartbeatDateTime;
            }
            set
            {
                if (latestHeartbeatDateTime == null)
                {
                    latestHeartbeatDateTime = value;
                    Shortest = PrecisionTimeSpan.MaxValue;
                    Longest = PrecisionTimeSpan.MinValue;
                    Total = new PrecisionTimeSpan();
                }
                else
                {
                    if (Shortest.Value > value - latestHeartbeatDateTime.Value)
                    {
                        Shortest = value - latestHeartbeatDateTime.Value;
                    }
                    if (Longest.Value < value - latestHeartbeatDateTime.Value)
                    {
                        Longest = value - latestHeartbeatDateTime.Value;
                    }

                    Total += value - latestHeartbeatDateTime;
                    numberOfHeartbeats++;
                    latestHeartbeatDateTime = value;
                    Average = Total.Value.TotalSeconds / numberOfHeartbeats;

                }
            }
        }
    }
}
