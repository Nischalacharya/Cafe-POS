using Cafe.POS.Models;
using Cafe.POS.Models.Base;
using Cafe.POS.Models.Enums;
using Cafe.POS.Services;
using Microsoft.AspNetCore.Components;

namespace Cafe.POS.Components.Pages;

public partial class Users
{
    [CascadingParameter] 
    private GlobalState _globalState { get; set; }

    private List<User> _users { get; set; }
    
    private bool _showAddUserDialog { get; set; }
    
    private bool _showDeleteUserDialog { get; set; }
    
    private string _addUserErrorMessage { get; set; }
    
    private string _deleteUserErrorMessage { get; set; }
    
    private User? _userModel { get; set; }
    
    private string _dialogTitle { get; set; }
    
    private string _dialogOkLabel { get; set; }
    
    private Role _userRole { get; set; }
    
    private string _userPassword { get; set; }
    
    private string _tabFilter = "All";
    
    private string _sortBy = "username";
    
    private string _sortDirection = "ascending";
    
    private readonly string _usersPath = UtilityService.GetAppUsersFilePath();

    protected override void OnInitialized()
    {
        _users = UserService.GetAll(_usersPath);
    }

    private void RoleChangeHandler(ChangeEventArgs e)
    {
        _userRole = (Role)Enum.Parse(typeof(Role), e.Value.ToString());
    }

    private void OpenAddUserDialog()
    {
        _dialogTitle = "Add a new user";

        _dialogOkLabel = "Add";

        _userModel = new User();

        _userPassword = "";
        
        _userRole = new ();

        _showAddUserDialog = true;
    }

    private void OpenDeleteUserDialog(User? user)
    {
        _dialogTitle = "Remove an existing user";

        _dialogOkLabel = "Confirm";

        _userModel = user;

        _showDeleteUserDialog = true;
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
            _users = UserService.GetAll(_usersPath).Where(p => p.Username.ToLower().Contains(searchItem.ToLower()))
                .ToList();
        }
        else
        {
            _users = UserService.GetAll(_usersPath);
        }
    }

    private void OnAddUserDialogClose(bool isClosed)
    {
        if (isClosed)
        {
            _showAddUserDialog = false;
        }
        else
        {
            try
            {
                _addUserErrorMessage = "";

                var user = new User()
                {
                    CreatedBy = _globalState.User.Id,
                    Username = _userModel.Username,
                    Email = _userModel.Email,
                    PasswordHash = _userPassword,
                    IsActive = true,
                    Role = _userRole,
                };
                
                _users = UserService.Create(user);

                _showAddUserDialog = false;
            }
            catch (Exception e)
            {
                _addUserErrorMessage = e.Message;

                Console.WriteLine(e.Message);
            }
        }
    }

    private void OnDeleteUserDialogClose(bool isClosed)
    {
        if (isClosed)
        {
            _showDeleteUserDialog = false;

            _userModel = null;
        }
        else
        {
            try
            {
                _deleteUserErrorMessage = "";

                _users = UserService.Delete(_userModel.Id);

                _showDeleteUserDialog = false;

                _userModel = null;
            }
            catch (Exception e)
            {
                _deleteUserErrorMessage = e.Message;

                Console.WriteLine(e.Message);
            }
        }
    }
}