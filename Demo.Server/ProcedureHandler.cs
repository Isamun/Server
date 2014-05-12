using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Demo.Protocol;
using Demo.Net;
using Newtonsoft.Json.Linq;


namespace Demo.Server
{
    public class ProcedureHandler : ICommands, IProtocolEvents
    {
        private IAsyncSocketServer _serverRef;
        private ListOfProcedures _procedurelist;

        public ProcedureHandler(IAsyncSocketServer ServerRef)
        {
            _serverRef = ServerRef;

            _procedurelist = new ListOfProcedures(true);
            

            //_testProcedure = new Procedure(true);
        }


        public void LoadSequenceFile(string seqfile)
        {
            throw new NotImplementedException();
        }

        public void StartExecution(string seqfile)
        {
            throw new NotImplementedException();
        }

        public void ResumeExecution(string seqfile)
        {
            throw new NotImplementedException();
        }

        public void PauseExecution(string seqfile)
        {
            throw new NotImplementedException();
        }

        public void StopExecution(string seqfile)
        {
            throw new NotImplementedException();
        }

        public void SendAllProcedures()
        {
            try
            {
                _serverRef.SendToAll(new PDU()
                {
                    MessageID = (int)EventMessageID.ProceduresSent,
                    MessageType = "Event",
                    MessageDescription = "Client please, Take these procedures I have boundled in this event and display it to the operator",
                    Source = "Server",
                    Data = JObject.FromObject(_procedurelist)

                }.ToJson());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

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
