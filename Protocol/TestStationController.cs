using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Demo.Protocol
{
    public class TestStationController : IProtocolCommands, IProtocolEvents
    {

        public event ProtocolEventHandler SequenceFileLoaded;

        public event ProtocolErrorEventHandler SequenceFileLoadFailed;

        public event ProtocolEventHandler ExecutionStarted;

        public event ProtocolErrorEventHandler ExecutionStartFailed;

        public event ProtocolEventHandler ExecutionPaused;

        public event ProtocolErrorEventHandler ExecutionPauseFailed;

        public event ProtocolEventHandler ExecutionResumed;

        public event ProtocolErrorEventHandler ExecutionResumeFailed;

        public event ProtocolEventHandler ExecutionStopped;

        public event ProtocolErrorEventHandler ExecutionStopFailed;

        public event ProtocolErrorEventHandler ExecutionFailed;

        public enum CommandMessageID
        {
            LoadSequenceFile = 1501,
            StartExecution = 1502,
            StopExecution = 1503,
            PauseExecution = 1504,
            ResumeExecution = 1505
        }

        public enum EventMessageID
        {
            SequenceFileLoaded = 1301,
            ExecutionStarted = 1302,
            ExecutionStopped = 1303,
            ExecutionPaused = 1304,
            ExecutionResumed = 1305,
            TestPassed = 1306,
            TestFailed = 1307
        };

        /*
         * Load the specified sequence file and throw back Execution loaded or failed event.
         * */


        public PDU LoadSequenceFile(string seqfile)
        {
            PDU pdu = new PDU()
            {
                MessageID = (int)CommandMessageID.LoadSequenceFile,
                MessageDescription = "Server Please, load the sequencefile I specified as part of this message.",
                Data = new JObject() 
            };

            pdu.Data.SequenceFileName = seqfile;
            return pdu;
        }

        public PDU StartExecution(string seqfile)
        {
            PDU pdu = new PDU()
            {
                 MessageID = (int)CommandMessageID.StartExecution,
                 MessageDescription = "Server Please, start the sequencefile I specified as part of this message.",
                 Data = new JObject()
            };

            pdu.Data.SequenceFileName = seqfile;
            return pdu;

            
        }

        public PDU ResumeExecution(string seqfile)
        {
            PDU pdu = new PDU()
            {
                MessageID = (int)CommandMessageID.ResumeExecution,
                MessageDescription = "Server Please, resume the sequencefile I specified as part of this message.",
                Data = new JObject()
            };

            pdu.Data.SequenceFileName = seqfile;
            return pdu;
        }

        public PDU PauseExecution(string seqfile)
        {
            PDU pdu = new PDU()
            {
                MessageID = (int)CommandMessageID.PauseExecution,
                MessageDescription = "Server Please, pause the sequencefile I specified as part of this message.",
                Data = new JObject()
            };

            pdu.Data.SequenceFileName = seqfile;
            return pdu;
        }

        public PDU StopExecution(string seqfile)
        {
            PDU pdu = new PDU()
            {
                MessageID = (int)CommandMessageID.StopExecution,
                MessageDescription = "Server Please, stop the sequencefile I specified as part of this message.",
                Data = new JObject()
            };

            pdu.Data.SequenceFileName = seqfile;
            return pdu;
        }



        public void ProcessNetworkEvent(String json) { 
            
        }


    }
}
