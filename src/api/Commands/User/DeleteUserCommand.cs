using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Commands.User;

public record DeleteUserCommand(Guid Id) : IRequest<Result<Unit, Error>>;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<Unit, Error>>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<Unit, Error>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            return UserErrors.NotFound;
        }

        await _userRepository.DeleteAsync(user.Id, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}