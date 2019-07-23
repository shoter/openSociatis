using Entities.enums;
using Sociatis.Models.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entities;
using Entities.Repository;
using WebServices;
using Entities.Extensions;

namespace Sociatis.Helpers
{
    public class NavigationHelper
    {
        public static void PrepareNavigation(ref NavigationViewModel navigation, UrlHelper urlHelper)
        {
            var entity = SessionHelper.CurrentEntity;


            switch ((EntityTypeEnum)entity.EntityTypeID)
            {
                case EntityTypeEnum.Citizen:
                    {
                        CreateCitizenNavigation(navigation, entity, urlHelper);
                        break;
                    }
                case EntityTypeEnum.Company:
                    {
                        CreateCompanyNavigation(navigation, entity, urlHelper);
                        break;
                    }
                case EntityTypeEnum.Party:
                    {
                        CreatePartyNavigation(navigation, entity, urlHelper);
                        break;
                    }
                case EntityTypeEnum.Newspaper:
                    {
                        CreateNewspaperNavigation(navigation, entity, urlHelper);
                        break;
                    }
                case EntityTypeEnum.Organisation:
                    {
                        CreateOrganisationNavigation(navigation, entity, urlHelper);
                        break;
                    }
                case EntityTypeEnum.Country:
                    {
                        break;
                    }
            }
        }

        private static void CreateOrganisationNavigation(NavigationViewModel navigation, Entity entity, UrlHelper urlHelper)
        {
            NavigationSectionViewModel organisationSection = new NavigationSectionViewModel()
            {
                Name = "Organisation"
            };

            organisationSection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Profile",
                Url = urlHelper.Action("View", "Organisation", new { newspaperID = entity.EntityID })
            });

