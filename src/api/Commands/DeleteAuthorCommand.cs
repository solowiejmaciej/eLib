using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Commands;

public record DeleteAuthorCommand(Guid Id) : IRequest<Result<Unit, Error>>;

public class DeleteAuthorCommandValidator : AbstractValidator<DeleteAuthorCommand>
{
    public DeleteAuthorCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class DeleteAuthorCommandHandler : IRequestHandler<DeleteAuthorCommand, Result<Unit, Error>>
{
    private readonly IAuthorRepository _authorRepository;

    public DeleteAuthorCommandHandler(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<Result<Unit, Error>> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (author == null)
        {
            return AuthorErrors.NotFound;
        }
        await _authorRepository.DeleteAsync(author.Id, cancellationToken);
        await _authorRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}