using System;
using Tizen.Applications;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class MenuButton
    {
        private readonly string _sharedRes = Application.Current.DirectoryInfo.SharedResource;
        private readonly int _iconSize = 32;

        private View _btn;
        private string _iconUrl = string.Empty;
        private string _activeIconUrl = string.Empty;
        private string _text = string.Empty;
        private string _key = string.Empty;

        private VisualView _icon;
        private SVGVisual _iconVisual;
        private readonly Menu _menu;

        public event EventHandler FocusGained;
        public event EventHandler FocusLost;

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public string Key 
        { 
            get { return _key; }
            set { _key = value; OnIntialize(); }
        }

        public View View 
        { 
            get { return _btn; } 
        }

        public View Icon
        {
            get { return _icon; }
        }

        public MenuButton(Menu menu) 
        { 
            _menu = menu;
        }

        private void OnIntialize()
        {
            _iconUrl = _sharedRes + "icons/" + _key + ".svg";
            _activeIconUrl = _sharedRes + "icons/" + _key + "_filled.svg";

            _btn = new View
            {
                Focusable = Key != "logo",
                SizeHeight = _menu.CollapsedWidth,
                SizeWidth = _menu.CollapsedWidth,
                Name = "MenuButton-" + _key,
                Opacity = Key != "logo" ? 0.5f : 1,
            };

            _icon = new VisualView()
            {
                Opacity = 0.8f,
                Size = new Size2D(_iconSize, _iconSize),
                PositionX = (_menu.CollapsedWidth - _iconSize) / 2,
                PositionY = (_menu.CollapsedWidth - _iconSize) / 2,
            };
            _btn.Add(_icon);

            _iconVisual = new SVGVisual
            {
                URL = _iconUrl
            };
            _icon.AddVisual($"{_key}-icon", _iconVisual);

            TextLabel label = new TextLabel
            {
                Text = _text,
                FontFamily = _key == "logo" ? "Roboto Thin" : "Roboto Light",
                PixelSize = _key == "logo" ? 32 : 24,
                TextColor = Color.White,
                PositionX = _menu.CollapsedWidth,
                SizeHeight = _menu.CollapsedWidth,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _btn.Add(label);

            _btn.FocusGained += (obj, e) =>
            {
                AnimateIconScaleTo(1.2f);
                FocusGained.Invoke(this, EventArgs.Empty);
            };
            _btn.FocusLost += (obj, e) =>
            {
                AnimateIconScaleTo(1);
                FocusLost?.Invoke(this, EventArgs.Empty);
            };
            _btn.RightFocusableView = null; 
            _btn.LeftFocusableView = null;

            _menu.ActiveButtonChanged += Menu_ActiveButtonChanged;

            if (_key == "settings")
                FlexLayout.SetFlexAlignmentSelf(_btn, FlexLayout.AlignmentType.FlexEnd);
        }

        private void Menu_ActiveButtonChanged(object sender, EventArgs e)
        {
            if (_menu.ActiveButton == this)
            {
                _iconVisual.URL = _activeIconUrl;
                AnimateOpacityTo(1);
            }
            else if (_iconVisual.URL == _activeIconUrl)
            {
                _iconVisual.URL = _iconUrl;
                AnimateOpacityTo(0.8f);
            }
        }

        public View GetView()
        {
            return _btn;
        }

        public void AddTo(View parent)
        {
            parent.Add(_btn);
        }

        private void AnimateOpacityTo(float destination)
        {
            var animation = new Animation(140);
            animation.AnimateTo(_btn, "Opacity", destination);
            animation.Play();
            animation.Finished += (a, ev) =>
            {
                if (a is Animation ani) ani.Dispose();
            };
        }

        private void AnimateIconScaleTo(float destination)
        {
            var animation = new Animation(140);
            animation.AnimateTo(_btn, "ScaleX", destination);
            animation.AnimateTo(_btn, "ScaleY", destination);
            animation.Play();
            animation.Finished += (a, ev) =>
            {
                if (a is Animation ani) ani.Dispose();
            };
        }
    }
}
