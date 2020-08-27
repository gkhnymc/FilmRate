using FilmReview.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FilmRate.Models
{
    public static class Extentions
    {
        public static string NullIF(this object obj)
        {
            if (obj == null)
                return "";
            else
                return obj.ToString().Trim();
        }
    }
    
    public static class SessionPersister
    {
        public static string usernameSessionvar = "_TEST";

        public static User LoginedUser
        {
            get
            {
                var sessionVar = HttpContext.Current.Session[usernameSessionvar];
                if (sessionVar != null)
                {
                    return sessionVar as User;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                HttpContext.Current.Session[usernameSessionvar] = value;
            }
        }
    }

    public class CustomPrincipal : IPrincipal
    {
        private User user;

        public CustomPrincipal(User username)
        {
            this.user = username;
            this.Identity = new GenericIdentity(user.ID.ToString());

        }
        public IIdentity Identity
        {
            get;
            set;
        }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }

        internal bool CheckRoles(string Controller, string Action)
        {
            return true;
        }
    }
    public class CustomAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public string Action { get; set; }
        public string Controller { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (SessionPersister.LoginedUser == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary("AdminLogin"));

            }
            else
            {
                if (Roles != null)
                {
                    if (Roles.Length > 0)
                    {
                        List<String> requiredRoles = Roles.Split(Convert.ToChar(",")).ToList();
                        if (requiredRoles.Where(x => x == SessionPersister.LoginedUser.IsAdminUser.ToString()).Count() == 0)
                        {
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary("AdminLogin"));
                        }
                    }
                }
                CustomPrincipal mp = new CustomPrincipal(SessionPersister.LoginedUser);
                if (Controller.NullIF() != "" && Action.NullIF() != "")
                {
                    if (!mp.CheckRoles(this.Controller, this.Action))
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary("AdminLogin"));
                    }
                }
            }
        }
    }
}