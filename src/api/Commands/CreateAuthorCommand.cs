using eLib.DAL.Entities;
using eLib.DAL.Repositories;
using eLib.Models.Results.Base;
using eLib.Services;
using FluentValidation;
using MediatR;

namespace eLib.Commands;

public record CreateAuthorCommand() : IRequest<Result<Guid, Error>>
{
    public string Name { get; init; }
    public string Surname { get; init; }
    public DateTime Birthday { get; init; }
    public string Biography { get; init; }
    public string PhotoUrl { get; init; }
}

public class CreateAuthorValidator : AbstractValidator<CreateAuthorCommand>
{
    public CreateAuthorValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Surname).NotEmpty();
        RuleFor(x => x.Birthday).NotEmpty();
        RuleFor(x => x.Biography).NotEmpty();
        RuleFor(x => x.PhotoUrl).NotEmpty();
    }
}

public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, Result<Guid, Error>>
{
    private readonly IAuthorRepository _authorRepository;

    public CreateAuthorCommandHandler(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<Result<Guid, Error>> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = Author.Create(
            request.Name,
            request.Surname,
            request.Birthday,
            request.Biography,
            request.PhotoUrl
        );

        await _authorRepository.AddAsync(author, cancellationToken);
        await _authorRepository.SaveChangesAsync(cancellationToken);

        return author.Id;
    }
}