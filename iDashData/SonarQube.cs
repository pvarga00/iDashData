using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Data.SqlClient;
using System.Data;

namespace iDashData
{
    public static class SonarQube
    {
        public static List<SonarQubeApp> GetSonarQubeApps()
        {
            List<SonarQubeApp> SonarQubeApps = new List<SonarQubeApp>();

            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", "91b946edaab687290698c9525257b19d0eae969c", "")))); // Username, Pwd 

                string UrlString = "http://sonar/api/projects/index";
                var Task = client.GetAsync(UrlString);

                Task.Wait();
                var data = Task.Result;
                var contentTask = data.Content.ReadAsStringAsync();
                contentTask.Wait();
                var jsonData = contentTask.Result;

                dynamic jsonObject = JArray.Parse(jsonData);

                try
                {
                    foreach(dynamic obj in jsonObject)
                    {
                        SonarQubeApp App = new SonarQubeApp();

                        App.SonarKey = obj.k;
                        App.Name = obj.nm;

                        SonarQubeApps.Add(App);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return SonarQubeApps;
        }



        public static void saveSonarQubeMetrics(List<SonarQubeApp> list, string dbConn)
        {
            string[] metList = new string[] { "complexity", "coverage", "violations", "sqale_index" };
            using (var connection = new SqlConnection(dbConn))
            {
                connection.Open();
                string query = "INSERT INTO dbo.SonarQubeData (AppID, AppName, MetricName, MetricValue, DateInserted, DateLastUpdated)" +
                               " VALUES (@AppID, @AppName, @MetricName, @MetricValue, @DateInserted, @DateLastUpdated)";
                foreach (SonarQubeApp sonarData in list)
                {
                    foreach(string metric in metList)
                    {
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.Parameters.Add("@AppID", SqlDbType.VarChar, 50).Value = sonarData.SonarKey;
                            cmd.Parameters.Add("@AppName", SqlDbType.VarChar, 50).Value = sonarData.Name;
                            cmd.Parameters.Add("@MetricName", SqlDbType.VarChar, 50).Value = metric;
                            cmd.Parameters.Add("@MetricValue", SqlDbType.VarChar, 50).Value = GetMetric(sonarData.SonarKey, metric);
                            cmd.Parameters.Add("@DateInserted", SqlDbType.DateTime, 50).Value = DateTime.Now;
                            cmd.Parameters.Add("@DateLastUpdated", SqlDbType.DateTime, 50).Value = DateTime.Now;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                connection.Close();
            }
        }

        public static string GetMetric(string sonarKey, string metric)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", "91b946edaab687290698c9525257b19d0eae969c", "")))); // Username, Pwd 

                string UrlString = "http://sonar/api/measures/component?componentKey=" + WebUtility.UrlEncode(sonarKey) + "&metricKeys=" + WebUtility.UrlEncode(metric);
                var Task = client.GetAsync(UrlString);

                Task.Wait();
                var data = Task.Result;
                var contentTask = data.Content.ReadAsStringAsync();
                contentTask.Wait();
                var jsonData = contentTask.Result;

                dynamic jsonObject = JObject.Parse(jsonData);

                try
                {
                    return jsonObject.component.measures[0].value;
                }
                catch
                {
                    return "N/A";
                }
            }
            catch (Exception e)
            {
                return "Error finding coverage metrics for " + sonarKey + e.Message;
            }
        }
    }
}
