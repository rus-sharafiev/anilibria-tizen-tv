using System;
using System.Diagnostics;
using Tizen.Applications;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class Poster
    {
        private readonly Size _posterSize;

        private readonly View _mainPageView;
        private readonly Release _releasePage;
        private VisualView _posterView;
        private ImageVisual _imageVisual;
        private readonly Models.Release _releaseData;
        private readonly string _posterUrl;
        private readonly AlphaFunction _easeOut;

        private View _parentContainer;

        public event EventHandler FocusGained;
        public event EventHandler Activated;

        public VisualView View 
        {
            get { return _posterView; }
        }

        public Size PosterSize
        {
            get { return _posterSize; }
        }

        public string URL
        {
            get { return _posterUrl; }
        }

        public View ParentContainer
        {
            set { _parentContainer = value; }
        }

        public Poster(string posterUrl, MainPage mainPage, Models.Release releaseData)
        {
            _mainPageView = mainPage.View;
            _posterUrl = posterUrl;
            _releasePage = mainPage.ReleasePage;
            _releaseData = releaseData;
            _posterSize = new Size(mainPage.PosterWidth, mainPage.PosterHeight);
            _easeOut = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseOutSquare);

            Initialize();
        }


        public void Initialize()
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

            _imageVisual = new ImageVisual
            {
                URL = Application.Current.DirectoryInfo.SharedResource + "images/poster.jpg", // _posterUrl,
                FittingMode = FittingModeType.ScaleToFill,
                AlphaMaskURL = Application.Current.DirectoryInfo.SharedResource + "images/alphaMask.png",
                DesiredHeight = 500,
                DesiredWidth = 350,
            };
            _posterView.AddVisual(_posterUrl, _imageVisual);
        }

        private bool PosterView_KeyEvent(object source, View.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down && e.Key.KeyPressedName == "Return")
            {
                var posX = _posterView.PositionX + _parentContainer.PositionX + _mainPageView.PositionX - _posterView.SizeWidth * 0.1f;
                var posY = _posterView.PositionY + _parentContainer.PositionY + _mainPageView.PositionY - _posterView.SizeHeight * 0.1f;

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
