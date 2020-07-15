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

        public BackgroundAPIController(HOB_WebAppContext context)
        {
            _context = context;
        }

        // GET api/<BackgroundAPIController>/5
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReminders>>> GetFirebaseNotifications()
        {

            var userList = await _context.MobileUsers.ToListAsync();
            var reminderList = await _context.UserReminders.ToListAsync();

            
            FireBasePush push = new FireBasePush("AAAAUZUJsVw:APA91bGGHh_Zfzb4Ry3ywy68mcnuqWMdmFFa1YyoYc4EhCiNPiY95KhR-KAHnFbuE55Az3jiaMO-zLHkQK87UFWPyb_sYwv2o5-uR5YVcn71P1J2lB9aeObdeEkpi5ylaX7awaU4ZYvA");

            // Create a Firebase notification for each user's reminders
            foreach (MobileUsers user in userList)
            {
                int countTodo = 0;
                int countOverdue = 0;
                string userinstanceid = user.InstanceId;

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
                push.SendPush(new PushMessage()
                {
                    to = userinstanceid,
                    notification = new PushMessageData
                    {
                        title = "You have new maintenance reminders",
                        text = "You have " + countOverdue + " overdue tasks and " + countTodo + " due tasks.",
                        //click_action = click_action
                    }
                });
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
