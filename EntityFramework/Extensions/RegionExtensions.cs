using Entities.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class RegionExtensions
    {
        public static List<Neighbour> GetNeighbours(this Region region)
        {
            List<Neighbour> neighbours = new List<Neighbour>();

            List<Passage> passages = new List<Passage>();

            passages.AddRange(region.Passages);
            passages.AddRange(region.Passages1);

            foreach(var passage in passages)
            {
                if(passage.FirstRegionID == region.ID)
                {
                    neighbours.Add(new Neighbour(passage.SecondRegion, passage));
                }
                else
                {
                    neighbours.Add(new Neighbour(passage.FirstRegion, passage));
                }
            }

            return neighbours;
        }

    }
}
