using Common.EntityFramework;
using Entities.enums;
using Entities.Models.Businesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class BusinessRepository : RepositoryBaseEntityless<SociatisEntities>, IBusinessRepository
    {
        public BusinessRepository(SociatisEntities context) : base(context)
        {
        }

        public IEnumerable<Business> GetBusinesses(int citizenID)
        {
            var data = new
            {
                Hotels = (from hotel in context.Hotels.Where(h => h.OwnerID == citizenID || h.HotelManagers.Any(m => m.CitizenID == citizenID))
                          join hotelEntity in context.Entities on hotel.ID equals hotelEntity.EntityID
                          join manager in context.HotelManagers.Where(m => m.CitizenID == citizenID && m.CanSwitchInto) on hotel.ID equals manager.HotelID into managers
                          join warning in context.Warnings on hotel.ID equals warning.EntityID into warnings
                          join message in context.MailboxMessages on hotel.ID equals message.Viewers_EntityID into messages
                          select new Business()
                          {
                              BusinessID = hotel.ID,
                              BusinessImgUrl = hotelEntity.ImgUrl,
                              BusinessName = hotelEntity.Name,
                              BusinessType = BusinessTypeEnum.Hotel,
                              Warnings = warnings.Count(),
                              UnreadWarnings = warnings.Where(w => w.Unread).Count(),
                              Messages = messages.Count(),
                              UnreadMessages = messages.Where(m => m.Unread).Count(),
                              CanSwitchInto = hotel.OwnerID == citizenID || managers.Any()
                          }).ToList(),
                Companies = (from company in context.Companies.Where(c => c.OwnerID == citizenID || c.CompanyManagers.Any(m => m.EntityID == citizenID))
                             join companyEntity in context.Entities on company.ID equals companyEntity.EntityID
                             join manager in context.CompanyManagers.Where(m => m.EntityID == citizenID && m.Switch) on company.ID equals manager.CompanyID into managers
                             join warning in context.Warnings on company.ID equals warning.EntityID into warnings
                             join message in context.MailboxMessages on company.ID equals message.Viewers_EntityID into messages
                             select new Business()
                             {
                                 BusinessID = company.ID,
                                 BusinessImgUrl = companyEntity.ImgUrl,
                                 BusinessName = companyEntity.Name,
                                 BusinessType = BusinessTypeEnum.Hotel,
                                 Warnings = warnings.Count(),
                                 UnreadWarnings = warnings.Where(w => w.Unread).Count(),
                                 Messages = messages.Count(),
                                 UnreadMessages = messages.Where(m => m.Unread).Count(),
                                 CanSwitchInto = company.OwnerID == citizenID || managers.Any()
                             }).ToList()
            };

            data.Hotels.AddRange(data.Companies);


            return data.Hotels.OrderBy(d => d.BusinessID);
        }
    }
}
