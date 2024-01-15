namespace Cafe.POS.Models.Base;

/// <summary>
/// Represents a base entity with common properties shared by various entities in the Cafe POS application.
/// </summary>
public class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public bool IsActive { get; set; } = true;
    
    public Guid CreatedBy { get; set; }
    
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public Guid LastModifiedBy { get; set; }
    
    public DateTime LastModifiedOn { get; set; }
}