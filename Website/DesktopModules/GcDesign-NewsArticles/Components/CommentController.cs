using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke;
using DotNetNuke.Common.Utilities;

namespace GcDesign.NewsArticles
{
    public class CommentController
    {
        public static void ClearCache(int articleID)
        {

            List<string> itemsToRemove = new List<string>();

            IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().ToLower().Contains("gcdesign-newsarticles-comments"))
                {
                    itemsToRemove.Add(enumerator.Key.ToString());
                }
            }

            foreach (string itemToRemove in itemsToRemove)
            {
                DataCache.RemoveCache(itemToRemove.Replace("DNN_", ""));
            }

        }

        #region " Public Methods "

        public List<CommentInfo> GetCommentList(int moduleID, int articleID, bool isApproved)
        {

            return GetCommentList(moduleID, articleID, isApproved, SortDirection.Ascending, Null.NullInteger);

        }

        public List<CommentInfo> GetCommentList(int moduleID, int articleID, bool isApproved, SortDirection direction, int maxCount)
        {

            string cacheKey = "gcdesign-newsarticles-comments-" + moduleID.ToString() + "-" + articleID.ToString() + "-" + isApproved.ToString() + "-" + direction.ToString() + "-" + maxCount.ToString();

            List<CommentInfo> objComments = (List<CommentInfo>)DataCache.GetCache(cacheKey);

            if (objComments == null)
            {
                objComments = CBO.FillCollection<CommentInfo>(DataProvider.Instance().GetCommentList(moduleID, articleID, isApproved, direction, maxCount));
                DataCache.SetCache(cacheKey, objComments);
            }

            return objComments;

        }

        public CommentInfo GetComment(int commentID)
        {

            return (CommentInfo)CBO.FillObject<CommentInfo>(DataProvider.Instance().GetComment(commentID));

        }

        public void DeleteComment(int commentID, int articleID)
        {

            DataProvider.Instance().DeleteComment(commentID);

            ArticleController.ClearArticleCache(articleID);
            CommentController.ClearCache(articleID);

        }

        public int AddComment(CommentInfo objComment)
        {

            int commentID = (int)DataProvider.Instance().AddComment(objComment.ArticleID, objComment.CreatedDate, objComment.UserID, objComment.Comment, objComment.RemoteAddress, objComment.Type, objComment.TrackbackUrl, objComment.TrackbackTitle, objComment.TrackbackBlogName, objComment.TrackbackExcerpt, objComment.AnonymousName, objComment.AnonymousEmail, objComment.AnonymousURL, objComment.NotifyMe, objComment.IsApproved, objComment.ApprovedBy);

            ArticleController.ClearArticleCache(objComment.ArticleID);
            CommentController.ClearCache(objComment.ArticleID);

            return commentID;

        }

        public void UpdateComment(CommentInfo objComment)
        {

            DataProvider.Instance().UpdateComment(objComment.CommentID, objComment.ArticleID, objComment.UserID, objComment.Comment, objComment.RemoteAddress, objComment.Type, objComment.TrackbackUrl, objComment.TrackbackTitle, objComment.TrackbackBlogName, objComment.TrackbackExcerpt, objComment.AnonymousName, objComment.AnonymousEmail, objComment.AnonymousURL, objComment.NotifyMe, objComment.IsApproved, objComment.ApprovedBy);

            ArticleController.ClearArticleCache(objComment.ArticleID);
            CommentController.ClearCache(objComment.ArticleID);

        }

        #endregion
    }
}