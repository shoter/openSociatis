using Common.utilities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class ShoutboxMessageDummyCreator : IDummyCreator<ShoutboxMessage>
    {
        private static UniqueIDGenerator uniqueID = new UniqueIDGenerator();
        private EntityDummyCreator entityDummyCreator = new EntityDummyCreator();


        private ShoutboxMessage message;
        public ShoutboxMessageDummyCreator()
        {
            message = create();
        }

        private ShoutboxMessage create()
        {
            var author = entityDummyCreator.Create();
            return new ShoutboxMessage()
            {
                Message = RandomGenerator.GenerateString(10),
                AuthorID = author.EntityID
                
            };
        }

        public ShoutboxMessageDummyCreator SetAuthor(Entity entity)
        {
            message.Author = entity;
            message.AuthorID = entity.EntityID;
            return this;
        }

        public ShoutboxMessage Create(ShoutboxChannel channel)
        {
            var temp = message;
            temp.ShoutboxChannel = channel;
            temp.ChannelID = channel.ID;
            message = create();
            return temp;
        }

        public ShoutboxMessage Create()
        {
            throw new NotImplementedException();
        }
    }
}
