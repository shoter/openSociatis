using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Country
{
    public class PresidentCandidateViewModel
    {
        public string CandidateName { get; set; }
        public int CandidateID { get; set; }
        public int CitizenID { get; set; }
        public ImageViewModel CandidateAvatar { get; set; }

        public PresidentCandidateViewModel(Entities.PresidentCandidate candidate)
        {
            CandidateAvatar = new ImageViewModel(candidate.Citizen.Entity.ImgUrl);
            CandidateName = candidate.Citizen.Entity.Name;
            CandidateID = candidate.ID;
            CitizenID = candidate.CandidateID;
        }
    }
}