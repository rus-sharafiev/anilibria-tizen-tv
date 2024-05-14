using System;
using System.Diagnostics;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class MenuButton
    {
        private View _btn;
        private string _iconUrl = string.Empty;
        private string _activeIconUrl = string.Empty;
        private string _text = string.Empty;
        private string _key = string.Empty;
        private readonly int _iconSize = 48;

        private ImageView _icon;
        private readonly Menu _menu;

        public event EventHandler FocusGained;
        public event EventHandler FocusLost;

        public string IconUrl
        {
            get { return _iconUrl; }
            set { _iconUrl = value; }
        }

        public string ActiveIconUrl
        {
            get { return _activeIconUrl; }
            set {  _activeIconUrl = value; }
        }

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


        public MenuButton(Menu menu) 
        { 
            _menu = menu;
        }

        private void OnIntialize()
        {
            _btn = new View
            {
                Focusable = Key != "logo",
                SizeHeight = _menu.CollapsedWidth,
                SizeWidth = _menu.CollapsedWidth,
                Name = _key,
                Opacity = Key != "logo" ? 0.5f : 1,
            };

            _icon = new ImageView
            {
                ResourceUrl = _iconUrl,
                Size2D = new Size2D(_iconSize, _iconSize),
                PositionX = (_menu.CollapsedWidth - _iconSize) / 2,
                PositionY = (_menu.CollapsedWidth - _iconSize) / 2,
            };
            _btn.Add(_icon);

            TextLabel label = new TextLabel
            {
                Text = _text,
                FontFamily = "Roboto",
                PixelSize = 28,
                TextColor = Color.White,
                PositionX = _menu.CollapsedWidth,
                SizeHeight = _menu.CollapsedWidth,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _btn.Add(label);

            _btn.FocusGained += (obj, e) =>
            {
                //_icon.SetImage(_activeIconUrl);
                AnimateOpacityTo(1);

                FocusGained.Invoke(this, EventArgs.Empty);
            };
            _btn.FocusLost += (obj, e) =>
            {
                //_icon.SetImage(_iconUrl);
                AnimateOpacityTo(0.5f);

                FocusLost?.Invoke(this, EventArgs.Empty);
            };
            _btn.RightFocusableView = null; 
            _btn.LeftFocusableView = null;

        }

        public View GetView()
        {
            return _btn;
        }

        public void AddTo(View parent)
        {
            parent.Add(_btn);
        }

        private Animation AnimateOpacityTo(float destination)
        {
            var animation = new Animation(140);
            animation.AnimateTo(_btn, "Opacity", destination);
            animation.Play();
            return animation;
        }
    }
}
