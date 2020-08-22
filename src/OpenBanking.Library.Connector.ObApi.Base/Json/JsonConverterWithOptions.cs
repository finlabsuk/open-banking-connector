// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.ObApi.Base.Json
{
    public abstract class JsonConverterWithOptions<TClass, TOptionsEnum> : JsonConverter<TClass>
        where TOptionsEnum : struct, Enum
    {
        protected JsonConverterWithOptions(TOptionsEnum activeOptions = default)
        {
            ActiveOptions = activeOptions;
        }

        protected TOptionsEnum ActiveOptions { get; }

        protected TOptionsEnum getOptions(JsonSerializer serializer)
        {
            List<string>? contextOptions = serializer.Context.Context as List<string>;
            if (contextOptions is null || ActiveOptions.Equals(default(TOptionsEnum)))
            {
                return default;
            }

            int options = 0;
            Array values = Enum.GetValues(typeof(TOptionsEnum));
            foreach (TOptionsEnum item in values)
            {
                bool optionActive = ActiveOptions.HasFlag(item);
                bool optionSelected = contextOptions.Contains($"{typeof(TOptionsEnum).Name}:{item.ToString()}");
                if (optionActive && optionSelected)
                {
                    options |= (int) (object) item;
                }
            }

            return (TOptionsEnum) (object) options;
        }
    }
}
