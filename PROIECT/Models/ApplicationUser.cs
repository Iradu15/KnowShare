using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PROIECT.Models
{
    //PASUL 1

    // mosteneste IdentityUser care are meotdele ce ne intereseaza(numar telefon, sa ne logam cu fb,google) 
    //Clasa pe care o mosteneste, IdentityUser, descrie Userul
    //in baza de date(cea care contine toate atributele unui utilizator).
    //ne ajuta sa extindem user ul daca vrem sa adaugam campuri noi
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Article>? Articles { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

    }
}
