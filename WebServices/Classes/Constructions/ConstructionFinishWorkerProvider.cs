using Entities;
using Entities.enums;
using Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.Classes.Constructions
{
    public static class ConstructionFinishWorkerProvider
    {
        public static IConstructionFinishWorker Provide(Construction construction)
        {
            switch (construction.Company.GetProducedProductType())
            {
                case ProductTypeEnum.DefenseSystem:
                    return new DefeneseSystemFinishWorker();
                case ProductTypeEnum.Hotel:
                    return new HotelFinishWorker();
            }
            throw new NotImplementedException();
        }
    }
}
