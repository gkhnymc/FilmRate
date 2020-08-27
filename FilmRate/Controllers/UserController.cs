using FilmReview.DAL;
using FilmReview.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FilmRate.Models;

namespace FilmRate.Controllers
{
    public class UserController : Controller
    {

        private mainContext db = new mainContext();

        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        [Route("Profile")]
        public ActionResult profile()
        {
            if (SessionPersister.LoginedUser == null)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            return View();
        }

        [Route("UserRate")]
        public ActionResult userRate()
        {
            if (SessionPersister.LoginedUser == null)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            var CommentList = db.Comments.Where(x => x.UserID == SessionPersister.LoginedUser.ID).ToList();
            var FilmIdList = CommentList.Select(x => x.FilmID).Distinct().ToList();
            var FilmList = db.Films.Where(x => FilmIdList.Contains(x.ID)).ToList();
            ViewBag.CommentList = CommentList;
            ViewBag.FilmList = FilmList;
            return View();
        }      

        public ActionResult signIn()
        {
            return View();
        }
    }
}