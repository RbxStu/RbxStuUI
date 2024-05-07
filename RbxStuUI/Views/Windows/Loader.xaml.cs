using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using RbxStuUI.ViewModels.Windows;

using Wpf.Ui;
using Wpf.Ui.Controls;

namespace RbxStuUI.Views.Windows;
/// <summary>
/// Interaction logic for Loader.xaml
/// </summary>
public partial class Loader : FluentWindow, ILoader {
    public static Loader m_singleton;
    public LoaderViewModel ViewModel { get; set; }
    public Loader(
        LoaderViewModel viewModel
    ) {
        DataContext = ViewModel = viewModel;
        InitializeComponent();
        m_singleton = this;
    }

    public static Loader GetSingleton() => m_singleton;

    private void OnLoaded(object sender, RoutedEventArgs e) {
        Task.Run(async () => {
            await ViewModel.OnLoaderLoaded(sender, e);
        });
    }
}
