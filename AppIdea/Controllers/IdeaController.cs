using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppIdea.Areas.Identity.Data;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;
using System.IO;
using X.PagedList;
using Microsoft.AspNetCore.Identity;

namespace AppIdea.Controllers
{
    public class IdeaController : Controller
    {
        private readonly AppIdeaContext _context;
        private readonly UserManager<AppIdeaUser> _userManager;
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;

        

        [Obsolete]
        public IdeaController(AppIdeaContext context, IHostingEnvironment hostingEnvironment,
            UserManager<AppIdeaUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        // GET: Idea
        public async Task<IActionResult> Index( int page = 1)
        {
            page = page < 1 ? 1 : page;

            int pagesize = 3;
            var appIdeaContext = _context.Ideas.Include(i => i.Categories).Include(i => i.Topics);
           
            return View( await appIdeaContext.ToPagedListAsync(page, pagesize));
        }



        // GET: Idea/Create
        public IActionResult Create(object? selectedcate = null, object? selectedTopic = null)
        {
            ///category
            var categoriesQuery = from d in _context.Categories
                                   orderby d.Name // Sort by name.
                                   select d;

            var CateSL = new SelectList(categoriesQuery.AsNoTracking(),
                nameof(Category.Id),// key
                nameof(Category.Name),//value
                selectedcate);// object selected

            ///////TOPIC
            var TopicsQuery = from d in _context.Topics
                                  orderby d.Name // Sort by name.
                                  select d;

            var TopicSL = new SelectList(TopicsQuery.AsNoTracking(),
                nameof(Topic.Id),// key
                nameof(Topic.Name),//value
                selectedTopic);// object selected


            Idea model = new Idea
            {
                CateNameSL = CateSL,
                TopicNameSL = TopicSL
                
            };

            return View(model);
        }

        // POST: Idea/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content,FilePath,Datetime,CategoryId,TopicId")] Idea idea, IFormFile colectfile)
        {
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                
                //get name file
                string filename = colectfile.FileName;
                filename = Path.GetFileName(filename);
                string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads", filename);
                var stream = new FileStream(uploadfile, FileMode.Create);
                await colectfile.CopyToAsync(stream);

                idea.FilePath = filename;
                idea.UsersId = user.Id;
                _context.Add(idea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name), idea.CategoryId);
            ViewData["TopicId"] = new SelectList(_context.Topics, nameof(Topic.Id), nameof(Topic.Name), idea.TopicId);
            return View(idea);
        }

        // GET: Idea/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Ideas == null)
            {
                return NotFound();
            }

            var idea = await _context.Ideas.FindAsync(id);
            if (idea == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", idea.CategoryId);
            ViewData["TopicId"] = new SelectList(_context.Topics, "Id", "Name", idea.TopicId);
            return View(idea);
        }

        // POST: Idea/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Content,FilePath,Datetime,CategoryId,TopicId")] Idea idea)
        {
            if (id != idea.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(idea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IdeaExists(idea.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", idea.CategoryId);
            ViewData["TopicId"] = new SelectList(_context.Topics, "Id", "Name", idea.TopicId);
            return View(idea);
        }

        // GET: Idea/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Ideas == null)
            {
                return NotFound();
            }

            var idea = await _context.Ideas
                .Include(i => i.Categories)
                .Include(i => i.Topics)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (idea == null)
            {
                return NotFound();
            }

            return View(idea);
        }

        // POST: Idea/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_context.Ideas == null)
            {
                return Problem("Entity set 'AppIdeaContext.Ideas'  is null.");
            }
            var idea = await _context.Ideas.FindAsync(id);
            if (idea != null)
            {
                string uploadfile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads", idea.FilePath);
                System.IO.File.Delete(uploadfile);
                _context.Ideas.Remove(idea);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IdeaExists(int? id)
        {
            return _context.Ideas.Any(e => e.Id == id);
        }
    }
}
