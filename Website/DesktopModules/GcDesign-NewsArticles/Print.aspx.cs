using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace GcDesign.NewsArticles
{
    public partial class Print : PageBase
    {

        #region " private Members "

        private int _articleID = Null.NullInteger;
        private int _moduleID = Null.NullInteger;
        private int _tabModuleID = Null.NullInteger;
        private int _tabID = Null.NullInteger;
        private int _pageID = Null.NullInteger;
        private int _portalID = Null.NullInteger;
        private string _template = Null.NullString;

        private ArticleSettings _articleSettings;

        #endregion

        #region " private Methods "

        private void ReadQueryString()
        {

            if (Request["ArticleID"] != null)
            {
                _articleID = Convert.ToInt32(Request["ArticleID"]);
            }

            if (Request["ModuleID"] != null)
            {
                _moduleID = Convert.ToInt32(Request["ModuleID"]);
            }

            if (Request["TabID"] != null)
            {
                _tabID = Convert.ToInt32(Request["TabID"]);
            }

            if (Request["TabModuleID"] != null)
            {
                _tabModuleID = Convert.ToInt32(Request["TabModuleID"]);
            }

            if (Request["PageID"] != null)
            {
                _pageID = Convert.ToInt32(Request["PageID"]);
            }

            if (Request["PortalID"] != null)
            {
                _portalID = Convert.ToInt32(Request["PortalID"]);
            }

        }

        private void ManageStyleSheets(bool PortalCSS)
        {

            // initialize reference paths to load the cascading style sheets
            // 初始化引用路径 来加载级联样式表
            Control objCSS = this.FindControl("CSS");
            HtmlGenericControl objLink;
            string ID;

            Hashtable objCSSCache = (Hashtable)DataCache.GetCache("CSS");
            if (objCSSCache == null)
            {
                objCSSCache = new Hashtable();
            }

            if (objCSS != null)
            {
                if (!PortalCSS)
                {
                    // module style sheet
                    ID = Globals.CreateValidID("PropertyAgent");
                    objLink = new HtmlGenericControl("link");
                    objLink.ID = ID;
                    objLink.Attributes["rel"] = "stylesheet";
                    objLink.Attributes["type"] = "text/css";
                    objLink.Attributes["href"] = this.ResolveUrl("module.css");
                    objCSS.Controls.Add(objLink);

                    // default style sheet ( required )
                    ID = Globals.CreateValidID(DotNetNuke.Common.Globals.HostPath);
                    objLink = new HtmlGenericControl("link");
                    objLink.ID = ID;
                    objLink.Attributes["rel"] = "stylesheet";
                    objLink.Attributes["type"] = "text/css";
                    objLink.Attributes["href"] = DotNetNuke.Common.Globals.HostPath + "default.css";
                    objCSS.Controls.Add(objLink);

                    // skin package style sheet
                    ID = Globals.CreateValidID(PortalSettings.ActiveTab.SkinPath);
                    if (!objCSSCache.ContainsKey(ID))
                    {
                        if (File.Exists(Server.MapPath(PortalSettings.ActiveTab.SkinPath) + "skin.css"))
                        {
                            objCSSCache[ID] = PortalSettings.ActiveTab.SkinPath + "skin.css";
                        }
                        else
                        {
                            objCSSCache[ID] = "";
                        }
                        if (DotNetNuke.Common.Globals.PerformanceSetting != DotNetNuke.Common.Globals.PerformanceSettings.NoCaching)
                        {
                            DataCache.SetCache("CSS", objCSSCache);
                        }
                    }
                    if (objCSSCache[ID].ToString() != "")
                    {
                        objLink = new HtmlGenericControl("link");
                        objLink.ID = ID;
                        objLink.Attributes["rel"] = "stylesheet";
                        objLink.Attributes["type"] = "text/css";
                        objLink.Attributes["href"] = objCSSCache[ID].ToString();
                        objCSS.Controls.Add(objLink);
                    }

                    // skin file style sheet
                    ID = Globals.CreateValidID(PortalSettings.ActiveTab.SkinSrc.Replace(".ascx", ".css"));
                    if (!objCSSCache.ContainsKey(ID))
                    {
                        if (File.Exists(Server.MapPath(PortalSettings.ActiveTab.SkinSrc.Replace(".ascx", ".css"))))
                        {
                            objCSSCache[ID] = PortalSettings.ActiveTab.SkinSrc.Replace(".ascx", ".css");
                        }
                        else
                        {
                            objCSSCache[ID] = "";
                        }
                        if (DotNetNuke.Common.Globals.PerformanceSetting != DotNetNuke.Common.Globals.PerformanceSettings.NoCaching)
                        {
                            DataCache.SetCache("CSS", objCSSCache);
                        }
                    }
                    if (objCSSCache[ID].ToString() != "")
                    {
                        objLink = new HtmlGenericControl("link");
                        objLink.ID = ID;
                        objLink.Attributes["rel"] = "stylesheet";
                        objLink.Attributes["type"] = "text/css";
                        objLink.Attributes["href"] = objCSSCache[ID].ToString();
                        objCSS.Controls.Add(objLink);
                    }
                }
                else
                {
                    // portal style sheet
                    ID = Globals.CreateValidID(PortalSettings.HomeDirectory);
                    objLink = new HtmlGenericControl("link");
                    objLink.ID = ID;
                    objLink.Attributes["rel"] = "stylesheet";
                    objLink.Attributes["type"] = "text/css";
                    objLink.Attributes["href"] = PortalSettings.HomeDirectory + "portal.css";
                    objCSS.Controls.Add(objLink);
                }

            }

        }

        private void BindArticle()
        {

            if (_articleID == Null.NullInteger)
            {
                Response.Redirect(Globals.NavigateURL(_tabID), true);
            }

            ArticleController objArticleController = new ArticleController();
            ArticleInfo objArticle = objArticleController.GetArticle(_articleID);

            if (objArticle != null)
            {

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
                                Response.Redirect(ArticleSettings.SecureUrl + "&returnurl=" + Server.UrlEncode(url), true);
                            }
                            else
                            {
                                Response.Redirect(ArticleSettings.SecureUrl + "?returnurl=" + Server.UrlEncode(url), true);
                            }
                        }
                        else
                        {
                            Response.Redirect(Globals.NavigateURL(_tabID), true);
                        }
                    }
                }

                ModuleController objModuleController = new ModuleController();
                ModuleInfo objModule = objModuleController.GetModule(objArticle.ModuleID, _tabID);

                if (objModule != null)
                {
                    if (!DotNetNuke.Security.PortalSecurity.IsInRoles(objModule.AuthorizedViewRoles))
                    {
                        Response.Redirect(Globals.NavigateURL(_tabID), true);
                    }

                    if (objModule.PortalID != PortalSettings.PortalId)
                    {
                        Response.Redirect(Globals.NavigateURL(_tabID), true);
                    }
                }

                LayoutController objLayoutController = new LayoutController(PortalSettings, ArticleSettings, objModule, Page);

                //Dim objLayoutController As New LayoutController(PortalSettings, ArticleSettings, Page, False, _tabID, _moduleID, _tabModuleID, _portalID, _pageID, Null.NullInteger, "Articles-Print-" + _moduleID.ToString())
                LayoutInfo objLayoutItem = LayoutController.GetLayout(ArticleSettings, objModule, Page, LayoutType.Print_Item_Html);
                objLayoutController.ProcessArticleItem(phArticle.Controls, objLayoutItem.Tokens, objArticle);
                objLayoutController.LoadStyleSheet(ArticleSettings.Template);

                LayoutInfo objLayoutTitle = LayoutController.GetLayout(ArticleSettings, objModule, Page, LayoutType.View_Title_Html);
                if (objLayoutTitle.Template != "")
                {
                    PlaceHolder phPageTitle = new PlaceHolder();
                    objLayoutController.ProcessArticleItem(phPageTitle.Controls, objLayoutTitle.Tokens, objArticle);
                    this.Title = RenderControlToString(phPageTitle);
                }

                LayoutInfo objLayoutDescription = LayoutController.GetLayout(ArticleSettings, objModule, Page, LayoutType.View_Description_Html);
                if (objLayoutDescription.Template != "")
                {
                    PlaceHolder phPageDescription = new PlaceHolder();
                    objLayoutController.ProcessArticleItem(phPageDescription.Controls, objLayoutDescription.Tokens, objArticle);
                    HtmlMeta meta = new HtmlMeta();
                    meta.Name = "MetaDescription";
                    meta.Content = RenderControlToString(phPageDescription);
                    if (meta.Content != "")
                    {
                        this.Header.Controls.Add(meta);
                    }
                }

                LayoutInfo objLayoutKeyword = LayoutController.GetLayout(ArticleSettings, objModule, Page, LayoutType.View_Keyword_Html);
                if (objLayoutKeyword.Template != "")
                {
                    PlaceHolder phPageKeyword = new PlaceHolder();
                    objLayoutController.ProcessArticleItem(phPageKeyword.Controls, objLayoutKeyword.Tokens, objArticle);

                    HtmlMeta meta = new HtmlMeta();
                    meta.Name = "MetaKeywords";
                    meta.Content = RenderControlToString(phPageKeyword);
                    if (meta.Content != "")
                    {
                        this.Header.Controls.Add(meta);
                    }
                }

            }
            else
            {

                Response.Redirect(Globals.NavigateURL(), true);

            }

        }

        protected string GetSharedResource(string key)
        {

            string path = this.TemplateSourceDirectory + "/" + DotNetNuke.Services.Localization.Localization.LocalResourceDirectory + "/" + DotNetNuke.Services.Localization.Localization.LocalSharedResourceFile;
            path = "~" + path.Substring(path.IndexOf("/DesktopModules/"), path.Length - path.IndexOf("/DesktopModules/"));
            return DotNetNuke.Services.Localization.Localization.GetString(key, path);

        }

        private string RenderControlToString(Control ctrl)
        {

            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);

            ctrl.RenderControl(hw);

            return sb.ToString();

        }

        protected string StripHtml(string html)
        {

            string pattern = "<(.|\n)*?>";
            return Regex.Replace(html, pattern, String.Empty);

        }

        #endregion

        #region " private Properties "

        public DotNetNuke.Framework.CDefault BasePage
        {
            get
            {
                return (DotNetNuke.Framework.CDefault)this.Page;
            }
        }

        public ArticleSettings ArticleSettings
        {
            get
            {
                if (_articleSettings == null)
                {
                    ModuleController objModuleController = new ModuleController();
                    Hashtable settings = objModuleController.GetModuleSettings(_moduleID);
                    //Add TabModule Settings
                    settings = DotNetNuke.Entities.Portals.PortalSettings.GetTabModuleSettings(_tabModuleID, settings);
                    ModuleInfo objModule = objModuleController.GetModule(_moduleID, _tabID);
                    _articleSettings = new ArticleSettings(settings, this.PortalSettings, objModule);
                }
                return _articleSettings;
            }
        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Init += Page_Initialization;
            this.Load += new EventHandler(this.Page_Load);
            
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Initialization(object sender, EventArgs e)
        {

            ManageStyleSheets(false);
            ManageStyleSheets(true);
            ReadQueryString();
            BindArticle();

        }

        private void Page_Load(object sender, EventArgs e)
        {


        }

        #endregion
    }
}