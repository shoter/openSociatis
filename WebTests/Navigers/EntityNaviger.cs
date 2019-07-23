using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTests.Navigers
{
    public class EntityNaviger : SociatisNaviger
    {
        protected readonly int entityID;
        public EntityNaviger(IWebDriver driver, string baseURL, int entityID) : base(driver, baseURL)
        {
            this.entityID = entityID;
        }

        public void GotoGift()
        {
            navigateTo($"Gift/{entityID}");
        }
    }
}
