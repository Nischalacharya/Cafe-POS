using Cafe.POS.Models;

namespace Cafe.POS.Services;

/// <summary>
/// Service class for managing orders in the Cafe POS application.
/// </summary>
public class OrderService : GenericService<Order>
{
    private static readonly string AppDataDirectoryPath = UtilityService.GetAppDirectoryPath();
    private static readonly string AppOrdersFilePath = UtilityService.GetAppOrdersFilePath();
    
    public static List<Order> Create(Order order)
    {
        var orders = GetAll(AppOrdersFilePath);

        orders.Add(order);

        SaveAll(orders, AppDataDirectoryPath, AppOrdersFilePath);

        return orders;
    }
}