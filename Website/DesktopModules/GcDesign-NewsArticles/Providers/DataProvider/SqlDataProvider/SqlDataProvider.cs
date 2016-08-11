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
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using System.Web.UI.WebControls;

namespace GcDesign.NewsArticles
{

    /// ----------------------------------------------------------------------------- 
    /// <summary> 
    /// SQL Server实现DataProvider类抽象
    /// </summary> 
    /// <remarks> 
    /// </remarks> 
    /// <history> 
    /// </history> 
    /// ----------------------------------------------------------------------------- 
    public class SqlDataProvider : DataProvider
    {


        #region "Private Members"

        private const string ModuleQualifier = "GcDesign_NewsArticles_";

        private const string ProviderType = "data";

        private ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);
        private string _connectionString;
        private string _providerPath;
        private string _objectQualifier;
        private string _databaseOwner;


        #endregion

        #region "构造函数"

        public SqlDataProvider()
        {

            // 读取配置文件中关于该提供程序的信息
            Provider objProvider = (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

            // 读取provider的属性

            //获得连接字符串
            _connectionString = Config.GetConnectionString();

            if (_connectionString == "")
            {
                // 如果connectionString配置节中连接字符串为空，使用data配置的connectionString属性的值
                _connectionString = objProvider.Attributes["connectionString"];
            }

            _providerPath = objProvider.Attributes["providerPath"];

            _objectQualifier = objProvider.Attributes["objectQualifier"];
            if (_objectQualifier != "" & _objectQualifier.EndsWith("_") == false)
            {
                _objectQualifier += "_";
            }

            _databaseOwner = objProvider.Attributes["databaseOwner"];
            if (_databaseOwner != "" & _databaseOwner.EndsWith(".") == false)
            {
                _databaseOwner += ".";
            }

        }

        #endregion

        #region "属性Properties"

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public string ProviderPath
        {
            get { return _providerPath; }
        }

        public string ObjectQualifier
        {
            get { return _objectQualifier; }
        }

        public string DatabaseOwner
        {
            get { return _databaseOwner; }
        }

        #endregion

        #region "Private Methods"

        private string GetFullyQualifiedName(string name)
        {
            return DatabaseOwner + ObjectQualifier + ModuleQualifier + name;
        }

        #endregion

        #region "Public Methods"

        private object GetNull(object field)
        {
            //在程序中将程序中null转化为数据库中的null值
            return DotNetNuke.Common.Utilities.Null.GetNull(field, DBNull.Value);
        }

        #region "文章方法 Article Methods "
        /// <summary>
        /// 根据条件搜索新闻 isApproved是否发表
        /// </summary>
        /// <param name="moduleID">模块ID</param>
        /// <param name="isApproved">是否发表</param>
        /// <returns></returns>
        public override IDataReader GetArticleListByApproved(int moduleID, bool isApproved)
        {
            if (isApproved)
            {
                //根据条件搜索新闻 isApproved是否发表
                return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetArticleListBySearchCriteria", GetNull(moduleID), DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, isApproved, false, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, "StartDate", "DESC", DBNull.Value, DBNull.Value);
            }
            else
            {
                return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetArticleListBySearchCriteria", GetNull(moduleID), DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, isApproved, false, DBNull.Value, DBNull.Value, true, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, "StartDate", "DESC", DBNull.Value, DBNull.Value);
            }

        }

        /// <summary>
        /// 根据条件搜索新闻
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="currentDate"></param>
        /// <param name="agedDate"></param>
        /// <param name="categoryID"></param>
        /// <param name="matchAll"></param>
        /// <param name="categoryIDExclude"></param>
        /// <param name="maxCount"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <param name="isApproved"></param>
        /// <param name="isDraft"></param>
        /// <param name="keywords"></param>
        /// <param name="authorID"></param>
        /// <param name="showPending"></param>
        /// <param name="showExpired"></param>
        /// <param name="showFeaturedOnly"></param>
        /// <param name="showNotFeaturedOnly"></param>
        /// <param name="showSecuredOnly"></param>
        /// <param name="showNotSecuredOnly"></param>
        /// <param name="articleIDs"></param>
        /// <param name="tagID"></param>
        /// <param name="matchAllTag"></param>
        /// <param name="rssGuid"></param>
        /// <param name="customFieldID"></param>
        /// <param name="customValue"></param>
        /// <param name="linkFilter"></param>
        /// <returns></returns>
        public override IDataReader GetArticleListBySearchCriteria(int moduleID, DateTime currentDate, DateTime agedDate, int[] categoryID, bool matchAll, int[] categoryIDExclude, int maxCount, int pageNumber, int pageSize, string sortBy, string sortDirection, bool isApproved, bool isDraft, string keywords, int authorID, bool showPending, bool showExpired, bool showFeaturedOnly, bool showNotFeaturedOnly, bool showSecuredOnly, bool showNotSecuredOnly, string articleIDs, int[] tagID, bool matchAllTag, string rssGuid, int customFieldID, string customValue, string linkFilter)
        {
            string categories = DotNetNuke.Common.Utilities.Null.NullString;
            int categoryIDCount = Null.NullInteger;

            if (categoryID != null)
            {
                foreach (int category in categoryID)
                {
                    if (categories != DotNetNuke.Common.Utilities.Null.NullString)
                    {
                        categories = categories + ",";
                    }
                    categories = categories + category.ToString();
                }

                if (matchAll)
                {
                    categoryIDCount = categoryID.Length - 1;
                }
            }

            string tags = DotNetNuke.Common.Utilities.Null.NullString;
            int tagIDCount = Null.NullInteger;

            if (tagID != null)
            {
                foreach (int tag in tagID)
                {
                    if (tags != DotNetNuke.Common.Utilities.Null.NullString)
                    {
                        tags = tags + ",";
                    }
                    tags = tags + tag.ToString();
                }

                if (matchAllTag)
                {
                    tagIDCount = tagID.Length - 1;
                }
            }

            string categoriesExclude = DotNetNuke.Common.Utilities.Null.NullString;
            if (categoryIDExclude != null)
            {
                foreach (int category in categoryIDExclude)
                {
                    if (categoriesExclude != DotNetNuke.Common.Utilities.Null.NullString)
                    {
                        categoriesExclude = categoriesExclude + ",";
                    }
                    categoriesExclude = categoriesExclude + category.ToString();
                }
            }

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetArticleListBySearchCriteria", moduleID, GetNull(currentDate), GetNull(agedDate), GetNull(categories), GetNull(categoryIDCount), GetNull(categoriesExclude), GetNull(maxCount), GetNull(pageNumber), GetNull(pageSize), sortBy, sortDirection, isApproved, isDraft, GetNull(keywords), GetNull(authorID), GetNull(showPending), GetNull(showExpired), showFeaturedOnly, showNotFeaturedOnly, showSecuredOnly, showNotSecuredOnly, GetNull(articleIDs), GetNull(tags), GetNull(tagIDCount), GetNull(rssGuid), GetNull(customFieldID), GetNull(customValue), GetNull(linkFilter));
        }

        /// <summary>
        /// 根据条件搜索新闻
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="currentDate"></param>
        /// <param name="agedDate"></param>
        /// <param name="categoryID"></param>
        /// <param name="matchAll"></param>
        /// <param name="cinditionMatchCount">匹配条件个数</param>
        /// <param name="categoryIDExclude"></param>
        /// <param name="maxCount"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <param name="isApproved"></param>
        /// <param name="isDraft"></param>
        /// <param name="keywords"></param>
        /// <param name="authorID"></param>
        /// <param name="showPending"></param>
        /// <param name="showExpired"></param>
        /// <param name="showFeaturedOnly"></param>
        /// <param name="showNotFeaturedOnly"></param>
        /// <param name="showSecuredOnly"></param>
        /// <param name="showNotSecuredOnly"></param>
        /// <param name="articleIDs"></param>
        /// <param name="tagID"></param>
        /// <param name="matchAllTag"></param>
        /// <param name="rssGuid"></param>
        /// <param name="customFieldID"></param>
        /// <param name="customValue"></param>
        /// <param name="linkFilter"></param>
        /// <returns></returns>
        public override IDataReader GetArticleListBySearchCriteria1(int moduleID, DateTime endDate, DateTime startDate, int[] categoryID, bool matchAll, int conditionMatchCount, int[] categoryIDExclude, int maxCount, int pageNumber, int pageSize, string sortBy, string sortDirection, bool isApproved, bool isDraft, string keywords, int authorID, bool showPending, bool timePeriod, bool showFeaturedOnly, bool showNotFeaturedOnly, bool showSecuredOnly, bool showNotSecuredOnly, string articleIDs, int[] tagID, bool matchAllTag, string rssGuid, int customFieldID, string customValue, string linkFilter)
        {
            string categories = DotNetNuke.Common.Utilities.Null.NullString;
            int categoryIDCount = Null.NullInteger;

            if (categoryID != null)
            {
                foreach (int category in categoryID)
                {
                    if (categories != DotNetNuke.Common.Utilities.Null.NullString)
                    {
                        categories = categories + ",";
                    }
                    categories = categories + category.ToString();
                }

                if (matchAll)
                {
                    categoryIDCount = conditionMatchCount;//匹配条件个数 三个条件就是2 2个就是1
                }
            }

            string tags = DotNetNuke.Common.Utilities.Null.NullString;
            int tagIDCount = Null.NullInteger;

            if (tagID != null)
            {
                foreach (int tag in tagID)
                {
                    if (tags != DotNetNuke.Common.Utilities.Null.NullString)
                    {
                        tags = tags + ",";
                    }
                    tags = tags + tag.ToString();
                }

                if (matchAllTag)
                {
                    tagIDCount = tagID.Length - 1;
                }
            }

            string categoriesExclude = DotNetNuke.Common.Utilities.Null.NullString;
            if (categoryIDExclude != null)
            {
                foreach (int category in categoryIDExclude)
                {
                    if (categoriesExclude != DotNetNuke.Common.Utilities.Null.NullString)
                    {
                        categoriesExclude = categoriesExclude + ",";
                    }
                    categoriesExclude = categoriesExclude + category.ToString();
                }
            }

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetArticleListBySearchCriteria1", moduleID, GetNull(endDate), GetNull(startDate), GetNull(categories), GetNull(categoryIDCount), GetNull(categoriesExclude), GetNull(maxCount), GetNull(pageNumber), GetNull(pageSize), sortBy, sortDirection, isApproved, isDraft, GetNull(keywords), GetNull(authorID), GetNull(showPending), GetNull(timePeriod), showFeaturedOnly, showNotFeaturedOnly, showSecuredOnly, showNotSecuredOnly, GetNull(articleIDs), GetNull(tags), GetNull(tagIDCount), GetNull(rssGuid), GetNull(customFieldID), GetNull(customValue), GetNull(linkFilter));
        }

        /// <summary>
        /// 通过文章ID获取新闻
        /// </summary>
        /// <param name="articleID"></param>
        /// <returns></returns>
        public override IDataReader GetArticle(int articleID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetArticle", articleID);
        }

        /// <summary>
        /// 获取文章所在的栏目列表 直接包含栏目不包括父类
        /// </summary>
        /// <param name="articleID"></param>
        /// <returns></returns>
        public override IDataReader GetArticleCategories(int articleID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetArticleCategories", articleID);
        }

        /// <summary>
        /// 获取文章归档 某一归档类有多少文章
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="categoryID"></param>
        /// <param name="categoryIDExclude"></param>
        /// <param name="authorID"></param>
        /// <param name="groupBy"></param>
        /// <param name="showPending"></param>
        /// <returns></returns>
        public override IDataReader GetNewsArchive(int moduleID, int[] categoryID, int[] categoryIDExclude, int authorID, string groupBy, bool showPending)
        {
            string categories = DotNetNuke.Common.Utilities.Null.NullString;

            if (categoryID != null)
            {
                foreach (int category in categoryID)
                {
                    if (categories != DotNetNuke.Common.Utilities.Null.NullString)
                    {
                        categories = categories + ",";
                    }
                    categories = categories + category.ToString();
                }
            }

            string categoriesExclude = DotNetNuke.Common.Utilities.Null.NullString;

            if (categoryIDExclude != null)
            {
                foreach (int category in categoryIDExclude)
                {
                    if (categoriesExclude != DotNetNuke.Common.Utilities.Null.NullString)
                    {
                        categoriesExclude = categoriesExclude + ",";
                    }
                    categoriesExclude = categoriesExclude + category.ToString();
                }
            }

            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetNewsArchive", moduleID, GetNull(categories), GetNull(categoriesExclude), GetNull(authorID), groupBy, GetNull(showPending));
        }

        public override void DeleteArticle(int articleID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_DeleteArticle", articleID);
        }

        public override void DeleteArticleCategories(int articleID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_DeleteArticleCategories", articleID);
        }

        public override int AddArticle(int authorID, int approverID, DateTime createdDate, DateTime lastUpdate, string title, string summary, bool isApproved, int numberOfViews, bool isDraft, DateTime startDate, DateTime endDate, int moduleID, string imageUrl, bool isFeatured, int lastUpdateID, string url, bool isSecure, bool isNewWindow, string metaTitle, string metaDescription, string metaKeywords, string pageHeadText, string shortUrl, string rssGuid)
        {
            return  Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_AddArticle", authorID, approverID, createdDate, lastUpdate, title, GetNull(summary), isApproved, numberOfViews, isDraft, GetNull(startDate), GetNull(endDate), moduleID, GetNull(imageUrl), isFeatured, GetNull(lastUpdateID), GetNull(url), isSecure, isNewWindow, GetNull(metaTitle), GetNull(metaDescription), GetNull(metaKeywords), GetNull(pageHeadText), GetNull(shortUrl), GetNull(rssGuid)));
        }

        /// <summary>
        /// 给文章添加对应的栏目
        /// </summary>
        /// <param name="articleID"></param>
        /// <param name="categoryID"></param>
        public override void AddArticleCategory(int articleID, int categoryID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_AddArticleCategory", articleID, categoryID);
        }

        public override void UpdateArticle(int articleID, int authorID, int approverID, DateTime createdDate, DateTime lastUpdate, string title, string summary, bool isApproved, int numberOfViews, bool isDraft, DateTime startDate, DateTime endDate, int moduleID, string imageUrl, bool isFeatured, int lastUpdateID, string url, bool isSecure, bool isNewWindow, string metaTitle, string metaDescription, string metaKeywords, string pageHeadText, string shortUrl, string rssGuid)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_UpdateArticle", articleID, authorID, approverID, createdDate, lastUpdate, title, GetNull(summary), isApproved, numberOfViews, isDraft, GetNull(startDate), GetNull(endDate), moduleID, GetNull(imageUrl), isFeatured, GetNull(lastUpdateID), GetNull(url), isSecure, isNewWindow, GetNull(metaTitle), GetNull(metaDescription), GetNull(metaKeywords), GetNull(pageHeadText), GetNull(shortUrl), GetNull(rssGuid));
        }

