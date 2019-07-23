using Entities.Models.Businesses;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Businesses
{
    public class BusinessIndexViewModel
    {
        public class BusinessViewModel
        {
            public int ID { get; set; }
            public SmallEntityAvatarViewModel Avatar { get; set; }
            public string ShortName { get; set; }
            public string Name { get; set; }
            public string Messages { get; set; }
            public string Warnings { get; set; }
            public bool CanSwitch { get; set; }

            public BusinessViewModel(Business business)
            {
                ID = business.BusinessID;
                ShortName = Name = business.BusinessName;
                CanSwitch = business.CanSwitchInto;
                Avatar = new SmallEntityAvatarViewModel(ID, Name, business.BusinessImgUrl)
                    .DisableNameInclude();

                if (ShortName.Length > 15)
                {
                    ShortName = $"{ShortName.Substring(0, 12)}...";
                    
                }

                if (business.CanSwitchInto && business.Warnings.HasValue)
                {
                    Warnings = $"{business.Warnings}";
                    if (business.UnreadWarnings.HasValue)
                        Warnings += $" ({business.UnreadWarnings} unread)";
                }

                if (business.CanSwitchInto && business.Messages.HasValue)
                {
                    Messages = $"{business.Messages}";
                    if (business.UnreadMessages.HasValue)
                        Messages += $" ({business.UnreadMessages} unread)";
                }
            }
        }

        public List<BusinessViewModel> Businesses { get; set; }
        public BusinessIndexViewModel(IEnumerable<Business> businesses)
        {
            Businesses = businesses
                .Select(b => new BusinessViewModel(b))
                .ToList();
        }
    }
}
