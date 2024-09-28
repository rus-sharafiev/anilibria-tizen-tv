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
        private readonly float _dayLabelfontSize = 64;

        private readonly ApiService _apiService;
        private readonly ImageService _imageService;

        private View _mainView;
        private View _activeMenuBtn;
        private ScrollContainer _scrollContainer;

        private View _lastFocusedView;

        private View _scheduleView;
        private WeekDay[] _weekDays;

        private bool _isActive = false;

        private int _columns;
        private int _posterSpacing;
        private float _posterWidth;
        private float _posterHeight;
        private Size _posterSize;

        public bool IsActive { get { return _isActive; } }
        public View LastFocusedView { get { return _lastFocusedView; } }

        public Schedule(ApiService apiService, ImageService imageService)
        {
            _apiService = apiService;
            _imageService = imageService;
        }

        public void RenderTo(Main main)
        {
            _isActive = true;
            main.SetTitle("Расписание", "выхода новых эпизодов", 320);

            if (_scrollContainer != null)
                _mainView.Add(_scrollContainer.View);
        }

        public async void InitializeAsync(Main main, bool updateData = false)
        {
            _mainView = main.View;
            _activeMenuBtn = main.ActiveMenuButton.View;

            _columns = main.Columns;
            _posterSpacing = main.PosterSpacing;
            _posterWidth = main.PosterWidth;
            _posterHeight = main.PosterHeight;
            _posterSize = new Size(_posterWidth, _posterHeight);

            ScheduleRelease[] scheduleReleases = Array.Empty<ScheduleRelease>();
            var loading = new LoadingIndicator().AddTo(_mainView);

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
                new Error(e);
                return;
            }
            finally
            {
                loading.Remove();
            }

            // Calc rows qty
            int rows = 0;
            foreach (var day in _weekDays)
            {
                rows += 1 + (int)Math.Ceiling((float)day.Releases.Count / _columns);
            }

            _scrollContainer = new ScrollContainer
            {
                TopScrollIndentation = _dayLabelfontSize,
                BottomScrollIndentation = _posterHeight * 0.2f,
            };
            _scrollContainer.View.RemovedFromWindow += ScheduleView_RemovedFromWindow;
            _mainView.Add(_scrollContainer.View);

            _scheduleView = new View()
            {
                Layout = new GridLayout
                {
                    Columns = _columns,
                    Rows = rows,
                    ColumnSpacing = _posterSpacing,
                    RowSpacing = _posterSpacing,
                    GridOrientation = GridLayout.Orientation.Horizontal,
                },
                PositionX = _posterWidth * 0.1f + _posterSpacing
            };
            _scrollContainer.View.Add(_scheduleView);

            int focusMatrixRow = 0;
            _scrollContainer.CreateFocusMatrix(rows, _columns);

            int row = 0;
            foreach (var day in _weekDays)
            {
                int column = 0;
                if (!day.IsFirst) row++;

                var dayLabelView = new View
                {
                    SizeHeight = _dayLabelfontSize * (day.IsFirst ? 0.8f : 1),
                };
                _scheduleView.Add(dayLabelView);

                var dayLabel = new TextLabel
                {
                    Text = day.Name,
                    TextColor = new Color(255, 255, 255, 0.5f),
                    PointSize = _dayLabelfontSize,
                    FontFamily = "Roboto Thin",
                    Name = $"SheduleDayLabel_{row}",
                    VerticalAlignment = VerticalAlignment.Bottom,
                    PositionY = day.IsFirst ? 0 : _dayLabelfontSize * 0.2f,
                    Opacity = 0.2f,
                };
                dayLabelView.Add(dayLabel);

                GridLayout.SetColumn(dayLabelView, 0);
                GridLayout.SetColumnSpan(dayLabelView, _columns);
                GridLayout.SetRow(dayLabelView, row);

                row++;
                focusMatrixRow++;
                foreach (var dayRelease in day.Releases)
                {
                    var releasePoster = new Poster(dayRelease, main);
                    releasePoster.FocusGained += ReleasePoster_FocusGained;
                    releasePoster.View.Name = $"ScheduleRelease_{row}_{column}";
                    releasePoster.ParentContainers = new View[] { 
                        _scheduleView, _scrollContainer.View, _mainView
                    };

                    _scheduleView.Add(releasePoster.View);
                    GridLayout.SetColumn(releasePoster.View, column);
                    GridLayout.SetRow(releasePoster.View, row);

                    _scrollContainer.AddToFocusMatrix(focusMatrixRow, column, releasePoster.View);

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
                        _activeMenuBtn.RightFocusableView = releasePoster.View;
                    }
                }
            }

            _scrollContainer.LeftFocusableView = _activeMenuBtn;
            _scrollContainer.GenerateFocusMatrix();
        }

        private void ReleasePoster_FocusGained(object sender, EventArgs e)
        {
            if (sender is Poster poster)
            {
                _scrollContainer.ScrollTo(poster.View);
                _activeMenuBtn.RightFocusableView = poster.View;
            }
        }

        private void ScheduleView_RemovedFromWindow(object sender, EventArgs e)
        {
            //_lastFocusedView = null;
            _isActive = false;
        }
    }
}