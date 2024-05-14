using System;
using System.Text.Json.Serialization;

namespace AnilibriaAppTizen.Models
{
    public partial class Release
    {
        public long Id { get; set; }
        public ValueDescription Type { get; set; }
        public long Year { get; set; }
        public Name Name { get; set; }
        public string Alias { get; set; }
        public ValueDescription Season { get; set; }
        public Poster Poster { get; set; }
        [JsonPropertyName("fresh_at")]
        public DateTimeOffset? FreshAt { get; set; }
        [JsonPropertyName("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
        [JsonPropertyName("is_ongoing")]
        public bool IsOngoing { get; set; }
        [JsonPropertyName("age_rating")]
        public AgeRating AgeRating { get; set; }
        [JsonPropertyName("publish_day")]
        public PublishDay PublishDay { get; set; }
        public string Description { get; set; }
        public string Notification { get; set; }
        [JsonPropertyName("episodes_total")]
        public long? EpisodesTotal { get; set; }
        [JsonPropertyName("external_player")]
        public string ExternalPlayer { get; set; }
        [JsonPropertyName("is_in_production")]
        public bool IsInProduction { get; set; }
        [JsonPropertyName("is_blocked_by_geo")]
        public bool IsBlockedByGeo { get; set; }
        [JsonPropertyName("episodes_are_unknown")]
        public bool EpisodesAreUnknown { get; set; }
        [JsonPropertyName("is_blocked_by_copyrights")]
        public bool IsBlockedByCopyrights { get; set; }
        [JsonPropertyName("added_in_users_favorites")]
        public long AddedInUsersFavorites { get; set; }
        [JsonPropertyName("average_duration_of_episode")]
        public long? AverageDurationOfEpisode { get; set; }
        public Genre[] Genres { get; set; }
        public Member[] Members { get; set; }
        public Sponsor Sponsor { get; set; }
        public Episode[] Episodes { get; set; }
        public Torrent[] Torrents { get; set; }
    }

    public partial class AgeRating
    {
        public string Value { get; set; }
        public string Label { get; set; }
        [JsonPropertyName("is_adult")]
        public bool IsAdult { get; set; }
        public string Description { get; set; }
    }

    public partial class Episode
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public long Ordinal { get; set; }
        public TimeInterval Opening { get; set; }
        public TimeInterval Ending { get; set; }
        public Poster Preview { get; set; }
        [JsonPropertyName("hls_480")]
        public Uri Hls480 { get; set; }
        [JsonPropertyName("hls_720")]
        public Uri Hls720 { get; set; }
        [JsonPropertyName("hls_1080")]
        public Uri Hls1080 { get; set; }
        public long Duration { get; set; }
        [JsonPropertyName("rutube_id")]
        public string RutubeId { get; set; }
        [JsonPropertyName("youtube_id")]
        public string YoutubeId { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
        [JsonPropertyName("sort_order")]
        public long SortOrder { get; set; }
        [JsonPropertyName("name_english")]
        public string NameEnglish { get; set; }
    }

    public partial class TimeInterval
    {
        public object Stop { get; set; }
        public object Start { get; set; }
    }

    public partial class Poster
    {
        public string Src { get; set; }
        public string Thumbnail { get; set; }
        public PosterOptimized Optimized { get; set; }
    }

    public partial class PosterOptimized
    {
        public string Src { get; set; }
        public string Thumbnail { get; set; }
    }

    public partial class Genre
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Image Image { get; set; }
        [JsonPropertyName("total_releases")]
        public long TotalReleases { get; set; }
    }

    public partial class Image
    {
        public string Preview { get; set; }
        public string Thumbnail { get; set; }
        public ImageOptimized Optimized { get; set; }
    }

    public partial class ImageOptimized
    {
        public string Preview { get; set; }
        public string Thumbnail { get; set; }
    }

    public partial class Member
    {
        public Guid Id { get; set; }
        public ValueDescription Role { get; set; }
        public string Nickname { get; set; }
        public User User { get; set; }
    }

    public partial class ValueDescription
    {
        public string Value { get; set; }
        public string Description { get; set; }
    }

    public partial class Name
    {
        public string Main { get; set; }
        public string English { get; set; }
        public string Alternative { get; set; }
    }

    public partial class PublishDay
    {
        public int Value { get; set; }
        public string Description { get; set; }
    }

    public partial class Sponsor
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [JsonPropertyName("url_title")]
        public string UrlTitle { get; set; }
        public Uri Url { get; set; }
    }

    public partial class Torrent
    {
        public long Id { get; set; }
        public string Hash { get; set; }
        public long Size { get; set; }
        public ValueDescription Type { get; set; }
        public string Label { get; set; }
        public ValueDescription Codec { get; set; }
        public ValueDescription Color { get; set; }
        public string Magnet { get; set; }
        public long Seeders { get; set; }
        public ValueDescription Quality { get; set; }
        public object Bitrate { get; set; }
        public string Filename { get; set; }
        public long Leechers { get; set; }
        [JsonPropertyName("sort_order")]
        public long SortOrder { get; set; }
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
        public string Description { get; set; }
        [JsonPropertyName("completed_times")]
        public long CompletedTimes { get; set; }
    }
}

