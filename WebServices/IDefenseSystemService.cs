using Common.Operations;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IDefenseSystemService
    {
        decimal GetGoldCostForStartingConstruction(int quality);
        MethodResult CanBuildDefenseSystem(Region region, Country country, int quality);
        int GetNeededConstructionPoints(int quality);
        void BuildDefenseSystem(Region region, Country country, int quality, IConstructionService constructionService);
    }
}
