using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppIdea.Areas.Identity.Data
{
    public class Comment
    {
        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       
        public int Id { get; set; }
        public string? Text { get; set; }
        public DateTime Datetime = DateTime.Now;

        [ForeignKey("UsersId")]
        public string? UsersId { get; set; }
        public virtual AppIdeaUser? Users { get; set; }

        [ForeignKey("IdeaId")]
        public int IdeaId { get; set; }
        public virtual Idea? Ideas { get; set; }
    }
}
