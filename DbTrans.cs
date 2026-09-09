using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntegrefKrediOnay
{
    public class DbTrans
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public DbTrans(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public int InsertValue(string query)
        {
            using (var cmd = new SqlCommand(query, _connection, _transaction))
            {
                cmd.CommandTimeout = 0;
                return cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetData(string query)
        {
            using (var cmd = new SqlCommand(query, _connection, _transaction))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public string GetValue(string query)
        {
            using (var cmd = new SqlCommand(query, _connection, _transaction))
            {
                var result = cmd.ExecuteScalar();
                return result?.ToString();
            }
        }
        public long GetRegister(string query)
        {
            try
            {
                using (var cmd = new SqlCommand(query, _connection, _transaction))
                {
                    var result = cmd.ExecuteScalar();
                    if (result == null)
                        throw new Exception("0");

                    return Convert.ToInt64(result);
                }
            }
            catch (Exception ex)
            {
                return Convert.ToInt64(ex);
            }
        }
        public string BulkInsertRetorn(DataTable dt, string tableName, bool keepNulls = true)
        {
            var options = keepNulls
           ? SqlBulkCopyOptions.KeepNulls
           : SqlBulkCopyOptions.Default;
            try
            {
                using (var bulk = new SqlBulkCopy(_connection, options, _transaction))
                {
                    bulk.DestinationTableName = tableName;
                    foreach (DataColumn c in dt.Columns)
                        bulk.ColumnMappings.Add(c.ColumnName, c.ColumnName);
                    bulk.WriteToServer(dt);
                }
                return "Aktarım Tamamlandı";
            }
            catch (SqlException ex)
            {
                // Adjust this index based on your mapping / table schema (colid 9 -> index 8)
                int problematicColumnIndex = 8;

                string columnName = dt.Columns[problematicColumnIndex].ColumnName;
                Console.WriteLine($"Checking column: {columnName}");

                foreach (DataRow row in dt.Rows)
                {
                    string value = row[problematicColumnIndex]?.ToString() ?? "";
                    // Replace 50 with your target SQL column max length
                    if (value.Length > 50)
                    {
                        Console.WriteLine($"Found bad row! Length: {value.Length}, Value: {value}");
                    }
                }
                StringBuilder sb = new StringBuilder();

                foreach (SqlError err in ex.Errors)
                {
                    sb.AppendLine(
                        $"Number:{err.Number}  " +
                        $"State:{err.State}  " +
                        $"Line:{err.LineNumber}  " +
                        $"Message:{err.Message}");
                }

                return "SQL Hatası : " + sb.ToString();
                //return "SQL Hatası : " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Genel Hata : " + ex.Message;
            }

        }
        public DataTable Query(string spName, Dictionary<string, string> param)
        {
            DataTable returnType = new DataTable();
            using (SqlCommand cmd = new SqlCommand(spName, _connection, _transaction))
            {
                cmd.CommandTimeout = 0;
                if (param != null)
                {
                    foreach (var item in param)
                    {
                        cmd.Parameters.AddWithValue(item.Key, item.Value);
                    }
                }
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adap = new SqlDataAdapter(cmd);
                adap.Fill(returnType);
            }
            return returnType;
        }
        //public string BulkInsertRetorn(DataTable dt, string tableName)
        //{
        //    using (var bulk = new SqlBulkCopy(_connection, SqlBulkCopyOptions.Default, _transaction))
        //    {
        //        bulk.DestinationTableName = tableName;
        //        bulk.WriteToServer(dt);
        //    }
        //    return "Aktarım Tamamlandı";
        //}
    }
}
