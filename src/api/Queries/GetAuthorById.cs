using eLib.DAL.Repositories;
using eLib.Models.Dtos;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Queries;

public record GetAuthorById(Guid Id) : IRequest<Result<AuthorDto, Error>>;

public class GetAuthorByIdValidator : AbstractValidator<GetAuthorById>
{
    public GetAuthorByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class GetAuthorByIdHandler : IRequestHandler<GetAuthorById, Result<AuthorDto, Error>>
{
    private readonly IAuthorRepository _authorRepository;

    public GetAuthorByIdHandler(
        IAuthorRepository authorRepository
        )
    {
        _authorRepository = authorRepository;
    }

    public async Task<Result<AuthorDto, Error>> Handle(GetAuthorById request, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetByIdWithDetails(request.Id, cancellationToken);
        if (author == null)
        {
            return AuthorErrors.NotFound;
        }
        return author.MapToDto();
    }
}