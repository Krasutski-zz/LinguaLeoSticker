using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace LinguaLeoSticker
{
    class LinguaLeoAPI
    {
        private CookieContainer _cookie = new CookieContainer();
        private const string api_url = "http://api.lingualeo.com/";

        private bool WriteHttpRequest(string url, out string http_response, ref CookieContainer cookie)
        {
            http_response = "";

            StringBuilder sb = new StringBuilder();

            byte[] buf = new byte[8192];

            HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(url);
            request.CookieContainer = cookie;
            try
            {

                HttpWebResponse response = (HttpWebResponse)
                    request.GetResponse();


                Stream resStream = response.GetResponseStream();


                string tempString = null;
                int count = 0;

                do
                {

                    count = resStream.Read(buf, 0, buf.Length);

                    if (count != 0)
                    {

                        tempString = Encoding.UTF8.GetString(buf, 0, count);

                        sb.Append(tempString);
                    }
                } while (count > 0);
                
                http_response = sb.ToString();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());                
                return false;
            }
        }

        public bool Auth(string email, string password, out string error_msg)
        {
            string response = "";
            error_msg = "";


            if (WriteHttpRequest(api_url + string.Format("api/login?email={0}&password={1}", email, password), out response, ref _cookie) == true)
            {

                dynamic api_response = JsonConvert.DeserializeObject(response);
                
                try
                {
                    error_msg = api_response.error_msg;
                    if (error_msg == "")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return false;
        }

        public bool AddWord(string word, string tword, string context, out string error_msg)
        {
            string response = "";
            error_msg = "";


            string url_parsams = string.Format("api/addword?word={0}&tword={1}&context={2}", word, tword, context);
            if (WriteHttpRequest(api_url + url_parsams, out response, ref _cookie))
            {
                dynamic api_response = JsonConvert.DeserializeObject(response);

                if (api_response.error_msg != "")                
                {
                    error_msg = api_response.error_msg;
                    return false;
                }

                return true;
            }

            return false;
        }

        public string GetTranslate(string word)
        {
            const string error_msg = "Error, cant get translation!";
            string response = "";

            if (WriteHttpRequest(api_url + "gettranslates?word=" + word, out response, ref _cookie) == true)
            {
                try
                {


                    dynamic api_response = JsonConvert.DeserializeObject(response);

                    if (api_response.error_msg == "")
                    {
                        return api_response.translate[0].value;
                    }
                    else
                    {
                        return api_response.error_msg;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return error_msg;
                }
            }

            return error_msg;
        }

    }

}
