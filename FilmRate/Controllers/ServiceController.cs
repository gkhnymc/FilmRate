using FilmReview.DAL;
using FilmReview.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FilmRate.Models;
using System.Web.Security;
using System.Web;
using System.IO;

namespace FilmRate.Controllers
{
    public class ServiceController : ApiController
    {
        private mainContext db = new mainContext();
        [Route("SignUp")]
        [HttpPost]
        public HttpResponseMessage SignUp(User user)
        {
            ReturnedResult result = new ReturnedResult();
            var checkUser = db.Users.Where(x => x.UserName == user.UserName).FirstOrDefault();

            if (checkUser != null)
            {
                result.ErrorString = "sistemde böyle bir kullanıcı mevcut. lüften kullanıcı adını değişitirip tekrar deneyiniz.";
                result.Success = false;                
            }

            else
            {
                if (ModelState.IsValid)
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    result.ErrorString = "başarıyla kayıt olundu. aşağıdan giriş yapabilirsiniz.";
                    result.Success = true;
                }

            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        [Route("SignIn")]
        [HttpPost]

        public HttpResponseMessage SignIn(User u)
        {
            ReturnedResult result = new ReturnedResult();
            var loggedUser = db.Users.Where(x => x.UserName == u.UserName && x.UserPassword == u.UserPassword).FirstOrDefault();
            if (loggedUser != null)
            {
                HttpCookie UserNameCookie = new HttpCookie("filmreviewusername");
                if (HttpContext.Current.Request.Cookies["filmreviewusername"] != null)
                {
                    UserNameCookie = HttpContext.Current.Request.Cookies["filmreviewusername"];
                    UserNameCookie.Expires = DateTime.Now.AddDays(-10);
                    HttpContext.Current.Response.Cookies.Add(UserNameCookie);
                }
                UserNameCookie.Expires = DateTime.Today.AddYears(10);
                UserNameCookie.Value = loggedUser.UserName.ToString();
                HttpContext.Current.Response.Cookies.Add(UserNameCookie);
                SessionPersister.LoginedUser = loggedUser as User;
                result.Success = true;
            }
            else
            {
                result.ErrorString = "Kullanıcı adı veya şifre hatalı!";
                result.Success = false;
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        [Route("Logout")]
        [HttpGet]
        public HttpResponseMessage Logout()
        {
            ReturnedResult result = new ReturnedResult();
            SessionPersister.LoginedUser = null;
            result.Success = true;
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        [Route("UpdateProfile")]
        [HttpPost]
        public HttpResponseMessage UpdateProfile(User NewUser)
        {
            ReturnedResult result = new ReturnedResult();
            var CurrentUser = db.Users.Where(x => x.ID == SessionPersister.LoginedUser.ID).FirstOrDefault();
            if (NewUser.UserName.NullIF() != "")
            {
                CurrentUser.UserName = NewUser.UserName;
            }
            if (NewUser.Email.NullIF() != "")
            {
                CurrentUser.Email = NewUser.Email;
            }
            try
            {
                db.SaveChanges();
                SessionPersister.LoginedUser = CurrentUser;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorString = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        [Route("UpdatePassword")]
        [HttpPost]
        public HttpResponseMessage UpdatePassword(User NewUser)
        {
            ReturnedResult result = new ReturnedResult();
            var CurrentUser = db.Users.Where(x => x.ID == SessionPersister.LoginedUser.ID).FirstOrDefault();
            if(NewUser.UserPassword.NullIF()!="")
                CurrentUser.UserPassword = NewUser.UserPassword;
            try
            {
                db.SaveChanges();
                SessionPersister.LoginedUser = CurrentUser;

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorString = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        [Route("DeleteFilm")]
        [HttpPost]
        public HttpResponseMessage DeleteFilm(Film film)
        {
            ReturnedResult result = new ReturnedResult();
            var ToDelete = db.Films.Where(x => x.ID == film.ID).FirstOrDefault();
            try
            {
                var toDeleteComments = db.Comments.Where(x => x.FilmID == ToDelete.ID).ToList();
                toDeleteComments.ForEach(x =>
                {
                    db.Comments.Remove(x);
                });
                db.Films.Remove(ToDelete);
                db.SaveChanges();
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorString = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        [Route("DeleteUser")]
        [HttpPost]
        public HttpResponseMessage DeleteUser(User user)
        {
            ReturnedResult result = new ReturnedResult();
            var ToDelete = db.Users.Where(x => x.ID == user.ID).FirstOrDefault();
            try
            {
                var toDeleteComments = db.Comments.Where(x => x.User.ID == ToDelete.ID).ToList();
                toDeleteComments.ForEach(x =>
                {
                    db.Comments.Remove(x);
                });
                db.Users.Remove(ToDelete);
                db.SaveChanges();
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorString = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        [Route("DeleteComment")]
        [HttpPost]
        public HttpResponseMessage DeleteComment(Comment comment)
        {
            ReturnedResult result = new ReturnedResult();
            var ToDelete = db.Comments.Where(x => x.ID == comment.ID).FirstOrDefault();
            try
            {
                db.Comments.Remove(ToDelete);
                db.SaveChanges();
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorString = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        [Route("AddComment")]
        [HttpPost]
        public HttpResponseMessage AddComment()
        {
            ReturnedResult result = new ReturnedResult();
            Comment comment = new Comment()
            {
                CommentText = HttpContext.Current.Request.Form["CommentText"],
                UserID = SessionPersister.LoginedUser.ID,
                FilmID = Convert.ToInt32(HttpContext.Current.Request.Form["FilmID"]),
                ReleaseDate = DateTime.Now
            };
            var selectedFilm = db.Films.Where(x => x.ID == comment.FilmID).FirstOrDefault();
            int commentsCount = selectedFilm.Comments.Count;
            selectedFilm.Rate = ((selectedFilm.Rate * commentsCount) + Convert.ToInt32(HttpContext.Current.Request.Form["FilmRate"])) / (commentsCount + 1);
            try
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorString = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [Route("AddFilm")]
        [HttpPost]
        public HttpResponseMessage AddFilm()
        {
            ReturnedResult result = new ReturnedResult();
            Film newFilm = new Film()
            {
                Director = HttpContext.Current.Request.Form["Director"],
                FilmName = HttpContext.Current.Request.Form["FilmName"],
                Genre = HttpContext.Current.Request.Form["Genre"],
                ReleaseDate = Convert.ToDateTime(HttpContext.Current.Request.Form["ReleaseDate"]),
                Rate = Convert.ToDouble(HttpContext.Current.Request.Form["FilmRate"]),
                TrailerLink = HttpContext.Current.Request.Form["TrailerLink"],
                FilmDescription = HttpContext.Current.Request.Form["FilmDescription"]

            };
            foreach (string file in HttpContext.Current.Request.Files)
            {
                var fileContent = HttpContext.Current.Request.Files[file];
                if (file == "Photo")
                {
                    newFilm.ImagePath = Path.GetFileName(fileContent.FileName);
                    var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Files/FilmImages"), newFilm.ImagePath);
                    fileContent.SaveAs(path);
                }
            }
            try
            {
                db.Films.Add(newFilm);
                db.SaveChanges();
                result.Success = true;
            }
            catch (Exception ex)
            {

                result.Success = false;
                result.ErrorString = "Getting Error while saving. " + ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        [Route("EditFilm")]
        [HttpPost]
        public HttpResponseMessage EditFilm()
        {
            ReturnedResult result = new ReturnedResult();
            int FilmID = Convert.ToInt32(HttpContext.Current.Request.Form["FilmID"]);
            var EditedFilm = db.Films.Where(x => x.ID == FilmID).FirstOrDefault();
            try
            {
                EditedFilm.Director = HttpContext.Current.Request.Form["Director"];
                EditedFilm.FilmName = HttpContext.Current.Request.Form["FilmName"];
                EditedFilm.Genre = HttpContext.Current.Request.Form["Genre"];
                EditedFilm.ReleaseDate = Convert.ToDateTime(HttpContext.Current.Request.Form["ReleaseDate"]);
                EditedFilm.TrailerLink = HttpContext.Current.Request.Form["TrailerLink"];
                EditedFilm.FilmDescription = HttpContext.Current.Request.Form["FilmDescription"];
                foreach (string file in HttpContext.Current.Request.Files)
                {
                    var fileContent = HttpContext.Current.Request.Files[file];
                    if (file == "Photo")
                    {
                        EditedFilm.ImagePath = Path.GetFileName(fileContent.FileName);
                        var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Files/FilmImages"), EditedFilm.ImagePath);
                        fileContent.SaveAs(path);
                    }
                }
                db.SaveChanges();
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorString=ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
