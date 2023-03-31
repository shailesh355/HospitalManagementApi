using MySql.Data.MySqlClient;
using System.Reflection;

namespace BaseClass
{

    /// <summary>
    /// Summary description for db_connection
    /// </summary>
    public class DBConnection
    {
        public MySqlDataAdapter? Adapter;
        public MySqlDataReader? reader;
        public MySqlCommand? command;
        public MySqlParameter? objMySqlParameter;
        private string connectionString;
        public DBConnection()
        {
            connectionString = GetConnectionString(DBConnectionList.TransactionDb);
        }

        #region Select Query without parameter
        /// <summary>
        /// Select query with default connection string without parameter
        /// </summary>
        /// <param name="_query"></param>
        /// <returns></returns>
        public ReturnClass.ReturnDataTable ExecuteSelectQuery(string _query)
        {
            ReturnClass.ReturnDataTable dt = new();
            try
            {
                using MySqlConnection connection = new(connectionString);
                using MySqlCommand cmd = new();
                connection.Open();
                cmd.Connection = connection;
                cmd.CommandText = _query;
                using (Adapter = new MySqlDataAdapter())
                {
                    Adapter.SelectCommand = cmd;
                    Adapter.Fill(dt.table);
                    dt.status = true;
                }
            }
            catch (MySqlException ex)
            {
                WriteLog.Error("ExecuteSelectQuery - Query: " + _query + "\n   error - ", ex);
                dt.status = false;
                dt.message = ex.Message;
            }
            return dt;
        }

        /// <summary>
        /// Async Select query with default connection string without parameter
        /// </summary>
        /// <param name="_query"></param>
        /// <returns></returns>
        public async Task<ReturnClass.ReturnDataTable> ExecuteSelectQueryAsync(string _query)
        {
            ReturnClass.ReturnDataTable dt = new();
            try
            {
                using MySqlConnection connection = new(connectionString);
                using MySqlCommand cmd = new();
                connection.Open();
                cmd.Connection = connection;
                cmd.CommandText = _query;
                using (Adapter = new MySqlDataAdapter())
                {
                    Adapter.SelectCommand = cmd;
                    await Adapter.FillAsync(dt.table);
                    dt.status = true;
                }
            }
            catch (MySqlException ex)
            {
                WriteLog.Error("ExecuteSelectQueryAsync - Query: " + _query + "\n   error - ", ex);
                dt.status = false;
                dt.message = ex.Message;
            }
            return dt;
        }

        /// <summary>
        /// Select query with custom connection string without parameter
        /// </summary>
        /// <param name="_query"></param>
        /// <param name="dbconname"></param>
        /// <returns></returns>
        public ReturnClass.ReturnDataTable ExecuteSelectQuery(string _query, DBConnectionList dbconname)
        {
            ReturnClass.ReturnDataTable dt = new();
            try
            {
                string connectionString = GetConnectionString(dbconname);
                using MySqlConnection connection = new(connectionString);
                using MySqlCommand cmd = new();
                connection.Open();
                cmd.Connection = connection;
                cmd.CommandText = _query;
                using (Adapter = new MySqlDataAdapter())
                {
                    Adapter.SelectCommand = cmd;
                    Adapter.Fill(dt.table);
                    dt.status = true;
                }
            }
            catch (MySqlException ex)
            {
                WriteLog.Error("ExecuteSelectQuery - Query: " + _query + "\n   error - ", ex);
                dt.status = false;
                dt.message = ex.Message;
            }
            return dt;
        }

