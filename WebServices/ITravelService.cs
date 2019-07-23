using Common.Operations;
using Entities;
using Entities.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface ITravelService
    {
        MethodResult CanTravel(Citizen citizen, Region startRegion, Region endRegion, MovingTicket ticket);
        void Travel(int citizenID, Region startRegion, Region endRegion, MovingTicket ticket);
    }
}
