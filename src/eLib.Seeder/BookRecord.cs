using CsvHelper.Configuration.Attributes;

namespace eLib.Seeder;

public class BookRecord
{
    [Name("title")]
    public string Title { get; set; }

    [Name("author")]
    public string Author { get; set; }

    [Name("description")]
    public string Description { get; set; }

    [Name("coverImg")]
    public string CoverUrl { get; set; }
}
