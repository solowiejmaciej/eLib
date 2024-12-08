using Bogus;
using CsvHelper;
using CsvHelper.Configuration;
using eLib.Common.Notifications;
using eLib.DAL;
using eLib.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using Serilog;

namespace eLib.Seeder;

public class Seeder : ISeeder
{
    private readonly LibraryDbContext _context;
    private readonly Faker<Author> _authorFaker;
    private CancellationTokenSource _cancellationTokenSource;
    private List<BookRecord> _csvBooks;

    public Seeder(LibraryDbContext context)
    {
        _context = context;
        _authorFaker = CreateAuthorFaker();
        LoadCsvData("books.csv");
    }

    private void LoadCsvData(string csvFilePath)
    {
        try
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            };

            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, config);
            _csvBooks = csv.GetRecords<BookRecord>()
                .Where(r => !string.IsNullOrWhiteSpace(r.Title) && !string.IsNullOrWhiteSpace(r.Author))
                .ToList();

            Log.Information($"Successfully loaded {_csvBooks.Count} records from CSV");
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to load CSV data: {ex.Message}");
            _csvBooks = new List<BookRecord>();
        }
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

    private async Task<Dictionary<string, Author>> CreateAuthorsFromCsv(CancellationToken cancellationToken)
    {
        var authorDict = new Dictionary<string, Author>();
        var uniqueAuthors = _csvBooks.Select(b => b.Author).Distinct();

        foreach (var authorFullName in uniqueAuthors)
        {
            var nameParts = authorFullName.Split(' ', 2);
            var firstName = nameParts[0];
            var lastName = nameParts.Length > 1 ? nameParts[1] : "";

            var author = Author.Create(
                firstName,
                lastName,
                _authorFaker.Generate().Birthday,
                _authorFaker.Generate().Details.Biography,
                _authorFaker.Generate().Details.PhotoUrl
            );

            authorDict[authorFullName] = author;
        }

        await _context.Authors.AddRangeAsync(authorDict.Values, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return authorDict;
    }

    public async Task SeedAuthorsOnlyAsync(int count, CancellationToken cancellationToken = default)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var stopwatch = Stopwatch.StartNew();
        Log.Information($"Seeding {count} authors...");

        var authors = _authorFaker.Generate(count);

        await _context.Authors.AddRangeAsync(authors, _cancellationTokenSource.Token);
        await _context.SaveChangesAsync(_cancellationTokenSource.Token);

        stopwatch.Stop();
        Log.Information($"Seeding completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
    }

    public async Task SeedBooksOnlyAsync(int count, CancellationToken cancellationToken = default)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var authors = await _context.Authors.ToListAsync(_cancellationTokenSource.Token);

        if (!authors.Any())
        {
            throw new InvalidOperationException("No authors found in database. Please seed authors first.");
        }

        var books = new List<Book>();
        var recordsToUse = _csvBooks.Any()
            ? _csvBooks.Take(count).ToList()
            : new List<BookRecord>();

        foreach (var csvBook in recordsToUse)
        {
            var author = authors[Random.Shared.Next(authors.Count)];

            string coverUrl = csvBook.CoverUrl;
            if (!string.IsNullOrEmpty(coverUrl) && coverUrl.StartsWith("://"))
            {
                coverUrl = "https" + coverUrl;
            }
            else
            {
                coverUrl = "https://picsum.photos/200/300";
            }

            var book = Book.Create(
                csvBook.Title,
                author.Id,
                csvBook.Description ?? "No description available",
                coverUrl,
                Random.Shared.Next(1, 10)
            );
            books.Add(book);
        }

        var remainingCount = count - books.Count;
        if (remainingCount > 0)
        {
            var bookFaker = new Faker<Book>()
                .CustomInstantiator(f => Book.Create(
                    f.Commerce.ProductName(),
                    f.PickRandom(authors).Id,
                    f.Lorem.Paragraphs(3),
                    f.Image.PicsumUrl(),
                    f.Random.Number(1, 10)
                ));

            books.AddRange(bookFaker.Generate(remainingCount));
        }

        var stopwatch = Stopwatch.StartNew();
        Log.Information($"Seeding {count} books...");

        await _context.Books.AddRangeAsync(books, _cancellationTokenSource.Token);
        await _context.SaveChangesAsync(_cancellationTokenSource.Token);

        stopwatch.Stop();
        Log.Information($"Seeding completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
    }

    public async Task SeedBooksWithAuthorsAsync(int count, CancellationToken cancellationToken = default)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var stopwatch = Stopwatch.StartNew();

        Log.Information("Creating authors from CSV data...");
        var authorDict = await CreateAuthorsFromCsv(_cancellationTokenSource.Token);

        var books = new List<Book>();
        var recordsToUse = _csvBooks.Take(count).ToList();

        foreach (var csvBook in recordsToUse)
        {
            if (authorDict.TryGetValue(csvBook.Author, out var author))
            {
                string coverUrl = !string.IsNullOrEmpty(csvBook.CoverUrl) && csvBook.CoverUrl.StartsWith("://")
                    ? "https" + csvBook.CoverUrl
                    : "https://picsum.photos/200/300";

                var book = Book.Create(
                    csvBook.Title,
                    author.Id,
                    csvBook.Description ?? "No description available",
                    coverUrl,
                    Random.Shared.Next(1, 10)
                );
                books.Add(book);
            }
        }

        var remainingCount = count - books.Count;
        if (remainingCount > 0)
        {
            var bookFaker = new Faker<Book>()
                .CustomInstantiator(f => Book.Create(
                    f.Commerce.ProductName(),
                    f.PickRandom(authorDict.Values.ToList()).Id,
                    f.Lorem.Paragraphs(3),
                    f.Image.PicsumUrl(),
                    f.Random.Number(1, 10)
                ));

            books.AddRange(bookFaker.Generate(remainingCount));
        }

        Log.Information($"Seeding {books.Count} books...");
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
            .RuleFor(u => u.Name, f => f.Name.FirstName())
            .RuleFor(u => u.Surname, f => f.Name.LastName())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Name, u.Surname))
            .CustomInstantiator(f => User.Create(
                f.Name.FirstName(),
                f.Name.LastName(),
                f.Internet.Email(),
                "string",
                f.Phone.PhoneNumber("#########"),
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

        if (!users.Any() || !bookDetails.Any())
        {
            throw new InvalidOperationException("No users or books found in database. Please seed users and books first.");
        }

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

    public async Task SeedUsersWithReservationsAsync(int count, CancellationToken cancellationToken = default)
    {
        await SeedUsersAsync(count, cancellationToken);
        await SeedReservationsAsync(count, cancellationToken);
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