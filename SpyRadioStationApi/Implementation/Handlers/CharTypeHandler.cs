using Dapper;
using System.Data;
using System.Text.Json;

namespace SpyRadioStationApi.Implementation.Handlers
{
    public class CharTypeHandler : SqlMapper.TypeHandler<char>
    {
        public override char Parse(object value)
        {
            return char.Parse(value.ToString());
        }
        public override void SetValue(IDbDataParameter parameter, char value)
        {
            parameter.Value = (int)value;
        }
    }
}
