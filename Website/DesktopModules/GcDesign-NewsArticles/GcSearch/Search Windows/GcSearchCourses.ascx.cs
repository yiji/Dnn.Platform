using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;

using GcDesign.NewsArticles.Base;
using System.Text;
using System.Collections;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Common;
using DotNetNuke.Services.Localization;

namespace GcDesign.NewsArticles
{
    public partial class GcSearchCourses : NewsArticleModuleBase
    {
        #region " Constants "

        private const string PARAM_SEARCH_ID = "Search";

        #endregion

        #region " private Members "

        private int _articleTabID = Null.NullInteger;
        private int _articleModuleID = Null.NullInteger;

        private string courses_data = "48";
        private string organization_data = "49";
        private bool postback = false;

        private ArticleSettings _articleSettings;
        private LayoutModeType _layoutMode;
        private int _pageNumber = 1;

        private LayoutController _objLayoutController;

        private LayoutInfo _objLayoutHeader;
        private LayoutInfo _objLayoutItem;
        private LayoutInfo _objLayoutFooter;

        private Hashtable _settings;

        private ArrayList _objPages;

        private DotNetNuke.Entities.Tabs.TabInfo _tab;

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

        private ArrayList Pages(int articleId)
        {

            if (_objPages == null)
            {
                PageController objPageController = new PageController();
                _objPages = objPageController.GetPageList(articleId);
            }
            return _objPages;

        }

        private DotNetNuke.Entities.Tabs.TabInfo Tab
        {
            get
            {
                if (_tab == null)
                {
                    DotNetNuke.Entities.Tabs.TabController objTabController = new DotNetNuke.Entities.Tabs.TabController();
                    _tab = objTabController.GetTab(_articleTabID, PortalSettings.PortalId, false);
                    //_tab = objTabController.GetTab(112, PortalSettings.PortalId, false)
                }

                return _tab;
            }
        }
        #endregion

        #region " Public Members "

        public CategoryInfo courses;
        public CategoryInfo organization;

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

        #endregion



        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);

