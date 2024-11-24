using Bogus;
using eLib.Common.Notifications;
using eLib.DAL;
using eLib.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Serilog;

namespace eLib.Seeder;

public class Seeder : ISeeder
{
    private readonly LibraryDbContext _context;
    private readonly Faker<Author> _authorFaker;
    private readonly Dictionary<string, Func<List<Author>, Faker<Book>>> _categoryFakerFactories;
    private CancellationTokenSource _cancellationTokenSource;

    public Seeder(LibraryDbContext context)
    {
        _context = context;
        _authorFaker = CreateAuthorFaker();
        _categoryFakerFactories = new Dictionary<string, Func<List<Author>, Faker<Book>>>
        {
            ["Programming"] = authors => CreateProgrammingBookFaker(authors),
            ["SciFi"] = authors => CreateSciFiBookFaker(authors),
            ["History"] = authors => CreateHistoryBookFaker(authors),
            ["Cooking"] = authors => CreateCookingBookFaker(authors)
        };
    }

    private Faker<Author> CreateAuthorFaker()
    {
        return new Faker<Author>()
            .CustomInstantiator(f => Author.Create(
                f.Name.FirstName(),
                f.Name.LastName(),
                f.Date.Recent(50, DateTime.UtcNow.AddYears(-25)),
                f.Lorem.Paragraphs(3),
                f.Image.PicsumUrl()
            ));
    }

    private List<Book> GenerateBooks(int totalCount, List<Author> authors)
    {
        var books = new List<Book>();
        var categoryCounts = DistributeCount(totalCount, _categoryFakerFactories.Count);

        var i = 0;
        foreach (var fakerFactory in _categoryFakerFactories.Values)
        {
            var faker = fakerFactory(authors);
            books.AddRange(faker.Generate(categoryCounts[i]));
            i++;
        }

        return books;
    }

    private Faker<Book> CreateProgrammingBookFaker(List<Author> authors)
    {
        var technologies = new[] { "C#", ".NET", "Python", "JavaScript", "React", "Angular", "Node.js", "TypeScript" };
        var topics = new[] { "Advanced", "Beginners Guide", "In Practice", "Design Patterns", "Clean Code", "Performance" };

        return new Faker<Book>()
            .CustomInstantiator(f => Book.Create(
                $"{f.PickRandom(technologies)} {f.PickRandom(topics)}",
                f.PickRandom(authors).Id,
                f.Lorem.Paragraph() + "\n\n" + f.Lorem.Paragraphs(2),
                f.Image.PicsumUrl(),
                f.Random.Number(10, 30)
            ));
    }

    private Faker<Book> CreateSciFiBookFaker(List<Author> authors)
    {
        var prefixes = new[] { "The", "Rise of", "Return to", "Beyond", "Chronicles of" };
        var subjects = new[] { "Stars", "Future", "AI", "Cyberspace", "Time", "Space Colony", "Android" };

        return new Faker<Book>()
            .CustomInstantiator(f => Book.Create(
                $"{f.PickRandom(prefixes)} {f.PickRandom(subjects)}",
                f.PickRandom(authors).Id,
                f.Lorem.Paragraph() + "\n\n" + f.Lorem.Paragraphs(2),
                f.Image.PicsumUrl(),
                f.Random.Number(5, 25)
            ));
    }

    private Faker<Book> CreateHistoryBookFaker(List<Author> authors)
    {
        var eras = new[] { "Ancient", "Medieval", "Renaissance", "Modern", "World War II", "Cold War" };
        var topics = new[] { "Europe", "Asia", "Americas", "Africa", "Global", "Wars", "Society", "Culture" };

        return new Faker<Book>()
            .CustomInstantiator(f => Book.Create(
                $"{f.PickRandom(eras)} {f.PickRandom(topics)}: A History",
                f.PickRandom(authors).Id,
                f.Lorem.Paragraph() + "\n\n" + f.Lorem.Paragraphs(2),
                f.Image.PicsumUrl(),
                f.Random.Number(3, 15)
            ));
    }

    private Faker<Book> CreateCookingBookFaker(List<Author> authors)
    {
        var cuisines = new[] { "Italian", "French", "Asian", "Mediterranean", "Mexican", "Indian" };
        var types = new[] { "Cookbook", "Kitchen", "Recipes", "Cooking", "Chef's Guide" };

        return new Faker<Book>()
            .CustomInstantiator(f => Book.Create(
                $"{f.PickRandom(cuisines)} {f.PickRandom(types)}",
                f.PickRandom(authors).Id,
                f.Lorem.Paragraph() + "\n\n" + f.Lorem.Paragraphs(2),
                f.Image.PicsumUrl(),
                f.Random.Number(8, 40)
            ));
    }

    private int[] DistributeCount(int total, int parts)
    {
        var random = new Random();
        var numbers = new int[parts];
        var remaining = total;

        for (int i = 0; i < parts - 1; i++)
        {
            var current = random.Next(1, remaining - (parts - i - 1));
            numbers[i] = current;
            remaining -= current;
        }
        numbers[parts - 1] = remaining;

        return numbers;
    }

