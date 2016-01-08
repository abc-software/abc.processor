// ----------------------------------------------------------------------------
// <copyright file="FilterTrigger.cs" company="ABC Software Ltd">
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

    /// <summary>
    /// Filter trigger.
    /// </summary>
    public class FilterTrigger : Trigger {
        private readonly Utils.IDateTimeProvider _dateTimeProvider;
        private TimeSpan _startTime;
        private TimeSpan _endTime;
        private Collection<DayOfWeek> _weekdays;
        private ITrigger _innerTrigger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterTrigger"/> class.
        /// </summary>
        public FilterTrigger()
            : this(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59), null) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterTrigger"/> class.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="innerTrigger">The inner trigger.</param>
        public FilterTrigger(TimeSpan startTime, TimeSpan endTime, ITrigger innerTrigger)
            :
            this(startTime, endTime, (DayOfWeek[])DayOfWeek.GetValues(typeof(DayOfWeek)), innerTrigger) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterTrigger"/> class.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="weekdays">The weekdays.</param>
        /// <param name="innerTrigger">The inner trigger.</param>
        public FilterTrigger(TimeSpan startTime, TimeSpan endTime, DayOfWeek[] weekdays, ITrigger innerTrigger)
            : this(startTime, endTime, weekdays, innerTrigger, new Utils.DateTimeProvider()) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterTrigger"/> class.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="weekdays">The weekdays.</param>
        /// <param name="innerTrigger">The inner trigger.</param>
        /// <param name="dateTimeProvider">The date time provider.</param>
        internal FilterTrigger(TimeSpan startTime, TimeSpan endTime, DayOfWeek[] weekdays, ITrigger innerTrigger, Utils.IDateTimeProvider dateTimeProvider) {
            if (weekdays == null) {
                throw new ArgumentNullException("weekdays");
            }

            if (dateTimeProvider == null) {
                throw new ArgumentNullException("dateTimeProvider");
            }

            _startTime = startTime;
            _endTime = endTime;
            /*_weekdays = new Collection<DayOfWeek>(weekdays);*/
            _weekdays = new Collection<DayOfWeek>(new List<DayOfWeek>(weekdays));
            _innerTrigger = innerTrigger;
            _dateTimeProvider = dateTimeProvider;
        }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        [DefaultValue(typeof(TimeSpan), "00:00:00")]
        [ReflectorPropertyAttribute("startTime", Required = true)]
        public TimeSpan StartTime {
            get { return _startTime; }
            set { _startTime = value; }
        }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>
        /// The end time.
        /// </value>
        [DefaultValue(typeof(TimeSpan), "23:59:59")]
        [ReflectorPropertyAttribute("endTime", Required = true)]
        public TimeSpan EndTime {
            get { return _endTime; }
            set { _endTime = value; }
        }

        /// <summary>
        /// Gets or sets the inner trigger.
        /// </summary>
        /// <value>
        /// The inner trigger.
        /// </value>
        [DefaultValue((Triggers.Trigger)null)]
        [ReflectorPropertyAttribute("innerTrigger", Required = true)]
        public ITrigger InnerTrigger {
            get { return _innerTrigger; }
            set { _innerTrigger = value; }
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
                DateTime innerTriggerFireTime = _innerTrigger.NextFireTime;
                if (IsInFilterRange(innerTriggerFireTime)) {
                    DateTime nextFireTime = new DateTime(innerTriggerFireTime.Year, innerTriggerFireTime.Month, innerTriggerFireTime.Day);
                    nextFireTime = nextFireTime.AddTicks(_endTime.Ticks);
                    return nextFireTime;
                }

                return innerTriggerFireTime;
            }
        }

        public override void ProcessingCompleted() {
            _innerTrigger.ProcessingCompleted();
        }

        public override bool Fire() {
            DateTime now = _dateTimeProvider.Now;
            if (IsInFilterRange(now)) {
                return false;
            }

            return _innerTrigger.Fire();
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

        private bool IsInFilterRange(DateTime dateTime) {
            return IsDateInFilterWeekDays(dateTime) && IsTimeInFilterTimeRange(dateTime);
        }

        private bool IsDateInFilterWeekDays(DateTime dateTime) {
            if (_weekdays.Count == 0) {
                return true;
            }

            return _weekdays.Contains(dateTime.DayOfWeek);
        }

        private bool IsTimeInFilterTimeRange(DateTime dateTime) {
            TimeSpan timeOfDay = dateTime.TimeOfDay;
            if (_startTime < _endTime) {
                return timeOfDay >= _startTime && dateTime.TimeOfDay <= _endTime;
            }
            else {
                return !(timeOfDay <= _startTime) || !(dateTime.TimeOfDay >= _endTime);
            }
        }
    }
}
