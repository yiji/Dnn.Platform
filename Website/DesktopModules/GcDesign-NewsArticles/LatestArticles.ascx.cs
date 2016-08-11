using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Security;
using GcDesign.NewsArticles.Components.CustomFields;

using GcDesign.NewsArticles.Base;
using System.Collections;
using System.Web.UI.HtmlControls;

namespace GcDesign.NewsArticles
{
    public partial class LatestArticles : NewsArticleModuleBase
    {
        #region " Controls "

        protected System.Web.UI.WebControls.Repeater rptLatestArticles;
        protected System.Web.UI.WebControls.DataList dlLatestArticles;
        protected System.Web.UI.WebControls.Label lblNotConfigured;

        #endregion

        #region " private Members "

        private LayoutController _objLayoutController;

        private LayoutInfo _objLayoutHeader;
        private LayoutInfo _objLayoutItem;
        private LayoutInfo _objLayoutFooter;

        private int _articleTabID = Null.NullInteger;
        private DotNetNuke.Entities.Tabs.TabInfo _articleTabInfo;
        private int _articleModuleID = Null.NullInteger;
        private string _rssLink = Null.NullString;
        private LayoutModeType _layoutMode;

        private int _pageNumber = 1;

        private int _serverTimeZone = Null.NullInteger;
        private ArticleSettings _articleSettings;

        private Hashtable _settings;

        #endregion

        #region " private Properties "

        public new ArticleSettings ArticleSettings
        {
            get
            {
                if (_articleSettings == null)
                {

                    _settings = DotNetNuke.Entities.Portals.PortalSettings.GetModuleSettings(_articleModuleID);

                    ModuleController objModuleController = new ModuleController();
                    ModuleInfo objModule = objModuleController.GetModule(_articleModuleID, _articleTabID);
                    if (objModule != null)
                    {
                        Hashtable objSettings = DotNetNuke.Entities.Portals.PortalSettings.GetTabModuleSettings(objModule.TabModuleID);

                        foreach (string key in objSettings.Keys)
                        {
                            if (!_settings.ContainsKey(key))
                            {
                                _settings.Add(key, objSettings[key]);
                            }
                        }
                    }

                    _articleSettings = new ArticleSettings(_settings, this.PortalSettings, this.ModuleConfiguration);

                }
                return _articleSettings;
            }
        }

        private DotNetNuke.Entities.Tabs.TabInfo ArticleTabInfo
        {
            get
            {
                if (_articleTabInfo == null)
                {
                    DotNetNuke.Entities.Tabs.TabController objTabController = new DotNetNuke.Entities.Tabs.TabController();
                    _articleTabInfo = objTabController.GetTab(_articleTabID, this.PortalId, false);
                }

                return _articleTabInfo;
            }
        }

        private int LatestAuthorID
        {
            get
            {
                int id = Null.NullInteger;
                if (Request["laauth-" + TabModuleId.ToString()] != null)
                {
                    if (Numeric.IsNumeric(Request["laauth-" + TabModuleId.ToString()]))
                    {
                        id = Convert.ToInt32(Request["laauth-" + TabModuleId.ToString()]);
                    }
                }

                return id;
            }
        }

        private int CategoryID
        {
            get
            {
                int id = Null.NullInteger;
                if (Request["lacat-" + TabModuleId.ToString()] != null)
                {
                    if (Numeric.IsNumeric(Request["lacat-" + TabModuleId.ToString()]))
                    {
                        id = Convert.ToInt32(Request["lacat-" + TabModuleId.ToString()]);
                    }
                }

                return id;
            }
        }

        private new int ServerTimeZone
        {
            get
            {
                if (_serverTimeZone == Null.NullInteger)
                {

                    _serverTimeZone = PortalSettings.TimeZoneOffset;

                    ModuleController objModuleSettingController = new ModuleController();
                    Hashtable newsSettings = objModuleSettingController.GetModuleSettings(_articleModuleID);

                    if (newsSettings != null)
                    {
                        if (newsSettings.Contains(ArticleConstants.SERVER_TIMEZONE))
                        {
                            if (Numeric.IsNumeric(newsSettings[ArticleConstants.SERVER_TIMEZONE].ToString()))
                            {
                                _serverTimeZone = Convert.ToInt32(newsSettings[ArticleConstants.SERVER_TIMEZONE].ToString());
                            }
                        }
                    }
                }

                return _serverTimeZone;
            }
        }

        private string SortBy
        {
            get
            {
                string sort = ArticleConstants.DEFAULT_SORT_BY;
                if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_SORT_BY))
                {
                    sort = Settings[ArticleConstants.LATEST_ARTICLES_SORT_BY].ToString();
                }
                if (Request["lasort-" + TabModuleId.ToString()] != null)
                {
                    switch (Request["lasort-" + TabModuleId.ToString()].ToLower())
                    {
                        case "publishdate":
                            sort = "StartDate";
                            break;
                        case "expirydate":
                            sort = "EndDate";
                            break;
                        case "lastupdate":
                            sort = "LastUpdate";
                            break;
                        case "rating":
                            sort = "Rating DESC, RatingCount";
                            break;
                        case "commentcount":
                            sort = "CommentCount";
                            break;
                        case "numberofviews":
                            sort = "NumberOfViews";
                            break;
                        case "random":
                            sort = "NewID()";
                            break;
                        case "title":
                            sort = "Title";
                            break;
                    }
                }

