namespace Assessment.Models;

public class User
{
    // properties
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    // null!; is to avoid null reference exception warnings


    // Relationships
    public UserProfile? UserProfile { get; set; } = null!;
    public ICollection<Post> Posts {     get; set; } = new List<Post>();
    //UserProfile? is to indicate it CAN be null
    //new List<Post>() to initialize the collection


}