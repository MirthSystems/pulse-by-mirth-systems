using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Domain.Entities;

[Table("special_items")]
public class SpecialItemEntity
{
    [Column("id")]
    public long Id { get; set; }

    [Column("specials_menu_id")]
    public long SpecialsMenuId { get; set; }
    [Column("special_item_category_id")]
    public int SpecialItemCategoryId { get; set; }

    [Column("name")]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("is_active")]
    [DefaultValue(true)]
    public bool IsActive { get; set; }

    public SpecialsMenuEntity SpecialsMenu { get; set; } = null!;
    public SpecialItemCategoryEntity Category { get; set; } = null!;
}
