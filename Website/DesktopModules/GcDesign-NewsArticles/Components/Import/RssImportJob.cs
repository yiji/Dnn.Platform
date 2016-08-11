using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.XPath;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Scheduling;
using System.Xml;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using System.Collections;

namespace GcDesign.NewsArticles.Import
{
    public class RssImportJob : SchedulerClient
    {
        #region " private Methods "

        private void ImportFeed(FeedInfo objFeed){

            XPathDocument doc;
            XPathNavigator navigator;
            XPathNodeIterator nodes;
            XPathNavigator node;

            // Create a new XmlDocument  
            doc = new XPathDocument(objFeed.Url);

            // Create navigator  
            navigator = doc.CreateNavigator();

            XmlNamespaceManager mngr = new XmlNamespaceManager(navigator.NameTable);
            mngr.AddNamespace("content", "http://purl.org/rss/1.0/modules/content/");

            // Get forecast with XPath  
            nodes = navigator.Select("/rss/channel/item");

            if (nodes.Count == 0) {
                ImportFeedRDF(objFeed);
            }

            DateTime publishedDate = DateTime.Now;

            while(nodes.MoveNext()){
                node = nodes.Current;

                XPathNavigator nodeTitle;
                XPathNavigator nodeDescription;
                XPathNavigator nodeLink;
                XPathNavigator nodeDate;
                XPathNavigator nodeGuid;
                XPathNavigator nodeEncoded;

                nodeTitle = node.SelectSingleNode("title");
                nodeDescription = node.SelectSingleNode("description");
                nodeLink = node.SelectSingleNode("link");
                nodeDate = node.SelectSingleNode("pubDate");
                nodeGuid = node.SelectSingleNode("guid");
                nodeEncoded = node.SelectSingleNode("content:encoded", mngr);

                string summary = "";
                if (nodeDescription != null) {
                    summary = nodeDescription.Value;
                }

                string pageDetail = "";
                if (nodeEncoded != null) {
                    pageDetail = nodeEncoded.Value;
                }
                else
                {
                    if (nodeDescription != null) {
                        pageDetail = nodeDescription.Value;
                    }
                }

                string guid = "";
                if (nodeGuid != null) {
                    guid = nodeGuid.Value;
                }
                else
                {
                    guid = nodeLink.Value;
                }

                ArticleController objArticleController = new ArticleController();
                int refint=0;
                List<ArticleInfo> objArticles = objArticleController.GetArticleList(objFeed.ModuleID, DateTime.Now, Null.NullDate, null, false, null, 25, 1, 25, ArticleConstants.DEFAULT_SORT_BY, ArticleConstants.DEFAULT_SORT_DIRECTION, true, false, Null.NullString, Null.NullInteger, true, true, false, false, false, false, Null.NullString, null, false, guid, Null.NullInteger, Null.NullString, Null.NullString, ref refint);

                if (objArticles.Count == 0) {

                    ArticleInfo objArticle = new ArticleInfo();

                    objArticle.AuthorID = objFeed.UserID;
                    objArticle.CreatedDate = DateTime.Now;
                    objArticle.Status = StatusType.Published;
                    objArticle.CommentCount = 0;
                    objArticle.RatingCount = 0;
                    objArticle.Rating = 0;
                    objArticle.ShortUrl = "";

                    objArticle.Title = nodeTitle.Value;
                    objArticle.IsFeatured = objFeed.AutoFeature;
                    objArticle.IsSecure = false;
                    objArticle.Summary = summary;

                    objArticle.LastUpdate = objArticle.CreatedDate;
                    objArticle.LastUpdateID = objFeed.UserID;
                    objArticle.ModuleID = objFeed.ModuleID;

                    objArticle.Url = nodeLink.Value;
                    if (objFeed.DateMode == FeedDateMode.ImportDate) {
                        objArticle.StartDate = publishedDate;
                    }
                    else
                    {
                        try{
                            string val = nodeDate.Value;

                            val = val.Replace("PST", "-0800");
                            val = val.Replace("MST", "-0700");
                            val = val.Replace("CST", "-0600");
                            val = val.Replace("EST", "-0500");

                            objArticle.StartDate = DateTime.Parse(val);
                                }
                        catch
                        {
                            objArticle.StartDate = publishedDate;
                        }
                    }

                    objArticle.EndDate = Null.NullDate;
                    if (objFeed.AutoExpire != Null.NullInteger && objFeed.AutoExpireUnit != FeedExpiryType.None) {
                        switch (objFeed.AutoExpireUnit){

                            case FeedExpiryType.Minute:
                                objArticle.EndDate = DateTime.Now.AddMinutes(objFeed.AutoExpire);
                                break;

                            case FeedExpiryType.Hour:
                                objArticle.EndDate = DateTime.Now.AddHours(objFeed.AutoExpire);
                                break;

                            case FeedExpiryType.Day:
                                objArticle.EndDate = DateTime.Now.AddDays(objFeed.AutoExpire);
                                break;

                            case FeedExpiryType.Month:
                                objArticle.EndDate = DateTime.Now.AddMonths(objFeed.AutoExpire);
                                break;

                            case FeedExpiryType.Year:
                                objArticle.EndDate = DateTime.Now.AddYears(objFeed.AutoExpire);
                                break;

                        }
                    }

                    objArticle.RssGuid = guid;

                    objArticle.ArticleID = objArticleController.AddArticle(objArticle);

                    PageInfo objPage = new PageInfo();
                    objPage.PageText = pageDetail;
                    objPage.ArticleID = objArticle.ArticleID;
                    objPage.Title = objArticle.Title;

                    PageController objPageController = new PageController();
                    objPageController.AddPage(objPage);

                    foreach(CategoryInfo objCategory in objFeed.Categories){
                        objArticleController.AddArticleCategory(objArticle.ArticleID, objCategory.CategoryID);
                    }

                    publishedDate = publishedDate.AddSeconds(-1);
                }
                else
                    {

                    if (objArticles.Count == 1){

                        objArticles[0].Title = nodeTitle.Value;
                        objArticles[0].Summary = summary;
                        objArticles[0].LastUpdate = DateTime.Now;
                        objArticleController.UpdateArticle(objArticles[0]);

                        PageController objPageController = new PageController();
                        ArrayList objPages  = objPageController.GetPageList(objArticles[0].ArticleID);

                        if (objPages.Count > 0) {
                            ((PageInfo)objPages[0]).PageText = pageDetail;
                            objPageController.UpdatePage((PageInfo)objPages[0]);
                        }

                    }

                }

            }

        }

