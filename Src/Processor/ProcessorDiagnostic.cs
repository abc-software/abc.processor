// ----------------------------------------------------------------------------
// <copyright file="ProcessorDiagnostic.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor {
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Threading;

    /// <summary>
    /// Processor diagnostic class.
    /// </summary>
    public static class ProcessorDiagnostic {
        private const string TraceSourceName = "Abc.Processor";

        private static TraceSource _ts = new TraceSource(TraceSourceName);

        /// <summary>
        /// Gets the trace source.
        /// </summary>
        /// <value>The trace source <see cref="TraceSource"/>.</value>
        public static TraceSource TraceSource {
            get {
                return _ts;
            }
        }

        /// <summary>
        /// Gets or sets the trace source switch.
        /// </summary>
        /// <value>The trace source switch <see cref="SourceSwitch"/>.</value>
        public static SourceSwitch Switch {
            get {
                return _ts.Switch; 
            }

            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            set {
                _ts.Switch = value;
            }
        }

        /// <summary>
        /// Determines whether the specified exception is fatal.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        ///     <c>true</c> if the specified exception is fatal; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFatal(Exception exception) {
            while (exception != null) {
                if ((exception is OutOfMemoryException && !(exception is InsufficientMemoryException)) ||
                    ((exception is ThreadAbortException || exception is AccessViolationException) ||
                    exception is SEHException)) {
                    return true;
                }

                if (!(exception is TypeInitializationException) && !(exception is TargetInvocationException)) {
                    break;
                }

                exception = exception.InnerException;
            }

            return false;
        }
    }
}
