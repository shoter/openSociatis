using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;
using System.Web.Mvc;
using System.Web.Routing;
using Entities.enums;
using Sociatis.Controllers;

namespace Sociatis.Models.Navigation
{
    public class NavigationViewModel
    {
        public int Day { get; set; }
        public List<NavigationSectionViewModel> MainSections { get; set; } = new List<NavigationSectionViewModel>();

        public void AddSociety()
        {
            NavigationSectionViewModel society = new NavigationSectionViewModel()
            {
                Name = "Society"
            };

            society.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Country"
            });

            society.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Party"
            });

            society.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Newspaper"
            });

            society.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Forum"
            });

            society.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Chat"
            });

            society.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Worldmap"
            });

            MainSections.Add(society);
        }

        internal void AddCountry(Entities.Country country, UrlHelper urlHelper)
        {
            NavigationSectionViewModel countrySection = new NavigationSectionViewModel()
            {
                Name = country.Entity.Name
            };

            countrySection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Information",
                Url = urlHelper.Action("View", "Country", new { countryID = country.ID })
            });

            countrySection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Geography",
                Url = urlHelper.Action("Geography", "Country", new { countryID = country.ID })
            });

            countrySection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "President",
                Url = urlHelper.Action("President", "Country", new { countryID = country.ID })
            });

            countrySection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Congress",
                Url = urlHelper.Action("View", "Congress", new { countryID = country.ID })
            });

           

            MainSections.Add(countrySection);
        }

        internal void AddMap(UrlHelper urlHelper)
        {
            NavigationSectionViewModel mapSection = new NavigationSectionViewModel()
            {
                Name = "Map"
            };

            mapSection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Political",
                Url = urlHelper.Action("NormalMode", "Map")
            });

            mapSection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Developement",
                Url = urlHelper.Action(nameof(MapController.DevelopementMode), "Map")
            });

            var resources = new NavigationSectionViewModel()
            {
                Name = "Resources",
            };

            foreach (ResourceTypeEnum resource in Enum.GetValues(typeof(ResourceTypeEnum)).Cast<ResourceTypeEnum>())
            {
                resources.Children.Add(new NavigationSectionViewModel()
                {
                    Name = resource.ToHumanReadable(),
                    Url = urlHelper.Action("ResourceMode", "Map", new { resource = resource })
                });
            }

            mapSection.Children.Add(resources);

            MainSections.Add(mapSection);

        }

        public void AddRankings(UrlHelper urlHelper)
        {
            NavigationSectionViewModel rankings = new NavigationSectionViewModel()
            {
                Name = "Rankings"
            };

            rankings.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Best citizens",
                Url = urlHelper.Action("Citizens", "Best")
            });

          /*  rankings.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Best countries"
            });

            rankings.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Best newspapers"
            });

            rankings.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Best parties"
            });*/

            MainSections.Add(rankings);
        }

        public void AddWars()
        {
            NavigationSectionViewModel wars = new NavigationSectionViewModel()
            {
                Name = "Wars"
            };

            wars.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Training"
            });

            wars.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Army"
            });

            wars.Children.Add(new NavigationSectionViewModel()
            {
                Name = "All wars"
            });

            wars.Children.Add(new NavigationSectionViewModel()
            {
                Name = "%Country% wars"
            });

            wars.Children.Add(new NavigationSectionViewModel()
            {
                Name = "All alliances"
            });

            wars.Children.Add(new NavigationSectionViewModel()
            {
                Name = "%Country% alliances"
            });

            MainSections.Add(wars);
        }

        public void AddMarket(Entities.Country country, UrlHelper urlHelper)
        {
            NavigationSectionViewModel market = new NavigationSectionViewModel()
            {
                Name = "Market"
            };

            market.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Hotels",
                Url = urlHelper.Action("Index", "Hotel")
            });

            market.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Houses",
                Url = urlHelper.Action("Houses", "MarketOffer", new { countryID = country.ID })
            });

            market.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Products",
                Url = urlHelper.Action("MarketOffers", "MarketOffer", new { countryID = country.ID, quality = 0, productID = 0 })
            });

            market.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Resources",
                Url = urlHelper.Action("ResourceOffers", "MarketOffer", new { countryID = country.ID, quality = 0, productID = 0 })
            });

            market.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Jobs",
                Url = urlHelper.Action("JobMarket", "JobOffer")
            });

            MainSections.Add(market);
        }

    }
}