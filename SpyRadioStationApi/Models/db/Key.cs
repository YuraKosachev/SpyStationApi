namespace SpyRadioStationApi.Models.db
{
    public class Key
    {
        public int Id { get; set; }
        public char Slow { get; set; }
        public char Medium { get; set; }
        public char Fast { get; set; }
        public int Day { get; set; }
        public IDictionary<char,char> Plugboard { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
