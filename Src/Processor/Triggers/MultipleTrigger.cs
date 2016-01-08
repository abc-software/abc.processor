// ----------------------------------------------------------------------------
// <copyright file="MultipleTrigger.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor.Triggers {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.ComponentModel;

    public enum TriggerOperator {
        Or,
        And
    }

    public class MultipleTrigger : Trigger {
        private ITrigger _firstTrigger;
        private ITrigger _secondTrigger;
        private TriggerOperator _operator;

        public MultipleTrigger() {
            _operator = TriggerOperator.Or;  
        }

        [DefaultValue((Triggers.Trigger)null)]
        [ReflectorPropertyAttribute("firstTrigger", Required = true)]
        public ITrigger FirstTrigger {
            get { return _firstTrigger; }
            set { _firstTrigger = value; } 
        }

        [DefaultValue((Triggers.Trigger)null)]
        [ReflectorPropertyAttribute("secondTrigger", Required = true)]
        public ITrigger SecondTrigger {
            get { return _secondTrigger; }
            set { _secondTrigger = value; } 
        }

        [DefaultValue(TriggerOperator.Or)]
        [ReflectorPropertyAttribute("operator", Required = false)]
        public TriggerOperator Operator {
            get { return _operator; }
            set { _operator = value; }
        }

        public override DateTime NextFireTime {
            get {
                if (_firstTrigger == null || _secondTrigger == null) {
                    return DateTime.MaxValue;
                }

                if (_operator == TriggerOperator.Or) {
                    return (_firstTrigger.NextFireTime <= _secondTrigger.NextFireTime) ? _firstTrigger.NextFireTime : _secondTrigger.NextFireTime;
                }
                else {
                    return (_firstTrigger.NextFireTime <= _secondTrigger.NextFireTime) ? _secondTrigger.NextFireTime : _firstTrigger.NextFireTime;
                }
            }
        }

        public override void ProcessingCompleted() {
            if (_firstTrigger != null) {
                _firstTrigger.ProcessingCompleted();
            }

            if (_secondTrigger != null) {
                _secondTrigger.ProcessingCompleted();
            }
        }

        public override bool Fire() {
            bool result;
            if (_operator == TriggerOperator.Or) {
                result = false;
                if (_firstTrigger != null) {
                    result = result || _firstTrigger.Fire();
                }

                if (_secondTrigger != null) {
                    result = result || _secondTrigger.Fire();
                }
            }
            else {
                result = true;
                if (_firstTrigger != null) {
                    result = result && _firstTrigger.Fire();
                }

                if (_secondTrigger != null) {
                    result = result && _secondTrigger.Fire();
                }
            }

            return result; 
        }
    }
}
