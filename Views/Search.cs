using AnilibriaAppTizen.Services;
using System;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class Search
    {
        private readonly ApiService _apiService;
        private readonly ImageService _imageService;

        private View _searchView;
        private View _mainPageView;
        private bool _isActive = false;

        public bool IsActive { get { return _isActive; } }

        public Search(ApiService apiService, ImageService imageService)
        {
            _apiService = apiService;
            _imageService = imageService;
        }

        public void RenderTo(MainPage mainPage, Release release)
        {
            _isActive = true;
            _mainPageView = mainPage.View;
            mainPage.SetTitle("Поиск");

            _searchView = new View();
            _mainPageView.Add(_searchView);
            _searchView.RemovedFromWindow += SettingsView_RemovedFromWindow;
        }

        private void SettingsView_RemovedFromWindow(object sender, EventArgs e)
        {
            _isActive = false;
            _searchView.Dispose();
            _searchView = null;
        }
    }
}
