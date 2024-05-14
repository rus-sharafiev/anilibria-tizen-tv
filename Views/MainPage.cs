using AnilibriaAppTizen.Services;
using System.Diagnostics;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class MainPage
    {
        private readonly int _titleHeight = 80;
        private readonly int _fontSize = 52;

        #pragma warning disable CS0618
        private readonly int windowSizeWidth = Window.Instance.Size.Width;
        private readonly int windowSizeHeight = Window.Instance.Size.Height;
        #pragma warning restore CS0618

        private readonly ApiService _apiService;
        private readonly ImageService _imageService;

        private readonly Release _release;
        private readonly Schedule _schedule;
        private readonly Home _home;
        private readonly Settings _settings;

        private View _view;
        private Menu _menu;
        private View _mainPageView;
        private View _mainTitleView;
        private TextLabel _title;

        public View View
        {
            get { return _mainPageView; }
        }

        public Menu Menu
        {
            get { return _menu; }
        }

        public MenuButton ActiveMenuButton
        {
            get { return _menu.ActiveButton; }
        }

        private readonly string _resPath;

        private Animation _opacityAnimation;


        public MainPage(string resPath, ApiService apiService, ImageService imageService, Release releaseView)
        {
            _resPath = resPath;
            _apiService = apiService;
            _imageService = imageService;
            _release = releaseView;

            _schedule = new Schedule(apiService, imageService);
            _home = new Home(apiService, imageService);
            _settings = new Settings(apiService, imageService);
        }

        public void Initialize()
        {
            _view = new View();

            _menu = new Menu(_resPath);
            _menu.RenderTo(_view);
            _menu.BtnFocusGained += MenuView_BtnFocusGained;
            _menu.BtnFocusLost += MenuView_BtnFocusLost;

            _mainPageView = new View
            {
                SizeHeight = windowSizeHeight - _titleHeight,
                SizeWidth = windowSizeWidth - _menu.CollapsedWidth,
                PositionY = _titleHeight,
                PositionX = _menu.CollapsedWidth,
                ClippingMode = ClippingModeType.ClipChildren,
                Name = "Main view"
            };
            _view.Add(_mainPageView);
            _mainPageView.ChildAdded += MainView_ChildAdded;

            _mainTitleView = new View
            {
                SizeHeight = _titleHeight,
                SizeWidth = windowSizeWidth - _menu.CollapsedWidth,
                PositionX = _menu.CollapsedWidth,
                Name = "Main title view"
            };
            _view.Add(_mainTitleView);

            _title = new TextLabel
            {
                Text = "",
                TextColor = new Color(255, 255, 255, 0.5f),
                PixelSize = _fontSize,
                Padding = new Extents(25, 0, 0, 0),
                FontFamily = "Roboto Light",
                Name = "Main title label",
                Size = _mainTitleView.Size,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _mainTitleView.Add(_title);

            Window.Instance.Add(_view);
        }

        private void MenuView_BtnFocusGained(object sender, System.EventArgs e)
        {
            switch (_menu.ActiveButton.Key)
            {
                case "home":
                    if (!_home.IsActive)
                    {
                        _home.RenderTo(this, _release);
                    }
                    break;

                case "schedule":
                    if (!_schedule.IsActive)
                    {
                        _schedule.RenderTo(this, _release);
                    }
                    break;

                case "settings":
                    if (!_settings.IsActive)
                    {
                        _settings.RenderTo(this);
                    }
                    break;
            }
            
            if (_menu.View.Children.Any(child => child.HasFocus()))
                AnimateMenuWidthTo(_menu.ExpandedWidth);
        }


        private void MenuView_BtnFocusLost(object sender, System.EventArgs e)
        {
            if (!_menu.View.Children.Any(child => child.HasFocus()))
                AnimateMenuWidthTo(_menu.CollapsedWidth);
        }

        public void SetTitle(string title)
        {
            _mainTitleView.Opacity = 0;
            _title.Text = title;
            AnimateOpacityTo(1, _mainTitleView);
        }

        private Animation AnimateOpacityTo(float destination, View view)
        {
            var animation = new Animation(140);
            animation.AnimateTo(view, "Opacity", destination);
            animation.Play();
            return animation;
        }

        private void MainView_ChildAdded(object sender, View.ChildAddedEventArgs e)
        {
            if (sender is View view && view.ChildCount > 1)
            {
                int lastIndex = (int)(view.ChildCount - 1);
                view.Children[lastIndex].Opacity = 0f;
                AnimateOpacityTo(1, view.Children[lastIndex]);

                for (int i = 0; i < view.Children.Count - 1; i++)
                {
                    _mainTitleView.Opacity = 0;
                    view.Remove(view.Children[i]);
                }
            }
        }

        private void AnimateMenuWidthTo(int destination)
        {
            var easeOut = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseOutSquare);

            var animation = new Animation(280);
            animation.AnimateTo(_menu.View, "SizeWidth", destination, easeOut);
            animation.AnimateTo(_mainPageView, "PositionX", destination, easeOut);
            animation.AnimateTo(_mainTitleView, "PositionX", destination, easeOut);
            animation.Play();
        }
    }
}
