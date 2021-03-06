// ----------------------------------------------------------------------------
// <copyright file="ProcessorClient.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor {
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Permissions;

    /// <summary>
    /// Klients pieslēģšānai pie processora vadītāja.
    /// </summary>
    [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.Infrastructure)]
    public class ProcessorClient {
        private string _url;
        private ProcessorManager _manager;

        /// <summary>
        /// Initializē jauno klases <see cref="ProceesorClient"/> instanci.
        /// </summary>
        /// <param name="url">Processora servera adrese.</param>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "Address")]
        public ProcessorClient(string url) {
            _url = url;
        }

        /// <summary>
        /// Atgriž processora servera adresi.
        /// </summary>
        /// <value>Processora servera adrese.</value>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Address")]
        public string Url {
            get { return _url; }
        }

        /// <summary>
        /// Atgriež procesoora vadītāju.
        /// </summary>
        /// <value>Processora vadītāis.</value>
        public ProcessorManager ProcessorManager {
            get {
                if (_manager == null) {
                    _manager = (ProcessorManager)Activator.GetObject(typeof(ProcessorManager), _url);
                }

                return _manager;
            }
        }
    }
}
