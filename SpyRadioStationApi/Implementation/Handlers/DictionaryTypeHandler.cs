using Dapper;
using SpyRadioStationApi.Models.db;
using System.Data;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.Json;

namespace SpyRadioStationApi.Implementation.Handlers
{
    public class DictionaryTypeHandler : SqlMapper.TypeHandler<IDictionary<char,char>>
    { 
        public override IDictionary<char,char> Parse(object value)
        {
            return JsonSerializer.Deserialize<IDictionary<char,char>>(value.ToString());
        }
        public override void SetValue(IDbDataParameter parameter, IDictionary<char,char> value)
        {
            parameter.Value = JsonSerializer.Serialize(value);
        }
    }
}
