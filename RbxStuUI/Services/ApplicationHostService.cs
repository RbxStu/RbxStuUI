using System.Windows.Navigation;

using Microsoft.Extensions.Hosting;

using RbxStuUI.Views.Windows;

using Wpf.Ui;

namespace RbxStuUI.Services {
    /// <summary>
    /// Managed host of the application.
    /// </summary>
    public class ApplicationHostService : IHostedService {
        private readonly IServiceProvider _serviceProvider;

        private INavigationWindow _navigationWindow;
        private Loader _loaderWindow;

        public ApplicationHostService(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public async Task StartAsync(CancellationToken cancellationToken) {
            await HandleActivationAsync();
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public async Task StopAsync(CancellationToken cancellationToken) {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Creates main window during activation.
        /// </summary>
        private async Task HandleActivationAsync() {
            if (!Application.Current.Windows.OfType<Loader>().Any()) {
                _loaderWindow = (Loader) _serviceProvider.GetService(typeof(ILoader));
                _loaderWindow.Show();
                _loaderWindow.Closed += (object? sender, EventArgs args) => {
                    _navigationWindow = (
                        _serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow
                    )!; // Main window
                    _navigationWindow.ShowWindow();
                    _navigationWindow.Navigate(typeof(Views.Pages.DashboardPage));
                };

            }

            await Task.CompletedTask;
        }
    }
}
