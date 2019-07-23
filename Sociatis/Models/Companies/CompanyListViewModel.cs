using Entities;
using Entities.enums;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs.Companies;

namespace Sociatis.Models.Companies
{
    public class CompanyListViewModel
    {


        public List<CompanyViewModel> Companies { get; set; } = new List<CompanyViewModel>();

        public CompanyListViewModel(List<ManageableCompanyInfo> cs)
        {
            cs.ForEach(c => Companies.Add(new CompanyViewModel(c)));
        }

        public class CompanyViewModel
        {
            public CompanyViewModel(ManageableCompanyInfo company)
            {
                this.ID = company.ID;
                this.Name = company.Name;
                this.Avatar = new ImageViewModel(company.ImgUrl);
                this.Product = Images.GetProductImage((ProductTypeEnum)company.ProductID).VM;

                this.UnreadWarnings = company.UnreadWarnings;

                this.UnreadMessages = company.UnreadMessages;
            }
            public int ID { get; set; }
            public string Name { get; set; }
            public ImageViewModel Avatar { get; set; }
            public ImageViewModel Product { get; set; }
            public int UnreadMessages { get; set; }
            public int UnreadWarnings { get; set; }
        }
    }
}
