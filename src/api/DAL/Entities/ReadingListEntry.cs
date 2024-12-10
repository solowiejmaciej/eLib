using eLib.Models.Results;
using eLib.Models.Results.Base;

namespace eLib.DAL.Entities;

public class ReadingListEntry : Entity
{
    private ReadingListEntry() : base(Guid.NewGuid()) { }

    public Guid BookId { get; private set; }
    public Guid UserId { get; private set; }
    public int Progress { get; private set; }
    public bool IsFinished { get; private set; }
    public DateTime DateAdded { get; private set; }

    public static ReadingListEntry Create(Guid bookId, Guid userId, int progress, bool isFinished)
    {
        var readingListEntry = new ReadingListEntry
        {
            BookId = bookId,
            UserId = userId,
            Progress = progress,
            IsFinished = isFinished,
            DateAdded = DateTime.UtcNow
        };
        return readingListEntry;
    }

    public Result<bool, Error> UpdateProgress(int progress)
    {
        if (progress < 0 || progress > 100)
        {
            return ReadingListErrors.InvalidProgress;
        }

        Progress = progress;
        return true;
    }

    public Result<bool, Error> MarkAsFinished()
    {
        if (IsFinished)
        {
            return ReadingListErrors.AlreadyFinished;
        }

        IsFinished = true;
        Progress = 100;
        return true;
    }

    public Result<bool, Error> MarkAsUnfinished()
    {
        if (!IsFinished)
        {
            return ReadingListErrors.AlreadyUnfinished;
        }

        IsFinished = false;
        Progress = 0;
        return true;
    }
}