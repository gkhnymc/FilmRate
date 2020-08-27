using FilmRate.Models;
using FilmReview.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FilmRate.Controllers
{
    public class FilmController : Controller
    {
        private mainContext db = new mainContext();

        // GET: Film
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult filmList()
        {
            return View(db.Films.ToList());
        }

        [Route("FilmDetail")]
        public ActionResult filmDetails(string ID)
        {
            int id = Convert.ToInt32(ID);
            var selectedFilm = db.Films.Where(x => x.ID == id).FirstOrDefault();
            return View(selectedFilm);
        }
        [Route("SearchFilm")]
        public PartialViewResult SearchFilm(string Filter)
        {
            var result = db.Films.Where(x => x.FilmName.ToLower().Contains(Filter.ToLower()) || x.Genre.ToLower().Contains(Filter.ToLower()) || x.Director.ToLower().Contains(Filter.ToLower())).ToList();
            return PartialView(result);
        }
    }
}