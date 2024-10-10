namespace SpyRadioStationApi.Models.db
{
    public class Radiogram : BaseEntity
    {
        public required string Message { get; set; }

        public string? Encode { get; set; }
    }
}
