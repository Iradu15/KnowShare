using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;
using PROIECT.Data;
using PROIECT.Models;
using System.Transactions;
using System.Text.RegularExpressions;
using NuGet.Packaging.Signing;

namespace PROIECT.Controllers
{
    [Authorize] // doar cei inregistrati sa vada articolele 
    public class ArticlesController : Controller
    {
        //In final, in cadrul fiecarui Controller se realizeaza conexiunea cu baza de date astfel:

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ArticlesController(ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [Authorize(Roles = "User,Editor,Admin")] // doar ei pot sa vada 
        //Pentru fiecare utilizator se afiseaza si utilizatorul care a postat articolul
        public IActionResult Index()
        {

            //!!!PAGINATIA TREBUIE PUSA ULTIMA PENTRU CA ATUNCI SE ALEG ARTICOLELE DIN PAGINA DORITA

            int _perPage = 5; // 5 pe pagina 

            var articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                        .Where(art=>art.ShowInDatabase == true)                                            
                                                        .OrderBy(a => a.Date);

            var search = "";
            ViewBag.SearchString = search;

            int sortBy = 0; // DEFAULT E DUPA ORA
            ViewBag.sortBy = sortBy;

            int sortWay = 0; // DEFAULT DESCRESCATOR 
            ViewBag.sortWay = sortWay;

            ViewBag.Domains = GetAllDomains();
            int chosenDomain = -1;
            ViewBag.ChoseDomain = -1;

            //SORTEAZA DUPA CRITERIU SI/SAU DOMENIU
            if (Convert.ToString(HttpContext.Request.Query["sort"]) != null ||
                Convert.ToString(HttpContext.Request.Query["sortWay"]) != null ||
                Convert.ToString(HttpContext.Request.Query["domain"]) != null ||
                Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {

                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
                ViewBag.SearchString = search;

                sortBy = Convert.ToInt16(HttpContext.Request.Query["sort"]);
                ViewBag.sortBy = sortBy;

                sortWay = Convert.ToInt16(HttpContext.Request.Query["sortWay"]);
                ViewBag.sortWay = sortWay;

                chosenDomain = Convert.ToInt16(HttpContext.Request.Query["domain"]);
                ViewBag.ChoseDomain = chosenDomain;

                if(search != null) // SE TINE CONT DE CAUTARE
                {

                    List<int> articleIds = db.Articles.Where(
                                        (at => at.Title.Contains(search)
                                        || at.Content.Contains(search) && at.ShowInDatabase == true)
                                        ).Select(a => a.Id).ToList();

                    // Lista articolelor care contin cuvantul cautat
                    // in articol -> Title si Content

                    if (chosenDomain == -1) // NU TIN CONT DE DOMENIU
                    {
                        if (sortBy == 0 && sortWay == 0) //DATA DESCRESCATOR 
                            articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                                .Where(article => articleIds.Contains(article.Id) && article.ShowInDatabase == true)
                                                                .OrderByDescending(a => a.Date);

                        else if (sortBy == 0 && sortWay == 1) //DATA CRESCATOR
                            articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                                .Where(article => articleIds.Contains(article.Id) && article.ShowInDatabase == true)
                                                                .OrderBy(a => a.Date);


                        else if (sortBy == 1 && sortWay == 0) //ALFABETIC DESCRESCATOR 
                            articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                                .Where(article => articleIds.Contains(article.Id) && article.ShowInDatabase == true)
                                                                .OrderByDescending(a => a.Title);

                        else if (sortBy == 1 && sortWay == 1) //ALFABETIC CRESCATOR
                            articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                                .Where(article => articleIds.Contains(article.Id) && article.ShowInDatabase == true)
                                                                .OrderBy(a => a.Title);

                    }
                    else // TIN CONT DE DOMENIU
                    {
                        if (sortBy == 0 && sortWay == 0) //DATA DESCRESCATOR 
                            articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                                .Where(article => articleIds.Contains(article.Id) && article.ShowInDatabase == true)
                                                                .Where(art => art.DomainId == chosenDomain)
                                                                .OrderByDescending(a => a.Date);

                        else if (sortBy == 0 && sortWay == 1) //DATA CRESCATOR
                            articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                                .Where(article => articleIds.Contains(article.Id) && article.ShowInDatabase == true)
                                                                .Where(art => art.DomainId == chosenDomain)
                                                                .OrderBy(a => a.Date);


                        else if (sortBy == 1 && sortWay == 0) //ALFABETIC DESCRESCATOR 
                            articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                                .Where(article => articleIds.Contains(article.Id) && article.ShowInDatabase == true)
                                                                .Where(art => art.DomainId == chosenDomain)
                                                                .OrderByDescending(a => a.Title);

                        else if (sortBy == 1 && sortWay == 1) //ALFABETIC CRESCATOR
                            articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                                .Where(article => articleIds.Contains(article.Id) && article.ShowInDatabase == true)
                                                                .Where(art => art.DomainId == chosenDomain)
                                                                .OrderBy(a => a.Title);
                    }
                }
                else // NU SE TINE CONT DE CAUTARE
                {
                    if (chosenDomain == -1) // NU TIN CONT DE DOMENIU
                    {
                        if (sortBy == 0 && sortWay == 0) //DATA DESCRESCATOR 
                            articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                                .Where(art => art.ShowInDatabase == true)
                                                                .OrderByDescending(a => a.Date);

                        else if (sortBy == 0 && sortWay == 1) //DATA CRESCATOR
                            articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                                .Where(art => art.ShowInDatabase == true)
                                                                .OrderBy(a => a.Date);


                        else if (sortBy == 1 && sortWay == 0) //ALFABETIC DESCRESCATOR 
                            articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                                .Where(art => art.ShowInDatabase == true)
                                                                .OrderByDescending(a => a.Title);

                        else if (sortBy == 1 && sortWay == 1) //ALFABETIC CRESCATOR
                            articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                                .Where(art => art.ShowInDatabase == true)
                                                                .OrderBy(a => a.Title);

                    }
                    else // TIN CONT DE DOMENIU
                    {
                        if (sortBy == 0 && sortWay == 0) //DATA DESCRESCATOR 
                            articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                                .Where(art => art.ShowInDatabase == true)
                                                                .Where(art => art.DomainId == chosenDomain)
                                                                .OrderByDescending(a => a.Date);

                        else if (sortBy == 0 && sortWay == 1) //DATA CRESCATOR
                            articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                                .Where(art => art.ShowInDatabase == true)
                                                                .Where(art => art.DomainId == chosenDomain)
                                                                .OrderBy(a => a.Date);


                        else if (sortBy == 1 && sortWay == 0) //ALFABETIC DESCRESCATOR 
                            articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                                .Where(art => art.ShowInDatabase == true)
                                                                .Where(art => art.DomainId == chosenDomain)
                                                                .OrderByDescending(a => a.Title);

                        else if (sortBy == 1 && sortWay == 1) //ALFABETIC CRESCATOR
                            articles = db.Articles.Include("Domain").Include("User") // initial ne uitam la toate articolele
                                                                .Where(art => art.ShowInDatabase == true)
                                                                .Where(art => art.DomainId == chosenDomain)
                                                                .OrderBy(a => a.Title);
                    }
                }
            }

            int totalItems = articles.Count(); // Fiind un numar variabil de articole, verificam de fiecare data 
           
            // Se preia pagina curenta din View-ul asociat
            // Numarul paginii este valoarea parametrului page din ruta
            // /Articles/Index?page=valoare
            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            // Pentru prima pagina offsetul o sa fie zero
            // Pentru pagina 2 o sa fie 3
            // Asadar offsetul este egal cu numarul de articole
            //care au fost deja afisate pe paginile anterioare
            var offset = 0;
            // Se calculeaza offsetul in functie de numarul paginii la care suntem
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            // Se preiau articolele corespunzatoare pentru
            //fiecare pagina la care ne aflam
            // in functie de offset
            var paginatedArticles = articles.Skip(offset).Take(_perPage);

            // Preluam numarul ultimei pagini
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            // Trimitem articolele cu ajutorul unui ViewBag
            //catre View-ul corespunzator
            ViewBag.Articles = paginatedArticles;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Msg = TempData["message"];
            }


            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Articles/Index/?search="
               + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Articles/Index/?page";
            }

            return View();
        }


        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Show(int id)
        {
            Article article = db.Articles.Include("Domain").Include("User")
                               .Where(art => art.Id == id && art.ShowInDatabase == true) // posibil sa nu fie nevoie de showInDatabase
                               .First();

            setAccesRights();

            ViewBag.Restrictionare = article.AllowCreatorEdit;

            //Pentru a putea trimite Modelul catre View si pentru a putea fi folosit
            //este nevoie de article ca parametru pentru View

            return View(article);
        }

