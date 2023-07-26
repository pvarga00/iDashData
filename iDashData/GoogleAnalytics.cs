using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Google.Apis.Services;
using Newtonsoft.Json;
using Google.Apis.Auth;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using System.Data.SqlClient;
using System.Data;

namespace iDashData
{
    public static class GoogleAnalytics
    {
        public static void gAppEnter(List<GoogleAnalyticsApp> list, string dbConn)
        {
            string[] metList = new string[] { "avgPageDownloadTime", "avgPageLoadTime", "avgTimeOnPage", "pageLoadTime", "uniquePageViews" };
            using (var connection = new SqlConnection(dbConn))
            {
                connection.Open();
                string query = "INSERT INTO dbo.GoogleAnalyticsData (AppID, AppName, MetricName, MetricValue, DateInserted, DateLastUpdated)" +
                               " VALUES (@AppID, @AppName, @MetricName, @MetricValue, @DateInserted, @DateLastUpdated)";
                foreach (GoogleAnalyticsApp analyticsData in list)
                {
                    List<string> returnList = GetGAnalyticsMetrics("54149095", analyticsData.Key, metList);
                    for(int i=0; i<returnList.Count; i++)
                    {
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.Parameters.Add("@AppID", SqlDbType.VarChar, 50).Value = analyticsData.AppId;
                            cmd.Parameters.Add("@AppName", SqlDbType.VarChar, 50).Value = analyticsData.Name;
                            cmd.Parameters.Add("@MetricName", SqlDbType.VarChar, 50).Value = metList[i];
                            cmd.Parameters.Add("@MetricValue", SqlDbType.VarChar, 50).Value = returnList[i];
                            cmd.Parameters.Add("@DateInserted", SqlDbType.DateTime, 50).Value = DateTime.Now;
                            cmd.Parameters.Add("@DateLastUpdated", SqlDbType.DateTime, 50).Value = DateTime.Now;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                connection.Close();
            }
        }
        public static GoogleCredential GAAuth()
        {
            GoogleCredential credential;

            try
            {
                // https://developers.google.com/identity/protocols/OAuth2ServiceAccount

                using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream)
                        .CreateScoped(AnalyticsService.Scope.Analytics);
                }
                return credential;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static List<string> GetGAnalyticsMetrics(string appId, string appName, params string[] metrics)
        {
            try
            {
                var credential = GAAuth();


                var service = new AnalyticsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = appId,
                    //ApiKey = "AIzaSyAdLMP-_LVYbOUbEcDU3u1nc88pLW35dbM"
                });


                string metricStr = "";

                for (int i = 0; i < metrics.Length; i++)
                {

                    if (i > 0 && i < metrics.Length)
                    {
                        metricStr += "%2Cga%3A" + metrics[i];
                    }
                    else
                    {
                        metricStr += "ga%3A" + metrics[i];
                    }

                }


                var Task = service.HttpClient.GetAsync("https://www.googleapis.com/analytics/v3/data/ga?ids=ga%3A" + appId + "&start-date=30daysAgo&end-date=yesterday&metrics=" + metricStr);

                Task.Wait();

                var data = Task.Result;

                dynamic contentTask = data.Content.ReadAsStringAsync();
                contentTask.Wait();

                dynamic results = JsonConvert.DeserializeObject(contentTask.Result);

                List<string> returnList = new List<string>();

                foreach (string metric in results.rows[0])
                {
                    returnList.Add(metric);
                }

                return returnList;
            }
            catch (Exception e)
            {

                return new List<string>(new string[] { e.Message });
            }
        }
    }
}
