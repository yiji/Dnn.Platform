using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Search;
using System.Xml;
using System.Security.Cryptography;
using System.Data;
using System.Collections;
using System.Text;
using DotNetNuke.Services.Search.Entities;

namespace GcDesign.NewsArticles
{
    public class ArticleController : ModuleSearchBase, IPortable
    {
        #region " Private Methods "

        private List<ArticleInfo> FillArticleCollection(IDataReader dr, ref int totalRecords, int maxCount)
        {
            List<ArticleInfo> objArticles = new List<ArticleInfo>();

            while (dr.Read())
            {
                objArticles.Add(CBO.FillObject<ArticleInfo>(dr,false));
            }

            bool nextResult = dr.NextResult();
            totalRecords = 0;

            if (dr.Read())
            {
                totalRecords = Convert.ToInt32(Null.SetNull(dr["TotalRecords"], totalRecords));
                if (maxCount != Null.NullInteger && maxCount < totalRecords)
                {
                    totalRecords = maxCount;
                }
            }

            if (dr != null)
            {
                dr.Close();
            }

            return objArticles;


        }

        private Hashtable GetTabModuleSettings(int TabModuleId, Hashtable settings)
        {

            IDataReader dr = DotNetNuke.Data.DataProvider.Instance().GetTabModuleSettings(TabModuleId);

            while (dr.Read())
            {

                if (!dr.IsDBNull(1))
                {
                    settings[dr.GetString(0)] = dr.GetString(1);
                }
                else
                {
                    settings[dr.GetString(0)] = "";
                }

            }

            dr.Close();

            return settings;

        }
        #endregion

        #region " Static Methods "

        public static void ClearArchiveCache(int moduleID)
        {
            List<string> itemsToRemove = new List<string>();

            if (HttpContext.Current != null)
            {
                IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Key.ToString().ToLower().Contains("gcdesign-newsarticles-archive-" + moduleID.ToString()))
                    {
                        itemsToRemove.Add(enumerator.Key.ToString());
                    }
                }