            organisationSection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Wallet",
                Url = urlHelper.Action("Wallet", "Organisation", new { newspaperID = entity.EntityID })
            });

            navigation.MainSections.Add(organisationSection);

            createBusinessSection(navigation, urlHelper, createCompanies: true, createOrganisations: false, createNewspapers: true);
        }

        private static void CreateNewspaperNavigation(NavigationViewModel navigation, Entities.Entity entity, UrlHelper urlHelper)
        {
            NavigationSectionViewModel newspaperSection = new NavigationSectionViewModel()
            {
                Name = "Newspaper"
            };


            newspaperSection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Profile",
                Url = urlHelper.Action("View", "Newspaper", new { newspaperID = entity.EntityID })
            });

            newspaperSection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Create article",
                Url = urlHelper.Action("WriteArticle", "Newspaper", new { newspaperID = entity.EntityID })
            });

            newspaperSection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Manage articles",
                Url = urlHelper.Action("ManageArticles", "Newspaper", new { newspaperID = entity.EntityID })
            });

            newspaperSection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Manage journalists",
                Url = urlHelper.Action("ManageJournalists", "Newspaper", new { newspaperID = entity.EntityID })
            });

            navigation.MainSections.Add(newspaperSection);
        }

        private static void CreatePartyNavigation(NavigationViewModel navigation, Entities.Entity entity, UrlHelper urlHelper)
        {
            NavigationSectionViewModel partySection = new NavigationSectionViewModel()
            {
                Name = "Party"
            };

            var party = entity.Party;

            partySection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Messages",
                Url = urlHelper.Action("Index", "Message")
            });

            partySection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Profile",
                Url = urlHelper.Action("View", "Party", new { partyID = party.ID })
            });

            partySection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Newspapers",
                Url = urlHelper.Action("Index", "Newspaper")
            });

            navigation.MainSections.Add(partySection);
        }

        private static void CreateCompanyNavigation(NavigationViewModel navigation, Entity entity, UrlHelper urlHelper)
        {
            NavigationSectionViewModel companySection = new NavigationSectionViewModel()
            {
                Name = "Company"
            };

            var company = entity.Company;
            var companyService = DependencyResolver.Current.GetService<ICompanyService>();

            var rights = companyService.GetCompanyRights(company, entity, SessionHelper.LoggedCitizen);

            companySection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Profile",
                Url = urlHelper.Action("View", "Company", new { companyID = company.ID })
            });

            if (rights.CanManageEquipment)
                companySection.Children.Add(new NavigationSectionViewModel()
                {
                    Name = "Inventory",
                    Url = urlHelper.Action("Inventory", "Company", new { companyID = company.ID })
                });

            companySection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Messages",
                Url = urlHelper.Action("Index", "Message")
            });

            companySection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Newspapers",
                Url = urlHelper.Action("Index", "Newspaper")
            });

            if (rights.CanSeeWallet)
                companySection.Children.Add(new NavigationSectionViewModel()
                {
                    Name = "Wallet",
                    Url = urlHelper.Action("Wallet", "Company", new { companyID = company.ID })
                });

            navigation.MainSections.Add(companySection);
        }

        private static void CreateCitizenNavigation(NavigationViewModel navigation, Entities.Entity entity, UrlHelper urlHelper)
        {
            NavigationSectionViewModel citizenSection = new NavigationSectionViewModel()
            {
                Name = "Citizen"
            };

            var citizen = entity.Citizen;

            var hotel = citizen.HotelRooms.Where(r => r.Hotel.RegionID == citizen.RegionID).Select(r => r.Hotel).FirstOrDefault();


            citizenSection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Profile",
                Url = urlHelper.Action("View", "Citizen", new { citizenID = citizen.ID })
            });

            var house = citizen.GetCurrentlyLivingHouse();

            if (house != null)
            {
                citizenSection.Children.Add(new NavigationSectionViewModel()
                {
                    Name = "Current house",
                    Url = urlHelper.Action("View", "House", new { houseID = house.ID })
                });
            }

            if(house != null || citizen.Houses.Any())
                citizenSection.Children.Add(new NavigationSectionViewModel()
                {
                    Name = "Your houses",
                    Url = urlHelper.Action("Index", "House")
                });

            citizenSection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Messages",
                Url = urlHelper.Action("Index", "Message")
            });

            citizenSection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Inventory",
                Url = urlHelper.Action("Inventory", "Citizen", new { ID = citizen.ID })
            });

            citizenSection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Training",
                Url = urlHelper.Action("Index", "Training")
            });

            citizenSection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Travel",
                Url = urlHelper.Action("Travel", "Citizen")
            });



            if (citizen.CompanyEmployee != null)
            {
                var companyID = citizen.CompanyEmployee.CompanyID;

                citizenSection.Children.Add(new NavigationSectionViewModel()
                {
                    Name = "Job",
                    Url = urlHelper.Action("View", "Company", new { companyID = companyID })
                });
            }

            if (citizen.PartyMember != null)
            {
                var partyID = citizen.PartyMember.PartyID;

                citizenSection.Children.Add(new NavigationSectionViewModel()
                {
                    Name = "Party",
                    Url = urlHelper.Action("View", "Party", new { PartyID = partyID })
                });
            }
            else
            {
                int? countryID = citizen.Region.CountryID;
                if (countryID.HasValue)
                    citizenSection.Children.Add(new NavigationSectionViewModel()
                    {
                        Name = "Parties",
                        Url = urlHelper.Action("Parties", "Party", new { countryID = countryID })
                    });
            }

            citizenSection.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Wallet",
                Url = urlHelper.Action("Wallet", "Citizen")
            });

            if (hotel != null)
            {
                citizenSection.Children.Add(new NavigationSectionViewModel()
                {
                    Name = $"Staying at {hotel.Entity.Name}" ,
                    Url = urlHelper.Action("View", "Hotel", new { hotelID = hotel.ID })
                });
            }




            navigation.MainSections.Add(citizenSection);

            createBusinessSection(navigation, urlHelper, createCompanies: true, createOrganisations: true, createNewspapers: true);
        }

        private static void createBusinessSection(NavigationViewModel navigation, UrlHelper urlHelper, bool createCompanies, bool createOrganisations, bool createNewspapers)
        {
            var buss = new NavigationSectionViewModel()
            {
                Name = "Business"
            };
            if (createCompanies)
                buss.Children.Add(new NavigationSectionViewModel()
                {
                    Name = "Businesses",
                    Url = urlHelper.Action("Index", "Business")
                });

         /*   if (createOrganisations)
                buss.Children.Add(new NavigationSectionViewModel()
                {
                    Name = "Organisations",
                    Url = urlHelper.Action("Index", "Organisation")
                });*/

            if (createNewspapers)
                buss.Children.Add(new NavigationSectionViewModel()
                {
                    Name = "Newspapers",
                    Url = urlHelper.Action("Index", "Newspaper")
                });

            buss.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Monetary Market",
                Url = urlHelper.Action("Index", "MonetaryMarket")
            });

            navigation.MainSections.Add(buss);
        }
    }
}