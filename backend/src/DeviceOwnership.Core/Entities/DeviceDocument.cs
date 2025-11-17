namespace DeviceOwnership.Core.Entities;

public class DeviceDocument
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public string DocumentType { get; set; } = string.Empty; // receipt, warranty, manual, insurance
    public string DocumentUrl { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string? MimeType { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual Device Device { get; set; } = null!;
}
