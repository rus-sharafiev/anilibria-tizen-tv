using AnilibriaAppTizen.Models;
using AnilibriaAppTizen.Services;
using System;
using System.Diagnostics;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class Schedule
    {
        private readonly ApiService _apiService;
        private readonly ImageService _imageService;

        private View _mainPageView;
        private Release _release;
        private Error _error;

        private View _lastFocusedView;
        private readonly int _columns = 10;
        private readonly float _fontSize = 48;
        private readonly int _animationDuration = 280;

        private TableView _scheduleView;
        private WeekDay[] _weekDays;

        private Animation _scheduleScrollAnimation;
        private readonly AlphaFunction _easeOut;
        private float _scrollPosition;

        private uint _prewSelectedRow;
        private bool _isActive = false;

        float _posterWidth;
        float _posterHeight;
        Size _posterSize;

        public bool IsActive { get { return _isActive; }}
        public View LastFocusedView {  get { return _lastFocusedView; }}

        public Schedule(ApiService apiService, ImageService imageService)
        {
            _apiService = apiService;
            _imageService = imageService;

            _scheduleScrollAnimation = new Animation(_animationDuration);
            _easeOut = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseOutSquare);
        }

        public async void RenderTo(MainPage mainPage, Release release, bool updateData = false)
        {
            _isActive = true;
            mainPage.SetTitle("Расписание");

            _mainPageView = mainPage.View;
            _release = release;
            _scrollPosition = 0;
            _posterWidth = _mainPageView.SizeWidth / _columns - 8.0f - (_mainPageView.SizeWidth * 0.02f / _columns);
            _posterHeight = _posterWidth * 500 / 350;
            _posterSize = new Size(_posterWidth, _posterHeight);

            ScheduleRelease[] scheduleReleases = Array.Empty<ScheduleRelease>();
            var loading = new LoadingIndicator().AddTo(_mainPageView);

            try
            {
                scheduleReleases = await _apiService.GetScheduleAsync(updateData);

                _weekDays = new WeekDays().Create();
                foreach (var scheduleRelease in scheduleReleases)
                {
                    _weekDays[scheduleRelease.Release.PublishDay.Value - 1].Releases.Add(scheduleRelease.Release);
                }
            }
            catch (Exception e)
            {
                _error = new Error(e);
            }
            finally
            {
                loading.Remove();
            }

            _scheduleView = new TableView()
            {
                Columns = _columns,
                CellPadding = new Vector2(4, 4),
                PositionX = _mainPageView.SizeWidth * 0.01f,
            };
            _mainPageView.Add(_scheduleView);
            _scheduleView.RemovedFromWindow += ScheduleView_RemovedFromWindow;

            for (uint i = 0; i < _columns; i++)
            {
                _scheduleView.SetFitWidth(i);
            }

            uint row = 0;
            foreach (var day in _weekDays)
            {
                uint column = 0;
                if (!day.IsFirst)
                    _scheduleView.InsertRow(++row);
                _scheduleView.SetFixedHeight(row, _fontSize + _posterHeight * 0.2f);

                TextLabel dayLabel = new TextLabel
                {
                    Text = day.Name,
                    TextColor = new Color(255, 255, 255, 0.5f),
                    PixelSize = _fontSize,
                    Focusable = !day.IsFirst,
                    Padding = new Extents(0, 0, (ushort)(_posterHeight * 0.1f), 0),
                    FontFamily = "Roboto Thin",
                };
                _scheduleView.AddChild(dayLabel, new TableView.CellPosition(row, 0, 1, (uint)_columns));
                dayLabel.Name = $"SheduleDayLabel_{row}";
                dayLabel.FocusGained += DayLabel_FocusGained;

                _scheduleView.InsertRow(++row);
                _scheduleView.SetFitHeight(row);
                foreach (var dayRelease in day.Releases)
                {
                    View releaseContainer = new View
                    {
                        Focusable = true,
                        Size = _posterSize,
                    };

                    //VisualView releaseImage = new VisualView()
                    //{
                    //    Opacity = 0.8f,
                    //    Size = _posterSize,
                    //};

                    //ImageVisual imageVisual = new ImageVisual
                    //{
                    //    URL = "https://anilibria.top" + dayRelease.Poster.Src,
                    //    FittingMode = FittingModeType.FitWidth,
                    //    DesiredHeight = (int)(_posterHeight * 2.0f),
                    //    DesiredWidth = (int)(_posterWidth * 2.0f),
                    //};

                    //releaseImage.AddVisual("Poster", imageVisual);

                    View releaseImage = new View
                    {
                        Opacity = 0.8f,
                        BackgroundColor = Color.White,
                        Size = _posterSize,
                    };

                    releaseContainer.Add(releaseImage);

                    releaseContainer.FocusGained += ReleaseImageContainer_FocusGained;
                    releaseContainer.FocusLost += ReleaseImageContainer_FocusLost;
                    //releaseImageContainer.KeyEvent += ReleaseImageContainer_KeyEvent;
                    if (column == 0)
                        releaseContainer.LeftFocusableView = mainPage.ActiveMenuButton.View;

                    releaseContainer.Name = $"ScheduleRelease_{row}_{column}";
                    _scheduleView.AddChild(releaseContainer, new TableView.CellPosition(row, column));

                    if (column + 1 < _columns)
                    {
                        column++;
                    }
                    else
                    {
                        _scheduleView.InsertRow(++row);
                        _scheduleView.SetFitHeight(row);
                        column = 0;
                    }

                    if (_lastFocusedView == null && dayRelease.PublishDay.Value == 1 && day.Releases[0] == dayRelease)
                    {
                        _lastFocusedView = releaseContainer;
                    }
                }
            }

            mainPage.ActiveMenuButton.View.RightFocusableView = _lastFocusedView;
        }

        private void DayLabel_FocusGained(object sender, EventArgs e)
        {
            if (sender is TextLabel textLabel)
            {
                var row = uint.Parse(textLabel.Name.Split("_")[1]);
                if (_prewSelectedRow > row)
                    FocusManager.Instance.MoveFocus(View.FocusDirection.Up);

                if (_prewSelectedRow < row)
                    FocusManager.Instance.MoveFocus(View.FocusDirection.Down);
            }
        }

        private void ReleaseImageContainer_FocusGained(object sender, EventArgs e)
        {
            if (sender is View releaseContainer)
            {
                var releaseImage = releaseContainer.Children[0];
                var scaleAnimation = new Animation(140);

                releaseImage.BackgroundColor = Color.Green;
                scaleAnimation.AnimateTo(releaseImage, "Opacity", 1.0f, _easeOut);
                scaleAnimation.AnimateTo(releaseImage, "ScaleX", 1.2f, _easeOut);
                scaleAnimation.AnimateTo(releaseImage, "ScaleY", 1.2f, _easeOut);
                scaleAnimation.Play();
                scaleAnimation.Finished += (a, ev) =>
                {
                    if (a is Animation ani) ani.Dispose();
                };

                ScrollTo(releaseContainer);
            }
        }

        private void ReleaseImageContainer_FocusLost(object sender, EventArgs e)
        {
            if (sender is View releaseContainer)
            {
                var releaseImage = releaseContainer.Children[0];
                var scaleAnimation = new Animation(140);

                releaseImage.BackgroundColor = Color.White;
                scaleAnimation.AnimateTo(releaseImage, "Opacity", 0.8f, _easeOut);
                scaleAnimation.AnimateTo(releaseImage, "ScaleX", 1.0f, _easeOut);
                scaleAnimation.AnimateTo(releaseImage, "ScaleY", 1.0f, _easeOut);
                scaleAnimation.Play();
                scaleAnimation.Finished += (a, ev) =>
                {
                    if (a is Animation ani) ani.Dispose();
                };

                var row = uint.Parse(releaseContainer.Name.Split("_")[1]);
                _prewSelectedRow = row;
            }
        }

        private bool ReleaseImageContainer_KeyEvent(object source, View.KeyEventArgs e)
        {
            return false;
        }

        private void ScrollTo(View focusedView)
        {
            float dayContainerTop = focusedView.PositionY;
            float dayContainerBottom = focusedView.PositionY + focusedView.SizeHeight;

            float visibleRectangleTop = -_scheduleView.PositionY;
            float visibleRectangleBottom = -_scheduleView.PositionY + _mainPageView.SizeHeight;

            if (dayContainerTop < visibleRectangleTop)
            {
                _scrollPosition += visibleRectangleTop - dayContainerTop + _fontSize + _posterHeight * 0.2f;
                AnimateTo(_scrollPosition);
            }
            else if (dayContainerBottom > visibleRectangleBottom)
            {
                _scrollPosition -= dayContainerBottom - visibleRectangleBottom + _posterHeight * 0.2f;
                AnimateTo(_scrollPosition);
            }
        }

        public void AnimateTo(float destination)
        {
            if (_scheduleScrollAnimation != null)
            {
                if (_scheduleScrollAnimation.State == Animation.States.Playing)
                {
                    _scheduleScrollAnimation.Stop();
                }
                _scheduleScrollAnimation.Clear();
            }
            _scheduleScrollAnimation = new Animation(_animationDuration);
            _scheduleScrollAnimation.AnimateTo(_scheduleView, "PositionY", destination, _easeOut);
            _scheduleScrollAnimation.Play();
        }

        public void UnloadPage()
        {
            if (_scheduleView != null)
            {
                _scheduleView.Unparent();
            }
        }

        private void ScheduleView_RemovedFromWindow(object sender, EventArgs e)
        {
            _lastFocusedView = null;
            _isActive = false;
            _scheduleView.Dispose();
            _scheduleView = null;
        }
    }
}