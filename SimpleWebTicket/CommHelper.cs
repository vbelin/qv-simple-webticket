using System;
using System.IO;
using System.Net;
using System.Text;

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
            catch (Exception)
            {
                return null;
            }
        }
    }

}