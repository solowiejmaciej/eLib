using eLib.DAL.Repositories;
using eLib.Models.Dtos;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Queries;

public record GetBookByIdQuery(Guid Id) : IRequest<Result<BookDto, Error>>;

public class GetBookByIdQueryValidator : AbstractValidator<GetBookByIdQuery>
{
    public GetBookByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, Result<BookDto, Error>>
{
    private readonly IBookRepository _bookRepository;

    public GetBookByIdQueryHandler(
        IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }


    public async Task<Result<BookDto, Error>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (book == null)
        {
            return BookErrors.NotFound;
        }

        return book.MapToDto();
    }
}