using System;
using System.Collections.Generic;
using System.Text;

namespace iDashData
{
    public class MySettings
    {
        public List<SonarQubeApp> SonarQubeApps;
        public List<AppDynamicsApp> AppDynamicsApps;
        public List<GoogleAnalyticsApp> GoogleAnalyticsApps;
        public List<TestCoverageApp> TestCoverageApps;
        public ConfigSetting ConfigSettings;
    }

    public class ConfigSetting
    {
        public string SonarQubeToken { get; set; }
        public string GoogleAnalyticsToken { get; set; }
        public string DbContextConnectionString { get; set; }
    }

    public class AppDynamicsApp
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string AvgResponseTime { get; set; }
    }

    public class SonarQubeApp
    {
        public string Name { get; set; }
        public string SonarKey { get; set; }
        public string Complexity { get; set; }
        public string Violations { get; set; }
        public string Coverage { get; set; }
        public string TechDebt { get; set; }
    }

    public class GoogleAnalyticsApp
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string AppId { get; set; }
        public string pageLoadTime { get; set; }
        public string avgPageLoadTime { get; set; }
        public string avgPageDownloadTime { get; set; }
        public string avgTimeOnPage { get; set; }
        public string uniquePageViews { get; set; }
    }

    public class TestCoverageApp
    {
        public string Name;
        public string relativeURL { get; set; }
        public string bugs { get; set; }
        public string app_functional_coverage { get; set; }
        public string AppId { get; set; }
    }
}
