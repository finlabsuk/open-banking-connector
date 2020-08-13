// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.RegularExpressions;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FluentValidation.Validators;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validation
{
    public static class ValidationRules
    {
        internal static bool HasLengthAtLeast<T>(T arg1, string arg2, PropertyValidatorContext arg3, int minLength)
        {
            return arg2.Length >= minLength;
        }

        internal static bool HasLengthAtMost<T>(T arg1, string arg2, PropertyValidatorContext arg3, int maxLength)
        {
            return arg2 != null && arg2.Length <= maxLength;
        }

        internal static bool IsNotNull<T, T2>(T arg1, T2 arg2, PropertyValidatorContext arg3)
            where T2 : class
        {
            return arg2 != null;
        }

        internal static bool IsNotNullOrEmpty<T>(T arg1, string arg2, PropertyValidatorContext arg3)
        {
            return !string.IsNullOrEmpty(arg2);
        }

        internal static bool IsNonWhitespace<T>(T arg1, string arg2, PropertyValidatorContext arg3)
        {
            return !string.IsNullOrWhiteSpace(arg2);
        }

        internal static bool IsUrl<T>(T arg1, string arg2, PropertyValidatorContext arg3)
        {
            return arg2 != null && Uri.TryCreate(uriString: arg2, uriKind: UriKind.Absolute, result: out _);
        }

        internal static bool IsAbsoluteUrl<T>(T arg1, Uri arg2, PropertyValidatorContext arg3)
        {
            return arg2 != null && arg2.IsAbsoluteUri;
        }

        internal static bool IsMatch<T>(T arg1, string arg2, PropertyValidatorContext arg3, Regex regex)
        {
            return arg2 != null && regex.Match(arg2).Success;
        }

        internal static bool HasDelimiters<T>(
            T arg1,
            string arg2,
            PropertyValidatorContext arg3,
            char delimiter,
            int maxLength)
        {
            return arg2 != null && arg2.DelimiterCount(delimiter) == maxLength;
        }

        internal static bool IsIsoCurrencyCode<T>(T arg1, string arg2, PropertyValidatorContext arg3)
        {
            return !string.IsNullOrWhiteSpace(arg2);
        }

        internal static bool IsNonZeroPositive<T>(T arg1, double arg2, PropertyValidatorContext arg3)
        {
            return arg2 > 0;
        }
    }
}
