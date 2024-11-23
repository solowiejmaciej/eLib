using eLib.Commands;
using eLib.Commands.Author;
using eLib.Common.Dtos;
using eLib.DomainEvents;

namespace eLib.DAL.Entities;

public sealed class Author : AggregateRoot
{
    private Author() : base(Guid.NewGuid()) { }

    private Author(
        string name,
        string surname,
        DateTime birthday,
        AuthorDetails details) : base(Guid.NewGuid())
    {
        Name = name;
        Surname = surname;
        Birthday = birthday;
        Details = details;
        DetailsId = details.Id;
    }

    public string Name { get; private set; }
    public string Surname { get; private set; }
    public DateTime Birthday { get; private set; }
    public Guid DetailsId { get; private set; }
    public AuthorDetails Details { get; private set; }

    public static Author Create(string name, string surname, DateTime birthday, string biography, string photoUrl)
    {
        var authorDetails = AuthorDetails.Create(biography, photoUrl);
        var author = new Author(name, surname, birthday, authorDetails);
        authorDetails.SetAuthorId(author.Id);

        author.RaiseDomainEvent(new AuthorCreatedEvent(author));

        return author;
    }

    public void Update(UpdateAuthorCommand request)
    {
        Name = request.Name;
        Surname = request.Surname;
        Birthday = request.Birthday;
        Details.Update(request.Biography, request.PhotoUrl);
    }

    public AuthorDto MapToDto()
        => new()
        {
            Id = Id,
            Name = Name,
            Surname = Surname,
            Birthday = Birthday,
            Details = Details.MapToDto()
        };
}