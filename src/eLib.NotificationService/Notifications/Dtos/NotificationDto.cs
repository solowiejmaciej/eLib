namespace eLib.NotificationService.Notifications.Dtos;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public Guid UserId { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public DateTime? SentAt { get; set; }
}