        /// <summary>
        /// 更新文章浏览次数
        /// </summary>
        /// <param name="articleID"></param>
        /// <param name="numberOfViews"></param>
        public override void UpdateArticleCount(int articleID, int numberOfViews)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_UpdateArticleCount", articleID, numberOfViews);
        }

        /// <summary>
        /// SQL语句就是 select 1 怀疑无用
        /// </summary>
        /// <param name="portalID"></param>
        /// <param name="articleID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public override bool SecureCheck(int portalID, int articleID, int userID)
        {
            return (bool)SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_SecureCheck", portalID, articleID, userID);
        }

        #endregion

        #region " Author Methods "
        /// <summary>
        /// 获取作者列表
        /// </summary>
        /// <param name="moduleID"></param>
        /// <returns></returns>
        public override IDataReader GetAuthorList(int moduleID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetAuthorList", moduleID);
        }

        /// <summary>
        /// 归档文章 作者为条件 归档类有几条文章
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="categoryID"></param>
        /// <param name="categoryIDExclude"></param>
        /// <param name="authorID"></param>
        /// <param name="sortBy"></param>
        /// <param name="showPending"></param>
        /// <returns></returns>
        public override IDataReader GetAuthorStatistics(int moduleID, int[] categoryID, int[] categoryIDExclude, int authorID, string sortBy, bool showPending)
        {
            string categories = DotNetNuke.Common.Utilities.Null.NullString;

            if (categoryID != null)
            {
                foreach (int category in categoryID)
                {
                    if (categories != DotNetNuke.Common.Utilities.Null.NullString)
                    {
                        categories = categories + ",";
                    }
                    categories = categories + category.ToString();
                }
            }

            string categoriesExclude = DotNetNuke.Common.Utilities.Null.NullString;

            if (categoryIDExclude != null)
            {
                foreach (int category in categoryIDExclude)
                {
                    if (categoriesExclude != DotNetNuke.Common.Utilities.Null.NullString)
                    {
                        categoriesExclude = categoriesExclude + ",";
                    }
                    categoriesExclude = categoriesExclude + category.ToString();
                }
            }
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetAuthorStatistics", moduleID, GetNull(categories), GetNull(categoriesExclude), GetNull(authorID), sortBy, GetNull(showPending));
        }
        #endregion

        #region " Category Methods "

        public override IDataReader GetCategoryList(int moduleID, int parentID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetCategoryList", moduleID, parentID);
        }

        public override IDataReader GetCategoryListAll(int moduleID, int authorID, bool showPending, int sortType)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetCategoryListAll", moduleID, GetNull(authorID), GetNull(showPending), sortType);
        }

        public override IDataReader GetCategory(int categoryID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetCategory", categoryID);
        }

        public override void DeleteCategory(int categoryID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_DeleteCategory", categoryID);
        }

        public override int AddCategory(int moduleID, int parentID, string name, string image, string description, int sortOrder, bool inheritSecurity, int categorySecurityType, string metaTitle, string metaDescription, string metaKeywords)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_AddCategory", moduleID, parentID, name, image, GetNull(description), sortOrder, inheritSecurity, categorySecurityType, GetNull(metaTitle), GetNull(metaDescription), GetNull(metaKeywords)));
        }

        public override void UpdateCategory(int categoryID, int moduleID, int parentID, string name, string image, string description, int sortOrder, bool inheritSecurity, int categorySecurityType, string metaTitle, string metaDescription, string metaKeywords)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_UpdateCategory", categoryID, moduleID, parentID, name, image, GetNull(description), sortOrder, inheritSecurity, categorySecurityType, GetNull(metaTitle), GetNull(metaDescription), GetNull(metaKeywords));
        }

        public override void MoveOrgBefore(int sourceID, int targetID, int parentID, int sortOrder)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("Category_MoveNodeBefore"), sourceID, targetID, parentID, sortOrder);
        }

        #endregion

        #region " Comment Methods "
        public override IDataReader GetCommentList(int moduleID, int articleID, bool isApproved, SortDirection direction, int maxCount)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetCommentList", moduleID, GetNull(articleID), isApproved, direction, GetNull(maxCount));
        }

        public override IDataReader GetComment(int commentID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetComment", commentID);
        }

        public override void DeleteComment(int commentID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_DeleteComment", commentID);
        }

        public override int AddComment(int articleID, DateTime createdDate, int userID, string comment, string remoteAddress, int type, string trackbackUrl, string trackbackTitle, string trackbackBlogName, string trackbackExcerpt, string anonymousName, string anonymousEmail, string anonymousURL, bool notifyMe, bool isApproved, int approvedBy)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_AddComment", articleID, createdDate, userID, comment, GetNull(remoteAddress), type, GetNull(trackbackUrl), GetNull(trackbackTitle), GetNull(trackbackBlogName), GetNull(trackbackExcerpt), GetNull(anonymousName), GetNull(anonymousEmail), GetNull(anonymousURL), notifyMe, isApproved, GetNull(approvedBy)));
        }

        public override void UpdateComment(int commentID, int articleID, int userID, string comment, string remoteAddress, int type, string trackbackUrl, string trackbackTitle, string trackbackBlogName, string trackbackExcerpt, string anonymousName, string anonymousEmail, string anonymousURL, bool notifyMe, bool isApproved, int approvedBy)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_UpdateComment", commentID, articleID, userID, comment, GetNull(remoteAddress), type, GetNull(trackbackUrl), GetNull(trackbackTitle), GetNull(trackbackBlogName), GetNull(trackbackExcerpt), GetNull(anonymousName), GetNull(anonymousEmail), GetNull(anonymousURL), notifyMe, isApproved, GetNull(approvedBy));
        }

        #endregion

        #region " Custom Field Methods "
        public override IDataReader GetCustomField(int customFieldID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetCustomField", customFieldID);
        }
        public override IDataReader GetCustomFieldList(int moduleID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetCustomFieldList", moduleID);
        }

        public override void DeleteCustomField(int customFieldID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_DeleteCustomField", customFieldID);
        }

        public override int AddCustomField(int moduleID, string name, int fieldType, string fieldElements, string defaultValue, string caption, string captionHelp, bool isRequired, bool isVisible, int sortOrder, int validationType, int length, string regularExpression)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_AddCustomField", moduleID, name, fieldType, GetNull(fieldElements), GetNull(defaultValue), GetNull(caption), GetNull(captionHelp), isRequired, isVisible, sortOrder, validationType, length, GetNull(regularExpression)));
        }

        public override void UpdateCustomField(int customFieldID, int moduleID, string name, int fieldType, string fieldElements, string defaultValue, string caption, string captionHelp, bool isRequired, bool isVisible, int sortOrder, int validationType, int length, string regularExpression)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_UpdateCustomField", customFieldID, moduleID, name, fieldType, GetNull(fieldElements), GetNull(defaultValue), GetNull(caption), GetNull(captionHelp), isRequired, isVisible, sortOrder, validationType, length, GetNull(regularExpression));
        }

        #endregion

        #region " Custom Value Methods "

        public override IDataReader GetCustomValueList(int articleID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetCustomValueList", articleID);
        }

        public override void DeleteCustomValue(int articleID, int customFieldID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_DeleteCustomValue", articleID, customFieldID);
        }

        public override int AddCustomValue(int articleID, int customFieldID, string customValue)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_AddCustomValue", articleID, customFieldID, customValue));
        }

        public override void UpdateCustomValue(int customValueID, int articleID, int customFieldID, string customValue)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_UpdateCustomValue", customValueID, articleID, customFieldID, customValue);
        }
        #endregion

        #region " Feed Methods "

        public override int AddFeed(int moduleID, string title, string url, int userID, bool autoFeature, bool isActive, int dateMode, int autoExpire, int autoExpireUnit)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_FeedAdd", moduleID, title, url, userID, autoFeature, isActive, dateMode, GetNull(autoExpire), GetNull(autoExpireUnit)));
        }
        public override void UpdateFeed(int feedID, int moduleID, string title, string url, int userID, bool autoFeature, bool isActive, int dateMode, int autoExpire, int autoExpireUnit)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_FeedUpdate", feedID, moduleID, title, url, userID, autoFeature, isActive, dateMode, GetNull(autoExpire), GetNull(autoExpireUnit));
        }

        public override void DeleteFeed(int feedID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_FeedDelete", feedID);
        }

        public override IDataReader GetFeed(int feedID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_FeedGet", feedID);
        }

        public override IDataReader GetFeedList(int moduleID, bool showActiveOnly)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_FeedList", moduleID, showActiveOnly);
        }

        public override void AddFeedCategory(int feedID, int categoryID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_FeedCategoryAdd", feedID, categoryID);
        }

        public override IDataReader GetFeedCategoryList(int feedID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_FeedCategoryList", feedID);
        }

        public override void DeleteFeedCategory(int feedID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_FeedCategoryDelete", feedID);
        }

        #endregion

        #region " File Methods "

        public override int AddFile(int articleID, string title, string fileName, string extension, int size, string contentType, string folder, int sortOrder, string fileGuid)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_FileAdd", GetNull(articleID), GetNull(title), GetNull(fileName), GetNull(extension), GetNull(size), GetNull(contentType), GetNull(folder), sortOrder, GetNull(fileGuid)));
        }

        public override void UpdateFile(int fileID, int articleID, string title, string fileName, string extension, int size, string contentType, string folder, int sortOrder, string fileGuid)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_FileUpdate", fileID, GetNull(articleID), GetNull(title), GetNull(fileName), GetNull(extension), GetNull(size), GetNull(contentType), GetNull(folder), sortOrder, GetNull(fileGuid));
        }

        public override void DeleteFile(int fileID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_FileDelete", fileID);
        }

        public override IDataReader GetFile(int fileID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_FileGet", fileID);
        }

        public override IDataReader GetFileList(int articleID, string fileGuid)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_FileList", GetNull(articleID), GetNull(fileGuid));
        }


        #endregion

        #region " Image Methods "

        public override int AddImage(int articleID, string title, string fileName, string extension, int size, int width, int height, string contentType, string folder, int sortOrder, string imageGuid, string description)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_ImageAdd", GetNull(articleID), GetNull(title), GetNull(fileName), GetNull(extension), GetNull(size), GetNull(width), GetNull(height), GetNull(contentType), GetNull(folder), sortOrder, GetNull(imageGuid), GetNull(description)));
        }

        public override void UpdateImage(int imageID, int articleID, string title, string fileName, string extension, int size, int width, int height, string contentType, string folder, int sortOrder, string imageGuid, string description)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_ImageUpdate", imageID, GetNull(articleID), GetNull(title), GetNull(fileName), GetNull(extension), GetNull(size), GetNull(width), GetNull(height), GetNull(contentType), GetNull(folder), sortOrder, GetNull(imageGuid), GetNull(description));
        }

        public override void DeleteImage(int imageID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_ImageDelete", imageID);
        }

        public override IDataReader GetImage(int imageID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_ImageGet", imageID);
        }

        public override IDataReader GetImageList(int articleID, string imageGuid)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_ImageList", GetNull(articleID), GetNull(imageGuid));
        }

        #endregion

        #region "镜像文章 Mirror Article Methods "

        public override void AddMirrorArticle(int articleID, int linkedArticleID, int linkedPortalID, bool autoUpdate)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_AddMirrorArticle", articleID, linkedArticleID, linkedPortalID, autoUpdate);
        }

        public override IDataReader GetMirrorArticle(int articleID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetMirrorArticle", articleID);
        }

        public override IDataReader GetMirrorArticleList(int linkedArticleID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetMirrorArticleList", linkedArticleID);
        }

        #endregion

        #region " Page Methods "

        public override IDataReader GetPageList(int articleID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetPageList", articleID);
        }

        public override IDataReader GetPage(int pageID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetPage", pageID);
        }

        public override void DeletePage(int pageID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_DeletePage", pageID);
        }

        public override int AddPage(int articleID, string title, string pageText, int sortOrder)
        {
            return  Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_AddPage", articleID, title, pageText, sortOrder));
        }

        public override void UpdatePage(int pageID, int articleID, string title, string pageText, int sortOrder)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_UpdatePage", pageID, articleID, title, pageText, sortOrder);
        }

        #endregion

        #region " EmailTemplate Methods "

        public override IDataReader GetEmailTemplate(int templateID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_EmailTemplateGet", templateID);
        }

        public override IDataReader GetEmailTemplateByName(int moduleID, string name)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_EmailTemplateGetByName", moduleID, name);
        }

        public override IDataReader ListEmailTemplate(int moduleID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_EmailTemplateList", moduleID);
        }

        public override int AddEmailTemplate(int moduleID, string name, string subject, string template)
        {
            return  Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_EmailTemplateAdd", moduleID, name, subject, template));
        }

        public override void UpdateEmailTemplate(int templateID, int moduleID, string name, string subject, string template)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_EmailTemplateUpdate", templateID, moduleID, name, subject, template);
        }
        public override void DeleteEmailTemplate(int templateID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_EmailTemplateDelete", templateID);
        }

        #endregion

        #region " Rating Methods "

        public override int AddRating(int articleID, int userID, DateTime createdDate, double rating)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_RatingAdd", articleID, userID, createdDate, GetNull(rating)));
        }

        public override IDataReader GetRating(int articleID, int userID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_RatingGet", articleID, userID);
        }

        public override IDataReader GetRatingByID(int ratingID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_RatingGetByID", ratingID);
        }

        public override void DeleteRating(int ratingID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_RatingDelete", ratingID);
        }

        #endregion

        #region "讲义 Handout Methods 可能就是文章说明标签"

        public override int AddHandout(int moduleID, int userID, string name, string description)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_AddHandout", moduleID, userID, name, GetNull(description)));
        }

        public override void AddHandoutArticle(int handoutID, int articleID, int sortOrder)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_AddHandoutArticle", handoutID, articleID, sortOrder);
        }

        public override void DeleteHandout(int handoutID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_DeleteHandout", handoutID);
        }

        public override void DeleteHandoutArticleList(int handoutID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_DeleteHandoutArticleList", handoutID);
        }

        public override IDataReader GetHandout(int handoutID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetHandout", handoutID);
        }

        public override IDataReader GetHandoutList(int userID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetHandoutList", userID);
        }

        public override IDataReader GetHandoutArticleList(int handoutID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_GetHandoutArticleList", handoutID);
        }

        public override void UpdateHandout(int handoutID, int moduleID, int userID, string name, string description)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_UpdateHandout", handoutID, moduleID, userID, name, GetNull(description));
        }



        #endregion

        #region " Tag Methods "

        public override IDataReader GetTag(int tagID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_TagGet", tagID);
        }

        public override IDataReader GetTagByName(int moduleID, string nameLowered)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_TagGetByName", moduleID, nameLowered);
        }

        public override IDataReader ListTag(int moduleID, int maxCount)
        {
            return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_TagList", moduleID, GetNull(maxCount));
        }

        public override int AddTag(int moduleID, string name, string nameLowered)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_TagAdd", moduleID, name, nameLowered));
        }

        public override void UpdateTag(int tagID, int moduleID, string name, string nameLowered, int usages)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_TagUpdate", tagID, moduleID, name, nameLowered, usages);
        }

        public override void DeleteTag(int tagID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_TagDelete", tagID);
        }

        public override void AddArticleTag(int articleID, int tagID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_ArticleTagAdd", articleID, tagID);
        }

        public override void DeleteArticleTag(int articleID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_ArticleTagDelete", articleID);
        }

        public override void DeleteArticleTagByTag(int tagID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "GcDesign_NewsArticles_ArticleTagDeleteByTag", tagID);
        }


        #endregion


        #endregion

    }
}