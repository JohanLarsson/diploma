﻿using System;
using System.Threading;
using Caliburn.Micro;
using Diploma.BLL.DTO;
using Diploma.BLL.Interfaces.Services;
using Diploma.Services.Interfaces;
using Diploma.Views;
using MaterialDesignThemes.Wpf;

namespace Diploma.ViewModels
{
    public sealed class DashboardViewModel : Screen
    {
        private readonly IMessageService _messageService;

        private readonly IUserService _userService;

        public DashboardViewModel(IMessageService messageService, IUserService userService)
        {
            _messageService = messageService;
            _userService = userService;
        }

        public UserDto CurrentUser { get; private set; }

        public async void Edit()
        {
            var viewModel = IoC.Get<EditUserDataViewModel>();
            viewModel.LastName = CurrentUser.LastName;
            viewModel.FirstName = CurrentUser.FirstName;
            viewModel.MiddleName = CurrentUser.MiddleName;
            viewModel.Gender = CurrentUser.Gender;
            viewModel.BirthDate = CurrentUser.BirthDate;

            var view = IoC.Get<EditUserDataView>();
            view.DataContext = viewModel;

            var result = await DialogHost.Show(view);
            if (!(result is bool) || !(bool)result)
            {
                return;
            }

            var validationResult = viewModel.Validate();
            if (validationResult)
            {
                _messageService.ShowMessage("There were problems saving your personal info. Check input and try again.");
                return;
            }

            var userUpdateRequestDataDto = new UserUpdateRequestDataDto
            {
                Id = CurrentUser.Id,
                LastName = viewModel.LastName,
                FirstName = viewModel.FirstName,
                MiddleName = viewModel.MiddleName,
                Gender = viewModel.Gender,
                BirthDate = viewModel.BirthDate
            };

            var operation = await _userService.UpdateUserAsync(userUpdateRequestDataDto);

            if (!operation.Success)
            {
                _messageService.ShowMessage(operation.NonSuccessMessage);
                return;
            }

            CurrentUser = operation.Result;
        }

        public void Init(UserDto currentUser)
        {
            CurrentUser = currentUser;
            _messageService.ShowMessage($"Hello, {CurrentUser.Username}");
            DisplayName = "Dashboard";
        }

        public void Logout()
        {
            _messageService.ShowMessage($"Goodbye, {CurrentUser.Username}");
            Thread.CurrentPrincipal = null;
            CurrentUser = null;
            ((ShellViewModel)Parent).ActiveItem = IoC.Get<LoginViewModel>();
        }
    }
}
