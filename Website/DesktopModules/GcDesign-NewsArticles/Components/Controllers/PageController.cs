using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Common.Utilities;

namespace GcDesign.NewsArticles
{
    public class PageController
    {
        #region " Public Methods "

        public ArrayList GetPageList(int articleId)
        {
            return CBO.FillCollection(DataProvider.Instance().GetPageList(articleId), typeof(PageInfo));
        }

        public PageInfo GetPage(int pageId)
        {
            return (PageInfo)CBO.FillObject<PageInfo>(DataProvider.Instance().GetPage(pageId));
        }

        public void DeletePage(int pageId)
        {
            DataProvider.Instance().DeletePage(pageId);
        }

        public int AddPage(PageInfo objPage)
        {
            int pageId = DataProvider.Instance().AddPage(objPage.ArticleID, objPage.Title, objPage.PageText, objPage.SortOrder);
            ArticleController.ClearArticleCache(objPage.ArticleID);
            return pageId;
        }

        public void UpdatePage(PageInfo objPage)
        {
            DataProvider.Instance().UpdatePage(objPage.PageID, objPage.ArticleID, objPage.Title, objPage.PageText, objPage.SortOrder);
            ArticleController.ClearArticleCache(objPage.ArticleID);
        }

        #endregion
    }
}