using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
// Add Reference Natinoal Instruments API Interop Assembly 
using NationalInstruments.TestStand.Interop.API;
using Demo.Net;
using System.Collections.ObjectModel;


namespace Demo.Server
{
    public class TestStandWrapper : ITestStandWrapper
    {

        public Engine engine;
        protected SequenceFile f;
        private SequenceFile modelSequenceFile;
        protected Execution exefil;
        protected BackgroundWorker bgw;
        
        private Queue<Action> commandQueue= new Queue<Action>();

        private IAsyncSocketServer _server;
        private System.Threading.Thread exeThread = null;
        

        public TestStandWrapper(IAsyncSocketServer server) {
            _server = server;
        }


        public void init()
        {
            // Houston, Initialize engine
            this.engine = new NationalInstruments.TestStand.Interop.API.Engine();
            really_register_callback();
        }

        /**
         *  Registering callback on engine UIMessage events.
         **/
        public void really_register_callback()
        {
            //without this, the engine will freeze up, and
            // The sequence file will never execute.
            engine.UIMessagePollingEnabled = false;
            //Adding callback to events
            engine.UIMessageEvent += engine_UIMessageEvent;

          
        }




        /**
       *  Commands Interface 
       *  
       * */

        public void LoadSequenceFile(string seqfile)
        {
            //Resetting f
            f = null;

            try
            {
                init();
                String strModelDescription;
                bool ok = true;
                SequenceFile seqFileModel; // for holding the  process model sequence
                String modelAbsPath; // for holding the abs path to the processmodel sequence file
                String strStationModelSequenceFile; // station.

                String strSeqFilePath = Path.GetFullPath(seqfile);


                // Houston, Initialize countdown sequence
                f = engine.GetSequenceFileEx(
                    strSeqFilePath,
                    0, TypeConflictHandlerTypes.
                    ConflictHandler_Error);


                // Apparently, this is required. VIL.
                f.ModelPath = strSeqFilePath;
                f.ModelOption = ModelOptions.ModelOption_RequireSpecificModel;

                // Getting model
                seqFileModel = f.GetModelSequenceFile(out strModelDescription);
                if (seqFileModel == null)
                    if (SequenceFileLoadFailed != null)
                        SequenceFileLoadFailed(this, "Could not load the process model sequence file: Single Pass");

                modelAbsPath = seqFileModel.GetModelAbsolutePath(out ok);
                modelSequenceFile = engine.GetStationModelSequenceFile(out strStationModelSequenceFile);
                f.ModelPath = strStationModelSequenceFile;



                if (SequenceFileLoaded != null)
                    SequenceFileLoaded(this, strSeqFilePath);

                _server.SendToAll(
                    TestStandEventProcessor.CreateEventPDU(
                        Protocol.EventMessageID.SequenceFileLoaded, 
                        GetCurrentExecutionState(f.GetSequence(0))
                    ).ToJson()
                );


            }
            catch (Exception e)
            {
                //TODO fire SequenceLoadFailed PDU/EVent
                throw e;
                if (SequenceFileLoadFailed != null)
                    SequenceFileLoadFailed(this, seqfile);
            }
        }

        /***********************************************************
         *  Start executing a sequence file.
         * 
         ***********************************************************/

        public void StartExecution(string seqfile)
        {
            try
            {

                if (exeThread == null)
                {

                    //TODO: This must be run in the teststandwrapperengine thread.
                    exeThread = new System.Threading.Thread(() => SinglePassExecuteSequenceFile(seqfile));
                    exeThread.Start();

                    //Fire execution starting event.

                }
                else
                {
                    

                }

            }
            catch (COMException ce)
            {
                if (ExecutionStartFailed != null)
                    ExecutionStartFailed(this, seqfile);
            }
            catch (Exception e)
            {
                if (ExecutionStartFailed != null)
                    ExecutionStartFailed(this, seqfile + e.Message);
            }
        }

