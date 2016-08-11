// 
// DotNetNuke?- http://www.dotnetnuke.com 
// Copyright (c) 2002-2013 
// by DotNetNuke Corporation 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software. 
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE. 
// 

using System;
using System.Data;
using System.Web.UI.WebControls;

using DotNetNuke;

namespace GcDesign.NewsArticles
{

    /// ----------------------------------------------------------------------------- 
    /// <summary> 
    /// 数据访问层抽象类，利用静态构造函数创建单例模式
    /// </summary> 
    /// <remarks> 
    /// </remarks> 
    /// <history> 
    /// </history> 
    /// ----------------------------------------------------------------------------- 
    public abstract class DataProvider
    {

        #region "Shared/Static Methods"

        /// <summary>
        /// 单例模式创建对象引用
        /// </summary>
        private static DataProvider objProvider = null;

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static DataProvider()
        {
            CreateProvider();
        }

        /// <summary>
        /// 动态实例化对象
        /// </summary>
        private static void CreateProvider()
        {
            objProvider = (DataProvider)DotNetNuke.Framework.Reflection.CreateObject("data", "GcDesign.NewsArticles", "");
        }

        /// <summary>
        /// 返回数据提供程序
        /// </summary>
        /// <returns></returns>
        public static DataProvider Instance()
        {
            return objProvider;
        }

        #endregion


        #region " Abstract methods "

        public abstract IDataReader GetArticleListByApproved(int moduleID, bool isApproved);
        public abstract IDataReader GetArticleListBySearchCriteria(int moduleID, DateTime currentDate, DateTime agedDate, int[] categoryID, bool matchAll, int[] categoryIDExclude, int maxCount, int pageNumber, int pageSize, string sortBy, string sortDirection, bool isApproved, bool isDraft, string keywords, int authorID, bool showPending, bool showExpired, bool showFeaturedOnly, bool showNotFeaturedOnly, bool showSecuredOnly, bool showNotSecuredOnly, string articleIDs, int[] tagID, bool matchAllTag, string rssGuid, int customFieldID, string customValue, string linkFilter);
        public abstract IDataReader GetArticleListBySearchCriteria1(int moduleID, DateTime endDate, DateTime startDate, int[] categoryID, bool matchAll,int conditionMatchCount, int[] categoryIDExclude, int maxCount, int pageNumber, int pageSize, string sortBy, string sortDirection, bool isApproved, bool isDraft, string keywords, int authorID, bool showPending, bool timePeriod, bool showFeaturedOnly, bool showNotFeaturedOnly, bool showSecuredOnly, bool showNotSecuredOnly, string articleIDs, int[] tagID, bool matchAllTag, string rssGuid, int customFieldID, string customValue, string linkFilter);
        public abstract IDataReader GetArticle(int articleID);
        public abstract IDataReader GetArticleCategories(int articleID);
        public abstract IDataReader GetNewsArchive(int moduleID, int[] categoryID, int[] categoryIDExclude, int authorID, string groupBy, bool showPending);
        public abstract void DeleteArticle(int articleID);
        public abstract void DeleteArticleCategories(int articleID);
        public abstract int AddArticle(int authorID, int approverID, DateTime createdDate, DateTime lastUpdate, string title, string summary, bool isApproved, int numberOfViews, bool isDraft, DateTime startDate, DateTime endDate, int moduleID, string imageUrl, bool isFeatured, int lastUpdateID, string url, bool isSecure, bool isNewWindow, string metaTitle, string metaDescription, string metaKeywords, string pageHeadText, string shortUrl, string rssGuid);
        public abstract void AddArticleCategory(int articleID, int categoryID);
        public abstract void UpdateArticle(int articleID, int authorID, int approverID, DateTime createdDate, DateTime lastUpdate, string title, string summary, bool isApproved, int numberOfViews, bool isDraft, DateTime startDate, DateTime endDate, int moduleID, string imageUrl, bool isFeatured, int lastUpdateID, string url, bool isSecure, bool isNewWindow, string metaTitle, string metaDescription, string metaKeywords, string pageHeadText, string shortUrl, string rssGuid);
        public abstract void UpdateArticleCount(int articleID, int numberOfViews);

        public abstract bool SecureCheck(int portalID, int articleID, int userID);

        public abstract IDataReader GetAuthorList(int moduleID);
        public abstract IDataReader GetAuthorStatistics(int moduleID, int[] categoryID, int[] categoryIDExclude, int authorID, string sortBy, bool showPending);

        #region Category
        public abstract IDataReader GetCategoryList(int moduleID, int parentID);
        public abstract IDataReader GetCategoryListAll(int moduleID, int authorID, bool showPending, int sortType);
        public abstract IDataReader GetCategory(int categoryID);
        public abstract void DeleteCategory(int categoryID);
        public abstract int AddCategory(int moduleID, int parentID, string name, string image, string description, int sortOrder, bool inheritSecurity, int categorySecurityType, string metaTitle, string metaDescription, string metaKeywords);
        public abstract void UpdateCategory(int categoryID, int moduleID, int parentID, string name, string image, string description, int sortOrder, bool inheritSecurity, int categorySecurityType, string metaTitle, string metaDescription, string metaKeywords);
        
        //将节点移到某个节点之上 更新排序
        public abstract void MoveOrgBefore(int sourceID, int targetID, int parentID, int sortOrder);
        #endregion

