using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.WebControls;
using GcDesign.NewsArticles.Components.CustomFields;
using DotNetNuke.Security;

using GcDesign.NewsArticles.Base;
using System.Collections;
using System.Web.UI.HtmlControls;
using GcDesign.NewsArticles.Components.WebControls;

namespace GcDesign.NewsArticles.Controls
{
    public partial class Listing : System.Web.UI.UserControl
    {

        #region "私有变量 private Members "

        private LayoutController _objLayoutController;

        private LayoutInfo _objLayoutHeader;
        private LayoutInfo _objLayoutItem;
        private LayoutInfo _objLayoutFeatured;
        private LayoutInfo _objLayoutFooter;
        private LayoutInfo _objLayoutEmpty;

        private List<ArticleInfo> _articleList;
        private int _articleCount;

        private DateTime _agedDate;
        private int _author;
        private bool _bindArticles;
        private bool _featuredOnly;
        private int[] _filterCategories;
        private bool _includeCategory;
        private MatchOperatorType _matchCategories;
        private int _maxArticles;
        private int _month;
        private bool _notFeaturedOnly;
        private bool _notSecuredOnly;
        private string _searchText;
        private bool _securedOnly;
        private bool _showExpired;
        private bool _showMessage;
        private bool _showPending;
        private string _sortBy;
        private string _sortDirection;
        private DateTime _startDate;
        private string _tag;
        private int _year;

        public bool IsIndexed = true;

        private int _customFieldID = Null.NullInteger;
        private string _customValue = Null.NullString;

        #endregion

        #region "私有属性 private Properties "

        private NewsArticleModuleBase ArticleModuleBase
        {
            get
            {
                return (NewsArticleModuleBase)Parent;
            }
        }

        private ArticleSettings ArticleSettings
        {
            get
            {
                return ArticleModuleBase.ArticleSettings;
            }
        }

        private int CurrentPage
        {
            get
            {
                if (Request["Page"] == null && Request["CurrentPage"] == null && Request["lapg"] == null)
                {
                    return 1;
                }
                else
                {
                    IsIndexed = false;
                    try
                    {
                        if (Request["Page"] != null)
                        {
                            return Convert.ToInt32(Request["Page"]);
                        }
                        else if (Request["CurrentPage"] != null)
                        {
                            return Convert.ToInt32(Request["CurrentPage"]);
                        }
                        else
                        {
                            return Convert.ToInt32(Request["lapg"]);
                        }
                    }
                    catch
                    {
                        return 1;
                    }
                }
            }
        }

        #endregion

        #region " public Properties "

        public DateTime AgedDate
        {
            get
            {
                return _agedDate;
            }
            set
            {
                _agedDate = value;
            }
        }

        public int Author
        {
            get
            {
                return _author;
            }
            set
            {
                _author = value;
            }
        }

        public bool BindArticles
        {
            get
            {
                return _bindArticles;
            }
            set
            {
                _bindArticles = value;
            }
        }

