// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class LocalReadParams
    {
        public LocalReadParams(Guid id, string? modifiedBy)
        {
            Id = id;
            ModifiedBy = modifiedBy;
        }

        public Guid Id { get; }
        public string? ModifiedBy { get; }
    }

    internal class BankRegistrationReadParams : LocalReadParams
    {
        public BankRegistrationReadParams(
            Guid id,
            string? modifiedBy,
            bool? includeExternalApiOperation,
            BankProfileEnum? bankProfileEnum) : base(id, modifiedBy)
        {
            IncludeExternalApiOperation = includeExternalApiOperation;
            BankProfileEnum = bankProfileEnum;
        }

        public bool? IncludeExternalApiOperation { get; }
        public BankProfileEnum? BankProfileEnum { get; }
    }

    internal class ConsentReadParams : LocalReadParams
    {
        public ConsentReadParams(Guid id, string? modifiedBy, bool includeExternalApiOperation) : base(id, modifiedBy)
        {
            IncludeExternalApiOperation = includeExternalApiOperation;
        }

        public bool IncludeExternalApiOperation { get; }
    }

    internal interface IObjectRead<TPublicResponse, in TReadParams>
        where TReadParams : LocalReadParams
    {
        Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ReadAsync(
            TReadParams readParams);
    }

    internal interface
        IObjectReadWithSearch<TPublicQuery, TPublicResponse, in TReadParams> : IObjectRead<TPublicResponse, TReadParams>
        where TReadParams : LocalReadParams
    {
        Task<(IQueryable<TPublicResponse> response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages
                )>
            ReadAsync(Expression<Func<TPublicQuery, bool>> predicate);
    }
}
