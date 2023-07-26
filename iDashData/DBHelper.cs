using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace iDashData
{
    public static class DBHelper
    {
        public static void insertMetric(string connString, string dbName, string appID, string appName, string metricName, string metricValue)
        {
            using (var connection = new SqlConnection(connString))
            {
                connection.Open();
                string query = "INSERT INTO " +  dbName +  " (AppID, AppName, MetricName, MetricValue, DateInserted, DateLastUpdated)" +
                               " VALUES (@AppID, @AppName, @MetricName, @MetricValue, @DateInserted, @DateLastUpdated)";


                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.Add("@AppID", SqlDbType.VarChar, 50).Value = appID;
                    cmd.Parameters.Add("@AppName", SqlDbType.VarChar, 50).Value = appName;
                    cmd.Parameters.Add("@MetricName", SqlDbType.VarChar, 50).Value = metricName;
                    cmd.Parameters.Add("@MetricValue", SqlDbType.VarChar, 50).Value = metricValue;
                    cmd.Parameters.Add("@DateInserted", SqlDbType.DateTime, 50).Value = DateTime.Now;
                    cmd.Parameters.Add("@DateLastUpdated", SqlDbType.DateTime, 50).Value = DateTime.Now;
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        //Metric dictionary key is the metricName, value is the metricValue
        public static void insertMultipleMetrics(string connString, string dbName, string appID, string appName, Dictionary<string, string> metricDictionary)
        {
            foreach(string key in metricDictionary.Keys)
            {
                insertMetric(connString, dbName, appID, appName, key, metricDictionary[key]);
            }
        }
    }
}
