using ScopeLibrary.ConnectionManagement;
using ScopeLibrary.SignalAnalysis;
using System.Threading.Tasks;

namespace XtraViewScopeFormApplication.ScopeAnalysis
{
    public class ScopeConnectionManagerConsumer
    {
        public void consumeConnectionManager()
        {
            //while (Program.runConnectionManagerConsumer)
            //{
            //    IScopeConnectionManager scopeConnectionManager = Program.scopeConnectionBlockingCollection.Take();

            //    SignalAnalyser signalAnalyser = new Add2SignalAnalyser();
            //    signalAnalyser.StartTime = scopeConnectionManager.StartTime;
            //    signalAnalyser.Waveforms = scopeConnectionManager.Waveforms;
            //    signalAnalyser.WaveformInfo = scopeConnectionManager.WaveformInfo;

            //    //Task.Factory.StartNew(signalAnalyser.analyseScopeSignal);
            //}
        }
    }
}
