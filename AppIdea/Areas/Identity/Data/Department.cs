using MessagePack;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace AppIdea.Areas.Identity.Data
{
    public class Department
    {
        public Department()
        {
            Users = new HashSet<AppIdeaUser>();
        }


        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }

        [Required]
        public string? Name {get; set; }

        public virtual ICollection<AppIdeaUser> Users { get; set; }

    }
}
