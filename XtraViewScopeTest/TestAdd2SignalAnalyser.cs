using Microsoft.VisualStudio.TestTools.UnitTesting;
using NationalInstruments;
using System.Collections.ObjectModel;
using XtraViewScopeFormApplication.Models.Dictionaries;
using XtraViewScopeFormApplication.Models.Enums;
using XtraViewScopeFormApplication.Models.XmpTransmission;

namespace XtraViewScopeTest
{
    /// <summary>
    /// Summary description for Add2SignalAnalyserTest
    /// </summary>
    [TestClass]
    public class TestAdd2SignalAnalyser
    {
        public TestAdd2SignalAnalyser()
        {
            //
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestNibbleCreationSucessful()
        {
            Nibble nibble = null;
            for (int i = 0; i < 16; i++)
            {
                nibble = createNibble(i);
                Assert.AreEqual(nibble.BinaryValue, (BinaryValue)i, "Nibble should have a binary value of " + (BinaryValue)i);
                Assert.AreEqual(nibble.DecimalValue, i, "Nibble should have a decimal value of " + i);
            }
        }

        [TestMethod]
        public void TestNibbleCreationFailure()
        {
            Nibble nibble = null;
            for (int i = 0; i <= 16; i++)
            {
                nibble = createFailedNibble(i);
                Assert.IsTrue(nibble.TimeDeviation > 50 || nibble.TimeDeviation < -50);
            }
        }

        [TestMethod]
        public void TestAdd2PacketCreationSucessful()
        {
            Add2Packet add2Packet = new Add2Packet();
            add2Packet.Nibbles = new Collection<Nibble>();

            for (int i = 0; i < 8; i++)
            {
                add2Packet.Nibbles.Add(createAdd2Nibble(i));

            }

            Assert.AreEqual(add2Packet.Nibbles[0].DecimalValue, 14, "The first nibble's decimal value should be 14");
            Assert.AreEqual(add2Packet.Checksum, 0, "The checksum of the add2packet must be 0");
            Assert.AreEqual(add2Packet.MessageType, (MessageType) 0, "This is the first add2packet and should have a message type of " + (MessageType) 0);
            Assert.AreEqual(add2Packet.SequenceNumber, 0, "This is the first add2packet in the sequence and should have a sequence number of 0");

        }

        #region Nibble creation
        public Nibble createNibble(int nibbleNum)
        {
            Nibble nibble = new Nibble();
            nibble.PulseStartTime = PrecisionDateTime.Now;

            switch (nibbleNum)
            {
                case 0:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000915);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 1:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001051);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 2:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001188);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 3:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001325);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 4:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001461);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 5:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001598);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 6:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001735);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 7:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001872);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 8:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002008);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 9:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002145);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 10:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002282);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 11:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002418);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 12:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002555);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 13:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002692);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 14:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002829);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
                case 15:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002965);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000055);
                        break;
                    }
            }

            nibble.BinaryValue = Add2Dictionary.GetBinaryData(nibble.TotalDuration.FractionalSeconds * 1000000);

            return nibble;
        }

        public Nibble createFailedNibble(int nibbleNum)
        {
            Nibble nibble = new Nibble();
            nibble.PulseStartTime = PrecisionDateTime.Now;

            switch (nibbleNum)
            {
                case 0:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000915);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 1:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001051);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 2:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001188);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 3:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001325);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 4:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001461);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 5:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001598);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 6:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001735);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 7:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001872);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 8:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002008);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 9:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002145);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 10:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002282);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 11:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002418);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 12:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002555);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 13:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002692);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 14:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002829);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 15:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.002965);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime - new PrecisionTimeSpan(0.000010);
                        break;
                    }
                case 16:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.003076);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000010);
                        break;
                    }
            }

            return nibble;
        }

        public Nibble createAdd2Nibble(int nibbleNum)
        {
            Nibble nibble = new Nibble();
            nibble.PulseStartTime = PrecisionDateTime.Now;

            switch (nibbleNum)
            {
                case 0:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.001);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.001884);
                        break;
                    }
                case 1:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000230);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.001560);
                        break;
                    }
                case 2:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000220);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000750);
                        break;
                    }
                case 3:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000225);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000745);
                        break;
                    }
                case 4:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000225);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000745);
                        break;
                    }
                case 5:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000225);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000745);
                        break;
                    }
                case 6:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000220);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.002390);
                        break;
                    }
                case 7:
                    {
                        nibble.PulseEndTime = nibble.PulseStartTime + new PrecisionTimeSpan(0.000225);
                        nibble.NoiseStartTime = nibble.PulseEndTime;
                        nibble.NoiseEndTime = nibble.NoiseStartTime + new PrecisionTimeSpan(0.000745);
                        break;
                    }
            }

            nibble.BinaryValue = Add2Dictionary.GetBinaryData(nibble.TotalDuration.FractionalSeconds * 1000000);

            return nibble;
        }
        #endregion
    }
}
