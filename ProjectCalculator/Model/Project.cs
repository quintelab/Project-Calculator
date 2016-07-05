using System.Collections.Generic;

namespace ProjectCalculator.Model
{
	public class Project
	{
		public int id { get; set; }

		public string name { get; set; }

		public int isActive { get; set; }
	}

	public class ProjectList
	{
		public List<Project> Projects { get; set;}
	}
}