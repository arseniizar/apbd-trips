namespace TripApp.Core.Model;

// This entity is generated during the database scaffolding (reverse engineering) 
public class Trip
{
    public int IdTrip { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public int MaxPeople { get; set; }

    // Navigation property representing one-to-many relationship where one Trip can have multiple ClientTrips
    public virtual ICollection<ClientTrip> ClientTrips { get; set; } = new List<ClientTrip>();

    // Navigation property representing one-to-many relationship where one Trip can have multiple countries
    public virtual ICollection<Country> IdCountries { get; set; } = new List<Country>();
}
