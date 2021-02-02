using KurumsalWeb.Models.DataContext;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList.Mvc;
using KurumsalWeb.Models.Model;

namespace KurumsalWeb.Controllers
{
    public class HomeController : Controller
    {
        private KurumsalDBContext db = new KurumsalDBContext();
        // GET: Home
        [Route("Anasayfa")]
        [Route("Home/Index")]
        public ActionResult Index()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();

            ViewBag.Hizmetler = db.Hizmet.ToList().OrderByDescending(x => x.HizmetId);
            
            return View();
        }
        public ActionResult SliderPartial()
        {
            return View(db.Slider.ToList().OrderByDescending(x=>x.SliderId));
        }
        public ActionResult HizmetPartial()
        {
            return View(db.Hizmet.ToList());
        }
        [Route("Hakkimizda")]
        [Route("Home/Hakkimizda")]
        public ActionResult Hakkimizda()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.Hakimizda.SingleOrDefault());
        }
        [Route("Hizmetlerimiz")]
        [Route("Home/Hizmetlerimiz")]
        public ActionResult hizmetlerimiz()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.Hizmet.ToList().OrderByDescending(x=>x.HizmetId));
        }
        [Route("iletisim")]
        [Route("Home/Iletisim")]
        public ActionResult Iletisim()
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.Iletisim.SingleOrDefault());
        }

        [HttpPost]
        public ActionResult Iletisim(string adsoyad=null,string email=null,string konu=null, string mesaj=null)
        {
            if (adsoyad!=null &&email!=null)
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.EnableSsl = true;
                WebMail.UserName = "kurumsalweb22@gmail.com";
                WebMail.Password = "123456as.";
                WebMail.SmtpPort = 587;
                WebMail.Send("kurumsalweb22@gmail.com", konu,email +"-"+ mesaj);
                ViewBag.Uyari = "Mesajınız Başarılı olarak gönderilmiştir.";

            }
            else
            {
                ViewBag.Uyari = "Hata oluştu. tekrar deneyiniz";
            }
            return View();
        }
        [Route("BlogPost")]
        [Route("Home/Blog")]
        public ActionResult Blog(int sayfa=1)
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            return View(db.Blog.Include("Kategori").OrderByDescending(x => x.BlogId).ToPagedList(sayfa,5));
        }
        [Route("BlogPost/{kategoriad}/{id:int}")]
        public ActionResult KategoriBlog(int id,int sayfa=1)
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            var b = db.Blog.Include("Kategori").OrderByDescending(x=>x.BlogId).Where(x => x.Kategori.KategoriId == id).ToPagedList(sayfa,5);
            return View(b);
        }
        [Route("BlogPost/{baslik}-{id:int}")]
        public ActionResult BlogDetay(int id)
        {
            ViewBag.Kimlik = db.Kimlik.SingleOrDefault();
            var b = db.Blog.Include("Kategori").Include("Yorums").Where(x => x.BlogId == id).SingleOrDefault();
            return View(b);
        }
        public JsonResult YorumYap(string adsoyad,string eposta,string icerik,int blogId)
        {
            if (icerik==null)
            {
                return Json(true,JsonRequestBehavior.AllowGet);
            }
            db.Yorum.Add(new Yorum 
            {
            AdSoyad=adsoyad,
            Eposta=eposta,
            Icerik=icerik,
            BlogId=blogId,
            Onay=false
            });
            db.SaveChanges();

            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BlogKategoriPartial()
        {
           
            return PartialView(db.Kategori.Include("Blogs").ToList().OrderByDescending(x => x.KategoriAd));
        }
        public ActionResult BlogSonKayitPartial()
        {

            return PartialView(db.Blog.ToList().OrderByDescending(x => x.BlogId));
        }

        public ActionResult FooterPartial()
        {

            ViewBag.Hizmetler = db.Hizmet.ToList().OrderByDescending(x => x.HizmetId);
            ViewBag.Iletisim = db.Iletisim.SingleOrDefault();
            ViewBag.Blog = db.Blog.ToList().OrderByDescending(x => x.BlogId);
            return PartialView();
        }
        
    }
}