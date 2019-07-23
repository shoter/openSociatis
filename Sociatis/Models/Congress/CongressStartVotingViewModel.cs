using Common.Extensions;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Models.Congress
{
    public class CongressStartVotingViewModel : CongressBase
    {
        public List<SelectListItem> VotingTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CommentRestrictions { get; set; } = new List<SelectListItem>();
        public int CountryID { get; set; }
        public int SelectedVotingTypeID { get; set; } = 0;
        /// <summary>
        /// Vote that was posted in the POST request.
        /// </summary>
        public CongressVotingViewModel EmbeddedVote { get; protected set; }
        public CongressStartVotingViewModel() { }

        public CongressStartVotingViewModel(List<VotingType> votingTypes, List<CommentRestriction> commentRestrictions, Entities.Country country)
            :base(country)
        {
            CountryID = country.ID;
            VotingTypes = CreateSelectList(votingTypes, vt => ((VotingTypeEnum)vt.ID).ToHumanReadable(), vt => vt.ID, true, "Select voting type")
                .OrderBy(vt => vt.Text)
                .ToList();

            var countryPolicy = country.CountryPolicy;

            AddMoreVotingTypesBasedOnPolicy(countryPolicy);

            CommentRestrictions = CreateSelectList(commentRestrictions,
                cr => ((CommentRestrictionEnum)cr.ID).ToHumanReadable().FirstUpper(),
                cr => cr.ID,
                false);
        }

        private void AddMoreVotingTypesBasedOnPolicy(CountryPolicy countryPolicy)
        {
            if (countryPolicy.CountryCompanyBuildLawAllowHolder == (int)LawAllowHolderEnum.Congress || countryPolicy.CountryCompanyBuildLawAllowHolder == (int)LawAllowHolderEnum.PresidentAndCongress)
                AddMoreVotingTypesBasedOnPolicy(VotingTypeEnum.CreateNationalCompany);

            VotingTypes = VotingTypes.OrderBy(vt => vt.Text)
                .ToList();
        }

        private void AddMoreVotingTypesBasedOnPolicy(VotingTypeEnum votingType)
        {
            VotingTypes.Add(new SelectListItem()
            {
                Text = votingType.ToHumanReadable(),
                Value = ((int)votingType).ToString()
            });
        }

        public void EmbedPostVote(CongressVotingViewModel vote)
        {
            EmbeddedVote = vote;
            SelectedVotingTypeID = (int)vote.VotingType;
        }
    }
}