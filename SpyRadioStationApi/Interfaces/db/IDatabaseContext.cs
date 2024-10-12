using System.Data;

namespace SpyRadioStationApi.Interfaces.db
{
    public interface IDatabaseContext
    {
        IDbConnection CreateConnection();
        Task Init();
    }
}
