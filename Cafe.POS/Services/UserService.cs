using Cafe.POS.Models;
using Cafe.POS.Models.Enums;

namespace Cafe.POS.Services;

/// <summary>
/// Service class for managing user-related operations in the Cafe POS application.
/// </summary>
public class UserService : GenericService<User>
{
	public const string SeedUsername = "admin";
	public const string SeedPassword = "admin";
	private const string SeedEmail = "admin@admin.com";

	private static readonly string AppDataDirectoryPath = UtilityService.GetAppDirectoryPath();
	private static readonly string AppUsersFilePath = UtilityService.GetAppUsersFilePath();
    
	/// <summary>
    /// The default password for seeding an admin user.
    /// </summary>
    public static void SeedUser()
	{
		var users = GetAll(AppUsersFilePath).FirstOrDefault(x => x.Role == Role.Admin);

		if (users != null) return;
		
		var user = new User()
		{
			Username = SeedUsername,
			Email = SeedEmail,
			PasswordHash = SeedPassword,
			Role = Role.Admin
		};
			
		Create(user);
	}

	public static User Login(string username, string password)
	{
		const string loginErrorMessage = "Invalid username or password.";

		var users = GetAll(AppUsersFilePath);

		var user = users.FirstOrDefault(x => x.Username.Equals(username.Trim(), StringComparison.CurrentCultureIgnoreCase));

		if (user == null)
		{
			throw new Exception(loginErrorMessage);
		}

		var passwordIsValid = UtilityService.VerifyHash(password, user.PasswordHash);

		if (!passwordIsValid)
		{
			throw new Exception(loginErrorMessage);
		}

		return user;
	}

	public static User GetById(Guid id)
	{
		if (id == Guid.Empty)
		{
			return new User();
		}
		
		var user = GetAll(AppUsersFilePath).FirstOrDefault(x => x.Id == id);

		if (user == null)
		{
			throw new Exception("User not found.");
		}
		
		return user;
	}

	public static List<User> Create(User user)
	{
		if(string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
		{
			throw new Exception("Please insert correct and valid input for each of the fields.");
		}

		var users = GetAll(AppUsersFilePath);

		var usernameExists = users.Any(x => x.Username == user.Username);

		if (usernameExists)
		{
			throw new Exception("A username already exists, please choose any other username.");
		}

		var userModel = new User()
		{
			Username = user.Username,
			Email = user.Email,
			PasswordHash = UtilityService.HashSecret(user.PasswordHash),
			Role = user.Role,
			CreatedBy = user.CreatedBy,
		};

		users.Add(userModel);

		SaveAll(users, AppDataDirectoryPath, AppUsersFilePath);

		return users;
	}

	public static List<User> Delete(Guid id)
	{
		var users = GetAll(AppUsersFilePath);

		var user = users.FirstOrDefault(x => x.Id == id);

		if (user == null)
		{
			throw new Exception("User not found.");
		}

		users.Remove(user);

		SaveAll(users, AppDataDirectoryPath, AppUsersFilePath);

		return users;
	}
}