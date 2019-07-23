using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTests.Navigers;
using WebUtils.SeleniumAdditions;
using Xunit;

namespace WebTests
{
    public class CompanyTests : SeleniumTestBase
    {
        private CompanyNaviger companyNaviger => naviger.Company;

        [Fact]
        public void JobOffersDisplayTest()
        {
            companyNaviger.GotoJobOffers();

            //it throws exception if not found. So test passes if there is no exception :)
            var normalJobsTable = driver.FindElement(By.Id("normalJobOffersTable"));
        }

        [Fact]
        public void MarketOffersDisplayTest()
        {
            companyNaviger.GotoMarketOffer();

            var marketOffersTable = driver.FindElement(By.ClassName("marketOffersList"));
        }

        [Fact]
        public void ManagementDisplayTest()
        {
            companyNaviger.GotoManagement();

            var contentBox = driver.FindElement(By.Id("companyManagement"));
        }

        [Fact]
        public void GiveGiftTest()
        {
            naviger.GiveMeMoney();
            companyNaviger.GotoGift();

            var moneyAmount = driver.FindElement(By.CssSelector(".moneyAmount input[data-currencyid]"));
            decimal cashAmount = decimal.Parse(moneyAmount.GetAttribute("value"));
            moneyAmount.Clear();
            moneyAmount.SendKeys("0.01");

            using (driver.WaitForLoad())
            {
                driver.TryClick(By.CssSelector(".moneyAmount .moneySendButton"));
                driver.WaitForAndAcceptAlert();
            }

            moneyAmount = driver.FindElement(By.CssSelector(".moneyAmount input[data-currencyid]"));
            decimal newCashAmount = decimal.Parse(moneyAmount.GetAttribute("value"));

            Assert.Equal(cashAmount - 0.01m, newCashAmount);
        }





    }
}
