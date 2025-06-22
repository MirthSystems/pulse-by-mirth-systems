using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using NodaTime;

namespace Application.Domain.Entities;

[Table("business_hours")]
public class BusinessHours
{
    [Column("id")]
    public long Id { get; set; }

    [Column("venue_id")]
    [Required]
    public long VenueId { get; set; }

    [Column("day_of_week_id")]
    [Required]
    public int DayOfWeekId { get; set; }

    [Column("open_time")]
    public LocalTime? OpenTime { get; set; }

    [Column("close_time")]
    public LocalTime? CloseTime { get; set; }

    [Column("is_closed")]
    [DefaultValue(false)]
    public bool IsClosed { get; set; }

    public Venue Venue { get; set; } = null!;
    public DayOfWeek DayOfWeek { get; set; } = null!;
}
