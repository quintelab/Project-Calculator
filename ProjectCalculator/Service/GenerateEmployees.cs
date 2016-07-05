using System.Collections.Generic;
using ProjectCalculator.Model;

namespace ProjectCalculator.Service
{
	/// <summary>
	/// Here you can create a list of user based on timeneye's user
	/// Cost is the hourly rate for a user
	/// </summary>
	public static class GenerateEmployees
	{
		public static List<Employe> GetEmployees()
		{
			List<Employe> employees = new List<Employe>();
			employees.Add(new Employe { Name = "James Jacob", Cost = 102.84 });
			employees.Add(new Employe { Name = "Anthony Daniel", Cost = 59.67 });
			employees.Add(new Employe { Name = "Michael Ryan", Cost = 90.10 });
			employees.Add(new Employe { Name = "Ethan Noah", Cost = 70.98 });

			return employees;
		}
	}
}