using AppIdea.Areas.Identity.Data;
using AppIdea.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace AppIdea.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppIdeaContext _context;

        public HomeController(ILogger<HomeController> logger,
            AppIdeaContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index( int page, string nameIdea)
        {
            page = page < 1 ? 1 : page;
            int pagesize = 6;

            var searchidea = from i in _context.Ideas.Include(i => i.Topics).Include(i => i.Categories).Include(i => i.Users) select i;

            if (!string.IsNullOrEmpty(nameIdea))
            {
                searchidea = searchidea.Where(i => i.Content.Contains(nameIdea));
            }


            return View(await searchidea.ToPagedListAsync(page, pagesize));
        }

     

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}