        private void ImportFeedRDF(FeedInfo objFeed){

            XPathDocument doc;
            XPathNavigator navigator;
            XPathNodeIterator nodes;
            XPathNavigator node;

            // Create a new XmlDocument  
            doc = new XPathDocument(objFeed.Url);

            // Create navigator  
            navigator = doc.CreateNavigator();

            XmlNamespaceManager mngr = new XmlNamespaceManager(navigator.NameTable);
            mngr.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
            mngr.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");

            // Get forecast with XPath  
            nodes = navigator.Select("/rdf:RDF/*", mngr);

            while(nodes.MoveNext()){
                node = nodes.Current;

                if (node.Name.ToLower() == "item") {

                    string title = "";
                    string description = "";
                    string link = "";
                    string dateNode = "";
                    string guid = "";

                    XPathNodeIterator objChildNodes = node.SelectChildren(XPathNodeType.All);

                    while(objChildNodes.MoveNext()){
                        XPathNavigator objChildNode = objChildNodes.Current;

                        switch(objChildNode.Name.ToLower()){

                            case "title":
                                title = objChildNode.Value;
                                break;

                            case "description":
                                description = objChildNode.Value;
                                break;

                            case "link":
                                link = objChildNode.Value;
                                break;

                            case "date":
                                dateNode = objChildNode.Value;
                                break;

                            case "guid":
                                guid = objChildNode.Value;
                                break;

                        }
                    }

                    if (title != "" && link != "") {

                        if (guid == "") {
                            guid = link;
                        }

                        ArticleController objArticleController = new ArticleController();
                        int refint=0;
                        List<ArticleInfo> objArticles = objArticleController.GetArticleList(objFeed.ModuleID, DateTime.Now, Null.NullDate, null, false, null, 1, 1, 10, ArticleConstants.DEFAULT_SORT_BY, ArticleConstants.DEFAULT_SORT_DIRECTION, true, false, Null.NullString, Null.NullInteger, true, true, false, false, false, false, Null.NullString, null, false, guid, Null.NullInteger, Null.NullString, Null.NullString, ref refint);

                        if (objArticles.Count == 0) {

                            DateTime publishedDate  = DateTime.Now;

                            ArticleInfo objArticle = new ArticleInfo();

                            objArticle.AuthorID = objFeed.UserID;
                            objArticle.CreatedDate = DateTime.Now;
                            objArticle.Status = StatusType.Published;
                            objArticle.CommentCount = 0;
                            objArticle.RatingCount = 0;
                            objArticle.Rating = 0;
                            objArticle.ShortUrl = "";

                            objArticle.Title = title;
                            objArticle.IsFeatured = objFeed.AutoFeature;
                            objArticle.IsSecure = false;
                            objArticle.Summary = description;

                            objArticle.LastUpdate = publishedDate;
                            objArticle.LastUpdateID = objFeed.UserID;
                            objArticle.ModuleID = objFeed.ModuleID;

                            objArticle.Url = link;
                            if (objFeed.DateMode == FeedDateMode.ImportDate) {
                                objArticle.StartDate = publishedDate;
                            }
                            else
                            {
                                try{
                                    string val = dateNode;

                                    val = val.Replace("PST", "-0800");
                                    val = val.Replace("MST", "-0700");
                                    val = val.Replace("CST", "-0600");
                                    val = val.Replace("EST", "-0500");

                                    objArticle.StartDate = DateTime.Parse(val);
                                        }
                                catch{
                                    objArticle.StartDate = publishedDate;
                                }
                        }

                            objArticle.EndDate = Null.NullDate;
                            if (objFeed.AutoExpire != Null.NullInteger && objFeed.AutoExpireUnit != FeedExpiryType.None) {
                                switch(objFeed.AutoExpireUnit){

                                    case FeedExpiryType.Minute:
                                        objArticle.EndDate = DateTime.Now.AddMinutes(objFeed.AutoExpire);
                                        break;

                                    case FeedExpiryType.Hour:
                                        objArticle.EndDate = DateTime.Now.AddHours(objFeed.AutoExpire);
                                        break;

                                    case FeedExpiryType.Day:
                                        objArticle.EndDate = DateTime.Now.AddDays(objFeed.AutoExpire);
                                        break;

                                    case FeedExpiryType.Month:
                                        objArticle.EndDate = DateTime.Now.AddMonths(objFeed.AutoExpire);
                                        break;

                                    case FeedExpiryType.Year:
                                        objArticle.EndDate = DateTime.Now.AddYears(objFeed.AutoExpire);
                                        break;

                                }
                            }

                            objArticle.RssGuid = guid;

                            objArticle.ArticleID = objArticleController.AddArticle(objArticle);

                            PageInfo objPage = new PageInfo();
                            objPage.PageText = description;
                            objPage.ArticleID = objArticle.ArticleID;
                            objPage.Title = objArticle.Title;

                            PageController objPageController= new PageController();
                            objPageController.AddPage(objPage);

                            foreach(CategoryInfo objCategory in objFeed.Categories){
                                objArticleController.AddArticleCategory(objArticle.ArticleID, objCategory.CategoryID);
                            }

                            publishedDate = publishedDate.AddSeconds(-1);

                        }

                    }

                }

            }

        }

