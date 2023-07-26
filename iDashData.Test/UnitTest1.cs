using System;
using Xunit;
using System.Collections.Generic;


namespace iDashData.Test
{ 
    public class TestCoverageTests
    {
        [Fact]
        public void FunctionalCoverageControllerReturnsValue()
        {
            try
            {
                TestCoverageApp coverageApp = new TestCoverageApp()
                {
                    Name = "MyQL",
                    relativeURL = "myql",
                    AppId = "myql",
                    bugs = "",
                    app_functional_coverage = ""

                };



                TestCoverageApp returnApp = TestCoverage.GetTestCoverage(coverageApp);
            }
            catch (Exception e)
            {

                Assert.False(true);
            }


        }

    }
}

