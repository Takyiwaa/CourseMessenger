using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace CourseMessengerWeb.Components
{
    public class SmsModule
    {
        public void SendSms(string phoneNumber, string message)
        {
            try
            {
                using (var wc = new WebClient())
                {
                    var url = "https://api.smsgh.com/v3/messages/send?From={From}&To={To}&Content={Content}&ClientId={ClientId}&ClientSecret={ClientSecret}";
                    url = url.Replace("{From}", "CMessenger");
                    url = url.Replace("{To}", phoneNumber);
                    url = url.Replace("{Content}", message);
                    url = url.Replace("{ClientId}", "tnaspqgl");
                    url = url.Replace("{ClientSecret}", "xebiyjfl");

                    var response = wc.DownloadString(new Uri(url));
                }
            }
            catch (Exception exception)
            {
                
            }
        }
    }
}