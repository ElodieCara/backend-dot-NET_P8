using GpsUtil.Location;
using System.Collections.Concurrent;
using TripPricer;

namespace TourGuide.Users;

public class User
{

    private readonly object _userLock = new object(); // Verrou spécifique à chaque utilisateur
    public Guid UserId { get; }
    public string UserName { get; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
    public DateTime LatestLocationTimestamp { get; set; }
    public List<VisitedLocation> VisitedLocations { get; } = new List<VisitedLocation>();
    public ConcurrentBag<UserReward> UserRewards { get; } = new ConcurrentBag<UserReward>();

    public UserPreferences UserPreferences { get; set; } = new UserPreferences();
    public List<Provider> TripDeals { get; set; } = new List<Provider>();

    public object UserLock => _userLock; // Propriété publique pour accéder au verrou


    public User(Guid userId, string userName, string phoneNumber, string emailAddress)
    {
        UserId = userId;
        UserName = userName;
        PhoneNumber = phoneNumber;
        EmailAddress = emailAddress;
    }

    public void AddToVisitedLocations(VisitedLocation visitedLocation)
    {
        VisitedLocations.Add(visitedLocation);
    }

    public void ClearVisitedLocations()
    {
        VisitedLocations.Clear();
    }

    public void AddUserReward(UserReward userReward)
    {
        lock (_userLock) // Verrou pour s'assurer que la vérification et l'ajout sont sûrs
        {
            // Vérifie s'il n'y a pas déjà une récompense pour cette attraction
            if (!UserRewards.Any(r => r.Attraction.AttractionName == userReward.Attraction.AttractionName))
            {
                UserRewards.Add(userReward); // Ajoute la récompense s'il n'y a pas de duplicat
            }
        }
    }

    public VisitedLocation GetLastVisitedLocation()
    {
        return VisitedLocations[^1];
    }
}
