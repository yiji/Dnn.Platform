using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Collections.Specialized;

namespace GcDesign.NewsArticles.Tracking
{
    public class PingJob
    {
        public string ArticleLink;
        public string PortalTitle;

        public void NotifyWeblogs()
        {

            try
            {

                PingProxy objPing = new PingProxy();
                objPing.Ping(PortalTitle, ArticleLink);
            }
            catch
            {
                // Anything can happen here, so just swallow exception
            }
            finally
            {
                System.Threading.Thread.CurrentThread.Abort();
            }




        }
    }
}