        /// <summary>
        /// Async Select query with custom connection string without parameter
        /// </summary>
        /// <param name="_query"></param>
        /// <param name="dbconname"></param>
        /// <returns></returns>
        public async Task<ReturnClass.ReturnDataTable> ExecuteSelectQueryAsync(string _query, DBConnectionList dbconname)
        {
            ReturnClass.ReturnDataTable dt = new();
            try
            {
                string connectionString = GetConnectionString(dbconname);
                using MySqlConnection connection = new(connectionString);
                using MySqlCommand cmd = new();
                connection.Open();
                cmd.Connection = connection;
                cmd.CommandText = _query;
                using (Adapter = new MySqlDataAdapter())
                {
                    Adapter.SelectCommand = cmd;
                    await Adapter.FillAsync(dt.table);
                    dt.status = true;
                }
            }
            catch (MySqlException ex)
            {
                WriteLog.Error("ExecuteSelectQueryAsync - Query: " + _query + "\n   error - ", ex);
                dt.status = false;
                dt.message = ex.Message;
            }
            return dt;
        }
        #endregion

        #region Select Query with parameter 

        /// <summary>
        /// Execute Select Query With Parameters
        /// </summary>
        /// <param name="_query"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public ReturnClass.ReturnDataTable ExecuteSelectQuery(string _query, MySqlParameter[] sqlParameter)
        {
            ReturnClass.ReturnDataTable dt = new();
            try
            {
                using MySqlConnection connection = new(connectionString);
                using MySqlCommand cmd = new();
                connection.Open();
                cmd.Connection = connection;
                cmd.CommandText = _query;
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(sqlParameter);

                using (Adapter = new MySqlDataAdapter())
                {
                    Adapter.SelectCommand = cmd;
                    Adapter.Fill(dt.table);
                    dt.status = true;
                }
            }
            catch (MySqlException ec)
            {
                WriteLog.Error("ExecuteSelectQuery - Query: " + _query + "\n   error - ", ec);
                dt.status = false;
                dt.message = ec.Message;
            }
            return dt;
        }
        /// <summary>
        /// Async select query with parameter
        /// </summary>
        /// <param name="_query"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public async Task<ReturnClass.ReturnDataTable> ExecuteSelectQueryAsync(string _query, MySqlParameter[] sqlParameter)
        {
            ReturnClass.ReturnDataTable dt = new();
            try
            {
                using MySqlConnection connection = new(connectionString);
                using MySqlCommand cmd = new();
                connection.Open();
                cmd.Connection = connection;
                cmd.CommandText = _query;
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(sqlParameter);

                using (Adapter = new MySqlDataAdapter())
                {
                    Adapter.SelectCommand = cmd;
                    await Adapter.FillAsync(dt.table);
                    dt.status = true;
                }
            }
            catch (MySqlException ec)
            {
                WriteLog.Error("ExecuteSelectQueryAsync - Query: " + _query + "\n   error - ", ec);
                dt.status = false;
                dt.message = ec.Message;
            }
            return dt;
        }

        /// <summary>
        /// select query with parameter and custom connection string
        /// </summary>
        /// <param name="_query"></param>
        /// <param name="sqlParameter"></param>
        /// <param name="dbconname"></param>
        /// <returns></returns>
        public ReturnClass.ReturnDataTable ExecuteSelectQuery(string _query, MySqlParameter[] sqlParameter, DBConnectionList dbconname)
        {
            ReturnClass.ReturnDataTable dt = new();
            try
            {
                using MySqlConnection connection = new(GetConnectionString(dbconname));
                using MySqlCommand cmd = new();
                connection.Open();
                cmd.Connection = connection;
                cmd.CommandText = _query;
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(sqlParameter);

                using (Adapter = new MySqlDataAdapter())
                {
                    Adapter.SelectCommand = cmd;
                    Adapter.Fill(dt.table);
                    dt.status = true;
                }
            }
            catch (MySqlException ec)
            {
                WriteLog.Error("ExecuteSelectQuery - Query: " + _query + "\n   error - ", ec);
                dt.status = false;
                dt.message = ec.Message;
            }
            return dt;
        }