        public abstract IDataReader GetCommentList(int moduleID, int articleID, bool isApproved, SortDirection direction, int maxCount);
        public abstract IDataReader GetComment(int commentID);
        public abstract void DeleteComment(int commentID);
        public abstract int AddComment(int articleID, DateTime createdDate, int userID, string comment, string remoteAddress, int type, string trackbackUrl, string trackbackTitle, string trackbackBlogName, string trackbackExcerpt, string anonymousName, string anonymousEmail, string anonymousURL, bool notifyMe, bool isApproved, int approvedBy);
        public abstract void UpdateComment(int commentID, int articleID, int userID, string comment, string remoteAddress, int type, string trackbackUrl, string trackbackTitle, string trackbackBlogName, string trackbackExcerpt, string anonymousName, string anonymousEmail, string anonymousURL, bool notifyMe, bool isApproved, int approvedBy);

        public abstract IDataReader GetCustomField(int customFieldID);
        public abstract IDataReader GetCustomFieldList(int moduleID);
        public abstract void DeleteCustomField(int customFieldID);
        public abstract int AddCustomField(int moduleID, string name, int fieldType, string fieldElements, string defaultValue, string caption, string captionHelp, bool isRequired, bool isVisible, int sortOrder, int validationType, int length, string regularExpression);
        public abstract void UpdateCustomField(int customFieldID, int moduleID, string name, int fieldType, string fieldElements, string defaultValue, string caption, string captionHelp, bool isRequired, bool isVisible, int sortOrder, int validationType, int length, string regularExpression);

        public abstract IDataReader GetCustomValueList(int articleID);
        public abstract int AddCustomValue(int articleID, int customFieldID, string customValue);
        public abstract void UpdateCustomValue(int customValueID, int articleID, int customFieldID, string customValue);
        public abstract void DeleteCustomValue(int articleID, int customFieldID);

        public abstract IDataReader GetFeed(int feedID);
        public abstract IDataReader GetFeedList(int moduleID, bool showActiveOnly);
        public abstract int AddFeed(int moduleID, string title, string url, int userID, bool autoFeature, bool isActive, int dateMode, int autoExpire, int autoExpireUnit);
        public abstract void UpdateFeed(int feedID, int moduleID, string title, string url, int userID, bool autoFeature, bool isActive, int dateMode, int autoExpire, int autoExpireUnit);
        public abstract void DeleteFeed(int feedID);

        public abstract IDataReader GetFeedCategoryList(int feedID);
        public abstract void AddFeedCategory(int feedID, int categoryID);
        public abstract void DeleteFeedCategory(int feedID);

        public abstract IDataReader GetFile(int fileID);
        public abstract IDataReader GetFileList(int articleID, string fileGuid);
        public abstract int AddFile(int articleID, string title, string fileName, string extension, int size, string contentType, string folder, int sortOrder, string fileGuid);
        public abstract void UpdateFile(int fileID, int articleID, string title, string fileName, string extension, int size, string contentType, string folder, int sortOrder, string fileGuid);
        public abstract void DeleteFile(int fileID);

        public abstract IDataReader GetImage(int imageID);
        public abstract IDataReader GetImageList(int articleID, string imageGuid);
        public abstract int AddImage(int articleID, string title, string fileName, string extension, int size, int width, int height, string contentType, string folder, int sortOrder, string imageGuid, string description);
        public abstract void UpdateImage(int imageID, int articleID, string title, string fileName, string extension, int size, int width, int height, string contentType, string folder, int sortOrder, string imageGuid, string description);
        public abstract void DeleteImage(int imageID);

        public abstract void AddMirrorArticle(int articleID, int linkedArticleID, int linkedPortalID, bool autoUpdate);
        public abstract IDataReader GetMirrorArticle(int articleID);
        public abstract IDataReader GetMirrorArticleList(int linkedArticleID);

        public abstract IDataReader GetPageList(int articleID);
        public abstract IDataReader GetPage(int pageID);
        public abstract void DeletePage(int pageID);
        public abstract int AddPage(int articleID, string title, string pageText, int sortOrder);
        public abstract void UpdatePage(int pageID, int articleID, string title, string pageText, int sortOrder);

        public abstract int AddRating(int articleID, int userID, DateTime createdDate, double rating);
        public abstract IDataReader GetRating(int articleID, int userID);
        public abstract IDataReader GetRatingByID(int ratingID);
        public abstract void DeleteRating(int ratingID);

        public abstract int AddHandout(int moduleID, int userID, string name, string description);
        public abstract void AddHandoutArticle(int handoutID, int articleID, int sortOrder);
        public abstract void DeleteHandout(int handoutID);
        public abstract void DeleteHandoutArticleList(int handoutID);
        public abstract IDataReader GetHandout(int handoutID);
        public abstract IDataReader GetHandoutList(int userID);
        public abstract IDataReader GetHandoutArticleList(int handoutID);
        public abstract void UpdateHandout(int handoutID, int moduleID, int userID, string name, string description);

        public abstract IDataReader GetTag(int tagID);
        public abstract IDataReader GetTagByName(int moduleID, string nameLowered);
        public abstract IDataReader ListTag(int moduleID, int maxCount);
        public abstract int AddTag(int moduleID, string name, string nameLowered);
        public abstract void UpdateTag(int tagID, int moduleID, string name, string nameLowered, int usages);
        public abstract void DeleteTag(int tagID);

        public abstract void AddArticleTag(int articleID, int tagID);
        public abstract void DeleteArticleTag(int articleID);
        public abstract void DeleteArticleTagByTag(int tagID);

        #endregion


        #region "Email模版方法 EmailTemplate Methods "

        public abstract IDataReader GetEmailTemplate(int templateID);
        public abstract IDataReader GetEmailTemplateByName(int moduleID, string name);
        public abstract IDataReader ListEmailTemplate(int moduleID);
        public abstract int AddEmailTemplate(int moduleID, string name, string subject, string template);
        public abstract void UpdateEmailTemplate(int templateID, int moduleID, string name, string subject, string template);
        public abstract void DeleteEmailTemplate(int templateID);

        #endregion


    }
}