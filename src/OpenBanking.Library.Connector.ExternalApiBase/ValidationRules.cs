// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.RegularExpressions;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.ExternalApiBase
{
    public static class ValidationRules
    {
        public static bool HasLengthAtLeast<T>(T arg1, string arg2, ValidationContext<T> arg3, int minLength)
        {
            return arg2.Length >= minLength;
        }

        public static bool HasLengthAtMost<T>(T arg1, string? arg2, ValidationContext<T> arg3, int maxLength)
        {
            return arg2 != null && arg2.Length <= maxLength;
        }

        public static bool IsNotNull<T, T2>(T arg1, T2? arg2, ValidationContext<T> arg3)
            where T2 : class
        {
            return arg2 != null;
        }

        public static bool IsNotNullOrEmpty<T>(T arg1, string? arg2, ValidationContext<T> arg3)
        {
            return !string.IsNullOrEmpty(arg2);
        }

        public static bool IsNonWhitespace<T>(T arg1, string? arg2, ValidationContext<T> arg3)
        {
            return !string.IsNullOrWhiteSpace(arg2);
        }

        public static bool IsUrl<T>(T arg1, string? arg2, ValidationContext<T> arg3)
        {
            return arg2 != null && Uri.TryCreate(arg2, UriKind.Absolute, out _);
        }

        public static bool IsAbsoluteUrl<T>(T arg1, Uri arg2, ValidationContext<T> arg3)
        {
            return arg2 != null && arg2.IsAbsoluteUri;
        }

        public static bool IsMatch<T>(T arg1, string? arg2, ValidationContext<T> arg3, Regex regex)
        {
            return arg2 != null && regex.Match(arg2).Success;
        }

        public static bool IsIsoCurrencyCode<T>(T arg1, string? arg2, ValidationContext<T> arg3)
        {
            return !string.IsNullOrWhiteSpace(arg2);
        }

        public static bool IsNonZeroPositive<T>(T arg1, double arg2, ValidationContext<T> arg3)
        {
            return arg2 > 0;
        }
    }
}
