using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.structs
{
    public class Neighbour
    {
        public Region Region { get; set; }
        public Passage Passage { get; set; }

        public Neighbour(Region region, Passage passage)
        {
            Region = region;
            Passage = passage;
        }

        public override string ToString()
        {
            return $"{Region.Name} - {Passage.Distance}";
        }
    }
}
