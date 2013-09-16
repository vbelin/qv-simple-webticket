using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace SimpleWebTicket
{
    public partial class Default
    {
        public string Execute(string address, string method = "GET", string data = "")
        {
            try
            {
                if (method == "POST")
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
                    if (!Anonymous)
                    {
                        if (UserName == "" && Password == "")
                            request.UseDefaultCredentials = true;
                        else
                            request.Credentials = new NetworkCredential(UserName, Password);

                        request.PreAuthenticate = true;
                    }
                    request.Method = method;
                    request.Timeout = 5000;
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                    var buffer = Encoding.UTF8.GetBytes(data);
                    request.ContentLength = buffer.Length;
                    var dataStream = request.GetRequestStream();
                    dataStream.Write(buffer, 0, buffer.Length);
                    dataStream.Close();

                    var response = (HttpWebResponse)request.GetResponse();
                    var responseStream = response.GetResponseStream();
                    var reader = new StreamReader(responseStream, Encoding.UTF8);
                    var result = reader.ReadToEnd();

                    reader.Close();
                    dataStream.Close();
                    response.Close();

                    return result;
                }
                else
                {
                    var req = WebRequest.Create(address.ToString());
                    var resp = req.GetResponse();
                    var sr = new StreamReader(resp.GetResponseStream());
                    return sr.ReadToEnd().Trim();
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message + "<br />");
                return null;
            }
        }

        /// <summary>
        /// Get webticket for specified user
        /// </summary>
        public void GetWebTicket()
        {
            if (String.IsNullOrEmpty(_userId))
                return;

            if (!String.IsNullOrEmpty(_userGroups))
                GetGroups();

            string webTicketXml = string.Format("<Global method=\"GetWebTicket\"><UserId>{0}</UserId>{1}</Global>", _userId, _userGroups);

            string result = Execute(GetWebTicketPath, "POST", webTicketXml);

            if (string.IsNullOrEmpty(result) || result.Contains("Invalid call"))
                return;

            XDocument doc = XDocument.Parse(result);

            _webTicket = doc.Root.Element("_retval_").Value;

            // Set friendly name cookie for AccessPoint
            if (!String.IsNullOrEmpty(_userFriendlyName))
            {
                var cookie = new HttpCookie("WelcomeName" + HttpUtility.UrlEncode(_userId)) { Value = _userFriendlyName, Path = String.Format("{0}QvAJAXZfc/", AccessPointServer) };
                Response.Cookies.Add(cookie);
            }
        }

        public void GetGroups()
        {
            var group = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(_userGroups))
            {
                group.Append("<GroupList>");

                foreach (string value in _userGroups.Split(';'))
                {
                    group.Append("<string>");
                    group.Append(value);
                    group.Append("</string>");
                }

                group.Append("</GroupList>");
                group.Append("<GroupsIsNames>");
                group.Append("true");
                group.Append("</GroupsIsNames>");
            }

            _userGroups = group.ToString();
        }

        public string GetSelections(string selections)
        {
            var selectionCollection = new StringBuilder();

            foreach (string value in selections.Split(';'))
            {
                selectionCollection.Append("&select=");
                selectionCollection.Append(value);
            }

            return selectionCollection.ToString();
        }

        /// <summary>
        /// Redirects to QlikView after succesfull retrieval of webticket
        /// </summary>
        /// <param name="document">QlikView document to open directly, bypassing AccessPoint</param>
        /// <param name="host">QlikView host name (as found in QEMC) is required when using document parameter</param>
        /// <param name="selections">Semicolon separated list of selections, ie: LB38,Yellow;LB39,Banana (Note: Only the first selection works at the moment)</param>
        public void RedirectToQlikView(string document = "", string host = "", string selections = "")
        {
            if (String.IsNullOrEmpty(document) || String.IsNullOrEmpty(host))
            {
                Response.Redirect(string.Format("{0}QvAJAXZfc/Authenticate.aspx?type=html&webticket={1}&try={2}&back={3}", AccessPointServer, _webTicket, TryUrl, BackUrl));
            }
            else
            {
                if (!String.IsNullOrEmpty(selections))
                    selections = GetSelections(selections);

                Response.Redirect(string.Format("{0}QvAJAXZfc/Authenticate.aspx?type=html&webticket={1}&try={2}&back={3}", AccessPointServer, _webTicket, Uri.EscapeDataString("/QvAJAXZfc/AccessPoint.aspx?open=&id=" + host + "%7C" + document + selections + "&client=Ajax"), BackUrl));
            }
        }

    
    }

}