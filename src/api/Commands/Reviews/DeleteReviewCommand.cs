using eLib.Auth.Providers;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Commands.Reviews;

public record DeleteReviewCommand : IResultCommand<Unit>
{
    public DeleteReviewCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; init; }
}

public class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
{
    public DeleteReviewCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class DeleteReviewCommandHandler : IResultCommandHandler<DeleteReviewCommand, Unit>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserInfoProvider _userInfoProvider;

    public DeleteReviewCommandHandler(IReviewRepository reviewRepository, IUserInfoProvider userInfoProvider)
    {
        _reviewRepository = reviewRepository;
        _userInfoProvider = userInfoProvider;
    }

    public async Task<Result<Unit, Error>> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review == null)
        {
            return ReviewErrors.NotFound;
        }

        var user = _userInfoProvider.GetCurrentUser();


        if (review.UserId != user.Id && !user.IsAdmin)
        {
            return UserErrors.NotAuthorized;
        }

        review.Delete();
        await _reviewRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}