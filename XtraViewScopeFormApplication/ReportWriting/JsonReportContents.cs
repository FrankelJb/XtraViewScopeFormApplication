using Newtonsoft.Json;
using ScopeLibrary.ReportWriting;
using ScopeLibrary.SignalAnalysis;
using System;
using XtraViewScopeFormApplication.Models.XmpTransmission;

namespace XtraViewScopeFormApplication.ReportWriting
{
    public class JsonReportContents : ReportContents
    {
        public override string ToString()
        {
            string jsonString;
            try
            {
                jsonString = JsonConvert.SerializeObject(SignalAnalysisResultContainer.SignalAnalysisResult, Formatting.Indented, new NibbleJsonConverter());
            }
            catch (Exception ex)
            {
                jsonString = JsonConvert.SerializeObject(ex.InnerException.Message);
            }

            return jsonString;
        }
    }
}
