using System.Management;

using Rufus.Contracts.Services;
using Rufus.Models;

namespace Rufus.Services;

/// <summary>
/// Enumerates USB storage devices through WMI.
/// </summary>
/// <remarks>
/// WMI is synchronous and can block for seconds when a device is spinning up or
/// unresponsive, so every query runs on a background thread.
/// </remarks>
public sealed class DriveService : IDriveService
{
    public Task<IReadOnlyList<UsbDrive>> GetDrivesAsync(bool includeHardDrives = false, CancellationToken cancellationToken = default)
        => Task.Run<IReadOnlyList<UsbDrive>>(() => Enumerate(includeHardDrives, cancellationToken), cancellationToken);

    private static List<UsbDrive> Enumerate(bool includeHardDrives, CancellationToken cancellationToken)
    {
        var drives = new List<UsbDrive>();

        using var searcher = new ManagementObjectSearcher(
            "SELECT DeviceID, Index, Model, Size, MediaType FROM Win32_DiskDrive WHERE InterfaceType = 'USB'");

        foreach (var disk in searcher.Get().OfType<ManagementObject>())
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (disk)
            {
                // Flash media reports "Removable Media"; USB-attached spinning and
                // solid state disks report "External hard disk media" or, on some
                // enclosures, plain "Fixed hard disk media".
                var mediaType = disk["MediaType"] as string ?? string.Empty;
                var isRemovable = mediaType.Contains("Removable", StringComparison.OrdinalIgnoreCase);

                if (!isRemovable && !includeHardDrives)
                {
                    continue;
                }

                var deviceId = disk["DeviceID"] as string;

                if (string.IsNullOrEmpty(deviceId))
                {
                    continue;
                }

                var (letters, label) = GetVolumes(deviceId, cancellationToken);

                drives.Add(new UsbDrive(
                    DeviceId: deviceId,
                    DiskNumber: disk["Index"] is null ? 0 : Convert.ToUInt32(disk["Index"]),
                    Model: (disk["Model"] as string)?.Trim() is { Length: > 0 } model ? model : "Unknown device",
                    SizeBytes: disk["Size"] is null ? 0 : Convert.ToUInt64(disk["Size"]),
                    VolumeLabel: label,
                    IsRemovableMedia: isRemovable,
                    DriveLetters: letters));
            }
        }

        return drives.OrderBy(drive => drive.DiskNumber).ToList();
    }

    /// <summary>
    /// Walks Win32_DiskDrive -> Win32_DiskPartition -> Win32_LogicalDisk to find the
    /// drive letters and volume label backing a physical disk. A disk with no
    /// recognised filesystem has partitions but no logical disks, which is normal.
    /// </summary>
    private static (IReadOnlyList<string> Letters, string? Label) GetVolumes(string deviceId, CancellationToken cancellationToken)
    {
        var letters = new List<string>();
        string? label = null;

        // WQL string literals take a backslash as an escape character, so the
        // "\\.\PHYSICALDRIVE2" form has to be doubled before it is interpolated.
        var escapedDeviceId = deviceId.Replace("\\", "\\\\");

        using var partitionSearcher = new ManagementObjectSearcher(
            $"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{escapedDeviceId}'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition");

        foreach (var partition in partitionSearcher.Get().OfType<ManagementObject>())
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (partition)
            {
                var partitionId = partition["DeviceID"] as string;

                if (string.IsNullOrEmpty(partitionId))
                {
                    continue;
                }

                using var logicalDiskSearcher = new ManagementObjectSearcher(
                    $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partitionId}'}} WHERE AssocClass = Win32_LogicalDiskToPartition");

                foreach (var logicalDisk in logicalDiskSearcher.Get().OfType<ManagementObject>())
                {
                    using (logicalDisk)
                    {
                        if (logicalDisk["DeviceID"] is string letter && letter.Length > 0)
                        {
                            letters.Add(letter);
                        }

                        if (string.IsNullOrEmpty(label) && logicalDisk["VolumeName"] is string volumeName && volumeName.Length > 0)
                        {
                            label = volumeName;
                        }
                    }
                }
            }
        }

        return (letters, label);
    }
}
