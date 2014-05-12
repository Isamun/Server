using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;

namespace Demo.Net
{



    public sealed class AsyncSocketServer : IAsyncSocketServer
    {
        

        private const ushort Limit = 250;

        private static readonly IAsyncSocketServer instance = new AsyncSocketServer();

        private readonly ManualResetEvent mre = new ManualResetEvent(false);
        private readonly IDictionary<int, IStateObject> clients = new Dictionary<int, IStateObject>();

        public event MessageReceivedHandler MessageReceived;

        public event MessageSubmittedHandler MessageSubmitted;

        public AsyncSocketServer()
        {
        }

        public static IAsyncSocketServer Instance
        {
            get
            {
                return instance;
            }
        }

        /* Starts the AsyncSocketListener */
        public void StartListening(String address, int port)
        {
            
            var ip = IPAddress.Parse(address);
            var endpoint = new IPEndPoint(ip, port);

            try
            {
                using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    listener.Bind(endpoint);
                    listener.Listen(Limit);
                    //Eventfulness
                    if(StartedListening != null)
                        StartedListening(this, (IPEndPoint)endpoint);

                    while (true)
                    {
                        this.mre.Reset();
                        listener.BeginAccept(this.OnClientConnect, listener);
                        this.mre.WaitOne();
                    }
                }
            }
            catch (SocketException e)
            {
                throw new Exception("Server could not listen: " + e.Message);
            }
        }

        /* Gets a socket from the clients dictionary by his Id. */
        private IStateObject GetClient(int id)
        {
            IStateObject state;

            return this.clients.TryGetValue(id, out state) ? state : null;
        }

        /* Checks if the socket is connected. */
        public bool IsConnected(int id)
        {
            var state = this.GetClient(id);
            
            //The client might have been disconnected here dudes.
            return state != null && !(state.Listener.Poll(1000, SelectMode.SelectRead) && state.Listener.Available == 0);
        }

        /* Add a socket to the clients dictionary. Lock clients temporary to handle multiple access.
         * ReceiveCallback raise a event, after the message receive complete. */
        #region Receive data
        public void OnClientConnect(IAsyncResult result)
        {
            this.mre.Set();

            try
            {
                IStateObject state;

                lock (this.clients)
                {
                    var id = !this.clients.Any() ? 1 : this.clients.Keys.Max() + 1;

                    state = new StateObject(((Socket)result.AsyncState).EndAccept(result), id);
                    this.clients.Add(id, state);

                    if (ClientConnected != null) {
                        ClientConnected(this, id); //as.getClient(id) --> state
                    }
                }

                state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, this.ReceiveCallback, state);
            }
            catch (SocketException e)
            {
                if (ClientFailedToConnect != null)
                    ClientFailedToConnect(this, e);
            }
        }

        public void ReceiveCallback(IAsyncResult result)
        {
            var state = (IStateObject)result.AsyncState;

            try
            {
                var receive = state.Listener.EndReceive(result);

                if (receive > 0)
                {   // Got a fragment
                    state.Append(Encoding.UTF8.GetString(state.Buffer, 0, receive));

                    if (receive == state.BufferSize)
                    {
                        // need the next fragment too
                        state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, this.ReceiveCallback, state);
                    }
                    else
                    {
                        // Got all the fragments. (packets)
                        var messageReceived = this.MessageReceived;

                        if (messageReceived != null)
                        {
                            messageReceived(state.Id, state.Text);
                        }

                        state.Reset();
                        // Lets receive again
                        state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, this.ReceiveCallback, state);
                    }
                }
                else { 
                    // ended a receive with 0 bytes.
                    // This indicates a disconnect
                    if (!IsConnected(state.Id))
                        Close(state.Id);
                }
            }
            catch (SocketException e )
            {
                /**
                 *  Check if the client is still connected
                 *  Disconnect it if not
                 **/
                if (!IsConnected(state.Id)) {
                    this.Close(state.Id);     
                }
            }
        }
        #endregion

        /* Send(int id, String msg, bool close) use bool to close the connection after the message sent. */
        #region Send data
        public void Send(int id, string msg, bool close)
        {
            var state = this.GetClient(id);

            if (state == null)
            {
                throw new Exception("Client does not exist.");
            }

            if (!this.IsConnected(state.Id))
            {
                this.Close(state.Id);
                throw new Exception("Destination socket is not connected.");
            }

            try
            {
                var send = Encoding.UTF8.GetBytes(msg);

                state.Close = close;
                state.Listener.BeginSend(send, 0, send.Length, SocketFlags.None, this.SendCallback, state);
            }
            catch (SocketException)
            {
                // TODO:
            }
            catch (ArgumentException)
            {
                // TODO:
            }
        }

        private void SendCallback(IAsyncResult result)
        {
            var state = (IStateObject)result.AsyncState;

            try
            {
                state.Listener.EndSend(result);
            }
            catch (SocketException)
            {
                // TODO:
            }
            catch (ObjectDisposedException)
            {
                // TODO:
            }
            finally
            {
                var messageSubmitted = this.MessageSubmitted;

                if (messageSubmitted != null)
                {
                    messageSubmitted(state.Id, state.Close);
                }
            }
        }
        #endregion

        public void Close(int id)
        {
            var state = this.GetClient(id);

            if (state == null)
            {
                throw new Exception("Client does not exist.");
            }

            try
            {
                state.Listener.Shutdown(SocketShutdown.Both);
                state.Listener.Close();
            }
            catch (SocketException)
            {
                // TODO:
            }
            finally
            {
                lock (this.clients)
                {
                    this.clients.Remove(state.Id);

                    if (ClientDisconnected != null)
                        ClientDisconnected(this, state.Id);
                }
            }
        }

        public void Dispose()
        {
            foreach (var id in this.clients.Keys)
            {
                this.Close(id);
            }

            this.mre.Dispose();
        }

        public event ClientConnectedHandler ClientConnected;

        public event ClientDisconnectedHandler ClientDisconnected;


        public event StartedListeningHandler StartedListening;

        public event StoppedListeningHandler StoppedListening;


        public event ClientFailedToConnectHandler ClientFailedToConnect;





        public void SendToAll(string msg)
        {
            foreach (var id in this.clients.Keys)
            {
                try
                {
                    Send(id, msg, false);
                }
                catch (SocketException e)
                {
                    //TODO: fail event
                }
                catch (Exception e) 
                {
                    //TODO: fail event
                }
            }
            
        }
    }
}