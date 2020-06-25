using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HOB_WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using HOB_WebApp.Models;

namespace HOB_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [AllowAnonymous]
    public class MaintenanceReminderAPIController : ControllerBase
    {
        private readonly HOB_WebAppContext _context;

        //Get the DB context
        public MaintenanceReminderAPIController(HOB_WebAppContext context)
        {
            _context = context;
        }


        /**
         * This is what we call to return a JSON of every Action Plan in the DB
         **/
        // GET: api/ActionPlanAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaintenanceReminders>>> GetContentModel()
        {
            return await _context.MaintenanceReminders.ToListAsync();
        }

        /**
         * This is a POST request. 
         * This will be called to create maintenance reminder tasks associated with specific mobile users
         * The "5" that they use in the sample url below is the id of a specific mobile user in the db
         * In order for this to work, you must know the particular mobile user ID ahead of time
         **/
        // POST: api/MobileUsersAPI
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<MobileUsers>> PostMaintenanceReminders(MobileUsers mobileUsers)
        {
            // Grab the current mobile user so their id can be used
            var currentUser = await _context.MobileUsers.Where(m => (m.FName == mobileUsers.FName) && (m.Lname == mobileUsers.Lname) && (m.Code == mobileUsers.Code) && (m.date == mobileUsers.date)).ToListAsync();
            MobileUsers currentUserId = currentUser.Find(m => m.Code == mobileUsers.Code);

            // Grab the current maintenance reminders and store them in a list
            var currentGlobalReminders = await _context.MaintenanceReminders.ToListAsync();
            var currentUserReminders = await _context.UserReminders.Where(m => m.UserId == currentUserId.Id).ToListAsync();

            

            var currentGlobalReminders1 = new List<MaintenanceReminders>();
            var currentUserReminders1 = new List<UserReminders>();// await _context.UserReminders.Where(m => (m.ReminderId == currentGlobalReminders[0].Id)).ToListAsync();

            // currentUserReminders = await _context.UserReminders.Where(m => (m.ReminderId == currentGlobalReminders[0].Id)).ToListAsync();

            //int count = 0;


            // If the current mobile user is accessing the maintenance reminder page for the first time, create individualized reminders for each current maintenance reminder
            if (currentUserReminders.Count() == 0 && currentGlobalReminders.Count() != 0)
            {
                for (int i = 0; i < currentGlobalReminders.Count(); i++)
                {
                    UserReminders reminder = new UserReminders();
                    reminder.ReminderId = currentGlobalReminders[i].Id;
                    reminder.UserId = currentUserId.Id;
                    reminder.FName = currentUserId.FName;
                    reminder.LName = currentUserId.Lname;
                    reminder.Address = currentUserId.address;
                    reminder.Reminder = currentGlobalReminders[i].Reminder;
                    reminder.Completed = "No";
                    _context.UserReminders.Add(reminder);
                    await _context.SaveChangesAsync();
                }
            }
            // Will be checked each time the current mobile user opens the maintenance reminder page
            /*else if (currentUserReminders.Count() != 0 && currentGlobalReminders.Count() != 0)
            {
                for (int i = 0; i < currentGlobalReminders.Count(); i++)
                {
                    for (int j = 0; j < currentUserReminders.Count(); j++)
                    {
                        MaintenanceReminders maintenanceReminder = currentGlobalReminders.Find(m => m.Id == currentUserReminders[j].ReminderId);
                        UserReminders userReminder = currentUserReminders.Find(m => m.UserId == currentUserId.Id);

                        if (maintenanceReminder != null)
                        {
                            count++;
                        }
                        else
                        {
                            currentGlobalReminders1.Add(maintenanceReminder);
                        }
                    }
                }
            }*/

            /*if (currentGlobalReminders.Count() != 0)
            {
                for (int i = 0; i < currentGlobalReminders.Count(); i++)
                {
                    for (int j = 0; j < currentUserReminders.Count(); j++)
                    {
                        MaintenanceReminders maintenanceReminder = currentGlobalReminders.Find(m => m.Id == currentUserReminders[j].ReminderId);
                        UserReminders userReminder = currentUserReminders.Find(m => m.UserId == currentUserId.Id);

                        if (maintenanceReminder != null)
                        {                            
                            count++;
                        }
                        else
                        {
                            currentGlobalReminders1.Add(maintenanceReminder);
                        }
                    }
                }
            }

            if (count < currentGlobalReminders.Count())
            {

            }
                
                //var currentUserReminders1 = 


            // Do a foreach loop over each UserReminder before doing the for loop
            // This foreach loop will

            for (int i = 0; i < currentUserReminders.Count(); i++)
            {
                for (int j = 0; j < currentGlobalReminders.Count(); j++)
                {
                    if (currentUserReminders[i].ReminderId != currentGlobalReminders[j].Id && (j == currentGlobalReminders.Count() - 1))
                    {
                        UserReminders reminder = new UserReminders();
                        reminder.ReminderId = currentGlobalReminders[i].Id;
                        reminder.UserId = currentUserId.Id;
                        reminder.FName = currentUserId.FName;
                        reminder.LName = currentUserId.Lname;
                        reminder.Reminder = currentGlobalReminders[i].Reminder;
                        reminder.Completed = currentGlobalReminders[i].Completed;
                        _context.UserReminders.Add(reminder);
                        await _context.SaveChangesAsync();
                    }
                }
            }    */        
            return NoContent();            
        }
    }
}
