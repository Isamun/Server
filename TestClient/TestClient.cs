using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Demo.Protocol;
using Demo.Client;
using Demo.Net;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace TestClient
{

    

    [TestClass]
    public class TestClientSideProtocol
    {

        public String AssertJsonStrKakePDU;
    
        private IAsyncClient _mockAsyncClient;
        private TestStationController _setup() {
            _mockAsyncClient = new MockAsyncClient();

            return new TestStationController(_mockAsyncClient);
        }

        [TestMethod]
        public void TestLoadSequenceFile()
        {
            TestStationController papi = _setup();
            String seqFile = "lolseq.seq";
            papi.LoadSequenceFile(seqFile);

            PDU kaki = ((MockAsyncClient)_mockAsyncClient).Assertkake;
            try
            {
                Assert.AreEqual(
                    "Server Please, load the sequencefile I specified as part of this message.", 
                    kaki.MessageDescription, 
                    "Message description was unexpected!");
                Assert.AreEqual(
                    (int)CommandMessageID.LoadSequenceFile, 
                    kaki.MessageID, 
                    "MessageID was waaaaaay off");
                Assert.AreEqual(seqFile, (string)kaki.Data.SequenceFileName, 
                    "Rework on data assignement needed!");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void TestStartExecution()
        {
            TestStationController papi = _setup();
            String seqFile = "lolseq.seq";
            papi.StartExecution(seqFile);
            PDU kaki = ((MockAsyncClient)_mockAsyncClient).Assertkake;
            try
            {
                Assert.AreEqual(
                    "Server Please, start the sequencefile I specified as part of this message.",
                    kaki.MessageDescription,
                    "Message description was unexpected!");
                Assert.AreEqual(
                    (int)CommandMessageID.StartExecution,
                    kaki.MessageID,
                    "MessageID was waaaaaay off");
                Assert.AreEqual(seqFile, (string)kaki.Data.SequenceFileName,
                    "Rework on data assignement needed!");


                /**
                 * 
                 *  Depending on the ussage of this class, maybe a event should
                 *  be fired now?
                 *  
                 * If so, it could be picked up, and the json could get checked
                 * Damn that date.
                 * 
                 * */



            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void TestStopExecution()
        {
            TestStationController papi = _setup();
            String seqFile = "lolseq.seq";
            papi.StopExecution(seqFile);
            PDU kaki = ((MockAsyncClient)_mockAsyncClient).Assertkake;
            

            try
            {
                Assert.AreEqual(
                    "Server Please, stop the sequencefile I specified as part of this message.",
                    kaki.MessageDescription,
                    "Message description was unexpected!");
                Assert.AreEqual(
                    (int)CommandMessageID.StopExecution,
                    kaki.MessageID,
                    "MessageID was waaaaaay off");
                Assert.AreEqual(seqFile, (string)kaki.Data.SequenceFileName,
                    "Rework on data assignement needed!");


                /**
                 * 
                 *  Depending on the ussage of this class, maybe a event should
                 *  be fired now?
                 *  
                 * If so, it could be picked up, and the json could get checked
                 * Damn that date.
                 * 
                 * */



            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [TestMethod]
        public void TestPauseExecution()
        {
            TestStationController papi = _setup();
            String seqFile = "lolseq.seq";
            papi.PauseExecution(seqFile);
            PDU kaki = ((MockAsyncClient)_mockAsyncClient).Assertkake;
            

            try
            {
                Assert.AreEqual(
                    "Server Please, pause the sequencefile I specified as part of this message.",
                    kaki.MessageDescription,
                    "Message description was unexpected!");
                Assert.AreEqual(
                    (int)CommandMessageID.PauseExecution,
                    kaki.MessageID,
                    "MessageID was waaaaaay off");
                Assert.AreEqual(seqFile, (string)kaki.Data.SequenceFileName,
                    "Rework on data assignement needed!");


                /**
                 * 
                 *  Depending on the ussage of this class, maybe a event should
                 *  be fired now?
                 *  
                 * If so, it could be picked up, and the json could get checked
                 * Damn that date.
                 * 
                 * */



            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }


        [TestMethod]
        public void TestResumeExecution()
        {
            TestStationController papi = _setup();
            String seqFile = "lolseq.seq";
            papi.ResumeExecution(seqFile);
            PDU kaki = ((MockAsyncClient)_mockAsyncClient).Assertkake;

            try
            {
                Assert.AreEqual(
                    "Server Please, resume the sequencefile I specified as part of this message.",
                    kaki.MessageDescription,
                    "Message description was unexpected!");
                Assert.AreEqual(
                    (int)CommandMessageID.ResumeExecution,
                    kaki.MessageID,
                    "MessageID was waaaaaay off");
                Assert.AreEqual(seqFile, (string)kaki.Data.SequenceFileName,
                    "Rework on data assignement needed!");


                /**
                 * 
                 *  Depending on the ussage of this class, maybe a event should
                 *  be fired now?
                 *  
                 * If so, it could be picked up, and the json could get checked
                 * Damn that date.
                 * 
                 * */



            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        class MockAsyncClient : IAsyncClient
        {
            public PDU Assertkake { get; set; }

            public event ConnectedHandler Connected;

            public event ConnectionFailedHandler ConnectionFailed;

            public event ClientMessageReceivedHandler MessageReceived;

            public event ClientMessageSubmittedHandler MessageSubmitted;

            public void StartClient(string HostnameOrIp, int port)
            {
                throw new NotImplementedException();
            }

            public void StartClient()
            {
                throw new NotImplementedException();
            }

            public bool IsConnected()
            {
                throw new NotImplementedException();
            }

            public void Receive()
            {
                throw new NotImplementedException();
            }

            public void Send(string msg, bool close)
            {
                this.Assertkake = new PDU(msg);
                
                
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }


            public event DisconnectedHandler Disconnected;

            public event DisconnectFailedHandler DisconnectFailed;


            public void Close()
            {
                throw new NotImplementedException();
            }
        }

        public PDU MockSequenceFileLoadedEventThingie()
        {

            PDU returneringsKake = new PDU(TestClient.Properties.Resources.MockSequenceFileLoaded);
            return returneringsKake;
        }



    }
}
