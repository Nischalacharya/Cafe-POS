namespace Cafe.POS.Models.DTOs;

public class OrderRecords
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public int TotalSales { get; set; }

    public string LastOrderedDate { get; set; }
}