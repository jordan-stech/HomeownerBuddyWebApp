using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HOB_WebApp.Data;
using HOB_WebApp.Models;
using System.Diagnostics;

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

                String newTags = "";
                //if there are no tags don't try to get them from the modelstate
                if(contentModel.Tags!=null)
                //Get tags from view
                newTags = ModelState.Root.Children[1].AttemptedValue;
                //Split tags into array
                String[] beforeTitle = newTags.Split(",");
                //Puncuation list, used to remove non alphanumeric characters from title
                char[] puncuation = { ' ', '.', ',', '\"', '{', '}', '[', ']', '(', ')', '<', '>' };
                //Grab title from view and split it into an array, removing characters from the puncuation list
                String[] titleTags = contentModel.Title.Split(puncuation, StringSplitOptions.RemoveEmptyEntries);
                //set all tags to lowercase
                titleTags = titleTags.Select(s => s.ToLowerInvariant()).ToArray();
                beforeTitle = beforeTitle.Select(s => s.ToLowerInvariant()).ToArray();
                //Put the tags and title together
                String[] afterTitle = beforeTitle.Union(titleTags).ToArray();
                //if there are no tags inputed and title was noly one word
                if (afterTitle.Length <= 1)
                    //set tags to the only word
                    contentModel.Tags = afterTitle[0];
                else
                    //Changes tags to the inputted tags + title
                    contentModel.Tags = String.Join(", ", afterTitle.Where(s => !string.IsNullOrEmpty(s)));

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
                if (!contentModel.Link.Contains("embed")) {
                    // Change YouTube link to embedded link
                    String url = contentModel.Link;
                    url = url.Split("v=")[1];
                    url = "https://www.youtube.com/embed/" + url;
                    contentModel.Link = url;
                }

                String newTags = "";
                //check to see if the tag child exists (if it doesn't then "Steps" will be [2])
                if (ModelState.Root.Children[2].AttemptedValue!=contentModel.Steps) {
                    //Get tags from view
                    newTags = ModelState.Root.Children[2].AttemptedValue;
                }
                //Split tags into array
                String[] beforeTitle = newTags.Split(",");
                //Puncuation list, used to remove non alphanumeric characters from title
                char[] puncuation = { ' ', '.', ',', '\"', '{', '}', '[', ']', '(', ')', '<', '>' };
                //Grab title from view and split it into an array, removing characters from the puncuation list
                String[] titleTags = contentModel.Title.Split(puncuation, StringSplitOptions.RemoveEmptyEntries);
                //set all tags to lowercase
                titleTags = titleTags.Select(s => s.ToLowerInvariant()).ToArray();
                beforeTitle = beforeTitle.Select(s => s.ToLowerInvariant()).ToArray();
                //Put the tags and title together
                String[] afterTitle = beforeTitle.Union(titleTags).ToArray();
                //if there are no tags inputed and title was noly one word
                if (afterTitle.Length <= 1) 
                //set tags to the only word
                    contentModel.Tags = afterTitle[0];
                else
                //Changes tags to the inputted tags + title
                    contentModel.Tags = String.Join(", ", afterTitle.Where(s => !string.IsNullOrEmpty(s)));

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
