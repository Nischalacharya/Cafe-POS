using Cafe.POS.Models;
using Cafe.POS.Models.Base;
using Cafe.POS.Services;
using Microsoft.AspNetCore.Components;

namespace Cafe.POS.Components.Pages;

public partial class AddIns
{
    [CascadingParameter] 
    private GlobalState _globalState { get; set; }

    private List<AddIn> _addIns { get; set; }
    
    private bool _showUpsertAddInDialog { get; set; }

    private bool _isForEditAction { get; set; } = false;
    
    private bool _showPasswordDialog { get; set; }

    private bool _showDeleteAddInDialog { get; set; }
    
    private AddIn? _addInModel { get; set; }
    
    private string _dialogTitle { get; set; }

    private string _password { get; set; }
    
    private string _dialogOkLabel { get; set; }
    
    private string _upsertAddInErrorMessage { get; set; }
    
    private string _passwordErrorMessage { get; set; }
    
    private string _deleteAddInErrorMessage { get; set; }
    
    private string _tabFilter = "All";
    
    private string _sortBy = "addIn";
    
    private string _sortDirection = "up";
    
    private readonly string _addInsPath = UtilityService.GetAppAddInsFilePath();

    protected override void OnInitialized()
    {
        _addIns = AddInService.GetAll(_addInsPath);
    }

    private void OpenAddAddInDialog()
    {
        _dialogTitle = "Add a new add in";

        _dialogOkLabel = "Add";

        _upsertAddInErrorMessage = "";

        _addInModel = new AddIn
        {
            Id = Guid.Empty
        };

        _showUpsertAddInDialog = true;
    }

    private void OpenPasswordDialog(AddIn addIn, bool isForEditAction)
    {
        _dialogTitle = "Enter your password";

        _dialogOkLabel = "Save";

        _passwordErrorMessage = "";

        _showPasswordDialog = true;

        _addInModel = addIn;
        
        _password = "";

        _isForEditAction = isForEditAction;
    }
    
    private void OpenEditAddInDialog(AddIn addIn)
    {
        _dialogTitle = "Edit an existing add in";

        _dialogOkLabel = "Save";

        _upsertAddInErrorMessage = "";

        _addInModel = addIn;

        _showUpsertAddInDialog = true;
    }

    private void OpenDeleteAddInDialog(AddIn addIn)
    {
        _dialogTitle = "Delete an add in";

        _dialogOkLabel = "Confirm";

        _addInModel = addIn;

        _showDeleteAddInDialog = true;
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

    private void SearchAddInName(ChangeEventArgs e)
    {
        var searchItem = e.Value.ToString();

        if (!string.IsNullOrEmpty(searchItem) && searchItem.Length > 1)
        {
            _addIns = AddInService.GetAll(_addInsPath).Where(p => p.Name.ToLower().Contains(searchItem.ToLower()))
                .ToList();
        }
        else
        {
            _addIns = AddInService.GetAll(_addInsPath);
        }
    }

    private void OnUpsertAddInDialogClose(bool isClosed)
    {
        if (isClosed)
        {
            _showUpsertAddInDialog = false;
        }
        else
        {
            try
            {
                _upsertAddInErrorMessage = "";

                if (_addInModel.Id == Guid.Empty)
                {
                    var addIn = new AddIn()
                    {
                        Name = _addInModel.Name,
                        Unit = _addInModel.Unit,
                        Description = _addInModel.Description,
                        Price = _addInModel.Price,
                        IsActive = true,
                        CreatedBy = _globalState.User.Id,
                    };

                    _addIns = AddInService.Create(addIn);
                }

                else
                {
                    var addIn = new AddIn()
                    {
                        Id = _addInModel.Id,
                        Name = _addInModel.Name,
                        Unit = _addInModel.Unit,
                        Description = _addInModel.Description,
                        Price = _addInModel.Price,
                        IsActive = true,
                        LastModifiedBy = _globalState.User.Id,
                        LastModifiedOn = DateTime.Now
                    };

                    _addIns = AddInService.Update(addIn);
                }

                _showUpsertAddInDialog = false;
            }
            catch (Exception e)
            {
                _upsertAddInErrorMessage = e.Message;

                Console.WriteLine(e.Message);
            }
        }
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
                    
                    OpenEditAddInDialog(_addInModel);
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
                    
                    OpenDeleteAddInDialog(_addInModel);
                }
            }
        }
        catch (Exception e)
        {
            _passwordErrorMessage = "Invalid password, please try again.";

            Console.WriteLine(e.Message);
        }
    }

    private void OnDeleteAddInDialogClose(bool isClosed)
    {
        if (isClosed)
        {
            _showDeleteAddInDialog = false;

            _addInModel = null;
        }
        else
        {
            try
            {
                _deleteAddInErrorMessage = "";

                _addIns = AddInService.Delete(_addInModel.Id);

                _showDeleteAddInDialog = false;

                _addInModel = null;
            }
            catch (Exception e)
            {
                _deleteAddInErrorMessage = e.Message;

                Console.WriteLine(e.Message);
            }
        }
    }

    private void UpdateActiveStatus(AddIn addIn)
    {
        var addInModel = new AddIn()
        {
            Id = addIn.Id,
            Name = addIn.Name,
            Description = addIn.Description,
            Unit = addIn.Unit,
            Price = addIn.Price,
            IsActive = !addIn.IsActive,
            LastModifiedBy = _globalState.User.Id,
            LastModifiedOn = DateTime.Now
        };
        
        _addIns = AddInService.Update(addInModel);
    }
}