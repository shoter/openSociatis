using Common.utilities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class PassageDummyCreator
    {
        Passage passage;
        private static UniqueIDGenerator idGenerator = new UniqueIDGenerator();
        public PassageDummyCreator()
        {
            passage = create();
        }

        public Passage Create(Region firstRegion, Region secondRegion)
        {
            passage.FirstRegion = firstRegion;
            passage.FirstRegionID = firstRegion.ID;

            passage.SecondRegion = secondRegion;
            passage.SecondRegionID = secondRegion.ID;

            firstRegion.Passages.Add(passage);
            secondRegion.Passages1.Add(passage);

            var _return = passage;
            passage = create();
            return _return;
        }


        private Passage create()
        {
            return new Passage()
            {
                ID = idGenerator.UniqueID,
                Distance = 1
            };
        }
    }
}
