using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ATS.API.Models;
using ATS.API.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace ATS.API.Services;

/// <summary>
/// Authentication service interface following Dependency Inversion Principle
/// Defines contracts for authentication operations
/// </summary>
public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<User?> GetUserByIdAsync(int userId);
}

/// <summary>
/// Authentication service implementation
/// Handles user registration, login, and JWT token generation
/// Single Responsibility: Only handles authentication
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    /// <summary>
    /// Registers a new user in the system with password hashing
    /// </summary>
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return null;
        }

        // Create new user with BCrypt hashed password
        var user = new User
        {
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role ?? "Recruiter"
        };

        await _userRepository.AddAsync(user);

        // Generate JWT token
        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Token = token,
            Role = user.Role
        };
    }

    /// <summary>
    /// Authenticates user with email and password, returns JWT token
    /// </summary>
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        if (!user.IsActive)
        {
            return null;
        }

        // Update last login timestamp
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        // Generate JWT token
        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Token = token,
            Role = user.Role
        };
    }

    /// <summary>
    /// Retrieves user by ID
    /// </summary>
    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    /// <summary>
    /// Generates JWT token for authenticated user
    /// Implements security best practices: expiration, audience validation
    /// </summary>
    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(Convert.ToDouble(jwtSettings["ExpirationHours"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

/// <summary>
/// Position service interface for position-related business logic
/// </summary>
public interface IPositionService
{
    Task<IEnumerable<Position>> GetPositionsAsync();
    Task<IEnumerable<Position>> GetOpenPositionsAsync();
    Task<Position?> GetPositionByIdAsync(int id);
    Task<Position?> CreatePositionAsync(CreatePositionRequest request, int userId);
    Task<Position?> UpdatePositionAsync(int id, UpdatePositionRequest request);
    Task<Position?> ClosePositionAsync(int id);
    Task<IEnumerable<Position>> SearchPositionsAsync(string query);
}

/// <summary>
/// Position service implementation
/// Single Responsibility: Only handles position business logic
/// </summary>
public class PositionService : IPositionService
{
    private readonly IPositionRepository _positionRepository;

    public PositionService(IPositionRepository positionRepository)
    {
        _positionRepository = positionRepository;
    }

    public async Task<IEnumerable<Position>> GetPositionsAsync()
    {
        return await _positionRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Position>> GetOpenPositionsAsync()
    {
        return await _positionRepository.GetOpenPositionsAsync();
    }

    public async Task<Position?> GetPositionByIdAsync(int id)
    {
        return await _positionRepository.GetByIdAsync(id);
    }

    public async Task<Position?> CreatePositionAsync(CreatePositionRequest request, int userId)
    {
        var position = new Position
        {
            Title = request.Title,
            Description = request.Description,
            Department = request.Department,
            SalaryMin = request.SalaryMin,
            SalaryMax = request.SalaryMax,
            Location = request.Location,
            CreatedByUserId = userId,
            Status = "Open"
        };

        await _positionRepository.AddAsync(position);
        return position;
    }

    public async Task<Position?> UpdatePositionAsync(int id, UpdatePositionRequest request)
    {
        var position = await _positionRepository.GetByIdAsync(id);
        if (position == null) return null;

        position.Title = request.Title ?? position.Title;
        position.Description = request.Description ?? position.Description;
        position.Department = request.Department ?? position.Department;
        position.SalaryMin = request.SalaryMin ?? position.SalaryMin;
        position.SalaryMax = request.SalaryMax ?? position.SalaryMax;
        position.Location = request.Location ?? position.Location;

        await _positionRepository.UpdateAsync(position);
        return position;
    }

    public async Task<Position?> ClosePositionAsync(int id)
    {
        var position = await _positionRepository.GetByIdAsync(id);
        if (position == null) return null;

        position.Status = "Closed";
        position.ClosedAt = DateTime.UtcNow;

        await _positionRepository.UpdateAsync(position);
        return position;
    }

    public async Task<IEnumerable<Position>> SearchPositionsAsync(string query)
    {
        var positions = await _positionRepository.GetAllAsync();
        var lowerQuery = query.ToLower();

        return positions.Where(p =>
            p.Title.ToLower().Contains(lowerQuery) ||
            p.Description.ToLower().Contains(lowerQuery) ||
            p.Department.ToLower().Contains(lowerQuery)
        ).ToList();
    }
}

// ============ DTOs ============

/// <summary>
/// Data Transfer Object for user registration
/// </summary>
public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Role { get; set; }
}

/// <summary>
/// Data Transfer Object for user login
/// </summary>
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Data Transfer Object for authentication response
/// </summary>
public class AuthResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// Data Transfer Object for creating a position
/// </summary>
public class CreatePositionRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal SalaryMin { get; set; }
    public decimal SalaryMax { get; set; }
    public string Location { get; set; } = string.Empty;
}

/// <summary>
/// Data Transfer Object for updating a position (all fields optional)
/// </summary>
public class UpdatePositionRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Department { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string? Location { get; set; }
}
