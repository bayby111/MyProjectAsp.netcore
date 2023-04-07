using AppIdea.Areas.Identity.Data;
using AppIdea.Core.ViewModel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Fingers10.ExcelExport.ActionResults;
using Fingers10.ExcelExport.Attributes;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using System.Threading;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using View = AppIdea.Areas.Identity.Data.View;

namespace AppIdea.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly AppIdeaContext _context;
        private readonly UserManager<AppIdeaUser> _userManager;
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;

        [Obsolete]
        public SubmissionController(AppIdeaContext context, UserManager<AppIdeaUser> userManager,
            IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string name, int page)
        {
            page = 1;
            page = page < 1 ? 1 : page;
            int pagesize = 6;

            var searchTopic = from t in _context.Topics select t;

            if (!string.IsNullOrEmpty(name))
            {
                searchTopic = searchTopic.Where(t => t.Name.Contains(name));

                 
            }

         
            return View(await searchTopic.ToPagedListAsync(page, pagesize));
        }

        
        [HttpGet]
        [Obsolete]
        public IActionResult DownloadZip(int? IdTopic)
        {
            if(IdTopic == null)
            {
                return NotFound();
            }

            
            var idea = _context.Ideas.FirstOrDefault(i => i.TopicId == IdTopic);

            if(idea == null)
            {
                return RedirectToAction("Error", "Submission");
            }

             string? filepath = idea.FilePath;
           

            var webroot = _hostingEnvironment.WebRootPath;
            var filename = "Myzipp.zip";

            var tempOutput = webroot + "/Uploads" + filename;

            using (ZipOutputStream zipOutputStream = new ZipOutputStream(System.IO.File.Create(tempOutput)))
            {
                zipOutputStream.SetLevel(9);
                byte[] buffer = new byte[4096];
                var file = new List<string>
                {
                    webroot + "/Uploads/"+ filepath
                };
                
                for(int i = 0; i < file.Count; i++)
                {
                    ZipEntry etry = new ZipEntry(Path.GetFileName(file[i]));
                    etry.DateTime = DateTime.Now;
                    etry.IsUnicodeText = true;

                    zipOutputStream.PutNextEntry(etry);

                    using (FileStream fileStream = System.IO.File.OpenRead(file[i]))
                    {
                        int sourceByte;

                        do
                        {
                            sourceByte = fileStream.Read(buffer, 0, buffer.Length);
                            zipOutputStream.Write(buffer, 0, sourceByte);

                        } while (sourceByte > 0);

                    }
                }

                zipOutputStream.Finish();
                zipOutputStream.Flush();
                zipOutputStream.Close();
            }

            byte[] finalResult = System.IO.File.ReadAllBytes(tempOutput);
            if (System.IO.File.Exists(tempOutput))
            {
                System.IO.File.Delete(tempOutput);
            }

            if(tempOutput == null || !tempOutput.Any())
            {
                throw new Exception(string.Format("Nothing Found"));
            }
            return File(finalResult,"application/zip", filename);
        }

        [HttpGet]
        public IActionResult DownloadExcel(int? IdTopic)
        {
            var idea = _context.Ideas.FirstOrDefault(i => i.TopicId == IdTopic);
            if (idea == null)
            {
                return RedirectToAction("Error", "Submission");
            }

            List<Idea> result = new List<Idea>
            {
                new Idea
                {
                    Id = idea.Id,
                    Content = idea.Content,
                    FilePath = idea.FilePath,
                    UsersId = idea.UsersId,
                    CategoryId = idea.CategoryId,
                    TopicId = idea.TopicId

                }
            };
            return new ExcelResult<Idea>(result, "Sheet1", "IdeaReport");
        }

        [HttpGet]
        public IActionResult ViewIdea(int? IdTopic, int page =1)
        {
           
            page = page < 1 ? 1 : page;
            int pagesize = 6;
            
            var topic = _context.Topics.FirstOrDefault(t => t.Id == IdTopic);
          
            var idea = _context.Ideas.Where(i => i.TopicId == IdTopic).ToPagedList(page, pagesize);


            ViewIdeaSubmit model = new ViewIdeaSubmit
            {
                Ideas = idea,
                Topic =topic
                        
            };
         
            return View(model);
        }

       

        [HttpGet]
        public async Task<IActionResult> detailsIdea(int? IdIdea, ViewIdeaSubmit model)
        {
            var idea = _context.Ideas.Include(i=> i.Categories).Include(i=>i.Topics).FirstOrDefault(i => i.Id == IdIdea);
            var comment = await _context.Comments.Where(c => c.IdeaId == IdIdea).Include(c=>c.Users).ToListAsync();
            model.Idea = idea;
            model.Comments = comment;
            ViewBag.Like = (from r in _context.Reacts where r.IdeaId == IdIdea select r.Like).FirstOrDefault();
            ViewBag.Dislike = (from r in _context.Reacts where r.IdeaId == IdIdea select r.Dislike).FirstOrDefault();

            var view = await _context.Views.Where(v => v.IdeaId == IdIdea).ToListAsync();
            model.Views = view;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddView(int? IdIdea, View model)
        {
            /// get user
            var view = await _context.Views.FindAsync(IdIdea);

            var user = await _userManager.GetUserAsync(User);

            if (IdIdea == null && user == null)
            {
                return NotFound();
            }
          
            if (user != null)
            {
                model.IdeaId = (int)IdIdea;
                model.UsersId = user.Id;
                _context.Add(model);



                model.Viewcount++;
                _context.SaveChanges();
                return RedirectToAction("detailsIdea", new { IdIdea = model.IdeaId });
            }





            return View(model);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> React(int? IdIdea, bool Islike, ViewIdeaSubmit model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (IdIdea == null)
            {
                return NotFound();
            }
            
            model.React = _context.Reacts.FirstOrDefault(r => r.IdeaId == IdIdea);

            // here we are checking whether user have done like or dislike    
            if ( model.React == null)
            {
                model.React = new React();
                model.React.UsersId = user.Id;
                model.React.IdeaId = (int)IdIdea;

                if (Islike)
                {
                    if (model.React.Like == null) // if no one has done like or dislike and first time any one doing like and dislike then assigning 1 and                                                                                0    
                    {
                        model.React.Like = model.React.Like + 1;
                        model.React.Dislike =  0;
                    }
                    else
                    {
                        model.React.Like = model.React.Like + 1;
                    }
                }
                else
                {
                    if (model.React.Dislike == null)
                    {
                        model.React.Dislike = model.React.Dislike + 1;
                        model.React.Like = 0;
                    }
                    else
                    {
                        model.React.Dislike = model.React.Dislike + 1;
                    }
                }
                _context.Add(model.React);
            }
            else
            {
                model.React.UsersId = user.Id;
                model.React.IdeaId = (int)IdIdea;

                if (Islike)
                {
                    // if user has click like button then need to increase +1 in like and -1 in Dislike    
                    model.React.Like = model.React.Like + 1;
                    if (model.React.Dislike == 0 || model.React.Dislike < 0)
                    {
                        model.React.Dislike = 0;
                    }
                    else
                    {
                        model.React.Dislike = model.React.Dislike - 1;
                    }
                }
                else
                {
                    // if user has click dislike then need to increase +1 in dislike and -1 in like    
                    model.React.Dislike = model.React.Dislike + 1;
                    if (model.React.Like == 0 || model.React.Like < 0)
                    {
                        model.React.Like = 0;
                    }
                    else
                    {
                        model.React.Like = model.React.Like - 1;
                    }
                }
            }
            
            

            await _context.SaveChangesAsync();

            return RedirectToAction("detailsIdea", new { IdIdea = model.React.IdeaId});


        }

        [HttpPost]
        public async Task<IActionResult> Comment(int? Id_Idea, ViewIdeaSubmit model)
        {
            if(Id_Idea == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);

            if(Id_Idea !=null && user != null)
            {
                model.Comment.UsersId = user.Id;
                model.Comment.IdeaId = (int)Id_Idea;

                _context.Add(model.Comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("detailsIdea", new { IdIdea = model.Comment.IdeaId });
            }

            return View(model);
        }

        public IActionResult Error()
        {
            return View();
        }




    }
}