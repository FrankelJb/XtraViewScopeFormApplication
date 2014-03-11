using NationalInstruments;
using Newtonsoft.Json;
using ScopeLibrary.ReportWriting;
using ScopeLibrary.SignalAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XtraViewScope.Models;
using XtraViewScope.Models.Exceptions;
using XtraViewScope.ScopeAnalysis;

namespace XtraViewScope.ReportWriting
{
    public class XmlReportContents : ReportContents
    {
        public override string ToString()
        {
            string jsonString;
            try
            {
                jsonString = JsonConvert.SerializeObject(SignalAnalysisResultContainer.SignalAnalysisResult, new NibbleJsonConverter());
            }
            catch (Exception ex)
            {
                jsonString = JsonConvert.SerializeObject(ex.InnerException);
            }

            //Because c#'s xmlSerializer is not so smart and newtonsoft's json serializer doesn't complain about types, we serialize into a json string and then deserialize into xml.
            return JsonConvert.DeserializeXNode(jsonString, "XtraViewScope").ToString();
        }
    }
}
