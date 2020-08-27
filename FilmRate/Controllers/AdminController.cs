using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FilmRate.Models;
using FilmReview.DAL;

namespace FilmRate.Controllers
{
    public class AdminController : Controller
    {
        private mainContext db = new mainContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View("AdminLogin");
        }
        [CustomAuthorize(Roles = "True")]
        [Route("Users")]
        public ActionResult Users()
        {
            return View(db.Users.ToList());
        }

        [CustomAuthorize(Roles = "True")]
        [Route("Films")]
        public ActionResult Films()
        {

            return View(db.Films.ToList());
        }

        [CustomAuthorize(Roles = "True")]
        [Route("Comments")]
        public ActionResult Comments()
        {

            return View(db.Comments.ToList());
        }
        [Route("AdminLogin")]
        public ActionResult AdminLogin()
        {
            if (Request.Cookies["filmreviewusername"] != null)
            {
                ViewBag.UserName = Request.Cookies["filmreviewusername"].Value;
            }
            return View();
        }

        [CustomAuthorize(Roles = "True")]
        [Route("AdminPanel")]
        public ActionResult AdminPanel()
        {
            return View("AdminPanel");
        }
        [CustomAuthorize(Roles = "True")]
        [Route("EditFilmForm")]
        public PartialViewResult EditFilmForm(string FilmID)
        {
            int _FilmID = Convert.ToInt32(FilmID);
            var selected=db.Films.Where(x => x.ID == _FilmID).FirstOrDefault();
            return PartialView(selected);
        }
    }
}