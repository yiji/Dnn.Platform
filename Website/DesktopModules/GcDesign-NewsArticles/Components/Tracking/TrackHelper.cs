using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace GcDesign.NewsArticles.Tracking
{
    public class TrackHelper
    {
        public static StringCollection BuildLinks(string text, StringCollection links)
        {

            string pattern = @"(?:[hH][rR][eE][fF]\s*=)(?:[\s""""']*)(?!#|[Mm]ailto|[lL]ocation.|[jJ]avascript|.*css|.*this\.)(.*?)(?:[\s>""""'])";

            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);

            Match m = r.Match(Common.HtmlDecode(text));
            string link = "";
            while (m.Success)
            {
                if (m.Groups.ToString().Length > 0)
                {
                    link = m.Groups[1].ToString();
                    if (!links.Contains(link))
                    {
                        links.Add(link);
                    }
                }
                m = m.NextMatch();
            }

            return links;

        }

        public static string GetPageText(string inURL)
        {

            WebRequest req = WebRequest.Create(inURL);
            HttpWebRequest wreq = (HttpWebRequest)req;
            if (wreq != null)
            {
                wreq.UserAgent = "My User Agent String";
                wreq.Referer = "http://www.wwwcoder.com/";
                wreq.Timeout = 60000;
            }
            HttpWebResponse response = (HttpWebResponse)wreq.GetResponse();
            Stream s = response.GetResponseStream();
            string enc = response.ContentEncoding.Trim();
            if (enc == "")
            {
                enc = "us-ascii";
            }
            Encoding encode = System.Text.Encoding.GetEncoding(enc);
            StreamReader sr = new StreamReader(s, encode);
            return sr.ReadToEnd();

        }
    }
}