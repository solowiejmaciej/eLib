using eLib.NotificationService.DAL;
using eLib.NotificationService.DAL.Pagination;
using eLib.NotificationService.Models;
using eLib.NotificationService.Notifications.Dtos;
using MediatR;

namespace eLib.NotificationService.Queries;

public record GetNotificationByUserIdQuery(Guid Id, PaginationParameters PaginationParameters) : IRequest<PaginationResult<NotificationDto>>;

public class GetNotificationByUserIdQueryHandler : IRequestHandler<GetNotificationByUserIdQuery, PaginationResult<NotificationDto>>
{
    private readonly INotificationRepository _repository;

    public GetNotificationByUserIdQueryHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResult<NotificationDto>> Handle(GetNotificationByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var notifications = await _repository.GetPaginatedForUserAsync(request.Id, request.PaginationParameters ,cancellationToken);
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