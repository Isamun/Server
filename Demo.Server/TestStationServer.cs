using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Protocol;
using Demo.Utilities;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using Demo.Net;
using Microsoft.CSharp.RuntimeBinder;
using System.Threading;

namespace Demo.Server
{
    class TestStationServer
    {

        private static bool isclosing = false;
        //The socket logics.
        static IAsyncSocketServer server;
        static ITestStandWrapper teststand; 
        static TestStandController controller;

        static void Main(string[] args)
        {

           
            // Loading config.
            //
            //  Tries to find and load a config file named
            //  server.config.json. 
            //  If it fails to find it, we have provided a 
            //  Default json string for config in the project resources.
            //

       

            dynamic config = DynamicConfig.GetConfig(
               "server.config.json",
               Properties.Resources.server_default_config);

            

            // Setting up server components.

            try
            {

                //The socket logics.
                server = new AsyncSocketServer();
                teststand = new TestStandWrapper(server);
                controller = new TestStandController(teststand, server);

                //Starting a test stand motorus.



                server.ClientConnected += server_ClientConnected;
                server.ClientDisconnected += server_ClientDisconnected;
                server.StartedListening += on_server_StartedListening;
                server.MessageReceived += server_MessageReceived;

               
                try
                {
                    Console.WriteLine("Attempting to listen on {0}:{1}", config.server.ip, config.server.port);
                    server.StartListening((string)config.server.ip, (int)config.server.port);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            catch (RuntimeBinderException re) {
                Console.WriteLine(" Oh oh:" + re.Message);
            }


            //To catch output from server
            while (true) ;
        }

        static void server_MessageReceived(int id, string msg)
        {
            Console.WriteLine("Got Message from client with ID: {0}",  id);
            Console.WriteLine(msg);
        }

        static void server_ClientDisconnected(object src, int id)
        {
            Console.WriteLine("Client Disconnected ID: {0}", id);
        }

        static void on_server_StartedListening(object src, System.Net.IPEndPoint ipendpoint)
        {
            Console.WriteLine("Started listening on : {0}", ipendpoint);
        }

        static void server_ClientConnected(object src, int id)
        {
            Console.WriteLine("Client Connected ID: {0}", id);
        }




 
    }
}
