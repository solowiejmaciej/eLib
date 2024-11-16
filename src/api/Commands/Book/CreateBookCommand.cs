using eLib.Models.Results.Base;
using eLib.Services;
using FluentValidation;
using MediatR;

namespace eLib.Commands.Book;

public record CreateBookCommand() : IResultCommand<Guid>
{
    public string Title { get; init; }
    public Guid AuthorId { get; init; }
    public string Description { get; init; }
    public string CoverImageUrl { get; init; }
    public int Quantity { get; set; }
}

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.AuthorId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.CoverImageUrl).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public sealed class CreateBookCommandHandler : IResultCommandHandler<CreateBookCommand, Guid>
{
    private readonly IBookService _bookService;

    public CreateBookCommandHandler(
        IBookService bookService
        )
    {
        _bookService = bookService;
    }
    public async Task<Result<Guid, Error>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var result = await _bookService.AddBookWithDetails(request, cancellationToken);
        return result;
    }
}