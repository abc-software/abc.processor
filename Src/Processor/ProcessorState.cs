// ----------------------------------------------------------------------------
// <copyright file="ProcessorState.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor {
    /// <summary>
    /// Iepējamie processora statusi.
    /// </summary>
    public enum ProcessorState {
        /// <summary>
        /// Processors is palaists.  
        /// </summary>
        Running,

        /// <summary>
        /// Processors tiks apstādīnāts, kad pabiegs strādājošo processu.
        /// </summary>
        Stopping,

        /// <summary>
        /// Processors apstādīnāts.
        /// </summary>
        Stopped 
    }
}
