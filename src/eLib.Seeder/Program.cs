using CommandLine;
using eLib.DAL;
using eLib.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Serilog;

namespace eLib.Seeder;

public class Options
{
    [Option('m', "mode", HelpText = "Seeding mode: 1 for books only, 2 for authors, 3 for books with authors", Default = Mode.Everything)]
    public Mode Mode { get; set; }

    [Option('c', "count", Required = true, HelpText = "Number of items to generate")]
    public int Count { get; set; }
}

public enum Mode
{
    Books = 1,
    Authors = 2,
    BooksWithAuthors = 3,
    Users = 4,
    Reservations = 5,
    UsersWithReservations = 6,
    Everything = 7
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(RunSeeder);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while seeding the database");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static async Task RunSeeder(Options options)
    {
        Log.Information("Starting database seeder in {Mode} mode...", options.Mode);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var services = new ServiceCollection();

        services.AddServices(configuration);

        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<ISeeder>();

        Log.Information("Ensuring database exists...");
        await context.Database.EnsureCreatedAsync();

        Log.Information("Starting to seed {Count} items in {Mode} mode...", options.Count, options.Mode);

        switch (options.Mode)
        {
            case Mode.Books:
                if (await context.Authors.CountAsync() == 0)
                {
                    Log.Error("Cannot generate books: no authors in database");
                    return;
                }
                await seeder.SeedBooksOnlyAsync(options.Count);
                break;

            case Mode.Authors:
                await seeder.SeedAuthorsOnlyAsync(options.Count);
                break;

            case Mode.BooksWithAuthors:
                await seeder.SeedBooksWithAuthorsAsync(options.Count);
                break;

            case Mode.Reservations:
                if (await context.Users.CountAsync() == 0)
                {
                    Log.Error("Cannot generate books: no authors in database");
                    return;
                }
                await seeder.SeedReservationsAsync(options.Count);
                break;

            case Mode.Users:
                await seeder.SeedUsersAsync(options.Count);
                break;

            case Mode.UsersWithReservations:
                await seeder.SeedUsersWithReservationsAsync(options.Count);
                break;

            case Mode.Everything:
                await seeder.SeedBooksWithAuthorsAsync(options.Count);
                await seeder.SeedUsersWithReservationsAsync(options.Count);
                break;
        }

        var booksCount = await context.Books.CountAsync();
        var authorsCount = await context.Authors.CountAsync();
        var usersCount = await context.Users.CountAsync();
        var reservationsCount = await context.Reservations.CountAsync();

        Log.Information("Database seeded with {Books} books, {Authors} authors, {Users} users and {Reservations} reservations",
            booksCount, authorsCount, usersCount, reservationsCount);
    }
}