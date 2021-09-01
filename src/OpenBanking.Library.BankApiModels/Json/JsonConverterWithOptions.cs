// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

#nullable enable

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.Json
{
    /// <summary>
    ///     Base class for JSON converter with options and optionally a label. A label allows bank-specific options
    ///     to be used only for specific instances of the converter.
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    /// <typeparam name="TOptionsEnum"></typeparam>
    public abstract class JsonConverterWithOptions<TClass, TOptionsEnum> : JsonConverter<TClass>
        where TOptionsEnum : struct, Enum
    {
        private readonly JsonConverterLabel _jsonConverterLabel;

        protected JsonConverterWithOptions(JsonConverterLabel jsonConverterLabel = JsonConverterLabel.NoLabel)
        {
            _jsonConverterLabel = jsonConverterLabel;
        }

        /// <summary>
        ///     Get options for label from serializer if available. Otherwise return default value (no options).
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected TOptionsEnum GetOptions(JsonSerializer serializer) =>
            _jsonConverterLabel is JsonConverterLabel.NoLabel
                ? default
                : serializer.Context.Context switch
                {
                    null => default,
                    Dictionary<JsonConverterLabel, int> x => (TOptionsEnum) (object)
                        (x.TryGetValue(_jsonConverterLabel, out int value) ? value : default),
                    _ => throw new ArgumentOutOfRangeException()
                };
    }
}
