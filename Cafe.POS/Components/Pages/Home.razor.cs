using Cafe.POS.Services;

namespace Cafe.POS.Components.Pages;

public partial class Home
{
    private int _customerCount { get; set; }
    
    private int _orderCount { get; set; }
    
    private decimal _revenueGenerated { get; set; }
    
    private decimal _todayRevenueGenerated { get; set; }
    
    private readonly string _customersPath = UtilityService.GetAppCustomersFilePath();
    
    private readonly string _ordersPath = UtilityService.GetAppOrdersFilePath();

    protected override void OnInitialized()
    {
        _customerCount = CustomerService.GetAll(_customersPath).Count;
        _orderCount = OrderService.GetAll(_ordersPath).Count;
        _revenueGenerated = OrderService.GetAll(_ordersPath).Sum(x => x.TotalPrice);
        _todayRevenueGenerated = OrderService.GetAll(_ordersPath).Where(x => x.CreatedOn.Date == DateTime.Now.Date)
            .Sum(x => x.TotalPrice);
    }
}