        private void setAccesRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Editor"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.UserCurent = _userManager.GetUserId(User);

            ViewBag.EsteAdmin = User.IsInRole("Admin");
        }

        [Authorize(Roles = "Editor,Admin")]
        public IActionResult New()
        {
            Article article = new Article();

            article.Dom = GetAllDomains();

            return View(article);
        }


        // Se adauga articolul in baza de date
        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public IActionResult New(Article article)
        {

            article.ShowInDatabase = true; // sa il afisam in baza de date
            article.Date = DateTime.Now;
            article.UserID = _userManager.GetUserId(User);
            article.AllowCreatorEdit = true; // initial creatorul poate modifica articolul 

            if (ModelState.IsValid)
            {
                article.PreviousArticles = new List<Article>(); // initializez lista de articole pentru versiunile anterioare. Momentan e goala, asta fiind prima VERSIUNE
                db.Articles.Add(article);
                
                ApplicationUser user = db.Users.Find(_userManager.GetUserId(User)); // user ul curent 
                
                if(user.Articles == null) // atribui articolul userului curent
                    user.Articles = new List<Article>();

                user.Articles.Add(article);

                db.SaveChanges();

                //Helperul TempData – poate seta o valoare care va fi disponibila intr-un request subsecvent
                TempData["message"] = "Articolul a fost adaugat";
                return RedirectToAction("Index");
            }
            else
            {
                article.Dom = GetAllDomains();
                return View(article);
            }
        }


