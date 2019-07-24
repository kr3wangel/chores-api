using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/child")]
    [ApiController]
    public class ChildController : ControllerBase
    {
        private readonly TodoContext _context;

        public ChildController(TodoContext context)
        {
            _context = context;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Child>>> GetUsers()
        {
            return await _context.Children.ToListAsync();
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Child>> GetChild(long id)
        {
            var child = await _context.Children.FindAsync(id);

            if (child == null)
            {
                return NotFound();
            }

            return child;
        }

        [HttpPost]
        public async Task<ActionResult<Child>> CreateChild(Child child)
        {
            _context.Children.Add(child);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetChild), new { id = child.ChildId }, child);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateChild(Child child)
        {
            if (child == null)
            {
                return BadRequest();
            }

            _context.Entry(child).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var child = await _context.Children.FindAsync(id);

            if (child == null)
            {
                return NotFound();
            }

            _context.Children.Remove(child);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}