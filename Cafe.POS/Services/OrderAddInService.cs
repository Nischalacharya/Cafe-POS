using Cafe.POS.Models;

namespace Cafe.POS.Services;

public class OrderAddInService: GenericService<OrderAddIn>
{
    private static readonly string AppDataDirectoryPath = UtilityService.GetAppDirectoryPath();
    private static readonly string AppOrderAddInsFilePath = UtilityService.GetAppOrderAddInsFilePath();
    
    public static List<OrderAddIn> Create(List<OrderAddIn> orderAddInModels)
    {
        var orderAddIns = GetAll(AppOrderAddInsFilePath);

        orderAddIns.AddRange(orderAddInModels);

        SaveAll(orderAddIns, AppDataDirectoryPath, AppOrderAddInsFilePath);

        return orderAddIns;
    }
}