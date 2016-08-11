using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Security;

using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class Archives : NewsArticleModuleBase
    {
        #region " protected Properties "

        protected int AuthorID
        {
            get
            {
                int id = Null.NullInteger;
                if (ArticleSettings.AuthorUserIDFilter)
                {
                    if (Request.QueryString[ArticleSettings.AuthorUserIDParam] != null)
                    {
                        try
                        {
                            id = Convert.ToInt32(Request.QueryString[ArticleSettings.AuthorUserIDParam]);
                        }
                        catch
                        { }
                    }
                }

                if (ArticleSettings.AuthorUsernameFilter)
                {
                    if (Request.QueryString[ArticleSettings.AuthorUsernameParam] != null)
                    {
                        try
                        {
                            DotNetNuke.Entities.Users.UserInfo objUser = DotNetNuke.Entities.Users.UserController.GetUserByName(this.PortalId, Request.QueryString[ArticleSettings.AuthorUsernameParam]);
                            if (objUser != null)
                            {
                                id = objUser.UserID;
                            }
                        }
                        catch
                        {

                        }
                    }
                }

                if (ArticleSettings.AuthorLoggedInUserFilter)
                {
                    if (Request.IsAuthenticated)
                    {
                        id = this.UserId;
                    }
                    else
                    {
                        id = -100;
                    }
                }

                if (ArticleSettings.Author != Null.NullInteger)
                {
                    id = ArticleSettings.Author;
                }

                return id;
            }
        }

        protected string AuthorIDRSS
        {
            get
            {
                if (AuthorID != Null.NullInteger)
                {
                    return "&amp;AuthorID=" + AuthorID.ToString();
                }
                return "";
            }
        }

        #endregion

        #region " protected Methods "

        protected string GetAuthorLink(int authorID, string username)
        {

            return Common.GetAuthorLink(this.TabId, this.ModuleId, authorID, username, ArticleSettings.LaunchLinks, ArticleSettings);

        }

        protected string GetAuthorLinkRss(string authorID)
        {

            return Page.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/RSS.aspx?TabID=" + TabId.ToString() + "&amp;ModuleID=" + ModuleId.ToString() + "&amp;AuthorID=" + authorID);

        }

        protected string GetCategoryLink(string categoryID, string name)
        {

            return Common.GetCategoryLink(this.TabId, this.ModuleId, categoryID, name, ArticleSettings.LaunchLinks, ArticleSettings);

        }

        protected string GetCategoryLinkRss(string categoryID)
        {

            return Page.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/RSS.aspx?TabID=" + TabId.ToString() + "&amp;ModuleID=" + ModuleId.ToString() + "&amp;CategoryID=" + categoryID + AuthorIDRSS);

        }

        protected string GetCurrentLinkRss(){

            return Page.ResolveUrl("~/DesktopModules/GcDesign-NewsArticles/RSS.aspx?TabID=" + TabId.ToString() + "&amp;ModuleID=" + ModuleId.ToString() + "&amp;MaxCount=25" + AuthorIDRSS);

        }

        protected string GetCurrentLink()
        {

            return Common.GetModuleLink(TabId, ModuleId, "", ArticleSettings);

        }

        protected string GetMonthLink(int month, int year)
        {

            return Common.GetModuleLink(this.TabId, this.ModuleId, "ArchiveView", ArticleSettings, "month=" + month.ToString(), "year=" + year.ToString());

        }

        protected string GetMonthLinkRss(int month, int year)
        {

            return Page.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/RSS.aspx?TabID=" + TabId.ToString() + "&amp;ModuleID=" + ModuleId.ToString() + "&amp;Month=" + month.ToString() + "&amp;Year=" + year.ToString() + AuthorIDRSS);

        }

        protected string GetMonthName(int month)
        {

            DateTime dt = new DateTime(2008, month, 1);
            return dt.ToString("MMMM");

        }

        protected string GetRssPath()
        {

            return Page.ResolveUrl(ArticleSettings.SyndicationImagePath);

        }

        protected bool IsSyndicationEnabled()
        {

            return ArticleSettings.IsSyndicationEnabled;

        }

        #endregion

        #region " Event Handlers "


        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            this.PreRender += new EventHandler(Page_PreRender);
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }


        protected void Page_Load(object sender, EventArgs e){

            try{

                CategoryController objCategoryController = new CategoryController();
                if (ArticleSettings.FilterSingleCategory != Null.NullInteger) {
                    int[] categoriesToDisplay=new int[1];
                    categoriesToDisplay[0] = ArticleSettings.FilterSingleCategory;

                    rptCategories.DataSource = objCategoryController.GetCategoriesAll(this.ModuleId, Null.NullInteger, categoriesToDisplay, AuthorID, Null.NullInteger, ArticleSettings.ShowPending, ArticleSettings.CategorySortType);
                    rptCategories.DataBind();
                }
                else
                {
                    List<CategoryInfo> objCategoriesSelected = new List<CategoryInfo>();
                    List<CategoryInfo> objCategoriesDisplay = objCategoryController.GetCategoriesAll(this.ModuleId, Null.NullInteger, ArticleSettings.FilterCategories, AuthorID, Null.NullInteger, ArticleSettings.ShowPending, ArticleSettings.CategorySortType);
                    foreach(CategoryInfo objCategory in objCategoriesDisplay){
                        if (objCategory.InheritSecurity) {
                            objCategoriesSelected.Add(objCategory);
                        }
                        else
                        {
                            if (Request.IsAuthenticated) {
                                if (Settings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING)) {
                                    if (PortalSecurity.IsInRoles(Settings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString())) {
                                        objCategoriesSelected.Add(objCategory);
                                    }
                                }
                            }
                        }
                    }

                    rptCategories.DataSource = objCategoriesSelected;
                    rptCategories.DataBind();
                }

                List<CategoryInfo> objCategories = objCategoryController.GetCategoriesAll(ModuleId, Null.NullInteger);

                List<int> excludeCategoriesRestrictive = new List<int>();

                foreach(CategoryInfo objCategory in objCategories){
                    if (!objCategory.InheritSecurity && objCategory.CategorySecurityType == CategorySecurityType.Restrict) {
                        if (Request.IsAuthenticated) {
                            if (Settings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING)) {
                                if (!PortalSecurity.IsInRoles(Settings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString())) {
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

                foreach(CategoryInfo objCategory in objCategories){
                    if (!objCategory.InheritSecurity && objCategory.CategorySecurityType == CategorySecurityType.Loose) {
                        if (Request.IsAuthenticated) {
                            if (Settings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING)) {
                                if (!PortalSecurity.IsInRoles(Settings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString())) {
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

                int[] filterCategories = ArticleSettings.FilterCategories;
                List<int> includeCategories = new List<int>();

                if (excludeCategories.Count > 0) {

                    foreach(CategoryInfo objCategoryToInclude in objCategories){

                        bool includeCategory = true;

                        foreach(int exclCategory in excludeCategories){
                            if (exclCategory == objCategoryToInclude.CategoryID) {
                                includeCategory = false;
                            }
                        }

                        if (ArticleSettings.FilterCategories != null) {
                            if (ArticleSettings.FilterCategories.Length > 0) {
                                bool filter = false;
                                foreach(int cat in ArticleSettings.FilterCategories){
                                    if (cat == objCategoryToInclude.CategoryID) {
                                        filter = true;
                                    }
                                }
                                if (!filter) {
                                    includeCategory = false;
                                }
                            }
                        }

                        if (includeCategory) {
                            includeCategories.Add(objCategoryToInclude.CategoryID);
                        }

                    }

                    if (includeCategories.Count > 0) {
                        includeCategories.Add(-1);
                    }

                    filterCategories = includeCategories.ToArray();

                }


                if (ArticleSettings.FilterSingleCategory != Null.NullInteger) {
                    int[] categoriesToDisplay=new int[1];
                    categoriesToDisplay[0] = ArticleSettings.FilterSingleCategory;

                    AuthorController objAuthorController = new AuthorController();
                    rptAuthors.DataSource = objAuthorController.GetAuthorStatistics(this.ModuleId, categoriesToDisplay, excludeCategoriesRestrictive.ToArray(), AuthorID, "DisplayName", ArticleSettings.ShowPending);
                    rptAuthors.DataBind();
                }
                else
                {
                    AuthorController objAuthorController = new AuthorController();
                    rptAuthors.DataSource = objAuthorController.GetAuthorStatistics(this.ModuleId, filterCategories, excludeCategoriesRestrictive.ToArray(), AuthorID, "DisplayName", ArticleSettings.ShowPending);
                    rptAuthors.DataBind();
                }

                if (ArticleSettings.FilterSingleCategory != Null.NullInteger) {
                    int[] categoriesToDisplay = new int[1];
                    categoriesToDisplay[0] = ArticleSettings.FilterSingleCategory;
                    ArticleController objArticleController = new ArticleController();
                    rptMonth.DataSource = objArticleController.GetNewsArchive(this.ModuleId, categoriesToDisplay, excludeCategoriesRestrictive.ToArray(), AuthorID, GroupByType.Month, ArticleSettings.ShowPending);
                    rptMonth.DataBind();
                    }
                else
                    {
                    ArticleController objArticleController = new ArticleController();
                    rptMonth.DataSource = objArticleController.GetNewsArchive(this.ModuleId, filterCategories, excludeCategoriesRestrictive.ToArray(), AuthorID, GroupByType.Month, ArticleSettings.ShowPending);
                    rptMonth.DataBind();
                }

                this.BasePage.Title = "Archives | " + this.BasePage.Title;
}
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            try
            {

                LoadStyleSheet();

                phCurrentArticles.Visible = ArticleSettings.ArchiveCurrentArticles;
                phCategory.Visible = ArticleSettings.ArchiveCategories;
                phAuthor.Visible = ArticleSettings.ArchiveAuthor;
                phMonth.Visible = ArticleSettings.ArchiveMonth;

                phArchives.Visible = IsSyndicationEnabled();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}