using Entities;
using Entities.enums;
using Sociatis.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices.structs.Votings;

namespace Sociatis.Models.Congress
{
    public abstract class CongressVotingViewModel : ViewModelBase
    {
        public int CreatorID { get; set; }
        public string CreatorName { get; set; }
        public int VotingID { get; set; }
        public virtual int CountryID { get; set; }

        protected CongressVotingViewModel() { }
        public CongressVotingViewModel(int countryID)
        {
            CountryID = countryID;
        }
        public CongressVotingViewModel(Entities.CongressVoting voting)
        {
            CreatorID = voting.Citizen.ID;
            VotingID = voting.ID;
            CreatorName = voting.Citizen.Entity.Name;
            CountryID = voting.CountryID;
        }
        public CongressVotingViewModel(FormCollection values)
        {
            Message = values["message"] ?? "";

            var properties = this.GetType().GetProperties();
            var baseProperties = typeof(CongressVotingViewModel).GetProperties();
            CountryID = Convert.ToInt32(values["CountryID"]);
            foreach (var property in properties)
            {
                if (baseProperties.Any(p => p.Name == property.Name))
                    continue;

                if (string.IsNullOrWhiteSpace(values[property.Name]) == false)
                {
                    if (property.PropertyType.IsEnum)
                    {
                        var value = int.Parse(values[property.Name]);
                        
                        property.SetValue(this, Enum.Parse(property.PropertyType, values[property.Name], true));
                    }
                    else
                    {
                        property.SetValue(this, Convert.ChangeType(values[property.Name], property.PropertyType));
                    }
                }
            }
        }
        public abstract VotingTypeEnum VotingType { get; set; }
        public string Message { get; set; }
        public abstract StartCongressVotingParameters CreateVotingParameters();
    }
}