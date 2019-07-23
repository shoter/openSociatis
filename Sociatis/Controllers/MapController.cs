using Entities;
using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Code;
using Sociatis.Code.Filters;
using Sociatis.Models.Map;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebUtils.Attributes;

namespace Sociatis.Controllers
{
    [DayChangeAuthorize]
    public class MapController : ControllerBase
    {
        private readonly IRegionRepository regionRepository;
        private readonly IPolygonRepository polygonRepository;
        private readonly IResourceRepository resourceRepository;

        public MapController(IPopupService popupService, IRegionRepository regionRepository, IPolygonRepository polygonRepository,
            IResourceRepository resourceRepository) : base(popupService)
        {
            this.regionRepository = regionRepository;
            this.polygonRepository = polygonRepository;
            this.resourceRepository = resourceRepository;
        }

        [SociatisAuthorize(Entities.enums.PlayerTypeEnum.Player)]
        public ActionResult Test()
        {
            return View();
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Map/Political")]
        public ActionResult NormalMode()
        {
            var regions = regionRepository
                .Where(r => r.PolygonID != null)
                .Include(r => r.Polygon)
                .Include("Polygon.PolygonPoints")
                .ToList();

            var vm = new MapNormalViewModel(regions);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Map/{resource:ResourceTypeEnum}")]
        public ActionResult ResourceMode(ResourceTypeEnum resource)
        {
            var colors = Colors.ResourceColors[resource];

            var resources = resourceRepository.Where(r => r.ResourceTypeID == (int)resource).ToDictionary(r => r.RegionID, r => r.ResourceQuality);

            foreach(var region in Persistent.Regions.GetAll())
                if (resources.ContainsKey(region.ID) == false)
                    resources.Add(region.ID, 0);


            var regions = regionRepository
                .Where(r => r.PolygonID != null)
               .Include(r => r.Polygon)
               .Include("Polygon.PolygonPoints")
               .ToList();

            var vm = new MapResourceViewModel(regions, resources, resource);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [AjaxOnly]
        public JsonResult GetRegionSummary(int regionID)
        {
            var region = regionRepository.GetById(regionID);

            var vm = new RegionMapSummaryViewModel(region);

            return Json(vm);
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Map/Infrastructure")]
        public ActionResult DevelopementMode()
        {
            var regions = regionRepository
               .Where(r => r.PolygonID != null)
               .Include(r => r.Polygon)
               .Include("Polygon.PolygonPoints")
               .ToList();

            var vm = new MapDevelopementViewModel(regions);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.SuperAdmin)]
        [HttpGet]
        public ActionResult Edit()
        {
            var regions = regionRepository.Query
                .Where(r => r.PolygonID != null)
                .Include(r => r.Polygon)
                .Include("Polygon.PolygonPoints")
                .ToList();

            var vm = new MapEditViewModel(regions);

            return View(vm);
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.SuperAdmin)]
        public ActionResult Edit(List<MapRegionViewModel> regions)
        {
            regions = regions.Where(r => r.RegionID.HasValue).ToList();

            var regionsID = regions.Select(r => r.RegionID.Value);

           // var existingPolys = regions.Where(r => r.PolygonID.HasValue);
           // var newPolys = regions.Where(r => r.PolygonID.HasValue == false);

            var dbRegions = regionRepository.Where(r => regionsID.Contains(r.ID)).ToList();

            foreach (var region in regions)
            {
                var dbReg = dbRegions.First(r => r.ID == region.RegionID);

                var polygon = new Polygon()
                {
                    Color = region.Color,
                    FillColor = region.FillColor,
                    FillOpacity = (decimal)region.FillOpacity,
                    Opacity = (decimal)region.Opacity,
                    Weight = (decimal)region.Weight
                };

                for (int i = 0; i < region.Points.Count; ++i)
                {
                    var p = region.Points[i];
                    polygon.PolygonPoints.Add(new PolygonPoint()
                    {
                        Latitude = p.Latitude,
                        Longitude = p.Longitude,
                        PointNumber = i
                    });
                }
                var oldPolygon = dbReg.Polygon;
                dbReg.Polygon = polygon;

                if (oldPolygon != null)
                {
                    polygonRepository.Remove(oldPolygon);
                }

                for(int i = 0;i < region.Fertility.Count; ++ i)
                {
                    var res = region.Fertility[i];
                    var dbRes = dbReg.Resources.FirstOrDefault(x => x.ID == i);
                    if (dbRes != null)
                    {
                        if (res == 0)
                            regionRepository.RemoveSpecific(dbRes);
                        else
                            dbRes.ResourceQuality = res;
                    }
                    else
                    {
                        resourceRepository.Add(new Resource()
                        {
                            RegionID = dbReg.ID,
                            ResourceTypeID = i,
                            ResourceQuality = res
                        });
                    }
                }

            }


            regionRepository.SaveChanges();

            return RedirectToAction(nameof(this.Edit));
        }
        public JsonResult SaveSingleRegion(MapRegionViewModel region)
        {
            try
            {
                if (region.RegionID.HasValue == false)
                    return JsonSuccess("Nothing to save!");


                var dbReg = regionRepository.First(r => r.ID == region.RegionID);

                var polygon = new Polygon()
                {
                    Color = region.Color,
                    FillColor = region.FillColor,
                    FillOpacity = (decimal)region.FillOpacity,
                    Opacity = (decimal)region.Opacity,
                    Weight = (decimal)region.Weight
                };

                for (int i = 0; i < region.Points.Count; ++i)
                {
                    var p = region.Points[i];
                    polygon.PolygonPoints.Add(new PolygonPoint()
                    {
                        Latitude = p.Latitude,
                        Longitude = p.Longitude,
                        PointNumber = i
                    });
                }
                var oldPolygon = dbReg.Polygon;
                dbReg.Polygon = polygon;

                if (oldPolygon != null)
                {
                    polygonRepository.Remove(oldPolygon);
                }

                List<int> resID = Enum.GetValues(typeof(ResourceTypeEnum)).Cast<int>().ToList();

                for (int i = 0; i < region.Fertility.Count; ++i)
                {
                    var res = region.Fertility[i];
                    var dbRes = dbReg.Resources.FirstOrDefault(x => x.ResourceTypeID == resID[i]);
                    if (dbRes != null)
                    {
                        if (res == 0)
                            regionRepository.RemoveSpecific(dbRes);
                        else
                            dbRes.ResourceQuality = res;
                    }
                    else
                    {
                        resourceRepository.Add(new Resource()
                        {
                            RegionID = dbReg.ID,
                            ResourceTypeID = resID[i],
                            ResourceQuality = res
                        });
                    }
                }
                resourceRepository.SaveChanges();
                regionRepository.SaveChanges();
                return JsonSuccess("Done");
            }
            catch (Exception e)
            {
                return UndefinedJsonError(e);
            }
        }

    }
}