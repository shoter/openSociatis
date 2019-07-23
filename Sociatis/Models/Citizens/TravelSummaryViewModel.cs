using Common.Operations;
using Entities.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices.structs;

namespace Sociatis.Models.Citizens
{
    public class TravelSummaryViewModel
    {
        public bool CanTravel { get; set; }
        public string CannotTravelReason { get; set; }
        public double? Distance { get; set; }
        public double TicketTravelDistance { get; set; }
        public int HpLoss { get; set; }

        public TravelSummaryViewModel(MethodResult canTravel, Path travelPath, MovingTicket ticket)
        {
            CanTravel = canTravel.isSuccess;
            if (CanTravel == false)
                CannotTravelReason = canTravel.ToString();

            if (Math.Abs((travelPath?.Distance ?? 0) - 0) < 0.0001)
                Distance = null;
            else
                Distance = travelPath.Distance;
            TicketTravelDistance = ticket.TravelDistance;
            HpLoss = ticket.HpLoss;
        }
    }
}