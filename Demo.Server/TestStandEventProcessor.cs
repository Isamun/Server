using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Protocol;
using Newtonsoft.Json.Linq;

using Demo.Net;

namespace Demo.Server
{
    

    public class TestStandEventProcessor
    {
        private ITestStandWrapper _teststandwrapper;
        private IAsyncSocketServer _server;

        public TestStandEventProcessor(ITestStandWrapper ts, IAsyncSocketServer server) {
            _teststandwrapper = ts;
            _server = server;

            ts.ExecutionStarted += ts_ExecutionStarted;

        }

        void ts_ExecutionStarted(object source, string Message)
        {
            
        }

        void send_event() {


   
        }   

        public static PDU CreateEventPDU(EventMessageID id, Execution state) {

            try
            {
                PDU temp = new PDU
                {
                    MessageID = (int)id,
                    MessageType = "Event",
                    MessageDescription = "Client please, Take this state I have boundled in this event and display it to the operator",
                    Source = "TestStand",
                    Data = JObject.FromObject(
                         state
                    )
                };

                return temp;
            }
            catch (Exception e) {
                throw e;
            }
        }




    }
}
