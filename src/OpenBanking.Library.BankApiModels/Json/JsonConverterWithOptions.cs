// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using Newtonsoft.Json;

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
        private readonly JsonConverterLabel? _jsonConverterLabel;

        protected JsonConverterWithOptions(JsonConverterLabel? jsonConverterLabel)
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
            _jsonConverterLabel switch
            {
                // return default if no label in use
                null => default,
                // return according to serialiser context
                _ => serializer.Context.Context switch
                {
                    null => default,
                    Dictionary<JsonConverterLabel, int> x => (TOptionsEnum) Enum.ToObject(
                        typeof(TOptionsEnum),
                        // return default if label not found in serialiser context
                        x.TryGetValue(_jsonConverterLabel.Value, out int value) ? value : default),
                    _ => throw new ArgumentOutOfRangeException()
                }
            };
    }
}