        private int DynamicAuthorID
        {
            get
            {
                int id = Null.NullInteger;
                if (Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                {
                    if (Numeric.IsNumeric(Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()]))
                    {
                        id = Convert.ToInt32(Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                }

                return id;
            }
        }

        private string DynamicAZ
        {
            get
            {
                string id = Null.NullString;
                if (Request["naaz-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                {
                    id = Request["naaz-" + ArticleModuleBase.TabModuleId.ToString()];
                }
                return id;
            }
        }

        private int DynamicCategoryID
        {
            get
            {
                int id = Null.NullInteger;
                if (Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                {
                    if (Numeric.IsNumeric(Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()]))
                    {
                        id = Convert.ToInt32(Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                }

                return id;
            }
        }

        private string DynamicSortBy
        {
            get
            {
                string sort = "";

                if (Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                {
                    switch (Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()].ToLower())
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

        private string DynamicTime
        {
            get
            {
                string val = "";

                if (Request["natime-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                {
                    val = Request["natime-" + ArticleModuleBase.TabModuleId.ToString()];
                }

                return val;
            }
        }

        public bool FeaturedOnly
        {
            get
            {
                return _featuredOnly;
            }
            set
            {
                _featuredOnly = value;
            }
        }

        public int[] FilterCategories
        {
            get
            {
                return _filterCategories;
            }
            set
            {
                _filterCategories = value;
            }
        }

        public bool IncludeCategory
        {
            get
            {
                return _includeCategory;
            }
            set
            {
                _includeCategory = value;
            }
        }

        public MatchOperatorType MatchCategories
        {
            get
            {
                return _matchCategories;
            }
            set
            {
                _matchCategories = value;
            }
        }

        public int MaxArticles
        {
            get
            {
                return _maxArticles;
            }
            set
            {
                _maxArticles = value;
            }
        }

        public int Month
        {
            get
            {
                return _month;
            }
            set
            {
                _month = value;
            }
        }

        public bool NotFeaturedOnly
        {
            get
            {
                return _notFeaturedOnly;
            }
            set
            {
                _notFeaturedOnly = value;
            }
        }

        public bool NotSecuredOnly
        {
            get
            {
                return _notSecuredOnly;
            }
            set
            {
                _notSecuredOnly = value;
            }
        }

        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
            }
        }

        public bool SecuredOnly
        {
            get
            {
                return _securedOnly;
            }
            set
            {
                _securedOnly = value;
            }
        }

        public bool ShowExpired
        {
            get
            {
                return _showExpired;
            }
            set
            {
                _showExpired = value;
            }
        }

        public bool ShowMessage
        {
            get
            {
                return _showMessage;
            }
            set
            {
                _showMessage = value;
            }
        }

        public bool ShowPending
        {
            get
            {
                return _showPending;
            }
            set
            {
                _showPending = value;
            }
        }

        public string SortBy
        {
            get
            {
                return _sortBy;
            }
            set
            {
                _sortBy = value;
            }
        }

        public string SortDirection
        {
            get
            {
                return _sortDirection;
            }
            set
            {
                _sortDirection = value;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }

        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        public int Year
        {
            get
            {
                return _year;
            }
            set
            {
                _year = value;
            }
        }

        #endregion

        #region " Private Methods "

        public void BindListing()
        {

            InitializeTemplate();

            if (_year != Null.NullInteger && _month != Null.NullInteger)
            {
                _agedDate = new DateTime(_year, _month, 1);
                StartDate = AgedDate.AddMonths(1).AddSeconds(-1);
            }

            if (_year != Null.NullInteger && _month == Null.NullInteger)
            {
                _agedDate = new DateTime(_year, 1, 1);
                StartDate = AgedDate.AddYears(1).AddSeconds(-1);
            }

            int[] objTags = null;
            if (_tag != Null.NullString)
            {
                TagController objTagController = new TagController();
                TagInfo objTag = objTagController.Get(ArticleModuleBase.ModuleId, _tag.ToLower());
                if (objTag != null)
                {
                    List<int> tags = new List<int>();
                    tags.Add(objTag.TagID);
                    objTags = tags.ToArray();
                }
            }

            if (FilterCategories != null && FilterCategories.Length == 1)
            {

                CategoryController objCategoryController = new CategoryController();
                CategoryInfo objCategory = objCategoryController.GetCategory(FilterCategories[0], ArticleModuleBase.ModuleId);

                if (objCategory != null)
                {

                    if (!objCategory.InheritSecurity)
                    {
                        if (ArticleModuleBase.Settings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                        {
                            if (PortalSecurity.IsInRoles(ArticleModuleBase.Settings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString()) == false)
                            {
                                Response.Redirect(Globals.NavigateURL(ArticleModuleBase.TabId), true);
                            }
                        }
                    }

                    ArticleController objArticleController = new ArticleController();
                    _articleList = objArticleController.GetArticleList(ArticleModuleBase.ModuleId, StartDate, _agedDate, FilterCategories, (MatchCategories == MatchOperatorType.MatchAll), null, MaxArticles, CurrentPage, ArticleSettings.PageSize, SortBy, SortDirection, true, false, SearchText.Replace("'", "''"), Author, ShowPending, ShowExpired, FeaturedOnly, NotFeaturedOnly, SecuredOnly, NotSecuredOnly, Null.NullString, objTags, false, Null.NullString, _customFieldID, _customValue, Null.NullString, ref _articleCount);

                }
            }
            else
            {
                CategoryController objCategoryController = new CategoryController();
                List<CategoryInfo> objCategories = objCategoryController.GetCategoriesAll(ArticleModuleBase.ModuleId, Null.NullInteger);

                List<int> excludeCategoriesRestrictive = new List<int>();

                foreach (CategoryInfo objCategory in objCategories)
                {
                    if (!objCategory.InheritSecurity && objCategory.CategorySecurityType == CategorySecurityType.Restrict)
                    {
                        if (Request.IsAuthenticated)
                        {
                            if (ArticleModuleBase.Settings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                            {
                                if (PortalSecurity.IsInRoles(ArticleModuleBase.Settings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString()) == false)
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

                List<int> excludeCategories = new List<int>();

                foreach (CategoryInfo objCategory in objCategories)
                {
                    if (!objCategory.InheritSecurity && objCategory.CategorySecurityType == CategorySecurityType.Loose)
                    {
                        if (Request.IsAuthenticated)
                        {
                            if (ArticleModuleBase.Settings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                            {
                                if (PortalSecurity.IsInRoles(ArticleModuleBase.Settings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString()) == false)
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

                List<int> includeCategories = new List<int>();

                if (excludeCategories.Count > 0)
                {

                    foreach (CategoryInfo objCategoryToInclude in objCategories)
                    {

                        bool includeCategorySecurity = true;

                        foreach (int exclCategory in excludeCategories)
                        {
                            if (exclCategory == objCategoryToInclude.CategoryID)
                            {
                                includeCategorySecurity = false;
                            }
                        }


                        if (FilterCategories != null)
                        {
                            if (FilterCategories.Length > 0)
                            {
                                bool filter = false;
                                foreach (int cat in FilterCategories)
                                {
                                    if (cat == objCategoryToInclude.CategoryID)
                                    {
                                        filter = true;
                                    }
                                }
                                if (!filter)
                                {
                                    includeCategorySecurity = false;
                                }
                            }
                        }

                        if (includeCategorySecurity)
                        {
                            includeCategories.Add(objCategoryToInclude.CategoryID);
                        }

                    }

                    if (includeCategories.Count > 0)
                    {
                        includeCategories.Add(-1);
                    }

                    FilterCategories = includeCategories.ToArray();

                }

                ArticleController objArticleController = new ArticleController();
                _articleList = objArticleController.GetArticleList(ArticleModuleBase.ModuleId, StartDate, _agedDate, FilterCategories, (MatchCategories == MatchOperatorType.MatchAll), excludeCategoriesRestrictive.ToArray(), MaxArticles, CurrentPage, ArticleSettings.PageSize, SortBy, SortDirection, true, false, SearchText.Replace("'", "''"), Author, ShowPending, ShowExpired, FeaturedOnly, NotFeaturedOnly, SecuredOnly, NotSecuredOnly, Null.NullString, objTags, false, Null.NullString, _customFieldID, _customValue, Null.NullString, ref _articleCount);
            }


            if (_articleList.Count == 0)
            {
                if (ShowMessage)
                {
                    ProcessHeader(phNoArticles.Controls, _objLayoutEmpty.Tokens);
                }
            }
            else
            {
                rptListing.DataSource = _articleList;
                rptListing.DataBind();
            }

        }

        private string GetParams(bool addDynamicFields)
        {

            string param = "";

            if (Request["ctl"] != null)
            {
                if (Request["ctl"].ToLower() == "categoryview" || Request["ctl"].ToLower() == "authorview" || Request["ctl"].ToLower() == "archiveview" || Request["ctl"].ToLower() == "search")
                {
                    param += "ctl=" + Request["ctl"] + "&mid=" + ArticleModuleBase.ModuleId.ToString();
                }
            }

            if (Request["articleType"] != null)
            {
                if (Request["articleType"].ToString().ToLower() == "categoryview" || Request["articleType"].ToString().ToLower() == "authorview" || Request["articleType"].ToString().ToLower() == "archiveview" || Request["articleType"].ToString().ToLower() == "search" || Request["articleType"].ToString().ToLower() == "myarticles" || Request["articleType"].ToString().ToLower() == "tagview")
                {
                    param += "articleType=" + Request["articleType"];
                }
            }

            if (FilterCategories != null && FilterCategories.Length > 0)
            {
                if (FilterCategories != ArticleSettings.FilterCategories)
                {
                    param += "&CategoryID=" + FilterCategories[0];
                }
            }

            bool authorSet = false;
            if (ArticleSettings.AuthorUserIDFilter)
            {
                if (ArticleSettings.AuthorUserIDParam != "")
                {
                    if (HttpContext.Current.Request[ArticleSettings.AuthorUserIDParam] != "")
                    {
                        param += "&" + ArticleSettings.AuthorUserIDParam + "=" + HttpContext.Current.Request[ArticleSettings.AuthorUserIDParam];
                        authorSet = true;
                    }
                }
            }

            if (ArticleSettings.AuthorUsernameFilter)
            {
                if (ArticleSettings.AuthorUsernameParam != "")
                {
                    if (HttpContext.Current.Request[ArticleSettings.AuthorUsernameParam] != "")
                    {
                        param += "&" + ArticleSettings.AuthorUsernameParam + "=" + HttpContext.Current.Request[ArticleSettings.AuthorUsernameParam];
                        authorSet = true;
                    }
                }
            }

            if (authorSet = false)
            {
                if (Author != ArticleSettings.Author)
                {
                    param += "&AuthorID=" + Author.ToString();
                }
            }

            if (Year != Null.NullInteger)
            {
                param += "&Year=" + Year.ToString();
            }

            if (Month != Null.NullInteger)
            {
                param += "&Month=" + Month.ToString();
            }

            if (Tag != Null.NullString)
            {
                param += "&Tag=" + Server.UrlEncode(Tag);
            }

            if (SearchText != Null.NullString)
            {
                if (Request["naaz-" + ArticleModuleBase.TabModuleId.ToString()] == null)
                {
                    param += "&Search=" + ArticleModuleBase.Server.UrlEncode(SearchText);
                }
            }

            if (addDynamicFields)
            {
                if (Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                {
                    param += "&nasort-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()];
                }
                if (Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                {
                    param += "&naauth-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()];
                }
                if (Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                {
                    param += "&nacat-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()];
                }
                if (Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                {
                    param += "&nacust-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()];
                }
                if (Request["natime-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                {
                    param += "&natime-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["natime-" + ArticleModuleBase.TabModuleId.ToString()];
                }
                if (Request["naaz-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                {
                    param += "&naaz-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["naaz-" + ArticleModuleBase.TabModuleId.ToString()];
                }
            }

            return param;

        }

        private void InitializeTemplate()
        {

            _objLayoutController = new LayoutController(ArticleModuleBase);
            _objLayoutController.IncludeCategory = IncludeCategory;

            _objLayoutHeader = LayoutController.GetLayout(ArticleModuleBase, LayoutType.Listing_Header_Html);
            _objLayoutFeatured = LayoutController.GetLayout(ArticleModuleBase, LayoutType.Listing_Featured_Html);
            _objLayoutItem = LayoutController.GetLayout(ArticleModuleBase, LayoutType.Listing_Item_Html);
            _objLayoutFooter = LayoutController.GetLayout(ArticleModuleBase, LayoutType.Listing_Footer_Html);
            _objLayoutEmpty = LayoutController.GetLayout(ArticleModuleBase, LayoutType.Listing_Empty_Html);

            if (_objLayoutFeatured.Template.Trim().Length == 0)
            {
                // Featured Template Empty or does not exist, use standard item.
                _objLayoutFeatured = _objLayoutItem;
            }

        }

        private void InitSettings()
        {

            _author = Null.NullInteger;

            if (ArticleSettings.AuthorUserIDFilter)
            {
                _author = -100;
                if (Request.QueryString[ArticleSettings.AuthorUserIDParam] != null)
                {
                    try
                    {
                        _author = Convert.ToInt32(Request.QueryString[ArticleSettings.AuthorUserIDParam]);
                    }
                    catch
                    {

                    }

                }
            }

            if (ArticleSettings.AuthorUsernameFilter)
            {
                _author = -100;
                if (Request.QueryString[ArticleSettings.AuthorUsernameParam] != null)
                {
                    try
                    {
                        DotNetNuke.Entities.Users.UserInfo objUser = DotNetNuke.Entities.Users.UserController.GetUserByName(ArticleModuleBase.PortalId, Request.QueryString[ArticleSettings.AuthorUsernameParam]);
                        if (objUser != null)
                        {
                            _author = objUser.UserID;
                        }
                    }
                    catch
                    {

                    }

                }
            }

            if (ArticleSettings.AuthorLoggedInUserFilter)
            {
                _author = -100;
                if (Request.IsAuthenticated)
                {
                    _author = ArticleModuleBase.UserId;
                }
            }

            if (ArticleSettings.Author != Null.NullInteger)
            {
                _author = ArticleSettings.Author;
            }

            if (DynamicAuthorID != Null.NullInteger)
            {
                _author = DynamicAuthorID;
            }

            _agedDate = Null.NullDate;
            _bindArticles = true;
            _featuredOnly = ArticleSettings.FeaturedOnly;
            if (ArticleSettings.FilterSingleCategory != Null.NullInteger)
            {
                List<int> cats = new List<int>();
                cats.Add(ArticleSettings.FilterSingleCategory);
                _filterCategories = cats.ToArray();
            }
            else
            {
                _filterCategories = ArticleSettings.FilterCategories;
            }
            if (DynamicCategoryID != Null.NullInteger)
            {
                List<int> cats = new List<int>();
                cats.Add(DynamicCategoryID);
                _filterCategories = cats.ToArray();
            }
            _includeCategory = false;
            _matchCategories = ArticleSettings.MatchCategories;
            _maxArticles = ArticleSettings.MaxArticles;
            _month = Null.NullInteger;
            _notFeaturedOnly = ArticleSettings.NotFeaturedOnly;
            _notSecuredOnly = ArticleSettings.NotSecuredOnly;
            _searchText = "";
            if (DynamicAZ != Null.NullString)
            {
                _searchText = DynamicAZ;
            }
            _securedOnly = ArticleSettings.SecuredOnly;
            _showExpired = false;
            _showMessage = true;
            _showPending = ArticleSettings.ShowPending;
            _sortBy = ArticleSettings.SortBy;
            if (ArticleSettings.BubbleFeatured)
            {
                _sortBy = "IsFeatured DESC, " + ArticleSettings.SortBy;
            }
            _sortDirection = ArticleSettings.SortDirection;
            _startDate = DateTime.Now.AddMinutes(1);
            _tag = Null.NullString;
            _year = Null.NullInteger;

            if (DynamicSortBy != "")
            {
                _sortBy = DynamicSortBy;
            }

            if (DynamicTime != "")
            {
                if (DynamicTime.ToLower() == "today")
                {
                    _startDate = DateTime.Now;
                    _agedDate = DateTime.Today;
                }
                if (DynamicTime.ToLower() == "yesterday")
                {
                    _startDate = DateTime.Today;
                    _agedDate = DateTime.Today.AddDays(-1);
                }
                if (DynamicTime.ToLower() == "threedays")
                {
                    _startDate = DateTime.Now;
                    _agedDate = DateTime.Today.AddDays(-3);
                }
                if (DynamicTime.ToLower() == "sevendays")
                {
                    _startDate = DateTime.Now;
                    _agedDate = DateTime.Today.AddDays(-7);
                }
                if (DynamicTime.ToLower() == "thirtydays")
                {
                    _startDate = DateTime.Now;
                    _agedDate = DateTime.Today.AddDays(-30);
                }
                if (DynamicTime.ToLower() == "ninetydays")
                {
                    _startDate = DateTime.Now;
                    _agedDate = DateTime.Today.AddDays(-90);
                }
                if (DynamicTime.ToLower() == "thisyear")
                {
                    _startDate = DateTime.Now;
                    _agedDate = DateTime.Today.AddYears(-1);
                }
            }

            if (Request["YEAR"] != null)
            {
                string start = Request["YEAR"] + "-1-1 00:00:00";
                string end = Request["YEAR"] + "-12-31 23:59:59";
                //截止日期
                _startDate = Convert.ToDateTime(end);
                //开始日期
                _agedDate = Convert.ToDateTime(start);
            }

            if (Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()] != null)
            {
                string val = Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()];
                if (val.Split('-').Length == 2)
                {
                    if (Numeric.IsNumeric(val.Split('-')[0]))
                    {
                        _customFieldID = Convert.ToInt32(val.Split('-')[0]);
                        _customValue = val.Split('-')[1];
                    }
                }
            }
        }

        private void ProcessHeader(ControlCollection objPlaceHolder, string[] templateArray)
        {

            int pageCount = ((_articleCount - 1) / ArticleSettings.PageSize) + 1;

            DropDownList drpTime;
            CategoryController objCategoryController;
            Literal objLiteral;
            HyperLink objLink;

            for (int iPtr = 0; iPtr < templateArray.Length; iPtr = iPtr + 2)
            {

                objPlaceHolder.Add(new LiteralControl(_objLayoutController.ProcessImages(templateArray[iPtr].ToString())));

                if (iPtr < templateArray.Length - 1)
                {
                    switch (templateArray[iPtr + 1])
                    {

                        case "AUTHOR":
                            AuthorController objAuthorController = new AuthorController();
                            DropDownList drpAuthor = new DropDownList();
                            drpAuthor.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());
                            drpAuthor.DataTextField = "DisplayName";
                            drpAuthor.DataValueField = "UserID";
                            drpAuthor.DataSource = objAuthorController.GetAuthorList(ArticleModuleBase.ModuleId);
                            drpAuthor.DataBind();
                            drpAuthor.Items.Insert(0, new ListItem(ArticleModuleBase.GetSharedResource("SelectAuthor.Text"), "-1"));
                            drpAuthor.AutoPostBack = true;
                            if (DynamicAuthorID != Null.NullInteger)
                            {
                                if (drpAuthor.Items.FindByValue(DynamicAuthorID.ToString()) != null)
                                {
                                    drpAuthor.SelectedValue = DynamicAuthorID.ToString();
                                }
                            }
                            drpAuthor.SelectedIndexChanged += new System.EventHandler(drpAuthor_SelectedIndexChanged);
                            objPlaceHolder.Add(drpAuthor);
                            break;
                        case "AZ":

                            string list = "";
                            foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray())
                            {

                                List<string> param = new List<string>();

                                string args = GetParams(false);
                                foreach (string arg in args.Split('&'))
                                {
                                    param.Add(arg);
                                }

                                if (Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                                {
                                    param.Add("nasort-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()]);
                                }
                                if (Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                                {
                                    param.Add("nacust-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()]);
                                }
                                if (Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                                {
                                    param.Add("naauth-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()]);
                                }
                                if (Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                                {
                                    param.Add("nacat-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()]);
                                }
                                if (Request["natime-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                                {
                                    param.Add("natime-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["natime-" + ArticleModuleBase.TabModuleId.ToString()]);
                                }

                                if (list == "")
                                {
                                    if (Request["naaz-" + ArticleModuleBase.TabModuleId.ToString()] == c.ToString())
                                    {
                                        list = c.ToString();
                                    }
                                    else
                                    {
                                        List<string> paramsCopy = param;
                                        paramsCopy.Add("naaz-" + ArticleModuleBase.TabModuleId.ToString() + "=" + c);
                                        list = "<a href=\"" + Globals.NavigateURL(ArticleModuleBase.TabId, "", paramsCopy.ToArray()) + "\">" + c + "</a>";
                                    }
                                }
                                else
                                {
                                    if (Request["naaz-" + ArticleModuleBase.TabModuleId.ToString()] == c.ToString())
                                    {
                                        list = list + "&nbsp;" + c.ToString();
                                    }
                                    else
                                    {
                                        List<string> paramsCopy = param;
                                        paramsCopy.Add("naaz-" + ArticleModuleBase.TabModuleId.ToString() + "=" + c);
                                        list = list + "&nbsp;" + "<a href=\"" + Globals.NavigateURL(ArticleModuleBase.TabId, "", paramsCopy.ToArray()) + "\">" + c + "</a>";
                                    }
                                }
                            }
                            if (Request["naaz-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                            {
                                list = list + "&nbsp;" + "<a href=\"" + Globals.NavigateURL(ArticleModuleBase.TabId, "", GetParams(false)) + "\">" + "All" + "</a>";
                            }
                            else
                            {
                                list = list + "&nbsp;" + "All";
                            }

                            objLiteral = new Literal();
                            objLiteral.Text = list;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TIMEYEAR":

                            string str = "";

                            int yearNow = DateTime.Now.Year;
                            int n = yearNow - 2013;
                            str += "<div class=\"btn-group\">" + "\r\n" +
                                    "<button type=\"button\" class=\"btn btn-default dropdown-toggle\" data-toggle=\"dropdown\">" + "\r\n" +
                                    "Filter by Year <span class=\"caret\"></span></button>" + "\r\n" +
                                    "<ul class=\"dropdown-menu\" role=\"menu\">";
                            for (int i = 0; i <= n; i++)
                            {
                                List<string> param = new List<string>();
                                Year = 2013 + i;
                                string args = GetParams(false);
                                foreach (string arg in args.Split('&'))
                                {
                                    param.Add(arg);
                                }
                                string url = Globals.NavigateURL(ArticleModuleBase.TabId, "", param.ToArray());
                                str += "<li><a href=\"" + url + "\">" + (2013 + i).ToString() + "</a></li>";
                            }
                            str += "</ul></div>";

                            objLiteral = new Literal();
                            objLiteral.Text = str;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "CATEGORY":
                            objCategoryController = new CategoryController();
                            DropDownList drpCategory = new DropDownList();
                            drpCategory.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());
                            drpCategory.DataTextField = "NameIndented";
                            drpCategory.DataValueField = "CategoryID";
                            drpCategory.DataSource = objCategoryController.GetCategoriesAll(ArticleModuleBase.ModuleId, Null.NullInteger, ArticleSettings.CategorySortType);
                            drpCategory.DataBind();
                            drpCategory.Items.Insert(0, new ListItem(ArticleModuleBase.GetSharedResource("SelectCategory.Text"), "-1"));
                            drpCategory.AutoPostBack = true;
                            if (DynamicCategoryID != Null.NullInteger)
                            {
                                if (drpCategory.Items.FindByValue(DynamicCategoryID.ToString()) != null)
                                {
                                    drpCategory.SelectedValue = DynamicCategoryID.ToString();
                                }
                            }
                            drpCategory.SelectedIndexChanged += new System.EventHandler(drpCategory_SelectedIndexChanged);
                            objPlaceHolder.Add(drpCategory);
                            break;
                        case "CATEGORYFILTER":
                            if (_filterCategories != null)
                            {
                                string categories = "";
                                objCategoryController = new CategoryController();
                                foreach (int ID in _filterCategories)
                                {
                                    CategoryInfo objCategory = objCategoryController.GetCategory(ID, ArticleModuleBase.ModuleId);
                                    if (objCategory != null)
                                    {
                                        if (categories == "")
                                        {
                                            categories = objCategory.Name;
                                        }
                                        else
                                        {
                                            categories = categories + " | " + objCategory.Name;
                                        }
                                    }
                                }
                                objLiteral = new Literal();
                                objLiteral.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());
                                objLiteral.Text = categories;
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "CATEGORYSELECTED":
                            if (Request["articleType"] != null && Request["articleType"].ToLower() != "categoryview")
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/CATEGORYSELECTED")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            else
                            {
                                if (Request["articleType"] == null)//""
                                {
                                    while (iPtr < templateArray.Length - 1)
                                    {
                                        if (templateArray[iPtr + 1] == "/CATEGORYSELECTED")
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                            }
                            break;
                        case "/CATEGORYSELECTED":
                            // Do Nothing
                            break;
                        case "CATEGORYNOTSELECTED":
                            if (Request["articleType"] != null && Request["articleType"].ToLower() == "categoryview")
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/CATEGORYNOTSELECTED")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/CATEGORYNOTSELECTED":
                            // Do Nothing
                            break;
                        case "CATEGORYNOTSELECTED2":
                            if (Request["articleType"] != null && Request["articleType"].ToLower() == "categoryview")
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/CATEGORYNOTSELECTED")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/CATEGORYNOTSELECTED2":
                            // Do Nothing
                            break;
                        case "CURRENTPAGE":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());
                            objLiteral.Text = CurrentPage.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "HASMULTIPLEPAGES":
                            if (pageCount == 1)
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/HASMULTIPLEPAGES")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASMULTIPLEPAGES":
                            // Do Nothing
                            break;
                        case "HASNEXTPAGE":

                            if (CurrentPage == pageCount)
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/HASNEXTPAGE")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASNEXTPAGE":
                            // Do Nothing
                            break;
                        case "HASPREVPAGE":
                            if (CurrentPage == 1)
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/HASPREVPAGE")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASPREVPAGE":
                            // Do Nothing
                            break;
                        case "LINKNEXT":
                            objLink = new HyperLink();
                            objLink.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());
                            objLink.CssClass = "CommandButton";
                            objLink.Enabled = (CurrentPage < pageCount);
                            objLink.NavigateUrl = Globals.NavigateURL(ArticleModuleBase.TabId, "", GetParams(true), "CurrentPage=" + (CurrentPage + 1).ToString());
                            objLink.Text = ArticleModuleBase.GetSharedResource("NextPage.Text");
                            objPlaceHolder.Add(objLink);
                            break;
                        case "LINKNEXTURL":
                            if (CurrentPage < pageCount)
                            {
                                objLiteral = new Literal();
                                objLiteral.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());
                                objLiteral.Text = Globals.NavigateURL(ArticleModuleBase.TabId, "", GetParams(true), "CurrentPage=" + (CurrentPage + 1).ToString());
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "LINKPREVIOUS":
                            objLink = new HyperLink();
                            objLink.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());
                            objLink.CssClass = "CommandButton";
                            objLink.Enabled = (CurrentPage > 1);
                            objLink.NavigateUrl = Globals.NavigateURL(ArticleModuleBase.TabId, "", GetParams(true), "CurrentPage=" + (CurrentPage - 1).ToString());
                            objLink.Text = ArticleModuleBase.GetSharedResource("PreviousPage.Text");
                            objPlaceHolder.Add(objLink);
                            break;
                        case "LINKPREVIOUSURL":
                            if (CurrentPage > 1)
                            {
                                objLiteral = new Literal();
                                objLiteral.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());
                                objLiteral.Text = Globals.NavigateURL(ArticleModuleBase.TabId, "", GetParams(true), "CurrentPage=" + (CurrentPage - 1).ToString());
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "PAGECOUNT":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());
                            objLiteral.Text = pageCount.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "PAGER":
                            PageControl ctlPagingControl = new PageControl();
                            ctlPagingControl.Visible = true;
                            ctlPagingControl.TotalRecords = _articleCount;
                            ctlPagingControl.PageSize = ArticleSettings.PageSize;
                            ctlPagingControl.CurrentPage = CurrentPage;
                            ctlPagingControl.CSSClassLinkInactive = "pageActive";
                            ctlPagingControl.CssClass = "pageCss";
                            ctlPagingControl.CSSClassPagingStatus = "pageStatus";
                            ctlPagingControl.QuerystringParams = GetParams(true);
                            ctlPagingControl.TabID = ArticleModuleBase.TabId;
                            ctlPagingControl.EnableViewState = false;
                            objPlaceHolder.Add(ctlPagingControl);
                            break;
                        case "PAGER2":
                            objLiteral = new Literal();
                            if (_articleCount > 0)
                            {
                                int pages = _articleCount / ArticleSettings.PageSize;
                                objLiteral.Text = objLiteral.Text + "<ul>";
                                for (int i = 1; i < pages; i++)
                                {
                                    if (CurrentPage == i)
                                    {
                                        objLiteral.Text = objLiteral.Text + "<li>" + i.ToString() + "</li>";
                                    }
                                    else
                                    {
                                        string param = GetParams(true);
                                        if (i > 1)
                                        {
                                            param += "&currentpage=" + i.ToString();
                                        }
                                        objLiteral.Text = objLiteral.Text + "<li><a href=\"" + Globals.NavigateURL(ArticleModuleBase.TabId, "", param) + "\">" + i.ToString() + "</a></li>";
                                    }
                                }
                                objLiteral.Text = objLiteral.Text + "</ul>";
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "SORT":
                            DropDownList drpSort = new DropDownList();
                            drpSort.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());
                            drpSort.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("PublishDate.Text"), "PublishDate"));
                            drpSort.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("ExpiryDate.Text"), "ExpiryDate"));
                            drpSort.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("LastUpdate.Text"), "LastUpdate"));
                            drpSort.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("HighestRated.Text"), "Rating"));
                            drpSort.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("MostCommented.Text"), "CommentCount"));
                            drpSort.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("MostViewed.Text"), "NumberOfViews"));
                            drpSort.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("Random.Text"), "Random"));
                            drpSort.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("SortTitle.Text"), "Title"));
                            drpSort.AutoPostBack = true;

                            if (Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                            {
                                if (drpSort.Items.FindByValue(Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()]) != null)
                                {
                                    drpSort.SelectedValue = Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()];
                                }
                            }
                            else
                            {
                                string sort = SortBy;

                                switch (SortBy.ToLower())
                                {
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

                                if (drpSort.Items.FindByValue(sort) != null)
                                {
                                    drpSort.SelectedValue = sort;
                                }
                            }

                            drpSort.SelectedIndexChanged += new System.EventHandler(drpSort_SelectedIndexChanged);
                            objPlaceHolder.Add(drpSort);
                            break;
                        case "TABID":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());
                            objLiteral.Text = ArticleModuleBase.TabId.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TIME":
                            drpTime = new DropDownList();
                            drpTime.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());

                            drpTime.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("Today.Text"), "Today"));
                            drpTime.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("Yesterday.Text"), "Yesterday"));
                            drpTime.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("ThreeDays.Text"), "ThreeDays"));
                            drpTime.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("SevenDays.Text"), "SevenDays"));
                            drpTime.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("ThirtyDays.Text"), "ThirtyDays"));
                            drpTime.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("NinetyDays.Text"), "NinetyDays"));
                            drpTime.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("ThisYear.Text"), "ThisYear"));
                            drpTime.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("AllTime.Text"), "AllTime"));
                            drpTime.AutoPostBack = true;

