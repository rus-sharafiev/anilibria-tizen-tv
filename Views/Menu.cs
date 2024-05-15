using System;
using System.Collections.Generic;
using Tizen.NUI.BaseComponents;
using Tizen.NUI;
using System.Diagnostics;

namespace AnilibriaAppTizen.Views
{
    internal class Menu
    {
        private readonly int _collapsedWidth = 80;
        private readonly int _expandedWidth = 280;

        #pragma warning disable CS0618
        readonly int windowSizeHeight = Window.Instance.Size.Height;
        #pragma warning restore CS0618

        private readonly MainPage _mainPage;
        private readonly AlphaFunction _easeOut;

        private View _menuView;
        private Animation _animation;

        private MenuButton _activeBtn;
        private List<View> _btnViews;

        public event EventHandler ActiveButtonChanged;

        public MenuButton ActiveButton
        {
            get { return _activeBtn; }
        }

        public View View
        {
            get { return _menuView; }
        }

        public int CollapsedWidth
        {
            get { return _collapsedWidth; }
        }

        public int ExpandedWidth
        {
            get { return _expandedWidth; }
        }

        public string SharedRes
        {
            get { return _mainPage.SharedRes; }
        }

        public Menu(MainPage mainPage)
        {
            _mainPage = mainPage;
            _easeOut =  new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseOutSquare);
        }

        private readonly Button[] menuItems =
        {
            new Button() {Key = "logo", Name = "Anilibria"},
            new Button() {Key = "home", Name = "Главная"},
            new Button() {Key = "schedule", Name = "Расписание"},
            new Button() {Key = "settings", Name = "Настройки"},
        };

        public void RenderTo(View view)
        {
            _menuView = new View
            {
                Layout = new FlexLayout
                {
                    Direction = FlexLayout.FlexDirection.Column,
                },
                SizeWidth = _collapsedWidth,
                SizeHeight = windowSizeHeight,
                Name = "Menu",
                Focusable = false,
                ClippingMode = ClippingModeType.ClipChildren
            };
            view.Add(_menuView);

            _btnViews = new List<View>();
            foreach (var btn in menuItems)
            {
                var menuBtn = new MenuButton(this)
                {
                    Text = btn.Name,
                    Key = btn.Key,
                };
                _menuView.Add(menuBtn.View);
                menuBtn.FocusGained += MenuBtn_FocusGained;

                if (btn.Key == "home")
                {
                    _activeBtn = menuBtn;
                    ActiveButtonChanged?.Invoke(this, EventArgs.Empty);
                }

                if (btn.Key != "logo")
                    _btnViews.Add(menuBtn.View);
            }

            for (int j = 0; j < _btnViews.Count; j++)
            {
                if (j != 0)
                    _btnViews[j].UpFocusableView = _btnViews[j - 1];

                if (j < _btnViews.Count - 1)
                    _btnViews[j].DownFocusableView = _btnViews[j + 1];
            }

            FocusManager.Instance.FocusChanged += FocusManagerInstance_FocusChanged;
        }

        private void MenuBtn_FocusGained(object sender, EventArgs e)
        {
            if (sender is MenuButton menuButton && menuButton != _activeBtn)
            {
                _activeBtn = menuButton;
                ActiveButtonChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void FocusManagerInstance_FocusChanged(object sender, FocusManager.FocusChangedEventArgs e)
        {
            if (e.NextView != null)
            {
                if (e.NextView.Name.Contains("MenuButton"))
                {
                    if (e.CurrentView == null || !e.CurrentView.Name.Contains("MenuButton"))
                    {
                        AnimateMenuWidthTo(_expandedWidth);
                    }
                }
                else
                {
                    AnimateMenuWidthTo(_collapsedWidth);
                }
            }
        }

        private void AnimateMenuWidthTo(int destination)
        {
            if (_animation != null)
            {
                if (_animation.State == Animation.States.Playing)
                {
                    _animation.Stop();
                }
                _animation.Clear();
                _animation.Dispose();
            }
            _animation = new Animation(280);
            _animation.AnimateTo(_menuView, "SizeWidth", destination, _easeOut);
            _animation.AnimateTo(_mainPage.View, "PositionX", destination, _easeOut);
            _animation.AnimateTo(_mainPage.TitleView, "PositionX", destination, _easeOut);
            _animation.Play();
        }
    }

    class Button
    {
        public string Key { get; set; }
        public string Name { get; set; }
    }
}
