// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.Model.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests
{
    internal static class TypeExtensions
    {
        public static IEnumerable<Type> GetPublicModelTypes()
        {
            var asmType = typeof(ModelFactory);
            var asm = asmType.Assembly;

            var asmTypes = asm.GetTypes().Where(t => t.IsInNamespace(asmType));


            return asmTypes;
        }

        public static IEnumerable<Type> GetOpenBankingModelTypes()
        {
            var asmType = typeof(PaymentsExtensions);
            var asm = asmType.Assembly;

            var asmTypes = asm.GetTypes().Where(t => t.IsInNamespace(asmType));


            return asmTypes;
        }

        public static Tuple<Type, ConstructorInfo> GetConstructor(this Type type)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            var ctor = type.GetConstructors(flags).FirstOrDefault(c => c.GetParameters().IsEmptyOrDefaults());
            if (ctor != null)
            {
                return Tuple.Create(type, ctor);
            }

            return null;
        }


        public static object CreateInstanceSafe(this ConstructorInfo ctor)
        {
            try
            {
                return ctor.CreateInstance();
            }
            catch (TargetInvocationException)
            {
                return null;
            }
        }

        public static object CreateInstance(this ConstructorInfo ctor)
        {
            object instance;
            var ctorParameters = ctor.GetParameters();

            if (ctorParameters.Length > 0)
            {
                var parameters = new object[ctorParameters.Length];

                for (var x = 0; x < ctorParameters.Length; x++)
                {
                    if (!ctorParameters[x].HasDefaultValue &&
                        ctorParameters[x].ParameterType.GetConstructor() != null)
                    {
                        try
                        {
                            parameters[x] = Activator.CreateInstance(ctorParameters[x].ParameterType);
                        }
                        catch (MissingMethodException)
                        {
                        }
                    }
                }

                instance = ctor.Invoke(parameters);
            }
            else
            {
                instance = ctor.Invoke(null);
            }

            return instance;
        }

        public static object PopulateRequiredFields(this object value)
        {
            var props = value.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
            var pairs = props.Select(p => new
            {
                Prop = p,
                Attr = p.GetCustomAttributes<JsonPropertyAttribute>().FirstOrDefault()
            }).Where(a => a.Attr != null);

            foreach (var pair in pairs)
            {
                if (pair.Attr.Required == Required.Always || pair.Attr.Required == Required.DisallowNull)
                {
                    var prop = pair.Prop.GetSetMethod();
                    if (pair.Prop.PropertyType == typeof(string))
                    {
                        prop.Invoke(value, new[] { "" });
                    }
                    else if (pair.Prop.PropertyType == typeof(Uri))
                    {
                        prop.Invoke(value, new[] { new Uri("http://aaa.com") });
                    }
                    else if (pair.Prop.PropertyType.IsClass)
                    {
                        var v = Activator.CreateInstance(pair.Prop.PropertyType);
                        prop.Invoke(value, new[] { v });
                    }
                }
            }

            return value;
        }

        private static bool IsEmptyOrDefaults(this ParameterInfo[] parameters)
        {
            return parameters.Length == 0 || parameters.All(p => p.HasDefaultValue);
        }

        private static bool HasDefaultValue(this ParameterInfo parameter)
        {
            return parameter.DefaultValue != null;
        }
    }
}
