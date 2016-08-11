using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Collections.Specialized;

namespace GcDesign.NewsArticles.Tracking
{
    public class NotificationJob
    {
        public ArticleInfo Article;
        public string ArticleLink;
        public string PortalTitle;

        public void NotifyLinkedSites(){

            try{

                StringCollection links = new StringCollection();
                TrackHelper.BuildLinks(Article.Summary, links);
                TrackHelper.BuildLinks(Article.Body, links);

                foreach(string link in links){

                    try
                    {

                        string pageText = TrackHelper.GetPageText(link);

                        if (pageText != null) {
                            bool success=false;

                            TrackBackProxy objTrackBackProxy =new TrackBackProxy();
                            success = objTrackBackProxy.TrackBackPing(pageText, link, Article.Title, ArticleLink, PortalTitle, "");

                            if (!success) {
                                // objEventLog.AddLog("Ping Exception", "Trackback failed ->" & link, DotNetNuke.Common.Globals.GetPortalSettings(), -1, DotNetNuke.Services.Log.EventLog.EventLogController.EventLogType.ADMIN_ALERT)

                                PingBackProxy objPingBackProxy = new PingBackProxy();
                                objPingBackProxy.Ping(pageText, ArticleLink, link);
                            }

                        }
}
                    catch
                    {}

                }
            }
            catch
            {
            
            }

        }
    }
}