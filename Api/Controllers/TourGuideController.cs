using GpsUtil.Location;
using Microsoft.AspNetCore.Mvc;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TripPricer;

namespace TourGuide.Controllers;

[ApiController]
[Route("[controller]")]
public class TourGuideController : ControllerBase
{
    private readonly ITourGuideService _tourGuideService;

    public TourGuideController(ITourGuideService tourGuideService)
    {
        _tourGuideService = tourGuideService;
    }

    [HttpGet("getLocation")]
    public ActionResult<VisitedLocation> GetLocation([FromQuery] string userName)
    {
        var location = _tourGuideService.GetUserLocation(GetUser(userName));
        return Ok(location);
    }


    [HttpGet("getNearbyAttractions")]
    public ActionResult<List<object>> GetNearbyAttractions([FromQuery] string userName)
    {
        var user = GetUser(userName);
        var visitedLocation = _tourGuideService.GetUserLocation(user);

        // Obtenez les 5 attractions les plus proches
        var attractions = _tourGuideService.GetNearByAttractions(visitedLocation)
            .Select(attraction => new
            {
                AttractionName = attraction.AttractionName,
                AttractionLocation = new { attraction.Latitude, attraction.Longitude },
                UserLocation = new { visitedLocation.Location.Latitude, visitedLocation.Location.Longitude },
                Distance = _tourGuideService.GetDistance(attraction, visitedLocation.Location),
                RewardPoints = _tourGuideService.GetRewardPoints(attraction, user)
            })
            .ToList();

        return Ok(attractions);
    }

    [HttpGet("getRewards")]
    public ActionResult<List<UserReward>> GetRewards([FromQuery] string userName)
    {
        var rewards = _tourGuideService.GetUserRewards(GetUser(userName));
        return Ok(rewards);
    }

    [HttpGet("getTripDeals")]
    public ActionResult<List<Provider>> GetTripDeals([FromQuery] string userName)
    {
        var deals = _tourGuideService.GetTripDeals(GetUser(userName));
        return Ok(deals);
    }

    private User GetUser(string userName)
    {
        return _tourGuideService.GetUser(userName);
    }
}
