// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FinnovationLabs.OpenBanking.Library.Connector
{
    internal static class AssertExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static T ArgNotNull<T>(this T value, string argName)
            where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(argName);
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static T ArgNotNullElseInvalidOp<T>(this T value, string message) where T : class
        {
            return value ?? throw new InvalidOperationException(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static T ArgStructNotNullElseInvalidOp<T>(this T? value, string message) where T : struct
        {
            return value ?? throw new InvalidOperationException(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static T InvalidOpOnNull<T>(this T value, string message)
            where T : class
        {
            if (value == null)
            {
                throw new InvalidOperationException(message);
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerStepThrough]
        public static T IsInRange<T>(this T value, Func<T, bool> inRange, string argName)
        {
            if (!inRange(value))
            {
                throw new ArgumentOutOfRangeException(argName);
            }

            return value;
        }
    }
}
