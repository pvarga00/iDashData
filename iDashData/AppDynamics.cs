using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;

namespace iDashData
{
    public static class AppDynamics
    {
        public static void appEnter(List<AppDynamicsApp> list, string dbConn)
        {
        
            using (var connection = new SqlConnection(dbConn))
            {
                connection.Open();
                string query = "INSERT INTO dbo.AppDynamicsData (AppID, AppName, MetricName, MetricValue, DateInserted, DateLastUpdated)" +
                               " VALUES (@AppID, @AppName, @MetricName, @MetricValue, @DateInserted, @DateLastUpdated)";
                foreach (AppDynamicsApp appData in list)
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.Add("@AppID", SqlDbType.VarChar, 50).Value = appData.Name;
                        cmd.Parameters.Add("@AppName", SqlDbType.VarChar, 50).Value = appData.Name;
                        cmd.Parameters.Add("@MetricName", SqlDbType.VarChar, 50).Value = "Average Response Time (ms)";
                        cmd.Parameters.Add("@MetricValue", SqlDbType.VarChar, 50).Value = GetMetric(appData.Key, "Average Response Time (ms)");
                        cmd.Parameters.Add("@DateInserted", SqlDbType.DateTime, 50).Value = DateTime.Now;
                        cmd.Parameters.Add("@DateLastUpdated", SqlDbType.DateTime, 50).Value = DateTime.Now;
                        cmd.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }

        public static string GetMetric(string appName, string metric)
        {
            // These are the application calls we need to make:
            // 
            // MyQL PROD (Avg Response Time): https://quicken.saas.appdynamics.com/controller/rest/applications/MyQL.com%20PROD/metric-data?metric-path=Overall%20Application%20Performance%7CAverage%20Response%20Time%20%28ms%29&time-range-type=BEFORE_NOW&duration-in-mins=15&output=json
            // Overall Application Performance|Average Response Time (ms)

            // Servicing PROD (Avg Response Time): https://quicken.saas.appdynamics.com/controller/rest/applications/Servicing%20PROD/metric-data?metric-path=Overall%20Application%20Performance%7Craven%7CAverage%20Response%20Time%20%28ms%29&time-range-type=BEFORE_NOW&duration-in-mins=15&output=json
            // Overall Application Performance|raven|Average Response Time (ms)

            // Servicing Running Rocket (Business Transaction) : https://quicken.saas.appdynamics.com/controller/rest/applications/Servicing%20PROD/metric-data?metric-path=Business%20Transaction%20Performance%7CBusiness%20Transactions%7Craven%7CRunning%20Rocket%20-%20Raven%7CAverage%20Response%20Time%20%28ms%29&time-range-type=BEFORE_NOW&duration-in-mins=15&output=json



            // AppD Auth Docs: 
            // https://docs.appdynamics.com/display/PRO42/Using+the+Controller+APIs - In order to access AppD data, you'll need to use the Controller's API
            // https://docs.appdynamics.com/display/PRO42/Managing+API+Keys


            try
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Accept.Clear();

                client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(
                            "Basic",
                            Convert.ToBase64String(
                                System.Text.ASCIIEncoding.ASCII.GetBytes(
                                      string.Format("{0}@{1}:{2}", "idash_appd", "quicken", "3UsW5fekecut")))); // <your_username>@<your_accountname>:<your_password> Username=idash_appd @Account=quicken : Pwd=3UsW5fekecut


                appName = appName.Replace(" ", "%20");
                metric = metric.Replace(" ", "%20");
                appName = appName.Replace("(", "%28");

                appName = appName.Replace(")", "%29");

                metric = metric.Replace("(", "%28");
                metric = metric.Replace(")", "%29");





                var url = "https://quicken.saas.appdynamics.com/controller/rest/applications/" + appName + "/metric-data?metric-path=Overall%20Application%20Performance%7C" + metric + "&time-range-type=BEFORE_NOW&duration-in-mins=15&output=json";
                var Task = client.GetAsync(url);
                Task.Wait();
                var data = Task.Result;
                var contentTask = data.Content.ReadAsStringAsync();
                contentTask.Wait();
                var jsonData = contentTask.Result;

                dynamic jsonArray = JArray.Parse(jsonData);

                //var JsonResult = JsonConvert.DeserializeObject<String>(jsonData);

                return jsonArray[0].metricValues[0].value;
            }
            catch (Exception e)
            {

                return "0";
            }
        }
    }
}
