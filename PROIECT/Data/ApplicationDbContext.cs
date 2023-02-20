using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PROIECT.Models;

namespace PROIECT.Data
{

    //PASUL 3
    //Conexiunea cu baza de date se va realiza in acest caz exclusiv in Program.cs.
    //ne asiguram ca foloseste clasa noastra(ApplicaionUser>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //vom avea acces la intrarile din baza de date; se pot interoga si
        //stoca instante de tip Article/Domain,etc.

        public DbSet<Article> Articles { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

}