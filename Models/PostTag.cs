namespace Assessment.Models;

public class PostTag
{
    // Properties
    public int PostId { get; set; }
    public int TagId { get; set; }

    // Relationships
    public Post Post { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
    // null!; is to avoid null reference exception warnings
}