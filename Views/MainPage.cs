using AnilibriaAppTizen.Services;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class MainPage
    {
        private readonly int _titleHeight = 80;

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
        private TextLabel _subTitle;
        private Animation _opacityAnimation;

        public View View
        {
            get { return _mainPageView; }
        }
        public View TitleView
        {
            get { return _mainTitleView; }
        }

        public Menu Menu
        {
            get { return _menu; }
        }

        public MenuButton ActiveMenuButton
        {
            get { return _menu.ActiveButton; }
        }

        public MainPage(ApiService apiService, ImageService imageService, Release releaseView)
        {
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

            _menu = new Menu(this);
            _menu.RenderTo(_view);
            _menu.ActiveButtonChanged += Menu_ActiveButtonChanged;

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
                Name = "Main title view",
            };
            _view.Add(_mainTitleView);
            
            _title = new TextLabel
            {
                Text = "",
                TextColor = new Color(255, 255, 255, 0.5f),
                PointSize = 48,
                FontFamily = "Roboto Light",
                Name = "Main title label",
                VerticalAlignment = VerticalAlignment.Bottom,
                SizeHeight = _mainTitleView.SizeHeight,
                PositionX = 50
            };
            _mainTitleView.Add(_title);

            _subTitle = new TextLabel
            {
                Text = "",
                TextColor = new Color(255, 255, 255, 0.5f),
                PointSize = 24,
                FontFamily = "Roboto Light",
                Name = "Main subtitle label",
                VerticalAlignment = VerticalAlignment.Bottom,
                SizeHeight = _mainTitleView.SizeHeight - 6,
            };
            _mainTitleView.Add(_subTitle);

            Window.Instance.Add(_view);
        }

        private void Menu_ActiveButtonChanged(object sender, System.EventArgs e)
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
        }

        public void SetTitle(string title, string subTitle = "", float subTitlePositionX = 0)
        {
            _mainTitleView.Opacity = 0;
            _title.Text = title;
            _subTitle.PositionX = subTitlePositionX;
            _subTitle.Text = subTitle;
            AnimateOpacityTo(1, _mainTitleView);
        }

        private Animation AnimateOpacityTo(float destination, View view)
        {
            var animation = new Animation(140);
            animation.AnimateTo(view, "Opacity", destination);
            animation.Play();
            animation.Finished += (a, e) =>
            {
                if (a is Animation ani) ani.Dispose();
            };
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
                    view.Remove(view.Children[i]);
                }
            }
        }
    }
}
