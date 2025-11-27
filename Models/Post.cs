namespace Assessment.Models;

public class Post
{
    // Properties
    public int Id {get; set;}
    public string Title {get; set;} = null!;
    public string Content {get; set;} = null!;
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
    // null!; is to avoid null reference exception warnings

    
    // Relationships
    public int UserId {get; set;}
    public User User {get; set;} = null!;
    public ICollection<PostTag> PostTags {get; set;} = new List<PostTag>();
    // new List<PostTag>() to initialize the collection
}