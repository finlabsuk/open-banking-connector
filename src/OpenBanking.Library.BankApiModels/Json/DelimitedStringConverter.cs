// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using Newtonsoft.Json;

#nullable enable

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.Json
{
    [Flags]
    public enum DelimitedStringConverterOptions
    {
        None = 0,
        JsonIsStringArrayNotString = 1
    }

    public abstract class
        DelimitedStringGenericConverter<TString> : JsonConverterWithOptions<TString,
            DelimitedStringConverterOptions>
    {
        public DelimitedStringGenericConverter() { }

        public DelimitedStringGenericConverter(JsonConverterLabel jsonConverterLabel) :
            base(jsonConverterLabel) { }
    }

    public class DelimitedStringNullableConverter : DelimitedStringGenericConverter<string?>
    {
        public DelimitedStringNullableConverter() { }

        public DelimitedStringNullableConverter(JsonConverterLabel jsonConverterLabel) :
            base(jsonConverterLabel) { }


        public override void WriteJson(JsonWriter writer, string? value, JsonSerializer serializer)
        {
            DelimitedStringConverterOptions options = GetOptions(serializer);
            if (value is null)
            {
                writer.WriteNull();
            }
            else if (options.HasFlag(DelimitedStringConverterOptions.JsonIsStringArrayNotString))
            {
                string[] elementList = value.Split(" ");
                writer.WriteStartArray();
                foreach (string element in elementList)
                {
                    writer.WriteValue(element);
                }

                writer.WriteEndArray();
            }
            else
            {
                writer.WriteValue(value);
            }
        }

        public override string? ReadJson(
            JsonReader reader,
            Type objectType,
            string? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (objectType == typeof(string))
            {
                DelimitedStringConverterOptions options = GetOptions(serializer);
                string? output = null;
                if (options.HasFlag(DelimitedStringConverterOptions.JsonIsStringArrayNotString) &&
                    reader.TokenType == JsonToken.StartArray)
                {
                    var builder = new StringBuilder();
                    while (reader.Read() && reader.TokenType == JsonToken.String && !(reader.Value is null))
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append(" ");
                        }

                        builder.Append((string) reader.Value);
                    }

                    output = builder.ToString();
                }
                else if (!options.HasFlag(DelimitedStringConverterOptions.JsonIsStringArrayNotString) &&
                         reader.TokenType == JsonToken.String &&
                         !(reader.Value is null))
                {
                    output = (string) reader.Value;
                }

                return output;
            }

            throw new NotSupportedException($"The type {objectType} is not supported.");
        }
    }
}
