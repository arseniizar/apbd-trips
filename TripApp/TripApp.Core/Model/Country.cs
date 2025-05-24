namespace TripApp.Core.Model;

// This entity is generated during the database scaffolding (reverse engineering)
public class Country
{
    public int IdCountry { get; set; }

    public string Name { get; set; } = null!;

    // Navigation property that represents one-to-many relationship, where one country can be assigned to many trips
    public virtual ICollection<Trip> IdTrips { get; set; } = new List<Trip>();
}
