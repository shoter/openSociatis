using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Items;
using Entities.Repository;
using Entities.Extensions;
using Entities.enums;
using WebServices.PathFinding;
using Weber.Html;
using Common.Operations;

namespace WebServices
{
    public class TravelService : BaseService, ITravelService
    {
        private readonly IRegionService regionService;
        private readonly IEquipmentService equipmentService;
        private readonly IEquipmentRepository equipmentRepository;
        private readonly IProductRepository productRepository;
        private readonly ICitizenRepository citizenRepository;
        private readonly IWarRepository warRepository;
        private readonly ITradeRepository tradeRepository;
        private readonly ITradeService tradeService;

        public TravelService(IRegionService regionService, IEquipmentService equipmentService, IEquipmentRepository equipmentRepository, IProductRepository productRepository,
            ICitizenRepository citizenRepository, IWarRepository warRepository, ITradeRepository tradeRepository, ITradeService tradeService)
        {
            this.regionService = Attach(regionService);
            this.equipmentService = Attach(equipmentService);
            this.equipmentRepository = equipmentRepository;
            this.citizenRepository = citizenRepository;
            this.productRepository = productRepository;
            this.warRepository = warRepository;
            this.tradeRepository = tradeRepository;
            this.tradeService = tradeService;
        }
        public MethodResult CanTravel(Citizen citizen, Region startRegion, Region endRegion, MovingTicket ticket)
        {
            if (citizen.HitPoints < ticket.HpLoss)
                return new MethodResult("You do not have enough HP to travel!");

            if (startRegion.ID == endRegion.ID)
                return new MethodResult("You are actually in this region!");

            var item = citizen.Entity.GetEquipmentItem(ticket, productRepository);

            if (item.Amount == 0)
                return new MethodResult("You do not have this kind of moving ticket!");

            var path = regionService.GetPathBetweenRegions(startRegion, endRegion, new DefaultRegionSelector(), new WarsPenaltyCostCalculator(citizen, warRepository, regionService) );

            if (path == null || path.Distance > ticket.TravelDistance)
                return new MethodResult("You cannot travel that distance with this type of moving ticket!");

           

            return MethodResult.Success;
        }

        public void Travel(int citizenID, Region startRegion, Region endRegion, MovingTicket ticket)
        {
            using (NoSaveChanges)
            {
                var citizen = citizenRepository.GetById(citizenID);

                citizen.RegionID = endRegion.ID;

                equipmentRepository.RemoveEquipmentItem(citizen.Entity.EquipmentID.Value, ticket);

                var trades = tradeRepository.GetTradesAssociatedWithEntity(citizenID)
                    .Where(trade => trade.TradeStatusID == (int)TradeStatusEnum.Ongoing)
                    .ToList();

                var citizenLink = EntityLinkCreator.Create(citizen.Entity);
                var reason = $"{citizenLink} moving into other region";
                foreach (var trade in trades)
                {
                    tradeService.AbortTrade(trade, reason);
                }

                citizen.HitPoints -= ticket.HpLoss;
            }

            ConditionalSaveChanges(citizenRepository);
        }
    }
}
