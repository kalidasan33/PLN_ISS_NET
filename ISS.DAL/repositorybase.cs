
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Configuration;
using System.Data.Common;
using Oracle.DataAccess.Client;
using Microsoft.Practices.Unity;
using LoggingUtil;
using ISS.Common;

namespace ISS.DAL
{
    public class RepositoryBase :IDisposable
    {
        [Dependency]
        protected ILogger Logger { get; set; }

        protected Database database { get; set; }

        protected bool IsInTrans { get; set; }

        protected DbTransaction trans { get; set; }

        protected DbConnection conn { get; set; }


        public string CurrentDataBaseKey { get; set; }

        public RepositoryBase()
        {
            Logger = new Logger();
        }
        public RepositoryBase(DbTransaction trans)
        {
            Logger = new Logger();
            if (trans != null)
            {
                this.trans = trans;
                IsInTrans = true;
            }
        }

        public bool BeginTransaction()
        {
            if (database == null) database = GetDataBase();
             conn = database.CreateConnection();
            if (conn != null)
            {
                conn.Open();
                trans = conn.BeginTransaction();
                IsInTrans = true;
                return true;
            }
            return false;
        }

        public void CommitTransaction()
        {
            if (IsInTrans)
            {
                try
                {
                    trans.Commit();
                }
                catch (Exception ee)
                {
                    Log("CommitTransaction failed.");
                    Log(ee);
                    throw ee;
                }
                finally
                {
                    try
                    {
                     if(conn.State!=ConnectionState.Closed)   conn.Close();
                    }
                    catch (Exception ee)
                    {
                        Log("CommitTransaction- Close conection failed.");
                        Log(ee);
                    }
                    
                }
                IsInTrans = false;
            }
        }

        public void RollbackTransaction()
        {
            if (IsInTrans)
            {
                try
                {
                    trans.Rollback();
                }
                catch (Exception ee)
                {
                    Log("CommitTransaction failed.");
                    Log(ee);
                }
                finally
                {
                    try
                    {
                        if (conn.State != ConnectionState.Closed) conn.Close();
                    }
                    catch (Exception ee)
                    {
                        Log("CommitTransaction- Close conection failed.");
                        Log(ee);
                    }

                }
                IsInTrans = false;
            }
        }

        private Database GetDataBase()
        {
            return EnterpriseLibraryContainer.Current.GetInstance<Database>();
        }

        protected IDataReader ExecuteReader(string query)
        {
            Log(query);
            var database = GetDataBase();
            IDataReader reader = database.ExecuteReader(CommandType.Text, query);
            return reader;
        }

        //protected IDataReader ExecuteReader1(string query, object[] inputParams)
        //{
        //    var database = GetDataBase();
        //    IDataReader reader = null;
        //    using (var sqlCmd = database.GetSqlStringCommand(query))
        //    {
        //        foreach (DbParameter parm in inputParams)
        //        {
        //            database.AddInParameter(sqlCmd, parm.ParameterName, parm.DbType, parm.Value);
        //        }
        //        reader = database.ExecuteReader(sqlCmd);
        //    }
        //    return reader;
        //}

        protected object ExecuteScalar(string query)
        {
            Log(query);
            var database = GetDataBase();
            if (IsInTrans)
            {
                return database.ExecuteScalar(trans, CommandType.Text, query);
            }
            else
            {
                return database.ExecuteScalar(CommandType.Text, query);
            }
        }

        //protected object ExecuteScalar1(string query, object[] inputParams)
        //{
        //    var database = GetDataBase();
        //    object output = null;

        //    using (var sqlCmd = database.GetSqlStringCommand(query))
        //    {
        //        foreach (DbParameter parm in inputParams)
        //        {
        //            database.AddInParameter(sqlCmd, parm.ParameterName, parm.DbType, parm.Value);
        //        }
        //        output = database.ExecuteScalar(sqlCmd);
        //    }
        //    return output;
        //}

        //protected int ExecuteNonQuery1(string query)
        //{
        //    Log(query);
        //    var database = GetDataBase();
        //    if (IsInTrans)
        //    {
        //        return database.ExecuteNonQuery(trans, CommandType.Text, query);

        //    }
        //    else
        //    {
        //        return database.ExecuteNonQuery(CommandType.Text, query);
        //    }
        //}

        //protected int ExecuteNonQuery1(string query, object[] inputParams)
        //{
        //    var database = GetDataBase();
        //    int output;

        //    using (var sqlCmd = database.GetSqlStringCommand(query))
        //    {
        //        foreach (DbParameter parm in inputParams)
        //        {
        //            database.AddInParameter(sqlCmd, parm.ParameterName, parm.DbType, parm.Value);
        //        }
        //        output = database.ExecuteNonQuery(sqlCmd);
        //    }
        //    return output;
        //}

