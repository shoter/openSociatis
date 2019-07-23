using Common.Converters;
using Common.Extensions;
using Entities;
using Entities.enums;
using Entities.Extensions;
using Sociatis.Models.Friends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Sociatis.Models.Citizens
{
    public class ViewCitizenViewModel
    {
        public ImageViewModel Avatar { get; set; }
        public string Name { get; set; }
        public string RegionName { get; set; }
        public string CountryName { get; set; }

        public int HitPoints { get; set; }
        public int Experience { get; set; }
        public int NextLevelExperience { get; set; }
        public int ExperienceProgress { get; set; }
        public int Level { get; set; }

        public string MilitaryRank { get; set; }
        public string NextMilitaryRank { get; set; }
        public int NextMilitaryRankProgress { get; set; }
        public string MilitaryRankIconCss { get; set; }

        public double Manufacturing { get; set; }
        public double Raw { get; set; }
        public double Construction { get; set; }
        public double Selling { get; set; }

        public string Strength { get; set; }
        public CitizenInfoViewModel Info { get; set; }
        public CitizenJobShortViewModel Job { get; set; }
        public MedalViewModel Medals { get; set; }

        public List<ViewCitizenFriendModel> Friends { get; set; } = new List<ViewCitizenFriendModel>();
        public int FriendCount { get; set; } = 0;

        public ViewCitizenViewModel(Entities.Citizen citizen, IList<Citizen> friends, ICitizenService citizenService, IFriendService friendService)
        {
            var entity = citizen.Entity;

            Info = new CitizenInfoViewModel(citizen, friendService);
            Avatar = new ImageViewModel(entity.ImgUrl);
            Name = entity.Name;
            RegionName = citizen.Region.Name;
            CountryName = citizen.Region.Country.Entity.Name;

            HitPoints = citizen.HitPoints;
            Experience = citizen.Experience;
            Level = citizen.ExperienceLevel;
            NextLevelExperience = citizenService.CalculateExperienceForNextLevel(citizen.ExperienceLevel);

            Manufacturing = (double)citizen.Manufacturing;
            Raw = (double)citizen.Raw;
            Selling = (double)citizen.Selling;
            Construction = (double)citizen.Construction;

            ExperienceProgress = (int)(Experience / (double)NextLevelExperience * 100.0);

            int militaryRank = (int)citizen.MilitaryRank;
            int nextMilitaryRank = (MilitaryRankEnumExtensions.CalculateNextMilitaryRankNeeded(militaryRank) ?? militaryRank);

            MilitaryRank = militaryRank.ConvertToBasicUnits();
            NextMilitaryRank = nextMilitaryRank.ConvertToBasicUnits();
            NextMilitaryRankProgress = (int)(militaryRank / (double)nextMilitaryRank * 100.0);
            MilitaryRankIconCss = MilitaryRankEnumExtensions.GetRankCss(MilitaryRankEnumExtensions.GetRankForMilitaryRank(militaryRank));

            Strength = string.Format("{0:0.##}", citizen.Strength);

            Medals = new MedalViewModel(citizen);

            var company = citizen.GetCurrentJob();

            if (company != null)
                Job = new CitizenJobShortViewModel(company);

            Friends = friends
                .TakeRandom(4)
                .Select(cit => new ViewCitizenFriendModel(cit))
                .ToList();

            FriendCount = friends.Count();
        }
    }
}
