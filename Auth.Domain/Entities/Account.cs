namespace Auth.Domain.Entities;
public class Account
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public Account(string name, string surname, string password, string email)
    {
        Name = name;
        Surname = surname;
        Password = password;
        Email = email;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}