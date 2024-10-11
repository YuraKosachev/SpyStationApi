namespace SpyRadioStationApi.Interfaces.db
{
    public interface IDatabaseBootstrap
    {
        Task<bool> Setup();
    }
}
