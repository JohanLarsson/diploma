﻿using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Diploma.BLL.Queries.Requests;
using Diploma.BLL.Services.Interfaces;
using Diploma.Framework.Validations;
using Diploma.Properties;
using Diploma.ViewModels;
using FluentValidation;

namespace Diploma.Validators
{
    internal sealed class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        private const int MaximumUsernameCharCount = 30;

        private const int MinimumUsernameCharCount = 5;

        private static readonly Regex UsernameCharCheck = new Regex("^[a-zA-Z0-9_.-]*$", RegexOptions.Compiled);

        private readonly IUserService _userService;

        public RegisterViewModelValidator(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.FirstName).NotEmpty().WithMessage(x => Resources.Validation_Registration_FirstName_Can_Not_Be_Empty);

            RuleFor(x => x.LastName).NotEmpty().WithMessage(x => Resources.Validation_Registration_LastName_Can_Not_Be_Empty);

            RuleFor(x => x.Username).NotEmpty().WithMessage(x => Resources.Validation_Registration_Username_Can_Not_Be_Empty)
                .Length(MinimumUsernameCharCount, MaximumUsernameCharCount)
                .WithMessage(x => Resources.Validation_Registration_Username_Invalid_Length).Matches(UsernameCharCheck)
                .WithMessage(x => Resources.Validation_Registration_Username_Contains_Invalid_Characters).MustAsync(BeUniqueUserNameAsync)
                .WithMessage(x => Resources.Validation_Registration_Username_Already_Taken);

            RuleFor(x => x.BirthDate).BirthDate().WithMessage(x => Resources.Validation_Registration_BirthDate_Must_Be_Be_Valid_Age);

            RuleFor(x => x.Password).NotEmpty().WithMessage(x => Resources.Validation_Registration_Password_Can_Not_Be_Empty);

            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(x => Resources.Validation_Registration_ConfirmPassword_Can_Not_Be_Empty)
                .Equal(customer => customer.Password).WithMessage(x => Resources.Validation_Registration_ConfirmPassword_Not_Match_Password);
        }

        private async Task<bool> BeUniqueUserNameAsync(string username, CancellationToken cancellationToken)
        {
            var request = new VerifyUsernameUniqueRequest { Username = username };

            var result = await _userService.IsUsernameUniqueAsync(request, cancellationToken).ConfigureAwait(false);

            return result.Data;
        }
    }
}
