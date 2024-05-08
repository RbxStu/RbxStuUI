using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

using RbxStuUI.Services;

using Wpf.Ui.Controls;

namespace RbxStuUI.ViewModels.Pages;

public partial class InformationalCue : ObservableObject {
    public InformationalCue(string title, string message, InfoBarSeverity severity, string cueId, bool showCue) {
        Title = title;
        Message = message;
        Severity = severity;
        CueId = cueId;
        ShowCue = showCue;
    }

    [ObservableProperty]
    private bool showCue;

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string message;

    [ObservableProperty]
    private string cueId;

    [ObservableProperty]
    private InfoBarSeverity severity;
}
public partial class DashboardViewModel : ObservableObject {
    public const string MODULE_UPDATE_STATUS = "module_update_status";
    public const string INJECTION_STATUS = "injection_status";
    private readonly InjectionService m_injectionService;

    public DashboardViewModel(InjectionService injectionService) {
        m_injectionService = injectionService;
        InformationalCues = [
            new ("Injection Status", "Not injected", InfoBarSeverity.Informational, INJECTION_STATUS, true),
            new ("Module Update Status", "Updated", InfoBarSeverity.Success, MODULE_UPDATE_STATUS, true),
        ];
    }

    [ObservableProperty]
    private ObservableCollection<InformationalCue> _informationalCues;

    [RelayCommand]
    private async Task Inject() {
        if (m_injectionService.IsInjected()) {
            var msgBox = new Wpf.Ui.Controls.MessageBox();

            msgBox.Title = "Already injected!";
            msgBox.Content = "You have already injected into Roblox Studio!";
            msgBox.Activate();
            await msgBox.ShowDialogAsync();
            return;
        }

        if (!m_injectionService.CanInject()) {
            var msgBox = new Wpf.Ui.Controls.MessageBox();

            msgBox.Title = "Missing DLL/Roblox Studio not found";
            msgBox.Content = "You are missing either RbxStu's Module or you are not running a Roblox Studio instance.";
            msgBox.Activate();
            await msgBox.ShowDialogAsync();
            return;
        }

        var injectionStatusCue = InformationalCues.First(x => x.CueId == INJECTION_STATUS);

        injectionStatusCue.Severity = InfoBarSeverity.Warning;
        injectionStatusCue.Message = "Attempting to inject into Roblox Studio...";

        await m_injectionService.InjectModule();

        await Task.Delay(300);

        if (m_injectionService.IsInjected()) {
            injectionStatusCue.Severity = InfoBarSeverity.Success;
            injectionStatusCue.Message = "Injected into Roblox Studio!";
        }
        else {
            injectionStatusCue.Severity = InfoBarSeverity.Error;
            injectionStatusCue.Message = $"Injection failed! -- Win32 Error -> {Marshal.GetLastWin32Error():X}";
        }

    }
}
