using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HOB_WebApp.Data;
using HOB_WebApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace HOB_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [AllowAnonymous]
    public class MobileUsersAPIController : ControllerBase
    {
        private readonly HOB_WebAppContext _context;

        public MobileUsersAPIController(HOB_WebAppContext context)
        {
            _context = context;
        }

        /**
         * This is what we call to return a JSON of every Mobile User in the DB
         * We will probably not use this
         **/
        // GET: api/MobileUsersAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MobileUsers>>> GetMobileUsers()
        {
            return await _context.MobileUsers.ToListAsync();
        }


        /**
         * This is what we call to return a specific Mobile User
         * The "5" that they use in the sample url below is the id of a specific mobile user in the db
         * In order for this to work, you must know the particular mobile user ID ahead of time
         * We will probably not use this
         **/
        // GET: api/MobileUsersAPI/5
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

        /**
         * This is a PUT request. I do not think we will need it, but I will leave it in just in case. 
         * The "5" that they use in the sample url below is the id of a specific mobile user in the db
         * In order for this to work, you must know the particular mobile user ID ahead of time
         **/
        // PUT: api/MobileUsersAPI/5
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


        /**
         * This is a POST request. 
         * This will be called to create a new user
         * The "5" that they use in the sample url below is the id of a specific mobile user in the db
         * In order for this to work, you must know the particular mobile user ID ahead of time
         **/
        // POST: api/MobileUsersAPI
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<MobileUsers>> PostMobileUsers(MobileUsers mobileUsers)
        {

            var homeCode = await _context.HomeCodes.Where(m => m.Code == mobileUsers.Code).ToListAsync();
            if (homeCode.Count() != 0)
            {
                HomeCodes hc =  homeCode.Find(m => m.Code == mobileUsers.Code);
                mobileUsers.address = hc.Address;
                _context.MobileUsers.Add(mobileUsers);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetMobileUsers", new { id = mobileUsers.Id }, mobileUsers);
            } else
            {
                return mobileUsers;
            }
        }


        /**
         * This is a DELETE request. 
         * This will be called when an account needs to be deleted
         * The "5" that they use in the sample url below is the id of a specific mobile user in the db
         * In order for this to work, you must know the particular mobile user ID ahead of time
         **/
        // DELETE: api/MobileUsersAPI/5
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
