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
        const string api_url = "http://api.lingualeo.com/";

        public string GetTranslate(string word)
        {     

            StringBuilder sb = new StringBuilder();

            byte[] buf = new byte[8192];

            HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(api_url + "gettranslates?word=" + word);

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

                dynamic api_response = JsonConvert.DeserializeObject(sb.ToString());

                return api_response.translate[0].value;
            }
            catch (Exception ex)
            {
                return "Not found translate!";
            }
        }

    }

}
