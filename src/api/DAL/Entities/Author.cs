using eLib.DomainEvents;

namespace eLib.DAL.Entities;

public sealed class Author : AggregateRoot
{
    private Author(
        string name,
        string surname,
        DateTime birthday,
        Guid detailsId) : base(Guid.NewGuid())
    {
        Name = name;
        Surname = surname;
        Birthday = birthday;
        DetailsId = detailsId;
    }

    public string Name { get; private set; }
    public string Surname { get; private set; }
    public DateTime Birthday { get; private set; }
    public Guid DetailsId { get; private set; }
    public AuthorDetails Details { get; private set; }

    public static Author Create(string name, string surname, DateTime birthday, string biography, string photoUrl)
    {
        var authorDetails = AuthorDetails.Create(biography, photoUrl);
        var author = new Author(name, surname, birthday, authorDetails.Id);
        authorDetails.SetAuthorId(author.Id);

        author.RaiseDomainEvent(new AuthorCreatedEvent(author));

        return author;
    }
}