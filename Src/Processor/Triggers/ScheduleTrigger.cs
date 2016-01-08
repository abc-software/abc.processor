// ----------------------------------------------------------------------------
// <copyright file="ScheduleTrigger.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor.Triggers {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
using System.ComponentModel;

    public class ScheduleTrigger : Trigger {
        private readonly Utils.IDateTimeProvider _dateTimeProvider;
        private DateTime _nextFireTime;
        private TimeSpan _fireTime;
        private Collection<DayOfWeek> _weekdays;
        private bool _triggered;

        public ScheduleTrigger()
            : this(new TimeSpan()) {
        }

        public ScheduleTrigger(TimeSpan time)
            : this(time, (DayOfWeek[])DayOfWeek.GetValues(typeof(DayOfWeek))) {
        }

        public ScheduleTrigger(TimeSpan time, DayOfWeek[] weekdays)
            : this(time, weekdays, new Utils.DateTimeProvider()) {
        }

        internal ScheduleTrigger(TimeSpan time, DayOfWeek[] weekdays, Utils.IDateTimeProvider dateTimeProvider) {
            if (weekdays == null) {
                throw new ArgumentNullException("weekdays");
            }

            if (dateTimeProvider == null) {
                throw new ArgumentNullException("dateTimeProvider");
            }

            _fireTime = time;
            _weekdays = new Collection<DayOfWeek>(new List<DayOfWeek>(weekdays));
            _dateTimeProvider = dateTimeProvider; 
        }

        /// <summary>
        /// Gets or sets the schedule time.
        /// </summary>
        /// <value>
        /// The schedule time.
        /// </value>
        [DefaultValue(typeof(TimeSpan), "00:00:00")]
        [ReflectorPropertyAttribute("time")]
        public TimeSpan Time {
            get { return _fireTime; }
            set { _fireTime = value; }
        }

        /// <summary>
        /// Gets the weekdays.
        /// </summary>
        //[Editor(typeof(Design.DayOfWeekUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Collection<DayOfWeek> Weekdays {
            get { return _weekdays; }
        }

        public override DateTime NextFireTime {
            get {
                if (_nextFireTime == DateTime.MinValue) {
                    IncrementNextFireTime();
                }

                return _nextFireTime;
            }
        }

        public override void ProcessingCompleted() {
            if (_triggered) {
                IncrementNextFireTime();
            }

            _triggered = false;
        }

        public override bool Fire() {
            DateTime now = _dateTimeProvider.Now;
            if (now > this.NextFireTime && IsValidWeekDay(now.DayOfWeek)) {
                _triggered = true;
                return true;
            }

            return false;
        }

        protected override bool OnReflectUnrecognizedAttribute(string name) {
            if (name == "weekDays") {
                string value = Attributes["weekDays"] as string;
                if (string.IsNullOrEmpty(value)) {
                    throw new ConfigurationErrorsException(SR.EmptyWeekdays);
                }

                string[] weekdays = value.Split(',');
                for (int i = 0; i < weekdays.Length; i++) {
                    try {
                        _weekdays.Add((DayOfWeek)Enum.Parse(typeof(DayOfWeek), weekdays[i]));
                    }
                    catch (Exception innerException) {
                        throw new ConfigurationErrorsException(SR.InvalidWeekdayFormat, innerException);
                    }
                }

                return true;
            }

            return base.OnReflectUnrecognizedAttribute(name);
        }

        protected DateTime IncrementNextFireTime() {
            DateTime now = _dateTimeProvider.Now;

            _nextFireTime = new DateTime(now.Year, now.Month, now.Day, _fireTime.Hours, _fireTime.Minutes, 0, 0);
            if (now >= _nextFireTime) {
                _nextFireTime = _nextFireTime.AddDays(1);
            }

            // CalculateNextIntegrationTime
            while (true) {
                if (IsValidWeekDay(_nextFireTime.DayOfWeek)) {
                    break;
                }

                _nextFireTime = _nextFireTime.AddDays(1);
            }

            return _nextFireTime;
        }

        private bool IsValidWeekDay(DayOfWeek nextFireDay) {
            return _weekdays.Contains(nextFireDay);
        }
    }
}
