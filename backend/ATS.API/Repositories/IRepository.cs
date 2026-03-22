using ATS.API.Models;

namespace ATS.API.Repositories;

/// <summary>
/// Generic repository interface following repository pattern
/// Demonstrates Interface Segregation Principle (ISP)
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}

/// <summary>
/// Position repository interface for position-specific queries
/// Follows Single Responsibility Principle - handles only Position queries
/// </summary>
public interface IPositionRepository : IRepository<Position>
{
    Task<IEnumerable<Position>> GetPositionsByStatusAsync(string status);
    Task<IEnumerable<Position>> GetPositionsByDepartmentAsync(string department);
    Task<IEnumerable<Position>> GetOpenPositionsAsync();
    Task<IEnumerable<Position>> GetPositionsByCandidateAsync(int candidateId);
}

/// <summary>
/// Candidate repository interface for candidate-specific queries
/// </summary>
public interface ICandidateRepository : IRepository<Candidate>
{
    Task<Candidate?> GetByEmailAsync(string email);
    Task<IEnumerable<Candidate>> GetCandidatesByPositionAsync(int positionId);
    Task<IEnumerable<Candidate>> SearchCandidatesAsync(string query);
    Task<int> GetApplicationCountAsync(int candidateId);
}

/// <summary>
/// Application repository interface for application-specific queries
/// </summary>
public interface IApplicationRepository : IRepository<Application>
{
    Task<Application?> GetApplicationAsync(int candidateId, int positionId);
    Task<IEnumerable<Application>> GetApplicationsByStatusAsync(string status);
    Task<IEnumerable<Application>> GetApplicationsByPositionAsync(int positionId);
    Task<IEnumerable<Application>> GetApplicationsByCandidateAsync(int candidateId);
    Task<IEnumerable<Application>> GetApplicationsByReviewerAsync(int reviewerId);
}

/// <summary>
/// User repository interface for user-specific queries
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
}
