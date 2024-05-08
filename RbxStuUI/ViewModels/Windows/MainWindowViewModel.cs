using System.Collections.ObjectModel;

using Wpf.Ui.Controls;

namespace RbxStuUI.ViewModels.Windows; 
public partial class MainWindowViewModel : ObservableObject {
    [ObservableProperty]
    private string _applicationTitle = "RbxStuUI";

    [ObservableProperty]
    private bool m_hasLoaded = false;

    [ObservableProperty]
    private ObservableCollection<object> _menuItems = new()
    {
        new NavigationViewItem()
        {
            Content = "RbxStu Dashboard - Control RbxStu",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
            TargetPageType = typeof(Views.Pages.DashboardPage)
        },
    };

    [ObservableProperty]
    private ObservableCollection<object> _footerMenuItems = new()
    {
        new NavigationViewItem()
        {
            Content = "Settings",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
            TargetPageType = typeof(Views.Pages.SettingsPage)
        }
    };

    [ObservableProperty]
    private ObservableCollection<MenuItem> _trayMenuItems = new()
    {
        new MenuItem { Header = "Dashboard", Tag = "tray_home" }
    };
}
