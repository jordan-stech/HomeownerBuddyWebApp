using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HOB_WebApp.Data;
using HOB_WebApp.Models;

namespace HOB_WebApp.Controllers
{
    public class MobileUsersController : Controller
    {
        private readonly HOB_WebAppContext _context;

        public MobileUsersController(HOB_WebAppContext context)
        {
            _context = context;
        }

        // GET: MobileUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.MobileUsers.ToListAsync());
        }

        // GET: MobileUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mobileUsers = await _context.MobileUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mobileUsers == null)
            {
                return NotFound();
            }

            return View(mobileUsers);
        }

        // GET: MobileUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MobileUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FName,Lname,HomeCode")] MobileUsers mobileUsers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mobileUsers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mobileUsers);
        }

        // GET: MobileUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mobileUsers = await _context.MobileUsers.FindAsync(id);
            if (mobileUsers == null)
            {
                return NotFound();
            }
            return View(mobileUsers);
        }

        // POST: MobileUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FName,Lname,HomeCode")] MobileUsers mobileUsers)
        {
            if (id != mobileUsers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mobileUsers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MobileUsersExists(mobileUsers.Id))
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
            return View(mobileUsers);
        }

        // GET: MobileUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mobileUsers = await _context.MobileUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mobileUsers == null)
            {
                return NotFound();
            }

            return View(mobileUsers);
        }

        // POST: MobileUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mobileUsers = await _context.MobileUsers.FindAsync(id);
            _context.MobileUsers.Remove(mobileUsers);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MobileUsersExists(int id)
        {
            return _context.MobileUsers.Any(e => e.Id == id);
        }
    }
}
