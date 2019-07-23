using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities;

namespace WebServices.Classes.Constructions
{
    public class HotelFinishWorker : IConstructionFinishWorker
    {
        private string companyName;
        private Region region;
        private Entity owner;
        public void AcumulateDataBeforeCompanyDelete(Construction construction)
        {
            companyName = construction.Company.Entity.Name;
            region = construction.Company.Region;
            owner = construction.Company.Owner;
        }

        public object ExecuteAction()
        {
            var hotelService = DependencyResolver.Current.GetService<IHotelService>();
            return hotelService.BuildHotel(companyName, region, owner);
        }
    }
}
