using eLib.NotificationService.Notifications;
using Microsoft.EntityFrameworkCore;

namespace eLib.NotificationService.DAL;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationsDbContext _context;

    public NotificationRepository(NotificationsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken)
        => await _context.Notifications.AddAsync(notification, cancellationToken);

    public async Task<IEnumerable<Notification>> GetAsync(CancellationToken cancellationToken)
        => await _context.Notifications.ToListAsync(cancellationToken);

    public async Task<IEnumerable<Notification>> GetForUserAsync(Guid userId, CancellationToken cancellationToken)
        => await _context.Notifications.Where(n => n.UserId == userId).ToListAsync(cancellationToken);

    public async Task DeleteAsync(Guid notificationId, CancellationToken cancellationToken)
    {
        var notification = await _context.Notifications.FindAsync(notificationId, cancellationToken);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
        }
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => _context.SaveChangesAsync(cancellationToken);
}

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken cancellationToken);
    Task<IEnumerable<Notification>> GetAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Notification>> GetForUserAsync(Guid userId, CancellationToken cancellationToken);
    Task DeleteAsync(Guid notificationId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}