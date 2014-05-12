using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Net
{
    public interface IAsyncSocketServer : IDisposable, IAsyncSocketEvents, IAsyncSocketServerEvents
    {

        void StartListening(String address, int port);

        bool IsConnected(int id);

        void OnClientConnect(IAsyncResult result);

        void ReceiveCallback(IAsyncResult result);

        void Send(int id, string msg, bool close);

        void SendToAll(string msg); //"broadcast"

        void Close(int id);
    }

    //could be common for listener and client.
    public interface IAsyncSocketEvents 
    {
        event MessageReceivedHandler MessageReceived;
        event MessageSubmittedHandler MessageSubmitted;

        //Todo: refacotr connect disconnect events.
    }

    public interface IAsyncSocketServerEvents {
        event ClientConnectedHandler ClientConnected;
        event ClientDisconnectedHandler ClientDisconnected;
        event ClientFailedToConnectHandler ClientFailedToConnect;
        event StartedListeningHandler StartedListening;
        event StoppedListeningHandler StoppedListening;
    }

    // delegates.
    public delegate void MessageReceivedHandler(int id, string msg);
    public delegate void MessageSubmittedHandler(int id, bool close);

    public delegate void ClientConnectedHandler(object src, int id);
    public delegate void ClientDisconnectedHandler(object src, int id);
    public delegate void ClientFailedToConnectHandler(object src, Exception e);
    public delegate void StartedListeningHandler(object src, IPEndPoint ipendpoint);
    public delegate void StoppedListeningHandler(object src, IPEndPoint ipendpoint);
}
