using System;
using NUnit.Framework;
using NUnit.Mocks;

using Abc.Processor.Triggers;
using Abc.Processor.Utils;

namespace Abc.Processor.UnitTests {

    [TestFixture]
    public class FilterTriggerTest  {
        private DynamicMock mockTrigger;
        private DynamicMock mockDateTime;
        private FilterTrigger trigger;

        [SetUp]
        protected void CreateMocksAndInitialiseObjectUnderTest() {
            mockTrigger = new DynamicMock(typeof(ITrigger));
            mockDateTime = new DynamicMock(typeof(IDateTimeProvider));

            trigger = new FilterTrigger(new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0), new DayOfWeek[] { DayOfWeek.Wednesday }, (ITrigger)mockTrigger.MockInstance, (IDateTimeProvider)mockDateTime.MockInstance);
        }

        [TearDown]
        protected void VerifyMocks() {
            mockDateTime.Verify();
            mockTrigger.Verify();
        }

        [Test]
        public void ShouldNotInvokeDecoratedTriggerWhenTimeAndWeekDayMatch() {
            mockTrigger.ExpectNoCall("Fire");
            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 12, 1, 10, 30, 0, 0));

            Assert.AreEqual(false, trigger.Fire());
        }

        [Test]
        public void ShouldNotInvokeDecoratedTriggerWhenWeekDaysNotSpecified() {
            mockTrigger.ExpectNoCall("Fire");
            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 12, 1, 10, 30, 0, 0));
            trigger.Weekdays.Clear();

            Assert.AreEqual(false, trigger.Fire());
        }

        [Test]
        public void ShouldInvokeDecoratedTriggerWhenTimeIsOutsideOfRange() {
            mockTrigger.SetReturnValue("Fire", true);
            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 12, 1, 11, 30, 0, 0));

            Assert.AreEqual(true, trigger.Fire());
        }

        [Test]
        public void ShouldNotInvokeOverMidnightTriggerWhenCurrentTimeIsBeforeMidnight() {
            trigger.StartTime = new TimeSpan(23, 0, 0);
            trigger.EndTime = new TimeSpan(7, 0, 0); 

            mockTrigger.ExpectNoCall("Fire");
            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 12, 1, 23, 30, 0, 0));

            Assert.AreEqual(false, trigger.Fire());
        }

        [Test]
        public void ShouldNotInvokeOverMidnightTriggerWhenCurrentTimeIsAfterMidnight() {
            trigger.StartTime = new TimeSpan(23, 0, 0);
            trigger.EndTime = new TimeSpan(7, 0, 0);

            mockTrigger.ExpectNoCall("Fire");
            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 12, 1, 00, 30, 0, 0));

            Assert.AreEqual(false, trigger.Fire());
        }

        [Test]
        public void ShouldInvokeOverMidnightTriggerWhenCurrentTimeIsOutsideOfRange() {
            trigger.StartTime = new TimeSpan(23, 0, 0);
            trigger.EndTime = new TimeSpan(7, 0, 0);

            mockTrigger.SetReturnValue("Fire", true);
            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 12, 1, 11, 30, 0, 0));

            Assert.AreEqual(true, trigger.Fire());
        }

        [Test]
        public void ShouldNotInvokeDecoratedTriggerWhenTimeIsEqualToStartTimeOrEndTime() {
            mockTrigger.ExpectNoCall("Fire");
            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 12, 1, 10, 00, 0, 0));
            Assert.AreEqual(false, trigger.Fire());

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 12, 1, 11, 00, 0, 0));
            Assert.AreEqual(false, trigger.Fire());
        }

        [Test]
        public void ShouldNotInvokeDecoratedTriggerWhenTodayIsOneOfSpecifiedWeekdays() {
            mockTrigger.SetReturnValue("Fire", true);
            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 12, 2, 10, 30, 0, 0));

            Assert.AreEqual(true, trigger.Fire());
        }

        [Test]
        public void ShouldDelegateIntegrationCompletedCallToInnerTrigger() {
            mockTrigger.Expect("ProcessingCompleted");
            trigger.ProcessingCompleted();
        }

        [Test]
        public void ShouldUseFilterEndTimeIfTriggerFireTimeIsInFilter() {
            DateTime triggerNextBuildTime = new DateTime(2004, 12, 1, 10, 30, 00);
            mockTrigger.SetReturnValue("get_NextFireTime", triggerNextBuildTime);
            Assert.AreEqual(new DateTime(2004, 12, 1, 11, 00, 00), trigger.NextFireTime);
        }

        [Test]
        public void ShouldNotFilterIfTriggerFireDayIsNotInFilter() {
            DateTime triggerNextBuildTime = new DateTime(2004, 12, 4, 10, 00, 00);
            mockTrigger.SetReturnValue("get_NextFireTime", triggerNextBuildTime);
            Assert.AreEqual(triggerNextBuildTime, trigger.NextFireTime);
        }

        [Test]
        public void ShouldNotFilterIfTriggerFireTimeIsNotInFilter() {
            DateTime nextBuildTime = new DateTime(2004, 12, 1, 13, 30, 00);
            mockTrigger.SetReturnValue("get_NextFireTime", nextBuildTime);
            Assert.AreEqual(nextBuildTime, trigger.NextFireTime);
        }

