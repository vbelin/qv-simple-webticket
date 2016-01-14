using System;
using System.Configuration;

namespace SimpleWebTicket
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // [REQUIRED] Userid to get a ticket for
            UserDetails.UserId = "rikard";

            // [OPTIONAL] Semicolon separated string with groups/roles the user belongs to for use with Section Access or authorization
            UserDetails.UserGroups = "PreSales;Europe;Stockholm Office";

            #region Forms authentication example
            //HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            //UserDetails.UserId = ticket.Name;
            #endregion

            if (!String.IsNullOrEmpty(UserDetails.UserId))
            {
                UserDetails.UserId = ConfigurationManager.AppSettings["Prefix"] + UserDetails.UserId;

                GetWebTicket();

                if (!String.IsNullOrEmpty(UserDetails.WebTicket))
                {
                    // Note: If redirecting directly to a document, the QVS host as defined in QMC must also be specified!
                    //RedirectToQlikView("Movies Database.qvw", "QVS@2008r2");
                    RedirectToQlikView();
                }
                else
                {
                    Response.Write(String.Format("Failed to retrieve web ticket for user id \"{0}\", try to verify the authentication settings.", UserDetails.UserId));
                }
            }
            else
            {
                Response.Write(String.Format("No user authenticated.", UserDetails.UserId));
            }

        }
    }
}