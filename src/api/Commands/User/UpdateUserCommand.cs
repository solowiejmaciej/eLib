using System.Text.Json.Serialization;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Commands.User;

public record UpdateUserCommand : IRequest<Result<Unit, Error>>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Surname).NotEmpty();
        RuleFor(x => x.Email).NotEmpty()
            .EmailAddress();
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\d{6,12}$");
    }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<Unit, Error>>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<Unit, Error>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (user == null)
        {
            return UserErrors.NotFound;
        }

        user.Update(request.Name, request.Surname, request.Email, request.PhoneNumber);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
