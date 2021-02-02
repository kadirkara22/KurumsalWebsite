using KurumsalWeb.Models.DataContext;
using KurumsalWeb.Models.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace KurumsalWeb.Controllers
{
    public class BlogController : Controller
    {
        private KurumsalDBContext db = new KurumsalDBContext();
        // GET: Blog
        public ActionResult Index()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return View(db.Blog.Include("Kategori").ToList().OrderByDescending(x=>x.BlogId));
        }
        public ActionResult Create()
        {
            ViewBag.KategoriId = new SelectList(db.Kategori , "KategoriId", "KategoriAd");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Blog blog,HttpPostedFileBase ResimURl)
        {
            if (ResimURl != null)
            {
                WebImage img = new WebImage(ResimURl.InputStream);
                FileInfo imginfo = new FileInfo(ResimURl.FileName);

                string blogimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                img.Resize(600, 400);
                img.Save("~/Uploads/Blog/" + blogimgname);

                blog.ResimURl = "/Uploads/Blog/" + blogimgname;

            }
            db.Blog.Add(blog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            if (id==null)
            {
                return HttpNotFound();
            }
            var b = db.Blog.Where(x => x.BlogId == id).SingleOrDefault();
            if (b==null)    
            {
                return HttpNotFound();
            }
            ViewBag.KategoriId = new SelectList(db.Kategori, "KategoriId", "KategoriAd", b.KategoriId);
            return View(b);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(int id,Blog blog,HttpPostedFileBase ResimURl)
        {
            if (ModelState.IsValid)
            {
                var b = db.Blog.Where(x => x.BlogId == id).SingleOrDefault();
                if (ResimURl != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(b.ResimURl)))
                        System.IO.File.Delete(Server.MapPath(b.ResimURl));

                    WebImage img = new WebImage(ResimURl.InputStream);
                    FileInfo imginfo = new FileInfo(ResimURl.FileName);

                    string blogimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(600, 400);
                    img.Save("~/Uploads/Blog/" + blogimgname);

                    b.ResimURl = "/Uploads/Blog/" + blogimgname;
                }
                b.Baslik = blog.Baslik;
                b.Icerik = blog.Icerik;
                b.KategoriId = blog.KategoriId;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blog);
        }
        
        public ActionResult Delete(int id)
        {
            var b = db.Blog.Find(id);
            if (b==null)
            {
                return HttpNotFound();
            }
            if (System.IO.File.Exists(Server.MapPath(b.ResimURl)))
            {
                System.IO.File.Delete(Server.MapPath(b.ResimURl));
            } 
            db.Blog.Remove(b);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}