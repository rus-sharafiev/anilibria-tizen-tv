using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tizen.Applications;

namespace AnilibriaAppTizen.Services
{

    internal class ImageService
    {
        private const string _baseUri = "https://anilibria.top";

        private readonly string _imageCacheFolder = Path.Combine(Application.Current.DirectoryInfo.Data, "cachedImages");
        private bool _isInitialized;

        private List<string> _imagesList;
        private readonly HttpClient client = new HttpClient();

        public ImageService()
        {
            _imagesList = new List<string>();
        }

        private async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                if (Directory.Exists(_imageCacheFolder))
                    _imagesList = await Task.Run(() => Directory.GetFiles(_imageCacheFolder).ToList() ?? new List<string>());
                else
                    Directory.CreateDirectory(_imageCacheFolder);

                _isInitialized = true;
            }
        }

        private bool CacheContains(string imagePath)
        {
            var filePath = Path.Combine(_imageCacheFolder, Path.GetFileName(imagePath));
            return _imagesList.Contains(filePath);
        }

        private string GetCachedImage(string imagePath)
        {
            return Path.Combine(_imageCacheFolder, Path.GetFileName(imagePath));
        }

        private async Task<string> CacheImageAsync(string imagePath)
        {
            var filePath = Path.Combine(_imageCacheFolder, Path.GetFileName(imagePath));

            var stream = await client.GetStreamAsync(_baseUri + imagePath);
            var fileStream = new FileStream(filePath, FileMode.CreateNew);
            await stream.CopyToAsync(fileStream);

            _imagesList.Add(filePath);
            return filePath;
        }

        public async Task<string> GetPath(string imagePath)
        {
            await InitializeAsync();
            //return _baseUri + imagePath;

            if (CacheContains(imagePath))
                return GetCachedImage(imagePath);
            else
            {
                _ = CacheImageAsync(imagePath);
                return _baseUri + imagePath;
            }
        }

        public List<string> GetImgs()
        {
            return _imagesList;
        }
    }
}