                            if (DynamicTime != "")
                            {
                                if (drpTime.Items.FindByValue(DynamicTime) != null)
                                {
                                    drpTime.SelectedValue = DynamicTime;
                                }
                            }
                            else
                            {
                                drpTime.SelectedValue = "AllTime";
                            }

                            drpTime.SelectedIndexChanged += new System.EventHandler(drpTime_SelectedIndexChanged);

                            objPlaceHolder.Add(drpTime);
                            break;
                        default:

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("CUSTOM:"))
                            {
                                string customField = templateArray[iPtr + 1].Substring(7, templateArray[iPtr + 1].Length - 7);

                                CustomFieldController objCustomFieldController = new CustomFieldController();
                                ArrayList objCustomFields = objCustomFieldController.List(ArticleModuleBase.ModuleId);

                                foreach (CustomFieldInfo objCustomField in objCustomFields)
                                {
                                    if (objCustomField.Name.ToLower() == customField.ToLower())
                                    {
                                        if (objCustomField.FieldType == CustomFieldType.DropDownList)
                                        {
                                            DropDownList drpCustom = new DropDownList();
                                            drpCustom.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());

                                            foreach (string val in objCustomField.FieldElements.Split('|'))
                                            {
                                                drpCustom.Items.Add(val);
                                            }

                                            string sel = ArticleModuleBase.GetSharedResource("SelectCustom.Text");
                                            if (sel.IndexOf("{0}") != -1)
                                            {
                                                sel = sel.Replace("{0}", objCustomField.Caption);
                                            }
                                            drpCustom.Items.Insert(0, new ListItem(sel, "-1"));
                                            drpCustom.Attributes.Add("CustomFieldID", objCustomField.CustomFieldID.ToString());
                                            drpCustom.AutoPostBack = true;

                                            if (Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                                            {
                                                string val = Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()];
                                                if (val.Split('-').Length == 2)
                                                {
                                                    if (val.Split('-')[0] == objCustomField.CustomFieldID.ToString())
                                                    {
                                                        if (drpCustom.Items.FindByValue(val.Split('-')[1].ToString()) != null)
                                                        {
                                                            drpCustom.SelectedValue = val.Split('-')[1].ToString();
                                                        }
                                                    }
                                                }
                                            }

                                            drpCustom.SelectedIndexChanged += new System.EventHandler(drpCustom_SelectedIndexChanged);
                                            objPlaceHolder.Add(drpCustom);

                                        }
                                        break;
                                    }
                                }

                                break;

                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("RESX:"))
                            {
                                string entry = templateArray[iPtr + 1].Substring(5, templateArray[iPtr + 1].Length - 5);

                                if (entry != "")
                                {
                                    objLiteral = new Literal();
                                    objLiteral.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());
                                    objLiteral.Text = ArticleModuleBase.GetSharedResource(entry);
                                    objPlaceHolder.Add(objLiteral);
                                }

                                break;

                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("SORT:"))
                            {

                                List<string> param = new List<string>();

                                string sortItem = templateArray[iPtr + 1].Substring(5, templateArray[iPtr + 1].Length - 5);
                                string sortValue = sortItem;

                                switch (sortItem.ToLower())
                                {
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

                                switch (sort.ToLower())
                                {
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

                                if (sortValue.ToLower() == sort.ToLower())
                                {
                                    objLiteral = new Literal();
                                    objLiteral.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());
                                    objLiteral.Text = ArticleModuleBase.GetSharedResource(sortItem + ".Text");
                                    objPlaceHolder.Add(objLiteral);
                                }
                                else
                                {
                                    objLink = new HyperLink();
                                    objLink.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());
                                    param.Add("nasort-" + ArticleModuleBase.TabModuleId.ToString() + "=" + sortValue);
                                    if (Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                                    {
                                        param.Add("naauth-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()]);
                                    }
                                    if (Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                                    {
                                        param.Add("nacat-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()]);
                                    }
                                    if (Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                                    {
                                        param.Add("nacust-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()]);
                                    }
                                    if (Request["natime-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                                    {
                                        param.Add("natime-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["natime-" + ArticleModuleBase.TabModuleId.ToString()]);
                                    }
                                    objLink.NavigateUrl = Globals.NavigateURL(ArticleModuleBase.TabId, "", param.ToArray());
                                    objLink.Text = ArticleModuleBase.GetSharedResource(sortItem + ".Text");
                                    objPlaceHolder.Add(objLink);
                                }
                                break;

                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("TIME:"))
                            {

                                string timeItem = templateArray[iPtr + 1].Substring(5, templateArray[iPtr + 1].Length - 5);

                                drpTime = new DropDownList();
                                drpTime.ID = Globals.CreateValidID("Article-Header-" + iPtr.ToString());

                                drpTime.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("Today"), "Today"));
                                drpTime.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("Yesterday"), "Yesterday"));
                                drpTime.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("ThreeDays"), "ThreeDays"));
                                drpTime.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("SevenDays"), "SevenDays"));
                                drpTime.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("ThirtyDays"), "ThirtyDays"));
                                drpTime.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("NinetyDays"), "NinetyDays"));
                                drpTime.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("ThisYear"), "ThisYear"));
                                drpTime.Items.Add(new ListItem(ArticleModuleBase.GetSharedResource("AllTime"), "AllTime"));
                                drpTime.AutoPostBack = true;

                                if (DynamicTime != "")
                                {
                                    if (drpTime.Items.FindByValue(DynamicTime) != null)
                                    {
                                        drpTime.SelectedValue = DynamicTime;
                                    }
                                }
                                else
                                {
                                    if (drpTime.Items.FindByValue(timeItem) != null)
                                    {
                                        List<string> param = new List<string>();
                                        if (Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                                        {
                                            param.Add("nasort-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()]);
                                        }
                                        if (Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                                        {
                                            param.Add("naauth-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()]);
                                        }
                                        if (Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                                        {
                                            param.Add("nacat-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()]);
                                        }

                                        param.Add("natime-" + ArticleModuleBase.TabModuleId.ToString() + "=" + timeItem);
                                        Response.Redirect(Globals.NavigateURL(ArticleModuleBase.TabId, "", param.ToArray()), true);
                                    }
                                    else
                                    {
                                        drpTime.SelectedValue = "AllTime";
                                    }
                                }

                                drpTime.SelectedIndexChanged += new System.EventHandler(drpTime_SelectedIndexChanged);
                                objPlaceHolder.Add(drpTime);
                                break;
                            }

                            TokenProcessor.ProcessMenuItem(templateArray[iPtr + 1], objPlaceHolder, _objLayoutController, ArticleModuleBase, ref iPtr, templateArray, MenuOptionType.CurrentArticles);
                            break;
                    }
                }

            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Init += new EventHandler(this.Page_Init);
            this.Load += new EventHandler(this.Page_Load);
            this.PreRender += new EventHandler(Page_PreRender);
            rptListing.ItemDataBound += rptListing_OnItemDataBound;

        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Init(object sender, EventArgs e)
        {

            try
            {

                InitSettings();
            }
            catch (Exception exc)  //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {

            try { }

            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            try
            {

                if (BindArticles)
                {
                    BindListing();
                }
                ArticleModuleBase.LoadStyleSheet();

                if (!IsIndexed)
                {
                    // no index but follow links

                    try
                    {
                        //remove the existing MetaRobots entry 
                        Page.Header.Controls.Remove(Page.Header.FindControl("MetaRobots"));

                        //build our own new entry 
                        HtmlMeta mymetatag = new HtmlMeta();
                        mymetatag.Name = "robots";
                        mymetatag.Content = "NOINDEX, FOLLOW";
                        Page.Header.Controls.Add(mymetatag);
                    }
                    catch (Exception ex)
                    {
                        //catch an exception if MetaRobots is not present 

                    }


                }
            }
            catch (Exception exc)  //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void rptListing_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            try
            {

                if (e.Item.ItemType == ListItemType.Header)
                {
                    ProcessHeader(e.Item.Controls, _objLayoutHeader.Tokens);
                }

                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {

                    ArticleInfo objArticle = (ArticleInfo)e.Item.DataItem;

                    if (objArticle.IsFeatured)
                    {
                        _objLayoutController.ProcessArticleItem(e.Item.Controls, _objLayoutFeatured.Tokens, objArticle);
                    }
                    else
                    {
                        _objLayoutController.ProcessArticleItem(e.Item.Controls, _objLayoutItem.Tokens, objArticle);
                    }

                }

                if (e.Item.ItemType == ListItemType.Footer)
                {
                    ProcessHeader(e.Item.Controls, _objLayoutFooter.Tokens);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void drpAuthor_SelectedIndexChanged(object sender, EventArgs e)
        {

            List<string> param = new List<string>();

            string args = GetParams(false);
            foreach (string arg in args.Split('&'))
            {
                param.Add(arg);
            }

            if (Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()] != null)
            {
                param.Add("nasort-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()]);
            }

            DropDownList drpAuthor = (DropDownList)sender;

            if (drpAuthor != null)
            {
                if (drpAuthor.SelectedValue != "-1")
                {
                    param.Add("naauth-" + ArticleModuleBase.TabModuleId.ToString() + "=" + drpAuthor.SelectedValue);
                    if (Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("nacat-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    if (Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("nacust-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    if (Request["natime-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("natime-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["natime-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    Response.Redirect(Globals.NavigateURL(ArticleModuleBase.TabId, "", param.ToArray()), true);
                }
                else
                {
                    if (Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("nacat-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    if (Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("nacust-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    if (Request["natime-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("natime-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["natime-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    Response.Redirect(Globals.NavigateURL(ArticleModuleBase.TabId, "", param.ToArray()), true);
                }
            }

        }

        protected void drpCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

            List<string> param = new List<string>();

            string args = GetParams(false);
            foreach (string arg in args.Split('&'))
            {
                param.Add(arg);
            }

            if (Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()] != null)
            {
                param.Add("nasort-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()]);
            }

            DropDownList drpCategory = (DropDownList)sender;

            if (drpCategory != null)
            {
                if (drpCategory.SelectedValue != "-1")
                {
                    if (Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("naauth-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    param.Add("nacat-" + ArticleModuleBase.TabModuleId.ToString() + "=" + drpCategory.SelectedValue);
                    if (Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("nacust-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    if (Request["natime-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("natime-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["natime-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    Response.Redirect(Globals.NavigateURL(ArticleModuleBase.TabId, "", param.ToArray()), true);
                }
                else
                {
                    if (Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("naauth-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    if (Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("nacust-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    if (Request["natime-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("natime-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["natime-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    Response.Redirect(Globals.NavigateURL(ArticleModuleBase.TabId, "", param.ToArray()), true);
                }
            }

        }

        protected void drpCustom_SelectedIndexChanged(object sender, EventArgs e)
        {

            List<string> param = new List<string>();

            string args = GetParams(false);
            foreach (string arg in args.Split('&'))
            {
                param.Add(arg);
            }

            if (Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()] != null)
            {
                param.Add("nasort-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()]);
            }

            DropDownList drpCustom = (DropDownList)sender;

            if (drpCustom != null)
            {
                if (drpCustom.SelectedValue != "-1")
                {
                    param.Add("nacust-" + ArticleModuleBase.TabModuleId.ToString() + "=" + drpCustom.Attributes["CustomFieldID"] + "-" + drpCustom.SelectedValue);
                    if (Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("naauth-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    if (Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("nacat-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    if (Request["natime-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("natime-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["natime-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    Response.Redirect(Globals.NavigateURL(ArticleModuleBase.TabId, "", param.ToArray()), true);
                }
                else
                {
                    if (Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("naauth-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    if (Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("nacat-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    if (Request["natime-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                    {
                        param.Add("natime-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["natime-" + ArticleModuleBase.TabModuleId.ToString()]);
                    }
                    Response.Redirect(Globals.NavigateURL(ArticleModuleBase.TabId, "", param.ToArray()), true);
                }
            }

        }

        protected void drpSort_SelectedIndexChanged(object sender, EventArgs e)
        {

            List<string> param = new List<string>();

            string args = GetParams(false);
            foreach (string arg in args.Split('&'))
            {
                param.Add(arg);
            }

            DropDownList drpSort = (DropDownList)sender;

            if (drpSort != null)
            {
                param.Add("nasort-" + ArticleModuleBase.TabModuleId.ToString() + "=" + drpSort.SelectedValue);
                if (Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                {
                    param.Add("naauth-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()]);
                }
                if (Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                {
                    param.Add("nacat-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()]);
                }
                if (Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                {
                    param.Add("nacust-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()]);
                }
                if (Request["natime-" + ArticleModuleBase.TabModuleId.ToString()] != null)
                {
                    param.Add("natime-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["natime-" + ArticleModuleBase.TabModuleId.ToString()]);
                }
                Response.Redirect(Globals.NavigateURL(ArticleModuleBase.TabId, "", param.ToArray()), true);
            }

        }

        protected void drpTime_SelectedIndexChanged(object sender, EventArgs e)
        {

            List<string> param = new List<string>();

            string args = GetParams(false);
            foreach (string arg in args.Split('&'))
            {
                param.Add(arg);
            }

            if (Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()] != null)
            {
                param.Add("nasort-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nasort-" + ArticleModuleBase.TabModuleId.ToString()]);
            }
            if (Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()] != null)
            {
                param.Add("naauth-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["naauth-" + ArticleModuleBase.TabModuleId.ToString()]);
            }
            if (Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()] != null)
            {
                param.Add("nacat-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacat-" + ArticleModuleBase.TabModuleId.ToString()]);
            }
            if (Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()] != null)
            {
                param.Add("nacust-" + ArticleModuleBase.TabModuleId.ToString() + "=" + Request["nacust-" + ArticleModuleBase.TabModuleId.ToString()]);
            }

            DropDownList drpTime = (DropDownList)sender;

            if (drpTime != null)
            {
                param.Add("natime-" + ArticleModuleBase.TabModuleId.ToString() + "=" + drpTime.SelectedValue);
                Response.Redirect(Globals.NavigateURL(ArticleModuleBase.TabId, "", param.ToArray()), true);
            }

        }

        #endregion



    }
}