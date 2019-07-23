using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using WebUtils.SeleniumAdditions;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using Entities.enums;

namespace WebTests
{
    public class LoginTests : SeleniumTestBase
    {
        [Fact]
        public void wrongPasswordTest()
        {
            Assert.DoesNotContain("soctest.sociatis.net/Account/Login", driver.Url);
        }

        [Fact]
        public void trainingTest()
        {
            naviger.GiveMeHealth();
            naviger.GiveMeTraining();
            naviger.GotoTraining();

            driver.ClickAndWaitForLoad("trainingButton");

            var result = driver.FindElement(By.Id("trainingResult"));

            var regexp = new Regex(@"You gained \+[0-9]+\.[0-9]+ strength from training");

            Assert.Matches(regexp, result.Text);
        }


        [Fact]
        public void workTest()
        {
            naviger.SwitchInto(351);
            naviger.GiveProduct(ProductTypeEnum.Iron, 1, 10);
            naviger.SwitchBack();
            naviger.GiveMeHealth();
            naviger.GiveMeWorkAbility();
            naviger.GotoWork();

            driver.ClickAndWaitForLoad("gotoWorkButton");

            var result = driver.FindElement(By.Id("workResult"));

            var regexp = new Regex(@"You worked today and you had produced [0-9]+\.[0-9]+");
            Assert.Matches(regexp, result.Text);

        }

        [Fact]
        public void fight5times()
        {
            naviger.GiveMeHealth();
            naviger.GotoTestBattle();

            driver.TryClick("preAttFightButton");

            var weaponSelect = new SelectElement(driver.FindElement(By.Id("weaponQuality")));
            weaponSelect.SelectByValue("0");
            for (int i = 0; i < 5; ++i)
            {
                driver.TryClick("realFightButton");
                driver.WaitForAndAcceptAlert();
            }

            naviger.GotoHome();
            var hp = driver.FindElement(By.Id("citizenHPText"));

            var regexp = new Regex(@"50%\s?HP");

            Assert.Matches(regexp, hp.Text);
        }
    }
}
