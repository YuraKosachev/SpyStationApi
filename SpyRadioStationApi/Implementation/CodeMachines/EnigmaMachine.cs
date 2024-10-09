using SpyRadioStationApi.Interfaces.CodeMachines;
using SpyRadioStationApi.Models;
using System.Diagnostics.Metrics;
using System.Text;

namespace SpyRadioStationApi.Implementation.CodeMachines
{
    public class EnigmaMachine : ICodeMachine
    {
        private readonly string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private readonly string _fastRotorStart = "JGDQOXUSCAMIFRVTPNEWKBLZYH";
        private readonly string _mediumRotorStart = "NTZPSFBOKMWRCJDIVLAEYUXHGQ";
        private readonly string _slowRotorStart = "JVIUBHTCDYAKEQZPOSGXNRMWFL";
        private readonly Dictionary<char, char> _reflector = new Dictionary<char, char> {
            { 'A','Y' },{ 'Y','A' },{ 'B','R' },{ 'R','B' },{ 'C','U' },{ 'U','C' },
            { 'D','H' },{ 'H','D' },{ 'E','Q' },{ 'Q','E' },{ 'F','S' },{ 'S','F' },
            { 'G','L' },{ 'L','G' },{ 'I','P' },{ 'P','I' },{ 'J','X' },{ 'X','J' },
            { 'K','N' },{ 'N','K' },{ 'M','O' },{ 'O','M' },{ 'T','Z' },{ 'Z','T' },
            { 'V','W' },{ 'W','V' }
        };

        public string Encode(string src, Settings settings)
        {
            if (src == null) return null;

            StringBuilder builder = new StringBuilder();
            src = src.ToUpper();
            var fastRotorRewind = Rewind(settings.FastRotorLetter, _fastRotorStart).ToCharArray();
            var mediumRotorRewind = Rewind(settings.MediumRotorLetter, _mediumRotorStart).ToCharArray();
            var slowRotorRewind = Rewind(settings.SlowRotorLetter, _slowRotorStart).ToCharArray();

            for (int i = 0; i < src.Length; i++)
            {

                var letter = src[i];
                letter = GetPlugboardValue(letter, settings.Plugboard);
                    

                letter = GetRotorsValue(fastRotorRewind, mediumRotorRewind, slowRotorRewind, letter);

                letter = GetPlugboardValue(letter, settings.Plugboard);

                builder.Append(letter);

                Rotate(fastRotorRewind);
                if (_fastRotorStart[0] == fastRotorRewind[0])
                {
                    Rotate(mediumRotorRewind);
                    if (_mediumRotorStart[0] == mediumRotorRewind[0])
                    {
                        Rotate(slowRotorRewind);
                    }
                }

            }

            return builder.ToString();
        }

        private char GetRotorsValue(char[] fastRotor, char[] mediumRotor, char[] slowRotor, char input)
        {
            if (!Char.IsLetter(input))
                return input;
            // In fast rotor
            int letterPositionFRIn = Array.IndexOf(fastRotor, input);
            char fastRotorLetterIn = _alphabet[letterPositionFRIn]; // out fast rotor char
           
            //In medium rotor
            int letterPositionMRIn = Array.IndexOf(mediumRotor, fastRotorLetterIn);
            char mediumRotorLetterIn = _alphabet[letterPositionMRIn];// out medium rotor char

            //In slow rotor
            int letterPositionSRIn = Array.IndexOf(slowRotor, mediumRotorLetterIn);
            char slowRotorLetterIn = _alphabet[letterPositionSRIn];// out medium rotor char

            //reflector
            char reflectorOut = _reflector[slowRotorLetterIn];

            int alphabetIndexSlow = _alphabet.IndexOf(reflectorOut);

            //Out slow rotor
            char slowRotorLetterOut = slowRotor[alphabetIndexSlow];

            //Out medium rotor
            int alphabetIndexMedium = _alphabet.IndexOf(slowRotorLetterOut);
            char mediumRotorLetterOut = mediumRotor[alphabetIndexMedium];

            //Out fast rotor
            int alphabetIndexFast = _alphabet.IndexOf(mediumRotorLetterOut);

            return fastRotor[alphabetIndexFast];
        }

        private void Rotate(char[] rotor)
        {
            char? tmp = null;

            for (int i = 0; i < rotor.Length; i++)
            {
                if (i == 0)
                {
                    tmp = rotor[i];
                    continue;
                }

                var swap = rotor[i];
                rotor[i] = tmp.Value;
                tmp = swap;

                if (rotor.Length - 1 == i)
                {
                    rotor[0] = tmp.Value;
                }
            }
        }

        private char GetPlugboardValue(char src, IDictionary<char,char>? plugboard) {
            return plugboard?.ContainsKey(src) == true ? plugboard[src] : src;
        }
        private string Rewind(char start, string src)
        {
            int index = src.IndexOf(start);
            return src.Substring(index, src.Length - index) + src.Substring(0, index);
        }

    }
}
