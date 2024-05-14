using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace AnilibriaAppTizen.Services
{
    public class HttpDataService
    {
        private readonly Dictionary<string, object> responseCache;
        private readonly HttpClient client;
        private readonly HttpClientHandler handler;
        public CookieContainer CookieContainer { get; set; } = new CookieContainer();

        public readonly JsonSerializerOptions serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public HttpDataService(string defaultBaseUrl = "")
        {
            handler = new HttpClientHandler() { CookieContainer = CookieContainer };
            client = new HttpClient(handler);

            if (!string.IsNullOrEmpty(defaultBaseUrl))
            {
                client.BaseAddress = new Uri($"{defaultBaseUrl}/");
            }

            responseCache = new Dictionary<string, object>();
        }

        // --------------------------------------------------------------------------------

        public async Task<T> GetAsync<T>(string uri, bool forceRefresh = false)
        {
            T result = default;

            if (forceRefresh || !responseCache.TryGetValue(uri, out var value))
            {
                var json = await client.GetStringAsync(uri);
                result = await Task.Run(() => JsonSerializer.Deserialize<T>(json, serializeOptions));

                if (responseCache.ContainsKey(uri))
                {
                    responseCache[uri] = result;
                }
                else
                {
                    responseCache.Add(uri, result);
                }
            }
            else
            {
                result = (T)value;
            }

            return result;
        }

        public async Task<T> PostAsJsonAsync<T>(string uri, object item)
        {
            if (item == null)
            {
                return default;
            }

            var serializedItem = JsonSerializer.Serialize(item);
            var response = await client.PostAsync(uri, new StringContent(serializedItem, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            return await Task.Run(() => JsonSerializer.Deserialize<T>(responseBody, serializeOptions));
        }

        public async Task<bool> PostAsync<T>(string uri, T item)
        {
            if (item == null)
            {
                return false;
            }

            var serializedItem = JsonSerializer.Serialize(item);
            var buffer = Encoding.UTF8.GetBytes(serializedItem);
            var byteContent = new ByteArrayContent(buffer);

            var response = await client.PostAsync(uri, byteContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutAsJsonAsync<T>(string uri, T item)
        {
            if (item == null)
            {
                return false;
            }

            var serializedItem = JsonSerializer.Serialize(item);
            var response = await client.PutAsync(uri, new StringContent(serializedItem, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutAsync<T>(string uri, T item)
        {
            if (item == null)
            {
                return false;
            }

            var serializedItem = JsonSerializer.Serialize(item);
            var buffer = Encoding.UTF8.GetBytes(serializedItem);
            var byteContent = new ByteArrayContent(buffer);

            var response = await client.PutAsync(uri, byteContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string uri)
        {
            var response = await client.DeleteAsync(uri);
            return response.IsSuccessStatusCode;
        }

        public CookieContainer GetCookieCollection() => handler.CookieContainer;
        public void RemoveCookies(Uri uri)
        {
            handler.CookieContainer.Add(uri, new CookieCollection());
        }

        public void AddAuthorizationHeader(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = null;
                return;
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}