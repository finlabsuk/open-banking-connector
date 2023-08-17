// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AutoMapper;

namespace FinnovationLabs.OpenBanking.Library.Connector.Converters;

public class StringToIListConverter : ITypeConverter<string, IList<string>>
{
    public IList<string> Convert(
        string source,
        IList<string> destination,
        ResolutionContext context) =>
        source.Split(" ").ToList();
}

public class StringToIEnumerableReverseConverter : ITypeConverter<IEnumerable<string>, string>
{
    public string Convert(
        IEnumerable<string> source,
        string destination,
        ResolutionContext context) =>
        string.Join(" ", source);
}

public class CommaDelimitedStringToIEnumerableValueConverter : IValueConverter<string, IEnumerable<string>>
{
    public IEnumerable<string> Convert(string sourceMember, ResolutionContext context) =>
        sourceMember.Split(", ").ToList();
}

public class CommaDelimitedStringToIEnumerableReverseValueConverter : IValueConverter<IEnumerable<string>, string>
{
    public string Convert(IEnumerable<string> sourceMember, ResolutionContext context) =>
        string.Join(", ", sourceMember);
}
