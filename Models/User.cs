using System;
using System.Text.Json.Serialization;

namespace AnilibriaAppTizen.Models
{
    public partial class User
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public Image Avatar { get; set; }
        public string Nickname { get; set; }
        public UserTorrents Torrents { get; set; }
        [JsonPropertyName("is_banned")]
        public bool IsBanned { get; set; }
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonPropertyName("is_with_ads")]
        public bool IsWithAds { get; set; }
    }

    public partial class UserTorrents
    {
        public string Passkey { get; set; }
        public object Uploaded { get; set; }
        public object Downloaded { get; set; }
    }

    public class UserToken
    {
        [JsonPropertyName("token")]
        public string AccessToken { get; set; }
    }
}
