using AnilibriaAppTizen.Services;
using System;
using System.Diagnostics;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class Home
    {
        private readonly ApiService _apiService;
        private readonly ImageService _imageService;

        private View _homeView;
        private View _mainView;
        private bool _isActive = false;

        public bool IsActive { get { return _isActive; } }

        public Home(ApiService apiService, ImageService imageService)
        {
            _apiService = apiService;
            _imageService = imageService;
        }

        public void RenderTo(Main main, Release release, bool updateData = false)
        {
            _isActive = true;
            _mainView = main.View;
            main.SetTitle("Главная");

            _homeView = new View()
            {
            };
            _mainView.Add(_homeView);
            _homeView.RemovedFromWindow += HomeView_RemovedFromWindow;
        }

        private void HomeView_RemovedFromWindow(object sender, EventArgs e)
        {
            _isActive = false;
            _homeView.Dispose();
            _homeView = null;
        }
    }
}
