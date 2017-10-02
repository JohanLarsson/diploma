﻿using Diploma.Common.Properties;
using Diploma.Validators.Properties;
using Diploma.ViewModels;
using FluentValidation;

namespace Diploma.Validators
{
    public class EditUserDataViewModelValidator : AbstractValidator<EditUserDataViewModel>
    {
        public EditUserDataViewModelValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.FirstName).NotEmpty().WithMessage(x => Resources.Editing_FirstName_Can_Not_Be_Empty);

            RuleFor(x => x.LastName).NotEmpty().WithMessage(x => Resources.Editing_LastName_Can_Not_Be_Empty);
            
            RuleFor(x => x.BirthDate).BirthDate().WithMessage(x => Resources.Editing_BirthDate_Must_Be_Be_Valid_Age);
        }
    }
}
