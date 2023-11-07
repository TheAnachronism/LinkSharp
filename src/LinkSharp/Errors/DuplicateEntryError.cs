using FluentResults;

namespace LinkSharp.Errors;

public class DuplicateEntryError<T> : Error
{
    public T DuplicateObject { get; }
    public DuplicateEntryError(T duplicateObject, string message) : base(message)
    {
        DuplicateObject = duplicateObject;
    }
}