            rptLatestArticles.ItemDataBound += rptLatestArticles_ItemDataBound;
        }

        void rptLatestArticles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            
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
                if (!IsPostBack)
                {
                    ReadQueryString();
                }


                if (FindSettings())
                {
                    lblNotConfigured.Visible = false;
                    BindListView();
                }
                else
                {
                    lblNotConfigured.Visible = true;
                }
                if (!IsPostBack && Request["postback"] == null)
                {
                    BindArticles();
                }
                if (postback)
                {
                    string json = "$link$" + Common.GetModuleLink(_articleTabID, _articleModuleID, "Search", ArticleSettings, "courses_data=" + courses_data, "organization_data=" + organization_data) + "$link$";
                    //Response.Redirect(Common.GetModuleLink(_articleTabID, _articleModuleID, "Search", ArticleSettings, "courses_data=" + courses_data, "organization_data=" + organization_data, "time_data=" + time_data), true);

                    //Response.Write(json);

                    //搜索文章
                    BindArticles();
                }
            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }



        #endregion

        #region " private Methods "

        private bool FindSettings()
        {

            if (Settings.Contains(ArticleConstants.NEWS_SEARCH_TAB_ID))
            {
                if (Numeric.IsNumeric(Settings[ArticleConstants.NEWS_SEARCH_TAB_ID].ToString()))
                {
                    _articleTabID = Convert.ToInt32(Settings[ArticleConstants.NEWS_SEARCH_TAB_ID].ToString());
                }
            }

            if (Settings.Contains(ArticleConstants.NEWS_SEARCH_MODULE_ID))
            {
                if (Numeric.IsNumeric(Settings[ArticleConstants.NEWS_SEARCH_MODULE_ID].ToString()))
                {
                    _articleModuleID = Convert.ToInt32(Settings[ArticleConstants.NEWS_SEARCH_MODULE_ID].ToString());
                    if (_articleModuleID != Null.NullInteger)
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        private void ReadQueryString()
        {

            if (Request["courses_data"] != null && Request["courses_data"] != "")
            {
                courses_data = Server.UrlDecode(Request["courses_data"]);
            }
            if (Request["organization_data"] != null && Request["organization_data"] != "")
            {
                organization_data = Server.UrlDecode(Request["organization_data"]);
            }
            if (Request["postback"] != null && Request["postback"] != "")
            {
                postback = bool.Parse(Server.UrlDecode(Request["postback"]));
            }

            if (Request.QueryString["lapg-" + this.TabModuleId.ToString()] != null)
            {
                _pageNumber = Convert.ToInt32(Request.QueryString["lapg-" + this.TabModuleId.ToString()]);
                if (postback)
                {
                    _pageNumber = 1;
                }
            }

        }

        private void BindListView()
        {
            CategoryController controller = new CategoryController();
            courses = controller.GetCategory(48, _articleModuleID);
            List<CategoryInfo> lstCourses = controller.GetCategoriesAll(_articleModuleID, 48, CategorySortType.SortOrder);
            lvwCourses.DataSource = lstCourses;
            lvwCourses.DataBind();

            organization = controller.GetCategory(49, _articleModuleID);
            List<CategoryInfo> lstOrganization = controller.GetCategoriesAll(_articleModuleID, 49, CategorySortType.SortOrder);
            lvwOrganization.DataSource = lstOrganization;
            lvwOrganization.DataBind();


        }

        private void BindArticles()
        {

            ArticleController objArticleController = new ArticleController();
            CategoryController objCategoryController = new CategoryController();

            //要显示的栏目ID 包括只栏目
            int[] cats = null;
            List<int> lstCats = new List<int>();
            int[] categories=  new int[] { int.Parse(courses_data), int.Parse(organization_data) };
            foreach (int categoryId in categories)
            {
                lstCats.Add(categoryId);
                foreach (CategoryInfo category in objCategoryController.GetCategories(_articleModuleID, categoryId))
                {
                    lstCats.Add(category.CategoryID);
                }
            }
            cats = lstCats.ToArray();

            //排除的栏目
            int[] catsExclude = null;


            
            List<CategoryInfo> objCategories = objCategoryController.GetCategoriesAll(_articleModuleID, Null.NullInteger, ArticleSettings.CategorySortType);

            List<int> excludeCategoriesRestrictive = new List<int>();
            //受限制的栏目 且 没权限访问 
            foreach (CategoryInfo objCategory in objCategories)
            {
                if (!objCategory.InheritSecurity && objCategory.CategorySecurityType == CategorySecurityType.Restrict)
                {
                    if (Request.IsAuthenticated)
                    {
                        if (_settings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                        {
                            if (!PortalSecurity.IsInRoles(_settings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString()))
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

            MatchOperatorType matchOperator = MatchOperatorType.MatchAll;

            bool matchAll = false;
            if (matchOperator == MatchOperatorType.MatchAll)
            {
                matchAll = true;
            }

            int count = 1000;


            DateTime startDate = Null.NullDate;
            startDate = Convert.ToDateTime("2010-01-01 00:00:00");

            DateTime endDate = Null.NullDate;
            endDate = Convert.ToDateTime("2118-01-01 00:00:00");



            bool featuredOnly = false;



            bool notFeaturedOnly = false;


            bool securedOnly = false;


            bool notSecuredOnly = false;



            string sort = SortBy;


            string sortDirection = "ASC";



            _layoutMode = ArticleConstants.LATEST_ARTICLES_LAYOUT_MODE_DEFAULT;


            int itemsPerRow = ArticleConstants.LATEST_ARTICLES_ITEMS_PER_ROW_DEFAULT;


            bool showPending = false;


            int authorID = ArticleConstants.LATEST_ARTICLES_AUTHOR_DEFAULT;


            string userIDFilter = "";


            string usernameFilter = "";






            bool bubbleFeatured = false;


            string articleIDs = "";

            int[] tagIDs = null;


            MatchOperatorType matchOperatorTags = MatchOperatorType.MatchAny;


            bool matchAllTags = false;
            if (tagIDs != null)
            {
                if (matchOperatorTags == MatchOperatorType.MatchAll)
                {
                    matchAllTags = true;
                }
            }

            int customFieldID = Null.NullInteger;
            string customValue = Null.NullString;


            string linkFilter = Null.NullString;


            bool doPaging = true;


            int pageSize =12;

            ModuleController objModuleController = new ModuleController();
            ModuleInfo objModule = objModuleController.GetModule(_articleModuleID, _articleTabID);
            _objLayoutController = new LayoutController(PortalSettings, ArticleSettings, objModule, Page);
            LatestLayoutController objLatestLayoutController = new LatestLayoutController();


            int articleCount = 0;

            List<ArticleInfo> objArticles;

            if (Session["Course-" + TabModuleId.ToString()] == null || postback||(!IsPostBack&&_pageNumber==1))
            {
                objArticles = objArticleController.GetSearchArticleList1(_articleModuleID, endDate, startDate, cats, matchAll,1, catsExclude, count, 1, 1000, sort, sortDirection, true, false, Null.NullString, authorID, showPending, true, featuredOnly, notFeaturedOnly, securedOnly, notSecuredOnly, articleIDs, tagIDs, matchAllTags, Null.NullString, customFieldID, customValue, linkFilter, ref articleCount);
                Session["Course-" + TabModuleId.ToString()] = objArticles;
            }
            else
            {
                objArticles = (List<ArticleInfo>)Session["Course-" + TabModuleId.ToString()];
            }

            List<ArticleInfo> results = (from item in objArticles select item).Skip((_pageNumber - 1) * pageSize).Take(pageSize).ToList();

            rptLatestArticles.DataSource = results;
            rptLatestArticles.DataBind();
            rptLatestArticles.Visible = (results.Count > 0);


            int pageCount = ((objArticles.Count - 1) / pageSize) + 1;

            if ((objArticles.Count > 0) && pageCount > 1)
            {
                ctlPagingControl.Visible = true;
                ctlPagingControl.PageParam = "lapg-" + this.TabModuleId.ToString();
                ctlPagingControl.TotalRecords = objArticles.Count;
                ctlPagingControl.PageSize = pageSize;
                ctlPagingControl.CurrentPage = _pageNumber;
                ctlPagingControl.TabID = TabId;
                ctlPagingControl.EnableViewState = false;
            }

            if (objArticles.Count == 0)
            {
                LayoutInfo objNoArticles = objLatestLayoutController.GetLayout(LatestLayoutType.Listing_Empty_Html, ModuleId, Settings);

            }

        }


        public string GenerateImageUrl(object article)
        {
            ArticleInfo objArticle = (ArticleInfo)article;
            ImageController objImageController = new ImageController();
            List<ImageInfo> objImages = objImageController.GetImageList(objArticle.ArticleID, Null.NullString);

            if (objImages.Count > 0)
            {
                return PortalSettings.HomeDirectory + objImages[0].Folder + objImages[0].FileName;
            }
            return PortalSettings.HomeDirectory + "NoImage.jpg";
        }

        public string GenerateArticleUrl(object article)
        {
            ArticleInfo objArticle = (ArticleInfo)article;
            Literal objLiteral_Link = new Literal();
            if (objArticle.Url == "")
            {
                int pageID = Null.NullInteger;
                if (ArticleSettings.AlwaysShowPageID)
                {
                    if (Pages(objArticle.ArticleID).Count > 0)
                    {
                        pageID = ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID;
                    }
                }
                return Common.GetArticleLink(objArticle, Tab, ArticleSettings,false, pageID);
            }
            else
            {
                return Globals.LinkClick(objArticle.Url, Tab.TabID, objArticle.ModuleID, false);
            }
        }

        #endregion
    }
}