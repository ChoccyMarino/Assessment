namespace Assessment.Models;

public class PostTag
{
    // Properties
    public int PostId { get; set; }
    public int TagId { get; set; }

    // Relationships
    public Post Post { get; set; }
    public Tag Tag { get; set; }
}