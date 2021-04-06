﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base.Json
{
    [Flags]
    public enum DelimitedStringConverterOptions
    {
        None = 0,
        JsonStringArrayNotString = 1
    }

    public abstract class
        DelimitedStringGenericConverter<TString> : JsonConverterWithOptions<TString,
            DelimitedStringConverterOptions>
    {
        public DelimitedStringGenericConverter() { }

        public DelimitedStringGenericConverter(DelimitedStringConverterOptions activeOptions) : base(activeOptions) { }
    }

    public class DelimitedStringNullableConverter : DelimitedStringGenericConverter<string?>
    {
        public DelimitedStringNullableConverter() { }

        public DelimitedStringNullableConverter(DelimitedStringConverterOptions activeOptions) : base(activeOptions) { }

        public override void WriteJson(JsonWriter writer, string? value, JsonSerializer serializer)
        {
            DelimitedStringConverterOptions options = GetOptions(serializer);
            if (value is null)
            {
                writer.WriteNull();
            }
            else if (options.HasFlag(DelimitedStringConverterOptions.JsonStringArrayNotString))
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
                if (options.HasFlag(DelimitedStringConverterOptions.JsonStringArrayNotString) &&
                    reader.TokenType == JsonToken.StartArray)
                {
                    StringBuilder builder = new StringBuilder();
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
                else if (!options.HasFlag(DelimitedStringConverterOptions.JsonStringArrayNotString) &&
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
