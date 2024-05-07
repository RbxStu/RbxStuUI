using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

using RbxStuUI.Services;
using RbxStuUI.Views.Windows;

using Wpf.Ui.Controls;

namespace RbxStuUI.ViewModels.Windows;
public partial class LoaderViewModel : ObservableObject {
    public LoaderViewModel(RbxStuService stuSvc) {
        m_rbxStuService = stuSvc;
    }

    private RbxStuService m_rbxStuService;
    private Loader m_loader => Loader.GetSingleton();

    [ObservableProperty]
    private string _applicationTitle = "RbxStuUI - Loader";
    [ObservableProperty]
    private string _statusText = "Loading";

    public async Task OnLoaderLoaded(object sender, RoutedEventArgs e) {
        if (!m_rbxStuService.InitialSetupCompleted()) {
            m_rbxStuService.CreateFolders();
            await m_rbxStuService.DownloadBinariesAsync((string status) => {
                StatusText = status;
                return Task.CompletedTask;
            });
            m_rbxStuService.MarkSetupCompleted();
        }

        // Validate workspace and other stuffz
        StatusText = "Verifying Dependencies 1/2...";

        if (m_rbxStuService.IsWorkspaceCorrupted()) {
            await m_loader.Dispatcher.Invoke(async () => {
                var msgBox = new Wpf.Ui.Controls.MessageBox();

                msgBox.Title = "Missing Workspace";
                msgBox.Content = "You are missing your workspace directory, your installation of RbxStu may have been messed up, do you wish to continue anyway and create a new one?";
                msgBox.IsPrimaryButtonEnabled = true;
                msgBox.IsSecondaryButtonEnabled = true;
                msgBox.PrimaryButtonText = "No";
                msgBox.SecondaryButtonText = "Yes";
                msgBox.Activate();
                var msgBoxResult = await msgBox.ShowDialogAsync();
                switch (msgBoxResult) {
                    case Wpf.Ui.Controls.MessageBoxResult.Primary: {
                        StatusText = "Creating Workspace...";
                        Directory.CreateDirectory("./workspace");
                        break;
                    }
                    case Wpf.Ui.Controls.MessageBoxResult.Secondary: {
                        Environment.Exit(0);
                        break;
                    }
                }
            });
        }

        StatusText = "Verifying Dependencies 2/2...";

        if (m_rbxStuService.AreBinariesCorrupted()) {
            await m_loader.Dispatcher.Invoke(async () => {
                var msgBox = new Wpf.Ui.Controls.MessageBox();

                msgBox.Title = "Missing Workspace";
                msgBox.Content = "You are missing the native binaries required for the UI to function. Do you wish to re-download them?";
                msgBox.IsPrimaryButtonEnabled = true;
                msgBox.IsSecondaryButtonEnabled = true;
                msgBox.PrimaryButtonText = "No";
                msgBox.SecondaryButtonText = "Yes";
                msgBox.Activate();
                var msgBoxResult = await msgBox.ShowDialogAsync();
                switch (msgBoxResult) {
                    case Wpf.Ui.Controls.MessageBoxResult.Primary: {
                        StatusText = "Creating Workspace...";
                        Directory.CreateDirectory("./workspace");
                        break;
                    }
                    case Wpf.Ui.Controls.MessageBoxResult.Secondary: {
                        Environment.Exit(0);
                        break;
                    }
                }
            });
        }

        StatusText = "Loading UI";

        m_loader.Dispatcher.Invoke(() => {
            m_loader.Close();
        });
    }
}