        /// <summary>
        /// Async select Query with parameter and custom connection string
        /// </summary>
        /// <param name="_query"></param>
        /// <param name="sqlParameter"></param>
        /// <param name="dbconname"></param>
        /// <returns></returns>
        public async Task<ReturnClass.ReturnDataTable> ExecuteSelectQueryAsync(string _query, MySqlParameter[] sqlParameter, DBConnectionList dbconname)
        {
            ReturnClass.ReturnDataTable dt = new();
            try
            {
                using MySqlConnection connection = new(GetConnectionString(dbconname));
                using MySqlCommand cmd = new();
                connection.Open();
                cmd.Connection = connection;
                cmd.CommandText = _query;
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(sqlParameter);

                using (Adapter = new MySqlDataAdapter())
                {
                    Adapter.SelectCommand = cmd;
                    await Adapter.FillAsync(dt.table);
                    dt.status = true;
                }
            }
            catch (MySqlException ec)
            {
                WriteLog.Error("ExecuteSelectQueryAsync - Query: " + _query + "\n   error - ", ec);
                dt.status = false;
                dt.message = ec.Message;
            }
            return dt;
        }

        #endregion

        #region Common Query Execution Functions
        /// <summary>
        /// Execute Sync Queries
        /// </summary>
        /// <param name="_query"></param>
        /// <param name="sqlParameter"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public ReturnClass.ReturnBool ExecuteQuery(string _query, MySqlParameter[] sqlParameter, string methodName)
        {
            ReturnClass.ReturnBool rb = new();
            try
            {
                using MySqlConnection connection = new(connectionString);
                using MySqlCommand cmd = new();
                connection.Open();
                cmd.Connection = connection;
                cmd.CommandText = _query;
                cmd.Parameters.AddRange(sqlParameter);
                cmd.ExecuteNonQuery();
                rb.status = true;
            }
            catch (MySqlException ex)
            {
                WriteLog.Error(methodName + " - Query: " + _query + "\n   error - ", ex);
                rb.status = false;
                rb.message = ex.Message;
            }
            return rb;
        }
        /// <summary>
        /// Execute Async Transactional Queries
        /// </summary>
        /// <param name="_query"></param>
        /// <param name="sqlParameter"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public async Task<ReturnClass.ReturnBool> ExecuteQueryAsync(string _query, MySqlParameter[] sqlParameter, string methodName)
        {
            ReturnClass.ReturnBool rb = new();
            try
            {
                using MySqlConnection connection = new(connectionString);
                using MySqlCommand cmd = new();
                connection.Open();
                cmd.Connection = connection;
                cmd.CommandText = _query;
                cmd.Parameters.AddRange(sqlParameter);
                await cmd.ExecuteNonQueryAsync();
                rb.status = true;
            }
            catch (MySqlException ex)
            {
                WriteLog.Error(methodName + " - Query: " + _query + "\n   error - ", ex);
                rb.status = false;
                rb.message = ex.Message;
            }
            return rb;
        }
        /// <summary>
        /// Execute Transactional Queries With Custom DB Connection
        /// </summary>
        /// <param name="_query"></param>
        /// <param name="sqlParameter"></param>
        /// <param name="methodName"></param>
        /// <param name="dbconname"></param>
        /// <returns></returns>
        public ReturnClass.ReturnBool ExecuteQuery(string _query, MySqlParameter[] sqlParameter, string methodName, DBConnectionList dbconname)
        {
            ReturnClass.ReturnBool rb = new();
            try
            {
                string connectionString = GetConnectionString(dbl: dbconname);
                using MySqlConnection connection = new(connectionString);
                using MySqlCommand cmd = new();
                connection.Open();
                cmd.Connection = connection;
                cmd.CommandText = _query;
                cmd.Parameters.AddRange(sqlParameter);
                cmd.ExecuteNonQuery();
                rb.status = true;
            }
            catch (MySqlException exp)
            {
                WriteLog.Error(methodName + " - Query: " + _query + "\n   error - ", exp);
                rb.status = false;
                rb.message = exp.Message;
            }
            return rb;
        }
        /// <summary>
        /// Execute Async Transactional Queries With Custom DB
        /// </summary>
        /// <param name="_query"></param>
        /// <param name="sqlParameter"></param>
        /// <param name="methodName"></param>
        /// <param name="dbconname"></param>
        /// <returns></returns>
        public async Task<ReturnClass.ReturnBool> ExecuteQueryAsync(string _query, MySqlParameter[] sqlParameter, string methodName, DBConnectionList dbconname)
        {
            ReturnClass.ReturnBool rb = new();
            try
            {
                string connectionString = GetConnectionString(dbl: dbconname);
                using MySqlConnection connection = new(connectionString);
                using MySqlCommand cmd = new();
                connection.Open();
                cmd.Connection = connection;
                cmd.CommandText = _query;
                cmd.Parameters.AddRange(sqlParameter);
                await cmd.ExecuteNonQueryAsync();
                rb.status = true;
            }
            catch (MySqlException exp)
            {
                WriteLog.Error(methodName + " - Query: " + _query + "\n   error - ", exp);
                rb.status = false;
                rb.message = exp.Message;
            }
            return rb;
        }
        #endregion

