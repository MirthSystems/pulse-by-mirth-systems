using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NodaTime;

namespace Application.Domain.Entities;

public class Venue
{
    #region Identity and primary fields
    public long Id { get; set; }
    public int CategoryId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    #endregion

    #region Contact information
    public string? PhoneNumber { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public string? ProfileImage { get; set; }
    #endregion

    #region Address fields
    public required string StreetAddress { get; set; }
    public string? SecondaryAddress { get; set; }
    public required string Locality { get; set; }
    public required string Region { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }
    public Point? Location { get; set; }
    #endregion

    #region Audit information
    public bool IsActive { get; set; } = true;
    #endregion

    #region Navigation properties
    public VenueCategory? Category { get; set; }
    public List<BusinessHours> BusinessHours { get; set; } = new List<BusinessHours>();

    #endregion
}
