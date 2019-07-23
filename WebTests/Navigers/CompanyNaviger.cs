using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace WebTests.Navigers
{
    public class CompanyNaviger : EntityNaviger
    {
        private int companyID => base.entityID;
        public CompanyNaviger(IWebDriver driver, string baseURL, int companyID) : base(driver, baseURL, companyID)
        {
        }

        public void GotoMarketOffer()
        {
            navigateTo($"Company/{companyID}/MarketOffers");
        }

        public void GotoJobOffers()
        {
            navigateTo($"Company/{companyID}/JobOffers");
        }

        public void GotoManagement()
        {
            navigateTo($"Company/{companyID}/Management");
        }


    }
}
