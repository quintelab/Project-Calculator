using System.Threading.Tasks;
using ProjectCalculator.ViewModel.Service;

namespace ProjectCalculator.View.Service
{
	public class NavigationService : INavigationService
	{
		public async Task NavigateToHome()
		{
			await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new HomeView());
		}
	}
}