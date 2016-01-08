// ----------------------------------------------------------------------------
// <copyright file="TriggerElement.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor.Configuration {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;

    using Abc.Processor.Triggers;

    /// <summary>
    /// Trigger configuration element.
    /// </summary>
    internal class TriggerElement : TypedElement {
        private static readonly ConfigurationProperty _propName = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
        private ConfigurationProperty _propTriggerTypeName;
        private IDictionary<string, string> _attributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerElement"/> class.
        /// </summary>
        public TriggerElement()
            : this(true) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerElement"/> class.
        /// </summary>
        /// <param name="allowReferences">if set to <c>true</c> [allow references].</param>
        public TriggerElement(bool allowReferences)
            : base(typeof(Trigger)) {
            ConfigurationPropertyOptions options = allowReferences ? ConfigurationPropertyOptions.None : ConfigurationPropertyOptions.IsRequired;
            this._propTriggerTypeName = new ConfigurationProperty("type", typeof(string), null, options);
            this._properties.Remove("type");
            this._properties.Add(_propTriggerTypeName);
            this._properties.Add(_propName);
        }

        /// <summary>
        /// Gets or sets the trigger name.
        /// </summary>
        /// <value>
        /// The trigger name.
        /// </value>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name {
            get {
                return (string)base[_propName];
            }

            set {
                base[_propName] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the trigger type.
        /// </summary>
        /// <value>
        /// The name of the trigger type.
        /// </value>
        [ConfigurationProperty("type")]
        public override string TypeName {
            get {
                return (string)base[this._propTriggerTypeName];
            }

            set {
                base[this._propTriggerTypeName] = value;
            }
        }

        /// <summary>
        /// Gets the trigger attributes.
        /// </summary>
        public IDictionary<string, string> Attributes {
            get {
                if (this._attributes == null) {
                    lock (this) {
                        if (this._attributes == null) {
                            this._attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        }
                    }
                }

                return this._attributes;
            }
        }

        /// <summary>
        /// Gets the runtime object.
        /// </summary>
        /// <returns>
        /// The trigger object <see cref="ITrigger"/>.
        /// </returns>
        public ITrigger GetRuntimeObject() {
            if (string.IsNullOrEmpty(this.TypeName)) {
                if (ProcessorConfiguration.SharedTriggers == null) {
                    throw new ConfigurationErrorsException(SR.ReferenceToNonexistentTriggerFormat(this.Name));
                }

                TriggerElement element = ProcessorConfiguration.SharedTriggers[this.Name];
                if (element == null) {
                    throw new ConfigurationErrorsException(SR.ReferenceToNonexistentTriggerFormat(this.Name));
                }

                this._runtimeObject = element.GetRuntimeObject();
                return (Trigger)this._runtimeObject;
            }

            ITrigger trigger = (ITrigger)this.BaseGetRuntimeObject();
            trigger.Name = this.Name;

            // Only for internal triggers
            var intrenalTrigger = trigger as Trigger;
            if (intrenalTrigger != null) {
                intrenalTrigger.SetAttributes(this.Attributes);
            }

            this._runtimeObject = trigger;
            return trigger;
        }

        internal override void ResetProperties() {
            base.ResetProperties();
            this._properties.Remove("type");
            this._properties.Add(_propTriggerTypeName);
            this._properties.Add(_propName);
        }

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value) {
            ConfigurationProperty property = new ConfigurationProperty(name, typeof(string), value);
            this._properties.Add(property);
            base[property] = value;
            this.Attributes.Add(name, value);
            return true;
        }
    }
}
