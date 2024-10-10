using SpyRadioStationApi.Interfaces.CodeMachines;
using SpyRadioStationApi.Interfaces.Services;
using SpyRadioStationApi.Models;

namespace SpyRadioStationApi.Implementation.Services
{
    public class CodeService : ICodeService
    {
        private readonly ICodeMachine _codeMachine;
        public CodeService(ICodeMachine codeMachine)
        {
            _codeMachine = codeMachine;
        }
        public string GetCodeMessage(string src, Settings settings)
        {
            return _codeMachine.Encode(src, settings);
        }
    }
}