                foreach (string itemToRemove in itemsToRemove)
                {
                    DataCache.RemoveCache(itemToRemove.Replace("DNN_", ""));
                }
            }
        }

        public static void ClearArticleCache(int articleID)
        {
            string cacheKey = "gcdesign-newsarticles-article-" + articleID.ToString();
            DataCache.RemoveCache(cacheKey);
        }

        #endregion

        #region " Public Methods "

        public List<ArticleInfo> GetArticleList(int moduleID)
        {
            return GetArticleList(moduleID, true);
        }

        public List<ArticleInfo> GetArticleList(int moduleID, bool isApproved)
        {
            int totalRecords = -1;
            return GetArticleList(moduleID, Null.NullDate, Null.NullDate, null, Null.NullBoolean, null, Null.NullInteger, Null.NullInteger, Null.NullInteger, "CreatedDate", "DESC", isApproved, Null.NullBoolean, Null.NullString, Null.NullInteger, Null.NullBoolean, Null.NullBoolean, Null.NullBoolean, Null.NullBoolean, Null.NullBoolean, Null.NullBoolean, Null.NullString, null, false, Null.NullString, Null.NullInteger, Null.NullString, Null.NullString, ref totalRecords);

        }
        public List<ArticleInfo> GetArticleList(int moduleID, bool isApproved, string sort)
        {
            int totalRecords = -1;
            return GetArticleList(moduleID, Null.NullDate, Null.NullDate, null, Null.NullBoolean, null, Null.NullInteger, Null.NullInteger, Null.NullInteger, sort, "DESC", isApproved, Null.NullBoolean, Null.NullString, Null.NullInteger, Null.NullBoolean, Null.NullBoolean, Null.NullBoolean, Null.NullBoolean, Null.NullBoolean, Null.NullBoolean, Null.NullString, null, false, Null.NullString, Null.NullInteger, Null.NullString, Null.NullString, ref totalRecords);
        }

        public List<ArticleInfo> GetArticleList(int moduleID, DateTime currentDate, DateTime agedDate, int[] categoryID, bool matchAll, int maxCount, int pageNumber, int pageSize, string sortBy, string sortDirection, bool isApproved, bool isDraft, string keywords, int authorID, bool showPending, bool showExpired, bool showFeaturedOnly, bool showNotFeaturedOnly, bool showSecuredOnly, bool showNotSecuredOnly, string articleIDs, ref int totalRecords)
        {
            return GetArticleList(moduleID, currentDate, agedDate, categoryID, matchAll, null, maxCount, pageNumber, pageSize, sortBy, sortDirection, isApproved, isDraft, keywords, authorID, showPending, showExpired, showFeaturedOnly, showNotFeaturedOnly, showSecuredOnly, showNotSecuredOnly, articleIDs, null, false, Null.NullString, Null.NullInteger, Null.NullString, Null.NullString, ref totalRecords);
        }

        public List<ArticleInfo> GetArticleList(int moduleID, DateTime currentDate, DateTime agedDate, int[] categoryID, bool matchAll, int[] categoryIDExclude, int maxCount, int pageNumber, int pageSize, string sortBy, string sortDirection, bool isApproved, bool isDraft, string keywords, int authorID, bool showPending, bool showExpired, bool showFeaturedOnly, bool showNotFeaturedOnly, bool showSecuredOnly, bool showNotSecuredOnly, string articleIDs, int[] tagID, bool matchAllTag, string rssGuid, int customFieldID, string customValue, string linkFilter, ref int totalRecords)
        {
            return FillArticleCollection(DataProvider.Instance().GetArticleListBySearchCriteria(moduleID, currentDate, agedDate, categoryID, matchAll, categoryIDExclude, maxCount, pageNumber, pageSize, sortBy, sortDirection, isApproved, isDraft, keywords, authorID, showPending, showExpired, showFeaturedOnly, showNotFeaturedOnly, showSecuredOnly, showNotSecuredOnly, articleIDs, tagID, matchAllTag, rssGuid, customFieldID, customValue, linkFilter), ref totalRecords, maxCount);
        }

        public List<ArticleInfo> GetSearchArticleList1(int moduleID, DateTime endDate, DateTime startDate, int[] categoryID, bool matchAll,int conditionMatchCount, int[] categoryIDExclude, int maxCount, int pageNumber, int pageSize, string sortBy, string sortDirection, bool isApproved, bool isDraft, string keywords, int authorID, bool showPending, bool timePeriod, bool showFeaturedOnly, bool showNotFeaturedOnly, bool showSecuredOnly, bool showNotSecuredOnly, string articleIDs, int[] tagID, bool matchAllTag, string rssGuid, int customFieldID, string customValue, string linkFilter, ref int totalRecords)
        {
            return FillArticleCollection(DataProvider.Instance().GetArticleListBySearchCriteria1(moduleID, endDate, startDate, categoryID, matchAll, conditionMatchCount, categoryIDExclude, maxCount, pageNumber, pageSize, sortBy, sortDirection, isApproved, isDraft, keywords, authorID, showPending, timePeriod, showFeaturedOnly, showNotFeaturedOnly, showSecuredOnly, showNotSecuredOnly, articleIDs, tagID, matchAllTag, rssGuid, customFieldID, customValue, linkFilter), ref totalRecords, maxCount);
        }

        public ArticleInfo GetArticle(int articleID)
        {
            string cacheKey = "gcdesign-newsarticles-article-" + articleID.ToString();
            ArticleInfo objArticle = (ArticleInfo)DataCache.GetCache(cacheKey);
            if (objArticle == null)
            {
                objArticle = (ArticleInfo)CBO.FillObject<ArticleInfo>(DataProvider.Instance().GetArticle(articleID));
                if (objArticle == null)
                {
                    return null;
                }
                DataCache.SetCache(cacheKey, objArticle);
            }
            return objArticle;
        }

        /// <summary>
        /// 获取文章所在的栏目列表 直接包含栏目不包括父类
        /// </summary>
        /// <param name="articleID"></param>
        /// <returns></returns>
        public ArrayList GetArticleCategories(int articleID)
        {

            ArrayList objArticleCategories = (ArrayList)DataCache.GetCache(ArticleConstants.CACHE_CATEGORY_ARTICLE + articleID.ToString());

            if (objArticleCategories == null)
            {
                objArticleCategories = CBO.FillCollection(DataProvider.Instance().GetArticleCategories(articleID), typeof(CategoryInfo));
                DataCache.SetCache(ArticleConstants.CACHE_CATEGORY_ARTICLE + articleID.ToString(), objArticleCategories);
            }
            return objArticleCategories;

        }

        public List<ArchiveInfo> GetNewsArchive(int moduleID, int[] categoryID, int[] categoryIDExclude, int authorID, GroupByType groupBy, bool showPending)
        {
            string categories = Null.NullString;
            if (categoryID != null)
            {
                foreach (int category in categoryID)
                {
                    if (categories != Null.NullString)
                    {
                        categories = categories + ",";
                    }
                    categories = categories + category.ToString();
                }
            }
            string categoriesExclude = Null.NullString;

            if (categoryIDExclude != null)
            {
                foreach (int category in categoryIDExclude)
                {
                    if (categoriesExclude != Null.NullString)
                    {
                        categoriesExclude = categoriesExclude + ",";
                    }
                    categoriesExclude = categoriesExclude + category.ToString();
                }
            }

            string hashCategories = "";
            if (categories != "" || categoriesExclude != "")
            {
                UnicodeEncoding Ue = new UnicodeEncoding();
                byte[] ByteSourceText = Ue.GetBytes(categories + categoriesExclude);
                MD5CryptoServiceProvider Md5 = new MD5CryptoServiceProvider();
                byte[] ByteHash = Md5.ComputeHash(ByteSourceText);
                hashCategories = Convert.ToBase64String(ByteHash);
            }

            string cacheKey = "gcdesign-newsarticles-archive-" + moduleID.ToString() + "-" + hashCategories + "-" + authorID.ToString() + "-" + groupBy.ToString() + "-" + showPending.ToString();

            List<ArchiveInfo> objArchives = (List<ArchiveInfo>)DataCache.GetCache(cacheKey);
            if (objArchives == null)
            {
                objArchives = CBO.FillCollection<ArchiveInfo>(DataProvider.Instance().GetNewsArchive(moduleID, categoryID, categoryIDExclude, authorID, groupBy.ToString(), showPending));
                DataCache.SetCache(cacheKey, objArchives);
            }

            return objArchives;

        }

        public void DeleteArticle(int articleID)
        {

            ArticleInfo objArticle = GetArticle(articleID);

            if (objArticle != null)
            {
                DeleteArticle(articleID, objArticle.ModuleID);
            }

        }

        public void DeleteArticle(int articleID, int moduleID)
        {

            DataProvider.Instance().DeleteArticle(articleID);
            CategoryController.ClearCache(moduleID);
            AuthorController.ClearCache(moduleID);
            ArticleController.ClearArchiveCache(moduleID);
            ArticleController.ClearArticleCache(articleID);

        }

        public void DeleteArticleCategories(int articleID)
        {

            DataProvider.Instance().DeleteArticleCategories(articleID);
            DataCache.RemoveCache(ArticleConstants.CACHE_CATEGORY_ARTICLE + articleID.ToString());

        }

        public int AddArticle(ArticleInfo objArticle)
        {

            int articleID = (int)DataProvider.Instance().AddArticle(objArticle.AuthorID, objArticle.ApproverID, objArticle.CreatedDate, objArticle.LastUpdate, objArticle.Title, objArticle.Summary, objArticle.IsApproved, objArticle.NumberOfViews, objArticle.IsDraft, objArticle.StartDate, objArticle.EndDate, objArticle.ModuleID, objArticle.ImageUrl, objArticle.IsFeatured, objArticle.LastUpdateID, objArticle.Url, objArticle.IsSecure, objArticle.IsNewWindow, objArticle.MetaTitle, objArticle.MetaDescription, objArticle.MetaKeywords, objArticle.PageHeadText, objArticle.ShortUrl, objArticle.RssGuid);

            CategoryController.ClearCache(objArticle.ModuleID);
            AuthorController.ClearCache(objArticle.ModuleID);
            ArticleController.ClearArchiveCache(objArticle.ModuleID);

            return articleID;

        }

        public void AddArticleCategory(int articleID, int categoryID)
        {

            DataProvider.Instance().AddArticleCategory(articleID, categoryID);
            DataCache.RemoveCache(ArticleConstants.CACHE_CATEGORY_ARTICLE + articleID.ToString());

        }

        public void UpdateArticle(ArticleInfo objArticle)
        {

            DataProvider.Instance().UpdateArticle(objArticle.ArticleID, objArticle.AuthorID, objArticle.ApproverID, objArticle.CreatedDate, objArticle.LastUpdate, objArticle.Title, objArticle.Summary, objArticle.IsApproved, objArticle.NumberOfViews, objArticle.IsDraft, objArticle.StartDate, objArticle.EndDate, objArticle.ModuleID, objArticle.ImageUrl, objArticle.IsFeatured, objArticle.LastUpdateID, objArticle.Url, objArticle.IsSecure, objArticle.IsNewWindow, objArticle.MetaTitle, objArticle.MetaDescription, objArticle.MetaKeywords, objArticle.PageHeadText, objArticle.ShortUrl, objArticle.RssGuid);

            CategoryController.ClearCache(objArticle.ModuleID);
            AuthorController.ClearCache(objArticle.ModuleID);
            ArticleController.ClearArchiveCache(objArticle.ModuleID);
            ArticleController.ClearArticleCache(objArticle.ArticleID);

        }

        public void UpdateArticleCount(int articleID, int count)
        {

            DataProvider.Instance().UpdateArticleCount(articleID, count);

        }

        public bool SecureCheck(int portalID, int articleID, int userID)
        {

            return DataProvider.Instance().SecureCheck(portalID, articleID, userID);

        }

        #endregion


        #region " Optional Interfaces "

        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo modInfo, DateTime beginDateUtc)
        {

            bool doSearch = false;

            if (modInfo.ModuleSettings.Contains(ArticleConstants.ENABLE_CORE_SEARCH_SETTING))
            {
                doSearch = Convert.ToBoolean(modInfo.ModuleSettings[ArticleConstants.ENABLE_CORE_SEARCH_SETTING]);
            }

            var searchDocuments = new List<SearchDocument>();

            if (doSearch)
            {

                List<ArticleInfo> objArticles = GetArticleList(modInfo.ModuleID);

                foreach (ArticleInfo objArticle in objArticles)
                {

                    if (objArticle.IsApproved)
                    {

                        PageController objPageController = new PageController();
                        ArrayList objPages = objPageController.GetPageList(objArticle.ArticleID);
                        for (int i = 0; i < objPages.Count; i++)
                        {
                            PageInfo objPage = (PageInfo)objPages[i];
                            string pageContent = HttpUtility.HtmlDecode(objArticle.Title) + " " + System.Web.HttpUtility.HtmlDecode(objPage.PageText);

                            foreach (DictionaryEntry Item in objArticle.CustomList)
                            {
                                if (Item.Value.ToString() != "")
                                {
                                    pageContent = pageContent + "\r\n" + Item.Value.ToString();
                                }
                            }

                            string pageDescription = HtmlUtils.Shorten(HtmlUtils.Clean(System.Web.HttpUtility.HtmlDecode(objPage.PageText), false), 100, "...");

                            string title = objArticle.Title + " - " + objPage.Title;
                            if (objArticle.Title == objPage.Title)
                            {
                                title = objArticle.Title;
                            }
                                var searchDoc = new SearchDocument
                                {
                                    UniqueKey = modInfo.ModuleID.ToString(),
                                    PortalId = modInfo.PortalID,
                                    Title = title,
                                    Description = pageDescription,
                                    Body = pageContent,
                                    ModifiedTimeUtc =objArticle.LastUpdate
                                };

                            searchDocuments.Add(searchDoc);
                        }

                    }


                }

            }

            return searchDocuments;
        }

        //public SearchItemInfoCollection GetSearchItems(ModuleInfo ModInfo)
        //{

        //    ModuleController objModuleController = new ModuleController();
        //    Hashtable settings = objModuleController.GetModuleSettings(ModInfo.ModuleID);
        //    settings = GetTabModuleSettings(ModInfo.TabModuleID, settings);

        //    bool doSearch = false;

        //    if (settings.Contains(ArticleConstants.ENABLE_CORE_SEARCH_SETTING))
        //    {
        //        doSearch = Convert.ToBoolean(settings[ArticleConstants.ENABLE_CORE_SEARCH_SETTING]);
        //    }

        //    SearchItemInfoCollection SearchItemCollection = new SearchItemInfoCollection();

        //    if (doSearch)
        //    {

        //        List<ArticleInfo> objArticles = GetArticleList(ModInfo.ModuleID);

        //        foreach (ArticleInfo objArticle in objArticles)
        //        {

        //            if (objArticle.IsApproved)
        //            {

        //                PageController objPageController = new PageController();
        //                ArrayList objPages = objPageController.GetPageList(objArticle.ArticleID);
        //                for (int i = 0; i < objPages.Count; i++)
        //                {
        //                    SearchItemInfo SearchItem;
        //                    PageInfo objPage = (PageInfo)objPages[i];
        //                    string pageContent = HttpUtility.HtmlDecode(objArticle.Title) + " " + System.Web.HttpUtility.HtmlDecode(objPage.PageText);

        //                    foreach (DictionaryEntry Item in objArticle.CustomList)
        //                    {
        //                        if (Item.Value.ToString() != "")
        //                        {
        //                            pageContent = pageContent + "\r\n" + Item.Value.ToString();
        //                        }
        //                    }

        //                    string pageDescription = HtmlUtils.Shorten(HtmlUtils.Clean(System.Web.HttpUtility.HtmlDecode(objPage.PageText), false), 100, "...");

        //                    string title = objArticle.Title + " - " + objPage.Title;
        //                    if (objArticle.Title == objPage.Title)
        //                    {
        //                        title = objArticle.Title;
        //                    }
        //                    if (i == 0)
        //                    {
        //                        SearchItem = new SearchItemInfo(title, pageDescription, objArticle.AuthorID, objArticle.LastUpdate, ModInfo.ModuleID, ModInfo.ModuleID.ToString() + "_" + objArticle.ArticleID.ToString() + "_" + objPage.PageID.ToString(), pageContent, "ArticleType=ArticleView&ArticleID=" + objArticle.ArticleID.ToString());
        //                    }
        //                    else
        //                    {
        //                        SearchItem = new SearchItemInfo(title, pageDescription, objArticle.AuthorID, objArticle.LastUpdate, ModInfo.ModuleID, ModInfo.ModuleID.ToString() + "_" + objArticle.ArticleID.ToString() + "_" + objPage.PageID.ToString(), pageContent, "ArticleType=ArticleView&ArticleID=" + objArticle.ArticleID.ToString() + "&PageID=" + objPage.PageID.ToString());
        //                    }
        //                    SearchItemCollection.Add(SearchItem);
        //                }

        //            }


        //        }

        //    }

        //    return SearchItemCollection;

        //}


        public string ExportModule(int ModuleID)
        {

            string strXML = "";
            strXML += WriteCategories(ModuleID, Null.NullInteger);
            strXML += WriteTags(ModuleID);
            strXML += WriteArticles(ModuleID);
            return strXML;

        }

        public void ImportModule(int ModuleID, string Content, string Version, int UserId)
        {

            XmlDocument objXmlDocument = new XmlDocument();
            objXmlDocument.LoadXml("<xml>" + Content + "</xml>");

            foreach (XmlNode xmlChildNode in objXmlDocument.ChildNodes[0].ChildNodes)
            {
                if (xmlChildNode.Name == "categories")
                {
                    int sortOrder = 0;
                    foreach (XmlNode xmlCategory in xmlChildNode.ChildNodes)
                    {
                        ReadCategory(ModuleID, xmlCategory, Null.NullInteger, sortOrder);
                        sortOrder = sortOrder + 1;
                    }
                }

                if (xmlChildNode.Name == "tags")
                {
                    foreach (XmlNode xmlTag in xmlChildNode.ChildNodes)
                    {
                        ReadTag(ModuleID, xmlTag);
                    }
                }

                if (xmlChildNode.Name == "articles")
                {
                    foreach (XmlNode xmlArticle in xmlChildNode.ChildNodes)
                    {
                        ReadArticle(ModuleID, xmlArticle);
                    }
                }
            }

        }

        private string WriteCategories(int ModuleID, int parentID)
        {

            string strXML = "";

            CategoryController objCategoryController = new CategoryController();
            List<CategoryInfo> objCategories = objCategoryController.GetCategories(ModuleID, parentID);

            if (objCategories.Count > 0)
            {
                strXML += "<categories>";
                foreach (CategoryInfo objCategory in objCategories)
                {
                    strXML += "<category>";
                    strXML += "<name>" + XmlUtils.XMLEncode(objCategory.Name) + "</name>";
                    strXML += "<description>" + XmlUtils.XMLEncode(objCategory.Description) + "</description>";
                    strXML += "<metaTitle>" + XmlUtils.XMLEncode(objCategory.MetaTitle) + "</metaTitle>";
                    strXML += "<metaDescription>" + XmlUtils.XMLEncode(objCategory.MetaDescription) + "</metaDescription>";
                    strXML += "<metaKeywords>" + XmlUtils.XMLEncode(objCategory.MetaKeywords) + "</metaKeywords>";
                    strXML += WriteCategories(ModuleID, objCategory.CategoryID);
                    strXML += "</category>";
                }
                strXML += "</categories>";
            }

            return strXML;

        }

        public void ReadCategory(int ModuleID, XmlNode xmlCategory, int parentCategoryID, int sortOrder){

            CategoryInfo objCategory =new CategoryInfo();
            objCategory.ParentID = parentCategoryID;
            objCategory.ModuleID = ModuleID;
            objCategory.Name = xmlCategory["name"].InnerText;
            objCategory.Description = xmlCategory["description"].InnerText;

            objCategory.InheritSecurity = true;
            objCategory.CategorySecurityType = CategorySecurityType.Loose;

            objCategory.MetaTitle = xmlCategory["metaTitle"].InnerText;
            objCategory.MetaDescription = xmlCategory["metaDescription"].InnerText;
            objCategory.MetaKeywords = xmlCategory["metaKeywords"].InnerText;

            objCategory.Image = Null.NullString;
            objCategory.SortOrder = sortOrder;

            CategoryController objCategoryController =new CategoryController();
            objCategory.CategoryID = objCategoryController.AddCategory(objCategory);

            int childSortOrder = 0;
            foreach(XmlNode xmlChildNode in xmlCategory.ChildNodes){
                if (xmlChildNode.Name.ToLower() == "categories") {
                    foreach(XmlNode xmlChildCategory in xmlChildNode.ChildNodes){
                        ReadCategory(ModuleID, xmlChildCategory, objCategory.CategoryID, childSortOrder);
                    }
                }
                childSortOrder = childSortOrder + 1;
            }

        }

        private string WriteTags(int ModuleID)
        {

            string strXML = "";

            TagController objTagController = new TagController();
            ArrayList objTags = objTagController.List(ModuleID, Null.NullInteger);

            if (objTags.Count > 0)
            {
                strXML += "<tags>";
                foreach (TagInfo objTag in objTags)
                {
                    strXML += "<tag>";
                    strXML += "<name>" + XmlUtils.XMLEncode(objTag.Name) + "</name>";
                    strXML += "<usage>" + XmlUtils.XMLEncode(objTag.Usages.ToString()) + "</usage>";
                    strXML += "</tag>";
                }
                strXML += "</tags>";
            }

            return strXML;

        }

        public void ReadTag(int ModuleID, XmlNode xmlTag)
        {

            TagInfo objTag = new TagInfo();

            objTag.ModuleID = ModuleID;
            objTag.Name = xmlTag["name"].InnerText;
            objTag.NameLowered = objTag.Name.ToLower();
            objTag.Usages = Convert.ToInt32(xmlTag["usage"].InnerText);

            TagController objTagController = new TagController();
            objTagController.Add(objTag);

        }

        private string WriteArticles(int ModuleID) {

            string strXML = "";

            ArticleController objArticleController = new ArticleController();
            int totalRecords=0;
            List<ArticleInfo> objArticles = objArticleController.GetArticleList(ModuleID, DateTime.Now, Null.NullDate, null, true, 10000, 1, 10000, "CreatedDate", "DESC", true, false, Null.NullString, Null.NullInteger, true, true, false, false, false, false, Null.NullString, ref totalRecords);

            if( objArticles.Count > 0 ){
                strXML += "<articles>";
               foreach(ArticleInfo objArticle in objArticles){
                    strXML += "<article>";
                    strXML += "<createdDate>" + XmlUtils.XMLEncode(objArticle.CreatedDate.ToString("O")) + "</createdDate>";
                    strXML += "<lastUpdate>" + XmlUtils.XMLEncode(objArticle.LastUpdate.ToString("O")) + "</lastUpdate>";
                    strXML += "<title>" + XmlUtils.XMLEncode(objArticle.Title) + "</title>";
                    strXML += "<isApproved>" + XmlUtils.XMLEncode(objArticle.IsApproved.ToString()) + "</isApproved>";
                    strXML += "<numberOfViews>" + XmlUtils.XMLEncode(objArticle.NumberOfViews.ToString()) + "</numberOfViews>";
                    strXML += "<isDraft>" + XmlUtils.XMLEncode(objArticle.IsDraft.ToString()) + "</isDraft>";
                    strXML += "<startDate>" + XmlUtils.XMLEncode(objArticle.StartDate.ToString("O")) + "</startDate>";
                    strXML += "<endDate>" + XmlUtils.XMLEncode(objArticle.EndDate.ToString("O")) + "</endDate>";
                    strXML += "<imageUrl>" + XmlUtils.XMLEncode(objArticle.ImageUrl.ToString()) + "</imageUrl>";
                    strXML += "<isFeatured>" + XmlUtils.XMLEncode(objArticle.IsFeatured.ToString()) + "</isFeatured>";
                    strXML += "<url>" + XmlUtils.XMLEncode(objArticle.Url) + "</url>";
                    strXML += "<isSecure>" + XmlUtils.XMLEncode(objArticle.IsSecure.ToString()) + "</isSecure>";
                    strXML += "<isNewWindow>" + XmlUtils.XMLEncode(objArticle.IsNewWindow.ToString()) + "</isNewWindow>";
                    strXML += "<commentCount>" + XmlUtils.XMLEncode(objArticle.CommentCount.ToString()) + "</commentCount>";
                    strXML += "<pageCount>" + XmlUtils.XMLEncode(objArticle.PageCount.ToString()) + "</pageCount>";
                    strXML += "<fileCount>" + XmlUtils.XMLEncode(objArticle.FileCount.ToString()) + "</fileCount>";
                    strXML += "<imageCount>" + XmlUtils.XMLEncode(objArticle.ImageCount.ToString()) + "</imageCount>";
                    if (objArticle.Rating != Null.NullDouble) {
                        strXML += "<rating>" + XmlUtils.XMLEncode(objArticle.Rating.ToString()) + "</rating>";
                    }
                    strXML += "<ratingCount>" + XmlUtils.XMLEncode(objArticle.RatingCount.ToString()) + "</ratingCount>";
                    strXML += "<summary>" + XmlUtils.XMLEncode(objArticle.Url) + "</summary>";
                    strXML += "<metaTitle>" + XmlUtils.XMLEncode(objArticle.MetaTitle) + "</metaTitle>";
                    strXML += "<metaDescription>" + XmlUtils.XMLEncode(objArticle.MetaDescription) + "</metaDescription>";
                    strXML += "<metaKeywords>" + XmlUtils.XMLEncode(objArticle.MetaKeywords) + "</metaKeywords>";
                    strXML += "<pageHeadText>" + XmlUtils.XMLEncode(objArticle.PageHeadText) + "</pageHeadText>";
                    strXML += "<shortUrl>" + XmlUtils.XMLEncode(objArticle.ShortUrl) + "</shortUrl>";

                    ArrayList objArticleCategories = objArticleController.GetArticleCategories(objArticle.ArticleID);
                    if (objArticleCategories.Count > 0) {
                        strXML += "<categories>";
                        foreach(CategoryInfo objCategory in objArticleCategories){
                            strXML += "<category>";
                            strXML += "<name>" + XmlUtils.XMLEncode(objCategory.Name) + "</name>";
                            strXML += "</category>";
                        }
                        strXML += "</categories>";
                    }

                    if (objArticle.Tags != "") {
                        strXML += "<tags>";
                        foreach(string tag in objArticle.Tags.Split(',')){
                            strXML += "<tag>";
                            strXML += "<name>" + XmlUtils.XMLEncode(tag) + "</name>";
                            strXML += "</tag>";
                        }
                        strXML += "</tags>";
                    }

                    if (objArticle.PageCount > 0) {
                        PageController objPageController =new PageController();
                        ArrayList objPages = objPageController.GetPageList(objArticle.ArticleID);

                        if (objPages.Count > 0) {
                            strXML += "<pages>";
                            foreach(PageInfo objPage in objPages){
                                strXML += "<page>";
                                strXML += "<title>" + XmlUtils.XMLEncode(objPage.Title) + "</title>";
                                strXML += "<pageText>" + XmlUtils.XMLEncode(objPage.PageText) + "</pageText>";
                                strXML += "</page>";
                            }
                            strXML += "</pages>";
                        }
                    }

                    if (objArticle.ImageCount > 0) {
                        ImageController objImageController =new ImageController();
                        List<ImageInfo> objImages = objImageController.GetImageList(objArticle.ArticleID, Null.NullString);

                        if (objImages.Count > 0) {
                            strXML += "<images>";
                            foreach(ImageInfo objImage in objImages){
                                strXML += "<image>";
                                strXML += "<title>" + XmlUtils.XMLEncode(objImage.Title) + "</title>";
                                strXML += "<filename>" + XmlUtils.XMLEncode(objImage.FileName) + "</filename>";
                                strXML += "<extension>" + XmlUtils.XMLEncode(objImage.Extension) + "</extension>";
                                strXML += "<size>" + XmlUtils.XMLEncode(objImage.Size.ToString()) + "</size>";
                                strXML += "<width>" + XmlUtils.XMLEncode(objImage.Width.ToString()) + "</width>";
                                strXML += "<height>" + XmlUtils.XMLEncode(objImage.Height.ToString()) + "</height>";
                                strXML += "<contentType>" + XmlUtils.XMLEncode(objImage.ContentType) + "</contentType>";
                                strXML += "<folder>" + XmlUtils.XMLEncode(objImage.Folder) + "</folder>";
                                strXML += "</image>";
                            }
                            strXML += "</images>";
                        }
                    }

                    if (objArticle.FileCount > 0) {
                        FileController objFileController =new FileController();
                        List<FileInfo> objFiles = objFileController.GetFileList(objArticle.ArticleID, Null.NullString);

                        if (objFiles.Count > 0) {
                            strXML += "<files>";
                            foreach(FileInfo objFile  in objFiles){
                                strXML += "<file>";
                                strXML += "<title>" + XmlUtils.XMLEncode(objFile.Title) + "</title>";
                                strXML += "<filename>" + XmlUtils.XMLEncode(objFile.FileName) + "</filename>";
                                strXML += "<extension>" + XmlUtils.XMLEncode(objFile.Extension) + "</extension>";
                                strXML += "<size>" + XmlUtils.XMLEncode(objFile.Size.ToString()) + "</size>";
                                strXML += "<contentType>" + XmlUtils.XMLEncode(objFile.ContentType) + "</contentType>";
                                strXML += "<folder>" + XmlUtils.XMLEncode(objFile.Folder) + "</folder>";
                                strXML += "</file>";
                        }
                            strXML += "</files>";
                    }
                    }

                    if (objArticle.CommentCount > 0) {
                        CommentController objCommentController=new CommentController();
                        List<CommentInfo> objComments = objCommentController.GetCommentList(ModuleID, objArticle.ArticleID, true);

                        if (objComments.Count > 0) {
                            strXML += "<comments>";
                            foreach(CommentInfo objComment in objComments){
                                strXML += "<comment>";
                                strXML += "<createdDate>" + XmlUtils.XMLEncode(objArticle.CreatedDate.ToString("O")) + "</createdDate>";
                                strXML += "<commentText>" + XmlUtils.XMLEncode(objComment.Comment) + "</commentText>";
                                strXML += "<remoteAddress>" + XmlUtils.XMLEncode(objComment.RemoteAddress) + "</remoteAddress>";
                                strXML += "<type>" + XmlUtils.XMLEncode(objComment.Type.ToString()) + "</type>";
                                strXML += "<trackbackUrl>" + XmlUtils.XMLEncode(objComment.TrackbackUrl) + "</trackbackUrl>";
                                strXML += "<trackbackTitle>" + XmlUtils.XMLEncode(objComment.TrackbackTitle) + "</trackbackTitle>";
                                strXML += "<trackbackBlogName>" + XmlUtils.XMLEncode(objComment.TrackbackBlogName) + "</trackbackBlogName>";
                                strXML += "<trackbackExcerpt>" + XmlUtils.XMLEncode(objComment.TrackbackExcerpt) + "</trackbackExcerpt>";
                                strXML += "<anonymousName>" + XmlUtils.XMLEncode(objComment.AnonymousName) + "</anonymousName>";
                                strXML += "<anonymousEmail>" + XmlUtils.XMLEncode(objComment.AnonymousEmail) + "</anonymousEmail>";
                                strXML += "<anonymousUrl>" + XmlUtils.XMLEncode(objComment.AnonymousURL) + "</anonymousUrl>";
                                strXML += "<notifyMe>" + XmlUtils.XMLEncode(objComment.NotifyMe.ToString()) + "</notifyMe>";
                                strXML += "</comment>";
                            }
                            strXML += "</comments>";
                        }
                    }

                    strXML += "</article>";
               }
                strXML += "</articles>";
            }

            return strXML;

        }

        public void ReadArticle(int ModuleID, XmlNode xmlArticle){

            ArticleInfo objArticle =new ArticleInfo();

            objArticle.ModuleID = ModuleID;

            objArticle.CreatedDate = DateTime.Parse(xmlArticle["createdDate"].InnerText);
            objArticle.LastUpdate = DateTime.Parse(xmlArticle["lastUpdate"].InnerText);

            objArticle.Title = xmlArticle["title"].InnerText;
            objArticle.IsApproved = Convert.ToBoolean(xmlArticle["isApproved"].InnerText);
            objArticle.NumberOfViews = Convert.ToInt32(xmlArticle["numberOfViews"].InnerText);
            objArticle.IsDraft = Convert.ToBoolean(xmlArticle["isDraft"].InnerText);

            objArticle.StartDate = DateTime.Parse(xmlArticle["startDate"].InnerText);
            objArticle.EndDate = DateTime.Parse(xmlArticle["endDate"].InnerText);

            objArticle.ImageUrl = xmlArticle["imageUrl"].InnerText;
            objArticle.IsFeatured = Convert.ToBoolean(xmlArticle["isFeatured"].InnerText);
            objArticle.Url = xmlArticle["url"].InnerText;
            objArticle.IsSecure = Convert.ToBoolean(xmlArticle["isSecure"].InnerText);
            objArticle.IsNewWindow = Convert.ToBoolean(xmlArticle["isNewWindow"].InnerText);

            objArticle.CommentCount = Convert.ToInt32(xmlArticle["commentCount"].InnerText);
            objArticle.PageCount = Convert.ToInt32(xmlArticle["pageCount"].InnerText);
            objArticle.FileCount = Convert.ToInt32(xmlArticle["fileCount"].InnerText);
            objArticle.ImageCount = Convert.ToInt32(xmlArticle["imageCount"].InnerText);

            objArticle.Rating = Null.NullInteger;
            objArticle.RatingCount = 0;
            objArticle.Summary = xmlArticle["summary"].InnerText;

            objArticle.MetaTitle = xmlArticle["metaTitle"].InnerText;
            objArticle.MetaDescription = xmlArticle["metaDescription"].InnerText;
            objArticle.MetaKeywords = xmlArticle["metaKeywords"].InnerText;
            objArticle.PageHeadText = xmlArticle["pageHeadText"].InnerText;
            objArticle.ShortUrl = xmlArticle["shortUrl"].InnerText;

            ArticleController objArticleController =new ArticleController();
            objArticle.ArticleID = objArticleController.AddArticle(objArticle);

            CategoryController objCategoryController =new CategoryController();
            List<CategoryInfo> objCategories = objCategoryController.GetCategoriesAll(ModuleID, Null.NullInteger);
            foreach(XmlNode xmlChildNode  in xmlArticle.ChildNodes){

                if (xmlChildNode.Name == "categories") {
                    foreach(XmlNode xmlCategoryNode in xmlChildNode.ChildNodes){
                        string name = xmlCategoryNode["name"].InnerText;
                        foreach(CategoryInfo objCategory in objCategories){
                            if (objCategory.Name.ToLower() == name.ToLower()) {
                                objArticleController.AddArticleCategory(objArticle.ArticleID, objCategory.CategoryID);
                                break;
                            }
                            }
                            }
                            }

                if (xmlChildNode.Name == "tags") {
                    foreach(XmlNode xmlTagNode in xmlChildNode.ChildNodes){
                        string name = xmlTagNode["name"].InnerText;
                        TagController objTagController = new TagController();
                        ArrayList objTags  = objTagController.List(ModuleID, Null.NullInteger);
                        foreach(TagInfo objTag in objTags){
                            if (objTag.Name.ToLower() == name.ToLower()) {
                                objTagController.Add(objArticle.ArticleID, objTag.TagID);
                               break;
                            }
                            }
                            }
                }

                if (xmlChildNode.Name == "images") {
                    int sortOrder = 0;
                    foreach(XmlNode xmlImageNode in xmlChildNode.ChildNodes){
                        ImageInfo objImage =new ImageInfo();

                        objImage.ArticleID = objArticle.ArticleID;
                        objImage.Title = xmlImageNode["title"].InnerText;
                        objImage.FileName = xmlImageNode["filename"].InnerText;
                        objImage.Extension = xmlImageNode["extension"].InnerText;
                        objImage.Size = Convert.ToInt32(xmlImageNode["size"].InnerText);
                        objImage.Width = Convert.ToInt32(xmlImageNode["width"].InnerText);
                        objImage.Height = Convert.ToInt32(xmlImageNode["height"].InnerText);
                        objImage.ContentType = xmlImageNode["contentType"].InnerText;
                        objImage.Folder = xmlImageNode["folder"].InnerText;
                        objImage.SortOrder = sortOrder;

                        ImageController objImageController =new ImageController();
                        objImageController.Add(objImage);

                        sortOrder = sortOrder + 1;
                    }
                    if (sortOrder > 0) {
                        objArticle.ImageCount = sortOrder;
                        objArticleController.UpdateArticle(objArticle);
                    }
                }

                if (xmlChildNode.Name == "files") {
                    int sortOrder  = 0;
                    foreach(XmlNode xmlImageNode  in xmlChildNode.ChildNodes){
                        FileInfo objFile=new FileInfo();

                        objFile.ArticleID = objArticle.ArticleID;
                        objFile.Title = xmlImageNode["title"].InnerText;
                        objFile.FileName = xmlImageNode["filename"].InnerText;
                        objFile.Extension = xmlImageNode["extension"].InnerText;
                        objFile.Size = Convert.ToInt32(xmlImageNode["size"].InnerText);
                        objFile.ContentType = xmlImageNode["contentType"].InnerText;
                        objFile.Folder = xmlImageNode["folder"].InnerText;
                        objFile.SortOrder = sortOrder;

                        FileController objFileController =new FileController();
                        objFileController.Add(objFile);

                        sortOrder = sortOrder + 1;
                    }
                    if (sortOrder > 0) {
                        objArticle.FileCount = sortOrder;
                        objArticleController.UpdateArticle(objArticle);
                    }
                }

                if (xmlChildNode.Name == "pages") {
                    int sortOrder = 0;
                    foreach(XmlNode xmlImageNode in xmlChildNode.ChildNodes){

                        PageInfo objPage = new PageInfo();

                        objPage.ArticleID = objArticle.ArticleID;
                        objPage.Title = xmlImageNode["title"].InnerText;
                        objPage.PageText = xmlImageNode["pageText"].InnerText;
                        objPage.SortOrder = sortOrder;

                        PageController objPageController =new PageController();
                        objPageController.AddPage(objPage);

                        sortOrder = sortOrder + 1;
                    }
                    if (sortOrder > 0) {
                        objArticle.PageCount = sortOrder;
                        objArticleController.UpdateArticle(objArticle);
                    }
                }

                if (xmlChildNode.Name == "comments") {
                    int sortOrder = 0;
                    foreach(XmlNode xmlImageNode in xmlChildNode.ChildNodes){

                        CommentInfo objComment =new CommentInfo();

                        objComment.UserID = Null.NullInteger;
                        objComment.ArticleID = objArticle.ArticleID;
                        objComment.CreatedDate = DateTime.Parse(xmlImageNode["createdDate"].InnerText);
                        objComment.Comment = xmlImageNode["commentText"].InnerText;
                        objComment.RemoteAddress = xmlImageNode["remoteAddress"].InnerText;
                        objComment.Type = Convert.ToInt32(xmlImageNode["type"].InnerText);
                        objComment.TrackbackUrl = xmlImageNode["trackbackUrl"].InnerText;
                        objComment.TrackbackTitle = xmlImageNode["trackbackTitle"].InnerText;
                        objComment.TrackbackBlogName = xmlImageNode["trackbackBlogName"].InnerText;
                        objComment.TrackbackExcerpt = xmlImageNode["trackbackExcerpt"].InnerText;
                        objComment.AnonymousName = xmlImageNode["anonymousName"].InnerText;
                        objComment.AnonymousEmail = xmlImageNode["anonymousEmail"].InnerText;
                        objComment.AnonymousURL = xmlImageNode["anonymousUrl"].InnerText;
                        objComment.NotifyMe = Convert.ToBoolean(xmlImageNode["notifyMe"].InnerText);
                        objComment.IsApproved = true;
                        objComment.ApprovedBy = Null.NullInteger;

                        CommentController objCommentController =new CommentController();
                        objCommentController.AddComment(objComment);

                        sortOrder = sortOrder + 1;
                    }
                    if (sortOrder > 0) {
                        objArticle.CommentCount = sortOrder;
                        objArticleController.UpdateArticle(objArticle);
                    }
                }
            }

                }

        #endregion

    }
}