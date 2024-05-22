﻿using AnilibriaAppTizen.Models;
using AnilibriaAppTizen.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tizen.Applications;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class Release
    {
        private readonly ApiService _apiService;

        private static readonly float _padding = 40;
        private static readonly float _layoutPadding = 10;
        private static readonly float _posterWidth = 350;
        private static readonly float _posterHeight = 500;

        private readonly Position _posterPosition = new Position(_padding, _padding);
        private readonly Size _posterSize = new Size(_posterWidth, _posterHeight);
        private readonly AlphaFunction _easeOut;

        private View _releaseView;
        private View _releaseContainer;
        private bool _isActive = false;

        private VisualView _posterView;
        private Position _posterStartPosition;
        private Size _posterStartSize;
        private View _originalPosterView;
        private Animation _animation;

        public bool IsActive { get { return _isActive; } }

        public Release(ApiService apiService)
        {
            _apiService = apiService;
            _easeOut = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseOutSquare);
        }

        public void Initialize()
        {
            _releaseView = new View
            {
                Size = Window.Instance.Size,
            };

            Window.Instance.Add(_releaseView);
        }

        public void Render(View originalPosterView, Position posterPosition, Models.Release release)
        {
            _isActive = true;
            _originalPosterView = originalPosterView;
            _posterStartPosition = posterPosition;
            _posterStartSize = new Size(originalPosterView.SizeWidth * 1.2f, originalPosterView.SizeHeight * 1.2f);

            _posterView = new VisualView()
            {
                Size = _posterStartSize,
                Position = _posterStartPosition,
                DrawMode = DrawModeType.Overlay2D
            };
            _releaseView.Add(_posterView);
            _originalPosterView.Opacity = 0;

            var imageVisual = new ImageVisual
            {
                URL = Application.Current.DirectoryInfo.SharedResource + "images/poster.jpg", // _posterUrl,
                FittingMode = FittingModeType.ScaleToFill,
                AlphaMaskURL = Application.Current.DirectoryInfo.SharedResource + "images/alphaMask.png",
                DesiredHeight = 500,
                DesiredWidth = 350,
            };
            _posterView.AddVisual(release.Poster.Src, imageVisual);
            AnimatePosterTo(_posterView, _posterPosition, _posterSize);

            // Container
            _releaseContainer = new View
            {
                Size = _releaseView.Size,
                BackgroundColor = Color.Black,
                Opacity = 0,
            };
            _releaseView.Add(_releaseContainer);

            var releaseInfo = new View
            {
                PositionX = _posterWidth + _padding * 2,
                PositionY = _padding,
                SizeWidth = _releaseContainer.SizeWidth - _padding * 3 - _posterWidth,
                SizeHeight = 580,
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    CellPadding = new Size(_padding, _padding),
                },
            };
            _releaseContainer.Add(releaseInfo);

            // Title
            var releaseTitle = new View
            {
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    CellPadding = new Size(_layoutPadding, _layoutPadding)
                },
            };
            releaseInfo.Add(releaseTitle);

            var russianLabel = new TextLabel
            {
                Text = release.Name.Main,
                TextColor = new Color(255, 255, 255, 1f),
                FontFamily = "Roboto Light",
                PointSize = 36,
                MultiLine = true,
                LineSpacing = 0,
                SizeWidth = releaseInfo.SizeWidth
            };
            releaseTitle.Add(russianLabel);

            var englishLabel = new TextLabel
            {
                Text = release.Name.English,
                TextColor = new Color(255, 255, 255, 0.5f),
                FontFamily = "Roboto Thin",
                PointSize = 24,
                MultiLine = true,
                SizeWidth = releaseInfo.SizeWidth
            };
            releaseTitle.Add(englishLabel);

            // General info
            var generalInfo = new View
            {
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    CellPadding = new Size(_layoutPadding, _layoutPadding)
                },
            };
            releaseInfo.Add(generalInfo);

            var genres = string.Empty;
            foreach (var genre in release.Genres)
            {
                if (genre != release.Genres[0]) genres += " • ";
                genres += genre.Name;
            }

            var generalInfoList = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("Тип:", release.Type.Description),
                new KeyValuePair<string, string>("Сезон:", release.Season.Description),
                new KeyValuePair<string, string>("Жанры:", genres),
                new KeyValuePair<string, string>("Год выхода:", release.Year.ToString()),
            };

            if (release.AverageDurationOfEpisode != null)
            {
                generalInfoList.Add(new KeyValuePair<string, string>("Длительность:", $"~ {release.AverageDurationOfEpisode} мин"));
            }

            if (release.EpisodesTotal != null)
            {
                var episodeNameDeclensionList = new string[3] { "эпизод", "эпизода", "эпизодов" };
                var episodesTotal = DeclensionOfNumber((int)release.EpisodesTotal, episodeNameDeclensionList);
                generalInfoList.Add(new KeyValuePair<string, string>("Всего эпизодов:", episodesTotal));
            }

            //if (release.NewReleaseEpisodeOrdinal != null)
            //{
            //    Debug.WriteLine($"NewReleaseEpisodeOrdinal: {release.NewReleaseEpisodeOrdinal}");
            //    var totalViewingTime = MinutesToHoursWithMinutes((int)(release.AverageDurationOfEpisode * release.NewReleaseEpisodeOrdinal));
            //    generalInfoList.Add(new KeyValuePair<string, string>("Общее время просмотра:", totalViewingTime));
            //}

            foreach (var info in generalInfoList)
            {
                var infoLine = new View
                {
                    Layout = new LinearLayout
                    {
                        LinearOrientation = LinearLayout.Orientation.Horizontal,
                        CellPadding = new Size(_layoutPadding, _layoutPadding)
                    }
                };
                generalInfo.Add(infoLine);

                var infoKey = new TextLabel
                {
                    Text = info.Key,
                    TextColor = new Color(255, 255, 255, 0.5f),
                    FontFamily = "Roboto Thin",
                    PointSize = 24,
                };
                infoLine.Add(infoKey);

                var infoValue = new TextLabel
                {
                    Text = info.Value,
                    TextColor = new Color(255, 255, 255, 1f),
                    FontFamily = "Roboto Light",
                    PointSize = 24,
                };
                infoLine.Add(infoValue);
            }

            // Description
            var releaseDescription = new View
            {
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    CellPadding = new Size(_layoutPadding, _layoutPadding)
                },
            };
            releaseInfo.Add(releaseDescription);

            var descriptionText = new TextLabel
            {
                Text = release.Description,
                TextColor = new Color(255, 255, 255, 1f),
                FontFamily = "Roboto Thin",
                PointSize = 24,
                MultiLine = true,
                SizeWidth = releaseInfo.SizeWidth
            };
            releaseDescription.Add(descriptionText);

            // Episodes
            _ = RenderEpisodesAsync(release.Id);

            AnimateContainerOpacityTo(1);

        }

        public async Task RenderEpisodesAsync(int releaseId)
        {
            try
            {
                var fullReleseData = await _apiService.GetReleaseAsync(releaseId);

                var episodesSection = new View
                {
                    SizeHeight = _releaseContainer.SizeHeight - (580 + _padding),
                    SizeWidth = _releaseContainer.SizeWidth - _padding * 2,
                    PositionY = 580 + _padding,
                    PositionX = _padding,
                    Opacity = 0
                };
                _releaseContainer.Add(episodesSection);

                var episodesSectionTitle = new View
                {
                    SizeHeight = 58,
                    SizeWidth = episodesSection.SizeWidth,
                    ClippingMode = ClippingModeType.ClipChildren
                };
                episodesSection.Add(episodesSectionTitle);

                var episodesSectionTitleLabel = new TextLabel
                {
                    Text = "Список эпизодов",
                    TextColor = new Color(255, 255, 255, 0.2f),
                    FontFamily = "Roboto Thin",
                    PointSize = 64,
                    PositionY = 2,
                    PositionX = _layoutPadding,
                    SizeWidth = episodesSection.SizeWidth
                };
                episodesSectionTitle.Add(episodesSectionTitleLabel);

                var episodesSectionDivider = new View
                {
                    SizeHeight = 2,
                    SizeWidth = episodesSectionTitle.SizeWidth,
                    PositionY = episodesSectionTitle.SizeHeight,
                    BackgroundColor = new Color(255, 255, 255, 0.2f),
                };
                episodesSection.Add(episodesSectionDivider);

                var episodesScrollContainer = new View
                {
                    SizeHeight = episodesSection.SizeHeight - episodesSectionTitle.SizeHeight - episodesSectionDivider.SizeHeight,
                    SizeWidth = episodesSection.SizeWidth,
                    PositionY = episodesSectionTitle.SizeHeight,
                };
                episodesSection.Add(episodesScrollContainer);

                AnimateOpacityTo(episodesSection, 1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
        }

        public void AnimatePosterTo(View poster, Position position, Size size)
        {
            var animation = new Animation(280);
            animation.AnimateTo(poster, "Position", position, _easeOut);
            animation.AnimateTo(poster, "Size", size, _easeOut);
            animation.Play();
            animation.Finished += (a, ev) =>
            {
                if (a is Animation ani) ani.Dispose();
            };
        }

        private Animation AnimateContainerOpacityTo(float destination)
        {
            if (_animation != null)
            {
                if (_animation.State == Animation.States.Playing) _animation.Stop();
                _animation.Clear();
            }
            _animation = new Animation(280);
            _animation.AnimateTo(_releaseContainer, "Opacity", destination, _easeOut);
            _animation.Play();
            return _animation;
        }

        public void AnimateOpacityTo(View episodesSection, float destination)
        {
            var animation = new Animation(280);
            animation.AnimateTo(episodesSection, "Opacity", destination, _easeOut);
            animation.Play();
            animation.Finished += (a, ev) =>
            {
                if (a is Animation ani) ani.Dispose();
            };
        }

        public void Dispose()
        {
            AnimatePosterTo(_posterView, _posterStartPosition, _posterStartSize);
            AnimateContainerOpacityTo(0);
            _animation.Finished += (_, ev) =>
            {
                _isActive = false;
                _releaseContainer.Unparent();
                _releaseContainer.Dispose();
                _releaseContainer = null;
                _posterView.Unparent();
                _posterView.Dispose();
                _posterView = null;
                _originalPosterView.Opacity = 1;
                _originalPosterView = null;
            };
        }
        private string DeclensionOfNumber(int number, string[] titles)
        {
            var decCases = new int[] { 2, 0, 1, 1, 1, 2 };
            var decCase = number % 100 > 4 && number % 100 < 20 ? 2 : decCases[Math.Min(number % 10, 5)];
            return $"{number} {titles[decCase]}";
        }

        private string MinutesToHoursWithMinutes(int min)
        {
            var hours = Math.Floor(min / 60.0);
            var minutes = Math.Floor((min / 60.0 - hours) * 60);

            var hoursDeclensionList = new string[3] { "час", "часа", "часов" };
            var minutesDeclensionList = new string[3] { "минута", "минуты", "минут" };
            var hoursString = DeclensionOfNumber((int)hours, hoursDeclensionList);
            var minutesString = DeclensionOfNumber((int)minutes, minutesDeclensionList);

            return $"{hoursString}, {minutesString}";
        }
    }
}
