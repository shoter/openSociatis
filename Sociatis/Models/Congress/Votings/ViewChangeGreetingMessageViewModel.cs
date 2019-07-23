using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entities.enums;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewChangeGreetingMessageViewModel : ViewVotingBaseViewModel
    {
        public string GreetingMessage { get; set; }

        public ViewChangeGreetingMessageViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
        :base(voting, isPlayerCongressman, canVote)
    {
            var greetingID = int.Parse(voting.Argument1);
            var greetingRepo = DependencyResolver.Current.GetService<IVotingGreetingMessageRepository>();
            GreetingMessage = greetingRepo.GetById(greetingID).Message;
        }
    }
}