        //protected DbParameter[] ConvertToDBParameters1(Dictionary<string, object> inputParams)
        //{
        //    List<OracleParameter> oracleParams = new List<OracleParameter>();
        //    foreach (string key in inputParams.Keys)
        //    {
        //        OracleParameter oracleParam = new OracleParameter(key, inputParams[key]);
        //        oracleParams.Add(oracleParam);
        //    }
        //    return oracleParams.ToArray();
        //}

       

       

        protected Database ExecuteProcedure(string storedProcedureName, params OracleParameter[] inputParams)
        {
            Database db = null;
            if (IsInTrans)
            {
                db = database;
            }
            else
            {
                db = GetDataBase();
            }
 
            DbCommand cmd = db.GetStoredProcCommand(storedProcedureName);
            db.DiscoverParameters(cmd);
            foreach (var param in inputParams)
            {
                if (param.Direction != ParameterDirection.Output)
                    db.SetParameterValue(cmd, param.ParameterName, param.Value);
            }
            if (IsInTrans)
            {
                db.ExecuteNonQuery(cmd, trans);
            }
            else
            {
                db.ExecuteNonQuery(cmd);
            }
            return db;
           
        }

        protected object ExecuteFunction(string storedProcedureName, String OutParam, params Object[] inputParams)
        {
            var database = GetDataBase();
            using (DbConnection connection = database.CreateConnection())
            {
                connection.Open();
                DbTransaction transaction = connection.BeginTransaction();

                try
                {
                    DbCommand cmd = database.GetStoredProcCommand(storedProcedureName);
                    database.DiscoverParameters(cmd);

                    database.ExecuteNonQuery(cmd, transaction);

                    transaction.Commit();

                    if (!String.IsNullOrWhiteSpace(OutParam))
                        return database.GetParameterValue(cmd, OutParam);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }

                return database;
            }
        }
        /// <summary>
        /// To read oracle SP using ExecuteReader
        /// </summary>
        /// <param name="query"></param>
        /// <param name="inputParams"></param>
        /// <returns></returns>
        protected IDataReader ExecuteSPReader(string query, object[] inputParams)
        {
            var database = GetDataBase();
            IDataReader reader = null;
            object[] sparams = new object[inputParams.Count() + 1];
            using (var sqlCmd = database.GetStoredProcCommand(query, sparams))
            {
                foreach (DbParameter parm in inputParams)
                {
                    database.SetParameterValue(sqlCmd, parm.ParameterName, parm.Value);
                }
                reader = database.ExecuteReader(sqlCmd);
            }
            return reader;
        }


        protected bool IsDBNull(object inputObject)
        {
            return System.Convert.IsDBNull(inputObject);
        }

        /// <summary>
        /// Validate the string contains ' quotes or special chars
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public String Val(String field)
        {
            if (!String.IsNullOrWhiteSpace(field))
                return field.Replace("'", "''");
            else
                return field;
        }

        protected string FormatInClause(string unformated)
        {
            string formated = "";

            if (!string.IsNullOrWhiteSpace(unformated))
            {
                formated = "'" + unformated.Replace(",", "','") + "'";
            }
            else
            {
                formated = "''";
            }
            return formated;
        }

        protected string FormatInClauseNumeric(string unformated)
        {
            string formated = "";

            if (!string.IsNullOrWhiteSpace(unformated))
            {
                formated = "" + CheckValue(unformated).Replace(",", ",") + "";
            }
            return formated;
        }

        protected string FormatIn(String source, String value)
        {
            if (source == "")
                return "'" + value + "'";
            else
                return source + ",'" + value + "'";
        }

        public String CheckValue(String str)
        {
            if (str.EndsWith(",")) return str.Substring(0, str.Length - 1);
            return str;
        }

        public String SetOperator(String field,bool IsNumeric=false)
        {

            if (!String.IsNullOrEmpty(field))
            {
                if (field.Contains(LOVConstants.GlobalWildCard))
                {
                    return " like '" + Val(field).Replace("*", "%") + "'";
                }
                else if (field.Contains(LOVConstants.GlobalSeperator))
                {
                    if (IsNumeric)
                        return " in (" + FormatInClauseNumeric(field) + ")";
                    else
                        return " in (" + FormatInClause (field)+ ")";
                }
            }

            
                return " = '" + Val(field) + "'";
            

        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void Log(string message)
        {
            Logger.Info(message);
        }

        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="severity">Severity of the Business Exception</param>
        protected void Log(Exception exception)
        {
            Logger.Error(exception);
        }

        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="severity">Severity of the Business Exception</param>
        protected void Log(String Msg,Exception exception)
        {
            Logger.Error(Msg,exception);
        }

       


        public void Dispose()
        {
            try
            {
                if (IsInTrans    && conn != null && !(conn.State == ConnectionState.Closed))
                {
                    conn.Close();
                    conn = null;
                }
            }
            catch { }
        }
    }
}


