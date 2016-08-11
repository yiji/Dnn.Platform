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
    public partial class GcSearchShow : NewsArticleModuleBase
    {
        #region " Constants "

        private const string PARAM_SEARCH_ID = "Search";

        #endregion

        #region " private Members "

        private int _articleTabID = Null.NullInteger;
        private int _articleModuleID = Null.NullInteger;

        private string courses_data = "58";
        private string place_data = "59";
        private string time_data = "year";
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
        public CategoryInfo place;

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

            lvwTime.ItemDataBound += lvwTime_ItemDataBound;

            rptLatestArticles.ItemDataBound += rptLatestArticles_ItemDataBound;
        }

        void rptLatestArticles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Control control = e.Item.FindControl("desWrap");

                ArticleInfoDes(e.Item.DataItem, ref control);
            }
        }

        void lvwTime_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            DateTime time = (DateTime)e.Item.DataItem;
            //e.Item.ItemType==ListItemType.AlternatingItem
            Literal ltr = new Literal();
            ltr.Text = string.Format("<a href='javascript:void(0)' data-value='{0}'>{1}</a>", time.ToString("yyyy-MM"), time.ToString("yyyy-MM"));
            e.Item.Controls.Add(ltr);
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
                    string json = "$link$" + Common.GetModuleLink(_articleTabID, _articleModuleID, "Search", ArticleSettings, "courses_data=" + courses_data, "time_data=" + time_data) + "$link$";
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
            if (Request["place_data"] != null && Request["place_data"] != "")
            {
                place_data = Server.UrlDecode(Request["place_data"]);
            }
            if (Request["time_data"] != null && Request["time_data"] != "")
            {
                time_data = Server.UrlDecode(Request["time_data"]);
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
            courses = controller.GetCategory(2, _articleModuleID);
            List<CategoryInfo> lstCourses = controller.GetCategoriesAll(_articleModuleID, 58, CategorySortType.SortOrder);
            lvwCourses.DataSource = lstCourses;
            lvwCourses.DataBind();


            place = controller.GetCategory(6, _articleModuleID);
            List<CategoryInfo> lstPlace = controller.GetCategoriesAll(_articleModuleID, 59, CategorySortType.SortOrder);
            lvwPlace.DataSource = lstPlace;
            lvwPlace.DataBind();

            DateTime time = DateTime.Now;
            List<DateTime> lstTime = new List<DateTime>();
            for (int i = 0; i < 12; i++)
            {
                lstTime.Add(DateTime.Now.AddMonths(i));

            }
            lvwTime.DataSource = lstTime;
            lvwTime.DataBind();
        }

        private void BindArticles()
        {

            ArticleController objArticleController = new ArticleController();
            CategoryController objCategoryController = new CategoryController();

            //要显示的栏目ID 包括只栏目
            int[] cats = null;
            List<int> lstCats = new List<int>();
            int[] categories=  new int[] { int.Parse(courses_data), int.Parse(place_data) };
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


            DateTime startDate = DateTime.Now;

            DateTime endDate = Null.NullDate;

            if (time_data == "year")
            {
                startDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-01 00:00:00"));
                endDate = Convert.ToDateTime(DateTime.Now.AddYears(1).ToString("yyyy-MM-01 00:00:00"));
            }
            else
            {
                DateTime time = Convert.ToDateTime(time_data);
                startDate = Convert.ToDateTime(time.ToString("yyyy-MM-01 00:00:00"));
                endDate=Convert.ToDateTime(time.AddMonths(1).ToString("yyyy-MM-01 00:00:00"));
            }

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

            int pageSize = 12;

            ModuleController objModuleController = new ModuleController();
            ModuleInfo objModule = objModuleController.GetModule(_articleModuleID, _articleTabID);
            _objLayoutController = new LayoutController(PortalSettings, ArticleSettings, objModule, Page);
            LatestLayoutController objLatestLayoutController = new LatestLayoutController();

            int articleCount = 0;
            List<ArticleInfo> objArticles;

            if (Session["Show-" + TabModuleId.ToString()] == null || postback || (!IsPostBack && _pageNumber == 1))
            {
                objArticles = objArticleController.GetSearchArticleList1(_articleModuleID, endDate, startDate, cats, matchAll, 1, catsExclude, count, _pageNumber, pageSize, sort, sortDirection, true, false, Null.NullString, authorID, showPending, true, featuredOnly, notFeaturedOnly, securedOnly, notSecuredOnly, articleIDs, tagIDs, matchAllTags, Null.NullString, customFieldID, customValue, linkFilter, ref articleCount);
                Session["Show-" + TabModuleId.ToString()] = objArticles;
            }
            else
            {
                objArticles = (List<ArticleInfo>)Session["Show-" + TabModuleId.ToString()];
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
                return Common.GetArticleLink(objArticle, Tab, ArticleSettings, false, pageID);
            }
            else
            {
                return Globals.LinkClick(objArticle.Url, Tab.TabID, objArticle.ModuleID, false);
            }
        }

        public void ArticleInfoDes(object article, ref Control control)
        {
            ArticleInfo objArticle = (ArticleInfo)article;
            Panel div = new Panel();
            ArticleController objController = new ArticleController();
            ArrayList categories = objController.GetArticleCategories(objArticle.ArticleID);

            CategoryController objCategoryController = new CategoryController();

            Dictionary<int, Control> controls = new Dictionary<int, Control>();
            foreach (object obj in categories)
            {

                CategoryInfo category = objCategoryController.GetCategory(((CategoryInfo)obj).CategoryID, _articleModuleID);

                if (category.ParentID == 58)
                {
                    Literal organization = new Literal();
                    organization.Text = string.Format("<p>展会分类：{0}</p>", category.Name);
                    controls.Add(1, organization);

                }
                if (category.ParentID == 59)
                {
                    Literal place = new Literal();
                    place.Text = string.Format("<p>地点：{0}</p>", category.Name);
                    controls.Add(2, place);

                }
            }

            if (controls.Count == 2)
            {
                div.Controls.Add(controls[1]);
                div.Controls.Add(controls[2]);
            }

            Literal time = new Literal();
            time.Text = string.Format("<p>时间：{0}至{1}</p>", objArticle.StartDate.ToString("yyyy-MM-dd"), objArticle.EndDate.AddDays(-1).ToString("yyyy-MM-dd"));
            div.Controls.Add(time);

            control.Controls.Add(div);
        }

        #endregion
    }
}