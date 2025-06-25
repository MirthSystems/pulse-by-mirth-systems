using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Application.Domain.Enums;

namespace Application.Domain.Entities;

[Table("months_of_year")]
public class MonthOfYearEntity
{
    [Column("id")]
    public Months Id { get; set; }

    [Column("name")]
    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = null!;

    [Column("short_name")]
    [Required]
    [MaxLength(3)]
    public string ShortName { get; set; } = null!;

    [Column("iso_number")]
    public int IsoNumber { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; }
}
