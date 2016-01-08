using System;
using NUnit.Framework;
using NUnit.Mocks;

using Abc.Processor.Triggers;
using Abc.Processor.Utils;


namespace Abc.Processor.UnitTests {
    [TestFixture]
    public class IntervalTriggerTest {
        private DynamicMock mockDateTime;
        private DateTime initialDateTimeNow;

        private IntervalTrigger trigger;

        [SetUp]
        public void Setup() {
            mockDateTime = new DynamicMock(typeof(IDateTimeProvider));
            initialDateTimeNow = new DateTime(2002, 1, 2, 3, 0, 0, 0);
            mockDateTime.ExpectAndReturn("get_Now", this.initialDateTimeNow);
            trigger = new IntervalTrigger(60, (IDateTimeProvider)mockDateTime.MockInstance);
        }

        [TearDown]
        public void VerifyAll() {
            mockDateTime.Verify();
        }

        //[Test]
        //public void ShouldFullyPopulateFromReflector() {
        //    string xml = string.Format(@"<intervalTrigger name=""continuous"" seconds=""1"" buildCondition=""ForceBuild"" />");
        //    trigger = (IntervalTrigger)NetReflector.Read(xml);
        //    Assert.AreEqual(1, trigger.IntervalSeconds);
        //    Assert.AreEqual(BuildCondition.ForceBuild, trigger.BuildCondition);
        //    Assert.AreEqual("continuous", trigger.Name);
        //}

        //[Test]
        //public void ShouldDefaultPopulateFromReflector() {
        //    string xml = string.Format(@"<intervalTrigger />");
        //    trigger = (IntervalTrigger)NetReflector.Read(xml);
        //    Assert.AreEqual(IntervalTrigger.DefaultIntervalSeconds, trigger.IntervalSeconds);
        //    Assert.AreEqual(BuildCondition.IfModificationExists, trigger.BuildCondition);
        //    Assert.AreEqual("IntervalTrigger", trigger.Name);
        //}

        [Test]
        public void VerifyThatShouldRequestIntegrationAfterTenSeconds() {
            trigger.IntervalSeconds = 10;

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 1, 1, 0, 0, 0));
            Assert.AreEqual(true, trigger.Fire());
            trigger.ProcessingCompleted();

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 1, 1, 0, 5, 0)); // 5 seconds later
            Assert.AreEqual(false, trigger.Fire());

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 1, 1, 0, 9, 0)); // 4 seconds later
            Assert.AreEqual(false, trigger.Fire());

            // sleep beyond the 1sec mark
            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 1, 1, 0, 14, 0)); // 5 seconds later
            Assert.AreEqual(true, trigger.Fire());
            
            trigger.ProcessingCompleted();
            Assert.AreEqual(false, trigger.Fire());
        }

        [Test]
        public void ProcessTrigger() {
            trigger.IntervalSeconds = 0.5;

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 1, 1, 0, 0, 0));
            Assert.AreEqual(true, trigger.Fire());

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 1, 1, 0, 0, 550));
            Assert.AreEqual(true, trigger.Fire());

            trigger.ProcessingCompleted();
            Assert.AreEqual(false, trigger.Fire());

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 1, 1, 0, 1, 50));
            Assert.AreEqual(true, trigger.Fire());

            trigger.ProcessingCompleted();
            Assert.AreEqual(false, trigger.Fire());

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 1, 1, 0, 1, 550));
            Assert.AreEqual(true, trigger.Fire());
        }

        [Test]
        public void ShouldReturnIntervalTimeForNextBuildOnServerStart() {
            trigger.IntervalSeconds = 10;

            Assert.AreEqual(initialDateTimeNow.AddSeconds(10), trigger.NextFireTime);
        }

        [Test]
        public void ShouldReturnIntervalTimeIfLastBuildJustOccured() {
            trigger.IntervalSeconds = 10;

            DateTime stubNow = new DateTime(2004, 1, 1, 1, 0, 0, 0);
            mockDateTime.SetReturnValue("get_Now", stubNow);

            trigger.ProcessingCompleted();
            Assert.AreEqual(stubNow.AddSeconds(10), trigger.NextFireTime);
        }
    }

}
