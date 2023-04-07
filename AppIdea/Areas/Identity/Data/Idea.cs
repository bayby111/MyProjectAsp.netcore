using Fingers10.ExcelExport.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppIdea.Areas.Identity.Data
{
    public class Idea
    {

        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [IncludeInReport(Order = 1)]
        public int Id { get; set; }

        [Required]
        [IncludeInReport(Order = 2)]
        public string? Content { get; set; }

        [IncludeInReport(Order = 3)]
        public string? FilePath { get; set; }

        public DateTime Datetime = DateTime.Now;

        [ForeignKey("UsersId")]
        [IncludeInReport(Order = 4)]
        public string? UsersId { get; set; }
        public virtual AppIdeaUser? Users { get; set; }

        [ForeignKey("CategoryId")]
        [IncludeInReport(Order = 5)]
        public int CategoryId { get; set; }
        public virtual Category? Categories { get; set; }

        //Not Mapped prevents EntityFramework from mapping the list to the model.
        //Since it's not part of the model database, it doesn't require a primary key.
        [NotMapped]
        public SelectList? CateNameSL { get; set; }

        [ForeignKey("TopicId")]
        [IncludeInReport(Order = 6)]
        public int TopicId { get; set; }
        public virtual Topic? Topics { get; set; }

        [NotMapped]
        public SelectList? TopicNameSL { get; set; }

        public List<View>? Views { get; set; }
        public List<Comment>? Comments { get; set; }
        public List<React>? Reacts { get; set; }


    }
}
