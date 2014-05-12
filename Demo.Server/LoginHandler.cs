using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Protocol;
using Demo.Net;
using Newtonsoft.Json.Linq;

namespace Demo.Server
{
    public class LoginHandler
    {
        private IAsyncSocketServer _serverRef;
        private Login _user;
        public bool authd;

        public void HandlerDoesHisThing(IAsyncSocketServer s, Login u)
        {
            _serverRef = s;
            _user = u;
        }

        public void AuthAccept()
        {
            try
            {
                _serverRef.SendToAll(new PDU()
                {
                    MessageID = (int)EventMessageID.AuthAccept,
                    MessageType = "Event",
                    MessageDescription = "Client, your credentials have been accepted",
                    Source = "Server",
                    Data = JObject.FromObject(_user)

                }.ToJson());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ProcessLoginInfo(PDU msg)
        {
            if (_user.Username == "test" && _user.Password == "bacon"){
                _user.Auth = true;
                authd = true;
            }
            else{
                _user.Auth = false;
                authd = false;
            }
        }


        public void AuthDenied()
        {
            try
            {
                _serverRef.SendToAll(new PDU()
                {
                    MessageID = (int)EventMessageID.AuthDenied,
                    MessageType = "Event",
                    MessageDescription = "Client, Im sorry but your credentials have been rejected",
                    Source = "Server",
                    Data = JObject.FromObject(_user)

                }.ToJson());
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