                return sort;
            }
        }

        private int StartPoint
        {
            get
            {
                if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_START_POINT))
                {
                    return Convert.ToInt32(Settings[ArticleConstants.LATEST_ARTICLES_START_POINT].ToString());
                }
                else
                {
                    return 0;
                }
            }
        }

        private string Time
        {
            get
            {
                string val = "";

                if (Request["latime-" + TabModuleId.ToString()] != null)
                {
                    val = Request["latime-" + TabModuleId.ToString()];
                }

                return val;
            }
        }

        #endregion

        #region " private Methods "

        private void BindArticles()
        {

            ArticleController objArticleController = new ArticleController();

            _rssLink = @"~/DesktopModules/GcDesign-NewsArticles/Rss.aspx?TabID=" + _articleTabID.ToString() + "&amp;ModuleID=" + _articleModuleID.ToString();

            int[] cats = null;

            bool showRelated = false;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_SHOW_RELATED))
            {
                showRelated = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_SHOW_RELATED].ToString());
            }

            if (showRelated && Request["CategoryID"] != null)
            {
                if (Numeric.IsNumeric(Request["CategoryID"]))
                {
                    List<int> categories = new List<int>();
                    categories.Add(Convert.ToInt32(Request["CategoryID"]));
                    cats = categories.ToArray();
                    _rssLink = _rssLink + "&CategoryID=" + Request["CategoryID"];
                }
                else
                {
                    showRelated = false;
                }
            }

            if (showRelated && (ArticleSettings.UrlModeType == Components.Types.UrlModeType.Classic) && Request["ArticleID"] != null)
            {
                if (Numeric.IsNumeric(Request["ArticleID"]))
                {
                    ArrayList categories = objArticleController.GetArticleCategories(Convert.ToInt32(Request["ArticleID"]));

                    string categoriesRSS = "";
                    List<int> categoriesList = new List<int>();
                    foreach (CategoryInfo objCategory in categories)
                    {
                        categoriesList.Add(objCategory.CategoryID);
                        if (categoriesRSS == "")
                        {
                            categoriesRSS = objCategory.CategoryID.ToString();
                        }
                        else
                        {
                            categoriesRSS = categoriesRSS + "," + objCategory.CategoryID.ToString();
                        }
                    }
                    cats = categoriesList.ToArray();
                    if (categoriesList.Count > 0)
                    {
                        _rssLink = _rssLink + "&CategoryID=" + categoriesRSS;
                    }
                }
            }

            if (showRelated && (ArticleSettings.UrlModeType == Components.Types.UrlModeType.Shorterned) && Request[ArticleSettings.ShortenedID] != null)
            {
                if (Numeric.IsNumeric(Request[ArticleSettings.ShortenedID]))
                {
                    ArrayList categories = objArticleController.GetArticleCategories(Convert.ToInt32(Request[ArticleSettings.ShortenedID]));

                    string categoriesRSS = "";
                    List<int> categoriesList = new List<int>();
                    foreach (CategoryInfo objCategory in categories)
                    {
                        categoriesList.Add(objCategory.CategoryID);
                        if (categoriesRSS == "")
                        {
                            categoriesRSS = objCategory.CategoryID.ToString();
                        }
                        else
                        {
                            categoriesRSS = categoriesRSS + "," + objCategory.CategoryID.ToString();
                        }
                    }
                    cats = categoriesList.ToArray();
                    if (categoriesList.Count > 0)
                    {
                        _rssLink = _rssLink + "&CategoryID=" + categoriesRSS;
                    }
                }
            }

            int[] catsExclude = null;

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_CATEGORIES_EXCLUDE))
            {
                if (Settings[ArticleConstants.LATEST_ARTICLES_CATEGORIES_EXCLUDE].ToString() != Null.NullString && Settings[ArticleConstants.LATEST_ARTICLES_CATEGORIES_EXCLUDE].ToString() != "-1")
                {
                    string[] categories = Settings[ArticleConstants.LATEST_ARTICLES_CATEGORIES_EXCLUDE].ToString().Split(',');

                    int[] categoriesToExclude = new int[categories.Length];
                    for (int i = 0; i < categories.Length; i++)
                    {
                        categoriesToExclude[i] = Convert.ToInt32(categories[i]);
                    }

                    if (categories.Length > 0)
                    {
                        _rssLink = _rssLink + "&CategoryIDExclude=" + Settings[ArticleConstants.LATEST_ARTICLES_CATEGORIES_EXCLUDE].ToString();
                    }

                    catsExclude = categoriesToExclude;
                }
            }

            CategoryController objCategoryController = new CategoryController();
            List<CategoryInfo> objCategories = objCategoryController.GetCategoriesAll(_articleModuleID, Null.NullInteger, ArticleSettings.CategorySortType);

            List<int> excludeCategoriesRestrictive = new List<int>();

            foreach (CategoryInfo objCategory in objCategories)
            {
                if (!objCategory.InheritSecurity && objCategory.CategorySecurityType == CategorySecurityType.Restrict)
                {
                    if (Request.IsAuthenticated)
                    {
                        if (_settings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                        {
                            if (PortalSecurity.IsInRoles(_settings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString()) == false)
                            {
                                excludeCategoriesRestrictive.Add(objCategory.CategoryID);
                            }
                        }
                    }
                    else
                    {
                        excludeCategoriesRestrictive.Add(objCategory.CategoryID);
                    }
                }
            }

            if (catsExclude == null)
            {
                if (excludeCategoriesRestrictive.Count > 0)
                {
                    catsExclude = excludeCategoriesRestrictive.ToArray();
                }
            }

            List<int> excludeCategories = new List<int>();

            if (catsExclude == null)
            {

                foreach (CategoryInfo objCategory in objCategories)
                {
                    if (!objCategory.InheritSecurity)
                    {
                        if (Request.IsAuthenticated)
                        {
                            if (_settings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                            {
                                if (!PortalSecurity.IsInRoles(_settings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString()))
                                {
                                    excludeCategories.Add(objCategory.CategoryID);
                                }
                            }
                        }
                        else
                        {
                            excludeCategories.Add(objCategory.CategoryID);
                        }
                    }
                }

            }

            if (!showRelated)
            {
                if (CategoryID == Null.NullInteger)
                {
                    if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_CATEGORIES))
                    {
                        if (Settings[ArticleConstants.LATEST_ARTICLES_CATEGORIES].ToString() != Null.NullString && Settings[ArticleConstants.LATEST_ARTICLES_CATEGORIES].ToString() != "-1")
                        {
                            string[] categories = Settings[ArticleConstants.LATEST_ARTICLES_CATEGORIES].ToString().Split(',');

                            int[] categoriesToDisplay = new int[categories.Length];
                            for (int i = 0; i < categories.Length; i++)
                            {
                                categoriesToDisplay[i] = Convert.ToInt32(categories[i]);
                            }

                            if (categories.Length > 0)
                            {
                                _rssLink = _rssLink + "&CategoryID=" + Settings[ArticleConstants.LATEST_ARTICLES_CATEGORIES].ToString();
                            }

                            cats = categoriesToDisplay;
                        }
                        else
                        {

                            if (excludeCategories.Count > 0)
                            {
                                List<int> includeCategories = new List<int>();

                                foreach (CategoryInfo objCategoryToInclude in objCategories)
                                {

                                    bool includeCategory = true;

                                    foreach (int exclCategory in excludeCategories)
                                    {
                                        if (exclCategory == objCategoryToInclude.CategoryID)
                                        {
                                            includeCategory = false;
                                        }
                                    }

                                    if (includeCategory)
                                    {
                                        includeCategories.Add(objCategoryToInclude.CategoryID);
                                    }

                                }

                                cats = includeCategories.ToArray();

                            }

                        }
                    }
                    else
                    {

                        if (excludeCategories.Count > 0)
                        {
                            List<int> includeCategories = new List<int>();

                            foreach (CategoryInfo objCategoryToInclude in objCategories)
                            {

                                bool includeCategory = true;

                                foreach (int exclCategory in excludeCategories)
                                {
                                    if (exclCategory == objCategoryToInclude.CategoryID)
                                    {
                                        includeCategory = false;
                                    }
                                }

                                if (includeCategory)
                                {
                                    includeCategories.Add(objCategoryToInclude.CategoryID);
                                }

                            }

                            if (includeCategories.Count > 0)
                            {
                                includeCategories.Add(-1);
                            }

                            cats = includeCategories.ToArray();

                        }

                    }
                }
                else
                {
                    int[] categoriesToDisplay = new int[1];
                    categoriesToDisplay[0] = CategoryID;
                    cats = categoriesToDisplay;
                    _rssLink = _rssLink + "&CategoryID=" + CategoryID.ToString();
                }
            }

            MatchOperatorType matchOperator = MatchOperatorType.MatchAny;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_MATCH_OPERATOR))
            {
                matchOperator = (MatchOperatorType)System.Enum.Parse(typeof(MatchOperatorType), Settings[ArticleConstants.LATEST_ARTICLES_MATCH_OPERATOR].ToString());
            }

            bool matchAll = false;
            if (matchOperator == MatchOperatorType.MatchAll)
            {
                matchAll = true;
                _rssLink = _rssLink + "&MatchCat=1";
            }

            int count = 10;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_COUNT))
            {
                count = Convert.ToInt32(Settings[ArticleConstants.LATEST_ARTICLES_COUNT].ToString());
            }

            _rssLink = _rssLink + "&MaxCount=" + count.ToString();

            int maxAge = Null.NullInteger;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_MAX_AGE))
            {
                if (Settings[ArticleConstants.LATEST_ARTICLES_MAX_AGE].ToString() != "")
                {
                    maxAge = Convert.ToInt32(Settings[ArticleConstants.LATEST_ARTICLES_MAX_AGE].ToString()) * -1;
                }
            }

            bool featuredOnly = false;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_FEATURED_ONLY))
            {
                featuredOnly = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_FEATURED_ONLY].ToString());
            }

            if (featuredOnly)
            {
                _rssLink = _rssLink + "&FeaturedOnly=" + featuredOnly.ToString();
            }

            bool notFeaturedOnly = false;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_NOT_FEATURED_ONLY))
            {
                notFeaturedOnly = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_NOT_FEATURED_ONLY].ToString());
            }

            if (notFeaturedOnly)
            {
                _rssLink = _rssLink + "&NotFeaturedOnly=" + notFeaturedOnly.ToString();
            }

            bool securedOnly = false;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_SECURED_ONLY))
            {
                securedOnly = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_SECURED_ONLY].ToString());
            }

            if (securedOnly)
            {
                _rssLink = _rssLink + "&SecuredOnly=" + securedOnly.ToString();
            }

            bool notSecuredOnly = false;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_NOT_SECURED_ONLY))
            {
                notSecuredOnly = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_NOT_SECURED_ONLY].ToString());
            }

            if (notFeaturedOnly)
            {
                _rssLink = _rssLink + "&NotSecuredOnly=" + notSecuredOnly.ToString();
            }

            string sort = SortBy;
            _rssLink = _rssLink + "&sortBy=" + sort;

            string sortDirection = ArticleConstants.DEFAULT_SORT_DIRECTION;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_SORT_DIRECTION))
            {
                sortDirection = Settings[ArticleConstants.LATEST_ARTICLES_SORT_DIRECTION].ToString();
            }
            if (sort == "Title" && Request["lasort-" + TabModuleId.ToString()] != null)
            {
                sortDirection = "ASC";
            }
            if (sort != "Title" && Request["lasort-" + TabModuleId.ToString()] != null)
            {
                sortDirection = "DESC";
            }
            _rssLink = _rssLink + "&sortDirection=" + sortDirection;

            _layoutMode = ArticleConstants.LATEST_ARTICLES_LAYOUT_MODE_DEFAULT;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_LAYOUT_MODE))
            {
                _layoutMode = (LayoutModeType)System.Enum.Parse(typeof(LayoutModeType), Settings[ArticleConstants.LATEST_ARTICLES_LAYOUT_MODE].ToString());
            }

            int itemsPerRow = ArticleConstants.LATEST_ARTICLES_ITEMS_PER_ROW_DEFAULT;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_ITEMS_PER_ROW))
            {
                itemsPerRow = Convert.ToInt32(Settings[ArticleConstants.LATEST_ARTICLES_ITEMS_PER_ROW].ToString());
            }

            bool showPending = false;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_SHOW_PENDING))
            {
                showPending = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_SHOW_PENDING].ToString());
            }

            int authorID = ArticleConstants.LATEST_ARTICLES_AUTHOR_DEFAULT;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_AUTHOR))
            {
                authorID = Convert.ToInt32(Settings[ArticleConstants.LATEST_ARTICLES_AUTHOR].ToString());
            }

            string userIDFilter = "";
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_QUERY_STRING_FILTER) && Settings.Contains(ArticleConstants.LATEST_ARTICLES_QUERY_STRING_PARAM))
            {
                if (Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_QUERY_STRING_FILTER].ToString()))
                {
                    authorID = -100;
                    string param = Settings[ArticleConstants.LATEST_ARTICLES_QUERY_STRING_PARAM].ToString();
                    if (param != "")
                    {
                        if (Request.QueryString[param] != null && Numeric.IsNumeric(Request.QueryString[param]))
                        {
                            userIDFilter = param + "=" + Request.QueryString[param];
                            authorID = Convert.ToInt32(Request[param]);
                        }
                    }
                }
            }

            string usernameFilter = "";
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_USERNAME_FILTER) && Settings.Contains(ArticleConstants.LATEST_ARTICLES_USERNAME_PARAM))
            {
                if (Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_USERNAME_FILTER].ToString()))
                {
                    authorID = -100;
                    string param = Settings[ArticleConstants.LATEST_ARTICLES_USERNAME_PARAM].ToString();
                    if (param != "")
                    {
                        if (Request.QueryString[param] != null)
                        {
                            usernameFilter = param + "=" + Request.QueryString[param];
                            DotNetNuke.Entities.Users.UserInfo objUser = DotNetNuke.Entities.Users.UserController.GetUserByName(this.PortalId, Request.QueryString[param]);
                            if (objUser != null)
                            {
                                authorID = objUser.UserID;
                            }
                        }
                    }
                }
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_LOGGED_IN_USER_FILTER))
            {
                if (Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_LOGGED_IN_USER_FILTER].ToString()))
                {
                    authorID = -100;
                    if (Request.IsAuthenticated)
                    {
                        authorID = this.UserId;
                    }
                }
            }

            if (authorID == Null.NullInteger)
            {
                authorID = LatestAuthorID;
            }

            DateTime startDate = DateTime.Now;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_START_DATE))
            {
                if (Settings[ArticleConstants.LATEST_ARTICLES_START_DATE].ToString() != "")
                {
                    startDate = Convert.ToDateTime(Settings[ArticleConstants.LATEST_ARTICLES_START_DATE].ToString());
                }
            }

            bool bubbleFeatured = false;
            if (Settings.Contains(ArticleConstants.BUBBLE_FEATURED_ARTICLES))
            {
                bubbleFeatured = Convert.ToBoolean(Settings[ArticleConstants.BUBBLE_FEATURED_ARTICLES].ToString());
                if (bubbleFeatured)
                {
                    sort = "IsFeatured DESC, " + sort;
                }
            }

            string articleIDs = "";
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_IDS))
            {
                if (Settings[ArticleConstants.LATEST_ARTICLES_IDS].ToString() != "")
                {
                    _rssLink = _rssLink + "&ArticleIDs=" + Settings[ArticleConstants.LATEST_ARTICLES_IDS].ToString();
                    articleIDs = Settings[ArticleConstants.LATEST_ARTICLES_IDS].ToString();
                }
            }

            DateTime agedDate = Null.NullDate;
            if (maxAge != Null.NullInteger)
            {
                if (startDate == Null.NullDate)
                {
                    agedDate = DateTime.Now.AddDays(maxAge);
                }
                else
                {
                    agedDate = startDate.AddDays(maxAge);
                }
            }
            else
            {
                if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_MAX_AGE))
                {
                    if (Settings[ArticleConstants.LATEST_ARTICLES_MAX_AGE].ToString() == "1")
                    {
                        agedDate = startDate.AddDays(-1);
                    }
                }
            }

            if (Time != "")
            {
                if (Time.ToLower() == "today")
                {
                    startDate = DateTime.Now;
                    agedDate = DateTime.Today;
                }
                if (Time.ToLower() == "yesterday")
                {
                    startDate = DateTime.Today;
                    agedDate = DateTime.Today.AddDays(-1);
                }
                if (Time.ToLower() == "threedays")
                {
                    startDate = DateTime.Now;
                    agedDate = DateTime.Today.AddDays(-3);
                }
                if (Time.ToLower() == "sevendays")
                {
                    startDate = DateTime.Now;
                    agedDate = DateTime.Today.AddDays(-7);
                }
                if (Time.ToLower() == "thirtydays")
                {
                    startDate = DateTime.Now;
                    agedDate = DateTime.Today.AddDays(-30);
                }
                if (Time.ToLower() == "ninetydays")
                {
                    startDate = DateTime.Now;
                    agedDate = DateTime.Today.AddDays(-90);
                }
                if (Time.ToLower() == "thisyear")
                {
                    startDate = DateTime.Now;
                    agedDate = DateTime.Today.AddYears(-1);
                }
            }

            int[] tagIDs = null;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_TAGS))
            {
                List<int> objTags = new List<int>();
                TagController objTagController = new TagController();
                string tags = Settings[ArticleConstants.LATEST_ARTICLES_TAGS].ToString();
                if (tags != "")
                {
                    foreach (string tag in tags.Split(','))
                    {
                        objTags.Add(Convert.ToInt32(tag));
                    }
                    if (objTags.Count > 0)
                    {
                        tagIDs = objTags.ToArray();
                        string rssTags = "";
                        foreach (int tagID in tagIDs)
                        {
                            if (rssTags == "")
                            {
                                rssTags = tagID.ToString();
                            }
                            else
                            {
                                rssTags = rssTags + "," + tagID.ToString();
                            }
                        }
                        if (rssTags != "")
                        {
                            _rssLink = _rssLink + "&TagIDs=" + rssTags;
                        }
                    }
                }
            }

            MatchOperatorType matchOperatorTags = MatchOperatorType.MatchAny;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_TAGS_MATCH_OPERATOR))
            {
                matchOperatorTags = (MatchOperatorType)System.Enum.Parse(typeof(MatchOperatorType), Settings[ArticleConstants.LATEST_ARTICLES_TAGS_MATCH_OPERATOR].ToString());
            }

            bool matchAllTags = false;
            if (tagIDs != null)
            {
                if (matchOperatorTags == MatchOperatorType.MatchAll)
                {
                    matchAllTags = true;
                    _rssLink = _rssLink + "&MatchTag=1";
                }
            }

            int customFieldID = Null.NullInteger;
            string customValue = Null.NullString;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_CUSTOM_FIELD_FILTER) && Settings.Contains(ArticleConstants.LATEST_ARTICLES_CUSTOM_FIELD_VALUE))
            {
                if (Numeric.IsNumeric(Settings[ArticleConstants.LATEST_ARTICLES_CUSTOM_FIELD_FILTER].ToString()))
                {
                    customFieldID = Convert.ToInt32(Settings[ArticleConstants.LATEST_ARTICLES_CUSTOM_FIELD_FILTER].ToString());
                }
                customValue = Settings[ArticleConstants.LATEST_ARTICLES_CUSTOM_FIELD_VALUE].ToString();
            }

            string linkFilter = Null.NullString;
            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_LINK_FILTER))
            {
                linkFilter = Settings[ArticleConstants.LATEST_ARTICLES_LINK_FILTER].ToString();
            }

            if (Request["lacust-" + this.TabModuleId.ToString()] != null)
            {
                string val = Request["lacust-" + this.TabModuleId.ToString()];
                if (val.Split('-').Length == 2)
                {
                    if (Numeric.IsNumeric(val.Split('-')[0]))
                    {
                        customFieldID = Convert.ToInt32(val.Split('-')[0]);
                        customValue = val.Split('-')[1];
                    }
                }
            }

            bool doPaging = false;
            if (Settings.Contains(ArticleConstants.LATEST_ENABLE_PAGER))
            {
                doPaging = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ENABLE_PAGER].ToString());
            }

            int pageSize = count;
            if (doPaging)
            {
                if (Settings.Contains(ArticleConstants.LATEST_PAGE_SIZE))
                {
                    pageSize = Convert.ToInt32(Settings[ArticleConstants.LATEST_PAGE_SIZE].ToString());
                }
                else
                {
                    pageSize = ArticleConstants.LATEST_PAGE_SIZE_DEFAULT;
                }
            }

            ModuleController objModuleController = new ModuleController();
            ModuleInfo objModule = objModuleController.GetModule(_articleModuleID, _articleTabID);
            //objLayoutController = new LayoutController(PortalSettings, ArticleSettings, Page, this.IsEditable, _articleTabID, _articleModuleID, this.TabModuleId, this.PortalId, Null.NullInteger, this.UserId, this.ModuleKey)
            _objLayoutController = new LayoutController(PortalSettings, ArticleSettings, objModule, Page);
            LatestLayoutController objLatestLayoutController = new LatestLayoutController();

            _objLayoutHeader = objLatestLayoutController.GetLayout(LatestLayoutType.Listing_Header_Html, ModuleId, Settings);
            _objLayoutItem = objLatestLayoutController.GetLayout(LatestLayoutType.Listing_Item_Html, ModuleId, Settings);
            _objLayoutFooter = objLatestLayoutController.GetLayout(LatestLayoutType.Listing_Footer_Html, ModuleId, Settings);

            int articleCount = 0;
            List<ArticleInfo> objArticles = objArticleController.GetArticleList(_articleModuleID, startDate, agedDate, cats, matchAll, catsExclude, count, _pageNumber, pageSize, sort, sortDirection, true, false, Null.NullString, authorID, showPending, false, featuredOnly, notFeaturedOnly, securedOnly, notSecuredOnly, articleIDs, tagIDs, matchAllTags, Null.NullString, customFieldID, customValue, linkFilter, ref articleCount);

            if (count < articleCount)
            {
                articleCount = count;
            }

            if (_layoutMode == LayoutModeType.Simple)
            {
                rptLatestArticles.DataSource = objArticles;
                rptLatestArticles.DataBind();
                rptLatestArticles.Visible = (objArticles.Count > 0);
            }
            else
            {
                dlLatestArticles.RepeatColumns = itemsPerRow;
                dlLatestArticles.DataSource = objArticles;
                dlLatestArticles.DataBind();
                dlLatestArticles.Visible = (objArticles.Count > 0);
            }

            int pageCount = ((articleCount - 1) / pageSize) + 1;

            if ((objArticles.Count > 0) && pageCount > 1)
            {
                ctlPagingControl.Visible = true;
                ctlPagingControl.PageParam = "lapg-" + this.TabModuleId.ToString();
                List<string> lstparams = new List<string>();
                if (userIDFilter != "")
                {
                    lstparams.Add(userIDFilter);
                }
                if (usernameFilter != "")
                {
                    lstparams.Add(usernameFilter);
                }
                if (Request["lasort-" + TabModuleId.ToString()] != null)
                {
                    lstparams.Add("lasort-" + TabModuleId.ToString() + "=" + Request["lasort-" + TabModuleId.ToString()]);
                }
                if (Request["laauth-" + TabModuleId.ToString()] != null)
                {
                    lstparams.Add("laauth-" + TabModuleId.ToString() + "=" + Request["laauth-" + TabModuleId.ToString()]);
                }
                if (Request["lacat-" + TabModuleId.ToString()] != null)
                {
                    lstparams.Add("lacat-" + TabModuleId.ToString() + "=" + Request["lacat-" + TabModuleId.ToString()]);
                }
                if (Request["lacust-" + TabModuleId.ToString()] != null)
                {
                    lstparams.Add("lacust-" + TabModuleId.ToString() + "=" + Request["lacust-" + TabModuleId.ToString()]);
                }
                if (Request["latime-" + TabModuleId.ToString()] != null)
                {
                    lstparams.Add("latime-" + TabModuleId.ToString() + "=" + Request["latime-" + TabModuleId.ToString()]);
                }
                foreach (string item in lstparams)
                {
                    if (ctlPagingControl.QuerystringParams != "")
                    {
                        ctlPagingControl.QuerystringParams = ctlPagingControl.QuerystringParams + "&" + item;
                    }
                    else
                    {
                        ctlPagingControl.QuerystringParams = item;
                    }
                }
                ctlPagingControl.TotalRecords = articleCount;
                ctlPagingControl.PageSize = pageSize;
                ctlPagingControl.CurrentPage = _pageNumber;
                ctlPagingControl.TabID = TabId;
                ctlPagingControl.EnableViewState = false;
            }

            if (objArticles.Count == 0)
            {
                LayoutInfo objNoArticles = objLatestLayoutController.GetLayout(LatestLayoutType.Listing_Empty_Html, ModuleId, Settings);
                ProcessHeaderFooter(phNoArticles.Controls, objNoArticles.Tokens);
            }

        }

        private bool FindSettings()
        {

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_TAB_ID))
            {
                if (Numeric.IsNumeric(Settings[ArticleConstants.LATEST_ARTICLES_TAB_ID].ToString()))
                {
                    _articleTabID = Convert.ToInt32(Settings[ArticleConstants.LATEST_ARTICLES_TAB_ID].ToString());
                }
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_MODULE_ID))
            {
                if (Numeric.IsNumeric(Settings[ArticleConstants.LATEST_ARTICLES_MODULE_ID].ToString()))
                {
                    _articleModuleID = Convert.ToInt32(Settings[ArticleConstants.LATEST_ARTICLES_MODULE_ID].ToString());
                    if (_articleModuleID != Null.NullInteger)
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        private void ProcessHeaderFooter(ControlCollection controls, string[] layoutArray){

            string moduleKey = "la-" + this.TabModuleId.ToString() + "-Header-";

            Literal objLiteral;
            DropDownList drpTime;

            for(int iPtr = 0 ;iPtr< layoutArray.Length ;iPtr+=2){
                controls.Add(new LiteralControl(layoutArray[iPtr].ToString()));

                if (iPtr < layoutArray.Length - 1) {
                    switch(layoutArray[iPtr + 1]){

                        case "AUTHOR":
                            AuthorController objAuthorController = new AuthorController();
                            DropDownList drpAuthor = new DropDownList();
                            drpAuthor.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                            drpAuthor.DataTextField = "DisplayName";
                            drpAuthor.DataValueField = "UserID";
                            drpAuthor.DataSource = objAuthorController.GetAuthorList(_articleModuleID);
                            drpAuthor.DataBind();
                            drpAuthor.Items.Insert(0, new ListItem(Localization.GetString("SelectAuthor", this.LocalResourceFile), "-1"));
                            drpAuthor.AutoPostBack = true;
                            if (LatestAuthorID != Null.NullInteger) {
                                if (drpAuthor.Items.FindByValue(LatestAuthorID.ToString()) != null) {
                                    drpAuthor.SelectedValue = LatestAuthorID.ToString();
                                }
                            }
                            drpAuthor.SelectedIndexChanged += new System.EventHandler(drpAuthor_SelectedIndexChanged);
                            controls.Add(drpAuthor);
                            break;
                        case "CATEGORY":
                            CategoryController objCategoryController = new CategoryController();
                            DropDownList drpCategory = new DropDownList();
                            drpCategory.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                            drpCategory.DataTextField = "NameIndented";
                            drpCategory.DataValueField = "CategoryID";
                            drpCategory.DataSource = objCategoryController.GetCategoriesAll(_articleModuleID, Null.NullInteger, _articleSettings.CategorySortType);
                            drpCategory.DataBind();
                            drpCategory.Items.Insert(0, new ListItem(Localization.GetString("SelectCategory", this.LocalResourceFile), "-1"));
                            drpCategory.AutoPostBack = true;
                            if (CategoryID != Null.NullInteger) {
                                if (drpCategory.Items.FindByValue(CategoryID.ToString()) != null) {
                                    drpCategory.SelectedValue = CategoryID.ToString();
                                }
                            }
                            drpCategory.SelectedIndexChanged += new System.EventHandler(drpCategory_SelectedIndexChanged);
                            controls.Add(drpCategory);
                            break;
                        case "RSSLINK":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                            objLiteral.Text = this.ResolveUrl(_rssLink);
                            controls.Add(objLiteral);
                            break;
                        case "SORT":
                            DropDownList drpSort = new DropDownList();
                            drpSort.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                            drpSort.Items.Add(new ListItem(Localization.GetString("PublishDate.Text", this.LocalResourceFile), "PublishDate"));
                            drpSort.Items.Add(new ListItem(Localization.GetString("ExpiryDate.Text", this.LocalResourceFile), "ExpiryDate"));
                            drpSort.Items.Add(new ListItem(Localization.GetString("LastUpdate.Text", this.LocalResourceFile), "LastUpdate"));
                            drpSort.Items.Add(new ListItem(Localization.GetString("HighestRated.Text", this.LocalResourceFile), "Rating"));
                            drpSort.Items.Add(new ListItem(Localization.GetString("MostCommented.Text", this.LocalResourceFile), "CommentCount"));
                            drpSort.Items.Add(new ListItem(Localization.GetString("MostViewed.Text", this.LocalResourceFile), "NumberOfViews"));
                            drpSort.Items.Add(new ListItem(Localization.GetString("Random.Text", this.LocalResourceFile), "Random"));
                            drpSort.Items.Add(new ListItem(Localization.GetString("SortTitle.Text", this.LocalResourceFile), "Title"));
                            drpSort.AutoPostBack = true;

                            if (Request["lasort-" + TabModuleId.ToString()] != null)
                            {
                                if (drpSort.Items.FindByValue(Request["lasort-" + TabModuleId.ToString()]) != null) {
                                    drpSort.SelectedValue = Request["lasort-" + TabModuleId.ToString()];
                                }
                            }
                            else
                            {
                                string sort = SortBy;

                                switch (SortBy.ToLower()){
                                    case "startdate":
                                        sort = "PublishDate";
                                        break;
                                    case "enddate":
                                        sort = "ExpiryDate";
                                        break;
                                    case "newid()":
                                        sort = "random";
                                        break;
                                }

                                if (drpSort.Items.FindByValue(sort) != null) {
                                    drpSort.SelectedValue = sort;
                                }
                            }

                            drpSort.SelectedIndexChanged += new System.EventHandler(drpSort_SelectedIndexChanged);
                            controls.Add(drpSort);
                            break;
                        case "TIME":
                             drpTime = new DropDownList();
                            drpTime.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());

                            drpTime.Items.Add(new ListItem(Localization.GetString("Today", this.LocalResourceFile), "Today"));
                            drpTime.Items.Add(new ListItem(Localization.GetString("Yesterday", this.LocalResourceFile), "Yesterday"));
                            drpTime.Items.Add(new ListItem(Localization.GetString("ThreeDays", this.LocalResourceFile), "ThreeDays"));
                            drpTime.Items.Add(new ListItem(Localization.GetString("SevenDays", this.LocalResourceFile), "SevenDays"));
                            drpTime.Items.Add(new ListItem(Localization.GetString("ThirtyDays", this.LocalResourceFile), "ThirtyDays"));
                            drpTime.Items.Add(new ListItem(Localization.GetString("NinetyDays", this.LocalResourceFile), "NinetyDays"));
                            drpTime.Items.Add(new ListItem(Localization.GetString("ThisYear", this.LocalResourceFile), "ThisYear"));
                            drpTime.Items.Add(new ListItem(Localization.GetString("AllTime", this.LocalResourceFile), "AllTime"));
                            drpTime.AutoPostBack = true;

                            if (Time != "") {
                                if (drpTime.Items.FindByValue(Time) != null) {
                                    drpTime.SelectedValue = Time;
                                }
                            }
                            else
                            {
                                drpTime.SelectedValue = "AllTime";
                            }

                            drpTime.SelectedIndexChanged += new System.EventHandler(drpTime_SelectedIndexChanged);

                            controls.Add(drpTime);
                            break;
                        default:

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("CUSTOM:")) {
                                string customField = layoutArray[iPtr + 1].Substring(7, layoutArray[iPtr + 1].Length - 7);

                                CustomFieldController objCustomFieldController = new CustomFieldController();
                                ArrayList objCustomFields = objCustomFieldController.List(_articleModuleID);

                                foreach(CustomFieldInfo objCustomField in objCustomFields){
                                    if (objCustomField.Name.ToLower() == customField.ToLower()) {
                                        if (objCustomField.FieldType == CustomFieldType.DropDownList) {
                                            DropDownList drpCustom = new DropDownList();
                                            drpCustom.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());

                                            foreach(string val in objCustomField.FieldElements.Split('|')){
                                                drpCustom.Items.Add(val);
                                            }

                                            string sel = Localization.GetString("SelectCustom", this.LocalResourceFile);
                                            if (sel.IndexOf("{0}") != -1) {
                                                sel = sel.Replace("{0}", objCustomField.Caption);
                                            }
                                            drpCustom.Items.Insert(0, new ListItem(sel, "-1"));
                                            drpCustom.Attributes.Add("CustomFieldID", objCustomField.CustomFieldID.ToString());
                                            drpCustom.AutoPostBack = true;


                                            if (Request["lacust-" + TabModuleId.ToString()] != null)
                                            {
                                                string val = Request["lacust-" + TabModuleId.ToString()];
                                                if (val.Split('-').Length == 2) {
                                                    if (val.Split('-')[0] == objCustomField.CustomFieldID.ToString()) {
                                                        if (drpCustom.Items.FindByValue(val.Split('-')[1].ToString()) != null) {
                                                            drpCustom.SelectedValue = val.Split('-')[1].ToString();
                                                        }
                                                    }
                                                }
                                            }

                                            drpCustom.SelectedIndexChanged += new System.EventHandler(drpCustom_SelectedIndexChanged);
                                            controls.Add(drpCustom);
                                        }
                                        break;
                                    }
                                }

                                break;

                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("SORT:")) {

                                List<string> lstparams = new List<string>();

                                string sortItem = layoutArray[iPtr + 1].Substring(5, layoutArray[iPtr + 1].Length - 5);
                                string sortValue = sortItem;

                                switch(sortItem.ToLower()){
                                    case "highestrated":
                                        sortValue = "Rating";
                                        break;
                                    case "mostcommented":
                                        sortValue = "CommentCount";
                                        break;
                                    case "mostviewed":
                                        sortValue = "NumberOfViews";
                                        break;
                                    case "sorttitle":
                                        sortValue = "Title";
                                        break;
                                }

                                string sort = SortBy;

                                switch (sort.ToLower()){
                                    case "startdate":
                                        sort = "PublishDate";
                                        break;
                                    case "enddate":
                                        sort = "ExpiryDate";
                                        break;
                                    case "newid()":
                                        sort = "random";
                                        break;
                                    case "rating desc, ratingcount":
                                        sort = "rating";
                                        break;
                                }

                                if (sortValue.ToLower() == sort.ToLower()) {
                                     objLiteral = new Literal();
                                    objLiteral.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                                    objLiteral.Text = Localization.GetString(sortItem + ".Text", this.LocalResourceFile);
                                    controls.Add(objLiteral);
                                }
                                else
                                {
                                    HyperLink objLink = new HyperLink();
                                    objLink.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                                    lstparams.Add("lasort-" + TabModuleId.ToString() + "=" + sortValue);
                                    if (Request["laauth-" + TabModuleId.ToString()] != null)
                                    {
                                        lstparams.Add("laauth-" + TabModuleId.ToString() + "=" + Request["laauth-" + TabModuleId.ToString()]);
                                    }
                                    if (Request["lacat-" + TabModuleId.ToString()] != null)
                                    {
                                        lstparams.Add("lacat-" + TabModuleId.ToString() + "=" + Request["lacat-" + TabModuleId.ToString()]);
                                    }
                                    if (Request["lacust-" + TabModuleId.ToString()] != null)
                                    {
                                        lstparams.Add("lacust-" + TabModuleId.ToString() + "=" + Request["lacust-" + TabModuleId.ToString()]);
                                    }
                                    if (Request["latime-" + TabModuleId.ToString()] != null)
                                    {
                                        lstparams.Add("latime-" + TabModuleId.ToString() + "=" + Request["latime-" + TabModuleId.ToString()]);
                                    }
                                    objLink.NavigateUrl = Globals.NavigateURL(this.TabId, "", lstparams.ToArray());
                                    objLink.Text = Localization.GetString(sortItem + ".Text", this.LocalResourceFile);
                                    controls.Add(objLink);
                                }
                                break;

                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("TIME:")) {

                                string timeItem  = layoutArray[iPtr + 1].Substring(5, layoutArray[iPtr + 1].Length - 5);

                                drpTime = new DropDownList();
                                drpTime.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());

                                drpTime.Items.Add(new ListItem(Localization.GetString("Today", this.LocalResourceFile), "Today"));
                                drpTime.Items.Add(new ListItem(Localization.GetString("Yesterday", this.LocalResourceFile), "Yesterday"));
                                drpTime.Items.Add(new ListItem(Localization.GetString("ThreeDays", this.LocalResourceFile), "ThreeDays"));
                                drpTime.Items.Add(new ListItem(Localization.GetString("SevenDays", this.LocalResourceFile), "SevenDays"));
                                drpTime.Items.Add(new ListItem(Localization.GetString("ThirtyDays", this.LocalResourceFile), "ThirtyDays"));
                                drpTime.Items.Add(new ListItem(Localization.GetString("NinetyDays", this.LocalResourceFile), "NinetyDays"));
                                drpTime.Items.Add(new ListItem(Localization.GetString("ThisYear", this.LocalResourceFile), "ThisYear"));
                                drpTime.Items.Add(new ListItem(Localization.GetString("AllTime", this.LocalResourceFile), "AllTime"));
                                drpTime.AutoPostBack = true;

                                if (Time != "") {
                                    if (drpTime.Items.FindByValue(Time) != null) {
                                        drpTime.SelectedValue = Time;
                                    }
                                }
                                else
                                {  
                                    if (drpTime.Items.FindByValue(timeItem) != null) {
                                        List<string> lstparams = new List<string>();
                                        if (Request["lasort-" + TabModuleId.ToString()] != null)
                                        {
                                            lstparams.Add("lasort-" + TabModuleId.ToString() + "=" + Request["lasort-" + TabModuleId.ToString()]);
                                        }
                                        if (Request["laauth-" + TabModuleId.ToString()] != null)
                                        {
                                            lstparams.Add("laauth-" + TabModuleId.ToString() + "=" + Request["laauth-" + TabModuleId.ToString()]);
                                        }
                                        if (Request["lacat-" + TabModuleId.ToString()] != null)
                                        {
                                            lstparams.Add("lacat-" + TabModuleId.ToString() + "=" + Request["lacat-" + TabModuleId.ToString()]);
                                        }

                                        lstparams.Add("latime-" + TabModuleId.ToString() + "=" + timeItem);
                                        Response.Redirect( Globals.NavigateURL(this.TabId, "", lstparams.ToArray()), true);
                                    }
                                    else
                                    {
                                        drpTime.SelectedValue = "AllTime";
                                    }
                                }

                                drpTime.SelectedIndexChanged += new System.EventHandler(drpTime_SelectedIndexChanged);
                                controls.Add(drpTime);
                                break;
                            }

                            Literal objLiteralOther = new Literal();
                            objLiteralOther.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                            objLiteralOther.Text = "[" + layoutArray[iPtr + 1] + "]";
                            objLiteralOther.EnableViewState = false;
                            controls.Add(objLiteralOther);
                            break;
                    }
                }
            }

        }

        private void ProcessBody(ControlCollection controls, ArticleInfo objArticle, string[] layoutArray)
        {

            if (ArticleTabInfo == null)
            {
                return;
            }

            _objLayoutController.ProcessArticleItem(controls, _objLayoutItem.Tokens, objArticle);

        }

        private void ReadQueryString()
        {

            if (Request.QueryString["lapg-" + this.TabModuleId.ToString()] != null)
            {
                _pageNumber = Convert.ToInt32(Request.QueryString["lapg-" + this.TabModuleId.ToString()]);
            }

        }

        #endregion

        #region " Event Handlers "


        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            this.PreRender += new EventHandler(Page_PreRender);

            rptLatestArticles.ItemDataBound += new RepeaterItemEventHandler(rptLatestArticles_OnItemDataBound);
            dlLatestArticles.ItemDataBound += dlLatestArticles_OnItemDataBound;
            
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {

                ReadQueryString();

                if (FindSettings())
                {
                    BindArticles();
                }
                else
                {
                    lblNotConfigured.Visible = true;
                    rptLatestArticles.Visible = false;
                }
            }
            catch (Exception exc)  //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            try
            {

                if (_articleTabID != Null.NullInteger)
                {
                    if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_INCLUDE_STYLESHEET))
                    {
                        if (Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_INCLUDE_STYLESHEET].ToString()))
                        {

                            Control objCSS = this.BasePage.FindControl("CSS");

                            if (objCSS != null)
                            {
                                HtmlLink objLink = new HtmlLink();
                                objLink.ID = "Template_" + this.ModuleId.ToString();
                                objLink.Attributes["rel"] = "stylesheet";
                                objLink.Attributes["type"] = "text/css";
                                objLink.Href = Page.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Templates/" + ArticleSettings.Template + "/Template.css");

                                objCSS.Controls.AddAt(0, objLink);
                            }

                        }
                    }
                }
            }
            catch (Exception exc)  //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void rptLatestArticles_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            try
            {

                if (e.Item.ItemType == ListItemType.Header)
                {
                    ProcessHeaderFooter(e.Item.Controls, _objLayoutHeader.Tokens);
                }

                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    if (e.Item.ItemIndex >= this.StartPoint)
                    {
                        ProcessBody(e.Item.Controls, (ArticleInfo)e.Item.DataItem, _objLayoutItem.Tokens);
                    }
                }

                if (e.Item.ItemType == ListItemType.Footer)
                {
                    ProcessHeaderFooter(e.Item.Controls, _objLayoutFooter.Tokens);
                }

            }
            catch (Exception exc)  //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void dlLatestArticles_OnItemDataBound(object sender, DataListItemEventArgs e)
        {

            try
            {

                if (e.Item.ItemType == ListItemType.Header)
                {
                    ProcessHeaderFooter(e.Item.Controls, _objLayoutHeader.Tokens);
                }

                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    if (e.Item.ItemIndex >= this.StartPoint)
                    {
                        ProcessBody(e.Item.Controls, (ArticleInfo)e.Item.DataItem, _objLayoutItem.Tokens);
                    }
                }

                if (e.Item.ItemType == ListItemType.Footer)
                {
                    ProcessHeaderFooter(e.Item.Controls, _objLayoutFooter.Tokens);
                }

            }
            catch (Exception exc)  //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void drpSort_SelectedIndexChanged(object sender, EventArgs e){

            List<string> lstparams = new List<string>();

            DropDownList drpSort = (DropDownList)sender;

            if (drpSort != null) {
                lstparams.Add("lasort-" + TabModuleId.ToString() + "=" + drpSort.SelectedValue);
                if (Request["laauth-" + TabModuleId.ToString()] != null)
                {
                    lstparams.Add("laauth-" + TabModuleId.ToString() + "=" + Request["laauth-" + TabModuleId.ToString()]);
                }
                if (Request["lacat-" + TabModuleId.ToString()] != null)
                {
                    lstparams.Add("lacat-" + TabModuleId.ToString() + "=" + Request["lacat-" + TabModuleId.ToString()]);
                }
                if (Request["latime-" + TabModuleId.ToString()] != null)
                {
                    lstparams.Add("latime-" + TabModuleId.ToString() + "=" + Request["latime-" + TabModuleId.ToString()]);
                }
                Response.Redirect(Globals.NavigateURL(this.TabId, "", lstparams.ToArray()), true);
            }

        }

        protected void drpAuthor_SelectedIndexChanged(object sender, EventArgs e)
        {

            List<string> lstparams = new List<string>();
            if (Request["lasort-" + TabModuleId.ToString()] != null)
            {
                lstparams.Add("lasort-" + TabModuleId.ToString() + "=" + Request["lasort-" + TabModuleId.ToString()]);
            }

            DropDownList drpAuthor = (DropDownList)sender;

            if (drpAuthor != null)
            {
                if (drpAuthor.SelectedValue != "-1")
                {
                    lstparams.Add("laauth-" + TabModuleId.ToString() + "=" + drpAuthor.SelectedValue);
                    if (Request["lacat-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("lacat-" + TabModuleId.ToString() + "=" + Request["lacat-" + TabModuleId.ToString()]);
                    }
                    if (Request["lacust-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("lacust-" + TabModuleId.ToString() + "=" + Request["lacust-" + TabModuleId.ToString()]);
                    }
                    if (Request["latime-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("latime-" + TabModuleId.ToString() + "=" + Request["latime-" + TabModuleId.ToString()]);
                    }
                    Response.Redirect(Globals.NavigateURL(this.TabId, "", lstparams.ToArray()), true);
                }
                else
                {
                    if (Request["lacat-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("lacat-" + TabModuleId.ToString() + "=" + Request["lacat-" + TabModuleId.ToString()]);
                    }
                    if (Request["lacust-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("lacust-" + TabModuleId.ToString() + "=" + Request["lacust-" + TabModuleId.ToString()]);
                    }
                    if (Request["latime-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("latime-" + TabModuleId.ToString() + "=" + Request["latime-" + TabModuleId.ToString()]);
                    }
                    Response.Redirect(Globals.NavigateURL(this.TabId, "", lstparams.ToArray()), true);
                }
            }

        }

        protected void drpCategory_SelectedIndexChanged(object sender, EventArgs e){

            List<string> lstparams = new List<string>();
            if (Request["lasort-" + TabModuleId.ToString()] != null)
            {
                lstparams.Add("lasort-" + TabModuleId.ToString() + "=" + Request["lasort-" + TabModuleId.ToString()]);
            }

            DropDownList drpCategory = (DropDownList)sender;

            if (drpCategory != null) {
                if (drpCategory.SelectedValue != "-1") {
                    if (Request["laauth-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("laauth-" + TabModuleId.ToString() + "=" + Request["laauth-" + TabModuleId.ToString()]);
                    }
                    lstparams.Add("lacat-" + TabModuleId.ToString() + "=" + drpCategory.SelectedValue);
                    if (Request["lacust-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("lacust-" + TabModuleId.ToString() + "=" + Request["lacust-" + TabModuleId.ToString()]);
                    }
                    if (Request["latime-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("latime-" + TabModuleId.ToString() + "=" + Request["latime-" + TabModuleId.ToString()]);
                    }
                    Response.Redirect( Globals.NavigateURL(this.TabId, "", lstparams.ToArray()), true);
                }
                else
                {
                    if (Request["laauth-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("laauth-" + TabModuleId.ToString() + "=" + Request["laauth-" + TabModuleId.ToString()]);
                    }
                    if (Request["lacust-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("lacust-" + TabModuleId.ToString() + "=" + Request["lacust-" + TabModuleId.ToString()]);
                    }
                    if (Request["latime-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("latime-" + TabModuleId.ToString() + "=" + Request["latime-" + TabModuleId.ToString()]);
                    }
                    Response.Redirect(Globals.NavigateURL(this.TabId, "", lstparams.ToArray()), true);
                }
            }

        }

        protected void drpCustom_SelectedIndexChanged(object sender, EventArgs e)
        {

            List<string> lstparams = new List<string>();
            if (Request["lasort-" + TabModuleId.ToString()] != null)
            {
                lstparams.Add("lasort-" + TabModuleId.ToString() + "=" + Request["lasort-" + TabModuleId.ToString()]);
            }

            DropDownList drpCustom = (DropDownList)sender;

            if (drpCustom != null)
            {
                if (drpCustom.SelectedValue != "-1")
                {
                    lstparams.Add("lacust-" + TabModuleId.ToString() + "=" + drpCustom.Attributes["CustomFieldID"] + "-" + drpCustom.SelectedValue);
                    if (Request["laauth-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("laauth-" + TabModuleId.ToString() + "=" + Request["laauth-" + TabModuleId.ToString()]);
                    }
                    if (Request["lacat-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("lacat-" + TabModuleId.ToString() + "=" + Request["lacat-" + TabModuleId.ToString()]);
                    }
                    if (Request["latime-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("latime-" + TabModuleId.ToString() + "=" + Request["latime-" + TabModuleId.ToString()]);
                    }
                    Response.Redirect(Globals.NavigateURL(this.TabId, "", lstparams.ToArray()), true);
                }
                else
                {
                    if (Request["laauth-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("laauth-" + TabModuleId.ToString() + "=" + Request["laauth-" + TabModuleId.ToString()]);
                    }
                    if (Request["lacat-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("lacat-" + TabModuleId.ToString() + "=" + Request["lacat-" + TabModuleId.ToString()]);
                    }
                    if (Request["latime-" + TabModuleId.ToString()] != null)
                    {
                        lstparams.Add("latime-" + TabModuleId.ToString() + "=" + Request["latime-" + TabModuleId.ToString()]);
                    }
                    Response.Redirect(Globals.NavigateURL(this.TabId, "", lstparams.ToArray()), true);
                }
            }

        }

        protected void drpTime_SelectedIndexChanged(object sender, EventArgs e)
        {

            List<string> lstparams = new List<string>();
            if (Request["lasort-" + TabModuleId.ToString()] != null)
            {
                lstparams.Add("lasort-" + TabModuleId.ToString() + "=" + Request["lasort-" + TabModuleId.ToString()]);
            }
            if (Request["laauth-" + TabModuleId.ToString()] != null)
            {
                lstparams.Add("laauth-" + TabModuleId.ToString() + "=" + Request["laauth-" + TabModuleId.ToString()]);
            }
            if (Request["lacat-" + TabModuleId.ToString()] != null)
            {
                lstparams.Add("lacat-" + TabModuleId.ToString() + "=" + Request["lacat-" + TabModuleId.ToString()]);
            }
            if (Request["lacust-" + TabModuleId.ToString()] != null)
            {
                lstparams.Add("lacust-" + TabModuleId.ToString() + "=" + Request["lacust-" + TabModuleId.ToString()]);
            }

            DropDownList drpTime = (DropDownList)sender;

            if (drpTime != null)
            {
                lstparams.Add("latime-" + TabModuleId.ToString() + "=" + drpTime.SelectedValue);
                Response.Redirect(Globals.NavigateURL(this.TabId, "", lstparams.ToArray()), true);
            }

        }

        #endregion
    }
}