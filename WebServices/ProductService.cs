using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using WebServices.structs;
using Entities.enums;
using Common;
using Entities.Repository;

namespace WebServices
{
    public class ProductService : IProductService
    {
        IProductTaxRepository productTaxRepository;
        ICountryRepository countryRepository;

        public ProductService(IProductTaxRepository productTaxRepository, ICountryRepository countryRepository)
        {
            this.productTaxRepository = productTaxRepository;
            this.countryRepository = countryRepository;
        }

        public ProductAllTaxes GetAllTaxesForProduct(int productID, int? countryID, int? foreignCountryID)
        {
            ProductTax vatTax = null;
            if (countryID.HasValue)
                vatTax = GetVatTax(productID, countryID);

            ProductTax importTax = null,
                exportTax = null;

            if (foreignCountryID.HasValue && foreignCountryID != countryID)
            {
                importTax = productTaxRepository.FirstOrDefault(t => t.ProductID == productID
                && t.CountryID == foreignCountryID
                && t.ForeignCountryID == countryID
                && t.ProductTaxTypeID == (int)ProductTaxTypeEnum.Import);

                exportTax = productTaxRepository.FirstOrDefault(t => t.ProductID == productID
               && t.CountryID == countryID
               && t.ForeignCountryID == foreignCountryID
               && t.ProductTaxTypeID == (int)ProductTaxTypeEnum.Export);
            }

            List<ProductTax> taxes = new List<ProductTax>() { vatTax, importTax, exportTax };

            var ret = new ProductAllTaxes(taxes);

            if (countryID.HasValue == false)
            {
                ret.VAT = 0;
            }
            if (foreignCountryID.HasValue == false)
            {
                ret.ImportTax = 0;
                ret.ExportTax = 0;
            }

            return ret;
        }

        public ProductTax GetVatTax(int productID, int? countryID)
        {
            return productTaxRepository.FirstOrDefault(t => t.ProductID == productID && t.CountryID == countryID && t.ProductTaxTypeID == (int)ProductTaxTypeEnum.VAT);
        }

        public List<ProductTypeEnum> GetRequiredProductTypes(ProductTypeEnum productType)
        {
            return GetRequiredRaws(productType, 1)
                .Select(requirement => requirement.RequiredProductType)
                .ToList();
        }

        public List<ProductRequirement> GetRequiredRaws(ProductTypeEnum productType, int quality)
        {
            List<ProductRequirement> _ret = new List<ProductRequirement>();
            switch (productType)
            {
                case ProductTypeEnum.Bread:
                    {
                        _ret.Add(
                            new ProductRequirement()
                            {
                                Quantity = quality * 2,
                                RequiredProductType = ProductTypeEnum.Grain
                            });
                        break;
                    }
                case ProductTypeEnum.Fuel:
                    {
                        _ret.Add(
                            new ProductRequirement()
                            {
                                Quantity = 1,
                                RequiredProductType = ProductTypeEnum.Oil
                            });
                        break;
                    }
                case ProductTypeEnum.UpgradePoints:
                    {
                        _ret.Add(
                            new ProductRequirement()
                            {
                                Quantity = quality * 2,
                                RequiredProductType = ProductTypeEnum.Wood
                            });
                        break;
                    }
                case ProductTypeEnum.MovingTicket:
                    {
                        _ret.Add(
                            new ProductRequirement()
                            {
                                Quantity = quality * 4,
                                RequiredProductType = ProductTypeEnum.Oil
                            });
                        break;
                    }
                case ProductTypeEnum.Tea:
                    {
                        _ret.Add(
                            new ProductRequirement()
                            {
                                Quantity = Utils.Fibbonaci(quality + 2),
                                RequiredProductType = ProductTypeEnum.TeaLeaf
                            });
                        break;
                    }
                case ProductTypeEnum.Weapon:
                    {
                        _ret.Add(
                            new ProductRequirement()
                            {
                                Quantity = quality + Utils.Fibbonaci(quality + 1),
                                RequiredProductType = ProductTypeEnum.Iron
                            });
                        break;
                    }
                case ProductTypeEnum.MedicalSupplies:
                    {
                        _ret.Add(
                            new ProductRequirement()
                            {
                                Quantity = Utils.Fibbonaci(quality + 2),
                                RequiredProductType = ProductTypeEnum.TeaLeaf
                            });
                        break;
                    }
                case ProductTypeEnum.Paper:
                    {
                        _ret.Add(
                            new ProductRequirement()
                            {
                                Quantity = 1,
                                RequiredProductType = ProductTypeEnum.Wood
                            });
                        break;
                    }
                case ProductTypeEnum.Development:
                    {
                        _ret.Add(
                           new ProductRequirement()
                           {
                               Quantity = 1,
                               RequiredProductType = ProductTypeEnum.Oil
                           });
                        _ret.Add(
                          new ProductRequirement()
                          {
                              Quantity = 1,
                              RequiredProductType = ProductTypeEnum.Iron
                          });
                        break;
                    }
                case ProductTypeEnum.ConstructionMaterials:
                    {
                        _ret.Add(
                          new ProductRequirement()
                          {
                              Quantity = 2,
                              RequiredProductType = ProductTypeEnum.Wood
                          });
                        _ret.Add(
                            new ProductRequirement()
                            {
                                Quality = 1,
                                RequiredProductType = ProductTypeEnum.Iron
                            });
                        break;
                    }
                case ProductTypeEnum.House:
                    {
                        _ret.Add(
                          new ProductRequirement()
                          {
                              Quantity = 1 * quality,
                              RequiredProductType = ProductTypeEnum.ConstructionMaterials
                          });
                        break;
                    }
                case ProductTypeEnum.Hotel:
                    {
                        _ret.Add(
                          new ProductRequirement()
                          {
                              Quantity = 2 * quality,
                              RequiredProductType = ProductTypeEnum.ConstructionMaterials
                          });
                        break;
                    }
                case ProductTypeEnum.Hospital:
                    {
                        _ret.Add(
                          new ProductRequirement()
                          {
                              Quantity = Utils.Fibbonaci(quality + 2),
                              RequiredProductType = ProductTypeEnum.ConstructionMaterials
                          });
                        break;
                    }
                case ProductTypeEnum.DefenseSystem:
                    {
                        _ret.Add(
                          new ProductRequirement()
                          {
                              Quantity = Utils.Fibbonaci(quality + 3),
                              RequiredProductType = ProductTypeEnum.ConstructionMaterials
                          });
                        break;
                    }
                case ProductTypeEnum.Grain:
                case ProductTypeEnum.Oil:
                case ProductTypeEnum.Wood:
                case ProductTypeEnum.TeaLeaf:
                case ProductTypeEnum.Iron:
                case ProductTypeEnum.SellingPower:
                    {
                        break; //nothing
                    }
            }
            return _ret;
        }

