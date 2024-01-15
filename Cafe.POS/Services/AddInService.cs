using Cafe.POS.Models;

namespace Cafe.POS.Services;

public class AddInService : GenericService<AddIn>
{
    private static readonly string AppDataDirectoryPath = UtilityService.GetAppDirectoryPath();
	private static readonly string AppAddInsFilePath = UtilityService.GetAppAddInsFilePath();

	public static AddIn GetById(Guid id)
	{
		var addIn = GetAll(AppAddInsFilePath).FirstOrDefault(x => x.Id == id);

		if(addIn == null)
		{
			throw new Exception("Add In not found.");
		}

		return addIn;
	}

	public static List<AddIn> Create(AddIn addIn)
	{
		var addIns = GetAll(AppAddInsFilePath);

		if(string.IsNullOrEmpty(addIn.Name) || string.IsNullOrEmpty(addIn.Description))
		{
			throw new Exception("Please insert correct and valid input for each of the fields.");
		}
		
		var addInExists = addIns.Any(x => string.Equals(x.Name, addIn.Name, StringComparison.CurrentCultureIgnoreCase));

		if (addInExists)
		{
			throw new Exception("Add In with the same name already exists, please add with a different title.");
		}

		addIns.Add(addIn);

		SaveAll(addIns, AppDataDirectoryPath, AppAddInsFilePath);

		return addIns;
	}

	public static List<AddIn> Update(AddIn addIn)
	{
		var addIns = GetAll(AppAddInsFilePath);

		var addInItem = addIns.FirstOrDefault(x => x.Id == addIn.Id);

		if (addInItem == null)
		{
			throw new Exception("Add In not found.");
		}

		addInItem.Name = addIn.Name;
		addInItem.Description = addIn.Description;
		addInItem.IsActive = addIn.IsActive;
		addInItem.Price = addIn.Price;
		addInItem.Unit = addIn.Unit;
		addInItem.LastModifiedOn = DateTime.Now;
		addInItem.LastModifiedBy = addIn.LastModifiedBy;

		SaveAll(addIns, AppDataDirectoryPath, AppAddInsFilePath);

		return addIns;
	}

	public static List<AddIn> Delete(Guid id)
	{
		var addIns = GetAll(AppAddInsFilePath);

		var addIn = addIns.FirstOrDefault(x => x.Id == id);

		if (addIn == null)
		{
			throw new Exception("AddIn not found.");
		}

		addIns.Remove(addIn);

		SaveAll(addIns, AppDataDirectoryPath, AppAddInsFilePath);

		return addIns;
	}

	public static List<AddIn> MergeSort(List<AddIn> unsorted)
	{
		if (unsorted.Count <= 1)
			return unsorted;

		var left = new List<AddIn>();
		var right = new List<AddIn>();

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

	private static List<AddIn> Merge(List<AddIn> left, List<AddIn> right)
	{
		var result = new List<AddIn>();

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