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

        private VisualView _posterView;
        private ColorVisual _colorVisual;
        private ImageVisual _imageVisual;
        private readonly string _posterUrl;
        private readonly AlphaFunction _easeOut;

        public event EventHandler FocusGained;

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

        public Poster(string posterUrl, Size posterSize)
        {
            _posterUrl = posterUrl;
            _posterSize = posterSize;
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

            _imageVisual = new ImageVisual
            {
                URL = Application.Current.DirectoryInfo.SharedResource + "images/poster.jpg", // _posterUrl,
                FittingMode = FittingModeType.ScaleToFill,
                AlphaMaskURL = Application.Current.DirectoryInfo.SharedResource + "images/alphaMask.png",
                //DesiredHeight = 500,
                //DesiredWidth = 350,
            };
            _posterView.AddVisual(_posterUrl, _imageVisual);
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
