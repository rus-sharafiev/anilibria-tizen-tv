using AnilibriaAppTizen.Services;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using Tizen.Applications;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.Pims.Contacts.ContactsViews;

namespace AnilibriaAppTizen.Views
{
    internal class Release
    {
        private readonly ApiService _apiService;
        private readonly ImageService _imageService;

        private static readonly float _padding = 40;
        private static readonly float _layoutPadding = 10;
        private static readonly float _posterWidth = 350;
        private static readonly float _posterHeight = 500;

        private readonly Position _posterPosition = new Position(_padding, _padding);
        private readonly Size _posterSize = new Size(_posterWidth, _posterHeight);
        private readonly AlphaFunction _easeOut;

        private View _releaseView;
        private View _releaseContainer;
        private View _playBtn;
        private bool _isActive = false;

        private VisualView _posterView;
        private Position _posterStartPosition;
        private Size _posterStartSize;
        private View _originalPosterView;
        private ScrollContainer _episodesScrollContainer;
        private Animation _animation;

        public bool IsActive { get { return _isActive; } }

        public Release(ApiService apiService, ImageService imageService)
        {
            _apiService = apiService;
            _imageService = imageService;
            _easeOut = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseInOut);
        }

        public void Initialize()
        {
            _releaseView = new View
            {
                Size = Window.Instance.Size,
            };

            Window.Instance.Add(_releaseView);
        }

        public async void Render(View originalPosterView, Position posterPosition, Models.Release release)
        {
            _isActive = true;
            _originalPosterView = originalPosterView;
            _posterStartPosition = posterPosition;
            _posterStartSize = new Size(originalPosterView.SizeWidth * 1.2f, originalPosterView.SizeHeight * 1.2f);

            // Poster
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
                URL = await _imageService.GetPath(release.Poster.Src),
                //URL = Application.Current.DirectoryInfo.SharedResource + "images/poster.jpg",
                FittingMode = FittingModeType.ScaleToFill,
                AlphaMaskURL = Application.Current.DirectoryInfo.SharedResource + "images/posterAlphaMask.png",
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

            // Play button
            _playBtn = new View
            {
                Focusable = true,
                SizeHeight = 70,
                SizeWidth = _posterWidth,
                PositionX = _padding,
                PositionY = _posterHeight + _padding + 10,
                Name = "play-btn",
                Opacity = 0.6f
            };
            _releaseContainer.Add(_playBtn);

            var playIcon = new VisualView()
            {
                Size = new Size2D(24, 24),
                PositionX = 25,
                PositionY = 23,
            };
            _playBtn.Add(playIcon);

            var playIconVisual = new SVGVisual
            {
                URL = Application.Current.DirectoryInfo.SharedResource + "icons/play.svg"
            };
            playIcon.AddVisual("play-icon", playIconVisual);

            var textLabel = new TextLabel
            {
                Text = "Начать просмотр",
                FontFamily = "Roboto",
                PointSize = 24,
                TextColor = Color.White,
                PositionX = 67,
                SizeHeight = 70,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _playBtn.Add(textLabel);

            _playBtn.FocusGained += (obj, e) =>
            {
                playIconVisual.URL = Application.Current.DirectoryInfo.SharedResource + "icons/play_filled.svg";
                AnimateOpacityTo(_playBtn, 1.0f);
            };
            _playBtn.FocusLost += (obj, e) =>
            {
                playIconVisual.URL = Application.Current.DirectoryInfo.SharedResource + "icons/play.svg";
                AnimateOpacityTo(_playBtn, 0.6f);
            };
            _playBtn.RightFocusableView = null;
            _playBtn.LeftFocusableView = null;
            _playBtn.UpFocusableView = null;

            // Info container
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
                ClippingMode = ClippingModeType.ClipChildren,
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
                TextColor = new Color(1, 1, 1, 0.8f),
                FontFamily = "Roboto Light",
                PointSize = 22,
                MultiLine = true,
                SizeWidth = releaseInfo.SizeWidth
            };
            releaseDescription.Add(descriptionText);

            // Episodes
            _ = RenderEpisodesAsync(release.Id);

            AnimateContainerOpacityTo(1);
            FocusManager.Instance.SetCurrentFocusView(_playBtn);
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
                    BackgroundColor = new Color(1, 1, 1, 0.2f),
                };
                episodesSection.Add(episodesSectionDivider);

                var episodesView = new View
                {
                    SizeHeight = episodesSection.SizeHeight - episodesSectionTitle.SizeHeight - episodesSectionDivider.SizeHeight,
                    SizeWidth = episodesSection.SizeWidth,
                    PositionY = episodesSectionTitle.SizeHeight,
                };
                episodesSection.Add(episodesView);

                _episodesScrollContainer = new ScrollContainer
                {
                    TopScrollIndentation = _layoutPadding,
                    BottomScrollIndentation = _layoutPadding,
                };
                episodesView.Add(_episodesScrollContainer.View);
                _episodesScrollContainer.View.PositionY = _layoutPadding;

                var columns = 4;
                var rows = (int)Math.Ceiling((float)fullReleseData.Episodes.Length / columns);
                var episodesGrid = new View()
                {
                    Layout = new GridLayout
                    {
                        Columns = columns,
                        Rows = rows,
                        ColumnSpacing = _layoutPadding,
                        RowSpacing = _layoutPadding,
                        GridOrientation = GridLayout.Orientation.Horizontal,
                    },
                };
                _episodesScrollContainer.View.Add(episodesGrid);

                var focusMatrix = new View[rows, columns];

                var column = 0;
                var row = 0;
                foreach (var episodeData in fullReleseData.Episodes)
                {
                    var episode = new Episode(episodeData, _imageService);

                    episodesGrid.Add(episode.View);
                    GridLayout.SetColumn(episode.View, column);
                    GridLayout.SetRow(episode.View, row);

                    focusMatrix[row, column] = episode.View;

                    if (column == 0 && row == 0) 
                    { 
                        _playBtn.DownFocusableView = episode.View;
                    }

                    if (column + 1 < columns)
                    {
                        column++;
                    }
                    else if (column < fullReleseData.Episodes.Length - 1)
                    {
                        row++;
                        column = 0;
                    }
                }

                // Create focus matrix
                for (int r = 0; r < focusMatrix.GetLength(0); r++)
                    for (int c = 0; c < focusMatrix.GetLength(1); c++)
                        if (focusMatrix[r, c] != null)
                        {
                            if (r > 0)
                                focusMatrix[r, c].UpFocusableView = GetClothestView(focusMatrix, r - 1, c);
                            else if (r == 0)
                                focusMatrix[r, c].UpFocusableView = _playBtn;

                            if (r < focusMatrix.GetLength(0) - 1)
                                focusMatrix[r, c].DownFocusableView = GetClothestView(focusMatrix, r + 1, c);

                            if (c > 0)
                                focusMatrix[r, c].LeftFocusableView = focusMatrix[r, c - 1];

                            if (c < focusMatrix.GetLength(1) - 1)
                                focusMatrix[r, c].RightFocusableView = focusMatrix[r, c + 1];
                        }

                AnimateOpacityTo(episodesSection, 1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
        }

        private View GetClothestView(View[,] focusMatrix, int r, int c)
        {
            if (focusMatrix[r, c] != null)
                return focusMatrix[r, c];
            else if (c > 0)
                for (int i = c - 1; i > 0; i--)
                {
                    if (focusMatrix[r, i] != null)
                    {
                        return focusMatrix[r, i];
                    }
                }

            return null;
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
                FocusManager.Instance.SetCurrentFocusView(_originalPosterView);
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
