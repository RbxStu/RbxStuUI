using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

using RbxStuUI.Services;

using Wpf.Ui.Controls;

namespace RbxStuUI.ViewModels.Pages;
public partial class InformationalCue : ObservableObject {
    public InformationalCue(string message, InfoBarSeverity severity) {
        Message = message;
        Severity = severity;
    }

    [ObservableProperty]
    private string message;

    [ObservableProperty]
    private InfoBarSeverity severity;
}
public partial class DashboardViewModel : ObservableObject {
    private readonly InjectionService m_injectionService;

    public DashboardViewModel(InjectionService injectionService) {
        m_injectionService = injectionService;
        InformationalCues = [
            new ("Goodbye!", InfoBarSeverity.Warning),
            new ("Hello!", InfoBarSeverity.Informational),
        ];
    }

    [ObservableProperty]
    private ObservableCollection<InformationalCue> _informationalCues;

    [RelayCommand]
    private async Task Inject() {
        if (!m_injectionService.CanInject()) {
            var msgBox = new Wpf.Ui.Controls.MessageBox();

            msgBox.Title = "Missing DLL/Roblox Studio not found";
            msgBox.Content = "You are missing either RbxStu's Module or you are not running a Roblox Studio instance.";
            msgBox.Activate();
            await msgBox.ShowDialogAsync();
            return;
        }
    }
}
