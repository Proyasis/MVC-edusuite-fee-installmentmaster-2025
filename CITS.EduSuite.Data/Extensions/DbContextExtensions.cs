
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Dynamic;

namespace CITS.EduSuite.Data
{
    public static class DbContextExtensions
    {

        public static IEnumerable<dynamic> CollectionFromSql(this DbContext dbContext,
                                                             string sql,
                                                             Dictionary<string, object> Parameters)
        {
            using (var cmd = dbContext.Database.Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (KeyValuePair<string, object> param in Parameters)
                {
                    var Keys = param.Key.Split('_');
                    var key = Keys[0];

                    DbParameter dbParameter = cmd.CreateParameter();
                    dbParameter.ParameterName = param.Key;
                    dbParameter.Value = param.Value ?? DBNull.Value;
                    dbParameter.IsNullable = param.Value == null;
                    cmd.Parameters.Add(dbParameter);
                }

                var retObject = new List<dynamic>();
                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var dataRow = new ExpandoObject() as IDictionary<string, object>;
                        for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                            dataRow.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);

                        retObject.Add((ExpandoObject)dataRow);
                    }
                }

                return retObject;
            }
        }

        public static IEnumerable<List<dynamic>> CollectionFromSqlSets(this DbContext dbContext,
                                                        string sql,
                                                        Dictionary<string, object> Parameters)
        {
            using (var cmd = dbContext.Database.Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (KeyValuePair<string, object> param in Parameters)
                {
                    var Keys = param.Key.Split('_');
                    var key = Keys[0];

                    DbParameter dbParameter = cmd.CreateParameter();
                    dbParameter.ParameterName = param.Key;
                    dbParameter.Value = param.Value ?? DBNull.Value;
                    dbParameter.IsNullable = param.Value == null;
                    cmd.Parameters.Add(dbParameter);
                }
                var retList = new List<List<dynamic>>();
                
                using (var dataReader = cmd.ExecuteReader())
                {

                    do
                    {
                        var retObject = new List<dynamic>();
                        while (dataReader.Read())
                        {
                            var dataRow = new ExpandoObject() as IDictionary<string, object>;
                            for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                                dataRow.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);

                            retObject.Add((ExpandoObject)dataRow);
                        }
                        retList.Add(retObject);
                    }
                    
                    while (dataReader.NextResult());

                }

                return retList;
            }
        }
    }
}