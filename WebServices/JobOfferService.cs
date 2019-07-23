using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.BigParams.jobOffers;
using Entities;
using Entities.enums;
using WebServices.Companies;

namespace WebServices
{
    public class JobOfferService : IJobOfferService
    {
        IJobOfferRepository jobOfferRepository;
        ITransactionsService transactionService;
        ICountryRepository countryRepository;
        ICompanyRepository companyRepository;
        ICompanyFinanceSummaryService companyFinanceSummaryService;

        public JobOfferService(IJobOfferRepository jobOfferRepository, ITransactionsService transactionService, ICountryRepository countryRepository,
            ICompanyRepository companyRepository, ICompanyFinanceSummaryService companyFinanceSummaryService)
        {
            this.jobOfferRepository = jobOfferRepository;
            this.transactionService = transactionService;
            this.countryRepository = countryRepository;
            this.companyRepository = companyRepository;
            this.companyFinanceSummaryService = companyFinanceSummaryService;
        }

        /// <summary>
        /// This method do not save changes in DB
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected JobOffer CreateJobOffer(CreateJobOfferParams parameters)
        {
            var jobOffer = new JobOffer()
            {
                Amount = parameters.Amount,
                CompanyID = parameters.CompanyID,
                TypeID = (int)parameters.OfferType,
                MinSkill = (decimal)parameters.MinimumSkill,
                CountryID = parameters.CountryID
            };

            return jobOffer;
        }

        public ContractJobOffer CreateJobOffer(CreateContractJobOfferParams parameters)
        {
            var jobOffer = CreateJobOffer(parameters as CreateJobOfferParams);

            var contract = new ContractJobOffer()
            {
                Length = parameters.Length,
                MinHP = parameters.MinimumHP,
                MinSalary = (decimal)parameters.MinimumSalary,
                SigneeID = parameters.SigneeID
            };

            jobOffer.ContractJobOffer = contract;

            if (parameters.Cost > 0 && parameters.CountryID.HasValue)
            {
                var country = countryRepository.GetById(parameters.CountryID.Value);
                var company = companyRepository.GetById(parameters.CompanyID);

                PayJobOfferFee(company, country, JobOfferTypeEnum.Contract, parameters.Amount, parameters.CurrencyID, parameters.Cost);
            }

            jobOfferRepository.Add(jobOffer);
            jobOfferRepository.SaveChanges();

            return jobOffer.ContractJobOffer;
        }

        private void PayJobOfferFee(Company company, Country country, JobOfferTypeEnum offerType, double offerAmount, int currencyID, decimal price)
        {
            transactionService.PayForJobOffer(company, country, currencyID, price);

            companyFinanceSummaryService.AddFinances(company, new JobOfferCostFinance(price, currencyID));
        }

        public NormalJobOffer CreateJobOffer(CreateNormalJobOfferParams parameters)
        {
            var jobOffer = CreateJobOffer(parameters as CreateJobOfferParams);
            var normal = new NormalJobOffer()
            {
                Salary = (decimal)parameters.Salary
            };

            jobOffer.NormalJobOffer = normal;


            if(parameters.Cost > 0 && parameters.CountryID.HasValue)
            {
                var country = countryRepository.GetById(parameters.CountryID.Value);
                var company = companyRepository.GetById(parameters.CompanyID);

                PayJobOfferFee(company, country, JobOfferTypeEnum.Normal, parameters.Amount, parameters.CurrencyID, parameters.Cost);
            }


            jobOfferRepository.Add(jobOffer);
            jobOfferRepository.SaveChanges();

            return jobOffer.NormalJobOffer;
        }

        public void TakeJobOffer(int jobOfferID)
        {
            var jobOffer = jobOfferRepository.GetById(jobOfferID);
            jobOffer.Amount--;

            if(jobOffer.Amount <= 0)
            {
                jobOfferRepository.Remove(jobOfferID);
            }

            jobOfferRepository.SaveChanges();
            
        }

        public void TakeOutJobOffer(int jobOfferID)
        {
            jobOfferRepository.Remove(jobOfferID);
            jobOfferRepository.SaveChanges();
        }

        public virtual bool IsCompliantWithMinimalWage(decimal salary, int countryID)
        {
            var minimalWage = countryRepository.GetCountryPolicySetting(countryID, x => x.MinimalWage);

            return salary >= minimalWage;
        }

    }
}
