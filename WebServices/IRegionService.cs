using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.BigParams;
using WebServices.PathFinding;
using WebServices.structs;

namespace WebServices
{
    public interface IRegionService
    {
        Region CreateRegion(CreateRegionParameters param);
        Passage ConnectRegions(Region firstRegion, Region secondRegion, int distance);
        bool DoesPassageExist(Region firstRegion, Region secondRegion);
        Path GetPathBetweenRegions(Region startRegion, Region endRegion);
        Path GetPathBetweenRegions(Region startRegion, Region endRegion, IPathFindingRegionSelector regionSelector);
        Path GetPathBetweenRegions(Region startRegion, Region endRegion, IPathFindingRegionSelector regionSelector, IPathFindingCostCalculator costCalculator);
        void ProcessDayChange(int newDay);
        double CalculateDistanceWithDevelopement(double distance, double developement);

        /// <summary>
        /// Returns regions where citizen can spawn for given country.
        /// </summary>
        IQueryable<Region> GetBirthableRegions(int countryID);
    }
}
