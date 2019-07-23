using Entities;
using Entities.enums;
using Entities.Repository;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;
using WebServices.BigParams;

namespace CreateSociatisWorld
{
    static class Countries
    {
        static ICountryService countryService;
        static IEntityRepository entityRepository;
        static IRegionService regionService;
        static IRegionRepository regionRepository;

        static Countries()
        {
            countryService = Ninject.Current.Get<ICountryService>();
            entityRepository = Ninject.Current.Get<IEntityRepository>();
            regionService = Ninject.Current.Get<IRegionService>();
            regionRepository = Ninject.Current.Get<IRegionRepository>();
        }

        public static void Create()
        {
            //CreateGermany();
            //CreateBenelux();
            //  regionService.ConnectRegions(regionRepository.GetRegion("Brandenburg"), regionRepository.GetRegion("Test Region"), 240);

            CreateUSA();
            CreateFrance();
            CreateAlbania();
            CreateCroatia();
            CreateUK();
            CreateSpain();
            CreatePoland();
            CreateSerbia();
        }

        private static void CreateUSA()
        {
            using (var trs = new System.Transactions.TransactionScope())
            {
                CreateCountryParameters param = new CreateCountryParameters()
                {
                    CountryName = "USA",
                    CurrencyName = "Dollar",
                    CurrencyShortName = "DOL",
                    CurrencySymbol = "$",
                    CurrencyID = (int)CurrencyTypeEnum.Dollar,
                    Color = "#155CFF"
                };

                Country country = null;
                CountryRepository repo = null;
                if (countryExists(param))
                {
                    Console.WriteLine("Spain exists!");
                    repo = new CountryRepository(new SociatisEntities());
                    country = repo.First(c => c.Entity.Name == param.CountryName);
                }
                else
                    country = countryService.CreateCountry(param);

                var california = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "California Coast"
                });

