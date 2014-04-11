using ScopeLibrary.SignalAnalysis;
using ScopeLibrary.Util;
using System.Collections.Generic;
using XtraViewScopeFormApplication.Models.Dictionaries;

namespace XtraViewScopeFormApplication.Models.XmpTransmission
{
    public class Bit : AbstractInfromationUnit
    {
        public override double OptimalDuration
        {
            get
            {
                try
                {
                    return MnecDictionary.GetOptimalDuration(TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(TotalDuration));
                }
                catch (KeyNotFoundException)
                {
                    Status ="Bit duration does not fall within available ranges";
                    IsNotValid = true;

                    return MnecDictionary.GetFailedOptimalDuration(TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(TotalDuration));
                }
            }
        }
    }
}
