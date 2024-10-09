namespace SpyRadioStationApi.Models
{
    public class Settings
    {
        public char FastRotorLetter { get; init; }
        public char MediumRotorLetter { get; init; }
        public char SlowRotorLetter { get; init; }
        public IDictionary<char, char>? Plugboard { get; set; }
    }
}
