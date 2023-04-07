using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace AppIdea.Areas.Identity.Data
{
    public class Topic
    {
        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public DateTime Deadline_1 { get; set; }

        public DateTime Dealine_2 { get; set; }

        public virtual ICollection<Idea>? Ideas { get; set; }
    }
}