        /***********************************************************
         *  Resume execution of a sequence file.
         * 
         ***********************************************************/
        public void ResumeExecution(string seqfile)
        {
            try
            {
                this.resume();
            }
            catch (COMException ce)
            {
                if (ExecutionResumeFailed != null)
                    ExecutionResumeFailed(this, seqfile);
            }
            catch (Exception e)
            {
                if (ExecutionResumeFailed != null)
                    ExecutionResumeFailed(this, seqfile);
            }
        }

        /***********************************************************
         *  Pause a execution of a sequence file.
         * 
         ***********************************************************/
        public void PauseExecution(string seqfile)
        {
            try
            {
                this.pause();
            }
            catch (COMException ce)
            {
                if (ExecutionPauseFailed != null)
                    ExecutionPauseFailed(this, seqfile);
            }
            catch (Exception e)
            {
                if (ExecutionPauseFailed != null)
                    ExecutionPauseFailed(this, seqfile);
            }
        }

        /***********************************************************
         *  Stop executing a sequence file.
         *  
         * This implementation Aborts the execution. 
         * Fine grained Termination control could be used here.
         * 
         ***********************************************************/
        public void StopExecution(string seqfile)
        {
            try
            {
                this.stop();
            }
            catch (COMException ce)
            {
                if (ExecutionStopFailed != null)
                    ExecutionStopFailed(this, seqfile);
            }
            catch (Exception e)
            {
                if (ExecutionStopFailed != null)
                    ExecutionStopFailed(this, seqfile);
            }
        }






        /**
         *  SHOULD BE IN STEP CLASS.
         *  But it maps NI stepResult strings to the correct demo.protocol Step StatusType.
         **/
        private Demo.Protocol.Step.StatusTypes MapStepStatus(String s) 
        {            
            if (s.ToUpper().Equals("PASSED"))
                return Demo.Protocol.Step.StatusTypes.PASSED;
            else if (s.ToUpper().Equals("FAILED"))
                return Demo.Protocol.Step.StatusTypes.FAILED;

            // Default
            return Demo.Protocol.Step.StatusTypes.CAKE;
        }

        /**
         * 
         *  Maps a step of NI type to a step of Protocol type
         *  
         * */
        private Demo.Protocol.Step MapStep(Step from) 
        {
            Demo.Protocol.Step protocolStep = new Demo.Protocol.Step() 
            {

                Name = from.Name,
                Description = from.GetDescriptionEx(0),
                Settings = from.GetStepSettingsString(0),
                
                Status = MapStepStatus(from.ResultStatus),
                
                Index = from.StepIndex,
                Type = from.StepType.Name,
            };

            return protocolStep;
        }

        /**
         *  Maps a Sequence of NI type to a Sequence of Protocol type
         * 
         * */
        private Demo.Protocol.Sequence MapSequence(Sequence niSequence) 
        {
            Demo.Protocol.Sequence protocolSequence = new Demo.Protocol.Sequence()
            {
                    SequenceFileName = niSequence.Name,
                    StepList = new ObservableCollection<Demo.Protocol.Step>()
            };

            // Iterating over all the NI steps and converting them to protocol steps
            for (int s = 0; s < niSequence.GetNumSteps(StepGroups.StepGroup_Main); s++)
            {
                Step step = niSequence.GetStep(s, StepGroups.StepGroup_Main);
                Demo.Protocol.Step protoStep = MapStep(step);
               
                //verify
               //NO!

                //Add the mapped step.
                protocolSequence.StepList.Add(protoStep);

            }

            return protocolSequence;
            
        }


        public String GetResult()
        {

            Thread running_thread = exefil.GetThread(exefil.ForegroundThreadIndex);
            int context_id;

            SequenceContext context = running_thread.GetSequenceContext(0, out context_id);
            // got the sequence
            Sequence running_seq = context.Sequence;
            return context.Execution.ResultStatus;

            
        }

        public Sequence GetRunningSequence() {
           
                Thread running_thread = exefil.GetThread(exefil.ForegroundThreadIndex);
                int context_id;

                SequenceContext context = running_thread.GetSequenceContext(0, out context_id);
                // got the sequence
                Sequence running_seq = context.Sequence;
                
                return running_seq;
        }

