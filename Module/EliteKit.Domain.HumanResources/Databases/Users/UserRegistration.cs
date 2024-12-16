namespace EliteKit.Domain.HumanResources.Databases.Users;

public class UserRegistration
{
    public Guid Id { get; set; }
    public string Account { get; set; }
    public DateTime CreateTime { get; set; }
    public string Email { get; set; }
    
    
}