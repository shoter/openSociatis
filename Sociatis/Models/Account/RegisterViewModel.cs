using Entities.Repository;
using Sociatis.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Entities;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Models.Account
{
    public class RegisterViewModel : ViewModelBase
    {
        [Required]
        [DisplayName("Citizen name")]
        [StringLength(30, ErrorMessage = "Must be between 2 and 30 characters", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [DisplayName("Email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Must be between 6 and 255 characters", MinimumLength = 2)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DisplayName("Country")]
        public int? CountryID { get; set; }

        [Required]
        [DisplayName("Birth region")]
        public int? RegionID { get; set; }

        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> RegionsList { get; set; }

        public RegisterViewModel()
        {
            CountryList = new List<SelectListItem>();
            RegionsList = new List<SelectListItem>();
        }

        public void LoadSelectLists(ICountryRepository countriesRepository, IRegionService regionService)
        {
            var countries = countriesRepository.GetAll()
                .OrderBy(c => c.Entity.Name);

            CountryList = CreateSelectList(countries, t => t.Entity.Name, t => t.ID , true, "Select country");

            if(CountryID != null)
            {
                var country = countries
                    .First(c => c.ID == CountryID);

                var regions = regionService.GetBirthableRegions(CountryID.Value);

                RegionsList = regions.Select(r => new SelectListItem()
                {

                    Text = r.CountryCoreID == r.CountryID ? r.Name : r.Name + "(occupied)"

                ,
                    Value = r.ID.ToString()
                }).ToList();
            }
        }



    }
}