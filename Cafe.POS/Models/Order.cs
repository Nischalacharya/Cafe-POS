using Cafe.POS.Models.Base;
using Cafe.POS.Models.Enums;

namespace Cafe.POS.Models;

public class Order : BaseEntity
{
    public Guid CustomerId { get; set; } = Guid.Empty;
    
    public Guid CoffeeId { get; set; } = Guid.Empty;
    
    public int CoffeeQuantity { get; set; }
    
    public decimal TotalPrice { get; set; }
    
    public PaymentMode PaymentMode { get; set; }
}