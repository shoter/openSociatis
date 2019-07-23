using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTests.UoW;

namespace WebTests
{
    public class SeleniumTestBase : IDisposable
    {
        protected readonly UnitOfWork unit = new UnitOfWork();
        protected IWebDriver driver;
        protected SociatisNaviger naviger;

        protected virtual bool isHeadless => true;

        public SeleniumTestBase()
        {
            var options = new FirefoxOptions();
            if(isHeadless)
            options.AddArgument("headless");
            driver = new FirefoxDriver(options);
            naviger = new SociatisNaviger(driver, "test2", "abc", "http://soctest.sociatis.net");
        }
        public void Dispose()
        {
            driver.Close();
        }
    }
}
