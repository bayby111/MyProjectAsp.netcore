using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;
namespace AppIdea.Areas.Identity.Data
{
    public class Category
    {
        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public virtual ICollection<Idea>? Ideas { get; set; }
    }
}
