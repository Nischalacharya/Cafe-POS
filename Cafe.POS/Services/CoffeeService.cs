using Cafe.POS.Models;

namespace Cafe.POS.Services;

public class CoffeeService : GenericService<Coffee>
{
	private static readonly string AppDataDirectoryPath = UtilityService.GetAppDirectoryPath();
	private static readonly string AppCoffeesFilePath = UtilityService.GetAppCoffeesFilePath();

	public static Coffee GetById(Guid id)
	{
		var coffee = GetAll(AppCoffeesFilePath).FirstOrDefault(x => x.Id == id);

		if(coffee == null)
		{
			throw new Exception("Coffee not found.");
		}

		return coffee;
	}

	public static List<Coffee> Create(Coffee coffee)
	{
		var coffees = GetAll(AppCoffeesFilePath);

		if(string.IsNullOrEmpty(coffee.Name) || string.IsNullOrEmpty(coffee.Description))
		{
			throw new Exception("Please insert correct and valid input for each of the fields.");
		}
		
		var coffeeExists = coffees.Any(x => string.Equals(x.Name, coffee.Name, StringComparison.CurrentCultureIgnoreCase));

		if (coffeeExists)
		{
			throw new Exception("Coffee with the same name already exists, please add with a different title.");
		}

		coffees.Add(coffee);

		SaveAll(coffees, AppDataDirectoryPath, AppCoffeesFilePath);

		return coffees;
	}

	public static List<Coffee> Update(Coffee coffee)
	{
		var coffees = GetAll(AppCoffeesFilePath);

		var coffeeItem = coffees.FirstOrDefault(x => x.Id == coffee.Id);

		if (coffeeItem == null)
		{
			throw new Exception("Coffee not found.");
		}

		coffeeItem.Name = coffee.Name;
		coffeeItem.Description = coffee.Description;
		coffeeItem.Price = coffee.Price;
		coffeeItem.IsActive = coffee.IsActive;
		coffeeItem.LastModifiedOn = DateTime.Now;
		coffeeItem.LastModifiedBy = coffee.LastModifiedBy;

		SaveAll(coffees, AppDataDirectoryPath, AppCoffeesFilePath);

		return coffees;
	}

	public static List<Coffee> Delete(Guid id)
	{
		var coffees = GetAll(AppCoffeesFilePath);

		var coffee = coffees.FirstOrDefault(x => x.Id == id);

		if (coffee == null)
		{
			throw new Exception("Coffee not found.");
		}

		coffees.Remove(coffee);

		SaveAll(coffees, AppDataDirectoryPath, AppCoffeesFilePath);

		return coffees;
	}

	public static List<Coffee> MergeSort(List<Coffee> unsorted)
	{
		if (unsorted.Count <= 1)
			return unsorted;

		var left = new List<Coffee>();
		var right = new List<Coffee>();

		var middle = unsorted.Count / 2;

		for (var i = 0; i < middle; i++)  
		{
			left.Add(unsorted[i]);
		}
		
		for (var i = middle; i < unsorted.Count; i++)
		{
			right.Add(unsorted[i]);
		}

		left = MergeSort(left);
		right = MergeSort(right);
		
		return Merge(left, right);
	}

	private static List<Coffee> Merge(List<Coffee> left, List<Coffee> right)
	{
		var result = new List<Coffee>();

		while (left.Count > 0 || right.Count > 0)
		{
			switch (left.Count)
			{
				case > 0 when right.Count > 0:
				{
					if (left.First().Price <= right.First().Price)
					{
						result.Add(left.First());
						left.Remove(left.First());
					}
					else
					{
						result.Add(right.First());
						right.Remove(right.First());
					}

					break;
				}
				case > 0:
					result.Add(left.First());
					left.Remove(left.First());
					break;
				default:
				{
					if (right.Count > 0)
					{
						result.Add(right.First());
						right.Remove(right.First());
					}

					break;
				}
			}
		}
		return result;
	}
}