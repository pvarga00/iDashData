using System;
using System.IO;
using Newtonsoft.Json;
using FluentScheduler;
using Microsoft.CSharp.RuntimeBinder;
using System.Collections;
using System.Collections.Generic;

namespace iDashData
{
    class Program
    {
        MySettings res;
        static void Main(string[] args)
        {
            MySettings res;
            using (StreamReader r = new StreamReader(new FileStream("appSettings.json", FileMode.Open)))
            {
                string json = r.ReadToEnd();
                res = JsonConvert.DeserializeObject<MySettings>(json);
            }

            res.SonarQubeApps = iDashData.SonarQube.GetSonarQubeApps();

            JobManager.Initialize(new MyRegistry(res));

            Console.WriteLine("iDashData Metric Aggregator\n\n");

           Console.ReadLine();
        }
    }

    public class MyRegistry : Registry
    {
        public MyRegistry(MySettings res)
        {
            Schedule(() =>
            {

                Console.Write(DateTime.Now + " - Saving metrics...");

                SonarQube.saveSonarQubeMetrics(res.SonarQubeApps, res.ConfigSettings.DbContextConnectionString);

                GoogleAnalytics.gAppEnter(res.GoogleAnalyticsApps, res.ConfigSettings.DbContextConnectionString);

                AppDynamics.appEnter(res.AppDynamicsApps, res.ConfigSettings.DbContextConnectionString);

                TestCoverage.saveTestCoverage(res.TestCoverageApps, res.ConfigSettings.DbContextConnectionString);

                Console.WriteLine("Complete!");
            }).ToRunNow().AndEvery(1).Days().At(8, 00);
        }
    }
}