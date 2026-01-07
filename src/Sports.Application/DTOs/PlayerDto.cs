namespace Sports.Application.DTOs;

public class PlayerDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public int Height { get; set; }
    public int Weight { get; set; }
    public string Position { get; set; } = string.Empty;
    public int JerseyNumber { get; set; }
    public string? Photo { get; set; }
    public bool IsActive { get; set; }
    public Guid TeamId { get; set; }
    public string? TeamName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
