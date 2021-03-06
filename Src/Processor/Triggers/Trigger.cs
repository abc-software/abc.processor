// ----------------------------------------------------------------------------
// <copyright file="Trigger.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor.Triggers {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// Trigger base class.
    /// </summary>
    public abstract class Trigger : ReflectorBase, ITrigger {
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Trigger"/> class.
        /// </summary>
        protected Trigger() {
        }

        /// <summary>
        /// Atgrež uzstāda trigera nosaukums.
        /// </summary>
        /// <value>Trigera anosaukums.</value>
        public string Name {
            get { return this.name ?? string.Empty; }
            set { this.name = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public abstract DateTime NextFireTime {
            get;
        }

        public abstract void ProcessingCompleted();

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Fire method Name")]
        public abstract bool Fire();
    }
}
