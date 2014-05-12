using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Demo.Protocol;
using Demo.Net;

namespace Demo.Client
{
    public class TestStationController : ICommands
    {
        private IAsyncClient _socketRef;

        public TestStationController(IAsyncClient rf)
        {
            _socketRef = rf;
        }


        public event NetworkErrorHandler NetworkErrorOccured;



        /*
         * Load the specified sequence file and throw back Execution loaded or failed event.
         * */



        public void LoadSequenceFile(string seqfile)
        {
            PDU pdu = new PDU()
            {
                MessageID = (int)CommandMessageID.LoadSequenceFile,
                MessageDescription = "Server Please, load the sequencefile I specified as part of this message.",
                MessageType = "Command",
                Source = "Demo.Client",
                Data = new JObject()
            };

            pdu.Data.SequenceFileName = seqfile;
            try
            {
                _socketRef.Send(pdu.ToJson(), false);
            }
            catch (Exception e)
            {
                NetworkErrorOccured(this, pdu, e);
            }
        }

        public void StartExecution(string seqfile)
        {
            PDU pdu = new PDU()
            {
                MessageID = (int)CommandMessageID.StartExecution,
                MessageDescription = "Server Please, start the sequencefile I specified as part of this message.",
                MessageType = "Command",
                Source = "Demo.Client",
                Data = new JObject()
            };

            pdu.Data.SequenceFileName = seqfile;
            try
            {
                _socketRef.Send(pdu.ToJson(), false);
            }
            catch (Exception e)
            {
                NetworkErrorOccured(this, pdu, e);
            }
        }

        public void ResumeExecution(string seqfile)
        {
            PDU pdu = new PDU()
            {
                MessageID = (int)CommandMessageID.ResumeExecution,
                MessageDescription = "Server Please, resume the sequencefile I specified as part of this message.",
                MessageType = "Command",
                Source = "Demo.Client",
                Data = new JObject()
            };

            pdu.Data.SequenceFileName = seqfile;

            try
            {
                _socketRef.Send(pdu.ToJson(), false);
            }
            catch (Exception e)
            {
                NetworkErrorOccured(this, pdu, e);
            }
        }

        public void PauseExecution(string seqfile)
        {
            PDU pdu = new PDU()
            {
                MessageID = (int)CommandMessageID.PauseExecution,
                MessageDescription = "Server Please, pause the sequencefile I specified as part of this message.",
                MessageType = "Command",
                Source = "Demo.Client",
                Data = new JObject()
            };

            pdu.Data.SequenceFileName = seqfile;
            try
            {
                _socketRef.Send(pdu.ToJson(), false);
            }
            catch (Exception e)
            {
                NetworkErrorOccured(this, pdu, e);
            }
        }

        public void StopExecution(string seqfile)
        {
            PDU pdu = new PDU()
            {
                MessageID = (int)CommandMessageID.StopExecution,
                MessageDescription = "Server Please, stop the sequencefile I specified as part of this message.",
                MessageType = "Command",
                Source = "Demo.Client",
                Data = new JObject()
            };

            pdu.Data.SequenceFileName = seqfile;
            try
            {
                _socketRef.Send(pdu.ToJson(), false);
            }
            catch (Exception e)
            {
                NetworkErrorOccured(this, pdu, e);
            }
        }


        public void SendAllProcedures()
        {
            throw new NotImplementedException();
        }

        public void AuthAccept()
        {
            throw new NotImplementedException();
        }

        public void AuthDenied()
        {
            throw new NotImplementedException();
        }
    }
}
