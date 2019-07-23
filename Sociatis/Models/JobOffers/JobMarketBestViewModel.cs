using Entities.structs.jobOffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.JobOffers
{
    public class JobMarketBestViewModel
    {
        public int MinimumSalary { get; set; }
        public int MaximumSalary { get; set; }
        public double MinimumSkill { get; set; }
        public double MaximumSkill { get; set; }
        public string CurrencySymbol { get; set; }

        public JobMarketBestViewModel(Entities.Country country, CountryBestJobOffers best)
        {
            CurrencySymbol = country.Currency.Symbol;

            MaximumSalary = (int)Math.Ceiling(best.MaximumSalary ?? 10);
            MaximumSkill = (double)Math.Ceiling((double?)best.MaximumSkill ?? 10.0);

            if (MinimumSalary == MaximumSalary)
                MaximumSalary += 1;
            if (Math.Abs(MaximumSkill - MinimumSkill) < 0.00001)
                MaximumSkill += 1;


            //let it be for now. It's the best option i think?
            MinimumSalary = 0;
            MinimumSkill = 0;

        }
    }
}