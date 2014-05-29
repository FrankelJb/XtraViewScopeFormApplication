using Microsoft.VisualStudio.TestTools.UnitTesting;
using NationalInstruments.ModularInstruments.NIScope;
using ScopeLibrary.ConfigManagement;
using ScopeLibrary.ConnectionManagement;
using System.IO;
using XtraViewScopeFormApplication;
using XtraViewScopeFormApplication.ConnectionManagement;
using XtraViewScopeFormApplication.Models.Exceptions;

namespace XtraViewScopeTest
{
    [TestClass]
    public class TestXtraViewScopeConnectionManager
    {
        IScopeConnectionManager xtraViewScopeConnectionMananger = new XtraViewScopeConnectionManager();

        #region Exception Tests. First test that all methods throw an exception
        [TestMethod]
        [ExpectedException(typeof(ScopeNotInitialisedException),
            "There was a successful attempt to set the scope's vertical parameters.")]
        public void TestSetVerticalParameters()
        {
            xtraViewScopeConnectionMananger.SetVerticalParameters();
        }

        [TestMethod]
        [ExpectedException(typeof(ScopeNotInitialisedException),
            "There was a succesful attempt to set the scope's horizontal parameters.")]
        public void TestSetHorizontalParameters()
        {
            xtraViewScopeConnectionMananger.SetHorizontalParameters();
        }
        #endregion

        [TestInitialize]
        public void Initiailize()
        {
            
        }

        #region Then test that they all succeed
        [TestMethod]
        public void TestInitialisationSuccessful()
        {
            Program.configManager = new XmlConfigManager();

            foreach (string filename in Directory.GetFiles(@"..\..\Resources"))
            {
                if (filename.ToLower().Contains("config"))
                {
                    Program.configManager.ConfigFilePath = filename;
                }
            }
            Program.configManager.loadConfigDocument();
            xtraViewScopeConnectionMananger = new XtraViewScopeConnectionManager();
            xtraViewScopeConnectionMananger.InitialiseSession();
        }

        [TestMethod]
        public void TestSetVerticalParametersSuccessful()
        {
            xtraViewScopeConnectionMananger.InitialiseSession();
            xtraViewScopeConnectionMananger.SetVerticalParameters();
            Assert.AreEqual(xtraViewScopeConnectionMananger.ScopeSession.Channels[ScopeTriggerSource.Channel0].Range, xtraViewScopeConnectionMananger.VerticalRange, "Range of scope session is not the same as the range of the connection manager");
            Assert.AreEqual(xtraViewScopeConnectionMananger.ScopeSession.Channels[ScopeTriggerSource.Channel0].Offset, xtraViewScopeConnectionMananger.Offset, "Offset of scope session is not the same as the offset the of connection manager");
            Assert.AreEqual(xtraViewScopeConnectionMananger.ScopeSession.Channels[ScopeTriggerSource.Channel0].Coupling, xtraViewScopeConnectionMananger.Coupling, "Coupling of scope session not the same as the coupling of the connection manager");
            Assert.AreEqual(xtraViewScopeConnectionMananger.ScopeSession.Channels[ScopeTriggerSource.Channel0].ProbeAttenuation, xtraViewScopeConnectionMananger.ProbeAttenuation, "Probe attenuation of scope is session not the same as the probe attenuation of the connection manager");
        }

        [TestMethod]
        public void TestSetHorizontalParametersSuccessful()
        {
            xtraViewScopeConnectionMananger.InitialiseSession();
            xtraViewScopeConnectionMananger.SetHorizontalParameters();
            Assert.AreEqual(xtraViewScopeConnectionMananger.ScopeSession.Acquisition.RecordLength, xtraViewScopeConnectionMananger.NumberOfPointsMin, "Number of samples in a record of scope session is not the same as the number of samples in a record of the connection manager");            
        }
        #endregion
    }
}
