using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NodaTime;

namespace Application.Domain.Entities;

[Table("specials")]
public class SpecialEntity
{
    [Column("id")]
    public long Id { get; set; }

    [Column("venue_id")]
    public long VenueId { get; set; }

    [Column("special_category_id")]
    public int SpecialCategoryId { get; set; }

    [Column("title")]
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = null!;

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("start_date")]
    public LocalDate StartDate { get; set; }      // First occurrence date

    [Column("start_time")]
    public LocalTime StartTime { get; set; }      // Daily start time

    [Column("end_time")]
    public LocalTime? EndTime { get; set; }       // Daily end time (optional)

    [Column("end_date")]
    public LocalDate? EndDate { get; set; }       // Last occurrence date (for recurring/limited time)

    [Column("is_recurring")]
    public bool IsRecurring { get; set; }

    [Column("cron_schedule")]
    [MaxLength(50)]
    public string? CronSchedule { get; set; }     // "0 17 * * 1-5" for weekdays at 5 PM

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    public VenueEntity Venue { get; set; } = null!;
    public SpecialCategoryEntity Category { get; set; } = null!;
}
