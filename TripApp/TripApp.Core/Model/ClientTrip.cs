namespace TripApp.Core.Model;

// This entity is generated during the database scaffolding (reverse engineering)
public class ClientTrip
{
    public int IdClient { get; set; }

    public int IdTrip { get; set; }

    public DateTime RegisteredAt { get; set; }

    public DateTime? PaymentDate { get; set; }

    // Navigation property that represents many-to-one relationship. Where we have many client trips assigned to one Client
    public virtual Client IdClientNavigation { get; set; } = null!;

    // Navigation property that represents many-to-one relationship. Where we have many client trips assigned to one Trip
    public virtual Trip IdTripNavigation { get; set; } = null!;
}
