using Common.Exceptions;
using Entities;
using Entities.enums;
using Entities.Repository;
using Sociatis.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.structs.Votings;
using WebUtils.Mvc;

namespace Sociatis.Models.Congress
{
    public class BuildDefenseSystemViewModel : CongressVotingViewModel
    {
        public List<SelectListItem> QualitySelect { get; set; } = new CustomSelectList()
            .AddNumbers(1, 5);


        [Range(1, 5)]
        public int Quality { get; set; }

        public List<SelectListItem> RegionSelect { get; set; }


        [DisplayName("Region")]
        public int RegionID { get; set; }

        public List<double> GoldCost { get; set; } = new List<double>();
        public List<int> CPCost { get; set; } = new List<int>();



        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.BuildDefenseSystem;

        public BuildDefenseSystemViewModel()
        {
            fillGoldCost();
            fillCPCost();
        }
        public BuildDefenseSystemViewModel(int countryID) : base(countryID)
        {
            fillGoldCost();
            fillCPCost();
            fillRegions();
        }
        public BuildDefenseSystemViewModel(CongressVoting voting)
        : base(voting)
        {
            fillGoldCost();
            fillCPCost();
            fillRegions();
        }

        public BuildDefenseSystemViewModel(FormCollection values)
        : base(values)
        {
            fillGoldCost();
            fillCPCost();
            fillRegions();
        }


        public override StartCongressVotingParameters CreateVotingParameters()
        {
            var defenseSystemService = DependencyResolver.Current.GetService<IDefenseSystemService>();
            var countryRepository = DependencyResolver.Current.GetService<ICountryRepository>();
            var regionRepository = DependencyResolver.Current.GetService<IRegionRepository>();
            var country = countryRepository.GetById(CountryID);
            var region = regionRepository.GetById(RegionID);
            var result = defenseSystemService.CanBuildDefenseSystem(region, country, Quality);

            if (result.IsError)
                throw new UserReadableException(result.ToString("."));

            return new BuildDefenseSystemVotingsParameters(RegionID, Quality);
        }

        private void fillGoldCost()
        {
            GoldCost = new List<double>();
            var defenseSystemService = DependencyResolver.Current.GetService<IDefenseSystemService>();
            for (int i = 1; i <= 5; ++i)
            {
                GoldCost.Add((double)defenseSystemService.GetGoldCostForStartingConstruction(i));
            }
        }

        private void fillCPCost()
        {
            CPCost = new List<int>();
            var defenseSystemService = DependencyResolver.Current.GetService<IDefenseSystemService>();
            for (int i = 1; i <= 5; ++i)
            {
                CPCost.Add(defenseSystemService.GetNeededConstructionPoints(i));
            }
        }

        public void fillRegions()
        {
            var regionRepository = DependencyResolver.Current.GetService<IRegionRepository>();
            var regions = regionRepository.GetCountryRegions(CountryID)
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.ID.ToString()
                }).ToList();

            RegionSelect = regions;
        }
    }
}