        private Step GetCurrentStep() {
            Thread running_thread = exefil.GetThread(exefil.ForegroundThreadIndex);
            int context_id;

            SequenceContext context = running_thread.GetSequenceContext(0, out context_id);

            //Current step. Or so they say here: 
            // http://zone.ni.com/reference/en-XX/help/370052G-01/tsapiref/reftopics/sequencecontext_step_p/
            return context.Step;

        }

        private Demo.Protocol.Execution GetCurrentExecutionState(
                    Sequence now,
                    Protocol.Execution.ExecutionStates state= Protocol.Execution.ExecutionStates.STARTED)
        {

            return new Demo.Protocol.Execution()
            {
                State = state,
                CurrentSequence = MapSequence(now),
                //CurrentStep = MapStep(GetCurrentStep())
            };
        }


        private void pause() {
            exefil.Break();
        }


        private void resume() {
            exefil.Resume();
        }


        private void stop()
        {
            //soft stop
            //exefil.Terminate();
            //Cruel stop
            exefil.Abort();
        }



        // Event processor.


        /**
         * Handling UIMessage events from the engine.
         **/
        void engine_UIMessageEvent(UIMessage msg)
        {
            
            Demo.Protocol.Execution exeNow = null;
            Sequence now = null;
            String status;

            switch (msg.Event) { 
                case UIMessageCodes.UIMsg_StartExecution:
                    break;
                case UIMessageCodes.UIMsg_ModelState_BeginTesting:
                    break;
                /***
                 *  Starting execution for reals. The mains liz.
                 *  
                 * 
                 * */
                case UIMessageCodes.UIMsg_StartFileExecution:
                    
                    now = GetRunningSequence();

                    if (now.Name.Equals("MainSequence")) { 
                        //Now we are interested in updating the client.
                        
                        exeNow = GetCurrentExecutionState(now);

                        //Send it to the server
                        _server.SendToAll(TestStandEventProcessor.CreateEventPDU(Protocol.EventMessageID.ExecutionStarted, exeNow).ToJson());

                        //Lets make a execution started event.

                        if (ExecutionStarted != null)
                            ExecutionStarted(this, "");

                    }
                    break;
                    /***
                     *      Step Execution updates
                     * 
                     * 
                     * */
                case UIMessageCodes.UIMsg_Trace:
                    now = GetRunningSequence();    
                    // Now 
                    exeNow= GetCurrentExecutionState(now, Protocol.Execution.ExecutionStates.STARTED);
                    //Send it to the server
                     _server.SendToAll(TestStandEventProcessor.CreateEventPDU(Protocol.EventMessageID.ExecutionChanged, exeNow).ToJson());

                    break;

                case UIMessageCodes.UIMsg_BreakOnUserRequest:
                    if (ExecutionPaused != null)
                        ExecutionPaused(this, "");  //TODO, get seqfile from somewhere
                    now = GetRunningSequence();    
                    // Now 
                    exeNow= GetCurrentExecutionState(now, Protocol.Execution.ExecutionStates.PAUSED);
                    _server.SendToAll(TestStandEventProcessor.CreateEventPDU(Protocol.EventMessageID.ExecutionPaused, exeNow).ToJson());
                    break;

                case UIMessageCodes.UIMsg_ResumeFromBreak:
                    
                    now = GetRunningSequence();    
                    exeNow= GetCurrentExecutionState(now, Protocol.Execution.ExecutionStates.RESUMED);
                    _server.SendToAll(TestStandEventProcessor.CreateEventPDU(Protocol.EventMessageID.ExecutionResumed, exeNow).ToJson());
                    break;
                
                case UIMessageCodes.UIMsg_TerminatingExecution:
                    now = GetRunningSequence();    
                    exeNow= GetCurrentExecutionState(now, Protocol.Execution.ExecutionStates.TERMINATED);
                    _server.SendToAll(TestStandEventProcessor.CreateEventPDU(Protocol.EventMessageID.ExecutionStopped, exeNow).ToJson());

                    break;
                case UIMessageCodes.UIMsg_AbortingExecution:
                    now = GetRunningSequence();    
                    exeNow= GetCurrentExecutionState(now, Protocol.Execution.ExecutionStates.TERMINATED);
                    _server.SendToAll(TestStandEventProcessor.CreateEventPDU(Protocol.EventMessageID.ExecutionStopped, exeNow).ToJson());
                    break;

                case UIMessageCodes.UIMsg_EndExecution:
                        
                    
                    status = exefil.ResultStatus;

                    
                    break;
                case UIMessageCodes.UIMsg_ModelState_TestingComplete:
                    status = GetResult();
                    
                    //Presentation hack.

                    now = GetRunningSequence();
                    exeNow = GetCurrentExecutionState(now, Protocol.Execution.ExecutionStates.FINISHED);
                    exeNow.CurrentSequence.StepList.Clear();
                    _server.SendToAll(TestStandEventProcessor.CreateEventPDU(Protocol.EventMessageID.TestPassed, exeNow).ToJson());
                    break;

                
            }

            
            msg.Acknowledge();
        }


