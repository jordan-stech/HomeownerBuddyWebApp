using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HOB_WebApp.Data;
using HOB_WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HOB_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [AllowAnonymous]
    public class BackgroundAPIController : ControllerBase
    {
        private readonly HOB_WebAppContext _context;

        // GET api/<BackgroundAPIController>/5
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaintenanceReminders>>> GetFirebaseNotifications()
        {
            var userList = await _context.MobileUsers.ToListAsync();
            var reminderList = await _context.UserReminders.ToListAsync();

            int countTodo = 0;
            int countOverdue = 0;

            // Create a Firebase notification for each user's reminders
            foreach (MobileUsers user in userList)
            {
                foreach (UserReminders userReminder in reminderList)
                {                   
                    if (user.Id == userReminder.UserId && userReminder.Completed == "Due")
                    {
                        countTodo++;
                    }

                    else if (user.Id == userReminder.UserId && userReminder.Completed == "Overdue")
                    {
                        countOverdue++;
                    }

                }

                // Create Firebase notification here using the count variables
            }

            return NoContent();
        }

        // POST api/<BackgroundAPIController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<BackgroundAPIController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BackgroundAPIController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
