using Common.StringDecorators;
using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.BigParams;
using WebServices.enums;
using WebServices.PathFinding;
using WebServices.structs;
using WebUtils.Logging;

namespace WebServices
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository regionRepository;
        private readonly ICountryRepository countryRepository;
        private readonly IWarningService warningService;
        private readonly ITransactionsService transactionService;
        public RegionService(IRegionRepository regionRepository, ICountryRepository countryRepository, IWarningService warningService, ITransactionsService transactionService)
        {
            this.regionRepository = regionRepository;
            this.countryRepository = countryRepository;
            this.warningService = warningService;
            this.transactionService = transactionService;
        }


        public Region CreateRegion(CreateRegionParameters param)
        {
            if (regionRepository.Any(r => r.Name == param.Name && r.CountryCoreID == param.CountryID))
            {
                Console.WriteLine($"Region {param.Name} exists!");
                return regionRepository.Single(r => r.Name == param.Name && r.CountryCoreID == param.CountryID);
            }

            var region = new Region()
            {
                CanSpawn = param.CanSpawn,
                CountryID = param.CountryID,
                Name = param.Name,
                CountryCoreID = param.CountryID
            };

            regionRepository.Add(region);
            regionRepository.SaveChanges();

            Console.WriteLine($"Region {param.Name} created!");

            return region;
        }

        public Passage ConnectRegions(Region firstRegion, Region secondRegion, int distance)
        {
            if (firstRegion == null || secondRegion == null)
                return null;
            var passage = new Passage()
            {
                FirstRegionID = firstRegion.ID,
                SecondRegionID = secondRegion.ID,
                Distance = distance
            };

            //this will be not be called frequently. We will add some new checks for this.

            if (DoesPassageExist(firstRegion, secondRegion))
                return null; //this is acceptable. 

            regionRepository.AddPassage(passage);
            regionRepository.SaveChanges();

            Console.WriteLine($"Regions {firstRegion.Name} and {secondRegion.Name} connected!");

            return passage;
        }

        public bool DoesPassageExist(Region firstRegion, Region secondRegion)
        {
            return regionRepository.GetPassage(firstRegion.ID, secondRegion.ID) != null;
        }


        public Path GetPathBetweenRegions(Region startRegion, Region endRegion)
        {
            return GetPathBetweenRegions(startRegion, endRegion, new DefaultRegionSelector(), new DefaultPassageCostCalculator(this));
        }
        public Path GetPathBetweenRegions(Region startRegion, Region endRegion, IPathFindingRegionSelector regionSelector)
        {
            return GetPathBetweenRegions(startRegion, endRegion, regionSelector, new DefaultPassageCostCalculator(this));
        }

        public Path GetPathBetweenRegions(Region startRegion, Region endRegion, IPathFindingRegionSelector regionSelector, IPathFindingCostCalculator costCalculator)
        {
            List<Region> closedSet = new List<Region>();
            List<Region> openSet = new List<Region> { startRegion };
            Dictionary<int, Region> cameFrom = new Dictionary<int, Region>(); //key - regionID, value - region from which it came
            Dictionary<int, double> gScore = new Dictionary<int, double>();
            Dictionary<int, double> fScore = new Dictionary<int, double>();

            foreach (var region in Persistent.Regions.GetAll())
            {
                gScore.Add(region.ID, double.MaxValue);
                fScore.Add(region.ID, double.MaxValue);
            }

            gScore[startRegion.ID] = 0;
            fScore[startRegion.ID] = 0;// EstimateDistance(startRegion, endRegion);

            while(openSet.Count != 0)
            {
                var currentID = GetKeyWithMinimumValueInOpenSet(fScore, openSet);
                var current = Persistent.Regions.First(r => r.ID == currentID);

                if (currentID == endRegion.ID)
                {
                    return reconstructPath(cameFrom, startRegion, current, gScore[currentID]);
                }


                RemoveFrom(openSet, current);
                closedSet.Add(current);

                var neighbours = current.GetNeighbours();

                List<int> enemiesIDs = new List<int>();
                List<int> embargoIDs = new List<int>();



                foreach (var neighbour in neighbours)
                {
                    if (closedSet.Contains(neighbour.Region))
                        continue;
                    if (regionSelector.IsPassableRegion(neighbour) == false)
                    {
                        closedSet.Add(neighbour.Region);
                        continue;
                    }  
                    
                    
                    double tentative_gScore = gScore[current.ID] + costCalculator.CalculatePassageCost(neighbour.Passage);

                    if (openSet.Contains(neighbour.Region) == false)
                        openSet.Add(neighbour.Region);
                    else if (tentative_gScore >= gScore[neighbour.Region.ID])
                        continue;

                    cameFrom[neighbour.Region.ID] = current;
                    gScore[neighbour.Region.ID] = tentative_gScore;
                    fScore[neighbour.Region.ID] = tentative_gScore;
                }
            }

            return null; //No path
        }

        private void RemoveFrom(ICollection<Region> collection, Region region)
        {
            var collRegion = collection.First(r => r.ID == region.ID);
            collection.Remove(collRegion);
        }

        private Path reconstructPath(Dictionary<int, Region> cameFrom, Region startRegion, Region currentRegion, double gScore)
        {
            Path path = new Path()
            {
                StartRegion = startRegion,
                EndRegion = currentRegion,
                Distance = gScore
            };
#if DEBUG
            ServerFileLogger logger = new ServerFileLogger("~/logs/travel.txt", StringDecorators.TimeStamp);
            logger.LogLine("----");
            logger.LogLine("Came from - {0}", currentRegion.Name);
            while (cameFrom.ContainsKey(currentRegion.ID))
            { 
                currentRegion = cameFrom[currentRegion.ID];
                logger.LogLine("Came from - {0}", currentRegion.Name);
            }
#endif

            return path;
        }

        private static int GetKeyWithMinimumValueInOpenSet(Dictionary<int, double> dict, List<Region> openSet)
        {
            var openSetID = openSet.Select(r => r.ID);
            return dict.Where(x => openSetID.Contains(x.Key)).OrderBy(x => x.Value).First().Key;
        }

        private static double EstimateDistance(Region region, Region otherRegion)
        {
            return 1;
        }

        public void ProcessDayChange(int newDay)
        {
            processDevelopementChanges();
        }

        private void processDevelopementChanges()
        {
            var regionsInf = regionRepository
               .Select(r => new {
                   ID = r.ID,
                   Developement = r.Development,
                   CitizenCount = r.Citizens.Count
               }).ToList();

            using (var repo = new SingleChangeRepository())
            {
                foreach (var region in regionsInf)
                {
                    double newDev = CalculateNewDevelopementValue((double)region.Developement);
                    double diffrence = newDev - (double)region.Developement;

                    if (region.CitizenCount < 50)
                    {
                        diffrence *= ((region.CitizenCount + 1) / 50.0);
                        newDev = (double)region.Developement + diffrence;
                    }
                    repo.UpdateSingleField<Region, decimal>(x => x.Development, x => x.ID = region.ID, (decimal)newDev);
                }

            }
        }

        private TransactionResult makeUpkeepTransaction(Country country, string regionName, int regionID, double cost)
        {

            var payingEntity = country.Entity;

            var money = new Money()
            {
                Amount = (decimal)cost,
                Currency = Persistent.Currencies.GetById((int)CurrencyTypeEnum.Gold)
            };

            var transaction = new Transaction()
            {
                Arg1 = "Region upkeep",
                Arg2 = string.Format("{0}({1}) paid {2} region({3}) upkeep", payingEntity.Name, payingEntity.EntityID, regionName, regionID),
                Money = money,
                SourceEntityID = payingEntity.EntityID,
                TransactionType = TransactionTypeEnum.RegionUpkeep
            };

            return transactionService.MakeTransaction(transaction, false);
        }

        public double CalculateNewDevelopementValue(double infValue)
        {
            infValue = infValue - Math.Pow(Math.Sin((infValue / 5) * Math.PI / 2), 1.5) * 0.22;


            return Math.Max(Math.Min(Math.Round(infValue, 3), 5.0), 0.0);
        }

        public double CalculateDistanceWithDevelopement(double distance, double developement)
        {
            developement = Math.Max(0.1, developement);

            if (developement < 1)
                developement = Math.Pow(developement, 0.3);
            else
                developement = Math.Pow(developement, 0.4);

            return distance / developement;
        }

        public IQueryable<Region> GetBirthableRegions(int countryID)
        {
            var spawnableRegions = regionRepository.GetSpawnableRegionsForCountry(countryID);

            if (spawnableRegions.Any() == false)
                return regionRepository.GetCoreRegionsForCountry(countryID);

            return spawnableRegions;
                
        }
    }
}
