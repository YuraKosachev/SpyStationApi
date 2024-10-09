using SpyRadioStationApi.Models;

namespace SpyRadioStationApi.Interfaces.CodeMachines
{
    public interface ICodeMachine
    {
        string Encode(string src, Settings settings);
    }
}
