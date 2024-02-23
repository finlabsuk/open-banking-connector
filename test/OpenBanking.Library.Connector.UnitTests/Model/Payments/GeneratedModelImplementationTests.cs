// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Model.Payments;

public class GeneratedModelImplementationTests
{
    [Fact]
    public void AllObjectsAreEqualToThemselves()
    {
        IEnumerable<Tuple<Type, ConstructorInfo>> modelTypes = TypeExtensions.GetOpenBankingModelTypes()
            .Select(t => t.GetConstructor())
            .Where(t => t != null)!;

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
        IEnumerable<Tuple<Type, ConstructorInfo>> modelTypes = TypeExtensions.GetOpenBankingModelTypes()
            .Select(t => t.GetConstructor())
            .Where(t => t != null)!;

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
        IEnumerable<Tuple<Type, ConstructorInfo>> modelTypes = TypeExtensions.GetOpenBankingModelTypes()
            .Select(t => t.GetConstructor())
            .Where(t => t != null)!;

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
    public void AllValidationObjectsAreValidatable()
    {
        var validationContext = new ValidationContext(new object());

        IEnumerable<Tuple<Type, ConstructorInfo>> modelTypes = TypeExtensions.GetOpenBankingModelTypes()
            .Select(t => t.GetConstructor())
            .Where(t => t != null)!;

        foreach ((Type type, ConstructorInfo ctor) in modelTypes)
        {
            object? instance = ctor.CreateInstanceSafe();
            if (instance != null)
            {
                var validatableObject = instance as IValidatableObject;
                if (validatableObject != null)
                {
                    try
                    {
                        List<ValidationResult> result = validatableObject.Validate(validationContext).ToList();
                    }
                    catch (ArgumentNullException) { }
                }
            }
        }
    }
}
