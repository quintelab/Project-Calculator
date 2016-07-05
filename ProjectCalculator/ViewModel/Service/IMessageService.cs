using System.Threading.Tasks;

namespace ProjectCalculator.ViewModel.Service
{
	public interface IMessageService
	{
		Task ShowMessageAsync(string title, string message);
		void ShowMessage(string title, string message);
	}
}