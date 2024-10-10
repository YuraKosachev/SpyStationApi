using SpyRadioStationApi.Models.enums;

namespace SpyRadioStationApi.Models.db
{
    public class Notification : BaseEntity
    {
        public required string Message { get; set; }
        public Status Status { get; set; }
        public NotificationType NotificationType { get; set; }
    }
}
