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

        private readonly string _resPath;
        private View _menuView;

        private MenuButton _activeBtn;
        public event EventHandler BtnFocusGained;
        public event EventHandler BtnFocusLost;

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

        public Menu(string sharedResource) 
        {
            _resPath = sharedResource;
        }

        private readonly Button[] menuItems =
        {
            new Button() {Key = "logo", Name = "Anilibria", Icon = "anilibria"},
            new Button() {Key = "home", Name = "Главная", Icon = "home"},
            new Button() {Key = "schedule", Name = "Расписание", Icon = "calendar"},
            new Button() {Key = "settings", Name = "Настройки", Icon = "settings"},
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
            FocusManager.Instance.SetAsFocusGroup(_menuView, true);
            FocusManager.Instance.FocusGroupChanged += FocusInstance_FocusGroupChanged;
            view.Add(_menuView);

            List<View> btnViews = new List<View>();
            foreach (var btn in menuItems)
            {
                var menuBtn = new MenuButton(this)
                {
                    IconUrl = _resPath + "icons/" + btn.Icon + ".svg",
                    ActiveIconUrl = _resPath + "icons/" + btn.Icon + "_black.svg",
                    Text = btn.Name,
                    Key = btn.Key,
                };
                _menuView.Add(menuBtn.View);
                menuBtn.FocusGained += MenuBtn_FocusGained;
                menuBtn.FocusLost += MenuBtn_FocusLost;
                if (btn.Key == "home")
                    _activeBtn = menuBtn;

                if (btn.Key != "logo")
                    btnViews.Add(menuBtn.View);
            }

            for (int j = 0; j < btnViews.Count; j++)
            {
                if (j != 0)
                    btnViews[j].UpFocusableView = btnViews[j - 1];

                if (j < btnViews.Count - 1)
                    btnViews[j].DownFocusableView = btnViews[j + 1];
            }


        }

        private void FocusInstance_FocusGroupChanged(object sender, FocusManager.FocusGroupChangedEventArgs e)
        {
            Debug.WriteLine($"focus group {e.CurrentView.Name}");
        }

        private void MenuBtn_FocusGained(object sender, EventArgs e)
        {
            if (sender is MenuButton menuButton)
            {
                _activeBtn = menuButton;
                BtnFocusGained.Invoke(this, EventArgs.Empty);
            }
        }

        private void MenuBtn_FocusLost(object sender, EventArgs e)
        {
            BtnFocusLost.Invoke(this, EventArgs.Empty);
        }

    }

    class Button
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}
