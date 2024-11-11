using eLib.Models.Dtos;

namespace eLib.DAL.Entities;

public class AuthorDetails : Entity
{
    private AuthorDetails() : base(Guid.NewGuid()) { }

    private AuthorDetails(
        string biography,
        string photoUrl) : base(Guid.NewGuid())
    {
        Biography = biography;
        PhotoUrl = photoUrl;
    }

    public string Biography { get; private set; }
    public string PhotoUrl { get; private set; }
    public Guid AuthorId { get; private set; }

    public static AuthorDetails Create(string biography, string photoUrl)
    {
        var authorDetails = new AuthorDetails(biography, photoUrl);
        return authorDetails;
    }

    public void SetAuthorId(Guid authorId)
    {
        AuthorId = authorId;
    }

    public AuthorDetailsDto MapToDto()
        => new()
        {
            Id = Id,
            Biography = Biography,
            PhotoUrl = PhotoUrl
        };

    public void Update(string biography, string photoUrl)
    {
        Biography = biography;
        PhotoUrl = photoUrl;
    }
}