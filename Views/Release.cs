using AnilibriaAppTizen.Models;
using AnilibriaAppTizen.Services;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class Release
    {
        private ApiService _apiService;
        private View _releaseView;
        private View _activeView;

        public Release(ApiService apiService) 
        {
            _apiService = apiService;
        }

        public void Initialize()
        {
            _releaseView = new View
            {
                Size = Window.Instance.Size,
            };

            Window.Instance.Add(_releaseView);
        }

        public void Render(Models.Release release)
        {

        }

        public void Add(View poster, Position position)
        {
            _activeView = poster;
            _activeView.Position = position;
            _releaseView.Add(_activeView);
        }

        public void Dispose()
        {
            _releaseView.Dispose();
        }
    }
}
