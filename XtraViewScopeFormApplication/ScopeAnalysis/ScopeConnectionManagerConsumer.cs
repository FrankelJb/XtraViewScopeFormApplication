using Newtonsoft.Json;
using ScopeLibrary.ConnectionManagement;
using ScopeLibrary.SignalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XtraViewScope.Models;
using XtraViewScope.ScopeAnalysis;

namespace XtraViewScopeFormApplication.ScopeAnalysis
{
    public class ScopeConnectionManagerConsumer
    {
        public void consumeConnectionManager()
        {
            while (Program.runConnectionManagerConsumer)
            {
                IScopeConnectionManager scopeConnectionManager = Program.scopeConnectionBlockingQueue.Take();

                SignalAnalyser signalAnalyser = new Add2SignalAnalyser();
                signalAnalyser.StartTime = scopeConnectionManager.StartTime;
                signalAnalyser.Waveforms = scopeConnectionManager.Waveforms;
                signalAnalyser.WaveformInfo = scopeConnectionManager.WaveformInfo;

                Task.Factory.StartNew(signalAnalyser.analyseGenericSignal);
            }
        }
    }
}
