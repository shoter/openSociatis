using Entities.enums;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTests.UoW;
using Xunit;

namespace WebTests
{
    public class HospitalTests : SeleniumTestBase
    {
        

        public HospitalTests()
        {

        }


        [Fact]
        public void TestHealingAmount()
        {
            naviger.GiveMeMoney();
            var citizen = unit.GetTestCitizen();
            var hospital = citizen.Region.Hospital;
            var healingPrice = unit.HospitalRepository.GetHealingPrice(hospital.CompanyID, citizen.ID);
            var money = citizen.Entity.Wallet.WalletMoneys.First(w => w.CurrencyID == healingPrice.CurrencyID);
            decimal moneyAmount = money.Amount;
            naviger.SwitchInto(hospital.CompanyID);
            naviger.GiveProduct(ProductTypeEnum.MedicalSupplies, hospital.Company.Quality, 1);
            naviger.SwitchBack();
            naviger.SetMyHealth(30);
            naviger.HealMeAtHome();

            unit.CitizenRepository.ReloadEntity(citizen);
            unit.CitizenRepository.ReloadEntity(money);

            Assert.Equal(30 + hospital.Company.Quality * 10, citizen.HitPoints);
            Assert.Equal(moneyAmount - healingPrice.Cost, money.Amount);

        }
    }
}
