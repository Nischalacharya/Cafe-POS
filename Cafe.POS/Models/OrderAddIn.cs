using Cafe.POS.Models.Base;

namespace Cafe.POS.Models;

public class OrderAddIn : BaseEntity
{
    public Guid OrderId { get; set; } = Guid.Empty;
    
    public Guid AddInId { get; set; } = Guid.Empty;
    
    public int AddInQuantity { get; set; }
}