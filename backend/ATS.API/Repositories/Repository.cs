using ATS.API.Data;
using ATS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ATS.API.Repositories;

/// <summary>
/// Generic repository implementation following SOLID principles
/// Provides base CRUD operations for all entity types
/// </summary>
public abstract class Repository<T> : IRepository<T> where T : class
{
    protected readonly AtsDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AtsDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await SaveChangesAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

/// <summary>
/// Position repository implementation
/// Handles all position-related database operations
/// </summary>
public class PositionRepository : Repository<Position>, IPositionRepository
{
    public PositionRepository(AtsDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Position>> GetPositionsByStatusAsync(string status)
    {
        return await _dbSet.Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Position>> GetPositionsByDepartmentAsync(string department)
    {
        return await _dbSet.Where(p => p.Department == department)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Position>> GetOpenPositionsAsync()
    {
        return await GetPositionsByStatusAsync("Open");
    }

    public async Task<IEnumerable<Position>> GetPositionsByCandidateAsync(int candidateId)
    {
        return await _dbSet
            .Where(p => p.Applications.Any(a => a.CandidateId == candidateId))
            .ToListAsync();
    }
}

/// <summary>
/// Candidate repository implementation
/// </summary>
public class CandidateRepository : Repository<Candidate>, ICandidateRepository
{
    public CandidateRepository(AtsDbContext context) : base(context)
    {
    }

    public async Task<Candidate?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<IEnumerable<Candidate>> GetCandidatesByPositionAsync(int positionId)
    {
        return await _dbSet
            .Where(c => c.Applications.Any(a => a.PositionId == positionId))
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Candidate>> SearchCandidatesAsync(string query)
    {
        var lowerQuery = query.ToLower();
        return await _dbSet
            .Where(c => c.FirstName.ToLower().Contains(lowerQuery) ||
                       c.LastName.ToLower().Contains(lowerQuery) ||
                       c.Email.ToLower().Contains(lowerQuery) ||
                       c.CurrentCompany!.ToLower().Contains(lowerQuery))
            .ToListAsync();
    }

    public async Task<int> GetApplicationCountAsync(int candidateId)
    {
        return await _context.Applications
            .CountAsync(a => a.CandidateId == candidateId);
    }
}

/// <summary>
/// Application repository implementation
/// </summary>
public class ApplicationRepository : Repository<Application>, IApplicationRepository
{
    public ApplicationRepository(AtsDbContext context) : base(context)
    {
    }

    public async Task<Application?> GetApplicationAsync(int candidateId, int positionId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.CandidateId == candidateId && a.PositionId == positionId);
    }

    public async Task<IEnumerable<Application>> GetApplicationsByStatusAsync(string status)
    {
        return await _dbSet
            .Where(a => a.Status == status)
            .Include(a => a.Candidate)
            .Include(a => a.Position)
            .OrderByDescending(a => a.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Application>> GetApplicationsByPositionAsync(int positionId)
    {
        return await _dbSet
            .Where(a => a.PositionId == positionId)
            .Include(a => a.Candidate)
            .OrderByDescending(a => a.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Application>> GetApplicationsByCandidateAsync(int candidateId)
    {
        return await _dbSet
            .Where(a => a.CandidateId == candidateId)
            .Include(a => a.Position)
            .OrderByDescending(a => a.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Application>> GetApplicationsByReviewerAsync(int reviewerId)
    {
        return await _dbSet
            .Where(a => a.ReviewedByUserId == reviewerId)
            .Include(a => a.Candidate)
            .Include(a => a.Position)
            .OrderByDescending(a => a.ReviewedAt)
            .ToListAsync();
    }
}

/// <summary>
/// User repository implementation
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AtsDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
    {
        return await _dbSet.Where(u => u.Role == role && u.IsActive).ToListAsync();
    }
}
