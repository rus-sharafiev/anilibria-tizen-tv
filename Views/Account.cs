using AnilibriaAppTizen.Services;
using System;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class Account
    {
        private readonly UserService _userService;

        private View _accountView;
        private Main _main;
        private View _mainView;
        private bool _isActive = false;

        public bool IsActive { get { return _isActive; } }

        public Account(UserService userService)
        {
            _userService = userService;
        }

        public void RenderTo(Main main)
        {
            _isActive = true;
            _main = main;
            _mainView = main.View;
            _userService.UserChanged += UserService_UserChanged;

            if (_userService.User != null)
                _main.SetTitle("Профиль", "пользователя");
            else
                _main.SetTitle("Авторизация");

            _accountView = new View();
            _mainView.Add(_accountView);
            _accountView.RemovedFromWindow += SettingsView_RemovedFromWindow;
        }

        private void UserService_UserChanged(object sender, EventArgs e)
        {
            if (sender is UserService userService)
                if (userService.User != null)
                {
                    _main.SetTitle("Профиль", "пользователя");
                }
                else
                {
                    _main.SetTitle("Авторизация");
                }
        }

        private void SettingsView_RemovedFromWindow(object sender, EventArgs e)
        {
            _isActive = false;
            _accountView.Dispose();
            _accountView = null;
        }
    }
}
