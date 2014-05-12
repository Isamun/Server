using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Dynamic;
using System.Threading.Tasks;
using System.Text;
using Demo.Protocol;
using System.IO;
using Newtonsoft.Json.Linq;
using Demo.Client;


namespace Demo.TestProtocol
{
    [TestClass]
    public class TestPDUGeneration
    {
        [TestMethod]
        public void TestCreatingAEventPDU()
        {
            try
            {
                PDU execution_started = new PDU()
                {
                    MessageID = 1302,
                    MessageDescription = "Test Stand Engine has started Execution",
                    Data = { }
                };

                //Fields should match
                Assert.AreEqual(execution_started.MessageID, 1302);
                Assert.AreEqual(execution_started.MessageDescription, "Test Stand Engine has started Execution");

                Assert.AreNotEqual(execution_started.HASH, null);
                //Json serialization should match,.
                
            }


            catch (Exception e)
            {
                Assert.Fail();
            }
            
        }

        [TestMethod]
        public void TestComparingPDUs()
        {
            PDU execution_started = new PDU()
            {
                MessageID = 13,
                MessageDescription = "Test Stand Engine has started Execution",
                Data = { }
            };

            PDU another = new PDU()
            {
                MessageID = 12,
                MessageDescription = "Test Stand Engine has started Execution",
                Data = { }
            };

            PDU third = new PDU()
            {
                MessageID = 13,
                MessageDescription = "Test Stand Engine has started Execution",
                Data = { }
            };

            Assert.AreEqual(execution_started, third, "Equals method failed");
            Assert.AreNotEqual(execution_started, another);

            String kakeluren = "hehe";
            Assert.AreNotEqual(third, kakeluren, "Ehm, a PDU is not a string");

            Assert.IsFalse(execution_started == third, "Compare with == operator did not match, reference check matched value equality on two different objects.");
            Assert.IsTrue(execution_started == execution_started, "Compare with == did not match same object");

            PDU refToFirst = execution_started;
            Assert.IsTrue(execution_started == refToFirst, " Reference comparison did not match ");

            Assert.IsFalse(execution_started == another);
            


        }

        [TestMethod]
        public void TestInstantiatePDUFromJsonString()
        {

            /*
             * PDU pdu = new PDU(File.ReadAllText("testdata/jsonPdu.json"));
             *
             * Assert.AreEqual(EventMessageID.ExecutionStarted,
             *     (EventMessageID)pdu.MessageID,
             *     "Not correct Message ID");
             *
             *pdu.ToString();
             **/


            Execution recievedExecutionState;

            PDU RecievedSequenceLoadedEvent = new PDU(File.ReadAllText("testdata/out.json"));

            recievedExecutionState = RecievedSequenceLoadedEvent.Data.ToObject(typeof(Execution));

            Assert.IsInstanceOfType(recievedExecutionState, typeof(Execution), "nope");
        }
        

        [TestMethod]
        public void TestGenerateJsonString() {

            String seqfile = "loltest.seq";
            PDU pdu = new PDU()
            {
                MessageID = (int)CommandMessageID.ResumeExecution,
                MessageDescription = "Server Please, resume the sequencefile I specified as part of this message.",
                Data = new JObject()
            };

            pdu.Data.SequenceFileName = seqfile;

            File.WriteAllText("ResumeExecutionCommand.json", pdu.ToString());
        }

    }
}
