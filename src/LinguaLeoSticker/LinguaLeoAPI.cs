using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace LinguaLeoSticker
{
    internal class LinguaLeoApi
    {
        private CookieContainer _cookie = new CookieContainer();
        private const string ApiUrl = "http://api.lingualeo.com/";

        public bool IsAuth { get; set; }

        private static bool WriteHttpRequest(string url, out string httpResponse, ref CookieContainer cookie)
        {
            httpResponse = "";

            StringBuilder sb = new StringBuilder();

            byte[] buf = new byte[8192];

            HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(url);
            request.CookieContainer = cookie;
            int count = 0;
            try
            {

                HttpWebResponse response = (HttpWebResponse)
                    request.GetResponse();


                Stream resStream = response.GetResponseStream();


                do
                {
                    if (resStream != null) count = resStream.Read(buf, 0, buf.Length);

                    if (count != 0)
                    {
                        var tempString = Encoding.UTF8.GetString(buf, 0, count);

                        sb.Append(tempString);
                    }
                } while (count > 0);

                httpResponse = sb.ToString();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public void Auth(string email, string password)
        {
            string response;

            if (WriteHttpRequest($"{ApiUrl}{$"api/login?email={email}&password={password}"}", out response, ref _cookie))
            {
                string errorMsg = "";
                try
                {
                    dynamic apiResponse = JsonConvert.DeserializeObject(response);
                    errorMsg = apiResponse.error_msg;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    if (errorMsg != "")
                    {
                        throw new ArgumentException(errorMsg);
                    }
                    else
                    {
                        IsAuth = true;
                    }
                }
            }
        }

        public void GetUserDict(out string[] userDict)
        {
            GetUserDictImpl1(out userDict);        
        }

        private void GetUserDictImpl1(out string[] userDict)
        {
            string response;
            userDict = null;
            List<string> dict = new List<string>();

            //return only 400 word, sorted by Id, research:param
            if (WriteHttpRequest(ApiUrl + "userdict", out response, ref _cookie))
            {
                dynamic apiResponse = JsonConvert.DeserializeObject(response);

                string errorMsg = apiResponse.error_msg;
                if (errorMsg != "")
                {
                    throw new ArgumentException(errorMsg);
                }
                else
                {
                    for (int i = 0; i < apiResponse.words.Count; i++)
                    {
                        string word = apiResponse.words[i].word_value;
                        string tword = apiResponse.words[i].translate_value;

                        dict.Add($"{word.ToLower()}:{tword.ToLower()}");
                    }

                }

                userDict = dict.ToArray();
            }
        }
    
        public void AddWord(string word, string tword, string context)
        {
            string response;

            var urlParsams = $"api/addword?word={word.ToLower()}&tword={tword.ToLower()}&context={context}";
            if (WriteHttpRequest(ApiUrl + urlParsams, out response, ref _cookie))
            {
                dynamic apiResponse = JsonConvert.DeserializeObject(response);
                if (apiResponse == null) throw new ArgumentNullException(nameof(apiResponse));

                if (apiResponse.error_msg != "")                
                {
                    string errorMsg = apiResponse.error_msg;
                    if (errorMsg == null) throw new ArgumentNullException(nameof(errorMsg));
                    throw new ArgumentException(errorMsg);                    
                }
            }
        }

        public string GetTranslate(string word)
        {
            string response;

            if (WriteHttpRequest(ApiUrl + "gettranslates?word=" + word, out response, ref _cookie))
            {
                try
                {


                    dynamic apiResponse = JsonConvert.DeserializeObject(response);
                    if (apiResponse == null) throw new ArgumentNullException(nameof(apiResponse));

                    if (apiResponse.error_msg == "")
                    {
                        return apiResponse.translate[0].value;
                    }
                    else
                    {
                        return apiResponse.error_msg;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return "Error, cant get translation!";
                }
            }

            return "Error, cant get translation!";
        }

        public bool is_Auth()
        {
            return IsAuth;
        }

    }

}
