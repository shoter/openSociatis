using Common.EntityFramework;
using Common.Operations;
using Entities.enums;
using Entities.Models.Constructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IConstructionRepository : IRepository<Construction>
    {
        bool AnyConstructionTypeBuildInRegion(int regionID, ProductTypeEnum constructionType);
        List<NationalConstruction> GetNationalConstructions(int countryID);
    }
}
