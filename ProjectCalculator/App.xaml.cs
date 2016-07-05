using ProjectCalculator.View;
using ProjectCalculator.View.Service;
using ProjectCalculator.ViewModel;
using ProjectCalculator.ViewModel.Service;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using XLabs.Ioc;

namespace ProjectCalculator
{
	public partial class App : Application
	{
		public App()
		{
			if (!Resolver.IsSet)
				SetIoc();
			
			RegisterViews();
			
			MainPage = new NavigationPage((Page)ViewFactory.CreatePage<HomeViewModel, HomeView>());
		}

		private void RegisterViews()
		{
			ViewFactory.Register<HomeView, HomeViewModel>();
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}

		private void SetIoc()
		{
			var resolverContainer = new SimpleContainer();

			resolverContainer.Register<INavigationService, NavigationService>();
			resolverContainer.Register<IMessageService, MessageService>();
			resolverContainer.Register<HomeViewModel>(r => new HomeViewModel(r.Resolve<INavigationService>(), r.Resolve<IMessageService>()));

			Resolver.SetResolver(resolverContainer.GetResolver());
		}
	}
}

