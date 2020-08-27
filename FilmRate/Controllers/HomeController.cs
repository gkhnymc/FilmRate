using FilmRate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FilmReview.DAL;
namespace FilmRate.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        private mainContext db = new mainContext();
        public ActionResult Index()
        {
                return View(db.Films.OrderBy(x=>x.ReleaseDate).ToList());
        }
    }
}