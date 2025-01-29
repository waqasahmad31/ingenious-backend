using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection;

namespace DAS.DataAccess.Helpers
{
    public class DbHelper
    {
        public static async Task<int> ExecuteWithOutput(string procedureName, MySqlParameter[] parameters, string outputParameterName, ConnectionStrings connectionStrings)
        {
            try
            {
                string connStr = await connectionStrings.Get();
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(procedureName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(parameters);

                        var outputParam = new MySqlParameter(outputParameterName, MySqlDbType.Int32)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputParam);

                        await con.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        return (int)cmd.Parameters[outputParameterName].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure '{procedureName}'. Details: {ex.Message}", ex);
            }
        }

        public static async Task<int> ExecuteQuery(string procedureName, MySqlParameter[] parameters, ConnectionStrings connectionStrings)
        {
            try
            {
                string connStr = await connectionStrings.Get();
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(procedureName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(parameters);

                        await con.OpenAsync();
                        return await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure '{procedureName}'. Details: {ex.Message}", ex);
            }
        }

        public static async Task<int> ExecuteQuery(string procedureName, MySqlParameter[] parameters, ConnectionStrings connectionStrings, MySqlTransaction transaction)
        {
            try
            {
                string connStr = await connectionStrings.Get();
                using (var con = new MySqlConnection(connStr))
                {
                    using (var cmd = new MySqlCommand(procedureName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(parameters);
                        cmd.Transaction = transaction;

                        await con.OpenAsync();
                        return await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing stored procedure '{procedureName}' within a transaction. Details: {ex.Message}", ex);
            }
        }

        public static async Task<int> ExecuteNonQuery(string procedureName, MySqlParameter[] parameters, ConnectionStrings connectionStrings)
        {
            try
            {
                string connStr = await connectionStrings.Get();
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(procedureName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(parameters);

                        await con.OpenAsync();
                        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing non-query stored procedure '{procedureName}'. Details: {ex.Message}", ex);
            }
        }

        public static async Task<T> Get<T>(string procedureName, MySqlParameter[] parameters, ConnectionStrings connectionStrings) where T : new()
        {
            try
            {
                string connStr = await connectionStrings.Get();
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(procedureName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(parameters);

                        await con.OpenAsync();
                        using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            DataTable data = new DataTable();
                            data.Load(reader);
                            List<T> resultList = ConvertDataTableToList<T>(data);
                            return resultList.FirstOrDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving data for type '{typeof(T).Name}' using stored procedure '{procedureName}'. Details: {ex.Message}", ex);
            }
        }

        public static async Task<List<T>> GetList<T>(string procedureName, MySqlParameter[] parameters, ConnectionStrings connectionStrings) where T : new()
        {
            try
            {
                string connStr = await connectionStrings.Get();
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(procedureName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        await con.OpenAsync();
                        using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            DataTable data = new DataTable();
                            data.Load(reader);
                            return ConvertDataTableToList<T>(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving list of type '{typeof(T).Name}' using stored procedure '{procedureName}'. Details: {ex.Message}", ex);
            }
        }

        public static async Task<DataSet> GetDataSet(string procedureName, MySqlParameter[] parameters, ConnectionStrings connectionStrings)
        {
            try
            {
                string connStr = await connectionStrings.Get();
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(procedureName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        await con.OpenAsync();
                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            DataSet dataSet = new DataSet();
                            await Task.Run(() => da.Fill(dataSet));
                            return dataSet;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching DataSet using stored procedure '{procedureName}'. Details: {ex.Message}", ex);
            }
        }

        public static List<T> ConvertDataTableToList<T>(DataTable dataTable) where T : new()
        {
            try
            {
                List<T> dataList = new List<T>();

                foreach (DataRow row in dataTable.Rows)
                {
                    T item = CreateItemFromRow<T>(row);
                    dataList.Add(item);
                }

                return dataList;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error converting DataTable to list of type '{typeof(T).Name}'. Details: {ex.Message}", ex);
            }
        }

        public static T CreateItemFromRow<T>(DataRow row) where T : new()
        {
            try
            {
                T item = new T();

                foreach (DataColumn column in row.Table.Columns)
                {
                    // Find the property on T that matches column name (case-insensitive)
                    PropertyInfo? property = typeof(T).GetProperty(
                        column.ColumnName,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                    );

                    if (property != null && row[column] != DBNull.Value)
                    {
                        Type propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                        // 1) If it's an enum, parse the string
                        if (propertyType.IsEnum)
                        {
                            // e.g. "Pending" => OrderStatusEnum.Pending
                            var stringValue = row[column].ToString();
                            if (!string.IsNullOrEmpty(stringValue))
                            {
                                var enumValue = Enum.Parse(propertyType, stringValue);
                                property.SetValue(item, enumValue, null);
                            }
                        }
                        else
                        {
                            // 2) Otherwise do the normal Convert.ChangeType
                            object? safeValue = Convert.ChangeType(row[column], propertyType);
                            property.SetValue(item, safeValue, null);
                        }
                    }
                }

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error creating item of type '{typeof(T).Name}' from DataRow. Details: {ex.Message}",
                    ex
                );
            }
        }

    }
}
