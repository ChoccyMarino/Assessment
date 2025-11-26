namespace Assessment.Models;

public class Tag
{
    // Properties
    public int Id { get; set; }
    public string Name { get; set;}

    // Relationships
    public ICollection<PostTag> PostTags { get; set; }
}