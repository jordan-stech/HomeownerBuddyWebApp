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
    public class ActionPlanController : Controller
    {
        private readonly HOB_WebAppContext _context;

        public ActionPlanController(HOB_WebAppContext context)
        {
            _context = context;
        }

        // GET: ActionPlan
        public async Task<IActionResult> Index()
        {
            return View(await _context.ContentModel.ToListAsync());
        }

        // GET: ActionPlan/CreateActionPlan
        public IActionResult CreateActionPlan()
        {
            return View();
        }

        // GET: ActionPlan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contentModel = await _context.ContentModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contentModel == null)
            {
                return NotFound();
            }

            return View(contentModel);
        }

        // GET: ActionPlan/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ActionPlan/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Link,Steps,Category,Tags")] ContentModel contentModel)
        {
            if (ModelState.IsValid)
            {
                // Change YouTube link to embedded link
                String url = contentModel.Link;
                url = url.Split("v=")[1];
                url = "https://www.youtube.com/embed/" + url;
                contentModel.Link = url;

                //replace the currect tag (only the first Tag) with the String of every tag retreived from the model state
                contentModel.Tags = ModelState.Root.Children[1].AttemptedValue;
                //Edit string to add space between tags
                contentModel.Tags = contentModel.Tags.Replace(",", ", ");

                _context.Add(contentModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contentModel);
        }

        // GET: ActionPlan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contentModel = await _context.ContentModel.FindAsync(id);
            if (contentModel == null)
            {
                return NotFound();
            }

            //Change tags into a form easier to parse in edit
            string Tags = contentModel.Tags;
            //Put the tags into ViewData, which is visable in the view
            ViewBag.Tags = Tags;

            return View(contentModel);
        }

        // POST: ActionPlan/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Link,Steps,Category,Tags")] ContentModel contentModel)
        {
            if (id != contentModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                //replace the currect tags with the String of every tag retreived from the model state
                contentModel.Tags = ModelState.Root.Children[2].AttemptedValue;
                //Edit string to add space between tags
                contentModel.Tags = contentModel.Tags.Replace(",", ", ");
                try
                {
                    _context.Update(contentModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContentModelExists(contentModel.Id))
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
            return View(contentModel);
        }

        // GET: ActionPlan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contentModel = await _context.ContentModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contentModel == null)
            {
                return NotFound();
            }

            return View(contentModel);
        }

        // POST: ActionPlan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contentModel = await _context.ContentModel.FindAsync(id);
            _context.ContentModel.Remove(contentModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContentModelExists(int id)
        {
            return _context.ContentModel.Any(e => e.Id == id);
        }
    }
}
