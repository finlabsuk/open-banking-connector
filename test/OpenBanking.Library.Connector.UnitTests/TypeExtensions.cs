// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using Newtonsoft.Json;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests;

internal static class TypeExtensions
{
    public static IEnumerable<Type> GetPublicModelTypes()
    {
        Type asmType = typeof(BaseResponse);
        Assembly asm = asmType.Assembly;

        IEnumerable<Type> asmTypes = asm.GetTypes().Where(t => t.IsInNamespace(asmType));


        return asmTypes;
    }

    public static IEnumerable<Type> GetOpenBankingModelTypes()
    {
        Type asmType = typeof(PaymentInitiationModelsPublic.Meta);
        Assembly asm = asmType.Assembly;

        IEnumerable<Type> asmTypes = asm.GetTypes().Where(t => t.IsInNamespace(asmType));


        return asmTypes;
    }

    public static Tuple<Type, ConstructorInfo>? GetConstructor(this Type type)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        ConstructorInfo? ctor = type.GetConstructors(flags)
            .FirstOrDefault(c => c.GetParameters().IsEmptyOrDefaults());
        if (ctor != null)
        {
            return Tuple.Create(type, ctor);
        }

        return null;
    }


    public static object? CreateInstanceSafe(this ConstructorInfo ctor)
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
        ParameterInfo[] ctorParameters = ctor.GetParameters();

        if (ctorParameters.Length > 0)
        {
            object?[] parameters = new object[ctorParameters.Length];

            for (var x = 0; x < ctorParameters.Length; x++)
            {
                if (!ctorParameters[x].HasDefaultValue &&
                    ctorParameters[x].ParameterType.GetConstructor() != null)
                {
                    try
                    {
                        parameters[x] = Activator.CreateInstance(ctorParameters[x].ParameterType);
                    }
                    catch (MissingMethodException) { }
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
        PropertyInfo[] props = value.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
        var pairs = props
            .Select(
                p => new
                {
                    Prop = p,
                    Attr = p.GetCustomAttributes<JsonPropertyAttribute>().FirstOrDefault()
                })
            .Where(a => a.Attr is not null);

        foreach (var pair in pairs)
        {
            if (pair.Attr!.Required == Required.Always ||
                pair.Attr.Required == Required.DisallowNull)
            {
                MethodInfo? prop = pair.Prop.GetSetMethod();
                if (pair.Prop.PropertyType == typeof(string))
                {
                    prop!.Invoke(value, new object?[] { "" });
                }
                else if (pair.Prop.PropertyType == typeof(Uri))
                {
                    prop!.Invoke(value, new object?[] { new Uri("https://aaa.com") });
                }
                else if (pair.Prop.PropertyType.IsClass)
                {
                    object? v = Activator.CreateInstance(pair.Prop.PropertyType);
                    prop!.Invoke(value, new[] { v });
                }
            }
        }

        return value;
    }

    private static bool IsEmptyOrDefaults(this ParameterInfo[] parameters)
    {
        return parameters.Length == 0 || parameters.All(p => p.HasDefaultValue);
    }

    private static bool HasDefaultValue(this ParameterInfo parameter) => parameter.DefaultValue != null;
}
