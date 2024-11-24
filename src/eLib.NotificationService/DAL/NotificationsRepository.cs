using eLib.NotificationService.DAL.Pagination;
using eLib.NotificationService.Models;
using eLib.NotificationService.Services;

namespace eLib.NotificationService.DAL;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationsDbContext _context;
    private readonly IPaginationService _paginationService;

    public NotificationRepository(
        NotificationsDbContext context,
        IPaginationService paginationService)
    {
        _context = context;
        _paginationService = paginationService;
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken)
        => await _context.Notifications.AddAsync(notification, cancellationToken);

    public Task<PaginationResult<Notification>> GetPaginatedAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken)
    {
        var query = _context.Notifications.AsQueryable();
        return _paginationService.GetPaginatedResultAsync(query, paginationParameters, cancellationToken);
    }

    public Task<PaginationResult<Notification>> GetPaginatedForUserAsync(Guid userId, PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        var query = _context.Notifications.Where(x => x.UserId == userId);
        return _paginationService.GetPaginatedResultAsync(query, paginationParameters, cancellationToken);
    }

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
    Task<PaginationResult<Notification>> GetPaginatedAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken);
    Task<PaginationResult<Notification>> GetPaginatedForUserAsync(Guid userId, PaginationParameters paginationParameters, CancellationToken cancellationToken);
    Task DeleteAsync(Guid notificationId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}