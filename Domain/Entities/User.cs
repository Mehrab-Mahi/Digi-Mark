using Domain.Models;

namespace Domain.Entities;

public class User : Entity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public required string UserNameName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public bool IsSystemAdmin { get; set; } = false;
    public bool IsActive { get; set; } = true; 
}