                var eastCoast = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "East Coast"
                });

                var washington = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Capital"
                });



                var texas = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Texas"
                });

                var pacific = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Pacific Northwest"
                });

                var mid = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Mid-America"
                });

                var alaskas = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Alaska"
                });

                if(washington != null)
                country.CapitalID = washington.ID;

                regionService.ConnectRegions(california, pacific, 1050);
                regionService.ConnectRegions(texas, mid, 1050);
                regionService.ConnectRegions(mid, pacific, 1750);
                regionService.ConnectRegions(california, mid, 1500);
                regionService.ConnectRegions(california, texas, 1800);
                regionService.ConnectRegions(eastCoast, mid, 1750);
                regionService.ConnectRegions(eastCoast, texas, 1850);
                regionService.ConnectRegions(eastCoast, washington, 200);

                regionService.ConnectRegions(pacific, alaskas, 3150);
                regionService.ConnectRegions(pacific, california, 4000);

                regionService.ConnectRegions(eastCoast, regionRepository.GetRegion("Castile-Leon"), 5900);
                regionService.ConnectRegions(washington, regionRepository.GetRegion("Castile-Leon"), 6000);

                regionService.ConnectRegions(eastCoast, regionRepository.GetRegion("Andalusia"), 6300);
                regionService.ConnectRegions(washington, regionRepository.GetRegion("Andalusia"), 6350);

                regionService.ConnectRegions(eastCoast, regionRepository.GetRegion("Normandy"), 5427);
                regionService.ConnectRegions(eastCoast, regionRepository.GetRegion("Wessex"), 5300);

                repo?.SaveChanges();
                repo?.Dispose();

                trs.Complete();
            }
        }

        private static void CreateFrance()
        {
            using (var trs = new System.Transactions.TransactionScope())
            {
                CreateCountryParameters param = new CreateCountryParameters()
                {
                    CountryName = "France",
                    CurrencyName = "French Franc",
                    CurrencyShortName = "FRF",
                    CurrencySymbol = "FR",
                    CurrencyID = (int)CurrencyTypeEnum.FrenchFranc,
                    Color = "#5BFF6E"
                };

                Country country = null;
                CountryRepository repo = null;
                if (countryExists(param))
                {
                    Console.WriteLine("Spain exists!");
                    repo = new CountryRepository(new SociatisEntities());
                    country = repo.First(c => c.Entity.Name == param.CountryName);
                }
                else
                    country = countryService.CreateCountry(param);

                var normandy = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Normandy"
                });

                var centre = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Centre-Val de Loire"
                });


                var alpes = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Alpes Provence"
                });

                var aqua = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Aquaitaine-Limousin-Poitou-Charentes"
                });
                
                if(centre != null)
                country.CapitalID = centre.ID;

                regionService.ConnectRegions(centre, normandy, 250);
                regionService.ConnectRegions(normandy, aqua, 500);
                regionService.ConnectRegions(aqua, alpes, 350);
                regionService.ConnectRegions(centre, alpes, 390);

                regionService.ConnectRegions(alpes, regionRepository.GetRegion("Istria"), 650);
                regionService.ConnectRegions(centre, regionRepository.GetRegion("Istria"), 680);

                regionService.ConnectRegions(centre, regionRepository.GetRegion("Great Poland"), 800);
                regionService.ConnectRegions(centre, regionRepository.GetRegion("Silesia"), 750);

                repo?.SaveChanges();
                repo?.Dispose();

                trs.Complete();
            }
        }

        private static void CreatePoland()
        {
            using (var trs = new System.Transactions.TransactionScope())
            {
                CreateCountryParameters param = new CreateCountryParameters()
                {
                    CountryName = "Poland",
                    CurrencyName = "Polish Złoty",
                    CurrencyShortName = "PLN",
                    CurrencySymbol = "PLN",
                    CurrencyID = (int)CurrencyTypeEnum.PLN,
                    Color = "#FF2100"
                };

                Country country = null;
                CountryRepository repo = null;
                if (countryExists(param))
                {
                    Console.WriteLine("Spain exists!");
                    repo = new CountryRepository(new SociatisEntities());
                    country = repo.First(c => c.Entity.Name == param.CountryName);
                }
                else
                    country = countryService.CreateCountry(param);

                var pomerania = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Pomerania"
                });

                var mazuria = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Mazuria"
                });


                var great = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Great Poland"
                });

                var mazovia = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Mazovia"
                });

                var silesia = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Silesia"
                });

                var sub  = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Subcarpathian"
                });

                if(mazovia != null)
                country.CapitalID = mazovia.ID;

                regionService.ConnectRegions(sub, mazovia, 250);
                regionService.ConnectRegions(mazovia, mazuria, 190);
                regionService.ConnectRegions(mazuria, pomerania, 250);
                regionService.ConnectRegions(great, pomerania, 200);
                regionService.ConnectRegions(great, mazovia, 250);
                regionService.ConnectRegions(silesia, sub, 330);
                regionService.ConnectRegions(great, silesia, 180);
                regionService.ConnectRegions(great, mazuria, 300);
                regionService.ConnectRegions(silesia, mazovia, 250);

                regionService.ConnectRegions(silesia, regionRepository.GetRegion("Continental Croatia"), 550);
                regionService.ConnectRegions(sub, regionRepository.GetRegion("Vojvodina"), 450);

                repo?.SaveChanges();
                repo?.Dispose();

                trs.Complete();
            }
        }

        private static void CreateSpain()
        {
            using (var trs = new System.Transactions.TransactionScope())
            {
                CreateCountryParameters param = new CreateCountryParameters()
                {
                    CountryName = "Spain",
                    CurrencyName = "Spanish Peseta",
                    CurrencyShortName = "Peseta",
                    CurrencySymbol = "ESP",
                    CurrencyID = (int)CurrencyTypeEnum.SpainPeseta,
                    Color = "#FAFF00"
                };

                Country country = null;
                CountryRepository repo = null;
                if (countryExists(param))
                {
                    Console.WriteLine("Spain exists!");
                    repo = new CountryRepository(new SociatisEntities());
                    country = repo.First(c => c.Entity.Name == param.CountryName);
                }
                else
                    country = countryService.CreateCountry(param);

                var catalonia = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Aragon and Catalonia"
                });

                var castile = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Castile-Leon"
                });


                var mancha = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Castile-La Mancha"
                });

                var valencia = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Valencia"
                });

                var andalusia = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Andalusia"
                });

                if(mancha != null)
                country.CapitalID = mancha.ID;

                regionService.ConnectRegions(castile, catalonia, 380);
                regionService.ConnectRegions(castile, mancha, 200);
                regionService.ConnectRegions(mancha, catalonia, 300);
                regionService.ConnectRegions(mancha, valencia, 270);
                regionService.ConnectRegions(mancha, andalusia, 350);
                regionService.ConnectRegions(valencia, andalusia, 440);
                regionService.ConnectRegions(valencia, catalonia, 280);

                regionService.ConnectRegions(catalonia, regionRepository.GetRegion("Aquaitaine-Limousin-Poitou-Charentes"), 140);

                repo?.SaveChanges();
                repo?.Dispose();

                trs.Complete();
            }
        }


        private static void CreateUK()
        {
            using (var trs = new System.Transactions.TransactionScope())
            {
                CreateCountryParameters param = new CreateCountryParameters()
                {
                    CountryName = "United Kingdom",
                    CurrencyName = "Pound Sterling",
                    CurrencyShortName = "Pound",
                    CurrencySymbol = "GBP",
                    CurrencyID = (int)CurrencyTypeEnum.PoundSterling,
                    Color = "#FAFF00"
                };

                Country country = null;
                CountryRepository repo = null;
                if (countryExists(param))
                {
                    Console.WriteLine("UK exists!");
                    repo = new CountryRepository(new SociatisEntities());
                    country = repo.First(c => c.Entity.Name == param.CountryName);
                }
                else
                    country = countryService.CreateCountry(param);

             

                var northumbria = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Northumbria"
                });


                var wessex = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Wessex"
                });

                var london = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "London"
                });

                var east = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "East Anglia"
                });

                if(london != null)
                country.CapitalID = london.ID;

                regionService.ConnectRegions(northumbria, wessex, 300);
                regionService.ConnectRegions(northumbria, london, 270);
                regionService.ConnectRegions(northumbria, east, 230);

                regionService.ConnectRegions(london, wessex, 170);
                regionService.ConnectRegions(london, east, 130);

                regionService.ConnectRegions(london, regionRepository.GetRegion("Normandy"), 250);
                regionService.ConnectRegions(wessex, regionRepository.GetRegion("Normandy"), 270);

                repo?.SaveChanges();
                repo?.Dispose();

                trs.Complete();
            }
        }

        private static void CreateCroatia()
        {
            using (var trs = new System.Transactions.TransactionScope())
            {
                CreateCountryParameters param = new CreateCountryParameters()
                {
                    CountryName = "Croatia",
                    CurrencyName = "Croatian Kuna",
                    CurrencyShortName = "Kuna",
                    CurrencySymbol = "HRK",
                    CurrencyID = (int)CurrencyTypeEnum.CroatianKuna,
                    Color = "#B330FF"
                };

                Country country = null;
                CountryRepository repo = null;
                if (countryExists(param))
                {
                    Console.WriteLine("Spain exists!");
                    repo = new CountryRepository(new SociatisEntities());
                    country = repo.First(c => c.Entity.Name == param.CountryName);
                }
                else
                    country = countryService.CreateCountry(param);
                

                var slavonia = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Slavonia"
                });

                var continental = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Continental Croatia"
                });


                var istria = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Istria"
                });

                var kvarner = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Kvarner"
                });

                var dalmatia = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Dalmatia"
                });

                if(continental != null)
                country.CapitalID = continental.ID;

                regionService.ConnectRegions(dalmatia, kvarner, 185);
                regionService.ConnectRegions(istria, kvarner, 130);
                regionService.ConnectRegions(continental, kvarner, 150);
                regionService.ConnectRegions(continental, slavonia, 170);

                regionService.ConnectRegions(dalmatia, regionRepository.GetRegion("Lezhë"), 250);

                repo?.SaveChanges();
                repo?.Dispose();

                trs.Complete();
            }
        }

        private static void CreateSerbia()
        {
            using (var trs = new System.Transactions.TransactionScope())
            {
                CreateCountryParameters param = new CreateCountryParameters()
                {
                    CountryName = "Serbia",
                    CurrencyName = "Serbian Dinar",
                    CurrencyShortName = "Dinar",
                    CurrencySymbol = "RSD",
                    CurrencyID = (int)CurrencyTypeEnum.SerbianDinar,
                    Color = "#FF4F9B"
                };

                Country country = null;
                CountryRepository repo = null;
                if (countryExists(param))
                {
                    Console.WriteLine("Spain exists!");
                    repo = new CountryRepository(new SociatisEntities());
                    country = repo.First(c => c.Entity.Name == param.CountryName);
                }
                else
                    country = countryService.CreateCountry(param);

                var vojvodina = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Vojvodina"
                });

                var belgrad = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Belgrade"
                });


                var sumadija = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Šumadija and Western Serbia"
                });

                var kosovo = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Kosovo and Metohija"
                });

                var south = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "South Eastern Serbia"
                });

                if(belgrad != null)
                country.CapitalID = belgrad.ID;

                regionService.ConnectRegions(vojvodina, belgrad, 125);
                regionService.ConnectRegions(vojvodina, south, 300);
                regionService.ConnectRegions(vojvodina, sumadija, 250);

                regionService.ConnectRegions(belgrad, sumadija, 125);
                regionService.ConnectRegions(belgrad, south, 100);


                regionService.ConnectRegions(kosovo, south, 150);
                regionService.ConnectRegions(sumadija, south, 140);

                regionService.ConnectRegions(kosovo, sumadija, 140);

                regionService.ConnectRegions(vojvodina, regionRepository.GetRegion("Slavonia"), 150);
                regionService.ConnectRegions(kosovo, regionRepository.GetRegion("Lezhë"), 180);

                repo?.SaveChanges();
                repo?.Dispose();

                trs.Complete();
            }
        }

        private static void CreateAlbania()
        {
            using (var trs = new System.Transactions.TransactionScope())
            {
                CreateCountryParameters param = new CreateCountryParameters()
                {
                    CountryName = "Albania",
                    CurrencyName = "Albanian Lek",
                    CurrencyShortName = "Lek",
                    CurrencySymbol = "ALL",
                    CurrencyID = (int)CurrencyTypeEnum.AlbanianLek,
                    Color = "#FFBF11"
                };

                Country country = null;
                CountryRepository repo = null;
                if (countryExists(param))
                {
                    Console.WriteLine("Spain exists!");
                    repo = new CountryRepository(new SociatisEntities());
                    country = repo.First(c => c.Entity.Name == param.CountryName);
                }
                else
                    country = countryService.CreateCountry(param);



                var Lezhe = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Lezhë"
                });

                var tirana = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Tirana"
                });

                var fier = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Fier"
                });

                if(tirana != null)
                country.CapitalID = tirana.ID;

                regionService.ConnectRegions(fier, tirana, 72);
                regionService.ConnectRegions(Lezhe, tirana, 58);


                repo?.SaveChanges();
                repo?.Dispose();

                trs.Complete();
            }
        }


        private static void CreateBenelux()
        {
            using (var trs = new System.Transactions.TransactionScope())
            {
                CreateCountryParameters param = new CreateCountryParameters()
                {
                    CountryName = "Benelux",
                    CurrencyName = "Euro",
                    CurrencyShortName = "EUR",
                    CurrencySymbol = "€",
                    CurrencyID = 4
                };

                if (countryExists(param))
                    return;

                var country = countryService.CreateCountry(param);

                var luxemburg = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Luxemburg"
                });

                var belgium = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Belgium"
                });

                var netherlands = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Netherlands"
                });

                regionService.ConnectRegions(luxemburg, belgium, 170);
                regionService.ConnectRegions(belgium, netherlands, 140);
                regionService.ConnectRegions(luxemburg, regionRepository.GetRegion("Western Germany"), 110);
                regionService.ConnectRegions(belgium, regionRepository.GetRegion("Western Germany"), 240);
                regionService.ConnectRegions(netherlands, regionRepository.GetRegion("Western Germany"), 300);
                regionService.ConnectRegions(netherlands, regionRepository.GetRegion("Lower Saxony"), 200);

                trs.Complete();
            }
        }

        private static void CreateGermany()
        {
            using (var trs = new System.Transactions.TransactionScope())
            {
                CreateCountryParameters param = new CreateCountryParameters()
                {
                    CountryName = "Germany",
                    CurrencyName = "Deutsche Mark",
                    CurrencyShortName = "DEM",
                    CurrencySymbol = "DM",
                    CurrencyID = 3
                };

                if (countryExists(param))
                    return;

                var country = countryService.CreateCountry(param);

                var northernGermany = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Northern Germany"
                });

                var brandenburg = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Brandenburg"
                });

                var southernGermany = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Southern Germany"
                });

                var lowerSaxony = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Lower Saxony"
                });

                var saxony = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Saxony"
                });

                var westernGermany = regionService.CreateRegion(new CreateRegionParameters()
                {
                    CanSpawn = true,
                    CountryID = country.ID,
                    Name = "Western Germany"
                });

                regionService.ConnectRegions(saxony, brandenburg, 190);
                regionService.ConnectRegions(northernGermany, brandenburg, 230);
                regionService.ConnectRegions(northernGermany, lowerSaxony, 150);
                regionService.ConnectRegions(saxony, lowerSaxony, 250);
                regionService.ConnectRegions(saxony, westernGermany, 300);
                regionService.ConnectRegions(lowerSaxony, westernGermany, 270);
                regionService.ConnectRegions(southernGermany, westernGermany, 330);
                regionService.ConnectRegions(saxony, southernGermany, 260);

                trs.Complete();
            }
        }

        private static bool countryExists(CreateCountryParameters param)
        {
            return entityRepository.Any(e => e.Name == param.CountryName);
        }
    }
}
