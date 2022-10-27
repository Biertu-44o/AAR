using System.ComponentModel.DataAnnotations;

public class Users
{
    [Key]
    [MaxLength(20)]
    public string Name { get; set; }
    [MaxLength(100)]
    public string Password { get; set; } = "";
    [MaxLength(20)]
    public string Second { get; set; } = "";
    public string Role { get; set; } = "user";
}