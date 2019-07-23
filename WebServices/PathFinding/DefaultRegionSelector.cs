using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.structs;

namespace WebServices.PathFinding
{
    public class DefaultRegionSelector : IPathFindingRegionSelector
    {
        public bool IsPassableRegion(Neighbour neighbour)
        {
            return true; //everything is allowed here
        }
    }
}
