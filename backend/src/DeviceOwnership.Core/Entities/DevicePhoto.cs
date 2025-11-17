namespace DeviceOwnership.Core.Entities;

public class DevicePhoto
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string PhotoType { get; set; } = string.Empty; // device, serial_number, receipt, damage
    public bool IsPrimary { get; set; }
    public string? Caption { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual Device Device { get; set; } = null!;
}
