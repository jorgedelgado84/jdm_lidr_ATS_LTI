using ATS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ATS.API.Controllers;

/// <summary>
/// Authentication controller handling user registration and login
/// RESTful endpoints with proper HTTP status codes
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user in the system
    /// POST /api/auth/register
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _authService.RegisterAsync(request);
            if (result == null)
                return BadRequest("User already exists or registration failed");

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return StatusCode(500, "An error occurred during registration");
        }
    }

    /// <summary>
    /// Authenticates user and returns JWT token
    /// POST /api/auth/login
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _authService.LoginAsync(request);
            if (result == null)
                return Unauthorized("Invalid credentials");

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return StatusCode(500, "An error occurred during login");
        }
    }

    /// <summary>
    /// Gets current authenticated user information
    /// GET /api/auth/profile
    /// Requires: Valid JWT token
    /// </summary>
    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(new { user.Id, user.Email, user.FullName, user.Role });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user profile");
            return StatusCode(500, "An error occurred");
        }
    }
}

/// <summary>
/// Positions controller for managing job positions
/// Implements role-based authorization and error handling
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PositionsController : ControllerBase
{
    private readonly IPositionService _positionService;
    private readonly ILogger<PositionsController> _logger;

    public PositionsController(IPositionService positionService, ILogger<PositionsController> logger)
    {
        _positionService = positionService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all positions
    /// GET /api/positions
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAllPositions()
    {
        try
        {
            var positions = await _positionService.GetPositionsAsync();
            return Ok(positions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving positions");
            return StatusCode(500, "Error retrieving positions");
        }
    }

    /// <summary>
    /// Gets all open positions
    /// GET /api/positions/open
    /// </summary>
    [HttpGet("open")]
    public async Task<ActionResult> GetOpenPositions()
    {
        try
        {
            var positions = await _positionService.GetOpenPositionsAsync();
            return Ok(positions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving open positions");
            return StatusCode(500, "Error retrieving open positions");
        }
    }

    /// <summary>
    /// Gets a specific position by ID
    /// GET /api/positions/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetPositionById(int id)
    {
        try
        {
            var position = await _positionService.GetPositionByIdAsync(id);
            if (position == null)
                return NotFound("Position not found");

            return Ok(position);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving position {PositionId}", id);
            return StatusCode(500, "Error retrieving position");
        }
    }

    /// <summary>
    /// Creates a new position (Admin or Manager only)
    /// POST /api/positions
    /// Requires: JWT token with Admin or Manager role
    /// </summary>
    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    public async Task<ActionResult> CreatePosition([FromBody] CreatePositionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var position = await _positionService.CreatePositionAsync(request, userId);
            return CreatedAtAction(nameof(GetPositionById), new { id = position?.Id }, position);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating position");
            return StatusCode(500, "Error creating position");
        }
    }

    /// <summary>
    /// Updates a position
    /// PUT /api/positions/{id}
    /// Requires: JWT token with Admin or Manager role
    /// </summary>
    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdatePosition(int id, [FromBody] UpdatePositionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var position = await _positionService.UpdatePositionAsync(id, request);
            if (position == null)
                return NotFound("Position not found");

            return Ok(position);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating position {PositionId}", id);
            return StatusCode(500, "Error updating position");
        }
    }

    /// <summary>
    /// Closes a position
    /// PUT /api/positions/{id}/close
    /// Requires: JWT token with Admin or Manager role
    /// </summary>
    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id}/close")]
    public async Task<ActionResult> ClosePosition(int id)
    {
        try
        {
            var position = await _positionService.ClosePositionAsync(id);
            if (position == null)
                return NotFound("Position not found");

            return Ok(position);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing position {PositionId}", id);
            return StatusCode(500, "Error closing position");
        }
    }

    /// <summary>
    /// Searches positions by query
    /// GET /api/positions/search/{query}
    /// </summary>
    [HttpGet("search/{query}")]
    public async Task<ActionResult> SearchPositions(string query)
    {
        try
        {
            var positions = await _positionService.SearchPositionsAsync(query);
            return Ok(positions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching positions with query: {Query}", query);
            return StatusCode(500, "Error searching positions");
        }
    }
}
