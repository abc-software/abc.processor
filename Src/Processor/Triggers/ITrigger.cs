// ----------------------------------------------------------------------------
// <copyright file="ITrigger.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor.Triggers {
    using System;

    /// <summary>
    /// Interfies visiem trigeriem. kuri izmantojas Pieprasijuma servisa.
    /// Trigeri izmantojās procesu palisšānai.
    /// </summary>
    public interface ITrigger {
        /// <summary>
        /// Trigera nosaukums.
        /// </summary>
        string Name {
            get;
            set;
        }

        /// <summary>
        /// Atgriež nākāmo trigera nostrādes laiku.
        /// </summary>
        DateTime NextFireTime {
            get;
        }
        
        /// <summary>
        /// Ziņojam trigeram ka process ir pabeigts.
        /// </summary>
        void ProcessingCompleted();

        /// <summary>
        /// Pārbaudam uz procesa uzsākšanu.
        /// </summary>
        /// <returns>Atgriež <c>True</c> ja processu nepieciešams uzsākt, citādi <c>False</c>.</returns>
        bool Fire();
    }
}
