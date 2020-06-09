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
            var user = await _context.MobileUsers.Where(m => (m.FName == mobileUsers.FName) && (m.Lname == mobileUsers.Lname) && (m.Code == mobileUsers.Code) && (m.date == mobileUsers.date)).ToListAsync();
            //user = await _context.MobileUsers.Where(m => (m.RegDate == mobileUsers.date)).ToListAsync();
            var homeCode = await _context.HomeCodes.Where(m => m.Code == mobileUsers.Code).ToListAsync();
            //Only register a user if they use a valid, pre-exisiting home code
            if (homeCode.Count() != 0 && user.Count() == 0)
            {
                HomeCodes hc = homeCode.Find(m => m.Code == mobileUsers.Code);
                mobileUsers.address = hc.Address;
                _context.MobileUsers.Add(mobileUsers);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetMobileUsers", new { id = mobileUsers.Id }, mobileUsers);
            } else if (homeCode.Count() != 0 && user.Count() != 0)
            {
                //User is already created, send back a message stating so
                mobileUsers.Id = -1;
                return CreatedAtAction("GetMobileUsers", new { id = "-1" }, mobileUsers);
            } else { 
                return NotFound();
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
    }
}
