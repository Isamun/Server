/* ------------------Demo.Client EventProcessor---------------
 * This code is meant to listen for JSON packets representing 
 * events, recieved from the Asynchronous TCP client from the
 * test station server. It relies on the fact that the messages
 * recieved are serialized JSON strings that can be casted to
 * a PDU type. 
 * 
 * Depending on the message it will do various operation on the
 * viewmodel.
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Net;
using Demo.Protocol;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows;

namespace Demo.Client
{
    public class EventProcessor
    {
        private IAsyncClient _asyncClientRef;
        private MainWindow _Mainviewref;
        private IViewModel _iVewModelRef;
        private delegate void oneatgthingie(PDU temp);
        
        public EventProcessor(IAsyncClient refe, IViewModel IVMref, MainWindow view)
        {
            _asyncClientRef = refe;
            _asyncClientRef.MessageReceived += _acRef_MessageReceived;
            _iVewModelRef = IVMref;
            _Mainviewref = view;
        }

        public void TestMessageReceived(IAsyncClient a, string msg) {
            _acRef_MessageReceived(a, msg);
        }


        //rm
        void dummy(IAsyncClient a, string msg) {
            MessageBox.Show(msg);
        }

        void _acRef_MessageReceived(IAsyncClient a, string msg)
        {
            try
            {
                PDU temp = new PDU(msg);

                _Mainviewref.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new oneatgthingie(UpdateModel), temp); 
                
            }
            catch (Exception e)
            {
                throw e;
            }



        }
        public void UpdateModel(PDU temp)
        {
            
            try
            {
                // Get execution from PDU
                Execution TempExecution = temp.Data.ToObject(typeof(Execution));
                //Set the state into viewmodel
                
                
                _iVewModelRef.CurrentExecution.State = TempExecution.State;
                //Fetch the steplist
                ObservableCollection<Step> templist = (ObservableCollection<Step>)TempExecution.CurrentSequence.StepList;

                if(TempExecution.State == Execution.ExecutionStates.FINISHED){
                    _iVewModelRef.CurrentExecution.State = TempExecution.State;
                }
                //update the steplist
                else if (templist != null)
                {
                    _iVewModelRef.CurrentExecution.CurrentSequence.StepList.Clear();
                    foreach (Step step in templist)
                    {
                        _iVewModelRef.CurrentExecution.CurrentSequence.StepList.Add(step);
                    }
                }
            }
            
            catch (Exception e)
            {
                throw new Exception("ServerEvent event recieved. \n Something went wrong tring to cast it to usable type. See PDU constructor for refrence \n Here's the thingie: " + e);
            }
        }
    }

    
}
