using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text.RegularExpressions;

using CookComputing.XmlRpc;

namespace GcDesign.NewsArticles.Tracking
{
    public class PingBackProxy : XmlRpcClientProtocol
    {
        #region " private Members "

        string _errorMessage;

        #endregion

        #region " private Properties "

        private string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

        #endregion

        #region " Public Methods "

        public bool Ping(string pageText, string sourceURI, string targetURI)
        {
            string pingbackURL = GetPingBackURL(pageText, targetURI, sourceURI);
            if (pingbackURL != null)
            {
                this.Url = pingbackURL;
                try
                {
                    Notifiy(sourceURI, targetURI);
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorMessage = "Error: " + ex.Message;
                }
            }
            return false;

        }

        [XmlRpcMethod("pingback.ping")]
        public void Notifiy(string sourceURI, string targetURI)
        {

            Invoke("Notifiy", new object[] { sourceURI, targetURI });

        }

        #endregion

        #region " private Methods "

        private string GetPingBackURL(string pageText, string url, string PostUrl)
        {
            if (!Regex.IsMatch(pageText, PostUrl, RegexOptions.IgnoreCase) | Regex.IsMatch(pageText, PostUrl, RegexOptions.Singleline))
            {
                if (pageText != null)
                {
                    string pat = @"<link rel=\""pingback\"" href=\""([^\""]+)\"" ?/?>";
                    Regex reg = new Regex(pat, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    Match m = reg.Match(pageText);
                    if (m.Success)
                    {
                        return m.Result("$1");
                    }
                }
            }

            return null;
        }

        #endregion
    }
}