        /// <summary>
        /// Returns Connection String
        /// </summary>
        /// <param name="dbl"></param>
        /// <returns></returns>
        private string GetConnectionString(DBConnectionList dbl)
        {
            Utilities util = new();
            string connectionStringlocal = "";
            ReturnClass.ReturnBool rb = util.GetAppSettings("Build", "Version");

            if (rb.status)
            {
                string buildType = rb.message.ToLower();
                if (buildType == "production")
                {
                    connectionStringlocal = dbl switch
                    {
                        DBConnectionList.TransactionDb => util.GetAppSettings("DBConnection", "Production", "TransactionDB").message,
                        DBConnectionList.ReportingDb => util.GetAppSettings("DBConnection", "Production", "ReportingDb").message,
                        _ => connectionString,
                    };
                }
                else if (buildType == "development")
                {
                    connectionStringlocal = dbl switch
                    {
                        DBConnectionList.TransactionDb => util.GetAppSettings("DBConnection", "Development", "TransactionDB").message,
                        DBConnectionList.ReportingDb => util.GetAppSettings("DBConnection", "Development", "ReportingDb").message,
                        _ => connectionString,
                    };
                }
            }
            else
            {
                connectionStringlocal = rb.error;
            }
            return connectionStringlocal;
        }



        /// <summary>
        /// SHAILESH - Async select query with parameter
        /// </summary>
        /// <param name="_query"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public async Task<ReturnClass.ReturnDataSet> executeSelectQueryForDataset_async(String _query, MySqlParameter[] sqlParameter)
        {
            ReturnClass.ReturnDataSet ds = new ReturnClass.ReturnDataSet();
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.CommandText = _query;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(sqlParameter);

                        using (Adapter = new MySqlDataAdapter())
                        {
                            Adapter.SelectCommand = cmd;
                            await Adapter.FillAsync(ds.dataset);
                            ds.status = true;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                WriteLog.Error(" Query: " + _query + "\n   error - ", ex);
                ds.status = false;
                ds.message = ex.Message;
            }
            return ds;
        }
        /// <summary>
        /// SHAILESH - Async select query with parameter
        /// </summary>
        /// <param name="_query"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public async Task<ReturnClass.ReturnDataSet> executeSelectQueryForDataset_async(String _query)
        {
            ReturnClass.ReturnDataSet ds = new ReturnClass.ReturnDataSet();
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        con.Open();
                        cmd.Connection = con;
                        cmd.CommandText = _query;
                        using (Adapter = new MySqlDataAdapter())
                        {
                            Adapter.SelectCommand = cmd;
                            await Adapter.FillAsync(ds.dataset);
                            ds.status = true;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                WriteLog.Error("Query: " + _query + "\n   error - ", ex);
                ds.status = false;
                ds.message = ex.Message;
            }
            return ds;
        }
    }
}