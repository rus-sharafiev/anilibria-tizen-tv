using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Tizen.Applications;

namespace AnilibriaAppTizen.Services
{
    public class LocalSettingsService
    {
        private readonly FileService _fileService;

        private readonly string _applicationDataFolder = Application.Current.DirectoryInfo.Data;
        private readonly string _localsettingsFile = "localSettings.json";

        private IDictionary<string, object> _settings;

        private bool _isInitialized;

        public LocalSettingsService(FileService fileService)
        {
            _fileService = fileService;
            _settings = new Dictionary<string, object>();
        }

        private async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                _settings = await Task.Run(() => _fileService.Read<IDictionary<string, object>>(_applicationDataFolder, _localsettingsFile)) ?? new Dictionary<string, object>();
                _isInitialized = true;
            }
        }

        public async Task<T> ReadSettingAsync<T>(string key)
        {
            await InitializeAsync();

            if (_settings != null && _settings.TryGetValue(key, out var value))
            {
                return JsonSerializer.Deserialize<T>((string)value);
            }
            return default;
        }

        public async Task SaveSettingAsync<T>(string key, T value)
        {
            await InitializeAsync();

            _settings[key] = JsonSerializer.Serialize(value);
            await Task.Run(() => _fileService.Save(_applicationDataFolder, _localsettingsFile, _settings));
        }
    }
}