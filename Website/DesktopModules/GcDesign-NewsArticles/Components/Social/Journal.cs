using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Services.Journal;

namespace GcDesign.NewsArticles.Components.Social
{
    public class Journal
    {
        public const string ContentTypeName = "Ventrian_Article_";

        #region "Internal Methods"

        internal void AddArticleToJournal(ArticleInfo objArticle, int portalId, int tabId, int journalUserId, int journalGroupID, string url)
        {
            string objectKey = "Ventrian_Article_" + objArticle.ArticleID.ToString() + "_" + journalGroupID.ToString();
            JournalItem ji = JournalController.Instance.GetJournalItemByKey(portalId, objectKey);

            if (ji != null)
            {
                JournalController.Instance.DeleteJournalItemByKey(portalId, objectKey);
            }

            ji = new JournalItem();

            ji.PortalId = portalId;
            ji.ProfileId = journalUserId;
            ji.UserId = journalUserId;
            ji.ContentItemId = objArticle.ArticleID;
            ji.Title = objArticle.Title;
            ji.ItemData = new ItemData();
            ji.ItemData.Url = url;
            ji.Summary = objArticle.Summary;
            ji.Body = null;
            ji.JournalTypeId = 15;
            ji.ObjectKey = objectKey;
            ji.SecuritySet = "E,";
            ji.SocialGroupId = journalGroupID;

            JournalController.Instance.SaveJournalItem(ji, tabId);
        }

        internal void AddCommentToJournal(ArticleInfo objArticle, CommentInfo objComment, int portalId, int tabId, int journalUserId, string url)
        {
            string objectKey = "Ventrian_Article_Comment_" + objArticle.ArticleID.ToString() + ":" + objComment.CommentID.ToString();
            JournalItem ji = JournalController.Instance.GetJournalItemByKey(portalId, objectKey);

            if (ji != null)
            {
                JournalController.Instance.DeleteJournalItemByKey(portalId, objectKey);
            }

            ji = new JournalItem();

            ji.PortalId = portalId;
            ji.ProfileId = journalUserId;
            ji.UserId = journalUserId;
            ji.ContentItemId = objComment.CommentID;
            ji.Title = objArticle.Title;
            ji.ItemData = new ItemData();
            ji.ItemData.Url = url;
            ji.Summary = objComment.Comment;
            ji.Body = null;
            ji.JournalTypeId = 18;
            ji.ObjectKey = objectKey;
            ji.SecuritySet = "E,";

            JournalController.Instance.SaveJournalItem(ji, tabId);
        }

        internal void AddRatingToJournal(ArticleInfo objArticle, RatingInfo objRating, int portalId, int tabId, int journalUserId, string url)
        {
            string objectKey = "Ventrian_Article_Rating_" + objArticle.ArticleID.ToString() + ":" + objRating.RatingID.ToString();
            JournalItem ji = JournalController.Instance.GetJournalItemByKey(portalId, objectKey);

            if (ji != null)
            {
                JournalController.Instance.DeleteJournalItemByKey(portalId, objectKey);
            }

            ji = new JournalItem();

            ji.PortalId = portalId;
            ji.ProfileId = journalUserId;
            ji.UserId = journalUserId;
            ji.ContentItemId = objRating.RatingID;
            ji.Title = objArticle.Title;
            ji.ItemData = new ItemData();
            ji.ItemData.Url = url;
            ji.Summary = objRating.Rating.ToString();
            ji.Body = null;
            ji.JournalTypeId = 17;
            ji.ObjectKey = objectKey;
            ji.SecuritySet = "E,";

            JournalController.Instance.SaveJournalItem(ji, tabId);
        }

        #endregion
    }
}