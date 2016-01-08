using System;
using NUnit.Framework;
using NUnit.Mocks;

using Abc.Processor.Triggers;
using Abc.Processor.Utils;

namespace Abc.Processor.UnitTests {

    [TestFixture]
    public class ScheduleTriggerTest {
        private DynamicMock mockDateTime;
        private ScheduleTrigger trigger;

        [SetUp]
        public void Setup() {
            mockDateTime = new DynamicMock(typeof(IDateTimeProvider));
            mockDateTime.ExpectAndReturn("get_Now", new DateTime(2002, 1, 2, 3, 0, 0, 0));

            trigger = new ScheduleTrigger(new TimeSpan(), (DayOfWeek[])DayOfWeek.GetValues(typeof(DayOfWeek)), (IDateTimeProvider)mockDateTime.MockInstance);
        }

        [TearDown]
        public void VerifyAll() {
            mockDateTime.Verify();
        }

        [Test]
        public void ShouldRunIntegrationIfCalendarTimeIsAfterIntegrationTime() {
            trigger.Time = new TimeSpan(23, 30, 0);

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
            Assert.AreEqual(false, trigger.Fire());

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 1, 23, 31, 0, 0));
            Assert.AreEqual(true, trigger.Fire());
        }

        [Test]
        public void ShouldRunIntegrationOnTheNextDay() {
            trigger.Time = new TimeSpan(23, 30, 0);

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
            Assert.AreEqual(false, trigger.Fire());

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 2, 1, 1, 0, 0));
            Assert.AreEqual(true, trigger.Fire());
        }

        [Test]
        public void ShouldIncrementTheIntegrationTimeToTheNextDayAfterIntegrationIsCompleted() {
            trigger.Time = new TimeSpan(14, 30, 0);

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 6, 27, 13, 00, 0, 0));
            Assert.AreEqual(false, trigger.Fire());

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 6, 27, 15, 00, 0, 0));
            Assert.AreEqual(true, trigger.Fire());

            trigger.ProcessingCompleted();
            Assert.AreEqual(false, trigger.Fire());

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 6, 28, 15, 00, 0, 0));
            Assert.AreEqual(true, trigger.Fire());
        }

        [Test]
        public void ShouldOnlyRunOnSpecifiedDays() {
            trigger.Weekdays.Clear();
            trigger.Weekdays.Add(DayOfWeek.Monday);
            trigger.Weekdays.Add(DayOfWeek.Wednesday);

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 11, 30));
            Assert.AreEqual(new DateTime(2004, 12, 1), trigger.NextFireTime);

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 12, 1, 0, 0, 1));
            Assert.AreEqual(true, trigger.Fire());

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 12, 2));
            Assert.AreEqual(false, trigger.Fire());
        }

//        [Test]
//        public void ShouldFullyPopulateFromReflector() {
//            string xml = string.Format(@"<scheduleTrigger name=""nightly"" time=""12:00:00"" buildCondition=""ForceBuild"">
//<weekDays>
//	<weekDay>Monday</weekDay>
//	<weekDay>Tuesday</weekDay>
//</weekDays>
//</scheduleTrigger>");
//            trigger = (ScheduleTrigger)NetReflector.Read(xml);
//            Assert.AreEqual("12:00:00", trigger.Time);
//            Assert.AreEqual(DayOfWeek.Monday, trigger.WeekDays[0]);
//            Assert.AreEqual(DayOfWeek.Tuesday, trigger.WeekDays[1]);
//            Assert.AreEqual(BuildCondition.ForceBuild, trigger.BuildCondition);
//            Assert.AreEqual("nightly", trigger.Name);
//        }

        //[Test]
        //public void ShouldMinimallyPopulateFromReflector() {
        //    string xml = string.Format(@"<scheduleTrigger time=""10:00:00"" />");
        //    trigger = (ScheduleTrigger)NetReflector.Read(xml);
        //    Assert.AreEqual("10:00:00", trigger.Time);
        //    Assert.AreEqual(7, trigger.WeekDays.Length);
        //    Assert.AreEqual(BuildCondition.IfModificationExists, trigger.BuildCondition);
        //    Assert.AreEqual("ScheduleTrigger", trigger.Name);
        //}

        [Test]
        public void NextBuildTimeShouldBeSameTimeNextDay() {
            mockDateTime.SetReturnValue("get_Now", new DateTime(2005, 2, 4, 13, 13, 0));

            trigger.Time = new TimeSpan(10, 0, 0);
            trigger.Weekdays.Clear();
            trigger.Weekdays.Add(DayOfWeek.Friday);
            trigger.Weekdays.Add(DayOfWeek.Saturday);
            trigger.Weekdays.Add(DayOfWeek.Sunday);
            trigger.ProcessingCompleted();

            DateTime expectedDate = new DateTime(2005, 2, 5, 10, 0, 0);
            Assert.AreEqual(expectedDate, trigger.NextFireTime);
        }

        [Test]
        public void NextBuildTimeShouldBeTheNextSpecifiedDay() {
            mockDateTime.SetReturnValue("get_Now", new DateTime(2005, 2, 4, 13, 13, 0));		// Friday
            trigger.Time = new TimeSpan(10, 0, 0);
            trigger.Weekdays.Clear();
            trigger.Weekdays.Add(DayOfWeek.Friday);
            trigger.Weekdays.Add(DayOfWeek.Sunday);
            Assert.AreEqual(new DateTime(2005, 2, 6, 10, 0, 0), trigger.NextFireTime);		// Sunday

            mockDateTime.SetReturnValue("get_Now", new DateTime(2005, 2, 6, 13, 13, 0));		// Sunday
            Assert.AreEqual(true, trigger.Fire());
            trigger.ProcessingCompleted();
            Assert.AreEqual(new DateTime(2005, 2, 11, 10, 0, 0), trigger.NextFireTime);	// Friday
        }

        [Test]
        public void NextBuildTimeShouldBeTheNextSpecifiedDayWithTheNextDayFarAway() {
            mockDateTime.SetReturnValue("get_Now", new DateTime(2005, 2, 4, 13, 13, 0));
            trigger.Time = new TimeSpan(10, 0, 0);
            trigger.Weekdays.Clear();
            trigger.Weekdays.Add(DayOfWeek.Friday);
            trigger.Weekdays.Add(DayOfWeek.Thursday);
            Assert.AreEqual(new DateTime(2005, 2, 10, 10, 0, 0), trigger.NextFireTime);

            mockDateTime.SetReturnValue("get_Now", new DateTime(2005, 2, 10, 13, 13, 0));
            Assert.AreEqual(true, trigger.Fire());
            trigger.ProcessingCompleted();
            Assert.AreEqual(new DateTime(2005, 2, 11, 10, 0, 0), trigger.NextFireTime);
        }

        [Test]
        public void ShouldNotUpdateNextBuildTimeUnlessScheduledBuildHasRun() {
            mockDateTime.SetReturnValue("get_Now", new DateTime(2005, 2, 4, 9, 0, 1));
            trigger.Time = new TimeSpan(10, 0, 0);
            Assert.AreEqual(new DateTime(2005, 2, 4, 10, 0, 0), trigger.NextFireTime);

            mockDateTime.SetReturnValue("get_Now", new DateTime(2005, 2, 4, 10, 0, 1));
            trigger.ProcessingCompleted();
            Assert.AreEqual(new DateTime(2005, 2, 4, 10, 0, 0), trigger.NextFireTime);
        }
    }
}
