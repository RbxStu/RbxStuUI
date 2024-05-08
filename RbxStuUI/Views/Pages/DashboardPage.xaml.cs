using RbxStuUI.Services;
using RbxStuUI.ViewModels.Pages;

using Wpf.Ui.Controls;

namespace RbxStuUI.Views.Pages {
    public partial class DashboardPage : INavigableView<DashboardViewModel> {
        public DashboardViewModel ViewModel { get; }

        public DashboardPage(DashboardViewModel viewModel) {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        private async void OnInjectionButtonClicked(object sender, RoutedEventArgs e) {
            await ViewModel.InjectCommand.ExecuteAsync(sender);
        }
    }
}
