// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    /// <summary>
    ///     Base request for any entity.
    /// </summary>
    public abstract class Base
    {
        /// <summary>
        ///     Optional friendly name for object in local database to support debugging etc. The name must be unique (i.e. not
        ///     already in use).
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        ///     Optional "created by" string in local database. Similar to "modified by" for mutable fields in local database, this
        ///     field
        ///     can be used to denote authorship.
        /// </summary>
        public string? CreatedBy { get; set; }
        
        /// <summary>
        ///     Optional reference for linking object to something else - e.g. a user ID in the client
        ///     application. This field is not used by Open Banking Connector.
        /// </summary>
        public string? Reference { get; set; }

    }
}
