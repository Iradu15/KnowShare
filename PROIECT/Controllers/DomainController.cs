using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROIECT.Data;
using System.Data;
using PROIECT.Models;
using Microsoft.EntityFrameworkCore;

namespace PROIECT.Controllers
{
    [Authorize(Roles = "Admin")]

    public class DomainsController : Controller
    {
        private readonly ApplicationDbContext db;

        public DomainsController(ApplicationDbContext context)
        {
            db = context;
        }
        public ActionResult Index()
        {
            //domain poate avea orice nume 
            var domains = from domain in db.Domains
                          orderby domain.DomainName
                          select domain;

            ViewBag.Domains = domains;
            return View();
        }

        public ActionResult Show(int id)
        {
            Domain domain = db.Domains.Find(id);

            return View(domain);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Domain dom)
        {

            if (ModelState.IsValid)
            {
                db.Domains.Add(dom);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(dom);
            }

        }

        public ActionResult Edit(int id)
        {
            Domain domain = db.Domains.Find(id);
            return View(domain);
        }

        [HttpPost]
        public ActionResult Edit(int id, Domain requestDomain)
        {
            Domain domain = db.Domains.Find(id);

            if (ModelState.IsValid)
            {
                domain.DomainName = requestDomain.DomainName;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(domain);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Domain Domain = db.Domains.Find(id);
            db.Domains.Remove(Domain);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
