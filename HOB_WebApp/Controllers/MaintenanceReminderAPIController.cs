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
            var currentUser = await _context.MobileUsers.Where(m => (m.FName == mobileUsers.FName) && (m.Lname == mobileUsers.Lname) && (m.Code == mobileUsers.Code) && (m.date == mobileUsers.date)).ToListAsync();
            MobileUsers currentUserId = currentUser.Find(m => m.Code == mobileUsers.Code);

            // Grab the current maintenance reminders and store them in a list
            var currentGlobalReminders = await _context.MaintenanceReminders.ToListAsync();
            var currentUserReminders = await _context.UserReminders.Where(m => m.UserId == currentUserId.Id).ToListAsync();
            var allUserReminders = await _context.UserReminders.ToListAsync();


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


            // If the current mobile user is accessing the maintenance reminder page for the first time, create individualized reminders for each current maintenance reminder
            if (currentUserReminders.Count() == 0 && currentGlobalReminders.Count() != 0)
            {
                for (int i = 0; i < currentGlobalReminders.Count(); i++)
                {
                    // Convert the user's registration date to DateTime object to use the AddDays() function based on the NotificationInterval
                    var date = DateTime.Today;
                    var newDate = date;

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
                            newDate = firstdayOfNextWeek.AddDays(7);
                        }
                        else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                        {
                            newDate = firstdayOfNextWeek.AddDays(14);
                        }
                        else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                        {
                            newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                        }
                        else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                        {
                            newDate = firstdayOfNextMonth.AddMonths(3).AddDays(-1);
                            // Set to last day of the third month

                        }
                        else if (currentGlobalReminders[i].NotificationInterval == "Yearly")
                        {
                            if (date.AddMonths(1) <= lastDayOfYear)
                            {
                                newDate = lastDayOfYear;
                            }
                            else
                            {
                                // Set to last day of next year
                                newDate = new DateTime(nextYear, 12, 31);
                            }
                        }
                    }

                    // Spring
                    else if ((currentGlobalReminders[i].SeasonSpring == "true") && (currentGlobalReminders[i].SeasonSummer == "false") && (currentGlobalReminders[i].SeasonFall == "false") && (currentGlobalReminders[i].SeasonWinter == "false"))
                    {
                        if (date >= startSpring && date <= endSpring)
                        {
                            if (currentGlobalReminders[i].NotificationInterval == "Weekly")
                            {
                                if (firstdayOfNextWeek.AddDays(7) <= endSpring)
                                {
                                    newDate = firstdayOfNextWeek.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if (firstdayOfNextWeek.AddDays(14) <= endSpring)
                                {
                                    newDate = firstdayOfNextWeek.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endSpring)
                                {
                                    newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(14) <= endSpring)
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
                                if (date.AddDays(14) <= endSpring)
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
                                if (firstdayOfNextWeek.AddDays(7) <= endSummer)
                                {
                                    newDate = firstdayOfNextWeek.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if (firstdayOfNextWeek.AddDays(14) <= endSummer)
                                {
                                    newDate = firstdayOfNextWeek.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endSummer)
                                {
                                    newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(14) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(14) <= endSummer)
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
                                if (date.AddDays(14) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(14) <= endSummer)
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
                                if ((firstdayOfNextWeek.AddDays(7) <= endSpring) || (firstdayOfNextWeek.AddDays(7) <= endFall))
                                {
                                    newDate = firstdayOfNextWeek.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if ((firstdayOfNextWeek.AddDays(14) <= endSpring) || (firstdayOfNextWeek.AddDays(14) <= endFall))
                                {
                                    newDate = firstdayOfNextWeek.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if ((firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endSpring) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endFall))
                                {
                                    newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(14) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(14) <= endFall)
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
                                if (date.AddDays(14) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(14) <= endFall)
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
                                if ((firstdayOfNextWeek.AddDays(7) <= endSpring) || (firstdayOfNextWeek.AddDays(7) <= thisYearEndWinter) || (firstdayOfNextWeek.AddDays(7) <= nextYearEndWinter))
                                {
                                    newDate = firstdayOfNextWeek.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if ((firstdayOfNextWeek.AddDays(14) <= endSpring) || (firstdayOfNextWeek.AddDays(14) <= thisYearEndWinter) || (firstdayOfNextWeek.AddDays(14) <= nextYearEndWinter))
                                {
                                    newDate = firstdayOfNextWeek.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if ((firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endSpring) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= thisYearEndWinter) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= nextYearEndWinter))
                                {
                                    newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(14) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(14) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(14) <= nextYearEndWinter)
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
                                if (date.AddDays(14) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(14) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(14) <= nextYearEndWinter)
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
                                if (firstdayOfNextWeek.AddDays(7) <= endFall)
                                {
                                    newDate = firstdayOfNextWeek.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if (firstdayOfNextWeek.AddDays(14) <= endFall)
                                {
                                    newDate = firstdayOfNextWeek.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endFall)
                                {
                                    newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(14) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(14) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(14) <= endFall)
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
                                if (date.AddDays(14) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(14) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(14) <= endFall)
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
                                if ((firstdayOfNextWeek.AddDays(7) <= endSpring) || (firstdayOfNextWeek.AddDays(7) <= endFall) || (firstdayOfNextWeek.AddDays(7) <= thisYearEndWinter) || (firstdayOfNextWeek.AddDays(7) <= nextYearEndWinter))
                                {
                                    newDate = firstdayOfNextWeek.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if ((firstdayOfNextWeek.AddDays(14) <= endSpring) || (firstdayOfNextWeek.AddDays(14) <= endFall) || (firstdayOfNextWeek.AddDays(14) <= thisYearEndWinter) || (firstdayOfNextWeek.AddDays(14) <= nextYearEndWinter))
                                {
                                    newDate = firstdayOfNextWeek.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if ((firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endSpring) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endFall) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= thisYearEndWinter) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= nextYearEndWinter))
                                {
                                    newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(14) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(14) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else if (date.AddDays(14) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(14) <= nextYearEndWinter)
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
                                if (date.AddDays(14) <= endSpring)
                                {
                                    newDate = endSpring;
                                }
                                else if (date.AddDays(14) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else if (date.AddDays(14) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(14) <= nextYearEndWinter)
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
                                if (firstdayOfNextWeek.AddDays(7) <= endSummer)
                                {
                                    newDate = firstdayOfNextWeek.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if (firstdayOfNextWeek.AddDays(14) <= endSummer)
                                {
                                    newDate = firstdayOfNextWeek.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endSummer)
                                {
                                    newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(14) <= endSummer)
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
                                if (date.AddDays(14) <= endSummer)
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
                                if (firstdayOfNextWeek.AddDays(7) <= endFall)
                                {
                                    newDate = firstdayOfNextWeek.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if (firstdayOfNextWeek.AddDays(14) <= endFall)
                                {
                                    newDate = firstdayOfNextWeek.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endFall)
                                {
                                    newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(14) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(14) <= endFall)
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
                                if (date.AddDays(14) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(14) <= endFall)
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
                                if ((firstdayOfNextWeek.AddDays(7) <= endSummer) || (firstdayOfNextWeek.AddDays(7) <= thisYearEndWinter) || (firstdayOfNextWeek.AddDays(7) <= nextYearEndWinter))
                                {
                                    newDate = firstdayOfNextWeek.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if ((firstdayOfNextWeek.AddDays(14) <= endSummer) || (firstdayOfNextWeek.AddDays(14) <= thisYearEndWinter) || (firstdayOfNextWeek.AddDays(14) <= nextYearEndWinter))
                                {
                                    newDate = firstdayOfNextWeek.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if ((firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endSummer) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= thisYearEndWinter) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= nextYearEndWinter))
                                {
                                    newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(14) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(14) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(14) <= nextYearEndWinter)
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
                                if (date.AddDays(14) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(14) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(14) <= nextYearEndWinter)
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
                                if ((firstdayOfNextWeek.AddDays(7) <= nextYearEndWinter) || (firstdayOfNextWeek.AddDays(7) <= thisYearEndWinter))
                                {
                                    newDate = firstdayOfNextWeek.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if ((firstdayOfNextWeek.AddDays(14) <= nextYearEndWinter) || (firstdayOfNextWeek.AddDays(14) <= thisYearEndWinter))
                                {
                                    newDate = firstdayOfNextWeek.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if ((firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= nextYearEndWinter) || firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= thisYearEndWinter)
                                {
                                    newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(14) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(14) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else if (date.AddDays(14) <= thisYearEndWinter)
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
                                if (date.AddDays(14) <= endSummer)
                                {
                                    newDate = endSummer;
                                }
                                else if (date.AddDays(14) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else if (date.AddDays(14) <= thisYearEndWinter)
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
                                if (firstdayOfNextWeek.AddDays(7) <= endFall)
                                {
                                    newDate = firstdayOfNextWeek.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if (firstdayOfNextWeek.AddDays(14) <= endFall)
                                {
                                    newDate = firstdayOfNextWeek.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endFall)
                                {
                                    newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(14) <= endFall)
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
                                if (date.AddDays(14) <= endFall)
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
                                if ((firstdayOfNextWeek.AddDays(7) <= nextYearEndWinter) || (firstdayOfNextWeek.AddDays(7) <= thisYearEndWinter))
                                {
                                    newDate = firstdayOfNextWeek.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if ((firstdayOfNextWeek.AddDays(14) <= nextYearEndWinter) || (firstdayOfNextWeek.AddDays(14) <= thisYearEndWinter))
                                {
                                    newDate = firstdayOfNextWeek.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if ((firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= nextYearEndWinter) || firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= thisYearEndWinter)
                                {
                                    newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(14) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else if (date.AddDays(14) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else if (date.AddDays(14) <= thisYearEndWinter)
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
                                if (date.AddDays(14) <= endFall)
                                {
                                    newDate = endFall;
                                }
                                else if (date.AddDays(14) <= nextYearEndWinter)
                                {
                                    newDate = nextYearEndWinter;
                                }
                                else if (date.AddDays(14) <= thisYearEndWinter)
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
                                if ((firstdayOfNextWeek.AddDays(7) <= thisYearEndWinter) || (firstdayOfNextWeek.AddDays(7) <= nextYearEndWinter))
                                {
                                    newDate = firstdayOfNextWeek.AddDays(7);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Biweekly")
                            {
                                if ((firstdayOfNextWeek.AddDays(14) <= thisYearEndWinter) || (firstdayOfNextWeek.AddDays(14) <= nextYearEndWinter))
                                {
                                    newDate = firstdayOfNextWeek.AddDays(14);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Monthly")
                            {
                                if ((firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= thisYearEndWinter) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= nextYearEndWinter))
                                {
                                    newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                }
                                else
                                {
                                    noDueDate = "Not currently due";
                                }
                            }
                            else if (currentGlobalReminders[i].NotificationInterval == "Quarterly")
                            {
                                if (date.AddDays(14) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(14) <= nextYearEndWinter)
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
                                if (date.AddDays(14) <= thisYearEndWinter)
                                {
                                    newDate = thisYearEndWinter;
                                }
                                else if (date.AddDays(14) <= nextYearEndWinter)
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

                    if (noDueDate != "Not currently due")
                    {
                        reminder.DueDate = newDate.ToString("MM/dd/yyyy");
                        reminder.Completed = "Due";

                        var nextDueDate = "";
                        var tempNextDueDate = Convert.ToDateTime(reminder.DueDate);

                        if (reminder.NotificationInterval == "Weekly")
                        {
                            nextDueDate = tempNextDueDate.AddDays(7).ToString("MM/dd/yyyy");
                        }
                        else if (reminder.NotificationInterval == "Biweekly")
                        {
                            nextDueDate = tempNextDueDate.AddDays(14).ToString("MM/dd/yyyy");
                        }
                        else if (reminder.NotificationInterval == "Monthly")
                        {
                            nextDueDate = tempNextDueDate.AddMonths(1).ToString("MM/dd/yyyy");
                        }
                        else if (reminder.NotificationInterval == "Quarterly")
                        {
                            nextDueDate = tempNextDueDate.AddMonths(3).ToString("MM/dd/yyyy");
                        }
                        else if (reminder.NotificationInterval == "Yearly")
                        {
                            // Set to last day of next year
                            nextDueDate = new DateTime(nextYear, 12, 31).ToString("MM/dd/yyyy");
                        }

                        reminder.PrevDueDate = reminder.DueDate;
                        reminder.NextDueDate = nextDueDate;

                    }
                    else
                    {
                        reminder.DueDate = noDueDate;
                        reminder.Completed = "Not in season";
                    }
                    _context.UserReminders.Add(reminder);
                    await _context.SaveChangesAsync();
                }
            }

            // If the current mobile user is accessing the maintenance reminder after being assigned reminders
            else if (currentUserReminders.Count() > 0 && currentGlobalReminders.Count() != 0)
            {
                foreach (UserReminders userReminder in allUserReminders)
                {
                    // Convert the user's registration date to DateTime object to use the AddDays() function based on the NotificationInterval
                    var date = DateTime.Today;
                    var newDate = date;

                    /*DateTime temp1 = DateTime.Today;
                    DateTime day = new DateTime(temp1.Add(1).Year, temp1.AddMonths(1).Month, 1);
                    var currD = day.ToString("MM/dd/yyyy");*/

                    // Will be used if a reminder is not currently in season
                    var noDueDate = "";

                    // If the reminder is due and the current date is later than the due date, mark it as Overdue
                    if (userReminder.DueDate != "Not currently due")
                    {
                        DateTime currDate = DateTime.Now;
                        DateTime dueDate = Convert.ToDateTime(userReminder.DueDate);
                        //DateTime lastCompletedDate = Convert.ToDateTime(reminders.LastCompleted);

                        int dateDiff = DateTime.Compare(dueDate, currDate);

                        // If the current date is later than the due date, mark the user's task as Overdue
                        if (dateDiff < 0)
                        {
                            userReminder.Completed = "Overdue";
                            _context.UserReminders.Update(userReminder);
                            await _context.SaveChangesAsync();
                        }
                    }

                    var lastCompletedDate = Convert.ToDateTime(userReminder.LastCompleted);
                    int checkDates = (date.Date - lastCompletedDate.Date).Days;

                    //var nextDueDate = string.Concat("hello".Reverse().Skip(3).Reverse());

                    // The reminder is waiting for a new due date, so check against the current date to assign it 
                    if (userReminder.Completed == "Completed" || userReminder.Completed == "Not in season")
                    {
                        // If season is "All year"
                        if (((userReminder.SeasonSpring == "false") && (userReminder.SeasonSummer == "false") && (userReminder.SeasonFall == "false") && (userReminder.SeasonWinter == "false")) || ((userReminder.SeasonSpring == "true") && (userReminder.SeasonSummer == "true") && (userReminder.SeasonFall == "true") && (userReminder.SeasonWinter == "true")))
                        {
                            if (userReminder.NotificationInterval == "Weekly" && (date >= newDate))
                            {
                                newDate = firstdayOfNextWeek.AddDays(7);
                            }
                            else if (userReminder.NotificationInterval == "Biweekly")
                            {
                                newDate = firstdayOfNextWeek.AddDays(14);
                            }
                            else if (userReminder.NotificationInterval == "Monthly")
                            {
                                newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                            }
                            else if (userReminder.NotificationInterval == "Quarterly")
                            {
                                newDate = firstdayOfNextMonth.AddMonths(3).AddDays(-1);
                                // Set to last day of the third month

                            }
                            else if (userReminder.NotificationInterval == "Yearly")
                            {
                                if (date.AddMonths(1) <= lastDayOfYear)
                                {
                                    newDate = lastDayOfYear;
                                }
                                else
                                {
                                    // Set to last day of next year
                                    newDate = new DateTime(nextYear, 12, 31);
                                }
                            }
                        }

                        // Spring
                        else if ((userReminder.SeasonSpring == "true") && (userReminder.SeasonSummer == "false") && (userReminder.SeasonFall == "false") && (userReminder.SeasonWinter == "false"))
                        {
                            if (date >= startSpring && date <= endSpring)
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if (firstdayOfNextWeek.AddDays(7) <= endSpring)
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if (firstdayOfNextWeek.AddDays(14) <= endSpring)
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endSpring)
                                    {
                                        newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(14) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(14) <= endSpring)
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
                        else if ((userReminder.SeasonSpring == "true") && (userReminder.SeasonSummer == "true") && (userReminder.SeasonFall == "false") && (userReminder.SeasonWinter == "false"))
                        {
                            if (date >= startSpring && date <= endSummer)
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if (firstdayOfNextWeek.AddDays(7) <= endSummer)
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if (firstdayOfNextWeek.AddDays(14) <= endSummer)
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endSummer)
                                    {
                                        newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(14) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(14) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(14) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(14) <= endSummer)
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
                        else if ((userReminder.SeasonSpring == "true") && (userReminder.SeasonSummer == "false") && (userReminder.SeasonFall == "true") && (userReminder.SeasonWinter == "false"))
                        {
                            if ((date >= startSpring && date <= endSpring) || (date >= startFall && date <= endFall))
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if ((firstdayOfNextWeek.AddDays(7) <= endSpring) || (firstdayOfNextWeek.AddDays(7) <= endFall))
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if ((firstdayOfNextWeek.AddDays(14) <= endSpring) || (firstdayOfNextWeek.AddDays(14) <= endFall))
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if ((firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endSpring) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endFall))
                                    {
                                        newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(14) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(14) <= endFall)
                                    {
                                        newDate = endSummer;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(14) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(14) <= endFall)
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
                        else if ((userReminder.SeasonSpring == "true") && (userReminder.SeasonSummer == "false") && (userReminder.SeasonFall == "false") && (userReminder.SeasonWinter == "true"))
                        {
                            if ((date >= startSpring && date <= endSpring) || (date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if ((firstdayOfNextWeek.AddDays(7) <= endSpring) || (firstdayOfNextWeek.AddDays(7) <= thisYearEndWinter) || (firstdayOfNextWeek.AddDays(7) <= nextYearEndWinter))
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if ((firstdayOfNextWeek.AddDays(14) <= endSpring) || (firstdayOfNextWeek.AddDays(14) <= thisYearEndWinter) || (firstdayOfNextWeek.AddDays(14) <= nextYearEndWinter))
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if ((firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endSpring) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= thisYearEndWinter) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= nextYearEndWinter))
                                    {
                                        newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(14) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(14) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (date.AddDays(14) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(14) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(14) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (date.AddDays(14) <= nextYearEndWinter)
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
                        else if ((userReminder.SeasonSpring == "true") && (userReminder.SeasonSummer == "true") && (userReminder.SeasonFall == "true") && (userReminder.SeasonWinter == "false"))
                        {
                            if (date >= startSpring && date <= endFall)
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if (firstdayOfNextWeek.AddDays(7) <= endFall)
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if (firstdayOfNextWeek.AddDays(14) <= endFall)
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endFall)
                                    {
                                        newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(14) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(14) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (date.AddDays(14) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(14) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(14) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (date.AddDays(14) <= endFall)
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
                        else if ((userReminder.SeasonSpring == "true") && (userReminder.SeasonSummer == "false") && (userReminder.SeasonFall == "true") && (userReminder.SeasonWinter == "true"))
                        {
                            if ((date >= startSpring && date <= endSpring) || (date >= startFall && date <= endFall) || (date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if ((firstdayOfNextWeek.AddDays(7) <= endSpring) || (firstdayOfNextWeek.AddDays(7) <= endFall) || (firstdayOfNextWeek.AddDays(7) <= thisYearEndWinter) || (firstdayOfNextWeek.AddDays(7) <= nextYearEndWinter))
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if ((firstdayOfNextWeek.AddDays(14) <= endSpring) || (firstdayOfNextWeek.AddDays(14) <= endFall) || (firstdayOfNextWeek.AddDays(14) <= thisYearEndWinter) || (firstdayOfNextWeek.AddDays(14) <= nextYearEndWinter))
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if ((firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endSpring) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endFall) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= thisYearEndWinter) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= nextYearEndWinter))
                                    {
                                        newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(14) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(14) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else if (date.AddDays(14) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (date.AddDays(14) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(14) <= endSpring)
                                    {
                                        newDate = endSpring;
                                    }
                                    else if (date.AddDays(14) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else if (date.AddDays(14) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (date.AddDays(14) <= nextYearEndWinter)
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
                        else if ((userReminder.SeasonSpring == "false") && (userReminder.SeasonSummer == "true") && (userReminder.SeasonFall == "false") && (userReminder.SeasonWinter == "false"))
                        {
                            if (date >= startSummer && date <= endSummer)
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if (firstdayOfNextWeek.AddDays(7) <= endSummer)
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if (firstdayOfNextWeek.AddDays(14) <= endSummer)
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endSummer)
                                    {
                                        newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(14) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(14) <= endSummer)
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
                        else if ((userReminder.SeasonSpring == "false") && (userReminder.SeasonSummer == "true") && (userReminder.SeasonFall == "true") && (userReminder.SeasonWinter == "false"))
                        {
                            if (date >= startSummer && date <= endFall)
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if (firstdayOfNextWeek.AddDays(7) <= endFall)
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if (firstdayOfNextWeek.AddDays(14) <= endFall)
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endFall)
                                    {
                                        newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(14) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (date.AddDays(14) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(14) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (date.AddDays(14) <= endFall)
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
                        else if ((userReminder.SeasonSpring == "false") && (userReminder.SeasonSummer == "true") && (userReminder.SeasonFall == "false") && (userReminder.SeasonWinter == "true"))
                        {
                            if ((date >= startSummer && date <= endSummer) || (date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if ((firstdayOfNextWeek.AddDays(7) <= endSummer) || (firstdayOfNextWeek.AddDays(7) <= thisYearEndWinter) || (firstdayOfNextWeek.AddDays(7) <= nextYearEndWinter))
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if ((firstdayOfNextWeek.AddDays(14) <= endSummer) || (firstdayOfNextWeek.AddDays(14) <= thisYearEndWinter) || (firstdayOfNextWeek.AddDays(14) <= nextYearEndWinter))
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if ((firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endSummer) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= thisYearEndWinter) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= nextYearEndWinter))
                                    {
                                        newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(14) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (date.AddDays(14) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (date.AddDays(14) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(14) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (date.AddDays(14) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (date.AddDays(14) <= nextYearEndWinter)
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
                        else if ((userReminder.SeasonSpring == "false") && (userReminder.SeasonSummer == "true") && (userReminder.SeasonFall == "true") && (userReminder.SeasonWinter == "true"))
                        {
                            if ((date >= startSummer && date <= nextYearEndWinter) || (date >= startSummer.AddYears(-1) && date <= thisYearEndWinter))
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if ((firstdayOfNextWeek.AddDays(7) <= nextYearEndWinter) || (firstdayOfNextWeek.AddDays(7) <= thisYearEndWinter))
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if ((firstdayOfNextWeek.AddDays(14) <= nextYearEndWinter) || (firstdayOfNextWeek.AddDays(14) <= thisYearEndWinter))
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if ((firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= nextYearEndWinter) || firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= thisYearEndWinter)
                                    {
                                        newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(14) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (date.AddDays(14) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else if (date.AddDays(14) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(14) <= endSummer)
                                    {
                                        newDate = endSummer;
                                    }
                                    else if (date.AddDays(14) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else if (date.AddDays(14) <= thisYearEndWinter)
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
                        else if ((userReminder.SeasonSpring == "false") && (userReminder.SeasonSummer == "false") && (userReminder.SeasonFall == "true") && (userReminder.SeasonWinter == "false"))
                        {
                            if (date >= startFall && date <= endFall)
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if (firstdayOfNextWeek.AddDays(7) <= endFall)
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if (firstdayOfNextWeek.AddDays(14) <= endFall)
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= endFall)
                                    {
                                        newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(14) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(14) <= endFall)
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
                        else if ((userReminder.SeasonSpring == "false") && (userReminder.SeasonSummer == "false") && (userReminder.SeasonFall == "true") && (userReminder.SeasonWinter == "true"))
                        {
                            if ((date >= startFall && date <= nextYearEndWinter) || (date >= startFall.AddYears(-1) && date <= thisYearEndWinter))
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if ((firstdayOfNextWeek.AddDays(7) <= nextYearEndWinter) || (firstdayOfNextWeek.AddDays(7) <= thisYearEndWinter))
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if ((firstdayOfNextWeek.AddDays(14) <= nextYearEndWinter) || (firstdayOfNextWeek.AddDays(14) <= thisYearEndWinter))
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if ((firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= nextYearEndWinter) || firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= thisYearEndWinter)
                                    {
                                        newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(14) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else if (date.AddDays(14) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else if (date.AddDays(14) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(14) <= endFall)
                                    {
                                        newDate = endFall;
                                    }
                                    else if (date.AddDays(14) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else if (date.AddDays(14) <= thisYearEndWinter)
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
                        else if ((userReminder.SeasonSpring == "false") && (userReminder.SeasonSummer == "false") && (userReminder.SeasonFall == "false") && (userReminder.SeasonWinter == "true"))
                        {
                            if ((date >= lastYearStartWinter && date <= thisYearEndWinter) || (date >= thisYearStartWinter && date <= nextYearEndWinter))
                            {
                                if (userReminder.NotificationInterval == "Weekly")
                                {
                                    if ((firstdayOfNextWeek.AddDays(7) <= thisYearEndWinter) || (firstdayOfNextWeek.AddDays(7) <= nextYearEndWinter))
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(7);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Biweekly")
                                {
                                    if ((firstdayOfNextWeek.AddDays(14) <= thisYearEndWinter) || (firstdayOfNextWeek.AddDays(14) <= nextYearEndWinter))
                                    {
                                        newDate = firstdayOfNextWeek.AddDays(14);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Monthly")
                                {
                                    if ((firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= thisYearEndWinter) || (firstdayOfNextMonth.AddMonths(1).AddDays(-1) <= nextYearEndWinter))
                                    {
                                        newDate = firstdayOfNextMonth.AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Quarterly")
                                {
                                    if (date.AddDays(14) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (date.AddDays(14) <= nextYearEndWinter)
                                    {
                                        newDate = nextYearEndWinter;
                                    }
                                    else
                                    {
                                        noDueDate = "Not currently due";
                                    }
                                }
                                else if (userReminder.NotificationInterval == "Yearly")
                                {
                                    if (date.AddDays(14) <= thisYearEndWinter)
                                    {
                                        newDate = thisYearEndWinter;
                                    }
                                    else if (date.AddDays(14) <= nextYearEndWinter)
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



                        if (noDueDate != "Not currently due")
                        {
                            userReminder.DueDate = newDate.ToString("MM/dd/yyyy");
                            userReminder.Completed = "Due";

                            var nextDueDate = "";
                            var tempNextDueDate = Convert.ToDateTime(userReminder.DueDate);
                            
                            if (userReminder.NotificationInterval == "Weekly")
                            {
                                nextDueDate = tempNextDueDate.AddDays(7).ToString("MM/dd/yyyy");
                            }
                            else if (userReminder.NotificationInterval == "Biweekly")
                            {
                                nextDueDate = tempNextDueDate.AddDays(14).ToString("MM/dd/yyyy");
                            }
                            else if (userReminder.NotificationInterval == "Monthly")
                            {
                                nextDueDate = tempNextDueDate.AddMonths(1).ToString("MM/dd/yyyy");
                            }
                            else if (userReminder.NotificationInterval == "Quarterly")
                            {
                                nextDueDate = tempNextDueDate.AddMonths(3).ToString("MM/dd/yyyy");
                            }
                            else if (userReminder.NotificationInterval == "Yearly")
                            {
                                // Set to last day of next year
                                nextDueDate = new DateTime(nextYear, 12, 31).ToString("MM/dd/yyyy");
                            }

                            userReminder.PrevDueDate = userReminder.DueDate;
                            userReminder.NextDueDate = nextDueDate;
                            
                        }
                        else
                        {
                            userReminder.DueDate = noDueDate;
                            userReminder.Completed = "Not in season";
                        }
                        _context.UserReminders.Add(userReminder);
                        await _context.SaveChangesAsync();
                    }
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
        public async Task<ActionResult<UserReminders>> PutContentModel(string reminderId)
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

            var tempPrevDueDate = Convert.ToDateTime(reminder.DueDate);
            var prevDueDate = tempPrevDueDate.AddDays(1);

            if (reminder.Completed != "Overdue" && reminder.Completed != "Completed" && reminder.Completed != "Not in season")
            {
                if (reminder.NotificationInterval == "Weekly")
                {
                    reminder.DueDate = "Next weekly notification: " + prevDueDate.ToString("MM/dd/yyyy");
                }
                else if (reminder.NotificationInterval == "Biweekly")
                {
                    reminder.DueDate = "Next biweekly notification: " + prevDueDate.AddDays(7).ToString("MM/dd/yyyy");
                }
                else if (reminder.NotificationInterval == "Monthly")
                {
                    reminder.DueDate = "Next monthly notification: " + prevDueDate.AddMonths(1).ToString("MM/dd/yyyy");
                }
                else if (reminder.NotificationInterval == "Quarterly")
                {
                    reminder.DueDate = "Next quarterly notification: " + prevDueDate.AddMonths(3).ToString("MM/dd/yyyy");
                }
                else if (reminder.NotificationInterval == "Yearly")
                {
                    // Set to last day of next year
                    //reminder.DueDate = new DateTime(nextYear, 12, 31).ToString("MM/dd/yyyy");

                    reminder.DueDate = "Next yearly notification: " + firstDayOfNextYear.ToString("MM/dd/yyyy");
                }

                reminder.Completed = "Completed";
                var timeUtc = DateTime.UtcNow;
                var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                reminder.LastCompleted = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone).ToString("MM/dd/yyyy");
            }
            else
            {
                // Code to deal with setting date after marking done while task is overdue
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


