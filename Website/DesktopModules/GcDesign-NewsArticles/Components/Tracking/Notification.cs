using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Common.Utilities;

using System.Threading;

namespace GcDesign.NewsArticles.Tracking
{
    public class Notification
    {
        Thread t;

        public void NotifyExternalSites(ArticleInfo objArticle, string articleLink, string portalTitle)
        {

            NotificationJob objNotification = new NotificationJob();
            objNotification.Article = objArticle;
            objNotification.ArticleLink = articleLink;
            objNotification.PortalTitle = portalTitle;

            t = new Thread(objNotification.NotifyLinkedSites);
            t.IsBackground = true;
            t.Start();

        }

        public void NotifyWeblogs(string articleLink, string portalTitle)
        {

            PingJob objPing = new PingJob();
            Thread t = new Thread(objPing.NotifyWeblogs);
            objPing.ArticleLink = articleLink;
            objPing.PortalTitle = portalTitle;
            t.IsBackground = true;
            t.Start();

        }

    }
}