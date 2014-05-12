using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Net
{
    public interface IAsyncClient : IDisposable
    {
        event ConnectedHandler Connected;

        event DisconnectedHandler Disconnected;

        event DisconnectFailedHandler DisconnectFailed;

        event ConnectionFailedHandler ConnectionFailed;

        event ClientMessageReceivedHandler MessageReceived;

        event ClientMessageSubmittedHandler MessageSubmitted;

        void StartClient(String HostnameOrIp, int port);

        void StartClient();

        bool IsConnected();

        void Receive();

        void Send(string msg, bool close);

        void Close();
    }

    public delegate void ConnectedHandler(IAsyncClient a);
    public delegate void ClientMessageReceivedHandler(IAsyncClient a, string msg);
    public delegate void ClientMessageSubmittedHandler(IAsyncClient a, bool close);
    public delegate void ConnectionFailedHandler(IAsyncClient a, Exception e);
    public delegate void DisconnectedHandler(IAsyncClient a);
    public delegate void DisconnectFailedHandler(IAsyncClient a, Exception e);
}
