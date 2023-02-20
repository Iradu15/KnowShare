using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PROIECT.Data;
using PROIECT.Models;
using System.Data;

namespace PROIECT.Controllers
{
    //doar adminul poate modifica rolurile utilizatorilor 
    [Authorize(Roles = "Admin")]

    public class AdminsController : Controller
    {
        //configurarea managerului de useri si roluri
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminsController(ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public ActionResult Index()
        {
            //domain poate avea orice nume 
            ViewBag.AllUsers = _userManager.Users;

            return View();
        }

        // task pentru ca vreau sa fie asyncron, adica un thread preia sarcini in timp ce asteapta 
        public async Task<ActionResult> Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.Roles = roles;

            return View(user);
        }

        public IActionResult New()
        {
            ApplicationUser user = new ApplicationUser();

            user.EmailConfirmed = true;

            user.AllRoles = GetAllRoles();

            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> New(ApplicationUser user, [FromForm] string newRole)
        {

            if (ModelState.IsValid)
            {
                user.EmailConfirmed = true;
                var result = await _userManager.CreateAsync(user, "password123!");
                var roleName = await _roleManager.FindByIdAsync(newRole);
                db.ApplicationUsers.Add(user);
                db.SaveChanges();
                await _userManager.AddToRoleAsync(user, roleName.ToString());
                TempData["message"] = "User-ul a fost adaugat";
                db.SaveChanges();
            }
            else
            {
                TempData["message"] = "Eroare la adaugarea user-ului";
            }

            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);

            user.AllRoles = GetAllRoles();

            var roleNames = await _userManager.GetRolesAsync(user); // Lista de nume de roluri

            // Cautam ID-ul rolului in baza de date
            var currentUserRole = _roleManager.Roles
                                              .Where(r => roleNames.Contains(r.Name))
                                              .Select(r => r.Id)
                                              .First(); // Selectam 1 singur rol
            ViewBag.UserRole = currentUserRole;

            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string id, ApplicationUser newData, [FromForm] string newRole)
        {
            ApplicationUser user = db.Users.Find(id);

            user.AllRoles = GetAllRoles();

            if (ModelState.IsValid)
            {
                user.UserName = newData.UserName;
                user.Email = newData.Email;

                // Cautam toate rolurile din baza de date
                var roles = db.Roles.ToList();

                foreach (var role in roles)
                {
                    // Scoatem userul din rolurile anterioare
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                // Adaugam noul rol selectat
                var roleName = await _roleManager.FindByIdAsync(newRole);
                await _userManager.AddToRoleAsync(user, roleName.ToString());

                db.SaveChanges();

                TempData["message"] = "User-ul a fost modificat";

            }
            else
            {
                TempData["message"] = "Eroare la modificarea user-ului";
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = db.Users
                         .Include("Articles")
                         .Where(u => u.Id == id)
                         .First();

            // Delete user articles
            if (user.Articles.Count > 0)
            {
                foreach (var article in user.Articles)
                {
                    db.Articles.Remove(article);
                }
            }

            db.ApplicationUsers.Remove(user);
            db.SaveChanges();

            TempData["message"] = "User-ul a fost sters";

            return RedirectToAction("Index");
        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles
                        select role;

            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }
    }
}