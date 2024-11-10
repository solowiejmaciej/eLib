using System.Text.Json.Serialization;
using eLib.Models.Results.Base;
using eLib.Services;
using FluentValidation;
using MediatR;

namespace eLib.Commands;

public record UpdateBookCommand : IRequest<Result<Unit, Error>>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Title { get; init; }
    public Guid AuthorId { get; init; }
    public string Description { get; init; }
    public string CoverImageUrl { get; init; }
    public int Quantity { get; set; }
}

public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id).Empty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.AuthorId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.CoverImageUrl).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public sealed class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, Result<Unit, Error>>
{
    private readonly IBookService _bookService;

    public UpdateBookCommandHandler(
        IBookService bookService
        )
    {
        _bookService = bookService;
    }
    public async Task<Result<Unit, Error>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var result = await _bookService.UpdateBookWithDetails(request, cancellationToken);
        return result;
    }
}