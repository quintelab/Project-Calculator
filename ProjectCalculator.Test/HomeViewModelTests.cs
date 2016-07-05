using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using ProjectCalculator.Model;
using ProjectCalculator.View.Service;
using ProjectCalculator.ViewModel;
using ProjectCalculator.ViewModel.Service;
using Xamarin.Forms;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace ProjectCalculator.Test
{
	[TestFixture]
	public class HomeViewModelTests
	{
		private Mock<INavigationService> navigationMock;
		private Mock<IMessageService> messageMock;
		HomeViewModel viewModel;
		Project project;

		[SetUp]
		public void Setup()
		{
			navigationMock = new Mock<INavigationService>();
			messageMock = new Mock<IMessageService>();

			viewModel = new HomeViewModel(navigationMock.Object, messageMock.Object);
			mockProject();
		}

		private void mockProject()
		{
			project = new Project();
			project.id = 1;

			viewModel.SelectedProject = project;
		}

		[Test()]
		public void TimeneyeUrlShouldNotContainsEndTime()
		{
			viewModel.SearchPeriod = false;

			string url = viewModel.buildUrl("https://track.timeneye.com/api/3/entries/?limit=10000");
			string expectedUrl = string.Format("https://track.timeneye.com/api/3/entries/?limit=10000&projectId={0}&dateFrom=2010/01/01", project.id);


			Assert.AreEqual(expectedUrl, url);
		}

		[Test()]
		public void TimeneyeUrlShouldContainsEndTime()
		{
			viewModel.SearchPeriod = true;
			viewModel.InitialDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
			viewModel.EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 30);

			string url = viewModel.buildUrl("https://track.timeneye.com/api/3/entries/?limit=10000");
			string expectedUrl = string.Format("https://track.timeneye.com/api/3/entries/?limit=10000&projectId={0}&dateFrom={1}&dateTo={2}",
											   project.id,
											   new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy/MM/dd"),
											  new DateTime(DateTime.Now.Year, DateTime.Now.Month, 30).ToString("yyyy/MM/dd"));


			Assert.AreEqual(expectedUrl, url);
		}

		[Test()]
		public void ShouldReturnTwoHoursBasedOn120Minutes()
		{
			string hours = viewModel.calcHours(120);
			Assert.AreEqual("2:00", hours);
		}

		[Test()]
		public void ShouldReturnTwoHoursAnd30MinutesBasedOn150Minutes()
		{
			string hours = viewModel.calcHours(150);
			Assert.AreEqual("2:30", hours);
		}

		[Test()]
		public void ShouldGroupResultsBasedOnUserName()
		{
			List<Record> mockRecordsList = new List<Record>();
			mockRecordsList.Add(new Record { id = 1, projectId = 1, userName = "João", minutes = 10, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) });
			mockRecordsList.Add(new Record { id = 2, projectId = 1, userName = "João", minutes = 30, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 2) });
			mockRecordsList.Add(new Record { id = 3, projectId = 2, userName = "João", minutes = 10, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) });
			mockRecordsList.Add(new Record { id = 4, projectId = 1, userName = "Maria", minutes = 120, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 3) });
			mockRecordsList.Add(new Record { id = 5, projectId = 2, userName = "Maria", minutes = 300, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 3) });
			mockRecordsList.Add(new Record { id = 6, projectId = 2, userName = "Maria", minutes = 10, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 4) });

			var groupedList = viewModel.groupRecords(mockRecordsList);

			Assert.AreEqual("João", groupedList[0].userName);
			Assert.AreEqual("Maria", groupedList[1].userName);
			Assert.AreEqual(50, groupedList[0].minutes);
			Assert.AreEqual(430, groupedList[1].minutes);
		}

		[Test()]
		public void ShouldCalcProjectCost()
		{
			List<Record> mockRecordsList = new List<Record>();
			mockRecordsList.Add(new Record { id = 1, projectId = 1, userName = "João", minutes = 10, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) });
			mockRecordsList.Add(new Record { id = 2, projectId = 1, userName = "João", minutes = 30, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 2) });
			mockRecordsList.Add(new Record { id = 3, projectId = 1, userName = "João", minutes = 10, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) });
			mockRecordsList.Add(new Record { id = 4, projectId = 1, userName = "Maria", minutes = 120, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 3) });
			mockRecordsList.Add(new Record { id = 5, projectId = 1, userName = "Maria", minutes = 300, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 3) });
			mockRecordsList.Add(new Record { id = 6, projectId = 1, userName = "Maria", minutes = 10, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 4) });

			List<Employe> mockEmployeList = new List<Employe>();
			mockEmployeList.Add(new Employe { Cost = 100.00, Name = "João" }); //Cost = (TotalMinutes (50) / 60) * Cost (100.00) =  83.34
			mockEmployeList.Add(new Employe { Cost = 250.75, Name = "Maria" }); //Cost = (TotalMinutes (430) / 60) * Cost (250.75) = 1797,04

			viewModel.SumValues(mockRecordsList, mockEmployeList);


			double totalCost = 1880.38;

			Assert.AreEqual("Total of minutes: 480", viewModel.TotalMinutes);
			Assert.AreEqual("Total of hours: 8:00", viewModel.TotalHours);
			Assert.AreEqual(string.Format("Total invested: {0}", totalCost.ToString("C")), viewModel.TotalCost);

		}

		[Test()]
		public void ShouldThrowExceptionWhenCalcProjectCost()
		{
			List<Record> mockRecordsList = new List<Record>();
			mockRecordsList.Add(new Record { id = 1, projectId = 1, userName = "João", minutes = 10, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) });
			mockRecordsList.Add(new Record { id = 2, projectId = 1, userName = "João", minutes = 30, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 2) });
			mockRecordsList.Add(new Record { id = 3, projectId = 1, userName = "João", minutes = 10, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) });
			mockRecordsList.Add(new Record { id = 4, projectId = 1, userName = "Maria", minutes = 120, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 3) });
			mockRecordsList.Add(new Record { id = 5, projectId = 1, userName = "Maria", minutes = 300, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 3) });
			mockRecordsList.Add(new Record { id = 6, projectId = 1, userName = "José", minutes = 10, entryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 4) });

			List<Employe> mockEmployeList = new List<Employe>();
			mockEmployeList.Add(new Employe { Cost = 100.00, Name = "João" });
			mockEmployeList.Add(new Employe { Cost = 250.75, Name = "Maria" });

			try
			{
				viewModel.SumValues(mockRecordsList, mockEmployeList);
				Assert.Fail("An exception should have been thrown");
			}
			catch (Exception e)
			{
				Assert.AreEqual("Employee not found: JOSÉ", e.Message);
			}
		}
	}
}

