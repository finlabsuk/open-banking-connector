// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Validators
{
    public class NullableValidator<TValidator> : IValidator<TValidator?>
        where TValidator : class
    {
        private readonly IValidator<TValidator> _baseValidator;

        public NullableValidator(IValidator<TValidator> baseValidator)
        {
            _baseValidator = baseValidator;
        }

        public ValidationResult Validate(IValidationContext context) => _baseValidator.Validate(context);

        public async Task<ValidationResult> ValidateAsync(
            IValidationContext context,
            CancellationToken cancellation = new CancellationToken()) =>
            await _baseValidator.ValidateAsync(context, cancellation);

        public IValidatorDescriptor CreateDescriptor() => _baseValidator.CreateDescriptor();

        public bool CanValidateInstancesOfType(Type type) => _baseValidator.CanValidateInstancesOfType(type);

        public ValidationResult Validate(TValidator? instance) => instance is null
            ? throw new InvalidOperationException("FluentValidation should not try to validate null")
            : _baseValidator.Validate(instance);

        public async Task<ValidationResult> ValidateAsync(
            TValidator? instance,
            CancellationToken cancellation = new CancellationToken()) =>
            instance is null
                ? throw new InvalidOperationException("FluentValidation should not try to validate null")
                : await _baseValidator.ValidateAsync(instance, cancellation);
    }
}
