using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly TodoContext _context;

        public UserController(TodoContext context)
        {
            _context = context;

            if (_context.Users.Count() == 0)
            {
                // Create a new TodoItem if collection is empty,
                _context.Users.Add(new User { Username = "Angel", Password = "test" });
                _context.SaveChanges();
            }
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            var user = await _context.Users
                .Include(u => u.Children)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]User userParam)
        {
            var user = await Task.Run(() => _context.Users.SingleOrDefault(x => x.Username == userParam.Username && x.Password == userParam.Password));

            // return null if user not found
            if (user == null)
                return Unauthorized();

            // authentication successful so return user details without password
            user.Password = null;

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(User user)
        {
            var existingUser = _context.Users
                .Where(u => u.UserId == user.UserId)
                .Include(u => u.Children)
                .SingleOrDefault();
            
            if (user == null || existingUser == null)
            {
                return BadRequest();
            }

            // _context.Entry(user).State = EntityState.Modified;
            this.UpdateFullUser(user, existingUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private void UpdateFullUser(User user, Models.User existingUser)
        {
            // Update parent
            _context.Entry(existingUser).CurrentValues.SetValues(user);

            var childList = user.Children.ToList();

            // Delete children
            childList.ForEach(child =>
            {
                if (!user.Children.Any(c => c.ChildId == child.ChildId))
                {
                    _context.Children.Remove(child);
                }
            });

            childList.ForEach(child =>
            {
                var existingChild = existingUser.Children
                    .Where(c => c.ChildId == child.ChildId)
                    .SingleOrDefault();

                if (existingChild != null)
                {
                    // Update child
                    _context.Entry(existingChild).CurrentValues.SetValues(child);
                }
                else
                {
                    // Insert child
                    existingUser.Children.Add(child);
                }
            });
        }
    }
}