using ScopeLibrary.SignalAnalysis;
using ScopeLibrary.Util;
using System.Collections.Generic;
using XtraViewScopeFormApplication.Models.Dictionaries;

namespace XtraViewScopeFormApplication.Models.XmpTransmission
{
    public class Nibble : AbstractInformationUnit
    {
        public override double OptimalDuration
        {
            get
            {
                try
                {
                    return Add2Dictionary.GetOptimalDuration(TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(TotalDuration));
                }
                catch (KeyNotFoundException)
                {
                    Status ="Nibble duration does not fall within available ranges";
                    IsNotValid = true;

                    return Add2Dictionary.GetFailedOptimalDuration(TimeConversion.PrecisionTimeSpanFractionalSecondComponentToNanos(TotalDuration));
                }
            }
        }
    }
}
