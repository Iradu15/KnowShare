using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROIECT.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; } // PK

        //. Dupa adaugarea validarilor in Model, urmeaza preluarea mesajelor de validare in View.

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractere")]
        [MinLength(5, ErrorMessage = "Titlul trebuie sa aiba mai mult de 5 caractere")]
        public string Title { get; set; }


        [Required(ErrorMessage = "Continutul articolului este obligatoriu")]
        public string Content { get; set; }


        //Chiar daca acest atribut este obligatoriu, in
        //implementarea lui interna se considera ca poate fi si null. In momentul in care
        //este null, se afiseaza automat mesajul de validare intern “The value is invalid”
        public DateTime Date { get; set; }


        [Required(ErrorMessage = "Domeniul este obligatoriu")]
        //optional la nivel de Model, astfel
        //incat in momentul in care inputul nu are o valoare selectata sa preia corect
        //mesajul configurat la nivel de Model si sa paseze acel mesaj pentru afisare in
        //View.

        public int? DomainId { get; set; } // FK Domain

        public string? UserID { get; set; } // FK User
        public virtual ApplicationUser? User { get; set; } //un articol apartine unui singur utilizator
                                                           // o sa fie un moment cand User o sa fie null deoarece mai intai se pune FK ul si de abia dupa
                                                           // se asociaza cu user ul propriu zis, timp in care o sa fie null si de aceea punem ?
        public virtual Domain? Domain { get; set; }

        public bool? ShowInDatabase { get; set; }    //daca e false nu il afisez in baza de date, insemanand ca e una din veriunile altui articol, nu unul de sine statator

        public bool? AllowCreatorEdit{ get; set; }    //daca e true atunci creatorul nu l mai poate edita 


        public virtual ICollection<Article>? PreviousArticles { get; set; } // pentru versiunea anterioara 

        [NotMapped] // nu apare in baza de date
        public IEnumerable<SelectListItem>? Dom { get; set; } // Lista de Domenii, folosita pentru dropDown
    }


}
