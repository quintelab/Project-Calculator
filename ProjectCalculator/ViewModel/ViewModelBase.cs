using System.Runtime.CompilerServices;
using ProjectCalculator.ViewModel.Service;
using Xamarin.Forms;

namespace ProjectCalculator.ViewModel
{
	public abstract class ViewModelBase : XLabs.Forms.Mvvm.ViewModel
	{
		protected readonly INavigationService _navigationService;
		protected readonly IMessageService _messageService;

		public ViewModelBase(INavigationService navigationService, IMessageService messageService)
		{
			this._navigationService = navigationService;
			this._messageService = messageService;
		}

		protected void Notify([CallerMemberName] string propertyName = "")
		{
			NotifyPropertyChanged(propertyName);
		}
	}
}