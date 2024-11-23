using eLib.NotificationService.DAL;
using eLib.NotificationService.Notifications.Dtos;
using MediatR;

namespace eLib.NotificationService.Queries;

public record GetAllNotificationsQuery : IRequest<IEnumerable<NotificationDto>>;


public class GetAllNotificationsQueryHandler : IRequestHandler<GetAllNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly INotificationRepository _notificationRepository;

    public GetAllNotificationsQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _notificationRepository.GetAsync(cancellationToken);
        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Message = n.Message,
            Title = n.Title,
            UserId = n.UserId,
            CreatedAt = n.CreatedAt,
            FailedAt = n.FailedAt,
            SentAt = n.SentAt,
        });
    }
}