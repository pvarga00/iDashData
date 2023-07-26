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
using System.Net.Http;




namespace iDashData
{
    public static class TestCoverage
    {
        public static void saveTestCoverage(List<TestCoverageApp> testCoverageApps, string connString)
        {

            foreach (TestCoverageApp app in testCoverageApps)
            {

                TestCoverageApp populatedApp = GetTestCoverage(app);

                Dictionary<string, string> metricDictionary = 
                    new Dictionary<string, string>
                    {
                        { "bugs", populatedApp.bugs},
                        { "app_functional_coverage", populatedApp.app_functional_coverage}
                    };

                    DBHelper.insertMultipleMetrics(connString, "dbo.TestCoverageData", app.AppId, app.Name, metricDictionary);
                    
                }
        }


        public static TestCoverageApp GetTestCoverage(TestCoverageApp app)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();

            string baseURL = "http://10.9.11.51:4000/api/v2/";
            string relativeURL = app.relativeURL;

            var appCoverageValue = "";
            var bugsValue = "";

            try
            {
                var Task = client.GetAsync(baseURL + relativeURL);
                Task.Wait();
                var data = Task.Result;
                var contentTask = data.Content.ReadAsStringAsync();
                contentTask.Wait();
                var jsonData = contentTask.Result;

                dynamic JsonResult = JObject.Parse(jsonData);

                //appCoverageValue = JsonResult["app_functional_coverage"].value;
                //bugsValue = JsonResult["bugs"].value;
                appCoverageValue = JsonResult.app_functional_coverage.Value;
                bugsValue = JsonResult.bugs.Value;
            }
            catch (Exception e)
            {
                appCoverageValue = "N/A";
                bugsValue = "N/A";
            }

            return new TestCoverageApp { app_functional_coverage = appCoverageValue, bugs = bugsValue, AppId = app.AppId, Name = app.Name, relativeURL = app.relativeURL};
        }
    }
}