//        [Test]
//        public void ShouldFullyPopulateFromReflector() {
//            string xml =
//                string.Format(
//                    @"<filterTrigger startTime=""8:30:30"" endTime=""22:30:30"" buildCondition=""ForceBuild"">
//											<trigger type=""scheduleTrigger"" time=""12:00:00""/>
//											<weekDays>
//												<weekDay>Monday</weekDay>
//												<weekDay>Tuesday</weekDay>
//											</weekDays>
//										</filterTrigger>");
//            trigger = (FilterTrigger)NetReflector.Read(xml);
//            Assert.AreEqual("08:30:30", trigger.StartTime);
//            Assert.AreEqual("22:30:30", trigger.EndTime);
//            Assert.AreEqual(typeof(ScheduleTrigger), trigger.InnerTrigger.GetType());
//            Assert.AreEqual(DayOfWeek.Monday, trigger.WeekDays[0]);
//            Assert.AreEqual(DayOfWeek.Tuesday, trigger.WeekDays[1]);
//            Assert.AreEqual(BuildCondition.ForceBuild, trigger.BuildCondition);
//        }

//        [Test]
//        public void ShouldMinimallyPopulateFromReflector() {
//            string xml =
//                string.Format(
//                    @"<filterTrigger startTime=""8:30:30"" endTime=""22:30:30"">
//											<trigger type=""scheduleTrigger"" time=""12:00:00"" />
//										</filterTrigger>");
//            trigger = (FilterTrigger)NetReflector.Read(xml);
//            Assert.AreEqual("08:30:30", trigger.StartTime);
//            Assert.AreEqual("22:30:30", trigger.EndTime);
//            Assert.AreEqual(typeof(ScheduleTrigger), trigger.InnerTrigger.GetType());
//            Assert.AreEqual(7, trigger.WeekDays.Length);
//            Assert.AreEqual(BuildCondition.NoBuild, trigger.BuildCondition);
//        }

//        [Test]
//        public void ShouldHandleNestedFilterTriggers() {
//            string xml =
//                @"<filterTrigger startTime=""19:00"" endTime=""07:00"">
//                    <trigger type=""filterTrigger"" startTime=""0:00"" endTime=""23:59:59"">
//                        <trigger type=""intervalTrigger"" name=""continuous"" seconds=""900"" buildCondition=""ForceBuild""/>
//                        <weekDays>
//                            <weekDay>Saturday</weekDay>
//                            <weekDay>Sunday</weekDay>
//                        </weekDays>
//                    </trigger>
//				  </filterTrigger>";
//            trigger = (FilterTrigger)NetReflector.Read(xml);
//            Assert.AreEqual(typeof(FilterTrigger), trigger.InnerTrigger.GetType());
//            Assert.AreEqual(typeof(IntervalTrigger), ((FilterTrigger)trigger.InnerTrigger).InnerTrigger.GetType());
//        }

        [Test]
        public void ShouldOnlyFireBetween7AMAnd7PMOnWeekdays() {
            FilterTrigger outerTrigger = new FilterTrigger(new TimeSpan(), new TimeSpan(), (DayOfWeek[])DayOfWeek.GetValues(typeof(DayOfWeek)), null, (IDateTimeProvider)mockDateTime.MockInstance);
            outerTrigger.StartTime = new TimeSpan(19, 0, 0);
            outerTrigger.EndTime = new TimeSpan(7, 0, 0);
            outerTrigger.InnerTrigger = trigger;

            trigger.StartTime = new TimeSpan(0, 0, 0);
            trigger.EndTime = new TimeSpan(23, 59, 59);
            trigger.Weekdays.Clear();
            trigger.Weekdays.Add(DayOfWeek.Saturday);
            trigger.Weekdays.Add(DayOfWeek.Sunday);
            mockTrigger.SetReturnValue("Fire", true);

            mockDateTime.SetReturnValue("get_Now", new DateTime(2006, 8, 10, 11, 30, 0, 0)); // Thurs midday
            Assert.AreEqual(true, outerTrigger.Fire());

            mockDateTime.SetReturnValue("get_Now", new DateTime(2006, 8, 10, 19, 30, 0, 0)); // Thurs evening
            Assert.AreEqual(false, outerTrigger.Fire());

            mockDateTime.SetReturnValue("get_Now", new DateTime(2006, 8, 12, 11, 30, 0, 0)); // Sat midday
            Assert.AreEqual(false, outerTrigger.Fire());

            mockDateTime.SetReturnValue("get_Now", new DateTime(2006, 8, 12, 19, 30, 0, 0)); // Sat evening
            Assert.AreEqual(false, outerTrigger.Fire());
        }
    }

}
