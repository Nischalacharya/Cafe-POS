using Cafe.POS.Models;

namespace Cafe.POS.Services;

public class CustomerService : GenericService<Customer>
{
    private static readonly string AppDataDirectoryPath = UtilityService.GetAppDirectoryPath();
    private static readonly string AppCustomersFilePath = UtilityService.GetAppCustomersFilePath();
    private static readonly string AppOrdersFilePath = UtilityService.GetAppOrdersFilePath();

	public static Customer GetById(Guid id)
	{
		var customer = GetAll(AppCustomersFilePath).FirstOrDefault(x => x.Id == id);

		if(customer == null)
		{
			throw new Exception("Customer not found.");
		}

		return customer;
	}

	public static List<Customer> Create(Customer customer)
	{
		var customers = GetAll(AppCustomersFilePath);

		if(string.IsNullOrEmpty(customer.Username) || string.IsNullOrEmpty(customer.PhoneNumber))
		{
			throw new Exception("Please insert correct and valid input for each of the fields.");
		}
		
		var customerExists = customers.Any(x => string.Equals(x.Username, customer.Username, StringComparison.CurrentCultureIgnoreCase) || string.Equals(x.PhoneNumber, customer.PhoneNumber, StringComparison.CurrentCultureIgnoreCase));

		if (customerExists)
		{
			throw new Exception("Customer with the same name or phone number already exists, please add with a different title.");
		}

		customers.Add(customer);

		SaveAll(customers, AppDataDirectoryPath, AppCustomersFilePath);

		return customers;
	}

	public static List<Customer> Update(Customer customer)
	{
		var customers = GetAll(AppCustomersFilePath);

		var customerItem = customers.FirstOrDefault(x => x.Id == customer.Id);

		if (customerItem == null)
		{
			throw new Exception("Customer not found.");
		}

		customerItem.Username = customer.Username;
		customerItem.PhoneNumber = customer.PhoneNumber;
		customerItem.LastModifiedOn = DateTime.Now;
		customerItem.LastModifiedBy = customer.LastModifiedBy;

		SaveAll(customers, AppDataDirectoryPath, AppCustomersFilePath);

		return customers;
	}
	
	public static void UpdateOrderCount(Guid customerId)
	{
		var customers = GetAll(AppCustomersFilePath);

		var customerItem = customers.FirstOrDefault(x => x.Id == customerId);

		if (customerItem == null)
		{
			throw new Exception("Customer not found.");
		}

		customerItem.Orders++;

		SaveAll(customers, AppDataDirectoryPath, AppCustomersFilePath);
	}
	
	public static bool IsAvailableForComplimentaryCoffee(Guid customerId)
	{
		var customers = GetAll(AppCustomersFilePath);

		var customerItem = customers.FirstOrDefault(x => x.Id == customerId);

		if (customerItem == null)
		{
			throw new Exception("Customer not found.");
		}

		return customerItem.Orders >= 10 && customerItem.Orders % 10 == 0;
	}
	
	public static bool IsRegularCustomer(Guid customerId)
	{
		var customers = GetAll(AppCustomersFilePath);

		var customerItem = customers.FirstOrDefault(x => x.Id == customerId);

		if (customerItem == null)
		{
			throw new Exception("Customer not found.");
		}

		var orders = OrderService.GetAll(AppOrdersFilePath);
		
		var currentMonth = DateTime.Now.AddMonths(-1);
		
		var customerOrders = orders.Where(x => x.CustomerId == customerId && x.CreatedOn >= currentMonth && x.CreatedOn <= DateTime.Now).ToList();
		
		return customerOrders.Count >= 15;
	}

	public static List<Customer> Delete(Guid id)
	{
		var customers = GetAll(AppCustomersFilePath);

		var customer = customers.FirstOrDefault(x => x.Id == id);

		if (customer == null)
		{
			throw new Exception("Customer not found.");
		}

		customers.Remove(customer);

		SaveAll(customers, AppDataDirectoryPath, AppCustomersFilePath);

		return customers;
	}

	public static List<Customer> GetRegularCustomers()
	{
		var customers = GetAll(AppCustomersFilePath);

		var customerIds = (from customer in customers where IsRegularCustomer(customer.Id) select customer.Id).ToList();

		return customers.Where(x => customerIds.Contains(x.Id)).ToList();
	}
	
	public static List<Customer> GetComplimentaryCoffeeCustomers()
	{
		var users = GetAll(AppCustomersFilePath).Where(x => x.Orders >= 10);

		return users.Where(x => x.Orders % 10 == 0).ToList();
	}
	
	public static List<Customer> MergeSort(List<Customer> unsorted)
	{
		if (unsorted.Count <= 1)
			return unsorted;

		var left = new List<Customer>();
		var right = new List<Customer>();

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

	private static List<Customer> Merge(List<Customer> left, List<Customer> right)
	{
		var result = new List<Customer>();

		while (left.Count > 0 || right.Count > 0)
		{
			switch (left.Count)
			{
				case > 0 when right.Count > 0:
				{
					if (left.First().Orders <= right.First().Orders)
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