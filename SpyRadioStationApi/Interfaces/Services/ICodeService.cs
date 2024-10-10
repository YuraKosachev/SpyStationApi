using SpyRadioStationApi.Models;

namespace SpyRadioStationApi.Interfaces.Services
{
    public interface ICodeService
    {
        string GetCodeMessage(string src, Settings settings);
    }
}
