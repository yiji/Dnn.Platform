using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class LatestComments : NewsArticleModuleBase
    {
        #region " private Members "

        private LayoutController _objLayoutController;

        private LayoutInfo _objLayoutHeader;
        private LayoutInfo _objLayoutItem;
        private LayoutInfo _objLayoutFooter;

        private int _articleTabID = Null.NullInteger;
        private DotNetNuke.Entities.Tabs.TabInfo _articleTabInfo;
        private int _articleModuleID = Null.NullInteger;
        private ArticleSettings _articleSettings;

        #endregion

        #region " private Properties "

        public new ArticleSettings ArticleSettings
        {
            get
            {
                if (_articleSettings == null)
                {

                    Hashtable _settings = DotNetNuke.Entities.Portals.PortalSettings.GetModuleSettings(_articleModuleID);

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

        private void BindComments()
        {

            ModuleController objModuleController = new ModuleController();
            ModuleInfo objModule = objModuleController.GetModule(_articleModuleID, _articleTabID);
            _objLayoutController = new LayoutController(PortalSettings, ArticleSettings, objModule, Page);

            string layoutHeader;
            if (Settings.Contains(ArticleConstants.LATEST_COMMENTS_HTML_HEADER))
            {
                layoutHeader = Settings[ArticleConstants.LATEST_COMMENTS_HTML_HEADER].ToString();
            }
            else
            {
                layoutHeader = ArticleConstants.DEFAULT_LATEST_COMMENTS_HTML_HEADER;
            }
            _objLayoutHeader = _objLayoutController.GetLayoutObject(layoutHeader);

            string layoutItem;
            if (Settings.Contains(ArticleConstants.LATEST_COMMENTS_HTML_BODY))
            {
                layoutItem = Settings[ArticleConstants.LATEST_COMMENTS_HTML_BODY].ToString();
            }
            else
            {
                layoutItem = ArticleConstants.DEFAULT_LATEST_COMMENTS_HTML_BODY;
            }
            _objLayoutItem = _objLayoutController.GetLayoutObject(layoutItem);

            string layoutFooter;
            if (Settings.Contains(ArticleConstants.LATEST_COMMENTS_HTML_FOOTER))
            {
                layoutFooter = Settings[ArticleConstants.LATEST_COMMENTS_HTML_FOOTER].ToString();
            }
            else
            {
                layoutFooter = ArticleConstants.DEFAULT_LATEST_COMMENTS_HTML_FOOTER;
            }
            _objLayoutFooter = _objLayoutController.GetLayoutObject(layoutFooter);

            int count = 10;
            if (Settings.Contains(ArticleConstants.LATEST_COMMENTS_COUNT))
            {
                count = Convert.ToInt32(Settings[ArticleConstants.LATEST_COMMENTS_COUNT].ToString());
            }

            CommentController objCommentController = new CommentController();
            rptLatestComments.DataSource = objCommentController.GetCommentList(_articleModuleID, Null.NullInteger, true, SortDirection.Descending, count);
            rptLatestComments.DataBind();

            if (rptLatestComments.Items.Count == 0)
            {
                phNoComments.Visible = true;
                rptLatestComments.Visible = false;

                string noComments = ArticleConstants.DEFAULT_LATEST_COMMENTS_HTML_NO_COMMENTS;
                if (Settings.Contains(ArticleConstants.LATEST_COMMENTS_HTML_NO_COMMENTS))
                {
                    noComments = Settings[ArticleConstants.LATEST_COMMENTS_HTML_NO_COMMENTS].ToString();
                }
                noComments = "<div id=\"NoRecords\" class=\"Normal\">" + noComments + "</div>";
                LayoutInfo objNoComments = _objLayoutController.GetLayoutObject(noComments);
                ProcessHeaderFooter(phNoComments.Controls, objNoComments.Tokens);
            }

        }

        private void ProcessBody(ControlCollection controls, CommentInfo objComment, string[] layoutArray)
        {

            if (ArticleTabInfo == null)
            {
                return;
            }

            ArticleController objArticleController = new ArticleController();
            ArticleInfo objArticle = objArticleController.GetArticle(objComment.ArticleID);

            _objLayoutController.ProcessComment(controls, objArticle, objComment, _objLayoutItem.Tokens);

        }

        private void ProcessHeaderFooter(ControlCollection controls, string[] layoutArray)
        {


            for (int iPtr = 0; iPtr < layoutArray.Length; iPtr += 2)
            {
                controls.Add(new LiteralControl(layoutArray[iPtr].ToString()));

                if (iPtr < layoutArray.Length - 1)
                {
                    switch (layoutArray[iPtr + 1])
                    {

                        default:
                            Literal objLiteralOther = new Literal();
                            objLiteralOther.Text = "[" + layoutArray[iPtr + 1] + "]";
                            objLiteralOther.EnableViewState = false;
                            controls.Add(objLiteralOther);
                            break;
                    }
                }
            }

        }

        #endregion

        #region " private Methods "

        private bool FindSettings()
        {

            if (Settings.Contains(ArticleConstants.LATEST_COMMENTS_TAB_ID))
            {
                if (Numeric.IsNumeric(Settings[ArticleConstants.LATEST_COMMENTS_TAB_ID].ToString()))
                {
                    _articleTabID = Convert.ToInt32(Settings[ArticleConstants.LATEST_COMMENTS_TAB_ID].ToString());
                }
            }

            if (Settings.Contains(ArticleConstants.LATEST_COMMENTS_MODULE_ID))
            {
                if (Numeric.IsNumeric(Settings[ArticleConstants.LATEST_COMMENTS_MODULE_ID].ToString()))
                {
                    _articleModuleID = Convert.ToInt32(Settings[ArticleConstants.LATEST_COMMENTS_MODULE_ID].ToString());
                    if (_articleModuleID != Null.NullInteger)
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            this.PreRender += new EventHandler(Page_PreRender);
            rptLatestComments.ItemDataBound += rptLatestComments_OnItemDataBound;
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

                if (FindSettings())
                {
                    BindComments();
                }
                else
                {
                    lblNotConfigured.Visible = true;
                    rptLatestComments.Visible = false;
                }
            }
            catch (Exception exc)    //Module failed to load
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
                    if (Settings.Contains(ArticleConstants.LATEST_COMMENTS_INCLUDE_STYLESHEET))
                    {
                        if (Convert.ToBoolean(Settings[ArticleConstants.LATEST_COMMENTS_INCLUDE_STYLESHEET].ToString()))
                        {
                            LoadStyleSheet();
                        }
                    }
                }

            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void rptLatestComments_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            try
            {

                if (e.Item.ItemType == ListItemType.Header)
                {
                    ProcessHeaderFooter(e.Item.Controls, _objLayoutHeader.Tokens);
                }

                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    ProcessBody(e.Item.Controls, (CommentInfo)e.Item.DataItem, _objLayoutItem.Tokens);
                }

                if (e.Item.ItemType == ListItemType.Footer)
                {
                    ProcessHeaderFooter(e.Item.Controls, _objLayoutFooter.Tokens);
                }

            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}