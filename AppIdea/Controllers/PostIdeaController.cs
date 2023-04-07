using AppIdea.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace AppIdea.Controllers
{
    public class PostIdeaController : Controller
    {
        private readonly AppIdeaContext _context;
        private readonly UserManager<AppIdeaUser> _userManager;


        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;




        [Obsolete]
        public PostIdeaController(AppIdeaContext context, IHostingEnvironment hostingEnvironment,
            UserManager<AppIdeaUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

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
        [Obsolete]
        public async Task<IActionResult> Create([Bind("Id,Content,FilePath,Datetime,UsersId,CategoryId,TopicId")] Idea idea, IFormFile colectfile)
        {
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                //get name file
                string filename = colectfile.FileName;
                filename = Path.GetFileName(filename);
                string uploadfile = Path.Combine(_hostingEnvironment.WebRootPath, "Uploads");
                string fullpath = Path.Combine(uploadfile, filename);
                var stream = new FileStream(fullpath, FileMode.Create);
                await colectfile.CopyToAsync(stream);
                idea.FilePath = filename;
                idea.UsersId = user.Id;
                _context.Add(idea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", idea.CategoryId);
            ViewData["TopicId"] = new SelectList(_context.Topics, "Id", "Id", idea.TopicId);
            return View(idea);
        }

      


    }
}
