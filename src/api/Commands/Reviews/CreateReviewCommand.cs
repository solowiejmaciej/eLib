using eLib.Auth.Providers;
using eLib.DAL.Entities;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;

namespace eLib.Commands.Reviews;

public record CreateReviewCommand(Guid BookId, string Content, int Rating) : IResultCommand<Guid>;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5);
    }
}

public class CreateReviewCommandHandler : IResultCommandHandler<CreateReviewCommand, Guid>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserInfoProvider _userInfoProvider;
    private readonly IBookRepository _bookRepository;

    public CreateReviewCommandHandler(
        IReviewRepository reviewRepository,
        IUserRepository userRepository,
        IUserInfoProvider userInfoProvider,
        IBookRepository bookRepository)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _userInfoProvider = userInfoProvider;
        _bookRepository = bookRepository;
    }

    public async Task<Result<Guid, Error>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {

        var userId = _userInfoProvider.GetCurrentUserID();
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return UserErrors.NotFound;
        }

        var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book == null)
        {
            return BookErrors.NotFound;
        }

        var result = Review.Create(request.Content, request.Rating, request.BookId, userId);
        if (result.HasError())
        {
            return result.Error!;
        }

        await _reviewRepository.AddAsync(result.Value!, cancellationToken);
        await _reviewRepository.SaveChangesAsync(cancellationToken);

        return result.Value!.Id;
    }
}