using eLib.NotificationService.DAL;
using eLib.NotificationService.Notifications.Dtos;
using MediatR;

namespace eLib.NotificationService.Queries;

public record GetNotificationByUserIdQuery(Guid Id) : IRequest<IEnumerable<NotificationDto>>;

public class GetNotificationByUserIdQueryHandler : IRequestHandler<GetNotificationByUserIdQuery, IEnumerable<NotificationDto>>
{
    private readonly INotificationRepository _repository;

    public GetNotificationByUserIdQueryHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetNotificationByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var notifications = await _repository.GetForUserAsync(request.Id, cancellationToken);
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