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
    class DictionaryTypeHandler : SqlMapper.TypeHandler<Dictionary<string, object>>
    {
        public override void SetValue(IDbDataParameter parameter, Dictionary<string, object> value)
        {
            parameter.Value = (value == null)
                ? (object)DBNull.Value
                : JsonConvert.SerializeObject(value);
            parameter.DbType = DbType.String;
        }

        public override Dictionary<string, object> Parse(object value)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(value.ToString());
        }
    }
}