        #endregion

        #region " Public Methods "

        public void ImportFeeds(){

            FeedController objFeedController = new FeedController();
            List<FeedInfo> objFeeds = objFeedController.GetFeedList(Null.NullInteger, true);


            foreach(FeedInfo objFeed in objFeeds){
                if (this.ScheduleHistoryItem.GetSetting("NewsArticles-Import-Clear-" + objFeed.ModuleID) != "") {
                    if (Convert.ToBoolean(this.ScheduleHistoryItem.GetSetting("NewsArticles-Import-Clear-" + objFeed.ModuleID))) {
                        // Delete Articles
                        ArticleController objArticleController = new ArticleController();
                        int refint=0;
                        List<ArticleInfo> objArticles = objArticleController.GetArticleList(objFeed.ModuleID, DateTime.Now, Null.NullDate, null, false, null, 1000, 1, 1000, ArticleConstants.DEFAULT_SORT_BY, ArticleConstants.DEFAULT_SORT_DIRECTION, true, false, Null.NullString, Null.NullInteger, true, true, false, false, false, false, Null.NullString, null, false, Null.NullString, Null.NullInteger, Null.NullString, Null.NullString, ref refint);

                        this.ScheduleHistoryItem.AddLogNote(objArticles.Count.ToString());
                        foreach (ArticleInfo objArticle in objArticles){
                            objArticleController.DeleteArticleCategories(objArticle.ArticleID);
                            objArticleController.DeleteArticle(objArticle.ArticleID, objFeed.ModuleID);
                        }

                    }
                }
            }

            foreach(FeedInfo objFeed in objFeeds){
                try{
                    this.ScheduleHistoryItem.AddLogNote(objFeed.Url);    //OPTIONAL
                    ImportFeed(objFeed);
                }
                catch( Exception ex) {
                    this.ScheduleHistoryItem.AddLogNote("News Articles -> Failure to import feed: " + objFeed.Url + ex.ToString());     //OPTIONAL
            }
            }

        }

        #endregion

        #region " Constructors "

        public RssImportJob(DotNetNuke.Services.Scheduling.ScheduleHistoryItem objScheduleHistoryItem):base()
        {
            //MyBase.new()
            this.ScheduleHistoryItem = objScheduleHistoryItem;

        }

        #endregion

        #region " Interface Methods "

        public override void DoWork()
        {


            try
            {
                //notification that the event is progressing
                this.Progressing();    //OPTIONAL
                ImportFeeds();
                this.ScheduleHistoryItem.Succeeded = true;    //REQUIRED
            }
            catch (Exception exc) //REQUIRED
            {

                this.ScheduleHistoryItem.Succeeded = false;   //REQUIRED
                this.ScheduleHistoryItem.AddLogNote("News Articles -> Import RSS job failed. " + exc.ToString());     //OPTIONAL
                //notification that we have errored
                this.Errored(ref exc);    //REQUIRED
                //log the exception
                Exceptions.LogException(exc);   //OPTIONAL

            }

        }

        #endregion
    }
}