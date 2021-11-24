using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EbayProject.Helping_Classes
{
    public class MailSender
    {

        public static bool SendForgotPasswordEmail(string email, string BaseUrl = "")
        {
            try
            {
                string MailBody = "<html><head></head><body><nav class='navbar navbar-default'><div class='container-fluid'>" +
                "</div> </nav><center><div><h1 class='text-center'>Password Reset!</h1>" +
                "<p class='text-center'> Simply click the button showing below to reset your password (Link will expire after date change): </p><br>" +
                "<button style='background-color: rgb(0,174,239);'>" +
                    "<a href='" + BaseUrl + "Auth/ResetPassword?email=" + StringCipher.Base64Encode(email) + "&time=" + StringCipher.Base64Encode(DateTime.Now.ToString("MM/dd/yyyy")) + "' style='text-decoration:none;font-size:15px;color:white;'>Reset Password</a>" +
                "</button>" +
                "<p style='color:red;'>Link will not work in spam. Please move this mail into your inbox.</p></div></center>" +
                "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";


                RestClient client = new RestClient();
                client.BaseUrl = new Uri("https://api.mailgun.net/v3");
                client.Authenticator = new HttpBasicAuthenticator("api", "key-ff43f744db29071a74f3106541e3b925");

                RestRequest request = new RestRequest();
                request.AddParameter("domain", "nodlays.co.uk", ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";
                request.AddParameter("from", "no.replay.nodlays@gmail.com");
                request.AddParameter("to", email);
                request.AddParameter("subject", "Ebay | Password Reset");

                request.AddParameter("html", MailBody);

                request.Method = Method.POST;

                string response = client.Execute(request).Content.ToString();


                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}