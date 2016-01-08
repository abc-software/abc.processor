// ----------------------------------------------------------------------------
// <copyright file="IDateTimeProvider.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor.Utils {
    using System;

    internal interface IDateTimeProvider {
        DateTime Now { get; }
    }
}
