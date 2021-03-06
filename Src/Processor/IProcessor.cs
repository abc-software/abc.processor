// ----------------------------------------------------------------------------
// <copyright file="IProcessor.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor {
    using System.Diagnostics.CodeAnalysis;
    using Abc.Processor.Triggers;

    /// <summary>
    /// Interfeis prosessoriem, kas darbojās pieprasijuma servisa ietvaros.
    /// </summary>
    public interface IProcessor {
        /// <summary>
        /// Atgriež procesora trigeru.
        /// </summary>
        /// <value>Procesora trigers.</value>
        ITrigger Trigger {
            get;
            set;
        }

        /// <summary>
        /// Atgriež procesora nosakumu.
        /// </summary>
        /// <value>Procesora nosaukums.</value>
        string Name {
            get;
            set;
        }

        /// <summary>
        /// Uzsāc procesora izpildīšanu.
        /// </summary>
        void Start();

        /// <summary>
        /// Apstāda procesora izpildīšnau sagaidot tā pabeigšānu.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Leaved for backeard compatibility.")]
        void Stop();

        /// <summary>
        /// Apstādam procesa izpildīšnanu negaidot tā pabeigšānu.
        /// </summary>
        void Abort();
    }
}
