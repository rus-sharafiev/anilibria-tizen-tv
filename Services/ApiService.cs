using System;
using System.Net;
using System.Threading.Tasks;
using AnilibriaAppTizen.Models;

namespace AnilibriaAppTizen.Services
{
    public class ApiService
    {
        private readonly HttpDataService _instance = new HttpDataService();
        private readonly string _apiUrl = "https://anilibria.top/api/v1";

        /// <summary>
        /// GET auth user
        /// </summary>
        /// <param name="AccessToken">access token string</param>
        /// <returns></returns>
        public async Task<User> GetUserAsync(string accessToken)
        {
            _instance.AddAuthorizationHeader(accessToken);
            var response = await _instance.GetAsync<User>(_apiUrl + "/accounts/users/me/profile", false);

            await Task.CompletedTask;
            return response;
        }

        /// <summary>
        /// Sign Up
        /// </summary>
        /// <param name="signUpDto">Object with login and password</param>
        /// <returns></returns>
        public async Task<UserToken> SignUpAsync(SignUpDto signUpDto)
        {
            var response = await _instance.PostAsJsonAsync<UserToken>(_apiUrl + "/accounts/users/me/profile", signUpDto);

            await Task.CompletedTask;
            return response;
        }

        /// <summary>
        /// GET schedule
        /// </summary>
        /// <param name="forceRefresh">Whether to use data from a previous request </param>
        /// <returns></returns>
        public async Task<ScheduleRelease[]> GetScheduleAsync(bool forceRefresh = false)
        {
            var response = await _instance.GetAsync<ScheduleRelease[]>(_apiUrl + "/anime/schedule/week", forceRefresh);

            await Task.CompletedTask;
            return response;
        }

        public CookieCollection GetCookies() =>
            _instance.GetCookieCollection().GetCookies(new Uri("https://wwnd.space"));

        public void RemoveCookies() =>
            _instance.CookieContainer.Add(new Uri(_apiUrl), new Cookie("PHPSESSID", string.Empty));
    }
}