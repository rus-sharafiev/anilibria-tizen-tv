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

        private View _scheduleView;
        private View _scheduleViewScrollContainer;
        private WeekDay[] _weekDays;

        private Animation _scheduleScrollAnimation;
        private readonly AlphaFunction _easeOut;
        private float _scrollPosition;

        private uint _prewSelectedRow;
        private bool _isActive = false;

        float _posterWidth;
        float _posterHeight;
        Size _posterSize;

        public bool IsActive { get { return _isActive; } }
        public View LastFocusedView { get { return _lastFocusedView; } }

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
            mainPage.SetTitle("Расписание", "выхода новых эпизодов", 320);

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

            // Calc rows qty
            var rows = 0;
            foreach (var day in _weekDays)
            {
                rows += 1 + (int)Math.Ceiling((float)day.Releases.Count / _columns);
            }

            _scheduleViewScrollContainer = new View();
            _mainPageView.Add(_scheduleViewScrollContainer);

            var scheduleViewLayout = new GridLayout
            {
                Columns = _columns,
                Rows = rows,
                ColumnSpacing = 8,
                RowSpacing = 8,
                GridOrientation = GridLayout.Orientation.Horizontal,
            };
            _scheduleView = new View()
            {
                Layout = scheduleViewLayout,
                PositionX = _mainPageView.SizeWidth * 0.01f,
            };
            _scheduleViewScrollContainer.Add(_scheduleView);
            _scheduleViewScrollContainer.RemovedFromWindow += ScheduleView_RemovedFromWindow;

            var focusMatrixRow = 0;
            var focusMatrix = new View[rows, _columns];

            int row = 0;
            foreach (var day in _weekDays)
            {
                int column = 0;
                if (!day.IsFirst) row++;

                var dayLabelView = new View
                {
                    SizeHeight = _fontSize + (day.IsFirst ? 0 : _posterHeight * 0.1f),
                };
                _scheduleView.Add(dayLabelView);

                var dayLabel = new TextLabel
                {
                    Text = day.Name,
                    TextColor = new Color(255, 255, 255, 0.5f),
                    PointSize = _fontSize,
                    FontFamily = "Roboto Thin",
                    Name = $"SheduleDayLabel_{row}",
                    VerticalAlignment = VerticalAlignment.Bottom,
                    PositionY = day.IsFirst
                        ? _fontSize * 0.2f
                        : _posterHeight * 0.1f + _fontSize * 0.1f,
                    PositionX = -_posterWidth * 0.1f,
                    Opacity = 0.6f,
                };
                dayLabelView.Add(dayLabel);

                GridLayout.SetColumn(dayLabelView, 0);
                GridLayout.SetColumnSpan(dayLabelView, _columns);
                GridLayout.SetRow(dayLabelView, row);

                row++;
                focusMatrixRow++;
                foreach (var dayRelease in day.Releases)
                {
                    var url = "https://anilibria.top" + dayRelease.Poster.Src;
                    var releasePoster = new Poster(url, _posterSize);
                    releasePoster.View.Name = $"ScheduleRelease_{row}_{column}";
                    releasePoster.FocusGained += ReleasePoster_FocusGained;

                    _scheduleView.Add(releasePoster.View);
                    GridLayout.SetColumn(releasePoster.View, column);
                    GridLayout.SetRow(releasePoster.View, row);

                    focusMatrix[focusMatrixRow, column] = releasePoster.View;

                    if (column + 1 < _columns)
                    {
                        column++;
                    }
                    else if (column < day.Releases.Count - 1)
                    {
                        row++;
                        focusMatrixRow++;
                        column = 0;
                    }

                    if (_lastFocusedView == null && dayRelease.PublishDay.Value == 1 && day.Releases[0] == dayRelease)
                    {
                        _lastFocusedView = releasePoster.View;
                    }
                }
            }

            // Create focus matrix
            for (int r = 0; r < focusMatrix.GetLength(0); r++)
                for (int c = 0; c < focusMatrix.GetLength(1); c++)
                    if (focusMatrix[r, c] != null)
                    {
                        if (r > 0)
                            focusMatrix[r, c].UpFocusableView = focusMatrix[r - 1, c];

                        if (r < focusMatrix.GetLength(0) - 1)
                            focusMatrix[r, c].DownFocusableView = focusMatrix[r + 1, c];

                        if (c > 0)
                            focusMatrix[r, c].LeftFocusableView = focusMatrix[r, c - 1];
                        else if (c == 0)
                            focusMatrix[r, c].LeftFocusableView = mainPage.ActiveMenuButton.View;

                        if (c < focusMatrix.GetLength(1) - 1)
                            focusMatrix[r, c].RightFocusableView = focusMatrix[r, c + 1];
                    }

            mainPage.ActiveMenuButton.View.RightFocusableView = _lastFocusedView;
        }

        private void ReleasePoster_FocusGained(object sender, EventArgs e)
        {
            if (sender is Poster poster) ScrollTo(poster.View);
        }

        private void ScrollTo(View focusedView)
        {
            float focusedViewTop = focusedView.PositionY;
            float focusedViewBottom = focusedView.PositionY + _posterHeight;

            float visibleRectangleTop = -_scheduleViewScrollContainer.PositionY + _fontSize;
            float visibleRectangleBottom = -_scheduleViewScrollContainer.PositionY + _mainPageView.SizeHeight - _posterHeight * 0.2f;

            if (focusedViewTop < visibleRectangleTop)
            {
                _scrollPosition += visibleRectangleTop - focusedViewTop;
                AnimateTo(_scrollPosition);
            }
            else if (focusedViewBottom > visibleRectangleBottom)
            {
                _scrollPosition -= focusedViewBottom - visibleRectangleBottom;
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
            _scheduleScrollAnimation.AnimateTo(_scheduleViewScrollContainer, "PositionY", destination);
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