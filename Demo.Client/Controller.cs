using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Demo.Net;
using Demo.Protocol;

namespace Demo.Client
{
    public class Controller : IController
    {
        private IAsyncClient _ac;
        private TestStationController _tsc;
        private String _ip;
        private int _port;
        private EventProcessor _ep;
        private IViewModel _vm;
        private MainWindow _mainviewref;

        public Controller(IViewModel vm, MainWindow mainviewref)
        {
            _vm = vm;
            _mainviewref = mainviewref;
            
            //haha
            
        }


        public void Test()
        {
            mockSender m = new mockSender();
            m.Connected += _ac_Connected;

            
            // Running simulation of network events.
            m.TestSendSequenceLoadedEvent();
        }

        /*
         *  A mockup class that simulates network events for the gui.
         *  
         * 
         * 
         * */
        class mockSender : IAsyncClient
        {
            public event Demo.Net.ClientMessageReceivedHandler MessageReceived;

            /**
             *  Test Case:   When the teststation has loaded a sequence, we shall receive a Event (SequenceLoadedEvent), 
             *              and the whole execution state in the pdu.data.
             *              
             *  Assertions: Upon receiving such events, the steplist should be drawn in the gui.
             * 
             *  How does the test pass: A dude with eyes must watch the client and see that the steplist updates.
             */
            public void TestSendSequenceLoadedEvent()
            {
                if (Connected != null)
                    Connected(this);

                if (MessageReceived != null)
                    MessageReceived(this, Demo.Client.Properties.Resources.MockUpSequenceFileLoadedPDU);
            }

            public event ConnectedHandler Connected;
            public event DisconnectedHandler Disconnected;
            public event DisconnectFailedHandler DisconnectFailed;
            public event ConnectionFailedHandler ConnectionFailed;
            public event ClientMessageSubmittedHandler MessageSubmitted;

            public void StartClient(string HostnameOrIp, int port){}
            public void StartClient(){}
            public bool IsConnected()
            {
                return true;        
            }
            public void Receive(){}
            public void Send(string msg, bool close) { }
            public void Dispose() {}


            public void Close()
            {
                throw new NotImplementedException();
            }
        }



        void _ac_Connected(IAsyncClient a)
        {
            _tsc = new TestStationController(a);
            _tsc.NetworkErrorOccured += TSC_NetworkErrorOccured;
            _ep = new EventProcessor(a, _vm, _mainviewref);

            //Update model
            if(ViewModel.Instance != null)
            ViewModel.Instance.Connected = true;
            _connecting = false;
            

        }

        void TSC_NetworkErrorOccured(object source, Protocol.PDU Message, Exception e)
        {
            if (!_ac.IsConnected())
            {
                //_ac_ConnectionFailed(this, e);
            }
        }


        void _ac_ConnectionFailed(IAsyncClient a, Exception e)
        {
            //Update model
            if (ViewModel.Instance != null)
                ViewModel.Instance.Connected = false;
            System.Threading.Thread.Sleep(500);
            _connecting = false;

            Connect(_ip, _port);
        }

        public void Disconnect() {
            try
            {
                _connecting = false;
                _ac.Close();
            }
            catch (Exception e) {
                //Update model
                if (ViewModel.Instance != null)
                    ViewModel.Instance.Connected = false;
            }
        }

        
        private bool _connecting = false;

        public void Connect(string ip, int port)
        {
           
            _ip = ip;
            _port = port;
            try
            {
                _ac = new AsyncClient();
                _ac.StartClient(ip, port);

                _ac.Connected += _ac_Connected;
                _ac.ConnectionFailed += _ac_ConnectionFailed;
                _ac.Disconnected += _ac_Disconnected;
                _ac.DisconnectFailed += _ac_DisconnectFailed;
            }
            catch (Exception e)
            {
                //hum...
            }
            
        }

        void _ac_DisconnectFailed(IAsyncClient a, Exception e)
        {
            //Update model
            if (ViewModel.Instance != null)
                ViewModel.Instance.Connected = false;
        }

        void _ac_Disconnected(IAsyncClient a)
        {
            
            _ac = null;

            //Update model
            if (ViewModel.Instance != null)
                ViewModel.Instance.Connected = false;
            System.Threading.Thread.Sleep(500);
            
            Connect(_ip, _port);
        }

        public void LoadSequenceFile(string seqfile)
        {
            if (ReadyToSendCommand())
            _tsc.LoadSequenceFile(seqfile);
        }

        public void StartExecution(string seqfile)
        {

            if (ReadyToSendCommand())
            _tsc.StartExecution(seqfile);
        }

        public void ResumeExecution(string seqfile)
        {
            if (ReadyToSendCommand())
            _tsc.ResumeExecution(seqfile);
        }

        public void PauseExecution(string seqfile)
        {
            if (ReadyToSendCommand())
            _tsc.PauseExecution(seqfile);
        }

        public void StopExecution(string seqfile)
        {
            if(ReadyToSendCommand())
            _tsc.StopExecution(seqfile);
        }

        private bool ReadyToSendCommand() {
            //Is everything setup?
            if (_ac == null || _tsc == null || _ep == null)
            {
                //Update model
                if (ViewModel.Instance != null)
                    ViewModel.Instance.Connected = false;
                return false;
            }
            // Are we connected?
            if (!_ac.IsConnected())
            {
                //Update model
                if (ViewModel.Instance != null)
                    ViewModel.Instance.Connected = false;
            }

            return true;
        }

        public event NetworkErrorHandler NetworkErrorOccured;





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
