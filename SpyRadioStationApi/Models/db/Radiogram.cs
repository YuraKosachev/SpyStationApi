namespace SpyRadioStationApi.Models.db
{
    public class Radiogram
    {
        public int Id { get; set; }
        public required string Message { get; set; }

        public string? Encode { get; set; }

        public DateTime CreateAt { get; set; }

    }
}
