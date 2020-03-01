using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HOB_WebApp.Data;
using HOB_WebApp.Models;

namespace HOB_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileUsersAPIController : ControllerBase
    {
        private readonly HOB_WebAppContext _context;

        public MobileUsersAPIController(HOB_WebAppContext context)
        {
            _context = context;
        }

        // GET: api/MobileUsers1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MobileUsers>>> GetMobileUsers()
        {
            return await _context.MobileUsers.ToListAsync();
        }

        // GET: api/MobileUsers1/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MobileUsers>> GetMobileUsers(int id)
        {
            var mobileUsers = await _context.MobileUsers.FindAsync(id);

            if (mobileUsers == null)
            {
                return NotFound();
            }

            return mobileUsers;
        }

        // PUT: api/MobileUsers1/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMobileUsers(int id, MobileUsers mobileUsers)
        {
            if (id != mobileUsers.Id)
            {
                return BadRequest();
            }

            _context.Entry(mobileUsers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MobileUsersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/MobileUsers1
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<MobileUsers>> PostMobileUsers(MobileUsers mobileUsers)
        {
            _context.MobileUsers.Add(mobileUsers);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMobileUsers", new { id = mobileUsers.Id }, mobileUsers);
        }

        // DELETE: api/MobileUsers1/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MobileUsers>> DeleteMobileUsers(int id)
        {
            var mobileUsers = await _context.MobileUsers.FindAsync(id);
            if (mobileUsers == null)
            {
                return NotFound();
            }

            _context.MobileUsers.Remove(mobileUsers);
            await _context.SaveChangesAsync();

            return mobileUsers;
        }

        private bool MobileUsersExists(int id)
        {
            return _context.MobileUsers.Any(e => e.Id == id);
        }
    }
}
