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
    public class HomeCodesAPIController : ControllerBase
    {
        private readonly HOB_WebAppContext _context;

        public HomeCodesAPIController(HOB_WebAppContext context)
        {
            _context = context;
        }

        // GET: api/HomeCodes1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HomeCodes>>> GetHomeCodes()
        {
            return await _context.HomeCodes.ToListAsync();
        }

        // GET: api/HomeCodes1/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HomeCodes>> GetHomeCodes(int id)
        {
            var homeCodes = await _context.HomeCodes.FindAsync(id);

            if (homeCodes == null)
            {
                return NotFound();
            }

            return homeCodes;
        }

        // PUT: api/HomeCodes1/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHomeCodes(int id, HomeCodes homeCodes)
        {
            if (id != homeCodes.Id)
            {
                return BadRequest();
            }

            _context.Entry(homeCodes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HomeCodesExists(id))
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

        // POST: api/HomeCodes1
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<HomeCodes>> PostHomeCodes(HomeCodes homeCodes)
        {
            _context.HomeCodes.Add(homeCodes);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHomeCodes", new { id = homeCodes.Id }, homeCodes);
        }

        // DELETE: api/HomeCodes1/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<HomeCodes>> DeleteHomeCodes(int id)
        {
            var homeCodes = await _context.HomeCodes.FindAsync(id);
            if (homeCodes == null)
            {
                return NotFound();
            }

            _context.HomeCodes.Remove(homeCodes);
            await _context.SaveChangesAsync();

            return homeCodes;
        }

        private bool HomeCodesExists(int id)
        {
            return _context.HomeCodes.Any(e => e.Id == id);
        }
    }
}
