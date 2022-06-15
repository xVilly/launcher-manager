using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace launcher_manager
{
    class Messages
    {
        public static List<string> DevConsole { get; set; }

        public static HttpClient ApiClient { get; set; }

        public static JObject AuthenticationJson { get; set; }
        public static bool LoggedIn = false;

        public static string Expiration = "";

        public static void Initialize()
        {
            DevConsole = new List<string>();
            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static void LogDevConsole(string log)
        {
            DateTime n = DateTime.Now;
            DevConsole.Add(n.ToShortTimeString() + " " + log);
        }

        public static async Task<string> Heartbeat(string token)
        {
            string url = "http://127.0.0.1:5000/api/hb";
            try
            {
                LogDevConsole("PUT " + url);
                using (var request = new HttpRequestMessage(HttpMethod.Put, url))
                {
                    request.Headers.Add("Token", token);
                    var response = await ApiClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine(responseString);
                        JObject responseJson = JObject.Parse(responseString);
                        LogDevConsole("launcher-api -> " + (int)response.StatusCode + ": " + responseJson["message"].ToString());
                        Expiration = responseJson["exp"].ToString();
                        return responseJson["message"].ToString();
                    }
                    else
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        JObject responseJson = JObject.Parse(responseString);
                        Debug.WriteLine("Error " + response.StatusCode.ToString() + ":" + responseJson["message"]);
                        LogDevConsole("launcher-api -> " + (int)response.StatusCode + ": " + responseJson["message"].ToString());
                        return responseJson["message"].ToString();
                    }
                }
            }
            catch
            {
                return "Connection failed..";
            }
        }

        public static async Task<string> Authenticate(string user, string password)
        {
            string url = "http://127.0.0.1:5000/api/auth";
            try
            {
                LogDevConsole("GET "+url);
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    request.Headers.Add("D-Username", user);
                    request.Headers.Add("D-Password", password);
                    var response = await ApiClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine(responseString);
                        JObject responseJson = JObject.Parse(responseString);
                        AuthenticationJson = responseJson;
                        LoggedIn = true;
                        LogDevConsole("launcher-api -> " + (int)response.StatusCode + ": "+responseJson["message"].ToString());
                        return responseJson["message"].ToString();
                    }
                    else
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        JObject responseJson = JObject.Parse(responseString);
                        Debug.WriteLine("Error " + response.StatusCode.ToString() + ":" + responseJson["message"]);
                        LogDevConsole("launcher-api -> " + (int)response.StatusCode + ": " + responseJson["message"].ToString());
                        return responseJson["message"].ToString();
                    }
                }
            }
            catch
            {
                return "Connection failed..";
            }
        }
    }
}
