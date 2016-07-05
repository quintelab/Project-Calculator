using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using ProjectCalculator.Model;
using ProjectCalculator.ViewModel.Service;
using ProjectCalculator.Service;
using Xamarin.Forms;

namespace ProjectCalculator.ViewModel
{
	public class HomeViewModel : ViewModelBase
	{
		private List<Project> _projects;
		public List<Project> Projects
		{
			get
			{
				return _projects;
			}
			set
			{
				_projects = value;
				Notify();
			}
		}

		private Project _selectedProject;
		public Project SelectedProject
		{
			get
			{
				return _selectedProject;
			}
			set
			{
				_selectedProject = value;
				IsPanelCustomVisible = false;
				Notify();
			}
		}

		private DateTime _initialDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
		public DateTime InitialDate
		{
			get
			{
				return _initialDate;
			}
			set
			{
				_initialDate = value;
				Notify();
			}
		}

		private DateTime _endDate = DateTime.Now;
		public DateTime EndDate
		{
			get
			{
				return _endDate;
			}
			set
			{
				_endDate = value;
				Notify();
			}
		}

		private string _totalMinutes;
		public string TotalMinutes
		{
			get
			{
				return _totalMinutes;
			}
			set
			{
				_totalMinutes = value;
				Notify();
			}
		}

		private string _totalHours;
		public string TotalHours
		{
			get
			{
				return _totalHours;
			}
			set
			{
				_totalHours = value;
				Notify();
			}
		}

		private string _totalCost;
		public string TotalCost
		{
			get
			{
				return _totalCost;
			}
			set
			{
				_totalCost = value;
				Notify();
			}
		}

		private bool _searchPeriod = true;
		public bool SearchPeriod
		{
			get
			{
				return _searchPeriod;
			}
			set
			{
				_searchPeriod = value;
				IsInitialDateEnable = value;
				IsEndDateEnable = value;
				Notify();
			}
		}

		private bool _isInitialDateEnable = true;
		public bool IsInitialDateEnable
		{
			get
			{
				return _isInitialDateEnable;
			}
			set
			{
				_isInitialDateEnable = value;
				Notify();
			}
		}

		private bool _isEndDateEnable = true;
		public bool IsEndDateEnable
		{
			get
			{
				return _isEndDateEnable;
			}
			set
			{
				_isEndDateEnable = value;
				Notify();
			}
		}

		private bool _isPanelCustomVisible = false;
		public bool IsPanelCustomVisible
		{
			get
			{
				return _isPanelCustomVisible;
			}
			set
			{
				_isPanelCustomVisible = value;
				Notify();
			}
		}

		private ICommand _searchCommand;
		public ICommand SearchCommand
		{
			get
			{
				return _searchCommand;
			}
		}

		private bool _isLoading = false;
		public bool IsLoading
		{
			get
			{
				return _isLoading;
			}
			set
			{
				_isLoading = value;
				Notify();
			}
		}

		public HomeViewModel(INavigationService navigationService, IMessageService messageService)
			: base(navigationService, messageService)
		{
			FilterProjects();
			_searchCommand = new Command(FilterRegisters);
		}

		public async void FilterProjects()
		{
			Projects = await GetProjects();
		}

		public async Task<List<Project>> GetProjects()
		{
			const string SEARCH_URL = "https://track.timeneye.com/api/3/projects";
			var httpClient = new HttpClient();

			using (var request = new HttpRequestMessage(HttpMethod.Get, SEARCH_URL))
			{
				request.Headers.Add("Bearer", Token.accessToken);

				var response = await httpClient.SendAsync(request);

				if (response.StatusCode == HttpStatusCode.OK)
				{
					ProjectList projectList = JsonConvert.DeserializeObject<ProjectList>(await response.Content.ReadAsStringAsync());
					return projectList.Projects;
				}

				return null;
			}
		}

		private async void FilterRegisters()
		{
			this.IsLoading = true;
			await GetEntries();
			IsPanelCustomVisible = true;
			this.IsLoading = false;
		}

		public async Task<bool> GetEntries()
		{
			string searchUrl = buildUrl("https://track.timeneye.com/api/3/entries/?limit=10000");
			var httpClient = new HttpClient();

			using (var request = new HttpRequestMessage(HttpMethod.Get, searchUrl))
			{
				request.Headers.Add("Bearer", Token.accessToken);

				var response = await httpClient.SendAsync(request);

				if (response.StatusCode == HttpStatusCode.OK)
				{
					RecordList recordList = JsonConvert.DeserializeObject<RecordList>(await response.Content.ReadAsStringAsync());

					try
					{
						SumValues(recordList.Entries, GenerateEmployees.GetEmployees());
						return true;
					}
					catch (Exception ex)
					{
						await _messageService.ShowMessageAsync("Error", ex.Message);
						return false;
					}

				}

				return false;
			}
		}

		public string buildUrl(string originalURL)
		{
			string url = string.Empty;

			url = string.Format("{0}&projectId={1}", originalURL, SelectedProject.id);

			if (!SearchPeriod)
				url = string.Format("{0}&dateFrom={1}", url, Convert.ToDateTime("01/01/2010").ToString("yyyy/MM/dd"));
			else
			{
				url = string.Format("{0}&dateFrom={1}&dateTo={2}", url, InitialDate.ToString("yyyy/MM/dd"), EndDate.ToString("yyyy/MM/dd"));
			}

			return url;
		}

		public void SumValues(List<Record> records, List<Employe> employeList)
		{
			double totalCost = 0;
			int totalMinutes = records.Sum(entry => entry.minutes);
			TotalMinutes = string.Format("Total of minutes: {0}", totalMinutes);
			TotalHours = string.Format("Total of hours: {0}", calcHours(totalMinutes));

			List<Record> groupedRecords = groupRecords(records);

			foreach (Record record in groupedRecords)
			{

				if (employeList.Count(e => e.Name.ToUpper() == record.userName.ToUpper()) == 0)
				{
					throw new Exception(string.Format("Employee not found: {0}", record.userName.ToUpper()));
				}

				double hours = Convert.ToDouble(record.minutes) / 60;
				totalCost += employeList.FirstOrDefault(e => e.Name.ToUpper() == record.userName.ToUpper()).Cost * hours;
			}

			TotalCost = string.Format("Total invested: {0}", totalCost.ToString("C"));
		}

		public string calcHours(int totalminutes)
		{
			int hours = totalminutes / 60;
			int minutes = totalminutes % 60;
			return string.Format("{0}:{1}", hours, minutes.ToString().PadLeft(2, '0'));
		}

		public List<Record> groupRecords(List<Record> records)
		{
			return records
				.GroupBy(e => e.userName)
				.Select(cl => new Record
				{
					userName = cl.First().userName,
					minutes = cl.Sum(c => c.minutes)
				}).ToList();
		}
	}
}

