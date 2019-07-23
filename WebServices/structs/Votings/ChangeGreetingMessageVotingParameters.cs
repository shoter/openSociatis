using Entities;
using Entities.enums;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebServices.structs.Votings
{
    public class ChangeGreetingMessageVotingParameters : StartCongressVotingParameters
    {
        public string Message { get; set; }

        public override VotingTypeEnum VotingType { protected set; get; } = VotingTypeEnum.ChangeGreetingMessage;

        public ChangeGreetingMessageVotingParameters(string message)
        {
            this.Message = message;
        }

        public override void FillCongressVotingArguments(CongressVoting voting)
        {
            var repository = DependencyResolver.Current.GetService<IVotingGreetingMessageRepository>();
            var id = repository.Add(Message);
            voting.Argument1 = id.ToString();
        }
    }
}