        [Authorize(Roles = "Editor,Admin")]
        public IActionResult Edit(int id)
        {
            Article article = db.Articles.Include("Domain")
                                         .Where(art => art.Id == id)
                                         .First();

            ViewBag.R = article.AllowCreatorEdit;
            
            article.Dom = GetAllDomains();
            if (article.UserID == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                return View(article);

            else
            {
                TempData["message"] = "Nu aveti drepuri suficiente";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public IActionResult Edit(int id, Article requestArticle)
        {
            Article article = db.Articles.Include("PreviousArticles")
                                         .Where(art => art.Id == id)
                                         .First();

            if (ModelState.IsValid)
            {
                if (article.UserID == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {

                    // adaug articolul inainte sa fie modificat ca fiind variant ANTERIOARA 
                    //versiunea anterioara deoarece au acc id, solutia e sa creez Articol nou dar fara sa l bag in baza de date probabil
                    Article aux = new Article();
                    aux.Title = article.Title;
                    aux.Content = article.Content;
                    aux.DomainId = article.DomainId;
                    aux.Date = DateTime.Now;
                    aux.UserID = _userManager.GetUserId(User);
                    aux.ShowInDatabase = false;
                    article.PreviousArticles.Add(aux);

                    article.Title = requestArticle.Title;
                    article.Content = requestArticle.Content;
                    article.DomainId = requestArticle.DomainId;
                    TempData["message"] = "Articolul a fost modificat";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti drepuri suficiente";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                requestArticle.Dom = GetAllDomains();
                return View(requestArticle);
            }
        }


        // Se sterge un articol din baza de date 
        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Article article = db.Articles.Include("PreviousArticles")
                                         .Where(art => art.Id == id)
                                         .First();
            if (article.UserID == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Articles.Remove(article);
                TempData["message"] = "Articolul a fost sters";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti drepuri suficiente";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Undo(int id)
        {
            Article article = db.Articles.Include("PreviousArticles")
                                         .Where(art => art.Id == id)
                                         .First();


            if (article.PreviousArticles.Count > 0)
            {

                if (User.IsInRole("Admin")) // doar daca e admin poate reveni la forma initala 
                {
                    Article previous = db.Articles.Find(article.PreviousArticles.Last().Id);

                    article.Title = previous.Title;
                    article.Content = previous.Content;
                    article.DomainId = previous.DomainId;
                    article.PreviousArticles.Remove(article.PreviousArticles.Last());
                    TempData["message"] = "Articolul a fost adus la versiunea anterioara";
                    db.Articles.Remove(previous);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti drepuri suficiente";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                if (article.PreviousArticles.Count == 0)
                    TempData["message"] = "Articolul nu are o versiune anterioara";

                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Restrict(int id, Article requestArticle)
        {
            Article article = db.Articles.Include("Domain")
                                         .Where(art => art.Id == id)
                                         .First();
           
            article.AllowCreatorEdit ^= true; // din 1 devine 0 si din 0 devine 1
            db.SaveChanges();
            //Helperul TempData – poate seta o valoare care va fi disponibila intr-un request subsecvent
            TempData["message"] = "Restrictionarea a fost modificata cu succes";

            return RedirectToAction("Index");
        }


        public IEnumerable<SelectListItem> GetAllDomains()
        {
            // generam o lista de tipul SelectListItem fara elemente
            var selectList = new List<SelectListItem>();
            // extragem toate domeniile din baza de date
            var domains = from dom in db.Domains
                          select dom;
            // iteram prin categorii
            foreach (var domain in domains)
            {
                // adaugam in lista elementele necesare pentru dropdown
                // id-ul domeniului si denumirea acestuia
                selectList.Add(new SelectListItem
                {
                    Value = domain.Id.ToString(),
                    Text = domain.DomainName.ToString()
                });
            }
            // returnam lista de domenii
            return selectList;
        }
    }

}

