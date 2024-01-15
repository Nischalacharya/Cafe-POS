using Cafe.POS.Models.DTOs;

namespace Cafe.POS.PDF;
// <summary>
/// Represents a PDF document containing information about coffee and add-in order records.
/// </summary>
public class PDF
{
    /// <summary>
    /// Gets or sets the frequency of the PDF document (e.g., daily, weekly).
    /// </summary>
    public string Frequency { get; set; }
    
    public string Title { get; set; }
    
    public decimal TotalRevenue { get; set; }

    public string FileName { get; set; }
    
    public string UserName { get; set; }

    public string Role { get; set; }
    
    public IEnumerable<OrderRecords> CoffeeRecords { get; set; }

    public IEnumerable<OrderRecords> AddInRecords { get; set; }
}