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
        public string GetCodeMessage(string src)
        {
            //TO DO get text for coding 
            var settings = new Settings
            {
                FastRotorLetter = 'A',
                SlowRotorLetter = 'A',
                MediumRotorLetter = 'A'
            };
            return _codeMachine.Encode(src, settings);
        }
    }
}
