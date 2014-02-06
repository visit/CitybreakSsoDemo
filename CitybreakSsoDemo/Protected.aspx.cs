using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using JWT;

namespace CitybreakSsoDemo
{
    public partial class Protected : System.Web.UI.Page
    {
        private IDictionary<string, object> UserData
        {
            get { return Session["User"] as Dictionary<string, object>; }
            set { Session["User"] = value; }
        }

        private bool IsLoggedIn()
        {
            return UserData != null;
        }

        private void Logout()
        {
            Session.Clear();
        }

        private static HashSet<string> usedTokens = new HashSet<string>();

        private void EnsureUnusedToken(string tokenId)
        {
            //Very crude replay check

            if(usedTokens.Contains(tokenId))
                throw new Exception("Token already used");

            usedTokens.Add(tokenId);
        }

        public DateTime FromUnixTime(int unixTime)
        {
            //the times are sent as epoch in JWT
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //This secret is shared with the authenticating entity
            const string sharedSecretKey = "iJIUzI1NiJ9.eyJjbGFpbTEiOj";

            if (IsLoggedIn())
            {
                Message.Text = "You are already logged in.";
                RenderPayload(UserData);
                LogoutButton.Visible = true;
            }
            else
            {
                string token = Request.QueryString["token"];
                if (token != null)
                {
                    try
                    {
                        var payload = JsonWebToken.DecodeToObject(token, sharedSecretKey) as IDictionary<string, object>;
                        //The payload verified and deserialized successfully

                        //we should verify that the token is fresh
                        var expires = FromUnixTime((int) payload["exp"]);

                        if (DateTime.Now > expires)
                        {
                            throw new TokenExpiredException("Token expired at " + expires);
                        }

                        UserData = payload;
                        RenderPayload(payload);
                        Message.Text = "You have now been logged in.";
                        LogoutButton.Visible = true;

                        //This is to prevent replay attacks.
                        EnsureUnusedToken((string)payload["jti"]);

                        //This is a good time to use the payload to locate an existing user in The Portal db, or create one using the provided Dynamics org account from the payload
                    }
                    catch (SignatureVerificationException)
                    {
                        Message.Text = "Invalid token.";
                    }
                    catch (TokenExpiredException)
                    {
                        Message.Text = "Login token expired.";
                    }
                    
                }
                else
                {
                    //Not logged in, so redirect user to login page
                    var ssoUrl = ConfigurationManager.AppSettings["SsoUrl"];
                    bool containsQuestionMark = ssoUrl.Contains("?");


                    Response.Redirect(ssoUrl + (containsQuestionMark ? ":" : "?" ) + "redirectto=" + HttpUtility.UrlEncode(Request.Url.ToString()));
                }
            }
        }

        private void RenderPayload(IDictionary<string, object> payload)
        {
            foreach (KeyValuePair<string, object> keyValuePair in payload)
            {
                tokenPayload.Controls.Add(new HtmlGenericControl("dt") {InnerText = keyValuePair.Key});
                tokenPayload.Controls.Add(new HtmlGenericControl("dd") {InnerText = keyValuePair.Value.ToString()});
            }
        }

        protected void LogoutClick(object sender, EventArgs e)
        {
            Logout();
            LogoutButton.Visible = false;
            Response.Redirect("/");
        }
    }

    [Serializable]
    public class TokenExpiredException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public TokenExpiredException()
        {
        }

        public TokenExpiredException(string message) : base(message)
        {
        }

        public TokenExpiredException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TokenExpiredException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

}