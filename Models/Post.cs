namespace Assessment.Models;

public class Post
{
    // Properties
    public int Id {get; set;}
    public string Title {get; set;}
    public string Content {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}

    // Relationships
    public int UserId {get; set;}
    public User User {get; set;}
    public ICollection<PostTag> PostTags {get; set;}
}