        public int GetTeaHealedAmount(int quality) => quality * 10;

        public decimal GetFuelCostForTranposrt(Path path, ProductTypeEnum productType, int quality, int amount)
        {
            return GetFuelCostForTranposrt(path.Distance, productType, quality, amount);
        }

        public decimal GetFuelCostForTranposrt(double distance, ProductTypeEnum productType, int quality, int amount)
        {
            double fuelMultiplier = getFuelMultiplier(productType, quality);

            var dist = ((100.0 + distance) / 100.0);

            return (decimal)((dist * getDistanceMultiplier(productType, quality)
                + Math.Pow(amount, 0.82) * getAmountMultiplier(productType, quality)
                + Math.Pow(dist, 0.333) * Math.Pow(amount, 0.333) * getDistanceAmountMultiplier(productType, quality)) * 0.75);
        }

        private double getAmountMultiplier(ProductTypeEnum productType, int quality)
        {
            double mult;

            switch (productType)
            {
                case ProductTypeEnum.Grain:
                case ProductTypeEnum.Iron:
                case ProductTypeEnum.Oil:
                case ProductTypeEnum.TeaLeaf:
                case ProductTypeEnum.Wood:
                    mult = 0.01; break;
                case ProductTypeEnum.Fuel:
                    mult = 0.05; break;
                case ProductTypeEnum.UpgradePoints:
                    mult = 0.075; break;
                case ProductTypeEnum.Weapon:
                    mult = 0.4; break;
                default:
                    mult = 0.2; break;
            }

            return mult * (1.0 + (quality - 1) * 0.3);
        }


        private double getDistanceMultiplier(ProductTypeEnum productType, int quality)
        {
            double mult = 1;
            switch (productType)
            {
                case ProductTypeEnum.Grain:
                case ProductTypeEnum.Iron:
                case ProductTypeEnum.Oil:
                case ProductTypeEnum.TeaLeaf:
                case ProductTypeEnum.Wood:
                    mult = 0.80; break;
                case ProductTypeEnum.Fuel:
                    mult = 0.55; break;
                case ProductTypeEnum.Weapon:
                    mult = 3; break;
                default:
                    mult = 2.15; break;
            }
            return mult * (1.0 + quality * 0.25);
        }

        private double getDistanceAmountMultiplier(ProductTypeEnum productType, int quality)
        {
            double mult;

            switch (productType)
            {
                case ProductTypeEnum.Bread:
                case ProductTypeEnum.Iron:
                case ProductTypeEnum.Oil:
                case ProductTypeEnum.TeaLeaf:
                case ProductTypeEnum.Wood:
                    mult = 0.0005; break;
                case ProductTypeEnum.Fuel:
                    mult = 0.001; break;
                default:
                    mult = 0.01; break;
            }

            return mult + (1.0 + quality * 0.01);
        }


        private double getFuelMultiplier(ProductTypeEnum productType, int quality)
        {
            switch (productType)
            {
                case ProductTypeEnum.Grain:
                case ProductTypeEnum.Iron:
                case ProductTypeEnum.Oil:
                case ProductTypeEnum.TeaLeaf:
                case ProductTypeEnum.Wood:
                    return 0.15;
                case ProductTypeEnum.Fuel:
                    return 0.15;
                case ProductTypeEnum.Weapon:
                    return Math.Sqrt(Math.Min(1, quality / 1.5));
            }
            return 1.0;
        }

        public bool CanUseQuality(ProductTypeEnum productType)
        {
            switch (productType)
            {
                case ProductTypeEnum.Bread:
                case ProductTypeEnum.MedicalSupplies:
                case ProductTypeEnum.MovingTicket:
                case ProductTypeEnum.Tea:
                case ProductTypeEnum.Weapon:
                    return true;
            }

            return false;
        }

        public bool CanUseFertility(ProductTypeEnum productType)
        {
            switch (productType)
            {
                case ProductTypeEnum.Grain:
                case ProductTypeEnum.Wood:
                case ProductTypeEnum.TeaLeaf:
                case ProductTypeEnum.Iron:
                case ProductTypeEnum.Oil:
                    return true;
            }

            return false;
        }
    }
}
