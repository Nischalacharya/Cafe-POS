using Cafe.POS.Models;
using Cafe.POS.Models.Base;
using Cafe.POS.Services;
using Microsoft.AspNetCore.Components;

namespace Cafe.POS.Components.Pages;

public partial class Customers
{
    [CascadingParameter] 
    private GlobalState _globalState { get; set; }

    private List<Customer> _customers { get; set; }
    
    private bool _showAddCustomerDialog { get; set; }
    
    private bool _showDeleteCustomerDialog { get; set; }
    
    private string _addCustomerErrorMessage { get; set; }
    
    private string _deleteCustomerErrorMessage { get; set; }
    
    private Customer? _customerModel { get; set; }
    
    private string _dialogTitle { get; set; }
    
    private string _dialogOkLabel { get; set; }
    
    private string _tabFilter = "All";

    private string _sortBy = "customerName";
    
    private string _sortDirection = "ascending";
    
    private readonly string _customersPath = UtilityService.GetAppCustomersFilePath();
    
    protected override void OnInitialized()
    {
        _customers = CustomerService.GetAll(_customersPath);
    }
    
    private void OpenAddUserDialog()
    {
        _dialogTitle = "Add a new user";

        _dialogOkLabel = "Add";

        _customerModel = new Customer();

        _showAddCustomerDialog = true;
    }

    private void OpenDeleteUserDialog(Customer? customer)
    {
        _dialogTitle = "Remove an existing customer";

        _dialogOkLabel = "Confirm";

        _customerModel = customer;

        _showDeleteCustomerDialog = true;
    }

    private void SortByHandler(string sortByName)
    {
        if (_sortBy == sortByName)
        {
            _sortDirection = _sortDirection == "ascending" ? "descending" : "ascending";
        }
        else
        {
            _sortBy = sortByName;

            _sortDirection = "ascending";
        }
    }

    private void TabFilter(string status)
    {
        _tabFilter = status;
    }
    
    private void SearchUserName(ChangeEventArgs e)
    {
        var searchItem = e.Value.ToString();

        if (!string.IsNullOrEmpty(searchItem) && searchItem.Length > 1)
        {
            _customers = CustomerService.GetAll(_customersPath).Where(p => p.Username.ToLower().Contains(searchItem.ToLower())).ToList();
        }
        else
        {
            _customers = CustomerService.GetAll(_customersPath);
        }
    }

    private void OnAddCustomerDialogClose(bool isClosed)
    {
        if (isClosed)
        {
            _showAddCustomerDialog = false;
        }
        else
        {
            try
            {
                _addCustomerErrorMessage = "";

                var customer = new Customer()
                {
                    Username = _customerModel.Username,
                    PhoneNumber = _customerModel.PhoneNumber,
                    Orders = 0,
                    IsActive = true,
                    CreatedBy = _globalState.User.Id,
                };
                
                _customers = CustomerService.Create(customer);

                _showAddCustomerDialog = false;
            }
            catch (Exception e)
            {
                _addCustomerErrorMessage = e.Message;

                Console.WriteLine(e.Message);
            }
        }
    }

    private void OnDeleteUserDialogClose(bool isClosed)
    {
        if (isClosed)
        {
            _showDeleteCustomerDialog = false;

            _customerModel = null;
        }
        else
        {
            try
            {
                _deleteCustomerErrorMessage = "";

                _customers = CustomerService.Delete(_customerModel.Id);

                _showDeleteCustomerDialog = false;

                _customerModel = null;
            }
            catch (Exception e)
            {
                _deleteCustomerErrorMessage = e.Message;

                Console.WriteLine(e.Message);
            }
        }
    }
}