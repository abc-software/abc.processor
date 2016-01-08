using System;
using NUnit.Framework;
using NUnit.Mocks;

using Abc.Processor.Triggers;
using Abc.Processor.Utils;

namespace Abc.Processor.UnitTests {

    [TestFixture]
    public class MultipleTriggerTest  {
        private DynamicMock subTrigger1Mock;
        private DynamicMock subTrigger2Mock;
        private ITrigger subTrigger1;
        private ITrigger subTrigger2;
        private MultipleTrigger trigger;

        [SetUp]
        public void Setup() {
            subTrigger1Mock = new DynamicMock(typeof(ITrigger));
            subTrigger2Mock = new DynamicMock(typeof(ITrigger));
            subTrigger1 = (ITrigger)subTrigger1Mock.MockInstance;
            subTrigger2 = (ITrigger)subTrigger2Mock.MockInstance;
            trigger = new MultipleTrigger();
            trigger.FirstTrigger = subTrigger1;
            trigger.SecondTrigger = subTrigger2;  
        }

        private void VerifyAll() {
            subTrigger1Mock.Verify();
            subTrigger2Mock.Verify();
        }

        [Test]
        public void ShouldReturnNoBuildWhenNoTriggers() {
            trigger = new MultipleTrigger();
            Assert.AreEqual(false, trigger.Fire());
        }

        [Test]
        public void ShouldNotFailWhenNoTriggersAndProcessingCompletedCalled() {
            trigger = new MultipleTrigger();
            trigger.ProcessingCompleted();
        }

        [Test]
        public void ShouldPassThroughProcessingCompletedCallToAllSubTriggers() {
            subTrigger1Mock.Expect("ProcessingCompleted");
            subTrigger2Mock.Expect("ProcessingCompleted");
            trigger.ProcessingCompleted();
            VerifyAll();
        }

        [Test]
        public void ShouldReturnFalseIfAllFalse() {
            subTrigger1Mock.ExpectAndReturn("Fire", false);
            subTrigger2Mock.ExpectAndReturn("Fire", false);
            Assert.AreEqual(false, trigger.Fire());
            VerifyAll();
        }

        [Test]
        public void ShouldReturnTrueIfOneTrue() {
            subTrigger1Mock.ExpectAndReturn("Fire", false);
            subTrigger2Mock.ExpectAndReturn("Fire", true);
            Assert.AreEqual(true, trigger.Fire());
            VerifyAll();
        }

        [Test]
        public void ShouldReturnNeverIfNoTriggerExists() {
            trigger = new MultipleTrigger();
            Assert.AreEqual(DateTime.MaxValue, trigger.NextFireTime);
        }

        [Test]
        public void ShouldReturnEarliestTriggerTimeForNextFireTime() {
            DateTime earlierDate = new DateTime(2005, 1, 1);
            subTrigger1Mock.SetReturnValue("get_NextFireTime", earlierDate);
            DateTime laterDate = new DateTime(2005, 1, 2);
            subTrigger2Mock.SetReturnValue("get_NextFireTime", laterDate);
            Assert.AreEqual(earlierDate, trigger.NextFireTime);
        }

        //[Test]
        //public void ShouldPopulateFromConfiguration() {
        //    string xml = @"<multiTrigger operator=""And""><triggers><intervalTrigger /></triggers></multiTrigger>";
        //    trigger = (MultipleTrigger)NetReflector.Read(xml);
        //    Assert.AreEqual(1, trigger.Triggers.Length);
        //    Assert.AreEqual(Operator.And, trigger.Operator);
        //}

        //[Test]
        //public void ShouldPopulateFromConfigurationWithComment() {
        //    string xml = @"<multiTrigger><!-- foo --><triggers><intervalTrigger /></triggers></multiTrigger>";
        //    trigger = (MultipleTrigger)NetReflector.Read(xml);
        //    Assert.AreEqual(1, trigger.Triggers.Length);
        //    Assert.AreEqual(typeof(IntervalTrigger), trigger.Triggers[0].GetType());
        //}

        //[Test]
        //public void ShouldPopulateFromMinimalConfiguration() {
        //    string xml = @"<multiTrigger />";
        //    trigger = (MultipleTrigger)NetReflector.Read(xml);
        //    Assert.AreEqual(0, trigger.Triggers.Length);
        //    Assert.AreEqual(Operator.Or, trigger.Operator);
        //}

        [Test]
        public void UsingAndConditionOneFalseResturnFalse() {
            trigger.Operator = TriggerOperator.And;
            subTrigger1Mock.ExpectAndReturn("Fire", false);
            subTrigger2Mock.ExpectAndReturn("Fire", true);
            Assert.AreEqual(false, trigger.Fire());
        }
    }
}
