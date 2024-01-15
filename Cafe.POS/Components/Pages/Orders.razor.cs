using Cafe.POS.Models;
using Cafe.POS.Models.Base;
using Cafe.POS.Services;
using Microsoft.AspNetCore.Components;

namespace Cafe.POS.Components.Pages;

public partial class Orders
{
    [CascadingParameter] 
    private GlobalState GlobalState { get; set; }

    private List<Order> OrdersList { get; set; }

    private List<OrderAddIn> OrderAddIns { get; set; }

    private List<Customer> Customers { get; set; }
    
    private List<Coffee> Coffees { get; set; }
    
    private List<AddIn> AddIns { get; set; }
    
    private bool _showAddOrderDialog { get; set; }

    private bool? _isRegularCustomer { get; set; }

    private bool? _isComplimentaryCoffee { get; set; }
    
    private string _addOrderErrorMessage { get; set; }
    
    private string _dialogTitle { get; set; }
    
    private string _dialogOkLabel { get; set; }
    
    private decimal _actualAmount;
    
    private decimal _payableAmount;
    
    private Order? _orderModel { get; set; }

    private List<OrderAddIn> _orderAddInModel =
    [
        new OrderAddIn()
        {
            AddInId = Guid.Empty,
            AddInQuantity = 0
        }
    ];
    
    private readonly string _ordersPath = UtilityService.GetAppOrdersFilePath();

    private readonly string _orderAddInsPath = UtilityService.GetAppOrderAddInsFilePath();

    private readonly string _customersPath = UtilityService.GetAppCustomersFilePath();
    
    private readonly string _coffeesPath = UtilityService.GetAppCoffeesFilePath();
    
    private readonly string _addInsPath = UtilityService.GetAppAddInsFilePath();

    protected override void OnInitialized()
    {
        OrdersList = OrderService.GetAll(_ordersPath);
        Customers = CustomerService.GetAll(_customersPath);
        Coffees = CoffeeService.GetAll(_coffeesPath).Where(x => x.IsActive).ToList();
        AddIns = AddInService.GetAll(_addInsPath).Where(x => x.IsActive).ToList();
        OrderAddIns = OrderAddInService.GetAll(_orderAddInsPath);
    }

    private void OpenAddUserDialog()
    {
        _dialogTitle = "Add a new order";

        _dialogOkLabel = "Add";

        _orderModel = new Order();

        _orderAddInModel = 
        [
            new OrderAddIn()
            {
                AddInId = Guid.Empty,
                AddInQuantity = 0
            }
        ];
        
        _showAddOrderDialog = true;
    }

    private void OnAddOrderDialogClose(bool isClosed)
    {
        if (isClosed)
        {
            _showAddOrderDialog = false;
        }
        else
        {
            try
            {
                _addOrderErrorMessage = "";

                var order = new Order()
                {
                    CoffeeId = _orderModel.CoffeeId,
                    CustomerId = _orderModel.CustomerId,
                    CoffeeQuantity = _orderModel.CoffeeQuantity,
                    TotalPrice = _payableAmount,
                    PaymentMode = _orderModel.PaymentMode,
                    IsActive = true,
                    CreatedBy = GlobalState.User.Id,
                };

                if(order.CustomerId == Guid.Empty)
                {
                    throw new Exception("Please select a customer before proceeding your order transaction.");
                }
                
                if(order.CoffeeId == Guid.Empty || order.CoffeeQuantity == 0)
                {
                    throw new Exception("Please select a coffee and its respective quantity to be ordered.");
                }
                
                var orderAddIns = _orderAddInModel.Select(x => new OrderAddIn()
                {
                    OrderId = order.Id,
                    AddInId = x.AddInId,
                    AddInQuantity = x.AddInQuantity,
                    IsActive = true,
                    CreatedBy = GlobalState.User.Id,
                }).ToList();
                
                if(orderAddIns.Any(x => x.AddInId == Guid.Empty || x.AddInQuantity == 0))
                {
                    throw new Exception("Please select an add-in and its respective quantity to be ordered.");
                }
                
                OrdersList = OrderService.Create(order);

                OrderAddIns = OrderAddInService.Create(orderAddIns);

                CustomerService.UpdateOrderCount(_orderModel.CustomerId);
                
                _showAddOrderDialog = false;
            }
            catch (Exception e)
            {
                _addOrderErrorMessage = e.Message;

                Console.WriteLine(e.Message);
            }
        }
    }

