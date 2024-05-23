using AnilibriaAppTizen.Services;
using System;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class Settings
    {
        private readonly ApiService _apiService;
        private readonly ImageService _imageService;

        private View _settingsView;
        private View _mainView;
        private bool _isActive = false;

        public bool IsActive { get { return _isActive; } }

        public Settings(ApiService apiService, ImageService imageService)
        {
            _apiService = apiService;
            _imageService = imageService;
        }

        public void RenderTo(Main main)
        {
            _isActive = true;
            _mainView = main.View;
            main.SetTitle("Настройки");

            _settingsView = new TableView()
            {
            };
            _mainView.Add(_settingsView);
            _settingsView.RemovedFromWindow += SettingsView_RemovedFromWindow;
        }

        private void SettingsView_RemovedFromWindow(object sender, EventArgs e)
        {
            _isActive = false;
            _settingsView.Dispose();
            _settingsView = null;
        }
    }
}
