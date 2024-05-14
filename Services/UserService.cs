using AnilibriaAppTizen.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnilibriaAppTizen.Services
{
    internal class UserService
    {
        private readonly ApiService _apiService;
        private readonly LocalSettingsService _localSettingsService;
        private const string settingsKey = "accessToken";
        private string accessToken { get; set; }

        public User User { get; set; }
        public event EventHandler UserChanged;

        public bool LoginError { get; set; }
        public event EventHandler LoginErrorChanged;

        public UserService(LocalSettingsService localSettingsService, ApiService apiService)
        {
            _apiService = apiService;
            _localSettingsService = localSettingsService;
        }

        public async Task InitializeAsync()
        {
            accessToken = await LoadAccessTokenFromSettingsAsync();
            if (accessToken != null)
            {
                try
                {
                    User = await _apiService.GetUserAsync(accessToken);
                    UserChanged?.Invoke(this, new EventArgs());
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("User fetch error" + ex);
                }

            }
            await Task.CompletedTask;
        }

        public async void Login(string login, string password)
        {
            SignUpDto signUpDto = new SignUpDto()
            {
                Login = login,
                Password = password
            };

            var response = await _apiService.SignUpAsync(signUpDto);

            if (response?.AccessToken != null)
            {
                accessToken = response.AccessToken;
                await SaveAccessTokenInSettingsAsync(accessToken);

                try
                {
                    User = await _apiService.GetUserAsync(accessToken);
                    UserChanged?.Invoke(this, new EventArgs());

                    LoginError = false;
                    LoginErrorChanged?.Invoke(this, new EventArgs());
                }
                catch (Exception)
                {
                    LoginError = true;
                    LoginErrorChanged?.Invoke(this, new EventArgs());
                }
            }
            else
            {
                LoginError = true;
                LoginErrorChanged?.Invoke(this, new EventArgs());
            }
        }

        public async void LogOut()
        {
            User = null;
            accessToken = null;
            await RemoveAccessTokenFromSettingsAsync();
            UserChanged?.Invoke(this, new EventArgs());
            _apiService.RemoveCookies();
        }

        public string GetAccessToken()
        {
            return accessToken;
        }

        private async Task<string> LoadAccessTokenFromSettingsAsync()
        {
            return await _localSettingsService.ReadSettingAsync<string>(settingsKey);
        }

        private async Task SaveAccessTokenInSettingsAsync(string accessToken)
        {
            await _localSettingsService.SaveSettingAsync(settingsKey, accessToken);
        }

        private async Task RemoveAccessTokenFromSettingsAsync()
        {
            await _localSettingsService.SaveSettingAsync(settingsKey, string.Empty);
        }
    }
}