using System;
using NUnit.Framework;
using NUnit.Mocks;

using Abc.Processor.Triggers;
using Abc.Processor.Utils;

namespace Abc.Processor.UnitTests {
    [TestFixture]
    public class CronTriggerTest {
        private DynamicMock mockDateTime;
        private CronTrigger trigger;

        [SetUp]
        public void Setup() {
            mockDateTime = new DynamicMock(typeof(IDateTimeProvider));
            mockDateTime.ExpectAndReturn("get_Now", new DateTime(2002, 1, 2, 3, 0, 0, 0));

            trigger = new CronTrigger("* * * * * ?", (IDateTimeProvider)mockDateTime.MockInstance);
        }

        [TearDown]
        public void VerifyAll() {
            mockDateTime.Verify();
        }

        [Test]
        public void VerifyThatShouldRequestIntegrationAfterTenSeconds() {
            trigger.CronExpression = "0/10 * * * * ?";

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 1, 1, 0, 0, 0));
            Assert.AreEqual(false, trigger.Fire());
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
        public void ShouldRunIntegrationIfCalendarTimeIsAfterIntegrationTime() {
            trigger.CronExpression = "0 30 23 * * ?";

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
            Assert.AreEqual(false, trigger.Fire());

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 1, 23, 31, 0, 0));
            Assert.AreEqual(true, trigger.Fire());
        }

        [Test]
        public void ShouldRunIntegrationOnTheNextDay() {
            trigger.CronExpression = "0 30 23 * * ?";

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 1, 23, 25, 0, 0));
            Assert.AreEqual(false, trigger.Fire());

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 1, 2, 1, 1, 0, 0));
            Assert.AreEqual(true, trigger.Fire());
        }

        [Test]
        public void ShouldIncrementTheIntegrationTimeToTheNextDayAfterIntegrationIsCompleted() {
            trigger.CronExpression = "0 30 14 * * ?";

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
            trigger.CronExpression = "* * * ? * MON,WED";

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 11, 30));
            Assert.AreEqual(new DateTime(2004, 12, 1), trigger.NextFireTime);

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 12, 1, 0, 0, 1));
            Assert.AreEqual(true, trigger.Fire());

            mockDateTime.SetReturnValue("get_Now", new DateTime(2004, 12, 2));
            Assert.AreEqual(false, trigger.Fire());
        }

        [Test]
        public void NextBuildTimeShouldBeSameTimeNextDay() {
            mockDateTime.SetReturnValue("get_Now", new DateTime(2005, 2, 4, 13, 13, 0));

            trigger.CronExpression = "0 0 10 ? * SUN,FRI,SAT";
            trigger.ProcessingCompleted();

            DateTime expectedDate = new DateTime(2005, 2, 5, 10, 0, 0);
            Assert.AreEqual(expectedDate, trigger.NextFireTime);
        }

        [Test]
        public void NextBuildTimeShouldBeTheNextSpecifiedDay() {
            mockDateTime.SetReturnValue("get_Now", new DateTime(2005, 2, 4, 13, 13, 0));		// Friday
            trigger.CronExpression = "0 0 10 ? * 1,6"; // Sunday,Friday

            Assert.AreEqual(new DateTime(2005, 2, 6, 10, 0, 0), trigger.NextFireTime);		// Sunday

            mockDateTime.SetReturnValue("get_Now", new DateTime(2005, 2, 6, 13, 13, 0));		// Sunday
            Assert.AreEqual(true, trigger.Fire());
            trigger.ProcessingCompleted();
            Assert.AreEqual(new DateTime(2005, 2, 11, 10, 0, 0), trigger.NextFireTime);	// Friday
        }
    }
}
