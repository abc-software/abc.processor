// ----------------------------------------------------------------------------
// <copyright file="CronTrigger.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor.Triggers {
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The CRON expressoion trigger.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cron", Justification = "Cron is UNIX fromat")]
    public class CronTrigger : Trigger {
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cron", Justification = "Cron is UNIX fromat")]
        public const string DefaultCronExpression = "* * * * * ?";

        private readonly Utils.IDateTimeProvider dateTimeProvider;
        private DateTime nextFireTime;
        private bool triggered;
        private Quartz.CronExpression cronexpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="CronTrigger"/> class.
        /// </summary>
        public CronTrigger() :
            this(DefaultCronExpression) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CronTrigger"/> class.
        /// </summary>
        /// <param name="expression">The CRON expression.</param>
        public CronTrigger(string expression)
            : this(expression, new Utils.DateTimeProvider()) {
        }

        internal CronTrigger(string expression, Utils.IDateTimeProvider dateTimeProvider) {
            if (expression == null) {
                throw new ArgumentNullException("expression");
            }

            if (dateTimeProvider == null) {
                throw new ArgumentNullException("dateTimeProvider");
            }

            this.cronexpression = new Quartz.CronExpression(expression);
            this.dateTimeProvider = dateTimeProvider;
        }

        /// <summary>
        /// Gets or sets the schedule time.
        /// </summary>
        /// <value>
        /// The schedule time.
        /// </value>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cron", Justification = "Cron is UNIX fromat")]
        [DefaultValue(DefaultCronExpression)]
        [ReflectorPropertyAttribute("cronExpression")]
        public string CronExpression {
            get {
                return this.cronexpression.CronExpressionString;
            }

            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }

                this.cronexpression = new Quartz.CronExpression(value);
                this.nextFireTime = DateTime.MinValue;
            }
        }

        /// <inheritdoc/>
        public override DateTime NextFireTime {
            get {
                if (this.nextFireTime == DateTime.MinValue) {
                    this.IncrementNextFireTime();
                }

                return this.nextFireTime;
            }
        }

        /// <inheritdoc/>
        public override bool Fire() {
            DateTime now = this.dateTimeProvider.Now;
            if (now > this.NextFireTime && !this.triggered) {
                this.triggered = true;
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public override void ProcessingCompleted() {
            if (this.triggered) {
                this.IncrementNextFireTime();
            }

            this.triggered = false;
        }

        /// <summary>
        /// Increments the next fire time.
        /// </summary>
        /// <returns>The next fire time.</returns>
        protected DateTime IncrementNextFireTime() {
            DateTime now = this.dateTimeProvider.Now;

            var nextTime = cronexpression.GetNextValidTimeAfter(now.ToUniversalTime());
            if (nextTime.HasValue) {
                this.nextFireTime = nextTime.Value.ToLocalTime();
            }

            return this.nextFireTime;
        }
    }
}