using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Finances
{
    public class CompanyFinance
    {
        public decimal SellRevenue { get; set; }
        public decimal GiftBalance { get; set; }
        public decimal TradeBalance { get; set; }
        public decimal SalaryCost { get; set; }
        public decimal ImportTax { get; set; }
        public decimal ExportTax { get; set; }
        public decimal JobOfferCost { get; set; }
        public decimal MarketOfferCost { get; set; }

        public decimal Total =>
            SellRevenue + GiftBalance + TradeBalance
            - SalaryCost - ImportTax - ExportTax - JobOfferCost - MarketOfferCost;

        public CompanyFinance(CompanyFinanceSummary summary)
        {
            SellRevenue = summary.SellRevenue;
            GiftBalance = summary.GiftBalance;
            TradeBalance = summary.TradeBalance;
            SalaryCost = summary.SalaryCost;
            ImportTax = summary.ImportTax;
            ExportTax = summary.ExportTax;
            JobOfferCost = summary.JobOfferCost;
            MarketOfferCost = summary.MarketOfferCost;
        }
    }
}
