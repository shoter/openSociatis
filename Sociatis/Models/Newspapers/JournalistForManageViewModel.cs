using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Newspapers
{
    public class JournalistForManageViewModel
    {
        public int CitizenID { get; set; }
        public string Name { get; set; }
        public bool CanWriteArticles { get; set; }
        public bool CanManageJournalists { get; set; }
        public bool CanManageArticles { get; set; }

        public JournalistForManageViewModel(NewspaperJournalist journalist)
        {
            CitizenID = journalist.CitizenID;
            Name = journalist.Citizen.Entity.Name;
            CanManageArticles = journalist.CanManageArticles;
            CanManageJournalists = journalist.CanManageJournalists;
            CanWriteArticles = journalist.CanWriteArticles;
        }

        public JournalistForManageViewModel() { }

        public NewspaperRightsEnum GetRights()
        {
            var rights = NewspaperRightsEnum.None;

            if (CanWriteArticles)
                rights |= NewspaperRightsEnum.CanWriteArticles;
            if (CanManageArticles)
                rights |= NewspaperRightsEnum.CanManageArticles;
            if (CanManageJournalists)
                rights |= NewspaperRightsEnum.CanManageJournalists;

            return rights;
        }


    }
}