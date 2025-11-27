namespace Assessment.Models;

public class Tag
{
    // Properties
    public int Id { get; set; }
    public string Name { get; set;} = null!;

    // Relationships
    public ICollection<PostTag> PostTags { get; set; } = null!;
    // null!; is to avoid null reference exception warnings
}