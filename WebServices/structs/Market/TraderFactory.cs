using Entities;
using Entities.enums;
using Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Market
{
    public class TraderFactory
    {
        private readonly IEquipmentService equipmentService;

        public TraderFactory(IEquipmentService equipmentService)
        {
            this.equipmentService = equipmentService;
        }

        public ITrader GetTrader(Entity entity)
        {
            switch (entity.GetEntityType())
            {
                case EntityTypeEnum.Citizen:
                    return new CitizenTrader(entity, equipmentService);
                case EntityTypeEnum.Company:
                    return new CompanyTrader(entity, equipmentService);
                case EntityTypeEnum.Hotel:
                    return new HotelTrader(entity, equipmentService);
                case EntityTypeEnum.Newspaper:
                    return new NewspaperTrader(entity, equipmentService);
            }

            throw new NotImplementedException(); 
        }
    }
}
