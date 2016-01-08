// ----------------------------------------------------------------------------
// <copyright file="IntervalTrigger.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor.Triggers {
    using System;
    using System.ComponentModel;

    public class IntervalTrigger : Trigger {
        public const double DefaultIntervalSeconds = 60;

        private readonly Utils.IDateTimeProvider _dateTimeProvider;
        private double _intervalSeconds;
        private DateTime _nextFireTime;

        public IntervalTrigger()
            : this(DefaultIntervalSeconds) {
        }

        public IntervalTrigger(double intervalSeconds)
            : this(intervalSeconds, new Utils.DateTimeProvider()) {
        }

        internal IntervalTrigger(double intervalSeconds, Utils.IDateTimeProvider dateTimeProvider) {
            this._intervalSeconds = intervalSeconds;
            this._dateTimeProvider = dateTimeProvider;
        }

        [DefaultValue(DefaultIntervalSeconds)]
        [ReflectorPropertyAttribute("intervalSeconds")]
        public double IntervalSeconds {
            get {
                return this._intervalSeconds; 
            }

            set {
                this._intervalSeconds = value;
                IncrementNextFireTime();
            }
        }

        public override DateTime NextFireTime {
            get {
                if (this._nextFireTime == DateTime.MinValue) {
                    IncrementNextFireTime();
                }

                return this._nextFireTime;
            }
        }

        public override void ProcessingCompleted() {
            IncrementNextFireTime();
        }

        public override bool Fire() {
            return this._dateTimeProvider.Now >= this.NextFireTime;
        }
        
        protected DateTime IncrementNextFireTime() {
            return this._nextFireTime = this._dateTimeProvider.Now.AddSeconds(this._intervalSeconds);
        }
    }
}
