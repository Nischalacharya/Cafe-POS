using Cafe.POS.Models;
using Cafe.POS.Models.Base;
using Cafe.POS.Services;
using Microsoft.AspNetCore.Components;

namespace Cafe.POS.Components.Pages;

public partial class Coffees
{
    [CascadingParameter] private GlobalState _globalState { get; set; }

    private List<Coffee> _coffees { get; set; }
    
    private bool _showUpsertCoffeeDialog { get; set; }
    
    private bool _showDeleteCoffeeDialog { get; set; }
    
    private Coffee _coffeeModel { get; set; }
    
    private string _dialogTitle { get; set; }
    
    private string _dialogOkLabel { get; set; }
    
    private string _upsertCoffeeErrorMessage { get; set; }
    
    private string _deleteCoffeeErrorMessage { get; set; }
    
    private string _tabFilter = "All";
    
    private string _sortBy = "coffee";
    
    private string _sortDirection = "up";
    
    private string _passwordErrorMessage = "";
    
    private bool _showPasswordDialog = false;
    
    private string _password;

    private bool _isForEditAction = false;
    
    private readonly string _coffeesPath = UtilityService.GetAppCoffeesFilePath();

    protected override void OnInitialized()
    {
        _coffees = CoffeeService.GetAll(_coffeesPath);
    }

    private void OpenAddCoffeeDialog()
    {
        _dialogTitle = "Add a new coffee";

        _dialogOkLabel = "Add";

        _upsertCoffeeErrorMessage = "";

        _coffeeModel = new Coffee
        {
            Id = Guid.Empty
        };

        _showUpsertCoffeeDialog = true;
    }

    private void OpenEditCoffeeDialog(Coffee coffee)
    {
        _dialogTitle = "Edit an existing coffee";

        _dialogOkLabel = "Save";

        _upsertCoffeeErrorMessage = "";

        _coffeeModel = coffee;

        _showUpsertCoffeeDialog = true;
    }

    private void OpenDeleteCoffeeDialog(Coffee coffee)
    {
        _dialogTitle = "Delete a coffee";

        _dialogOkLabel = "Confirm";

        _coffeeModel = coffee;

        _showDeleteCoffeeDialog = true;
    }

    private void SortByHandler(string sortByName)
    {
        if (_sortBy == sortByName)
        {
            _sortDirection = _sortDirection == "up" ? "down" : "up";
        }
        else
        {
            _sortBy = sortByName;

            _sortDirection = "up";
        }
    }

    private void TabFilter(string status)
    {
        _tabFilter = status;
    }

    private void SearchCoffeeName(ChangeEventArgs e)
    {
        var searchItem = e.Value.ToString();

        if (!string.IsNullOrEmpty(searchItem) && searchItem.Length > 1)
        {
            _coffees = CoffeeService.GetAll(_coffeesPath).Where(p => p.Name.Contains(searchItem, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }
        else
        {
            _coffees = CoffeeService.GetAll(_coffeesPath);
        }
    }

    private void OnUpsertCoffeeDialogClose(bool isClosed)
    {
        if (isClosed)
        {
            _showUpsertCoffeeDialog = false;
        }
        else
        {
            try
            {
                _upsertCoffeeErrorMessage = "";

                if (_coffeeModel.Id == Guid.Empty)
                {
                    var coffee = new Coffee()
                    {
                        Name = _coffeeModel.Name,
                        Description = _coffeeModel.Description,
                        Price = _coffeeModel.Price,
                        IsActive = true,
                        CreatedBy = _globalState.User.Id
                    };
                    
                    _coffees = CoffeeService.Create(coffee);
                }

                else
                {
                    var coffee = new Coffee()
                    {
                        Id = _coffeeModel.Id,
                        Name = _coffeeModel.Name,
                        Description = _coffeeModel.Description,
                        Price = _coffeeModel.Price,
                        IsActive = true,
                        LastModifiedBy = _globalState.User.Id,
                        LastModifiedOn = DateTime.Now
                    };
                    
                    _coffees = CoffeeService.Update(coffee);
                }

                _showUpsertCoffeeDialog = false;
            }
            catch (Exception e)
            {
                _upsertCoffeeErrorMessage = e.Message;

                Console.WriteLine(e.Message);
            }
        }
    }

    private void OnDeleteCoffeeDialogClose(bool isClosed)
    {
        if (isClosed)
        {
            _showDeleteCoffeeDialog = false;

            _coffeeModel = null;
        }
        else
        {
            try
            {
                _deleteCoffeeErrorMessage = "";

                _coffees = CoffeeService.Delete(_coffeeModel.Id);

                _showDeleteCoffeeDialog = false;

                _coffeeModel = null;
            }
            catch (Exception e)
            {
                _deleteCoffeeErrorMessage = e.Message;

                Console.WriteLine(e.Message);
            }
        }
    }
    
    private void UpdateActiveStatus(Coffee coffee)
    {
        var coffeeModel = new Coffee()
        {
            Id = coffee.Id,
            Name = coffee.Name,
            Description = coffee.Description,
            Price = coffee.Price,
            IsActive = !coffee.IsActive,
            LastModifiedBy = _globalState.User.Id,
            LastModifiedOn = DateTime.Now
        };
        
        _coffees = CoffeeService.Update(coffeeModel);
    }

    private void OnPasswordVerifyDialogClose(bool isClosed)
    {
        try
        {
            if (isClosed)
            {
                _showPasswordDialog = false;
            }
            else
            {
                if (_isForEditAction)
                {
                    _passwordErrorMessage = "";

                    var username = _globalState.User.Username;
                    
                    var passwordIsValid = UserService.Login(username, _password);

                    if (passwordIsValid == null)
                    {
                        throw new Exception("Invalid password, please try again.");
                    }

                    _showPasswordDialog = false;
                    
                    OpenEditCoffeeDialog(_coffeeModel);
                }
                else
                {
                    _passwordErrorMessage = "";

                    var username = _globalState.User.Username;
                    
                    var passwordIsValid = UserService.Login(username, _password);

                    if (passwordIsValid == null)
                    {
                        throw new Exception("Invalid password, please try again.");
                    }

                    _showPasswordDialog = false;
                    
                    OpenDeleteCoffeeDialog(_coffeeModel);
                }
            }
        }
        catch (Exception e)
        {
            _passwordErrorMessage = "Invalid password, please try again.";

            Console.WriteLine(e.Message);
        }
    }
    
    
    private void OpenPasswordDialog(Coffee coffee, bool isForEditAction)
    {
        _dialogTitle = "Enter your password";

        _dialogOkLabel = "Save";

        _passwordErrorMessage = "";

        _showPasswordDialog = true;

        _coffeeModel = coffee;
        
        _password = "";

        _isForEditAction = isForEditAction;
    }
}