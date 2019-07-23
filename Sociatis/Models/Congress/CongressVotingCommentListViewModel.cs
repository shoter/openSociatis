using Entities;
using Sociatis.Code;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUtils;
using WebUtils.Extensions;

namespace Sociatis.Models.Congress
{
    public class CongressVotingCommentListViewModel
    {
        public List<CongressVotingCommentViewModel> Comments { get; set; } = new List<CongressVotingCommentViewModel>();
        public PagingParam PagingParam { get; set; } = new PagingParam();
        public int CongressVotingID { get; set; }

        public CongressVotingCommentListViewModel(PagingParam pp, ICollection<CongressVotingComment> comments, CongressVoting voting)
        {
            PagingParam = pp;
            PagingParam.RecordsPerPage = Config.CongressVotingCommentsPerPage;


            ///To list to ensure that it is not queryable
            foreach(var comment in comments.ToList().Apply(pp))
            {
                Comments.Add(new CongressVotingCommentViewModel(comment));
            }
        }
    }
}