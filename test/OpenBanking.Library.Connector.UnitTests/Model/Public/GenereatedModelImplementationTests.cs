// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Model.Public;

public class GenereatedModelImplementationTests
{
    [Fact]
    public void AllObjectsAreEqualToThemselves()
    {
        IEnumerable<Tuple<Type, ConstructorInfo>> modelTypes = TypeExtensions.GetPublicModelTypes()
            .Select(t => t.GetConstructor())
            .Where(t => t != null)
            .Select(t => t!);

        foreach ((Type type, ConstructorInfo ctor) in modelTypes)
        {
            object? instance = ctor.CreateInstanceSafe();
            if (instance != null)
            {
                bool r = instance.Equals(instance);
            }
        }
    }

    [Fact]
    public void AllObjectsHaveToString()
    {
        IEnumerable<Tuple<Type, ConstructorInfo>> modelTypes = TypeExtensions.GetPublicModelTypes()
            .Select(t => t.GetConstructor())
            .Where(t => t != null)
            .Select(t => t!);

        foreach ((Type type, ConstructorInfo ctor) in modelTypes)
        {
            object? instance = ctor.CreateInstanceSafe();
            if (instance != null)
            {
                var r = instance.ToString();

                r.Should().NotBeNullOrWhiteSpace();
            }
        }
    }


    [Fact]
    public void AllObjectsHaveGetHashCode()
    {
        IEnumerable<Tuple<Type, ConstructorInfo>> modelTypes = TypeExtensions.GetPublicModelTypes()
            .Select(t => t.GetConstructor())
            .Where(t => t != null)
            .Select(t => t!);

        foreach ((Type type, ConstructorInfo ctor) in modelTypes)
        {
            object? instance = ctor.CreateInstanceSafe();
            if (instance != null)
            {
                int r = instance.GetHashCode();
            }
        }
    }


    [Fact]
    public void AllObjectsAreSerialisable()
    {
        IEnumerable<Tuple<Type, ConstructorInfo>> modelTypes = TypeExtensions.GetPublicModelTypes()
            .Select(t => t.GetConstructor())
            .Where(t => t != null)
            .Select(t => t!);

        foreach ((Type type, ConstructorInfo ctor) in modelTypes)
        {
            object? instance = ctor.CreateInstanceSafe();
            if (instance != null)
            {
                instance = instance.PopulateRequiredFields();

                string r = JsonConvert.SerializeObject(instance);

                object? r2 = JsonConvert.DeserializeObject(r, type);
            }
        }
    }
}
