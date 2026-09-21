using Rufus.Models;

namespace Rufus.Contracts.Services;

public interface IDriveService
{
    /// <summary>
    /// Enumerates connected USB storage devices.
    /// </summary>
    /// <param name="includeHardDrives">
    /// When false, only removable media is returned. When true, USB-attached hard
    /// disks are included as well — the equivalent of Rufus's "List USB Hard Drives".
    /// </param>
    Task<IReadOnlyList<UsbDrive>> GetDrivesAsync(bool includeHardDrives = false, CancellationToken cancellationToken = default);
}