        public void ExecuteSequenceFile(String strSeqFilePath, String strEntryPoint="MainSequence", Boolean blocking=true) 
        {
            String strModelDescription;

            

            // Houston, Initialize countdown sequence
            f = engine.GetSequenceFileEx(
                strSeqFilePath,
                0, TypeConflictHandlerTypes.
                ConflictHandler_Error);

            //Lets release it
            engine.ReleaseSequenceFileEx(f);

            // Apparently, this is required. VIL.
            f.ModelPath = strSeqFilePath;
            f.ModelOption = ModelOptions.ModelOption_RequireSpecificModel;

            // Getting model
            SequenceFile seqFileModel = f.GetModelSequenceFile(out strModelDescription);

            exefil = engine.NewExecution(f, strEntryPoint, seqFileModel, false, 0, null, null, null);
            

            if (blocking) 
            {
                // Executing
                exefil.WaitForEndEx(-1, true, System.Type.Missing, System.Type.Missing);
            }
        }


        public void SinglePassExecuteSequenceFile(String strSeqFilePath, String strEntryPoint = "Single Pass", Boolean blocking = true)
        {
            try
            {
                LoadSequenceFile(strSeqFilePath);

                if (f == null)
                    return; //TODO fire Did not work thingy

                exefil = engine.NewExecution(f, "Single Pass", modelSequenceFile, false, 0, null, null, null);

                if (blocking)
                {
                    // Executing
                    exefil.WaitForEndEx(-1, true, System.Type.Missing, System.Type.Missing);

                    if (ExecutionStopped != null)
                        ExecutionStopped(this, strSeqFilePath);

                    exeThread = null;
                    engine.ReleaseSequenceFileEx(f);
                    engine = null;
                    exefil = null;
                    exeThread = null;
                    f = null;
                    modelSequenceFile = null;

                    //engine.ShutDown(true);

                }
            }
            catch (Exception e) { 
                //Probably wrong FILE!
                //TODO: fire something went wrong. And define soomethinge.
            }
        }



      
       

        public event Protocol.ProtocolEventHandler SequenceFileLoaded;

        public event Protocol.ProtocolErrorEventHandler SequenceFileLoadFailed;

        public event Protocol.ProtocolEventHandler ExecutionStarted;

        public event Protocol.ProtocolErrorEventHandler ExecutionStartFailed;

        public event Protocol.ProtocolEventHandler ExecutionPaused;

        public event Protocol.ProtocolErrorEventHandler ExecutionPauseFailed;

        public event Protocol.ProtocolEventHandler ExecutionResumed;

        public event Protocol.ProtocolErrorEventHandler ExecutionResumeFailed;

        public event Protocol.ProtocolEventHandler ExecutionStopped;

        public event Protocol.ProtocolErrorEventHandler ExecutionStopFailed;

        public event Protocol.ProtocolErrorEventHandler ExecutionFailed;





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
