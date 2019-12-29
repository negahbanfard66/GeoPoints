using GeographicalPointsProject.Controllers.Base;
using GeographicalPointsProject.Model;
using GP.Lib.Base.DataLayer;
using GP.Lib.Base.Interfaces.Services;
using GP.Lib.Base.ViewModel;
using GP.Lib.Base.ViewModel.GeoPoint;
using GP.Lib.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeographicalPointsProject.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class GeoPointsController : BaseController
    {
        private readonly UserManager<DbUser> _userManager;
        private readonly IServiceGeoPoints _geoPointsService;
        private readonly IGeoLatLong _geoLatLong;
        private ICacheProvider _cache;
        public GeoPointsController(
            IServiceGeoPoints geoPointsService,
            UserManager<DbUser> userManager,
            ICacheProvider cache,
            IGeoLatLong geoLatLong,
    ILogger<GeoPointsController> logger) : base(logger)
        {
            _userManager = userManager;
            _geoPointsService = geoPointsService;
            _cache = cache;
            _geoLatLong = geoLatLong;
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> Post([FromBody]GeoPointsModel geoPointsModel)
        {
            try
            {
                var data = await _geoPointsService.AddAsync(new GP.Lib.Base.ViewModel.GeoPoint.VmGeoPointAdd()
                {
                    OriginLat = geoPointsModel.OriginLat,
                    OriginLon = geoPointsModel.OriginLon,
                    DestinationLat = geoPointsModel.DestinationLat,
                    DestinationLon = geoPointsModel.DestinationLon,
                    UserId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User))
                });

                return Ok(new { result = $"Distance : {distance(geoPointsModel)}" });
            }
            catch (Exception ex)
            {
                LogError(ex);
                var badRequestResponse = new ResponseViewModel<BadRequestObjectResult>
                {
                    IsAuthenticated = HttpContext.User.Identity.IsAuthenticated,
                    IsValid = false,
                    Message = "Something went wrong.",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return BadRequest(badRequestResponse);
            }
        }


        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetGeoPointsUserList()
        {
            try
            {
                List<VmGeoPointResult> cacheEntry;
                if (!_cache.Get<List<VmGeoPointResult>>(HttpContext.User.Identity.Name, out cacheEntry))
                {
                    var data = await _geoPointsService.FindGeoPointsByUserNameAsync(HttpContext.User.Identity.Name);
                    cacheEntry = data;
                    _cache.Set<List<VmGeoPointResult>>(HttpContext.User.Identity.Name, cacheEntry);
                }
                return Json(new { result = cacheEntry });
            }
            catch (Exception ex)
            {
                LogError(ex);
                var badRequestResponse = new ResponseViewModel<BadRequestObjectResult>
                {
                    IsAuthenticated = HttpContext.User.Identity.IsAuthenticated,
                    IsValid = false,
                    Message = "Something went wrong.",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return BadRequest(badRequestResponse);
            }
        }

        private string distance(GeoPointsModel geoPointsModel)
        {
            if ((geoPointsModel.OriginLat == geoPointsModel.OriginLon) && (geoPointsModel.DestinationLat == geoPointsModel.DestinationLon))
            {
                return "";
            }
            else
            {
                double theta = geoPointsModel.OriginLon - geoPointsModel.DestinationLon;
                double dist = Math.Sin(_geoLatLong.deg2rad(geoPointsModel.OriginLat)) * Math.Sin(_geoLatLong.deg2rad(geoPointsModel.DestinationLat)) + Math.Cos(_geoLatLong.deg2rad(geoPointsModel.OriginLat)) * Math.Cos(_geoLatLong.deg2rad(geoPointsModel.DestinationLat)) * Math.Cos(_geoLatLong.deg2rad(theta));
                dist = Math.Acos(dist);
                dist = _geoLatLong.rad2deg(dist);
                dist = dist * 60 * 1.1515;
                dist = dist * 1.609344;
                return $"{ dist } Kilometer";
            }
        }
    }
}