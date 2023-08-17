// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AutoMapper;

namespace FinnovationLabs.OpenBanking.Library.Connector.Converters;

public class IdentityValueConverter<TValue> : IValueConverter<TValue, TValue>
{
    public TValue Convert(TValue sourceMember, ResolutionContext context) => sourceMember;
}
