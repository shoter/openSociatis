using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;
using WebServices;
using Sociatis.Helpers;
using Entities.Extensions;
using Entities.enums;

namespace Sociatis.Models.Congress
{
    public class CongressCandidatesListViewModel : List<CongressCandidateViewModel>
    {
        public bool CanVote { get; set; }
        public CongressCandidatesListViewModel(List<CongressCandidate> candidates, bool canVote = false)
        {
            foreach (var candidate in candidates)
                Add(new CongressCandidateViewModel(candidate));

            CanVote = canVote;
        }
    }
}