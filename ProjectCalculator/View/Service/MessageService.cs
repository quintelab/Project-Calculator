using ProjectCalculator.ViewModel.Service;

namespace ProjectCalculator.View.Service
{
	public class MessageService : IMessageService
	{
		#region IMessageService implementation

		public async System.Threading.Tasks.Task ShowMessageAsync(string title, string message)
		{
			await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(title, message, "ok");
		}

		public void ShowMessage(string title, string message)
		{
			Xamarin.Forms.Application.Current.MainPage.DisplayAlert(title, message, "ok");
		}

		#endregion
	}
}