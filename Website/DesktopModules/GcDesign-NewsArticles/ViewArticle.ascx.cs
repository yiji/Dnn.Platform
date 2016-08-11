using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using GcDesign.NewsArticles.Components.Common;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;

using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

using System.Text;
using GcDesign.NewsArticles.Base;
using System.Collections;
using System.IO;
using DotNetNuke.Entities.Tabs;

namespace GcDesign.NewsArticles
{
    public partial class ViewArticle : NewsArticleModuleBase
    {

        #region " private Members "

        private int _articleID = Null.NullInteger;
        private int _pageID = Null.NullInteger;

        #endregion

        #region " private Methods "

        private void BindArticle()
        {

            if (_articleID == Null.NullInteger)
            {
                Response.Redirect(Globals.NavigateURL(), true);
            }

            ArticleController objArticleController = new ArticleController();
            ArticleInfo objArticle = objArticleController.GetArticle(_articleID);

            if (objArticle == null)
            {
                // Article doesn't exist.
                Response.Redirect(Globals.NavigateURL(), true);
            }

            bool includeCategory = false;
            if (ArticleSettings.CategoryBreadcrumb && Request["CategoryID"] != null)
            {
                includeCategory = true;
            }

            string targetUrl = "";
            if (_pageID != Null.NullInteger)
            {
                PageController objPageController = new PageController();
                ArrayList objPages = objPageController.GetPageList(objArticle.ArticleID);

                bool pageFound = false;
                foreach (PageInfo objPage in objPages)
                {
                    if (objPage.PageID == _pageID)
                    {
                        pageFound = true;
                    }
                }
                if (!pageFound)
                {
                    // redirect
                    Response.Status = "301 Moved Permanently";
                    Response.AddHeader("Location", Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false));
                    Response.End();
                }

                targetUrl = Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false, "PageID=" + _pageID.ToString());
            }
            else
            {
                targetUrl = Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false);
            }

            if (ArticleSettings.UseCanonicalLink)
            {
                Literal litCanonical = new Literal();
                litCanonical.Text = "<link rel=\"canonical\" href=\"" + targetUrl + "\"/>";
                this.BasePage.Header.Controls.Add(litCanonical);
            }

            if (objArticle.ModuleID != this.ModuleId)
            {
                // Article in the wrong ModuleID
                Response.Redirect(Globals.NavigateURL(), true);
            }

            // Check Article Security
            if (objArticle.IsSecure)
            {
                if (!ArticleSettings.IsSecureEnabled)
                {
                    if (ArticleSettings.SecureUrl != "")
                    {
                        string url = Request.Url.ToString().Replace(Globals.AddHTTP(Request.Url.Host), "");
                        if (ArticleSettings.SecureUrl.IndexOf("?") != -1)
                        {
                            Response.Redirect((ArticleSettings.SecureUrl + "&returnurl=" + Server.UrlEncode(url)).Replace("[ARTICLEID]", objArticle.ArticleID.ToString()), true);
                        }
                        else
                        {
                            Response.Redirect((ArticleSettings.SecureUrl + "?returnurl=" + Server.UrlEncode(url)).Replace("[ARTICLEID]", objArticle.ArticleID.ToString()), true);
                        }
                    }
                    else
                    {
                        Response.Redirect(Globals.NavigateURL(this.TabId), true);
                    }
                }
            }

            // Is Article Published?
            if (objArticle.Status == StatusType.AwaitingApproval || objArticle.Status == StatusType.Draft || (objArticle.StartDate > DateTime.Now && !ArticleSettings.ShowPending))
            {
                if (!ArticleSettings.IsAdmin && !ArticleSettings.IsApprover && this.UserId != objArticle.AuthorID)
                {
                    Response.Redirect(Globals.NavigateURL(this.TabId), true);
                }
            }

            if (objArticle.IsSecure)
            {
                if (ArticleSettings.SecureUrl != "")
                {
                    if (!objArticleController.SecureCheck(PortalId, _articleID, UserId) && !IsEditable && !UserInfo.IsSuperUser && !UserInfo.IsInRole("Administrators"))
                    {
                        string url = Request.Url.ToString().Replace(Globals.AddHTTP(Request.Url.Host), "");
                        if (ArticleSettings.SecureUrl.IndexOf("?") != -1)
                        {
                            Response.Redirect((ArticleSettings.SecureUrl + "&returnurl=" + Server.UrlEncode(url)).Replace("[ARTICLEID]", objArticle.ArticleID.ToString()), true);
                        }
                        else
                        {
                            Response.Redirect((ArticleSettings.SecureUrl + "?returnurl=" + Server.UrlEncode(url)).Replace("[ARTICLEID]", objArticle.ArticleID.ToString()), true);
                        }
                    }
                }
            }

            // Permission to view category?
            CategoryController objCategoryController = new CategoryController();
            List<CategoryInfo> objCategories = objCategoryController.GetCategoriesAll(ModuleId, Null.NullInteger);

            ArrayList objArticleCategories = objArticleController.GetArticleCategories(objArticle.ArticleID);
            foreach (CategoryInfo objArticleCategory in objArticleCategories)
            {
                foreach (CategoryInfo objCategory in objCategories)
                {
                    if (objCategory.CategoryID == objArticleCategory.CategoryID)
                    {
                        if (!objCategory.InheritSecurity)
                        {
                            if (Request.IsAuthenticated)
                            {

                                if (objCategory.CategorySecurityType == CategorySecurityType.Loose)
                                {
                                    bool doCheck = true;
                                    // Ensure there are no inherit security 
                                    foreach (CategoryInfo objCategoryOther in objArticleCategories)
                                    {
                                        foreach (CategoryInfo objCategoryOther2 in objCategories)
                                        {
                                            if (objCategoryOther.CategoryID == objCategoryOther2.CategoryID)
                                            {
                                                if (objCategoryOther2.InheritSecurity)
                                                {
                                                    doCheck = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (doCheck)
                                    {
                                        if (Settings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                                        {
                                            if (!PortalSecurity.IsInRoles(Settings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString()))
                                            {
                                                Response.Redirect(Globals.NavigateURL(this.TabId), true);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (Settings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                                    {
                                        if (!PortalSecurity.IsInRoles(Settings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString()))
                                        {
                                            Response.Redirect(Globals.NavigateURL(this.TabId), true);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (objCategory.CategorySecurityType == CategorySecurityType.Loose)
                                {
                                    bool doCheck = true;
                                    // Ensure there are no inherit security 
                                    foreach (CategoryInfo objCategoryOther in objArticleCategories)
                                    {
                                        foreach (CategoryInfo objCategoryOther2 in objCategories)
                                        {
                                            if (objCategoryOther.CategoryID == objCategoryOther2.CategoryID)
                                            {
                                                if (objCategoryOther2.InheritSecurity)
                                                {
                                                    doCheck = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (doCheck)
                                    {
                                        Response.Redirect(Globals.NavigateURL(this.TabId), true);
                                    }
                                }
                                else
                                {
                                    Response.Redirect(Globals.NavigateURL(this.TabId), true);
                                }
                            }
                        }
                    }
                }
            }

            // Check module security
            ModuleController objModuleController = new ModuleController();
            ModuleInfo objModule = objModuleController.GetModule(objArticle.ModuleID, this.TabId);

            if (objModule != null)
            {
                if (!DotNetNuke.Security.PortalSecurity.IsInRoles(objModule.AuthorizedViewRoles))
                {
                    Response.Redirect(Globals.NavigateURL(this.TabId), true);
                }
            }

            // Increment View Count
            HttpCookie cookie = Request.Cookies["Article" + _articleID.ToString()];
            if (cookie == null)
            {

                objArticle.NumberOfViews = objArticle.NumberOfViews + 1;
                objArticleController.UpdateArticleCount(objArticle.ArticleID, objArticle.NumberOfViews);

                cookie = new HttpCookie("Article" + _articleID.ToString());
                cookie.Value = "1";
                cookie.Expires = DateTime.Now.AddMinutes(20);
                Context.Response.Cookies.Add(cookie);

            }

            LayoutController objLayoutController = new LayoutController(this);
            if (ArticleSettings.CategoryBreadcrumb + Request["CategoryID"] != null)
            {
                objLayoutController.IncludeCategory = true;
            }

            LayoutInfo objLayoutItem = LayoutController.GetLayout(this, LayoutType.View_Item_Html);
            objLayoutController.ProcessArticleItem(phArticle.Controls, objLayoutItem.Tokens, objArticle);

            if (objArticle.MetaTitle != "")
            {
                this.BasePage.Title = objArticle.MetaTitle;
            }
            else
            {
                LayoutInfo objLayoutTitle = LayoutController.GetLayout(this, LayoutType.View_Title_Html);
                if (objLayoutTitle.Template != "")
                {
                    PlaceHolder phPageTitle = new PlaceHolder();
                    objLayoutController.ProcessArticleItem(phPageTitle.Controls, objLayoutTitle.Tokens, objArticle);
                    this.BasePage.Title = RenderControlToString(phPageTitle);
                }
            }

            if (ArticleSettings.UniquePageTitles)
            {
                if (_pageID != Null.NullInteger)
                {
                    this.BasePage.Title = this.BasePage.Title + " " + _pageID.ToString();
                }
            }

            if (objArticle.MetaDescription != "")
            {
                this.BasePage.Description = objArticle.MetaDescription;
            }
            else
            {
                LayoutInfo objLayoutDescription = LayoutController.GetLayout(this, LayoutType.View_Description_Html);
                if (objLayoutDescription.Template != "")
                {
                    PlaceHolder phPageDescription = new PlaceHolder();
                    objLayoutController.ProcessArticleItem(phPageDescription.Controls, objLayoutDescription.Tokens, objArticle);
                    this.BasePage.Description = RenderControlToString(phPageDescription);
                }
            }

            if (objArticle.MetaKeywords != "")
            {
                this.BasePage.KeyWords = objArticle.MetaKeywords;
            }
            else
            {
                LayoutInfo objLayoutKeyword = LayoutController.GetLayout(this, LayoutType.View_Keyword_Html);
                if (objLayoutKeyword.Template != "")
                {
                    PlaceHolder phPageKeyword = new PlaceHolder();
                    objLayoutController.ProcessArticleItem(phPageKeyword.Controls, objLayoutKeyword.Tokens, objArticle);
                    this.BasePage.KeyWords = RenderControlToString(phPageKeyword);
                }
            }

            if (objArticle.PageHeadText != "")
            {
                Literal litPageHeadText = new Literal();
                litPageHeadText.Text = objArticle.PageHeadText;
                this.BasePage.Header.Controls.Add(litPageHeadText);
            }

            LayoutInfo objLayoutPageHeader = LayoutController.GetLayout(this, LayoutType.View_PageHeader_Html);
            if (objLayoutPageHeader.Template != "")
            {
                PlaceHolder phPageHeaderTitle = new PlaceHolder();
                objLayoutController.ProcessArticleItem(phPageHeaderTitle.Controls, objLayoutPageHeader.Tokens, objArticle);
                this.BasePage.Header.Controls.Add(phPageHeaderTitle);
            }

        }

        private void ReadQueryString()
        {

            if (ArticleSettings.UrlModeType == Components.Types.UrlModeType.Shorterned)
            {
                try
                {
                    if (Numeric.IsNumeric(Request[ArticleSettings.ShortenedID]))
                    {
                        _articleID = Convert.ToInt32(Request[ArticleSettings.ShortenedID]);
                    }
                }
                catch
                { }
            }

            if (Numeric.IsNumeric(Request["ArticleID"]))
            {
                _articleID = Convert.ToInt32(Request["ArticleID"]);
            }

            if (Numeric.IsNumeric(Request["PageID"]))
            {
                _pageID = Convert.ToInt32(Request["PageID"]);
            }

        }

        private string RenderControlToString(Control ctrl)
        {

            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);

            ctrl.RenderControl(hw);

            return sb.ToString();

        }

        #endregion

        #region " protected Methods "

        protected string GetArticleID()
        {

            return _articleID.ToString();

        }

        protected string GetLocalizedValue(string key)
        {

            return Localization.GetString(key, this.LocalResourceFile);

        }

        #endregion

        #region " Event Handlers "


        protected void InitializeComponent()
        {
            this.Init += new EventHandler(this.Page_Init);
            this.PreRender += new EventHandler(Page_PreRender);
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Init(object sender, EventArgs e)
        {

            try
            {
                ReadQueryString();
                BindArticle();

                if (Request["CategoryID"] != null && Numeric.IsNumeric(Request["CategoryID"]))
                {
                    int _categoryID = Convert.ToInt32(Request["CategoryID"]);

                    CategoryController objCategoryController = new CategoryController();
                    List<CategoryInfo> objCategoriesAll = objCategoryController.GetCategoriesAll(this.ModuleId, Null.NullInteger, ArticleSettings.CategorySortType);

                    foreach (CategoryInfo objCategory in objCategoriesAll)
                    {
                        if (objCategory.CategoryID == _categoryID)
                        {

                            if (ArticleSettings.FilterSingleCategory == objCategory.CategoryID)
                            {
                                break;
                            }

                            string path = "";
                            if (ArticleSettings.CategoryBreadcrumb)
                            {
                                TabInfo objTab = new TabInfo();
                                objTab.TabName = objCategory.Name;
                                objTab.Url = Common.GetCategoryLink(TabId, ModuleId, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings);
                                PortalSettings.ActiveTab.BreadCrumbs.Add(objTab);

                                int parentID = objCategory.ParentID;
                                int parentCount = 0;

                                while (parentID != Null.NullInteger)
                                {
                                    foreach (CategoryInfo objParentCategory in objCategoriesAll)
                                    {
                                        if (objParentCategory.CategoryID == parentID)
                                        {
                                            if (ArticleSettings.FilterSingleCategory == objParentCategory.CategoryID)
                                            {
                                                parentID = Null.NullInteger;
                                                break;
                                            }
                                            TabInfo objParentTab = new DotNetNuke.Entities.Tabs.TabInfo();
                                            objParentTab.TabID = 10000 + objParentCategory.CategoryID;
                                            objParentTab.TabName = objParentCategory.Name;
                                            objParentTab.Url = Common.GetCategoryLink(TabId, ModuleId, objParentCategory.CategoryID.ToString(), objParentCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings);
                                            PortalSettings.ActiveTab.BreadCrumbs.Insert(PortalSettings.ActiveTab.BreadCrumbs.Count - 1 - parentCount, objParentTab);

                                            if (path.Length == 0)
                                            {
                                                path = " > " + objParentCategory.Name;
                                            }
                                            else
                                            {
                                                path = " > " + objParentCategory.Name + path;
                                            }

                                            parentCount = parentCount + 1;
                                            parentID = objParentCategory.ParentID;
                                        }
                                    }
                                }
                            }

                            if (ArticleSettings.IncludeInPageName)
                            {
                                HttpContext.Current.Items.Add("NA1-CategoryName", objCategory.Name);
                            }

                            break;
                        }
                    }
                }

                if (ArticleSettings.ArticleBreadcrumb)
                {
                    ArticleController objArticleController = new ArticleController();
                    ArticleInfo objArticle = objArticleController.GetArticle(_articleID);

                    TabInfo objTab = new TabInfo();
                    objTab.TabName = objArticle.Title;
                    objTab.Url = Request.RawUrl;
                    PortalSettings.ActiveTab.BreadCrumbs.Add(objTab);
                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void Page_PreRender(object sender, EventArgs e)
        {

            try
            {

                LoadStyleSheet();

                if (HttpContext.Current.Items.Contains("NA1-CategoryName"))
                {
                    PortalSettings.ActiveTab.TabName = HttpContext.Current.Items["NA1-CategoryName"].ToString();
                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}