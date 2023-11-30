using Microsoft.UI.Xaml.Controls;

using Rufus.ViewModels;

namespace Rufus.Views;

public sealed partial class LogsPage : Page
{
    public LogsViewModel ViewModel
    {
        get;
    }

    public LogsPage()
    {
        ViewModel = App.GetService<LogsViewModel>();
        InitializeComponent();
    }
}
