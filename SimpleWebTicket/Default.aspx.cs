using System;

namespace SimpleWebTicket
{
    public partial class Default : System.Web.UI.Page
    {
        // If TrustedIPs are used in c:\ProgramData\QlikTech\WebServer\config.xml this should be set to true, otherwise false
        private const bool Anonymous = false;
        // If Windows Authentication is used for GetWebTicket.aspx then credentials may be specified below. If no credentials is provided, UseDefaultCredentials will be used
        private const string UserName = "";
        private const string Password = "";
        
        // URL that the user is redirected to after a successful login. AccessPoint is usually where you want to go
        private const string TryUrl = "/QlikView/";
        // URL redirected to after a failed login attempt
        private const string BackUrl = "";
        // Server where the QlikView AccessPoint resides (ends with slash)
        private const string AccessPointServer = "http://localhost/";
        
        // Variable declarations (DON'T CHANGE HERE)
        private string _userId = "";
        private string _userFriendlyName = "";
        private string _userGroups = "";
        private string _webTicket = ""; 
 
        protected void Page_Load(object sender, EventArgs e)
        {
            // [REQUIRED] Name of the user to get a ticket for
            _userId = "rfn";
            // [OPTIONAL] A friendly name for the user. For example if the username is a social security number of phonenumber the friendly name could be his/hers real name
            _userFriendlyName = "Rikard Braathen";
            // [OPTIONAL] Semicolon separated string with groups/roles the user belongs to for use with Section Access or authorization
            _userGroups = "PreSales;Europe;Stockholm Office";

            GetWebTicket();

            if (!String.IsNullOrEmpty(_webTicket))
                RedirectToQlikView();
            else
                Response.Write("An error occured!");
        }
    }
}