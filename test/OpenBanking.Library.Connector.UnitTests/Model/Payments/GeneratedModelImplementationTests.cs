// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Model.Payments
{
    public class GeneratedModelImplementationTests
    {
        [Fact]
        public void AllObjectsAreEqualToThemselves()
        {
            IEnumerable<Tuple<Type, ConstructorInfo>> modelTypes = TypeExtensions.GetOpenBankingModelTypes()
                .Select(t => t.GetConstructor())
                .Where(t => t != null)!;

            foreach (var (type, ctor) in modelTypes)
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

            foreach (var (type, ctor) in modelTypes)
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

            foreach (var (type, ctor) in modelTypes)
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
            IEnumerable<Tuple<Type, ConstructorInfo>> modelTypes = TypeExtensions.GetOpenBankingModelTypes()
                .Where(t => !t.GetTypeInfo().IsDefined(typeof(CompilerGeneratedAttribute), true))
                .Select(t => t.GetConstructor())
                .Where(t => t != null)!;

            foreach (var (type, ctor) in modelTypes)
            {
                object? instance = ctor.CreateInstanceSafe();
                if (instance != null)
                {
                    string r = JsonConvert.SerializeObject(instance);

                    object? r2 = JsonConvert.DeserializeObject(r, type);
                }
            }
        }

        [Fact]
        public void AllValidationObjectsAreValidatable()
        {
            ValidationContext validationContext = new ValidationContext(new object());

            IEnumerable<Tuple<Type, ConstructorInfo>> modelTypes = TypeExtensions.GetOpenBankingModelTypes()
                .Select(t => t.GetConstructor())
                .Where(t => t != null)!;

            foreach (var (type, ctor) in modelTypes)
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
}
