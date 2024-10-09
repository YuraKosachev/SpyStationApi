using Coravel.Invocable;
using SpyRadioStationApi.Interfaces.Services;
using System.Diagnostics;

namespace SpyRadioStationApi.Jobs
{
    public class PreparingCodeMessageJob : IInvocable
    {
        private readonly ICodeService _codeService;
        public PreparingCodeMessageJob(ICodeService codeService) 
        {
            _codeService = codeService;
        }
        public async Task Invoke()
        {
            Debug.WriteLine("JOB JOB !!");
            await Task.CompletedTask;
        }
    }
}
