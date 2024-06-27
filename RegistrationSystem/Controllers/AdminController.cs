using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistrationSystem.Data;
using RegistrationSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RegistrationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.UserName == loginRequest.UserName && a.Password == loginRequest.Password);
            if (admin == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new { message = "Admin login successful" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAdmin([FromBody] LoginRequest loginRequest)
        {
            var existingAdmin = await _context.Admins
                .FirstOrDefaultAsync(a => a.UserName == loginRequest.UserName);
            if (existingAdmin != null)
                return BadRequest(new { message = "Admin already exists" });

            var admin = new Admin
            {
                UserName = loginRequest.UserName,
                Password = loginRequest.Password
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin registration successful" });
        }

        [HttpGet("registrations")]
        public async Task<IActionResult> GetRegistrations()
        {
            var registrations = await _context.Users
                .Where(u => !u.IsApproved)
                .ToListAsync();
            return Ok(registrations);
        }

        [HttpPost("approve")]
        public async Task<IActionResult> ApproveUser([FromBody] UserApprovalRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            user.IsApproved = true;
            await _context.SaveChangesAsync();
            return Ok(new { message = "User approved" });
        }

        [HttpPost("decline")]
        public async Task<IActionResult> DeclineUser([FromBody] UserApprovalRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "User declined" });
        }
    }

    public class UserApprovalRequest
    {
        public int UserId { get; set; }
    }
}
