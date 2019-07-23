using Common.Operations;
using Common.XUnitExtensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WebTests
{
    public class SessionTests : SeleniumTestBase
    {
        protected override bool isHeadless => true;


        private MethodResult ConcurrentHomeTestsResults;
        [Fact]
        public async void ConcurrentHomeTests()
        {
            ConcurrentHomeTestsResults = new MethodResult();
            DateTime now = DateTime.Now;

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; ++i)
                tasks.Add(Task.Factory.StartNew(homeTest));

            Task.WaitAll(tasks.ToArray());

            Assert.True(ConcurrentHomeTestsResults.isSuccess);
        }



        private MethodResult homeTest()
        {
            var options = new FirefoxOptions();
            if (isHeadless)
                options.AddArgument("headless");
            var driver = new FirefoxDriver(options);

            var naviger = new SociatisNaviger(driver, "test2", "abc", "http://soctest.sociatis.net");

            DateTime now = DateTime.Now;
            try
            {
                while (true)
                {

                    naviger.GotoHome();
                    var bodyText = driver.FindElement(By.TagName("body")).Text;
                    Assert.DoesNotContain(("Store update, insert, or delete statement"), bodyText);

                    if ((DateTime.Now - now).Seconds > 25)
                        break;

                }
            }
            catch (Exception e)
            {
                lock (ConcurrentHomeTestsResults)
                    ConcurrentHomeTestsResults.AddError(e.Message);
                return new MethodResult(e.Message);
            }
            finally
            {
                driver.Close();
            }

            return MethodResult.Success;
        }


    }
}
