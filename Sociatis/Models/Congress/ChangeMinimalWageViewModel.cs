using Entities;
using Entities.enums;
using Sociatis.Models.Congress.Votings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices.structs.Votings;

namespace Sociatis.Models.Congress
{
    public class ChangeMinimalWageViewModel : CongressVotingViewModel
    {
        [DisplayName("New minimal wage")]
        [Range(0.01, 100000)]
        public decimal NewMinimalWage { get; set; }



        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.ChangeMinimalWage;

        public ChangeMinimalWageViewModel() { }
        public ChangeMinimalWageViewModel(int countryID) :base(countryID)
    {
        }
        public ChangeMinimalWageViewModel(CongressVoting voting)
        :base(voting)
    {
        }

        public ChangeMinimalWageViewModel(FormCollection values)
        :base(values)
    {
        }


        public override StartCongressVotingParameters CreateVotingParameters()
        {
            return new ChangeMinimalWageVotingParameters(NewMinimalWage);
        }
    }

}