using System;
using System.Collections.Generic;

namespace ProjectCalculator.Model
{
	public class Record
	{
		public int id { get; set; }

		public DateTime entryDate { get; set; }

		public int projectId { get; set; }

		public string projectName { get; set; }

		public int userId { get; set; }

		public string userName { get; set; }

		public int taskId { get; set; }

		public string taskName { get; set; }

		public string notes { get; set; }

		public int minutes { get; set; }

		public int billed { get; set; }
	}

	public class RecordList
	{
		public List<Record> Entries { get; set; }
	}
}