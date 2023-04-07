using AppIdea.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppIdea.Core.ViewModel
{
    public class EditViewModel
    {
        public AppIdeaUser? User { get; set; }
        public IList<SelectListItem>? Role { get; set; }
        
    }
}
