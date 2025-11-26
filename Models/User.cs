namespace Assessment.Models;

public class User
{
    // properties
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Relationships
    public UserProfile UserProfile { get; set; }
    public ICollection<Post> Posts { get; set; }
}