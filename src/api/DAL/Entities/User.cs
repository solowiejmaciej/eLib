using eLib.DomainEvents;
using eLib.Models.Dtos;

namespace eLib.DAL.Entities;

public class User : AggregateRoot
{
    private User() : base(Guid.NewGuid()) { }

    private User(
        string name,
        string surname,
        string email,
        string phoneNumber,
        UserDetails details) : base(Guid.NewGuid())
    {
        Name = name;
        Surname = surname;
        Email = email;
        PhoneNumber = phoneNumber;
        Details = details;
        DetailsId = details.Id;
    }

    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public Guid DetailsId { get; private set; }
    public UserDetails Details { get; private set; }

    public static User Create(string name, string surname, string email, string password, string phoneNumber)
    {
        var userDetails = UserDetails.Create(password, false, false, false, false, false);
        var user = new User(name, surname, email, phoneNumber, userDetails);
        userDetails.SetUserId(user.Id);

        user.RaiseDomainEvent(new UserCreatedEvent(user));
        return user;
    }


    public void Update(string name, string surname, string email, string phoneNumber)
    {
        Name = name;
        Surname = surname;

        if (Email != email)
        {
            Email = email;
            Details.DisableEmailNotifications();
            Details.MarkEmailAsUnverified();
        }

        if (PhoneNumber != phoneNumber)
        {
            PhoneNumber = phoneNumber;
            Details.DisableSmsNotifications();
            Details.MarkPhoneNumberAsUnverified();
        }
    }

    public UserDto MapToDto()
        => new()
        {
            Id = Id,
            Name = Name,
            Surname = Surname,
            Email = Email,
            PhoneNumber = PhoneNumber,
            Details = Details.MapToDto()
        };
}