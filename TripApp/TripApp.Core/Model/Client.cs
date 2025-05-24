namespace TripApp.Core.Model;

// This entity is generated during the database scaffolding (reverse engineering)
public class Client
{
    public int IdClient { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Telephone { get; set; } = null!;

    public string Pesel { get; set; } = null!;
    
    // Represents one-to-many relationship between Client and ClientTrip. Such property is called "Navigation property"
    public virtual ICollection<ClientTrip> ClientTrips { get; set; } = new List<ClientTrip>();
}
