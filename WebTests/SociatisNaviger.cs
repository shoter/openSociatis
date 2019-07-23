using Entities.enums;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTests.Navigers;
using WebTests.UoW;
using WebUtils.SeleniumAdditions;

namespace WebTests
{
    public class SociatisNaviger
    {
        
        IWebDriver driver;
        private readonly string baseURL;

        public CompanyNaviger Company { get; set; }

        public SociatisNaviger(IWebDriver driver, string username, string password, string baseURL)
            :this(driver, baseURL)
        {
            this.Company = new CompanyNaviger(driver, baseURL, 191);
            navigateTo("Account/Login");


            var usernameField = driver.FindElement(By.Id("Login_Name"));
            var passwordField = driver.FindElement(By.Id("Login_Password"));
            var button = driver.FindElement(By.Id("loginButton"));

            usernameField.SendKeys(username);
            passwordField.SendKeys(password);

            using (driver.WaitForLoad())
                button.Submit();
        }

        protected SociatisNaviger(IWebDriver driver, string baseURL)
        {
            this.driver = driver;
            this.baseURL = baseURL;
        }

        protected void navigateTo(string url)
        {
            using (driver.WaitForLoad())
                driver.Url = $"{baseURL}/{url}";
        }

        public void GotoHome()
        {
            navigateTo("");
        }

        public void GotoTraining()
        {
            navigateTo("Training");
        }

        public void GiveMeHealth()
        {
            GotoHome();
            driver.ClickAndWaitForLoad("GiveMeHealth");
        }
        public void GiveMeTraining()
        {
            GotoHome();
            driver.ClickAndWaitForLoad("IWantTrainAgain");
        }
        public void GiveMeWorkAbility()
        {
            GotoHome();
            driver.ClickAndWaitForLoad("IWantWorkAgain");
        }

        public void GiveMeMoney()
        {
            GotoHome();
            driver.ClickAndWaitForLoad("GiveMeMoney");
        }

        public void SwitchInto(int entityID)
        {
            navigateTo($"Game/ForceSwitch?entityID={entityID}");
        }

        public void GiveProduct(ProductTypeEnum productType, int quality, int amount)
        {
            navigateTo($"Game/GiveProduct?productTypeID={(int)productType}&quality={quality}&amount={amount}");
        }

        public void SwitchBack()
        {
            navigateTo("Entity/SwitchBack");
        }

        public void SetMyHealth(int hitpoints)
        {
            navigateTo($"Game/SetMyHealth?hp={hitpoints}");
        }

        public void HealMeAtHome()
        {
            var unit = UnitOfWork.Instance;
            var citizen = unit.GetTestCitizen();
            citizen.UsedHospital = false;
            unit.SaveChanges();

            GotoHome();
            driver.ClickAndWaitForLoad("hospitalHealSummary");

        }

        public void GotoWork()
        {
            GotoHome();
           /* var link = driver.FindElement(By.Id("gotoWorkButton"));
            var href = link.GetAttribute("href");
            navigateTo(href);*/
            driver.ClickAndWaitForLoad("gotoWorkButton");
        }

        public void GotoTestBattle()
        {
            navigateTo("Battle/1");
        }
    }
}
