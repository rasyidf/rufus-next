using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Rufus.Contracts.Services;
using Rufus.Contracts.ViewModels;
using Rufus.Models;

namespace Rufus.ViewModels;

public partial class MainViewModel : ObservableRecipient, INavigationAware
{
    private readonly IDriveService _driveService;

    public ObservableCollection<UsbDrive> Drives { get; } = new();

    [ObservableProperty]
    private UsbDrive? selectedDrive;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private bool listUsbHardDrives;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasStatusMessage))]
    private string? statusMessage;

    public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

    public MainViewModel(IDriveService driveService)
    {
        _driveService = driveService;
    }

    public void OnNavigatedTo(object parameter) => RefreshCommand.Execute(null);

    public void OnNavigatedFrom()
    {
    }

    partial void OnListUsbHardDrivesChanged(bool value) => RefreshCommand.Execute(null);

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (IsRefreshing)
        {
            return;
        }

        IsRefreshing = true;

        try
        {
            // Hold onto the selection so that refreshing does not move the user off
            // the device they had chosen, as long as it is still attached.
            var previouslySelected = SelectedDrive?.DeviceId;

            var drives = await _driveService.GetDrivesAsync(ListUsbHardDrives);

            Drives.Clear();

            foreach (var drive in drives)
            {
                Drives.Add(drive);
            }

            SelectedDrive = Drives.FirstOrDefault(drive => drive.DeviceId == previouslySelected) ?? Drives.FirstOrDefault();

            StatusMessage = Drives.Count == 0
                ? ListUsbHardDrives ? "No USB devices detected." : "No removable USB devices detected."
                : null;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Could not enumerate devices: {ex.Message}";
        }
        finally
        {
            IsRefreshing = false;
        }
    }
}
