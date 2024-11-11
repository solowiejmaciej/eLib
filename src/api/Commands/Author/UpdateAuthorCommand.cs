using System.Text.Json.Serialization;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Commands.Author;

public record UpdateAuthorCommand() : IRequest<Result<Unit, Error>>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime Birthday { get; set; }
    public string Biography { get; set; }
    public string PhotoUrl { get; set; }
}

public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, Result<Unit, Error>>
{
    private readonly IAuthorRepository _authorRepository;

    public UpdateAuthorCommandHandler(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<Result<Unit, Error>> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetByIdWithDetails(request.Id, cancellationToken);
        if (author == null)
            return AuthorErrors.NotFound;

        author.Update(request);

        await _authorRepository.UpdateAsync(author, cancellationToken);
        await _authorRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}