namespace DeviceOwnership.Core.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? Channel { get; set; } // push, email, sms, in_app
    public string Priority { get; set; } = "normal"; // low, normal, high, urgent
    public string? ActionUrl { get; set; }
    public string? Data { get; set; } // JSON
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }

    // Navigation property
    public virtual User User { get; set; } = null!;
}
