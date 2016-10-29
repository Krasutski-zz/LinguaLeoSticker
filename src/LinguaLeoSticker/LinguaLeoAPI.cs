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
        private bool _is_auth = false;
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

        public void Auth(string email, string password)
        {
            string response = "";

            if (WriteHttpRequest(api_url + string.Format("api/login?email={0}&password={1}", email, password), out response, ref _cookie) == true)
            {
                string error_msg = "";
                try
                {
                    dynamic api_response = JsonConvert.DeserializeObject(response);
                    error_msg = api_response.error_msg;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    if (error_msg != "")
                    {
                        throw new System.ArgumentException(error_msg);
                    }
                    else
                    {
                        _is_auth = true;
                    }
                }
            }
        }

        public void GetUserDict(out string[] user_dict)
        {
            //debug todo del
            if (true)
            {
                GetUserDictImpl1(out user_dict);
            }
            else
            {
                GetUserDictImpl2(out user_dict);
            }

        }

        private void GetUserDictImpl1(out string[] user_dict)
        {
            string response = "";
            user_dict = null;
            List<string> dict = new List<string>();

            //return only 400 word, sorted by Id, research:param
            if (WriteHttpRequest(api_url + "userdict", out response, ref _cookie))
            {
                dynamic api_response = JsonConvert.DeserializeObject(response);

                string error_msg = api_response.error_msg;
                if (error_msg != "")
                {
                    throw new System.ArgumentException(error_msg);
                }
                else
                {
                    for (int i = 0; i < api_response.words.Count; i++)
                    {
                        string word = api_response.words[i].word_value;
                        string tword = api_response.words[i].translate_value;

                        dict.Add(string.Format("{0}:{1}", word.ToLower(), tword.ToLower()));
                    }

                }

                user_dict = dict.ToArray();
            }
        }
    

        private void GetUserDictImpl2(out string[] user_dict)
        {
            string response = "";
            user_dict = null;

            List<string> list_dict = new List<string>();

            //new words
            //string url = "http://lingualeo.com/ru/userdict/json?sortBy=date&wordType=1&filter=no_translate&page=1&groupId=dictionary";
            //100 words on page
            string url = "http://lingualeo.com/ru/userdict/json?sortBy=date&filter=all&page=2";

            if (WriteHttpRequest(url, out response, ref _cookie))
            {
                dynamic api_response = JsonConvert.DeserializeObject(response);

                if (api_response.error_msg != "")
                {
                    string error_msg = api_response.error_msg;

                    throw new System.ArgumentException(error_msg);
                }
                else
                {

                    for (int i = 0; i < api_response.userdict3.Count; i++)
                    {
                        var dict = api_response.userdict3[i];

                        for (int j = 0; j < (int)dict.count; j++)
                        {
                            try
                            {
                                string word = dict.words[j].word_value;
                                string tword = dict.words[j].user_translates[0].translate_value;

                                list_dict.Add(string.Format("{0}:{1}", word.ToLower(), tword.ToLower()));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }
                    }

                    user_dict = list_dict.ToArray();
                }
            }
        }

        public void AddWord(string word, string tword, string context)
        {
            string response = "";

            string url_parsams = string.Format("api/addword?word={0}&tword={1}&context={2}", word, tword, context);
            if (WriteHttpRequest(api_url + url_parsams, out response, ref _cookie))
            {
                dynamic api_response = JsonConvert.DeserializeObject(response);

                if (api_response.error_msg != "")                
                {
                    string error_msg = api_response.error_msg;
                    throw new System.ArgumentException(error_msg);                    
                }
            }
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

        public bool is_Auth()
        {
            return _is_auth;
        }

    }

}
