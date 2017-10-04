﻿using System;
using Diploma.Common.Properties;
using FluentValidation.Validators;

namespace Diploma.Framework.Validations.PropertyValidators
{
    public class BirthDateValidator : PropertyValidator
    {
        private const int DefaultMaximumAge = 131;

        private const int DefaultMinimumAge = 2;

        private readonly DateTime _minimumBirthDate;

        private readonly DateTime _maximumBirthDate;

        public BirthDateValidator()
            : this(DefaultMinimumAge, DefaultMaximumAge)
        {
        }

        public BirthDateValidator(int minimumAge, int maximumAge)
            : base("The '{PropertyName}' is not a valid date of birth. It must be between {From} and {To}. You entered {Value}.")
        {
            if (maximumAge < minimumAge)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumAge), Resources.Validation_BirthDate_Wrong_Maximum_Age);
            }

            _minimumBirthDate = DateTime.Today.AddYears(-maximumAge);
            _maximumBirthDate = DateTime.Today.AddYears(-minimumAge);
        }
        
        protected override bool IsValid(PropertyValidatorContext context)
        {
            var birthDate = (DateTime?)context.PropertyValue;

            if (birthDate == null)
            {
                return true;
            }
            
            if (birthDate >= _minimumBirthDate && birthDate < _maximumBirthDate)
            {
                return true;
            }

            context.MessageFormatter
                .AppendArgument("From", _minimumBirthDate)
                .AppendArgument("To", _maximumBirthDate)
                .AppendArgument("Value", context.PropertyValue);

            return false;
        }
    }
}