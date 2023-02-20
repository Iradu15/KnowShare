using PROIECT.Models;
using System.ComponentModel.DataAnnotations;


namespace PROIECT.Models
{
    public class Domain
    {
        [Key]
        public int Id { get; set; } // PK

        [Required(ErrorMessage = "Numele domeniului este obligatoriu")]
        public string DomainName { get; set; }
        public virtual ICollection<Article>? Articles { get; set; } // Obiectele articole asociate domeniului
    }

}
