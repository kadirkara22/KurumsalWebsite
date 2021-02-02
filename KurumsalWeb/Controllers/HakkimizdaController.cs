using KurumsalWeb.Models.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KurumsalWeb.Models.Model;


namespace KurumsalWeb.Controllers
{
    public class HakkimizdaController : Controller
    {
        KurumsalDBContext db = new KurumsalDBContext();
        // GET: Hakkimizda
        public ActionResult Index()
        {
            var h = db.Hakimizda.ToList();
            return View(h);
        }
        
        public ActionResult Edit(int id)    
        {
            var h = db.Hakimizda.Where(x => x.HakkimizdaId == id).FirstOrDefault();
            return View(h);  
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(int id,Hakimizda h)
        {
            if (ModelState.IsValid)
            {
                var hakkimizda = db.Hakimizda.Where(x => x.HakkimizdaId == id).SingleOrDefault();

                hakkimizda.Aciklama = h.Aciklama;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(h);
        }
    }
}