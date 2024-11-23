using eLib.NotificationService.DAL;
using MediatR;

namespace eLib.NotificationService.Commands;

public record DeleteNotificationCommand(Guid Id) : IRequest;

public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand>
{
    private readonly INotificationRepository _repository;

    public DeleteNotificationCommandHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}