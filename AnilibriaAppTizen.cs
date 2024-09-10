using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using System.Diagnostics;
using AnilibriaAppTizen.Services;
using AnilibriaAppTizen.Views;

namespace AnilibriaAppTizen
{
    internal class Program : NUIApplication
    {
        private ApiService _apiService;
        private FileService _fileService;
        private ImageService _imageService;
        private LocalSettingsService _localSettingsService;
        private UserService _userService;

        private Main _main;
        private Release _release;
        private Player _player;

        /// <summary>
        /// Initialize application
        /// </summary>
        async void Initialize()
        {
            // Add custom fonts
            FontClient.Instance.AddCustomFontDirectory(DirectoryInfo.SharedResource + "fonts/");

            // Add main key event
            Window.Instance.KeyEvent += OnKeyEvent;
            Window.Instance.BackgroundColor = Color.Transparent;

            // Create services
            _apiService = new ApiService();
            _fileService = new FileService();
            _imageService = new ImageService();
            _localSettingsService = new LocalSettingsService(_fileService);
            _userService = new UserService(_localSettingsService, _apiService);

            // Views
            _player = new Player();
            _release = new Release(_apiService, _imageService, _player);
            _main = new Main(_apiService, _imageService, _userService, _release);

            // Init user session
            await _userService.InitializeAsync();

            // Init views
            _main.Initialize();
            _release.Initialize();
            _player.Initialize(_release, _main);

            FocusManager.Instance.FocusIndicator = new View();
            FocusManager.Instance.FocusChanged += FocusManager_FocusChanged;
            FocusManager.Instance.SetCurrentFocusView(_main.ActiveMenuButton.View);
        }

        /// <summary>
        /// The method called when key pressed down
        /// </summary>
        /// <param name="sender">Key instance</param>
        /// <param name="e">Key's args</param>
        private void OnKeyEvent(object sender, Window.KeyEventArgs e)
        {
            // Home - XF86Home
            // play/pause - XF86PlayBack
            // next - XF86RaiseChannel
            // prev - XF86LowerChannel
            // ch_push - XF86ChannelGuide
            // Right, Down, Left, Up, Return

            if (e.Key.State == Key.StateType.Down)
                if (_player.IsActive)
                {
                    var player = _player.Instance;
                    switch (e.Key.KeyPressedName)
                    {
                        case "XF86PlayBack":  
                            if (player.State == Tizen.Multimedia.PlayerState.Playing)
                                player.Pause();
                            else if (player.State == Tizen.Multimedia.PlayerState.Paused)
                                player.Start();
                            break;

                        case "Right":
                            player.SetPlayPositionAsync(player.GetPlayPosition() + 5000, false);
                            break;

                        case "Left":
                            player.SetPlayPositionAsync(player.GetPlayPosition() - 5000, false);
                            break;

                        case "XF86Back":
                            _player.Dispose();
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    if (e.Key.KeyPressedName == "Escape" || e.Key.KeyPressedName == "XF86Back")
                    {
                        if (_release.IsActive)
                            _release.Dispose();
                        else
                            Exit();
                    }
                }
        }

        /// <summary>
        /// The method called when focus changed
        /// </summary>
        /// <param name="sender">FocusManager instance</param>
        /// <param name="e">FocusManager's args</param>
        private void FocusManager_FocusChanged(object sender, FocusManager.FocusChangedEventArgs e)
        {
            if (e.NextView != null)
            {
                Debug.WriteLine($"{e.NextView.Name}");
            }
        }

        /// <summary>
        /// The method called when application created
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
            Initialize();
        }

        /// <summary>
        /// Main function.
        /// </summary>
        /// <param name="args">arguments of Main (entry point)</param>
        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
