using Entities;
using Sociatis.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUtils;
using WebUtils.Extensions;

namespace Sociatis.Models.Congress
{
    public class CongressVotingsPartialViewModel
    {
        public List<CongressVotingsItemViewModel> Votings { get; set; } = new List<CongressVotingsItemViewModel>();
        public PagingParam PagingParam { get; set; }
        public int CountryID { get; set; }

        public CongressVotingsPartialViewModel(int countryID, IQueryable<CongressVoting> votingsQuery, PagingParam pagingParam)
        {
            PagingParam = pagingParam;

            PagingParam.RecordsPerPage = Config.CongressVotingsPerPage;

            var votings = votingsQuery
                .OrderByDescending(cv => cv.ID)
                .Apply(pagingParam).ToList();

            foreach (var voting in votings)
            {
                Votings.Add(new CongressVotingsItemViewModel(voting));
            }
        }
    }
}