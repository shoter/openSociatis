using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.PathFinding
{
    public interface IPathFindingCostCalculator
    {
        double CalculatePassageCost(Passage passage);
    }
}
