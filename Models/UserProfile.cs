namespace Assessment.Models;

public class UserProfile
{
    // Properties
    public int Id { get; set; }
    public string Bio { get; set; } = null!;
    
    // Relationships
    public int UserId {get; set;}
    public User User {get; set;} = null!;
    // null!; is to avoid null reference exception warnings
}