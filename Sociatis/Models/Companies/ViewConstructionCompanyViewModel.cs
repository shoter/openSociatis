using Common.Maths;
using Entities;
using Entities.Extensions;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using Entities.enums;
using WebServices.structs;

namespace Sociatis.Models.Companies
{
    public class ViewConstructionCompanyViewModel : ViewCompanyViewModel
    {
        public int Progress { get; set; }
        public int MaxProgress { get; set; }
        public bool IsOneTimeConstruction { get; set; }
        public double ProgressPercentage => Percentage.CalcPercent(Progress, MaxProgress, decimalPlaces: 2);

        public ViewConstructionCompanyViewModel(Company company, IProductRepository productRepository, ICompanyService companyService, CompanyRights companyRights, IRegionService regionService, IRegionRepository regionRepository) : base(company, productRepository, companyService, companyRights, regionService, regionRepository)
        {
            var constructionService = DependencyResolver.Current.GetService<IConstructionService>();
            var construction = company.Construction;

            Progress = construction.Progress;
            MaxProgress = constructionService.GetProgressNeededToBuild(construction.GetProductType(), company.Quality);

            Stock.ShowQuantity = construction.GetProductType().CanShowStockQuantityInCompanies();

            IsOneTimeConstruction = construction.GetProductType().IsOneTimeConstruction();
        }
    }
}