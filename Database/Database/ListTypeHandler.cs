using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Newtonsoft.Json;

namespace Database
{
    /// <summary>
    /// Used in order to help Dapper parse Dictionary class.
    /// </summary>
    class ListTypeHandler : SqlMapper.TypeHandler<List<KeyValuePair<string, object>>>
    {
        public override void SetValue(IDbDataParameter parameter, List<KeyValuePair<string, object>> value)
        {
            parameter.Value = (value == null)
                ? (object)DBNull.Value
                : JsonConvert.SerializeObject(value);
            parameter.DbType = DbType.String;
        }

        public override List<KeyValuePair<string, object>> Parse(object value)
        {
            return JsonConvert.DeserializeObject<List<KeyValuePair<string, object>>>(value.ToString());
        }
    }
}
