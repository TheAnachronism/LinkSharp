#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace LinkSharp.Database;

public enum CollectionAccessLevel
{
    Owner = 2,
    Write = 1,
    Read = 0
}

public class ApplicationUserCollectionAccess
{
    public ApplicationUser ApplicationUser { get; set; }
    public Collection Collection { get; set; }
    public CollectionAccessLevel CollectionAccessLevel { get; set; }
}