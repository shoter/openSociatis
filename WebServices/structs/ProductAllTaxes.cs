using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs
{
    public class ProductAllTaxes
    {
        public decimal VAT { get; set; }
        public decimal ExportTax { get; set; }
        public decimal ImportTax { get; set; }

        public ProductAllTaxes(List<ProductTax> taxes)
        {
            Dictionary<ProductTaxTypeEnum, decimal> dicTaxes = new Dictionary<ProductTaxTypeEnum, decimal>();

            foreach(var tax in taxes)
            {
                if (tax == null)
                    continue;
                KeyValuePair<ProductTaxTypeEnum, decimal> dicTax = createDicTax(tax);
                dicTaxes.Add(dicTax.Key, dicTax.Value);
            }

            foreach (ProductTaxTypeEnum taxType in Enum.GetValues(typeof(ProductTaxTypeEnum)))
            {
                KeyValuePair<ProductTaxTypeEnum, decimal> taxKey;

                if (dicTaxes.ContainsKey(taxType))
                    taxKey = new KeyValuePair<ProductTaxTypeEnum, decimal>(taxType, dicTaxes[taxType]);
                else
                    taxKey = getTaxDefaultValue(taxType);

                setTax(taxKey);
            }


        }

        private void setTax(KeyValuePair<ProductTaxTypeEnum, decimal> taxKey)
        {
            switch(taxKey.Key)
            {
                case ProductTaxTypeEnum.Export:
                    ExportTax = taxKey.Value;
                    break;
                case ProductTaxTypeEnum.Import:
                    ImportTax = taxKey.Value;
                    break;
                case ProductTaxTypeEnum.VAT:
                    VAT = taxKey.Value;
                    break;
                default:
                    throw new NotImplementedException();
            }
            
        }

        private KeyValuePair<ProductTaxTypeEnum, decimal> getTaxDefaultValue(ProductTaxTypeEnum taxType)
        {
            switch(taxType)
            {
                case ProductTaxTypeEnum.Export:
                    return new KeyValuePair<ProductTaxTypeEnum, decimal>(taxType, Constants.DefaultExportTax);
                case ProductTaxTypeEnum.Import:
                    return new KeyValuePair<ProductTaxTypeEnum, decimal>(taxType, Constants.DefaultImportTax);
                case ProductTaxTypeEnum.VAT:
                    return new KeyValuePair<ProductTaxTypeEnum, decimal>(taxType, Constants.DefaultVat);
            }
            throw new NotImplementedException();
        }

        private KeyValuePair<ProductTaxTypeEnum, decimal> createDicTax(ProductTax tax)
        {
            return new KeyValuePair<ProductTaxTypeEnum, decimal>((ProductTaxTypeEnum)tax.ProductTaxTypeID, tax.TaxRate);
        }
    }
}
