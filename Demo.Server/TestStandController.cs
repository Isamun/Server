using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Net;
using Demo.Protocol;

namespace Demo.Server
{
    public class TestStandController
    {

        private ITestStandWrapper _testStand;
        private IAsyncSocketServer _serverSocket;
        private ProcedureHandler _procedureHandler;
        private LoginHandler _loginHandler;

        public TestStandController(ITestStandWrapper teststandwrapper, IAsyncSocketServer serversocket) 
        {
            _testStand = teststandwrapper;

            _procedureHandler = new ProcedureHandler(serversocket);

            _serverSocket = serversocket;

            // Listening for network traffic from all clients:
            _serverSocket.MessageReceived += _serverSocket_MessageReceived;

        }

        /**
         *  TestStation Command Message Processor.
         *  
         * This thingy (the function below) Eats Commands for dinner lunch and breakfast. 
         * 
         * */
        

        void _serverSocket_MessageReceived(int id, string msg)
        {
            try 
            {
                PDU temp = new PDU(msg);

                switch ((CommandMessageID)temp.MessageID) 
                {
                    case CommandMessageID.LoginAttempt:
                        try
                        {
                            _loginHandler.HandlerDoesHisThing(_serverSocket, temp.Data.ToObject(typeof(Login)));
                            _loginHandler.ProcessLoginInfo(temp);
                            if (_loginHandler.authd) { _procedureHandler.SendAllProcedures(); }
                        }
                        catch (Exception e) { Console.WriteLine(e.ToString()); }
                        
                        break;
                    case CommandMessageID.SendAllProcedures:
                        _procedureHandler.SendAllProcedures();
                        break;
                    case CommandMessageID.StartExecution:        
                        _testStand.StartExecution(Protocol.Utilitites.getSequenceFileName(temp));
                        break;
                    case CommandMessageID.PauseExecution:
                        _testStand.PauseExecution(Protocol.Utilitites.getSequenceFileName(temp));
                        break;
                    case CommandMessageID.ResumeExecution:
                        _testStand.ResumeExecution(Protocol.Utilitites.getSequenceFileName(temp));
                        break;
                    case CommandMessageID.StopExecution:
                        _testStand.StopExecution(Protocol.Utilitites.getSequenceFileName(temp));
                        break;
                    case CommandMessageID.LoadSequenceFile:
                        _testStand.LoadSequenceFile(Protocol.Utilitites.getSequenceFileName(temp));
                        break;
                    default:
                        //Fire error pdu back to client.
                        break;
                }

            }
            catch (Exception e) { 
            
                //Todo handle invalid pdu received.
                Console.WriteLine(e.Message);
                
                // This could fire an "Unknown PDU thingy error.
            }
        }

    }
}
