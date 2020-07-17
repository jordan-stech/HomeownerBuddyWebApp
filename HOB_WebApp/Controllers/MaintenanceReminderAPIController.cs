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
using System.Data.SqlTypes;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

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
         * This is a placeholder
         **/
        // GET: api/ActionPlanAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaintenanceReminders>>> GetMaintenanceReminders()
        {
            return await _context.MaintenanceReminders.ToListAsync();
        }

        /**
         * This is what we call to return a JSON of every User Reminder in the DB
         **/
        // GET: api/MaintenanceReminderAPI
        [HttpGet("{UserId}")]
        public async Task<ActionResult<IEnumerable<UserReminders>>> GetUserReminders(string userId)
        {
            int newId = Int32.Parse(userId);
            return await _context.UserReminders.Where(m => m.UserId == newId).ToListAsync();
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
            var usersWithSameAddress = await _context.MobileUsers.Where(m => m.Code == mobileUsers.Code).ToListAsync();
            //MobileUsers currentUserId = currentUser.Find(m => m.Id == mobileUsers.Id);

            MobileUsers currentUserId = usersWithSameAddress.Find(m => m.Id == mobileUsers.Id);

            // Grab the current maintenance reminders and store them in a list
            var currentGlobalReminders = await _context.MaintenanceReminders.ToListAsync();
            var currentUserReminders = await _context.UserReminders.Where(m => m.UserId == currentUserId.Id).ToListAsync();
            var allUserReminders = await _context.UserReminders.ToListAsync();
            var allUsers = await _context.MobileUsers.ToListAsync();


            // Gets the first day of the next week, which is Sunday
            var firstdayOfNextWeek = DateTime.Now.LastDayOfWeek();

            // Gets the first day of the next month
            var firstdayOfNextMonth = DateTime.Now.FirstDayOfNextMonth();

            // Gets the first day of next year
            var newYear = DateTime.Now.AddYears(1);
            var nextYear = newYear.Year;
            var firstDayOfNextYear = new DateTime(nextYear, 1, 1);

            // Gets the last day of the current year
            var lastDayOfYear = new DateTime(DateTime.Now.Year, 12, 31);

            // First day of last year's winter
            var tempWinter1 = new DateTime(DateTime.Now.Year, 12, 1);
            var tempWinter2 = tempWinter1.AddYears(-1);
            int lastWinter = tempWinter2.Year;

            // Season date ranges
            var startSpring = new DateTime(DateTime.Now.Year, 3, 1);
            var endSpring = new DateTime(DateTime.Now.Year, 5, 31);
            var startSummer = new DateTime(DateTime.Now.Year, 6, 1);
            var endSummer = new DateTime(DateTime.Now.Year, 8, 31);
            var startFall = new DateTime(DateTime.Now.Year, 9, 1);
            var endFall = new DateTime(DateTime.Now.Year, 11, 30);
            var thisYearStartWinter = new DateTime(DateTime.Now.Year, 12, 1);
            var lastYearStartWinter = new DateTime(lastWinter, 12, 1);
            var nextYearEndWinter = new DateTime(nextYear, 2, 28);
            var thisYearEndWinter = new DateTime(DateTime.Now.Year, 2, 28);

            
            var userFound = false;
            if (usersWithSameAddress.Count() > 1)
            {
                for (int i = 0; i < usersWithSameAddress.Count(); i++)
                {                   
                    if (usersWithSameAddress[i].Id != currentUserId.Id && userFound == false)
                    {
                        var remindersToAdd = await _context.UserReminders.Where(m => m.UserId == usersWithSameAddress[i].Id).ToListAsync();

                        foreach (UserReminders reminderAdded in remindersToAdd)
                        {
                            UserReminders reminder = new UserReminders();
                            reminder.ReminderId = reminderAdded.Id;
                            reminder.ReminderDescription = reminderAdded.ReminderDescription;
                            reminder.ReminderItem = reminderAdded.ReminderItem;
                            reminder.UserId = currentUserId.Id;
                            reminder.FName = currentUserId.FName;
                            reminder.LName = currentUserId.Lname;
                            reminder.Address = currentUserId.address;
                            reminder.NotificationInterval = reminderAdded.NotificationInterval;
                            reminder.SeasonSpring = reminderAdded.SeasonSpring;
                            reminder.SeasonSummer = reminderAdded.SeasonSummer;
                            reminder.SeasonFall = reminderAdded.SeasonFall;
                            reminder.SeasonWinter = reminderAdded.SeasonWinter;
                            reminder.ActionPlanId = reminderAdded.ActionPlanId;
                            reminder.ActionPlanTitle = reminderAdded.ActionPlanTitle;
                            reminder.ActionPlanCategory = reminderAdded.ActionPlanCategory;
                            reminder.ActionPlanLink = reminderAdded.ActionPlanLink;
                            reminder.ActionPlanSteps = reminderAdded.ActionPlanSteps;
                            reminder.Reminder = reminderAdded.Reminder;
                            reminder.DueDate = reminderAdded.DueDate;
                            reminder.LastCompleted = reminderAdded.LastCompleted;
                            reminder.PrevDueDate = reminderAdded.PrevDueDate;
                            reminder.NextStartDate = reminderAdded.NextStartDate;
                            reminder.Scheduled = reminderAdded.Scheduled;
                            reminder.Completed = reminderAdded.Completed;


                            _context.UserReminders.Add(reminder);
                            await _context.SaveChangesAsync();
                        }

                        userFound = true;
                    }
                }

                    

            }

            // If this user's home code equals another user's home code, set this user's reminders to that user's reminders
            /*foreach (MobileUsers user in allUsers) 
            {
                if (userFound == true)
                {
                    break;
                }

                if (currentUserId.Code == user.Code && currentUserId.Id != user.Id)
                {
                    userFound = true;
                    foreach (UserReminders userReminder in allUserReminders)
                    {
                        if (userReminder.UserId == user.Id)
                        {
                            UserReminders reminder = new UserReminders();
                            reminder.ReminderId = userReminder.Id;
                            reminder.ReminderDescription = userReminder.ReminderDescription;
                            reminder.ReminderItem = userReminder.ReminderItem;
                            reminder.UserId = currentUserId.Id;
                            reminder.FName = currentUserId.FName;
                            reminder.LName = currentUserId.Lname;
                            reminder.Address = currentUserId.address;
                            reminder.NotificationInterval = userReminder.NotificationInterval;
                            reminder.SeasonSpring = userReminder.SeasonSpring;
                            reminder.SeasonSummer = userReminder.SeasonSummer;
                            reminder.SeasonFall = userReminder.SeasonFall;
                            reminder.SeasonWinter = userReminder.SeasonWinter;
                            reminder.ActionPlanId = userReminder.ActionPlanId;
                            reminder.ActionPlanTitle = userReminder.ActionPlanTitle;
                            reminder.ActionPlanCategory = userReminder.ActionPlanCategory;
                            reminder.ActionPlanLink = userReminder.ActionPlanLink;
                            reminder.ActionPlanSteps = userReminder.ActionPlanSteps;
                            reminder.Reminder = userReminder.Reminder;
                            reminder.DueDate = userReminder.DueDate;
                            reminder.LastCompleted = userReminder.LastCompleted;
                            reminder.PrevDueDate = userReminder.PrevDueDate;
                            reminder.NextStartDate = userReminder.NextStartDate;
                            reminder.Scheduled = userReminder.Scheduled;
                            reminder.Completed = userReminder.Completed;

                            
                            _context.UserReminders.Add(reminder);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }*/

            // If the current mobile user is accessing the maintenance reminder page for the first time, create individualized reminders for each current maintenance reminder
            if (currentUserReminders.Count() == 0 && currentGlobalReminders.Count() != 0 && userFound == false)
            {
                for (int i = 0; i < currentGlobalReminders.Count(); i++)
                {
                    // Convert the user's registration date to DateTime object to use the AddDays() function based on the NotificationInterval
                    var date = DateTime.Today;
                    var newDate = date;


                    /*var timeUtc = DateTime.UtcNow;
                    var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    var timeEST = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);

                    var newDate = timeEST;
                    var date = timeEST;*/


                    /*DateTime temp1 = DateTime.Today;
                    DateTime day = new DateTime(temp1.Add(1).Year, temp1.AddMonths(1).Month, 1);
                    var currD = day.ToString("MM/dd/yyyy");*/

                    // Will be used if a reminder is not currently in season
                    var noDueDate = "";

                    /*// Grab the first of the month for the next month
                    DateTime temp2 = DateTime.Today;
                    DateTime month = new DateTime(temp2.AddMonths(1).Year, temp2.AddMonths(1).Month, 1);
                    var nextMonth = month.ToString("MM/dd/yyyy");*/

                    // If season is "All year"
                    if (((currentGlobalReminders[i].SeasonSpring == "false") && (currentGlobalReminders[i].SeasonSummer == "false") && (currentGlobalReminders[i].SeasonFall == "false") && (currentGlobalReminders[i].SeasonWinter == "false")) || ((currentGlobalReminders[i].SeasonSpring == "true") && (currentGlobalReminders[i].SeasonSummer == "true") && (currentGlobalReminders[i].SeasonFall == "true") && (currentGlobalReminders[i].SeasonWinter == "true")))
                    {
                        if (currentGlobalReminders[i].NotificationInterval == "Weekly")
                        {
                            newDate = date.AddDays(7);
                        }
                        else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                        {
                            newDate = date.AddDays(14);
                        }
                        else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                        {
                            newDate = date.AddDays(30);
                        }
                        else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                        {
                            newDate = date.AddDays(90);
                        }
                        else if (currentGlobalReminders[i].NotificationInterval == "Yearly")
                        {
                            newDate = date.AddDays(365);
                        }
                    }

                    // Spring
                    else if ((currentGlobalReminders[i].SeasonSpring == "true") && (currentGlobalReminders[i].SeasonSummer == "false") && (currentGlobalReminders[i].SeasonFall == "false") && (currentGlobalReminders[i].SeasonWinter == "false"))
                    {
                        if (date >= startSpring && date <= endSpring)
                        {
                            if (currentGlobalReminders[i].NotificationInterval == "Weekly")
                            {
                                if (date.AddDays(7) <= endSpring)
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if (date.AddDays(14) <= endSpring)
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                        }
                        else
                        {
                            noDueDate = "Not currently due";
                        }
                    }
                    // SpringSummer
                    else if ((currentGlobalReminders[i].SeasonSpring == "true") && (currentGlobalReminders[i].SeasonSummer == "true") && (currentGlobalReminders[i].SeasonFall == "false") && (currentGlobalReminders[i].SeasonWinter == "false"))
                    {
                        if (date >= startSpring && date <= endSummer)
                        {
                            if (currentGlobalReminders[i].NotificationInterval == "Weekly")
                            {
                                if (date.AddDays(7) <= endSummer)
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if (date.AddDays(14) <= endSummer)
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                        }
                        else
                        {
                            noDueDate = "Not currently due";
                        }
                    }
                    // SpringFall
                    else if ((currentGlobalReminders[i].SeasonSpring == "true") && (currentGlobalReminders[i].SeasonSummer == "false") && (currentGlobalReminders[i].SeasonFall == "true") && (currentGlobalReminders[i].SeasonWinter == "false"))
                    {
                        if ((date >= startSpring && date <= endSpring) || (date >= startFall && date <= endFall))
                        {
                            if (currentGlobalReminders[i].NotificationInterval == "Weekly")
                            {
                                if ((date.AddDays(7) <= endSpring) || (date.AddDays(7) <= endFall))
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if ((date.AddDays(14) <= endSpring) || (date.AddDays(14) <= endFall))
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if ((date.AddDays(20) <= endSpring) || (date.AddDays(20) <= endFall))
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endSummer;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endSummer;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                        }
                        else
                        {
                            noDueDate = "Not currently due";
                        }
                    }
                    // SpringWinter
                    else if ((currentGlobalReminders[i].SeasonSpring == "true") && (currentGlobalReminders[i].SeasonSummer == "false") && (currentGlobalReminders[i].SeasonFall == "false") && (currentGlobalReminders[i].SeasonWinter == "true"))
                    {
                        if ((date >= startSpring && date <= endSpring) || (date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                        {
                            if (currentGlobalReminders[i].NotificationInterval == "Weekly")
                            {
                                if ((date.AddDays(7) <= endSpring) || (date.AddDays(7) <= thisYearEndWinter) || (date.AddDays(7) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if ((date.AddDays(14) <= endSpring) || (date.AddDays(14) <= thisYearEndWinter) || (date.AddDays(14) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if ((date.AddDays(20) <= endSpring) || (date.AddDays(20) <= thisYearEndWinter) || (date.AddDays(20) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                        }
                        else
                        {
                            noDueDate = "Not currently due";
                        }
                    }
                    // SpringSummerFall
                    else if ((currentGlobalReminders[i].SeasonSpring == "true") && (currentGlobalReminders[i].SeasonSummer == "true") && (currentGlobalReminders[i].SeasonFall == "true") && (currentGlobalReminders[i].SeasonWinter == "false"))
                    {
                        if (date >= startSpring && date <= endFall)
                        {
                            if (currentGlobalReminders[i].NotificationInterval == "Weekly")
                            {
                                if (date.AddDays(7) <= endFall)
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if (date.AddDays(14) <= endFall)
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if (date.AddDays(20) <= endFall)
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                        }
                        else
                        {
                            noDueDate = "Not currently due";
                        }
                    }
                    // SpringFallWinter
                    else if ((currentGlobalReminders[i].SeasonSpring == "true") && (currentGlobalReminders[i].SeasonSummer == "false") && (currentGlobalReminders[i].SeasonFall == "true") && (currentGlobalReminders[i].SeasonWinter == "true"))
                    {
                        if ((date >= startSpring && date <= endSpring) || (date >= startFall && date <= endFall) || (date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                        {
                            if (currentGlobalReminders[i].NotificationInterval == "Weekly")
                            {
                                if ((date.AddDays(7) <= endSpring) || (date.AddDays(7) <= endFall) || (date.AddDays(7) <= thisYearEndWinter) || (date.AddDays(7) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if ((date.AddDays(14) <= endSpring) || (date.AddDays(14) <= endFall) || (date.AddDays(14) <= thisYearEndWinter) || (date.AddDays(14) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if ((date.AddDays(20) <= endSpring) || (date.AddDays(20) <= endFall) || (date.AddDays(20) <= thisYearEndWinter) || (date.AddDays(20) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                        }
                        else
                        {
                            noDueDate = "Not currently due";
                        }
                    }
                    // Summer
                    else if ((currentGlobalReminders[i].SeasonSpring == "false") && (currentGlobalReminders[i].SeasonSummer == "true") && (currentGlobalReminders[i].SeasonFall == "false") && (currentGlobalReminders[i].SeasonWinter == "false"))
                    {
                        if (date >= startSummer && date <= endSummer)
                        {
                            if (currentGlobalReminders[i].NotificationInterval == "Weekly")
                            {
                                if (date.AddDays(7) <= endSummer)
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if (date.AddDays(14) <= endSummer)
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                        }
                        else
                        {
                            noDueDate = "Not currently due";
                        }
                    }
                    // SummerFall
                    else if ((currentGlobalReminders[i].SeasonSpring == "false") && (currentGlobalReminders[i].SeasonSummer == "true") && (currentGlobalReminders[i].SeasonFall == "true") && (currentGlobalReminders[i].SeasonWinter == "false"))
                    {
                        if (date >= startSummer && date <= endFall)
                        {
                            if (currentGlobalReminders[i].NotificationInterval == "Weekly")
                            {
                                if (date.AddDays(7) <= endFall)
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if (date.AddDays(14) <= endFall)
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if (date.AddDays(20) <= endFall)
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                        }
                        else
                        {
                            noDueDate = "Not currently due";
                        }
                    }
                    // SummerWinter
                    else if ((currentGlobalReminders[i].SeasonSpring == "false") && (currentGlobalReminders[i].SeasonSummer == "true") && (currentGlobalReminders[i].SeasonFall == "false") && (currentGlobalReminders[i].SeasonWinter == "true"))
                    {
                        if ((date >= startSummer && date <= endSummer) || (date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                        {
                            if (currentGlobalReminders[i].NotificationInterval == "Weekly")
                            {
                                if ((date.AddDays(7) <= endSummer) || (date.AddDays(7) <= thisYearEndWinter) || (date.AddDays(7) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if ((date.AddDays(14) <= endSummer) || (date.AddDays(14) <= thisYearEndWinter) || (date.AddDays(14) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if ((date.AddDays(20) <= endSummer) || (date.AddDays(20) <= thisYearEndWinter) || (date.AddDays(20) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                        }
                        else
                        {
                            noDueDate = "Not currently due";
                        }
                    }
                    // SummerFallWinter
                    else if ((currentGlobalReminders[i].SeasonSpring == "false") && (currentGlobalReminders[i].SeasonSummer == "true") && (currentGlobalReminders[i].SeasonFall == "true") && (currentGlobalReminders[i].SeasonWinter == "true"))
                    {
                        if ((date >= startSummer && date <= nextYearEndWinter) || (date >= startSummer.AddYears(-1) && date <= thisYearEndWinter))
                        {
                            if (currentGlobalReminders[i].NotificationInterval == "Weekly")
                            {
                                if ((date.AddDays(7) <= nextYearEndWinter) || (date.AddDays(7) <= thisYearEndWinter))
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if ((date.AddDays(14) <= nextYearEndWinter) || (date.AddDays(14) <= thisYearEndWinter))
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if ((date.AddDays(20) <= nextYearEndWinter) || (date.AddDays(20) <= thisYearEndWinter))
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                        }
                        else
                        {
                            noDueDate = "Not currently due";
                        }
                    }
                    // Fall
                    else if ((currentGlobalReminders[i].SeasonSpring == "false") && (currentGlobalReminders[i].SeasonSummer == "false") && (currentGlobalReminders[i].SeasonFall == "true") && (currentGlobalReminders[i].SeasonWinter == "false"))
                    {
                        if (date >= startFall && date <= endFall)
                        {
                            if (currentGlobalReminders[i].NotificationInterval == "Weekly")
                            {
                                if (date.AddDays(7) <= endFall)
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if (date.AddDays(14) <= endFall)
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if (date.AddDays(20) <= endFall)
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                        }
                        else
                        {
                            noDueDate = "Not currently due";
                        }
                    }
                    // FallWinter
                    else if ((currentGlobalReminders[i].SeasonSpring == "false") && (currentGlobalReminders[i].SeasonSummer == "false") && (currentGlobalReminders[i].SeasonFall == "true") && (currentGlobalReminders[i].SeasonWinter == "true"))
                    {
                        if ((date >= startFall && date <= nextYearEndWinter) || (date >= startFall.AddYears(-1) && date <= thisYearEndWinter))
                        {
                            if (currentGlobalReminders[i].NotificationInterval == "Weekly")
                            {
                                if ((date.AddDays(7) <= nextYearEndWinter) || (date.AddDays(7) <= thisYearEndWinter))
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if ((date.AddDays(14) <= nextYearEndWinter) || (date.AddDays(14) <= thisYearEndWinter))
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if ((date.AddDays(20) <= nextYearEndWinter) || date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                        }
                        else
                        {
                            noDueDate = "Not currently due";
                        }
                    }
                    // Winter
                    else if ((currentGlobalReminders[i].SeasonSpring == "false") && (currentGlobalReminders[i].SeasonSummer == "false") && (currentGlobalReminders[i].SeasonFall == "false") && (currentGlobalReminders[i].SeasonWinter == "true"))
                    {
                        if ((date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                        {
                            if (currentGlobalReminders[i].NotificationInterval == "Weekly")
                            {
                                if ((date.AddDays(7) <= thisYearEndWinter) || (date.AddDays(7) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if ((date.AddDays(14) <= thisYearEndWinter) || (date.AddDays(14) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if ((date.AddDays(20) <= thisYearEndWinter) || (date.AddDays(20) <= nextYearEndWinter))
                                {
                                    newDate = date.AddDays(30);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Yearly")
                            {
                                if (date.AddDays(20) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(20) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                        }
                        else
                        {
                            noDueDate = "Not currently due";
                        }
                    }

                    UserReminders reminder = new UserReminders();
                    reminder.ReminderId = currentGlobalReminders[i].Id;
                    reminder.ReminderDescription = currentGlobalReminders[i].Description;
                    reminder.ReminderItem = currentGlobalReminders[i].ReminderItem;
                    reminder.UserId = currentUserId.Id;
                    reminder.FName = currentUserId.FName;
                    reminder.LName = currentUserId.Lname;
                    reminder.Address = currentUserId.address;
                    reminder.NotificationInterval = currentGlobalReminders[i].NotificationInterval;
                    reminder.SeasonSpring = currentGlobalReminders[i].SeasonSpring;
                    reminder.SeasonSummer = currentGlobalReminders[i].SeasonSummer;
                    reminder.SeasonFall = currentGlobalReminders[i].SeasonFall;
                    reminder.SeasonWinter = currentGlobalReminders[i].SeasonWinter;
                    reminder.ActionPlanId = currentGlobalReminders[i].ActionPlanId;
                    reminder.ActionPlanTitle = currentGlobalReminders[i].ActionPlanTitle;
                    reminder.ActionPlanCategory = currentGlobalReminders[i].ActionPlanCategory;
                    reminder.ActionPlanLink = currentGlobalReminders[i].ActionPlanLink;
                    reminder.ActionPlanSteps = currentGlobalReminders[i].ActionPlanSteps;
                    reminder.Reminder = currentGlobalReminders[i].Reminder;
                    reminder.DueDate = (DateTime)SqlDateTime.Null;
                    reminder.LastCompleted = (DateTime)SqlDateTime.Null;
                    reminder.PrevDueDate = (DateTime)SqlDateTime.Null;
                    reminder.NextStartDate = (DateTime)SqlDateTime.Null;

                    if (noDueDate != "Not currently due")
                    {
                        reminder.Scheduled = "true";
                        reminder.DueDate = newDate;
                        reminder.Completed = "Due";
                    }
                    else
                    {
                        // Leave DueDate null since it's not in season 
                        reminder.Completed = "Not in season";
                        reminder.Scheduled = "false";
                    }
                    _context.UserReminders.Add(reminder);
                    await _context.SaveChangesAsync();
                }
            }
            return NoContent();
        }
                                   

        /**
         * This is a PUT request.
         * The "5" that they use in the sample url below is the id of a specific action plan in the db
         * In order for this to work, you must know the particular action plan ID ahead of time
         **/
                    // PUT: api/MaintenanceReminderAPI/5
                    // To protect from overposting attacks, please enable the specific properties you want to bind to, for
                    // more details see https://aka.ms/RazorPagesCRUD.
                    [HttpPut("{ReminderId}")]
        public async Task<ActionResult<UserReminders>> PutContentModel(string reminderId, UserReminders userReminder)
        {
            int newId = Int32.Parse(reminderId);

            var reminder = await _context.UserReminders.FindAsync(newId);
            if (reminder == null)
            {
                return NotFound();
            }

            // Convert the user's registration date to DateTime object to use the AddDays() function based on the NotificationInterval
            var date = DateTime.Today;
            var newDate = date;

            // Gets the first day of the next week, which is Sunday
            var firstdayOfNextWeek = DateTime.Now.LastDayOfWeek();

            // Gets the first day of the next month
            var firstdayOfNextMonth = DateTime.Now.FirstDayOfNextMonth();

            // Gets the first day of next year
            var newYear = DateTime.Now.AddYears(1);
            var nextYear = newYear.Year;
            var firstDayOfNextYear = new DateTime(nextYear, 1, 1);

            // Gets the last day of the current year
            var lastDayOfYear = new DateTime(DateTime.Now.Year, 12, 31);

            // First day of last year's winter
            var tempWinter1 = new DateTime(DateTime.Now.Year, 12, 1);
            var tempWinter2 = tempWinter1.AddYears(-1);
            int lastWinter = tempWinter2.Year;

            // Season date ranges
            var startSpring = new DateTime(DateTime.Now.Year, 3, 1);
            var endSpring = new DateTime(DateTime.Now.Year, 5, 31);
            var startSummer = new DateTime(DateTime.Now.Year, 6, 1);
            var endSummer = new DateTime(DateTime.Now.Year, 8, 31);
            var startFall = new DateTime(DateTime.Now.Year, 9, 1);
            var endFall = new DateTime(DateTime.Now.Year, 11, 30);
            var thisYearStartWinter = new DateTime(DateTime.Now.Year, 12, 1);
            var lastYearStartWinter = new DateTime(lastWinter, 12, 1);
            var nextYearEndWinter = new DateTime(nextYear, 2, 28);
            var thisYearEndWinter = new DateTime(DateTime.Now.Year, 2, 28);

            // Set the current due date as the previous due date since it is about to be updated
            reminder.PrevDueDate = reminder.DueDate;
            reminder.DueDate = (DateTime)SqlDateTime.Null;

            reminder.Completed = "Completed";
            reminder.LastCompleted = userReminder.LastCompleted;
            reminder.Scheduled = "false";

            if (reminder.NotificationInterval == "Weekly")
            {
                reminder.NextStartDate = reminder.LastCompleted.AddDays(7);
            }
            else if (reminder.NotificationInterval == "Biweekly")
            {
                reminder.NextStartDate = reminder.LastCompleted.AddDays(14);
            }
            else if (reminder.NotificationInterval == "Monthly")
            {
                reminder.NextStartDate = reminder.LastCompleted.AddDays(30);
            }
            else if (reminder.NotificationInterval == "Quarterly")
            {
                // If the reminder is "All Year", set it to start 3 months from today. Otherwise, it's seasonal and should 
                //if (((reminder.SeasonSpring == "false") && (reminder.SeasonSummer == "false") && (reminder.SeasonFall == "false") && (reminder.SeasonWinter == "false")) || ((reminder.SeasonSpring == "true") && (reminder.SeasonSummer == "true") && (reminder.SeasonFall == "true") && (reminder.SeasonWinter == "true")))
                //{
                    reminder.NextStartDate = reminder.LastCompleted.AddDays(30);
                //}
                //else
                //{
                    //reminder.NextStartDate = reminder.LastCompleted.AddDays(7);
                //}
            }
            else if (reminder.NotificationInterval == "Yearly")
            {
                reminder.NextStartDate = reminder.LastCompleted.AddDays(30);
            }

            _context.UserReminders.Update(reminder);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public static partial class DateTimeExtensions
    {
        public static DateTime FirstDayOfWeek(this DateTime dt)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;

            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-diff).Date;
        }

        public static DateTime LastDayOfWeek(this DateTime dt) =>
            dt.FirstDayOfWeek().AddDays(6);

        public static DateTime FirstDayOfMonth(this DateTime dt) =>
            new DateTime(dt.Year, dt.Month, 1);

        public static DateTime LastDayOfMonth(this DateTime dt) =>
            dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);

        public static DateTime FirstDayOfNextMonth(this DateTime dt) =>
            dt.FirstDayOfMonth().AddMonths(1);
    }
}


