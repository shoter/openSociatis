using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IRemovalService
    {
        void RemoveCompany(Company company);
        void CloseTrade(Trade trade, int removedEntityID);

        void RemoveEntity(Entity entity);
    }
}
