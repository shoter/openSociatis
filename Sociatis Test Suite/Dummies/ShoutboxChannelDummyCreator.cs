using Common.utilities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class ShoutboxChannelDummyCreator : IDummyCreator<ShoutboxChannel>
    {
        private static UniqueIDGenerator uniqueID = new UniqueIDGenerator();
        private ShoutboxChannel channel;

        public ShoutboxChannelDummyCreator()
        {
            channel = create();
        }

        private ShoutboxChannel create()
        {
            return new ShoutboxChannel()
            {
                ID = uniqueID,
                Name = RandomGenerator.GenerateString(10)
            };
        }

        public ShoutboxChannelDummyCreator SetName(string name)
        {
            channel.Name = name;
            return this;
        }
        public ShoutboxChannel Create()
        {
            var temp = channel;
            channel = create();
            return temp;
        }
    }
}
