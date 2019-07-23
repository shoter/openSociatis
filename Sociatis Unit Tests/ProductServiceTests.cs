using Entities;
using Entities.enums;
using Entities.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sociatis_Test_Suite.Dummies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    class ProductServiceTests
    {
        private Mock<IProductTaxRepository> productTaxRepository = new Mock<IProductTaxRepository>();
        private Mock<ICountryRepository> countryRepository = new Mock<ICountryRepository>();

        private List<Country> countries = new List<Country>();
        private List<ProductTax> productTaxes = new List<ProductTax>();

        private ProductService productService;

        public ProductServiceTests()
        {
            productService = new ProductService(productTaxRepository.Object, countryRepository.Object);

            var countryCreator = new CountryDummyCreator();

            countries.Add(countryCreator.Create());
            countries.Add(countryCreator.Create());
            countries.Add(countryCreator.Create());
            countries.Add(countryCreator.Create());

            countryRepository.Setup(x => x.Where(It.IsAny<Expression<Func<Country, bool>>>()))
                .Returns<Expression<Func<Country, bool>>>(
                p => countries.Where(p.Compile()).AsQueryable());

            productTaxRepository.Setup(x => x.Where(It.IsAny<Expression<Func<ProductTax, bool>>>()))
                .Returns<Expression<Func<ProductTax, bool>>>(
                p => productTaxes.Where(p.Compile()).AsQueryable());
        }

        [TestMethod]
        public void GetAllTaxesForProductNoTaxesTest()
        {
            var country = countries[0];

            var allTax = productService.GetAllTaxesForProduct(123, country.ID, 1234);

            Assert.AreEqual(allTax.VAT, Constants.DefaultVat);
            Assert.AreEqual(allTax.ImportTax, Constants.DefaultImportTax);
            Assert.AreEqual(allTax.ExportTax, Constants.DefaultExportTax);
        }

        [TestMethod]
            public void GetAllTaxesForProductTest()
        {
            var country = countries[0];
            productTaxes.Add(
                new ProductTax()
                {
                    CountryID = country.ID,
                    Country = country,
                    Product = null,
                    ProductID = 1,
                    ProductTaxTypeID = (int)ProductTaxTypeEnum.VAT,
                    TaxRate = (decimal)0.1
                });


            var allTax = productService.GetAllTaxesForProduct(123, country.ID, 12345);

            Assert.AreEqual(allTax.VAT, 0.1);
            Assert.AreEqual(allTax.ImportTax, Constants.DefaultImportTax);
            Assert.AreEqual(allTax.ExportTax, Constants.DefaultExportTax);


        }
    }
}
