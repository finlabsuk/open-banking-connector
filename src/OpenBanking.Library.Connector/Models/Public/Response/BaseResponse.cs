﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public interface IBaseQuery
    {
        public Guid Id { get; }

        public string? Name { get; }

        public DateTimeOffset Created { get; }

        public string? CreatedBy { get; }

        public string? Reference { get; }
    }

    /// <summary>
    ///     Base response for any entity.
    /// </summary>
    public abstract class BaseResponse : IBaseQuery
    {
        internal BaseResponse(Guid id, string? name, DateTimeOffset created, string? createdBy)
        {
            Id = id;
            Name = name;
            Created = created;
            CreatedBy = createdBy;
        }

        /// <summary>
        ///     Optional list of warning messages from Open Banking Connector.
        /// </summary>
        public IList<string>? Warnings { get; set; }

        /// <summary>
        ///     Unique Open Banking Connector ID (used in local database).
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        ///     Optional friendly name for object in local database to support debugging etc. The name must be unique (i.e. not
        ///     already in use).
        /// </summary>
        public string? Name { get; }

        /// <summary>
        ///     Created timestamp in local database.
        /// </summary>
        public DateTimeOffset Created { get; }

        /// <summary>
        ///     Optional "created by" string in local database. Similar to "modified by" for mutable fields in local database, this
        ///     field
        ///     can be used to denote authorship.
        /// </summary>
        public string? CreatedBy { get; }

        /// <summary>
        ///     Optional reference for linking object to something else - e.g. a user ID in the client
        ///     application. This field is not used by Open Banking Connector.
        /// </summary>
        public string? Reference { get; }
    }
}
