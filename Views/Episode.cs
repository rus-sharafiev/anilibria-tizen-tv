using AnilibriaAppTizen.Models;
using AnilibriaAppTizen.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tizen.Applications;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class Episode
    {
        private readonly ImageService _imageService;
        private readonly static float _padding = 8;

        private readonly static AlphaFunction easeOut = 
            new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseOutSquare);

        private View _episodeView;
        private VisualView _playVisualView;
        private Models.Episode _episodeData;

        public View View { get => _episodeView; }

        public Episode (Models.Episode episodeData, ImageService imageService)
        {
            _episodeData = episodeData;
            _imageService = imageService;

            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            _episodeView = new View
            {
                Focusable = true,
                SizeWidth = 450,
                SizeHeight = 117,
                Opacity = 0.6f,
            };
            _episodeView.FocusGained += EpisodeView_FocusGained;
            _episodeView.FocusLost += EpisodeView_FocusLost;

            var preview = new VisualView
            {
                SizeHeight = 117,
                SizeWidth = 204,
            };
            _episodeView.Add(preview);

            var borderVisual = new ImageVisual
            {
                URL = Application.Current.DirectoryInfo.SharedResource + "images/episodeWhiteBorder.png",
            };
            preview.AddVisual("episodeWhiteBorder", borderVisual);

            var previewVisual = new ImageVisual
            {
                URL = await _imageService.GetPath(_episodeData.Preview.Thumbnail),
                AlphaMaskURL = Application.Current.DirectoryInfo.SharedResource + "images/episodePreviewAlphaMask.png",
                SizePolicy = VisualTransformPolicyType.Absolute,
                Size = new Size2D(200, 113),
                PositionPolicy = VisualTransformPolicyType.Absolute,
                Position = new Vector2(2, 2),
            };
            preview.AddVisual(_episodeData.Preview.Thumbnail, previewVisual);

            _playVisualView = new VisualView
            {
                SizeHeight = 117,
                SizeWidth = 204,
                Opacity = 0,
            }; 
            _episodeView.Add(_playVisualView);

            var playVisual = new SVGVisual
            {
                URL = Application.Current.DirectoryInfo.SharedResource + "icons/play.svg",
                SizePolicy = VisualTransformPolicyType.Absolute,
                Size = new Size2D(48, 48),
                PositionPolicy= VisualTransformPolicyType.Absolute,
                Position = new Vector2(_playVisualView.SizeWidth / 2 - 24, _playVisualView.SizeHeight / 2 - 24),
            };
            _playVisualView.AddVisual("playIcon", playVisual);

            var episodeInfo = new View
            {
                PositionX = preview.SizeWidth + _padding,
                SizeWidth = _episodeView.SizeWidth - preview.SizeWidth - _padding * 2,
                SizeHeight = preview.SizeHeight,
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    CellPadding = new Size(_padding, _padding),
                }
            };
            _episodeView.Add(episodeInfo);

            var episodeNumber = new TextLabel
            {
                Text = $"Серия {_episodeData.Ordinal}",
                TextColor = Color.White,
                FontFamily = "Roboto Light",
                PointSize = 32,
                MultiLine = true,
                SizeWidth = episodeInfo.SizeWidth,
            };
            episodeInfo.Add(episodeNumber);

            var episodeTitle = new TextLabel
            {
                Text = _episodeData.Name,
                TextColor = new Color(1, 1, 1, 0.6f),
                FontFamily = "Roboto Light",
                PointSize = 14,
                MultiLine = true,
                SizeWidth = episodeInfo.SizeWidth,
            };
            episodeInfo.Add(episodeTitle);
        }

        private void EpisodeView_FocusGained(object sender, EventArgs e)
        {
            AnimateOpacity(1);
        }

        private void EpisodeView_FocusLost(object sender, EventArgs e)
        {
            AnimateOpacity(0);
        }

        private void AnimateOpacity(float destination)
        {
            var animation = new Animation(140);
            animation.AnimateTo(_episodeView, "Opacity", destination == 0 ? 0.6f : 1, easeOut);
            animation.AnimateTo(_playVisualView, "Opacity", destination, easeOut);
            animation.Play();
            animation.Finished += (a, ev) =>
            {
                if (a is Animation ani) ani.Dispose();
            };
        }
    }
}
