﻿namespace Application.Domain.Entities;

public class VenueCategory
{
    #region Identity and primary fields
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    #endregion

    #region Metadata properties
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
    #endregion

    #region Navigation properties
    public List<Venue> Venues { get; set; } = new List<Venue>();
    #endregion
}