    public async Task SeedBooksOnlyAsync(int count, CancellationToken cancellationToken = default)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var authors = await _context.Authors.ToListAsync(_cancellationTokenSource.Token);
        var books = GenerateBooks(count, authors);

        var stopwatch = Stopwatch.StartNew();
        Log.Information($"Seeding {count} books...");

        await _context.Books.AddRangeAsync(books, _cancellationTokenSource.Token);
        await _context.SaveChangesAsync(_cancellationTokenSource.Token);

        stopwatch.Stop();
        Log.Information($"Seeding completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
    }

    public async Task SeedAuthorsOnlyAsync(int count, CancellationToken cancellationToken = default)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var authors = _authorFaker.Generate(count);

        var stopwatch = Stopwatch.StartNew();
        Log.Information($"Seeding {count} authors...");

        await _context.Authors.AddRangeAsync(authors, _cancellationTokenSource.Token);
        await _context.SaveChangesAsync(_cancellationTokenSource.Token);

        stopwatch.Stop();
        Log.Information($"Seeding completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
    }

    public async Task SeedBooksWithAuthorsAsync(int count, CancellationToken cancellationToken = default)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var authors = _authorFaker.Generate(Math.Max(count / 3, 1));
        var books = GenerateBooks(count, authors);

        var stopwatch = Stopwatch.StartNew();
        Log.Information($"Seeding {count} books with {authors.Count} authors...");

        await _context.Authors.AddRangeAsync(authors, _cancellationTokenSource.Token);
        await _context.Books.AddRangeAsync(books, _cancellationTokenSource.Token);
        await _context.SaveChangesAsync(_cancellationTokenSource.Token);

        stopwatch.Stop();
        Log.Information($"Seeding completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
    }

    public async Task SeedUsersAsync(int count, CancellationToken cancellationToken = default)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var stopwatch = Stopwatch.StartNew();
        Log.Information($"Seeding {count} users...");

        var users = new Faker<User>()
            .CustomInstantiator(f => User.Create(
                f.Name.FirstName(),
                f.Name.LastName(),
                f.Internet.Email(),
                "string",
                f.Phone.PhoneNumber(),
                ENotificationChannel.System
            ))
            .Generate(count);


        await _context.Users.AddRangeAsync(users, _cancellationTokenSource.Token);
        await _context.SaveChangesAsync(_cancellationTokenSource.Token);

        stopwatch.Stop();
        Log.Information($"Seeding completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
    }

    public async Task SeedReservationsAsync(int count, CancellationToken cancellationToken = default)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var users = await _context.Users.ToListAsync(_cancellationTokenSource.Token);
        var bookDetails = await _context.BookDetails.ToListAsync(_cancellationTokenSource.Token);

        var reservations = new Faker<Reservation>()
            .CustomInstantiator(f => Reservation.Create(
                f.PickRandom(bookDetails),
                f.PickRandom(users).Id,
                f.Date.Between(DateTime.UtcNow, DateTime.UtcNow.AddDays(-5)),
                f.Date.Between(DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddDays(60))
            ).Value!)
            .Generate(count);

        var stopwatch = Stopwatch.StartNew();
        Log.Information($"Seeding {count} reservations...");

        await _context.Reservations.AddRangeAsync(reservations, _cancellationTokenSource.Token);
        await _context.SaveChangesAsync(_cancellationTokenSource.Token);

        stopwatch.Stop();
        Log.Information($"Seeding completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
    }

    [SuppressMessage("ReSharper.DPA", "DPA0007: Large number of DB records", MessageId = "count: 500")]
    public async Task SeedUsersWithReservationsAsync(int count, CancellationToken cancellationToken = default)
    {
        await SeedUsersAsync(count, cancellationToken);

        var users = await _context.Users.ToListAsync(_cancellationTokenSource.Token);
        var bookDetails = await _context.BookDetails.ToListAsync(_cancellationTokenSource.Token);

        var reservations = new Faker<Reservation>()
            .CustomInstantiator(f => Reservation.Create(
                f.PickRandom(bookDetails),
                f.PickRandom(users).Id,
                f.Date.Between(DateTime.UtcNow, DateTime.UtcNow.AddDays(-5)),
                f.Date.Between(DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddDays(60))
            ).Value!)
            .Generate(count);

        var stopwatch = Stopwatch.StartNew();
        Log.Information($"Seeding {count} users with {count} reservations...");

        await _context.Reservations.AddRangeAsync(reservations, _cancellationTokenSource.Token);
        await _context.SaveChangesAsync(_cancellationTokenSource.Token);

        stopwatch.Stop();
        Log.Information($"Seeding completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
    }

    public void CancelSeeding()
    {
        _cancellationTokenSource?.Cancel();
    }
}

public interface ISeeder
{
    Task SeedBooksOnlyAsync(int count, CancellationToken cancellationToken = default);
    Task SeedAuthorsOnlyAsync(int count, CancellationToken cancellationToken = default);
    Task SeedBooksWithAuthorsAsync(int count, CancellationToken cancellationToken = default);

    Task SeedUsersAsync(int count, CancellationToken cancellationToken = default);
    Task SeedReservationsAsync(int count, CancellationToken cancellationToken = default);
    Task SeedUsersWithReservationsAsync(int count, CancellationToken cancellationToken = default);

}