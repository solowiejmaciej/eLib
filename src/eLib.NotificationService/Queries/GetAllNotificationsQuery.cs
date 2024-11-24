using eLib.NotificationService.DAL;
using eLib.NotificationService.DAL.Pagination;
using eLib.NotificationService.Models;
using eLib.NotificationService.Notifications.Dtos;
using MediatR;

namespace eLib.NotificationService.Queries;

public record GetAllNotificationsQuery(PaginationParameters PaginationParameters) : IRequest<PaginationResult<NotificationDto>>;


public class
    GetAllNotificationsQueryHandler : IRequestHandler<GetAllNotificationsQuery, PaginationResult<NotificationDto>>
{
    private readonly INotificationRepository _notificationRepository;

    public GetAllNotificationsQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<PaginationResult<NotificationDto>> Handle(GetAllNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var notifications =
            await _notificationRepository.GetPaginatedAsync(request.PaginationParameters, cancellationToken);
        return new PaginationResult<NotificationDto>(notifications.Items.Select(x => new NotificationDto()
        {
            Id = x.Id,
            Title = x.Title,
            Message = x.Message,
            CreatedAt = x.CreatedAt,
            UserId = x.UserId,
            SentAt = x.SentAt,
            FailedAt = x.FailedAt,
            Channel = x.Channel
        }), notifications.TotalCount, notifications.PageNumber, notifications.PageSize);

    }
}