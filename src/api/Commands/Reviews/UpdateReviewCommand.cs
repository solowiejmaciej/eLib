using System.Text.Json.Serialization;
using eLib.Auth.Providers;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Commands.Reviews;

public record UpdateReviewCommand() : IResultCommand<Unit>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Content { get; set; }
    public int Rating { get; set; }
}

public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5);
    }
}

public class UpdateReviewCommandHandler : IResultCommandHandler<UpdateReviewCommand, Unit>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserInfoProvider _userInfoProvider;

    public UpdateReviewCommandHandler(
        IReviewRepository reviewRepository,
        IUserRepository userRepository,
        IUserInfoProvider userInfoProvider)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _userInfoProvider = userInfoProvider;
    }

    public async Task<Result<Unit, Error>> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = _userInfoProvider.GetCurrentUserID();
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return UserErrors.NotFound;
        }

        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review == null)
        {
            return ReviewErrors.NotFound;
        }

        var result = review.Update(request.Content, request.Rating);
        if (result.HasError())
        {
            return result.Error!;
        }

        await _reviewRepository.UpdateAsync(review, cancellationToken);
        await _reviewRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}