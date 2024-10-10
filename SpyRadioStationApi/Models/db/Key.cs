namespace SpyRadioStationApi.Models.db
{
    public class Key : BaseEntity
    {
        public char Slow { get; set; }
        public char Medium { get; set; }
        public char Fast { get; set; }
        public int Day { get; set; }
        public IDictionary<char,char> Plugboard { get; set; }
    }
}