    private void OnCustomerSelection(ChangeEventArgs e)
    {
        var customerId = Guid.Parse(e.Value.ToString());

        var isRegularCustomer = CustomerService.IsRegularCustomer(customerId);

        var isCoffeeComplimentary = CustomerService.IsAvailableForComplimentaryCoffee(customerId);
        
        _isRegularCustomer = isRegularCustomer;
        
        _isComplimentaryCoffee = isCoffeeComplimentary;

        _orderModel ??= new Order();

        _orderModel.CustomerId = customerId;
    }

    private void OnCoffeeQuantityChange(ChangeEventArgs e)
    {
        if (e.Value == null) return;

        var stringValue = e.Value.ToString();
            
        if (string.IsNullOrWhiteSpace(stringValue))
        {
            e.Value = 0;
        }
        else
        {
            e.Value = int.TryParse(stringValue, out var intValue) ? intValue : 0;
        }
        
        var coffeeQuantity = int.Parse(e.Value.ToString());

        var coffeePrice = Coffees.FirstOrDefault(x => x.Id == _orderModel.CoffeeId)?.Price ?? 0;

        var addInAmount = (from orderAddIn in _orderAddInModel 
            let addInPrice = AddInService.GetAll(_addInsPath).FirstOrDefault(x => x.Id == orderAddIn.AddInId)?.Price ?? 0 
            let addInQuantity = orderAddIn.AddInQuantity 
            select addInPrice * addInQuantity).Sum();

        var coffeeAmount = coffeePrice * coffeeQuantity;
        
        _actualAmount = coffeeAmount + addInAmount;

        if (_isRegularCustomer.HasValue && _isRegularCustomer.Value)
        {
            _payableAmount = _actualAmount - (_actualAmount * 0.15m);
        }
        else if (_isComplimentaryCoffee.HasValue && _isComplimentaryCoffee.Value)
        {
            _payableAmount = _actualAmount - coffeeAmount;
        }
        else
        {
            _payableAmount = _actualAmount;
        }
    }
    
    private void OnAddInQuantityChange(ChangeEventArgs e, int index)
    {
        if (e.Value == null) return;
        
        var stringValue = e.Value.ToString();
            
        if (string.IsNullOrWhiteSpace(stringValue))
        {
            e.Value = 0;
        }
        else
        {
            e.Value = int.TryParse(stringValue, out var intValue) ? intValue : 0;
        }
        
        var addInQuantityCount = int.Parse(e.Value.ToString());

        _orderAddInModel[index].AddInQuantity = addInQuantityCount;
        
        var coffeePrice = Coffees.FirstOrDefault(x => x.Id == _orderModel.CoffeeId)?.Price ?? 0;

        var coffeeQuantity = _orderModel?.CoffeeQuantity ?? 0;

        var coffeeAmount = coffeePrice * coffeeQuantity;

        var addInAmount = (from orderAddIn in _orderAddInModel 
            let addInPrice = AddInService.GetAll(_addInsPath).FirstOrDefault(x => x.Id == orderAddIn.AddInId)?.Price ?? 0 
            let addInQuantity = orderAddIn.AddInQuantity 
            select addInPrice * addInQuantity).Sum();
        
        _actualAmount = coffeeAmount + addInAmount;

        if (_isRegularCustomer.HasValue && _isRegularCustomer.Value)
        {
            _payableAmount = _actualAmount - (_actualAmount * 0.15m);
        }
        else if (_isComplimentaryCoffee.HasValue && _isComplimentaryCoffee.Value)
        {
            _payableAmount = _actualAmount - coffeeAmount;
        }
        else
        {
            _payableAmount = _actualAmount;
        }
    }

    private void AddAddIn()
    {
        _orderAddInModel.Add(new OrderAddIn()
        {
            AddInId = Guid.Empty,
            AddInQuantity = 0
        });
    }
}