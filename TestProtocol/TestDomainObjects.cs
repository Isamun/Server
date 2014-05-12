using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Demo.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProtocol
{
    [TestClass]
    public class TestDomainObjects
    {


        [TestMethod]
        public void TestMakeExecution() {
        Execution ex = new Execution();

            Assert.IsNotNull(ex);

            Assert.IsNotNull(ex.CurrentSequence, "Sequence was not null");
            Assert.IsNull(ex.CurrentStep, "step was not null");
            
       }


        private ManualResetEvent GotEvent = new ManualResetEvent(false);

        [TestMethod]
        public void TestSettingFieldOnAExecutionObjectShouldTriggerChangedEvent() {
            Execution ex = new Execution();

            bool eventReceivd = false;

            //Listening to changed events.
            ex.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e) { 
                //Signaling that we got the event.
                eventReceivd = true;
                GotEvent.Set();

            };

            //Fire in the hall!
            ex.CurrentSequence = new Sequence();

            GotEvent.WaitOne(5000, false);

            Assert.IsTrue(eventReceivd);

            

        }

        [TestMethod]
        public void TestSequenceMakingsOf() {
            Sequence s = new Sequence();

            Assert.IsNotNull(s, "Not making instances here");
            Assert.IsInstanceOfType(s.StepList, (new ObservableCollection<Step>()).GetType());

            Assert.IsTrue(s.StepList.Count == 0, "One too many steps in an empty steplist here");
        }


        [TestMethod]
        public void TestStepMakingsOfThatStepOneStepForHumanLittleStepsForComputar() {
            Step s = new Step();

            Assert.IsNull(s.SequenceRef, "No sequence nulling here. Should be");

            // Lets change some
            int changes = 0;
            s.PropertyChanged += delegate(object src, PropertyChangedEventArgs ea)
            {
                changes += 1;
            };

            s.Data = 10;

            s.Description = "Trololol";

            Assert.IsInstanceOfType(s.Data, (new Int32()).GetType(), "Data is not numeric. Duude");
            Assert.IsTrue((int)s.Data == 10, "Set data does not match read data");
            Assert.AreEqual("Trololol", s.Description);

            s.Index = 0;
            Assert.AreEqual(s.Index, 0);
            s.Name = "FailOnlyIfNotCakeIsPresent";
            Assert.AreEqual("FailOnlyIfNotCakeIsPresent", s.Name);
            s.Settings = "lol";
            Assert.AreEqual("lol", s.Settings);
            s.Status = Step.StatusTypes.PASSED;
            Assert.AreEqual(Step.StatusTypes.PASSED, s.Status);

            Assert.AreEqual(6, changes);
            

        }

    }

    

}
