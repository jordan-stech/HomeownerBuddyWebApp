using System;
using System.Collections.Generic;
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

        // GET: MaintenanceReminders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MaintenanceReminders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Reminder,Sent,Completed")] MaintenanceReminders maintenanceReminders)
        {
            if (ModelState.IsValid)
            {
                _context.Add(maintenanceReminders);
                await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Reminder,Sent,Completed")] MaintenanceReminders maintenanceReminders)
        {
            if (id != maintenanceReminders.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(maintenanceReminders);
                    await _context.SaveChangesAsync();
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
