using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class VotingGreetingMessageRepository : RepositoryBase<VotingGreetingMessage, SociatisEntities>, IVotingGreetingMessageRepository
    {
        public VotingGreetingMessageRepository(SociatisEntities context) : base(context)
        {
        }

        public int Add(string message)
        {
            var msg = new VotingGreetingMessage()
            {
                Message = message
            };

            Add(msg);
            SaveChanges();

            return msg.ID;
        }

        public void RemoveUnused()
        {
            throw new NotImplementedException();
        }
    }
}
