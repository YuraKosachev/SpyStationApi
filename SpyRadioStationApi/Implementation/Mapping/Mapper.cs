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
                Id = (int)src.id,
                Fast = Char.Parse(src.fast),
                Slow = Char.Parse(src.slow),
                Medium = Char.Parse(src.medium),
                CreateAt = (DateTime)src.createat,
                Day = (int)src.day,
                Plugboard = src.plugboard != null
                    ? JsonSerializer.Deserialize<IDictionary<char, char>>((string)src.plugboard)
                    : null
            };
        }

        public static Radiogram ToRadiogram(dynamic src)
        {
            if (src == null) return null;

            return new Radiogram
            {
                Id = (int)src.id,
                Message = (string)src.message,
                Encode = (string)src.encode,
                CreateAt = (DateTime)src.createat
            };
        }

        public static Notification ToNotification(dynamic src)
        {
            if (src == null) return null;

            return new Notification
            {
                Id = (int)src.id,
                Message = (string)src.message,
                Status = (Status)src.status,
                NotificationType = (NotificationType)src.type,
                CreateAt = (DateTime)src.createat
            };
        }
    }
}