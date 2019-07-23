using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;
using Entities.Extensions;
using Entities.enums;
using Sociatis.Helpers;

namespace Sociatis.Models.Congress
{
    public class CongressCandidateViewModel
    {
        public ImageViewModel Avatar { get; set; }
        public string Name { get; set; }
        public string RegionName { get; set; }
        public CongressCandidateStatusEnum CandidateStatus { get; set; }
        public int CandidateID { get; set; }
        public bool SameRegion { get; set; } = false;
        public bool CanResign { get; set; } = false;
        public bool IsActualCitizen { get; set; }
        public bool IsAccepted { get; set; }

        public CongressCandidateViewModel(CongressCandidate candidate)
        {
            var entity = candidate.Citizen.Entity;
            Name = entity.Name;
            CandidateID = candidate.ID;
            RegionName = entity.GetCurrentRegion().Name;
            Avatar = new ImageViewModel(entity.ImgUrl);
            CandidateStatus = (CongressCandidateStatusEnum)candidate.CongressCandidateStatusID;

            if (SessionHelper.CurrentEntity.GetCurrentRegion().ID == candidate.RegionID)
                SameRegion = true;

            if (candidate.CandidateID == SessionHelper.CurrentEntity.EntityID && candidate.CongressCandidateVoting.VotingStatusID == (int)VotingStatusEnum.NotStarted)
                CanResign = true;

            IsAccepted = (candidate.CongressCandidateStatusID == (int)CongressCandidateStatusEnum.Approved);
            IsActualCitizen = candidate.CandidateID == SessionHelper.LoggedCitizen.ID;
        }
    }
}