using Microsoft.AspNetCore.Http.HttpResults;
using SpyRadioStationApi.Models.db;
using SpyRadioStationApi.Models.enums;
using System.Text.Json;

namespace SpyRadioStationApi.Implementation.Mapping
{
    public static class Mapper
    {
        public static Key ToKey(dynamic src)
        {
            if (src == null) return null;

            return new Key
            {
                Id = (int)src.Id,
                Fast = (char)src.Fast,
                Slow = (char)src.Slow,
                Medium = (char)src.Medium,
                CreateAt = DateTime.Parse(src.CreateAt),
                Day = (int)src.Day,
                Plugboard = src.Plugboard != null
                    ? JsonSerializer.Deserialize<IDictionary<char, char>>((string)src.Plugboard)
                    : null

            };
        }

        public static Radiogram ToRadiogram(dynamic src) {

            if (src == null) return null;

            return new Radiogram
            {
                Id = (int)src.Id,
                Message = (string)src.Message,
                Encode = (string)src.Encode,
                CreateAt = DateTime.Parse(src.CreateAt)
            };
        }

        public static Notification ToNotification(dynamic src) {
            if (src == null) return null;


            return new Notification
            {
                Id = (int)src.Id,
                Message = (string)src.Message,
                Status = (Status)src.Status,
                NotificationType = (NotificationType)src.Type,
                CreateAt = DateTime.Parse(src.CreateAt)
            };
        }
    }
}
