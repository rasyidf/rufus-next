namespace Rufus.Models;

/// <summary>
/// A USB storage device as reported by WMI, along with the volumes mounted from it.
/// </summary>
public sealed record UsbDrive(
    string DeviceId,
    uint DiskNumber,
    string Model,
    ulong SizeBytes,
    string? VolumeLabel,
    bool IsRemovableMedia,
    IReadOnlyList<string> DriveLetters)
{
    /// <summary>
    /// Rufus-style summary, e.g. "UBUNTU 24_04 (E:) [14.4 GB]".
    /// </summary>
    public string DisplayName
    {
        get
        {
            var label = string.IsNullOrWhiteSpace(VolumeLabel) ? Model : VolumeLabel;
            var letters = DriveLetters.Count == 0 ? "no letter" : string.Join(", ", DriveLetters);

            return $"{label} ({letters}) [{FormatSize(SizeBytes)}]";
        }
    }

    public static string FormatSize(ulong bytes)
    {
        if (bytes == 0)
        {
            return "unknown size";
        }

        string[] units = { "B", "KB", "MB", "GB", "TB" };

        double value = bytes;
        var unit = 0;

        while (value >= 1024 && unit < units.Length - 1)
        {
            value /= 1024;
            unit++;
        }

        return value >= 100 ? $"{value:0} {units[unit]}" : $"{value:0.#} {units[unit]}";
    }
}
