using AnilibriaAppTizen.Services;
using System;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class Account
    {
        private readonly UserService _userService;

        private View _accountView;
        private MainPage _mainPage;
        private View _mainPageView;
        private bool _isActive = false;

        public bool IsActive { get { return _isActive; } }

        public Account(UserService userService)
        {
            _userService = userService;
        }

        public void RenderTo(MainPage mainPage)
        {
            _isActive = true;
            _mainPage = mainPage;
            _mainPageView = mainPage.View;
            _userService.UserChanged += UserService_UserChanged;

            if (_userService.User != null)
                _mainPage.SetTitle("Профиль", "пользователя");
            else
                _mainPage.SetTitle("Авторизация");

            _accountView = new View();
            _mainPageView.Add(_accountView);
            _accountView.RemovedFromWindow += SettingsView_RemovedFromWindow;
        }

        private void UserService_UserChanged(object sender, EventArgs e)
        {
            if (sender is UserService userService)
                if (userService.User != null)
                {
                    _mainPage.SetTitle("Профиль", "пользователя");
                }
                else
                {
                    _mainPage.SetTitle("Авторизация");
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
