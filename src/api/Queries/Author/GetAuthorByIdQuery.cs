using eLib.Common.Dtos;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Queries.Author;

public record GetAuthorByIdQuery(Guid Id) : IResultQuery<AuthorDto>;

public class GetAuthorByIdValidator : AbstractValidator<GetAuthorByIdQuery>
{
    public GetAuthorByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class GetAuthorByIdHandler : IResultQueryHandler<GetAuthorByIdQuery, AuthorDto>
{
    private readonly IAuthorRepository _authorRepository;

    public GetAuthorByIdHandler(
        IAuthorRepository authorRepository
        )
    {
        _authorRepository = authorRepository;
    }

    public async Task<Result<AuthorDto, Error>> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetByIdWithDetails(request.Id, cancellationToken);
        if (author == null)
        {
            return AuthorErrors.NotFound;
        }
        return author.MapToDto();
    }
}