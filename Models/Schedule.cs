using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static Tizen.Pims.Calendar.CalendarTypes;

namespace AnilibriaAppTizen.Models
{

    public partial class ScheduleRelease
    {
        public Release Release { get; set; }
        [JsonPropertyName("new_release_episode")]
        public Episode NewReleaseEpisode { get; set; }
        [JsonPropertyName("new_release_episode_ordinal")]
        public long NewReleaseEpisodeOrdinal { get; set; }
    }

    public class WeekDay
    {
        public string Name { get; set; }
        public bool IsFirst { get; set; }
        public List<Release> Releases { get; set; }
    }

    public class WeekDays
    {
        private WeekDay[] _weekDays;

        public WeekDay[] Create()
        {
            _weekDays = new WeekDay[7]
            {
                new WeekDay() { Name = "Понедельник", IsFirst = true, Releases = new List<Release>() },
                new WeekDay() { Name = "Вторник", Releases = new List<Release>() },
                new WeekDay() { Name = "Среда", Releases = new List<Release>() },
                new WeekDay() { Name = "Четверг", Releases = new List<Release>() },
                new WeekDay() { Name = "Пятница", Releases = new List<Release>() },
                new WeekDay() { Name = "Суббота", Releases = new List<Release>() },
                new WeekDay() { Name = "Воскресенье", Releases = new List<Release>() },
            };

            return _weekDays;
        }
    }
}
