﻿using System;
using System.Collections.Generic;
using Tizen.NUI.BaseComponents;
using Tizen.NUI;
using System.Diagnostics;
using AnilibriaAppTizen.Services;

namespace AnilibriaAppTizen.Views
{
    internal class Menu
    {
        private readonly int _collapsedWidth = 70;
        private readonly int _expandedWidth = 230;

        private readonly UserService _userService;

        #pragma warning disable CS0618
        readonly int windowSizeHeight = Window.Instance.Size.Height;
        #pragma warning restore CS0618

        private readonly Main _main;
        private readonly AlphaFunction _easeOut;

        private View _menuView;
        private Animation _animation;
        private MenuButton _accountBtn;

        private MenuButton _activeBtn;
        private List<View> _btnViews;

        public event EventHandler ActiveButtonChanged;

        public MenuButton ActiveButton
        {
            get { return _activeBtn; }
        }

        public MenuButton AccountBtn
        {
            get { return _accountBtn; }
            set { _accountBtn = value; }
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

        public Menu(Main main, UserService userService)
        {
            _main = main;
            _userService = userService;
            _easeOut =  new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseOutSquare);

            _userService.UserChanged += UserService_UserChanged;
        }

        private readonly Button[] menuItems =
        {
            new Button() {Key = "home", Name = "Главная", Position = "top"},
            new Button() {Key = "schedule", Name = "Расписание", Position = "top"},
            new Button() {Key = "search", Name = "Поиск", Position = "top"},
            new Button() {Key = "account", Name = "Войти", Position = "bottom"},
            new Button() {Key = "settings", Name = "Настройки", Position = "bottom"},
        };

        public void RenderTo(View view)
        {
            _menuView = new View
            {
                Layout = new FlexLayout
                {
                    Direction = FlexLayout.FlexDirection.Column,
                    Justification = FlexLayout.FlexJustification.SpaceBetween,
                },
                SizeWidth = _collapsedWidth,
                SizeHeight = windowSizeHeight,
                Name = "Menu",
                ClippingMode = ClippingModeType.ClipChildren
            };
            view.Add(_menuView);

            var logo = new MenuButton(this)
            {
                Text = "Anilibria",
                Key = "logo",
            };
            logo.View.Focusable = false;
            _menuView.Add(logo.View);
            FlexLayout.SetFlexPositionType(logo.View, FlexLayout.PositionType.Absolute);

            var topView = new View
            {
                Layout = new FlexLayout
                {
                    Direction = FlexLayout.FlexDirection.Column,
                },
                SizeHeight= _collapsedWidth * 1.5f,
                Padding = new Extents(0, 0, (ushort)(_collapsedWidth * 1.5f), 0),
            };
            _menuView.Add(topView);

            var bottomView = new View
            {
                Layout = new FlexLayout
                {
                    Direction = FlexLayout.FlexDirection.Column,
                },
            };
            _menuView.Add(bottomView);

            _btnViews = new List<View>();
            foreach (var btn in menuItems)
            {
                var menuBtn = new MenuButton(this)
                {
                    Text = btn.Name,
                    Key = btn.Key,
                };
                menuBtn.FocusGained += MenuBtn_FocusGained;

                if (btn.Position == "top")
                {
                    topView.Add(menuBtn.View);
                    topView.SizeHeight += _collapsedWidth;
                }
                else
                {
                    bottomView.Add(menuBtn.View);
                    bottomView.SizeHeight += _collapsedWidth;
                }

                if (btn.Key == "account")
                    _accountBtn = menuBtn;

                if (btn.Key == "schedule")
                {
                    _activeBtn = menuBtn;
                    ActiveButtonChanged?.Invoke(this, EventArgs.Empty);
                }
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
            _animation.AnimateTo(_main.View, "PositionX", destination, _easeOut);
            _animation.AnimateTo(_main.TitleView, "PositionX", destination, _easeOut);
            _animation.Play();
        }

        private void UserService_UserChanged(object sender, EventArgs e)
        {
            if (sender is UserService userService)
                if (userService.User != null)
                {
                    _accountBtn.IconURL = "user";
                    _accountBtn.Text = "Аккаунт";
                }
                else
                {
                    _accountBtn.IconURL = "account";
                    _accountBtn.Text = "Войти";
                }
        }
    }

    class Button
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
    }
}
