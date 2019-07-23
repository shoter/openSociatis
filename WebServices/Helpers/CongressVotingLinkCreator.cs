using Entities;
using Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Weber.Html;

namespace WebServices.Helpers
{
    public static class CongressVotingLinkCreator
    {
        public static MvcHtmlString Create(CongressVoting voting, string @class = null)
        {
            return LinkCreator.Create(voting.GetName(),
                "ViewVoting",
                "Congress",
                new { votingID = voting.ID },
                @class);
        }
    }
}
