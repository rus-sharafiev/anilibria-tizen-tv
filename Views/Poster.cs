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
        private int _depthIndex;

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

        public int DepthIndex
        {
            set { _depthIndex = value; }
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
                Opacity = 0.8f,
                Size = _posterSize,
                Focusable = true,
            };
            _posterView.FocusGained += PosterView_FocusGained;
            _posterView.FocusLost += PosterView_FocusLost;

            _colorVisual = new ColorVisual
            {
                Color = new Color(255, 255, 255, 1),
            };
            _posterView.AddVisual("Color" + _posterUrl, _colorVisual);

            _imageVisual = new ImageVisual
            {
                URL = Application.Current.DirectoryInfo.SharedResource + "images/poster.jpg", // _posterUrl,
                FittingMode = FittingModeType.ScaleToFill,
                AlphaMaskURL = Application.Current.DirectoryInfo.SharedResource + "images/alphaMask.png",
                //DesiredHeight = (int)(_posterView.SizeHeight * 2.0f),
                //DesiredWidth = (int)(_posterView.SizeWidth * 2.0f),
                //PremultipliedAlpha = true,
                DepthIndex = _depthIndex,
            };
            _posterView.AddVisual(_posterUrl, _imageVisual);
        }

        private void PosterView_FocusGained(object sender, System.EventArgs e)
        {
            _imageVisual.DepthIndex = 10;
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
            _imageVisual.DepthIndex = 0;

            var scaleAnimation = new Animation(140);

            scaleAnimation.AnimateTo(_posterView, "Opacity", 0.8f, _easeOut);
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
