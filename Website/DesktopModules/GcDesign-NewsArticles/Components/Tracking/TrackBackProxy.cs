using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace GcDesign.NewsArticles.Tracking
{
    public class TrackBackProxy
    {
        #region " public Methods "

        public bool TrackBackPing(string pageText, string url, string title, string link, string blogname, string description)
        {

            DotNetNuke.Services.Log.EventLog.EventLogController objLogController = new DotNetNuke.Services.Log.EventLog.EventLogController();
            DotNetNuke.Services.Log.EventLog.EventLogController objEventLog = new DotNetNuke.Services.Log.EventLog.EventLogController();

            // objEventLog.AddLog("Ping Exception", "Ping with a return URL of ->" & link, DotNetNuke.Common.Globals.GetPortalSettings(), -1, DotNetNuke.Services.Log.EventLog.EventLogController.EventLogType.ADMIN_ALERT)

            string trackBackItem = GetTrackBackText(pageText, url, link);

            if (trackBackItem != null)
            {

                if (!trackBackItem.ToLower().StartsWith("http://"))
                {
                    trackBackItem = "http://" + trackBackItem;
                }

                string parameters = "title=" + HtmlEncode(title) + "&url=" + HtmlEncode(link) + "&blog_name=" + HtmlEncode(blogname) + "&excerpt=" + HtmlEncode(description);
                SendPing(trackBackItem, parameters);
            }
            else
            {

                // objEventLog.AddLog("Ping Exception", "Pinging ->" & link & " -> Trackback Text not found on this page!", DotNetNuke.Common.Globals.GetPortalSettings(), -1, DotNetNuke.Services.Log.EventLog.EventLogController.EventLogType.ADMIN_ALERT)

            }

            return true;

        }

        #endregion

        #region " private Methods "

        private string GetTrackBackText(string pageText, string url, string PostUrl)
        {
            if (!Regex.IsMatch(pageText, PostUrl, RegexOptions.IgnoreCase | RegexOptions.Singleline))
            {

                string sPattern = @"<rdf:\w+\s[^>]*?>(</rdf:rdf>)?";
                Regex r = new Regex(sPattern, RegexOptions.IgnoreCase);
                Match m;

                m = r.Match(pageText);
                while (m.Success)
                {
                    if (m.Groups.ToString().Length > 0)
                    {

                        string text = m.Groups[0].ToString();
                        if (text.IndexOf(url) > 0)
                        {
                            string tbPattern = @"trackback:ping=\""([^\""]+)\""";
                            Regex reg = new Regex(tbPattern, RegexOptions.IgnoreCase);
                            Match m2 = reg.Match(text);
                            if (m2.Success)
                            {
                                return m2.Result("$1");
                            }

                            return text;
                        }
                    }
                    m = m.NextMatch();
                }
            }

            return null;

        }

        private string HtmlEncode(string text)
        {

            return System.Web.HttpUtility.HtmlEncode(text);

        }

        private void SendPing(string trackBackItem, string parameters)
        {

            StreamWriter myWriter = null;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(trackBackItem);
            if (request != null)
            {
                request.UserAgent = "My User Agent String";
                request.Referer = "http://www.smcculloch.net/";
                request.Timeout = 60000;
            }

            request.Method = "POST";
            request.ContentLength = parameters.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = false;

            // Try
            myWriter = new StreamWriter(request.GetRequestStream());
            myWriter.Write(parameters);
            //Finally
            myWriter.Close();
            //   End Try
        }

        #endregion
    }
}