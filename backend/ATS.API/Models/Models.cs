using System.ComponentModel.DataAnnotations;

namespace ATS.API.Models;

/// <summary>
/// Represents a user in the ATS system (recruiters, admins, etc.)
/// Follows SOLID principles with clear single responsibility
/// </summary>
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Role { get; set; } = "Recruiter"; // Admin, Recruiter, Reviewer

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Position> CreatedPositions { get; set; } = new List<Position>();
    public ICollection<Application> ReviewedApplications { get; set; } = new List<Application>();
}

/// <summary>
/// Represents a job position in the system
/// </summary>
public class Position
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    public decimal SalaryMin { get; set; }

    [Required]
    public decimal SalaryMax { get; set; }

    [StringLength(100)]
    public string Location { get; set; } = string.Empty;

    [StringLength(50)]
    public string Status { get; set; } = "Open"; // Open, Closed, On Hold

    public int CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ClosedAt { get; set; }

    // Navigation properties
    public User CreatedByUser { get; set; } = null!;
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}

/// <summary>
/// Represents a candidate applying for a position
/// </summary>
public class Candidate
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }

    public string? ResumeUrl { get; set; }

    [StringLength(500)]
    public string? Summary { get; set; }

    [StringLength(100)]
    public string? CurrentPosition { get; set; }

    [StringLength(100)]
    public string? CurrentCompany { get; set; }

    public int YearsOfExperience { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}

/// <summary>
/// Represents a candidate's application for a specific position
/// </summary>
public class Application
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CandidateId { get; set; }

    [Required]
    public int PositionId { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Submitted"; // Submitted, Reviewing, Shortlisted, Rejected, Accepted

    public int? ReviewedByUserId { get; set; }

    public string? ReviewNotes { get; set; }

    public int? RatingScore { get; set; } // 1-5 rating

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }

    // Navigation properties
    public Candidate Candidate { get; set; } = null!;
    public Position Position { get; set; } = null!;
    public User? ReviewedByUser { get; set; }
}
