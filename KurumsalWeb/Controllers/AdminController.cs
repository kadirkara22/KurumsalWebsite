using KurumsalWeb.Models;
using KurumsalWeb.Models.DataContext;
using KurumsalWeb.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace KurumsalWeb.Controllers
{
    public class AdminController : Controller
    {
        KurumsalDBContext db = new KurumsalDBContext();
        // GET: Admin
        [Route("yonetimpaneli")]
        public ActionResult Index()
        {
            ViewBag.BlogSay = db.Blog.Count();
            ViewBag.KategoriSay = db.Kategori.Count();
            ViewBag.HizmetSay = db.Hizmet.Count();
            ViewBag.YorumSay = db.Yorum.Count();

            ViewBag.YorumOnay = db.Yorum.Where(x => x.Onay == false).Count();
            var sorgu = db.Kategori.ToList();
            return View(sorgu);
        }
        [Route("yonetimpanali/giris")]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Admin admin,string sifre)
        {
            var md5pass = Crypto.Hash(sifre, "MD5");
            var login = db.Admin.Where(x => x.Eposta == admin.Eposta).SingleOrDefault();
            if (login != null)
            {
                if (login.Eposta == admin.Eposta && login.Sifre == md5pass)
                {
                    Session["adminid"] = login.AdminID;
                    Session["eposta"] = login.Eposta;
                    Session["yetki"] = login.Yetki;
                    return RedirectToAction("Index", "Admin");
                }
            }
            ViewBag.Uyari = "Kullanıcı Adı veya Şifre Yanlış";
            return View(admin);

        }
        public ActionResult Logout()
        {
            Session["adminid"] = null;
            Session["eposta"] = null;
            Session.Abandon();
            return RedirectToAction("Login", "Admin");
           
        }
        public ActionResult SifremiUnuttum()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SifremiUnuttum(string email)
        {
            var mail = db.Admin.Where(x => x.Eposta == email).SingleOrDefault();
            if ( mail!=null)
            {
                Random rnd = new Random();
                int yeniSifre = rnd.Next();

                
                mail.Sifre = Crypto.Hash(yeniSifre.ToString(),"MD5");
                db.SaveChanges();

                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.EnableSsl = true;
                WebMail.UserName = "kurumsalweb22@gmail.com";
                WebMail.Password = "123456as.";
                WebMail.SmtpPort = 587;
                WebMail.Send(email, "Admin panel giriş şifreniz","Şifreniz :"+ yeniSifre);
                ViewBag.Uyari = "Şifreniz Başarılı olarak gönderilmiştir.";

            }
            else
            {
                ViewBag.Uyari = "Hata oluştu. tekrar deneyiniz";
            }
            return View();
           
        }

        public ActionResult Adminler()
        {
            return View(db.Admin.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }

      [HttpPost]
        public ActionResult Create(Admin admin,string sifre,string eposta)
        {
            if (ModelState.IsValid)
            {
                admin.Sifre = Crypto.Hash(sifre, "MD5");
                db.Admin.Add(admin);
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }
            return View(admin);
        }

        public ActionResult Edit(int id)
        {
            var a = db.Admin.Where(x => x.AdminID == id).SingleOrDefault();
            return View(a);
        }
        [HttpPost]
        public ActionResult Edit(int id,Admin admin,string sifre, string eposta)
        {
            
            if (ModelState.IsValid)
            {
                var a = db.Admin.Where(x => x.AdminID == id).SingleOrDefault();
                a.Sifre = Crypto.Hash(sifre,"MD5");
                a.Eposta = admin.Eposta;

                a.Yetki = admin.Yetki;
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }

            return View(admin);
        }
        public ActionResult Delete(int id)
        {
            var a = db.Admin.Where(x => x.AdminID == id).SingleOrDefault();
            if (a!=null)
            {
                db.Admin.Remove(a);
                db.SaveChanges();
                return RedirectToAction("Adminler");
            }
            return View();
        }
    }
}