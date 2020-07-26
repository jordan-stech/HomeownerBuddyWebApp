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
    public class ManualNotificationAPIController : ControllerBase
    {
        private readonly HOB_WebAppContext _context;

        public ManualNotificationAPIController(HOB_WebAppContext context)
        {
            _context = context;
        }

        // GET api/<ManualNotificationAPIController>/5
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReminders>>> GetNotification(int id)
        {

            //var newUserId = Int32.Parse(id);

            var currentUser = await _context.MobileUsers
                .FirstOrDefaultAsync(m => m.Id == id);

            string userinstanceid = currentUser.InstanceId;

            FireBasePush push = new FireBasePush("AAAAUZUJsVw:APA91bGGHh_Zfzb4Ry3ywy68mcnuqWMdmFFa1YyoYc4EhCiNPiY95KhR-KAHnFbuE55Az3jiaMO-zLHkQK87UFWPyb_sYwv2o5-uR5YVcn71P1J2lB9aeObdeEkpi5ylaX7awaU4ZYvA");

            // Create and send notification
            push.SendPush(new PushMessage()
            {
                to = userinstanceid,
                notification = new PushMessageData
                {
                    title = "You have overdue maintenance reminders",
                    text = "Please review your overdue tasks.",
                }
            });

            return NoContent();
        }

        
    }
}
