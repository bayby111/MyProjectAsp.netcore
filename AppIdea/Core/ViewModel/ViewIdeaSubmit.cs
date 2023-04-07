using AppIdea.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace AppIdea.Core.ViewModel
{
    public class ViewIdeaSubmit
    {
       


        public IPagedList<Idea>? Ideas { get; set; }
        public Idea? Idea { get; set; }
        public ICollection<Idea>? ListIdea { get; set; }


        public IPagedList<Topic>? Topics { get; set; }
        public Topic? Topic { get; set; }


        public React? React { get; set; }
        public List<React>? Reacts { get; set; }

        
        public AppIdeaUser? User { get; set; }
        public List<AppIdeaUser> Users { get; set; }


        public Comment? Comment { get; set; }
        public ICollection<Comment>? Comments { get; set; }


       
        public IEnumerable<View>? Views { get; set; }

        public View? View { get; set; }

    }
}
