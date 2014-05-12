using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Demo.Net
{
   

    public sealed class AsyncClient : IAsyncClient
    {
        private Socket listener;
        private bool close;

        private readonly ManualResetEvent connected = new ManualResetEvent(false);
        private readonly ManualResetEvent sent = new ManualResetEvent(false);
        private readonly ManualResetEvent received = new ManualResetEvent(false);

        public event ConnectedHandler Connected;

        public event ConnectionFailedHandler ConnectionFailed;

        public event ClientMessageReceivedHandler MessageReceived;

        public event ClientMessageSubmittedHandler MessageSubmitted;
        public event DisconnectedHandler Disconnected;

        public event DisconnectFailedHandler DisconnectFailed;

        public void StartClient(String HostnameOrIp="localhost", int port=1337 )
        {
            var ip = getFIP(HostnameOrIp); ;
            var endpoint = new IPEndPoint(ip, port);

            try
            {
                this.listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.listener.BeginConnect(endpoint, this.OnConnectCallback, this.listener); 
            }
            catch (SocketException e)
            {
                if (ConnectionFailed != null)
                    ConnectionFailed(this, e);
            }
        }

        public void StartClient()
        {
            StartClient("localhost", 1337);
        }

        public static IPAddress getFIP(String HostnameOrIP)
        {

            //Get for F. a ip address from hostname or ip string.
            try
            {
                // Establish the remote endpoint for the socket.
                IPAddress ipa = IPAddress.Parse(HostnameOrIP);
                return ipa;
            }
            catch (Exception e)
            {
                // This will throw exception if it fails.
                return IPAddress.Parse(HostnameOrIP);
            }
        }

        public bool IsConnected()
        {
            try
            {
                if (!(this.listener.Poll(1000, SelectMode.SelectRead) && this.listener.Available == 0))
                {
                    //connected;
                    return true;
                }
                else {
                    if (Disconnected != null)
                        Disconnected(this);

                    return false;
                }
            }
            catch (SocketException se)
            {
                if (Disconnected != null)
                    Disconnected(this);
                return false;
            }
            catch (ObjectDisposedException ode)
            {
                if (Disconnected != null)
                    Disconnected(this);
                return false;
            }
        }

        private void OnConnectCallback(IAsyncResult result)
        {
            var server = (Socket)result.AsyncState;
            
            try
            {
                server.EndConnect(result);

                Receive();

                //event
                var connectedHandler = this.Connected;
                if (connectedHandler != null)
                {
                    connectedHandler(this);
                }
            }
            catch (SocketException e)
            {
                if (ConnectionFailed != null)
                    ConnectionFailed(this, e);
            }
        }

        #region Receive data
        public void Receive()
        {
            try
            {
                if (!this.IsConnected())
                    Close();

                var state = new StateObject(this.listener);

                state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, this.ReceiveCallback, state);
            }
            catch (SocketException se)
            {
                if (!this.IsConnected())
                    Close();
            }
            catch (Exception e) 
            {
                if (!this.IsConnected())
                    Close();
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {

            var state = (IStateObject)result.AsyncState;
            try
            {
                var receive = state.Listener.EndReceive(result);

                if (receive > 0)
                {
                    state.Append(Encoding.UTF8.GetString(state.Buffer, 0, receive));


                    if (receive == state.BufferSize)
                    {
                        state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, this.ReceiveCallback, state);
                    }
                    else
                    {
                        var messageReceived = this.MessageReceived;

                        if (messageReceived != null)
                        {
                            try
                            {
                                messageReceived(this, state.Text);
                            }
                            catch (Exception e)
                            {
                                //Receive error. SHould still listen for more.
                                // THis socket is fighting hard.
                                // Taking json bullets
                                // for lunch.

                            }
                        }

                        state.Reset();

                        Receive();
                    }
                }
                else
                {
                    if (!IsConnected())
                        Close();
                }
            }
            catch (ObjectDisposedException ode) {
                if (!this.IsConnected())
                    Close();
            }
            catch (SocketException e)
            {
                if (!this.IsConnected())
                    Close();
            }
            catch (Exception e)
            {

                if (!IsConnected())
                    Close();
            }
        }
        #endregion

        #region Send data
        public void Send(string msg, bool close)
        {
            try
            {

                
                if (!this.IsConnected())
                {
                    Close();
                }

                var response = Encoding.UTF8.GetBytes(msg);

                this.close = close;
                this.listener.BeginSend(response, 0, response.Length, SocketFlags.None, this.SendCallback, this.listener);

            }
            catch (ObjectDisposedException ode)
            {
                if (!this.IsConnected())
                    Close();
            }
            catch (SocketException e)
            {
                if (!this.IsConnected())
                    Close();
            }
            catch (Exception e)
            {

                if (!IsConnected())
                    Close();
            }
        }

        private void SendCallback(IAsyncResult result)
        {
            try
            {
                var resceiver = (Socket)result.AsyncState;

                resceiver.EndSend(result);


            }
            catch (SocketException)
            {
                if (!IsConnected())
                    Close();
            }
            catch (ObjectDisposedException)
            {
                if (!IsConnected())
                    Close();
            }
            catch (Exception e) {
                if (!IsConnected())
                    Close();
            }

            var messageSubmitted = this.MessageSubmitted;

            if (messageSubmitted != null)
            {
                messageSubmitted(this, this.close);
            }

            this.sent.Set();
        }
        #endregion

        public void Close()
        {
            try
            {
                if (!this.IsConnected())
                {
                    return;
                }

                this.listener.Shutdown(SocketShutdown.Both);
                this.listener.Close();

                //Todo: Make event that says disconnected.
                if (Disconnected != null)
                    Disconnected(this);
            }
            catch (SocketException e)
            {
                //hack
                if (Disconnected != null)
                    Disconnected(this);
                if (DisconnectFailed != null)
                    DisconnectFailed(this, e);
            }
            catch (ObjectDisposedException ode)
            {
                if (Disconnected != null)
                    Disconnected(this);
            }
            catch (Exception e) 
            {
                if (Disconnected != null)
                    Disconnected(this);
            }
        }

        public void Dispose()
        {
            this.connected.Dispose();
            this.sent.Dispose();
            this.received.Dispose();
            this.Close();
        }


      
    }
}