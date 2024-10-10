namespace SpyRadioStationApi.Models.db
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
