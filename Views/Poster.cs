using AnilibriaAppTizen.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tizen.Applications;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class Poster
    {
        private readonly Size _posterSize;
        private readonly ImageService _imageService;

        private readonly Release _releasePage;
        private VisualView _posterView;
        private ImageVisual _imageVisual;
        private readonly Models.Release _releaseData;
        private readonly AlphaFunction _easeOut;

        private View[] _parentContainers = Array.Empty<View>();

        public event EventHandler FocusGained;
        public event EventHandler Activated;

        public VisualView View { get => _posterView; }
        public Size PosterSize { get => _posterSize; }
        public View[] ParentContainers { set => _parentContainers = value; }

        public Poster(Models.Release releaseData, Main main)
        {
            _releasePage = main.ReleasePage;
            _imageService = main.ImageService;
            _releaseData = releaseData;
            _posterSize = new Size(main.PosterWidth, main.PosterHeight);
            _easeOut = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseOutSquare);

            _ = InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            _posterView = new VisualView()
            {
                Size = _posterSize,
                Focusable = true,
                Opacity = 0.7f
            };
            _posterView.FocusGained += PosterView_FocusGained;
            _posterView.FocusLost += PosterView_FocusLost;
            _posterView.KeyEvent += PosterView_KeyEvent;

            var url = _releaseData.Poster.Src;

            _imageVisual = new ImageVisual
            {
                URL = await _imageService.GetPath(url),
                //URL = Application.Current.DirectoryInfo.SharedResource + "images/poster.jpg",
                FittingMode = FittingModeType.ScaleToFill,
                AlphaMaskURL = Application.Current.DirectoryInfo.SharedResource + "images/alphaMask.png",
                DesiredHeight = 500,
                DesiredWidth = 350,
            };
            _posterView.AddVisual(url, _imageVisual);
        }

        private bool PosterView_KeyEvent(object source, View.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down && e.Key.KeyPressedName == "Return")
            {
                var posX = _posterView.PositionX - _posterView.SizeWidth * 0.1f;
                var posY = _posterView.PositionY - _posterView.SizeHeight * 0.1f;

                foreach (var container in _parentContainers)
                {
                    posX += container.PositionX;
                    posY += container.PositionY;
                }

                _releasePage.Render(_posterView, new Position(posX, posY), _releaseData);
                Activated?.Invoke(this, new EventArgs());
            }
            return false;
        }

        private void PosterView_FocusGained(object sender, System.EventArgs e)
        {
            _posterView.DrawMode = DrawModeType.Overlay2D;

            var scaleAnimation = new Animation(140);
            scaleAnimation.AnimateTo(_posterView, "Opacity", 1.0f, _easeOut);
            scaleAnimation.AnimateTo(_posterView, "ScaleX", 1.2f, _easeOut);
            scaleAnimation.AnimateTo(_posterView, "ScaleY", 1.2f, _easeOut);
            scaleAnimation.Play();
            scaleAnimation.Finished += (a, ev) =>
            {
                if (a is Animation ani) ani.Dispose();
            };

            FocusGained?.Invoke(this, new EventArgs());
        }

        private void PosterView_FocusLost(object sender, System.EventArgs e)
        {
            _posterView.DrawMode = DrawModeType.Normal;

            var scaleAnimation = new Animation(140);
            scaleAnimation.AnimateTo(_posterView, "Opacity", 0.7f, _easeOut);
            scaleAnimation.AnimateTo(_posterView, "ScaleX", 1.0f, _easeOut);
            scaleAnimation.AnimateTo(_posterView, "ScaleY", 1.0f, _easeOut);
            scaleAnimation.Play();
            scaleAnimation.Finished += (a, ev) =>
            {
                if (a is Animation ani) ani.Dispose();
            };
        }
    }
}
