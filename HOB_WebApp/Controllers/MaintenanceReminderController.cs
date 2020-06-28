using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using HOB_WebApp.Data;
using HOB_WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HOB_WebApp.Controllers
{
    public class MaintenanceReminderController : Controller
    {
        private readonly HOB_WebAppContext _context;

        public MaintenanceReminderController(HOB_WebAppContext context)
        {
            _context = context;
        }

        // GET: MaintenanceReminders
        public async Task<IActionResult> Index()
        {
            return View(await _context.MaintenanceReminders.ToListAsync());
        }

        // GET: MaintenanceReminders
        public async Task<IActionResult> Status()
        {
            return View(await _context.UserReminders.OrderBy(m => m.UserId).ToListAsync());
        }

        // GET: MaintenanceReminders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceReminders = await _context.MaintenanceReminders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenanceReminders == null)
            {
                return NotFound();
            }

            return View(maintenanceReminders);
        }

        // GET: MaintenanceReminders/Status/Details/5
        public async Task<IActionResult> UserReminderDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userReminders = await _context.UserReminders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userReminders == null)
            {
                return NotFound();
            }

            return View(userReminders);
        }

        // GET: MaintenanceReminders/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.ActionPlans = await _context.ContentModel.ToListAsync();
            return View();
        }

        // POST: MaintenanceReminders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Reminder,Description,NotificationInterval,ActionPlanId,ActionPlanTitle,ActionPlanCategory,SeasonSpring,SeasonSummer,SeasonFall,SeasonWinter")] MaintenanceReminders maintenanceReminders)
        {

            if (ModelState.IsValid)
            {
                // Grab the selected action plan's ID and Category for later use
                var currentAPList = await _context.ContentModel.Where(m => m.Title == maintenanceReminders.ActionPlanTitle).ToListAsync();
                ContentModel currentAP = currentAPList.Find(m => m.Title == maintenanceReminders.ActionPlanTitle);
                maintenanceReminders.ActionPlanId = currentAP.Id;
                maintenanceReminders.ActionPlanCategory = currentAP.Category;

                _context.Add(maintenanceReminders);
                await _context.SaveChangesAsync();

                var mobileUserList = await _context.MobileUsers.ToListAsync();

                // After the new minatenance reminder has been added to the db, add an entry for that reminder to each current mobile user on the Reminder Status page
                foreach (MobileUsers mobileUser in mobileUserList)
                {
                    UserReminders userReminder = new UserReminders();

                    userReminder.ReminderId = maintenanceReminders.Id;
                    userReminder.UserId = mobileUser.Id;
                    userReminder.FName = mobileUser.FName;
                    userReminder.LName = mobileUser.Lname;
                    userReminder.Address = mobileUser.address;
                    userReminder.Completed = "No";
                    userReminder.Reminder = maintenanceReminders.Reminder;
                    _context.UserReminders.Add(userReminder);
                    await _context.SaveChangesAsync();
                }

                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(maintenanceReminders);
        }

        // GET: MaintenanceReminders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.ActionPlans = await _context.ContentModel.ToListAsync();

            var maintenanceReminders = await _context.MaintenanceReminders.FindAsync(id);
            if (maintenanceReminders == null)
            {
                return NotFound();
            }
            return View(maintenanceReminders);
        }

        // POST: MaintenanceReminders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Reminder,Description,NotificationInterval,ActionPlanId,ActionPlanTitle,ActionPlanCategory,SeasonSpring,SeasonSummer,SeasonFall,SeasonWinter")] MaintenanceReminders maintenanceReminders)
        {
            if (id != maintenanceReminders.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Grab the action plan ID and Category tied to the maintenance reminder to be able to update it
                    var currentAPList = await _context.ContentModel.Where(m => m.Title == maintenanceReminders.ActionPlanTitle).ToListAsync();
                    ContentModel currentAP = currentAPList.Find(m => m.Title == maintenanceReminders.ActionPlanTitle);
                    maintenanceReminders.ActionPlanId = currentAP.Id;
                    maintenanceReminders.ActionPlanCategory = currentAP.Category;

                    _context.Update(maintenanceReminders);
                    await _context.SaveChangesAsync();

                    var userReminderList = await _context.UserReminders.Where(m => m.ReminderId == id).ToListAsync();

                    foreach (UserReminders userReminder in userReminderList)
                    {
                        _context.UserReminders.Update(userReminder);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaintenanceRemindersExists(maintenanceReminders.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(maintenanceReminders);
        }

        // GET: MaintenanceReminders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceReminders = await _context.MaintenanceReminders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenanceReminders == null)
            {
                return NotFound();
            }

            return View(maintenanceReminders);
        }

        // POST: MaintenanceReminders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var maintenanceReminders = await _context.MaintenanceReminders.FindAsync(id);
            var userReminderList = await _context.UserReminders.Where(m => m.ReminderId == id).ToListAsync();

            foreach (UserReminders userReminder in userReminderList)
            {
                _context.UserReminders.Remove(userReminder);
            }

            _context.MaintenanceReminders.Remove(maintenanceReminders);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaintenanceRemindersExists(int id)
        {
            return _context.MaintenanceReminders.Any(e => e.Id == id);
        }
    }
}
