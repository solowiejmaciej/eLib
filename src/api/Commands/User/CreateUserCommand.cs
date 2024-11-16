using eLib.DAL.Repositories;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Commands.User;

public record CreateUserCommand : IResultCommand<Guid>
{
    public string Name { get; init; }
    public string Surname { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; init; }
    public string Password { get; init; }
}

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Surname).NotEmpty();
        RuleFor(x => x.Email).NotEmpty()
            .EmailAddress();
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\d{6,12}$")
            .WithMessage("Phone number must be between 6 and 12 digits.");
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(50);
    }
}

public class CreateUserCommandHandler : IResultCommandHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<Guid, Error>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = DAL.Entities.User.Create(request.Name, request.Surname, request.Email, request.Password, request.PhoneNumber);
        var userId = await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
        return userId;
    }
}