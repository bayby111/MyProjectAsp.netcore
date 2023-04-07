using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace AppIdea.Areas.Identity.Data;

// Add profile data for application users by adding properties to the AppIdeaUser class
public class AppIdeaUser : IdentityUser
{
    
    public string? Firstname { get; set; }


    [ForeignKey("DepartmentsId")]
    public string? DepartmentsId { get; set; }
    
    public virtual Department? Departments { get; set; }
    








}
public class  AppIdeaRole : IdentityRole
{
    public string? Id { get; set; }

    [Required]
    public string? Name { get; set; }

}





