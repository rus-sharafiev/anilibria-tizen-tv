using AnilibriaAppTizen.Services;
using System;
using Tizen.Multimedia;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.TV.Multimedia;

namespace AnilibriaAppTizen.Views
{
    internal class Player
    {
        private View _playerView;
        private VideoView _videoView;
        private bool _isActive = false;
        private Episode _episode;
        private Tizen.TV.Multimedia.Player _player;

        private Release _release;
        private Main _main;

        public bool IsActive { get { return _isActive; } }
        public Tizen.Multimedia.Player Instance {  get { return _player; } } 

        public Player()
        {
        }

        public void Initialize(Release release, Main main)
        {
            _release = release;
            _main = main;

            _playerView = new View
            {
                Size = Window.Instance.Size,
                DrawMode = DrawModeType.Overlay2D,
            };

            Window.Instance.Add(_playerView);
        }

        public async void OpenVideo(Uri url, Episode episode)
        {
            _isActive = true;
            _release.Hide();
            _main.Hide();

            _player = new Tizen.TV.Multimedia.Player();
            _player.SetSource(new MediaUriSource(url.ToString()));
            _player.Display = new Display(Window.Instance);
            await _player.PrepareAsync();
            _player.Start();
        }

        public void Dispose()
        {
            _release.Show();
            _main.Show();

            _player.Stop();
            _player.Unprepare();
            _player.Dispose();
            _player = null;

            //FocusManager.Instance.SetCurrentFocusView(_episode.View);
            _isActive = false;
            _playerView.ClearBackground();
        }
    }
}
