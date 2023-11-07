#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace LinkSharp.Database;

public class Link
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public ApplicationUser Creator { get; set; }
    public List<Tag> Tags { get; set; }
    public List<Collection> Collections { get; set; }   
}