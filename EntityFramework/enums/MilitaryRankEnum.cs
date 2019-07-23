
using Common.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum MilitaryRankEnum
    {
        Private,
        PrivateFirstClass,
        Corporal,
        Sergeant,
        StaffSergeant,
        SergeantFirstClass,
        MasterSergeant,
        FirstSergeant,
        SergeantMajor, 
        CommandSergeantMajor,
        SergeantMajorOfTheArmy,
        SecondLieutenant,
        FirstLieutenant,
        Captain,
        Major,
        LieutenantColonel,
        Colonel,
        BrigadierGeneral,
        MajorGeneral,
        LieutenantGeneral,
        General,
        GeneralOfTheArmy
    }

    public static class MilitaryRankEnumExtensions
    {
        public static Dictionary<MilitaryRankEnum, int> ExperienceNeededForRank { get; private set; } = new Dictionary<MilitaryRankEnum, int>()
        {

        };

        static MilitaryRankEnumExtensions()
        {
            int exp = 100;
            int speed = 500;
            double mod = 1;
            double modAcc = 1.1;
            int acc = 500;

            ExperienceNeededForRank.Add(MilitaryRankEnum.Private, 0);

            foreach (MilitaryRankEnum rank in Enum.GetValues(typeof(MilitaryRankEnum)))
            {
                if (rank == MilitaryRankEnum.Private) continue;

                ExperienceNeededForRank.Add(rank, exp);

                exp += speed;
                speed += acc;
                mod *= modAcc;
                modAcc += 0.05;
                acc = (int)(500.0 * mod) + speed; 
                
            }
        }

        public static MilitaryRankEnum GetRankForMilitaryRank(double militaryRank)
        {
            foreach (var pair in ExperienceNeededForRank.Reverse())
            {
                if (pair.Value <= militaryRank)
                    return pair.Key;
            }

            return MilitaryRankEnum.Private;
        }

        public static string GetRankCss(MilitaryRankEnum rank)
        {
            switch (rank)
            {
                case MilitaryRankEnum.Private:
                    return "rank-03-e2";
                case MilitaryRankEnum.PrivateFirstClass:
                    return "rank-03-e3";
                case MilitaryRankEnum.Corporal:
                    return "rank-03-e4-1";
                case MilitaryRankEnum.Sergeant:
                    return "rank-03-e5";
                case MilitaryRankEnum.StaffSergeant:
                    return "rank-01-e6";
                case MilitaryRankEnum.FirstSergeant:
                    return "rank-01-e8-2";
                case MilitaryRankEnum.MasterSergeant:
                    return "rank-01-e9-1";
                case MilitaryRankEnum.SergeantMajor:
                    return "rank-03-e9-3";
                case MilitaryRankEnum.CommandSergeantMajor:
                    return "rank-03-e9-2";
                case MilitaryRankEnum.SergeantMajorOfTheArmy:
                    return "rank-03-e9-1";
                case MilitaryRankEnum.SecondLieutenant:
                    return "rank-01-o1";
                case MilitaryRankEnum.FirstLieutenant:
                    return "rank-01-o2";
                case MilitaryRankEnum.Captain:
                    return "rank-01-o3";
                case MilitaryRankEnum.Major:
                    return "rank-01-o4";
                case MilitaryRankEnum.LieutenantColonel:
                    return "rank-01-o5";
                case MilitaryRankEnum.Colonel:
                    return "rank-01-o6";
                case MilitaryRankEnum.BrigadierGeneral:
                    return "rank-01-o7";
                case MilitaryRankEnum.MajorGeneral:
                    return "rank-01-o8";
                case MilitaryRankEnum.LieutenantGeneral:
                    return "rank-01-o9";
                case MilitaryRankEnum.General:
                case MilitaryRankEnum.GeneralOfTheArmy:
                    return "rank-01-o10";
                

                default:
                    return "";

            }
        }

        public static int? CalculateNextMilitaryRankNeeded(double militaryRank)
        {
            var rank = GetRankForMilitaryRank(militaryRank);
            if (rank == MilitaryRankEnum.GeneralOfTheArmy)
                return null;

            return ExperienceNeededForRank[rank + 1];
        }

        public static string ToHumanReadable(this MilitaryRankEnum rank)
        {
            return rank.ToString().PascalCaseToWord();
        }
    }
}
