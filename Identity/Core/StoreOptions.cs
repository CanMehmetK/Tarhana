// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.CustomIdentity
{
    /// <summary>
    /// Used for store specific options
    /// </summary>
    public class StoreOptions
    {
        /// <summary>
        /// If set to a positive number, the default OnModelCreating will use this value as the max length for any 
        /// properties used as keys, i.e. UserId, LoginProvider, ProviderKey.
        /// </summary>
        public int MaxLengthForKeys { get; set; }
    }
}