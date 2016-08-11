using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;

namespace GcDesign.NewsArticles
{
    public partial class LatestCommentsOptions : ModuleSettingsBase
    {
        #region " private Methods "

        private void BindModules()
        {

            DesktopModuleController objDesktopModuleController = new DesktopModuleController();
            DesktopModuleInfo objDesktopModuleInfo = objDesktopModuleController.GetDesktopModuleByModuleName("GcDesign-NewsArticles");

            if (objDesktopModuleInfo != null)
            {

                TabController objTabController = new TabController();
                ArrayList objTabs = objTabController.GetTabs(PortalId);
                foreach (DotNetNuke.Entities.Tabs.TabInfo objTab in objTabs)
                {
                    if (objTab != null)
                    {
                        if (!objTab.IsDeleted)
                        {
                            ModuleController objModules = new ModuleController();
                            foreach (KeyValuePair<int, ModuleInfo> pair in objModules.GetTabModules(objTab.TabID))
                            {
                                ModuleInfo objModule = pair.Value;
                                if (!objModule.IsDeleted)
                                {
                                    if (objModule.DesktopModuleID == objDesktopModuleInfo.DesktopModuleID)
                                    {
                                        if (PortalSecurity.IsInRoles(objModule.AuthorizedEditRoles) && !objModule.IsDeleted)
                                        {
                                            string strPath = objTab.TabName;
                                            DotNetNuke.Entities.Tabs.TabInfo objTabSelected = objTab;
                                            while (objTabSelected.ParentId != Null.NullInteger)
                                            {
                                                objTabSelected = objTabController.GetTab(objTabSelected.ParentId, objTab.PortalID, false);
                                                if (objTabSelected == null)
                                                {
                                                    break;
                                                }
                                                strPath = objTabSelected.TabName + " -> " + strPath;
                                            }

                                            ListItem objListItem = new ListItem();

                                            objListItem.Value = objModule.TabID.ToString() + "-" + objModule.ModuleID.ToString();
                                            objListItem.Text = strPath + " -> " + objModule.ModuleTitle;

                                            drpModuleID.Items.Add(objListItem);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #endregion

        #region " Base Method Implementations "

        public override void LoadSettings()
        {

            if (!IsPostBack)
            {
                BindModules();

                if (Settings.Contains(ArticleConstants.LATEST_COMMENTS_MODULE_ID) && Settings.Contains(ArticleConstants.LATEST_COMMENTS_TAB_ID))
                {
                    if (drpModuleID.Items.FindByValue(Settings[ArticleConstants.LATEST_COMMENTS_TAB_ID].ToString() + "-" + Settings[ArticleConstants.LATEST_COMMENTS_MODULE_ID].ToString()) != null)
                    {
                        drpModuleID.SelectedValue = Settings[ArticleConstants.LATEST_COMMENTS_TAB_ID].ToString() + "-" + Settings[ArticleConstants.LATEST_COMMENTS_MODULE_ID].ToString();
                    }
                }

                if (Settings.Contains(ArticleConstants.LATEST_COMMENTS_COUNT))
                {
                    txtCommentCount.Text = Settings[ArticleConstants.LATEST_COMMENTS_COUNT].ToString();
                }
                else
                {
                    txtCommentCount.Text = "10";
                }

                if (Settings.Contains(ArticleConstants.LATEST_COMMENTS_HTML_HEADER))
                {
                    txtHtmlHeader.Text = Settings[ArticleConstants.LATEST_COMMENTS_HTML_HEADER].ToString();
                }
                else
                {
                    txtHtmlHeader.Text = ArticleConstants.DEFAULT_LATEST_COMMENTS_HTML_HEADER;
                }

                if (Settings.Contains(ArticleConstants.LATEST_COMMENTS_HTML_BODY))
                {
                    txtHtmlBody.Text = Settings[ArticleConstants.LATEST_COMMENTS_HTML_BODY].ToString();
                }
                else
                {
                    txtHtmlBody.Text = ArticleConstants.DEFAULT_LATEST_COMMENTS_HTML_BODY;
                }

                if (Settings.Contains(ArticleConstants.LATEST_COMMENTS_HTML_FOOTER))
                {
                    txtHtmlFooter.Text = Settings[ArticleConstants.LATEST_COMMENTS_HTML_FOOTER].ToString();
                }
                else
                {
                    txtHtmlFooter.Text = ArticleConstants.DEFAULT_LATEST_COMMENTS_HTML_FOOTER;
                }

                if (Settings.Contains(ArticleConstants.LATEST_COMMENTS_HTML_NO_COMMENTS))
                {
                    txtHtmlNoComments.Text = Settings[ArticleConstants.LATEST_COMMENTS_HTML_NO_COMMENTS].ToString();
                }
                else
                {
                    txtHtmlNoComments.Text = ArticleConstants.DEFAULT_LATEST_COMMENTS_HTML_NO_COMMENTS;
                }

                if (Settings.Contains(ArticleConstants.LATEST_COMMENTS_INCLUDE_STYLESHEET))
                {
                    chkIncludeStylesheet.Checked = Convert.ToBoolean(Settings[ArticleConstants.LATEST_COMMENTS_INCLUDE_STYLESHEET].ToString());
                }
                else
                {
                    chkIncludeStylesheet.Checked = ArticleConstants.DEFAULT_LATEST_COMMENTS_INCLUDE_STYLESHEET;
                }

            }

        }

        public override void UpdateSettings()
        {

            try
            {

                ModuleController objModuleController = new ModuleController();

                if (drpModuleID.Items.Count > 0)
                {

                    string[] values = drpModuleID.SelectedValue.Split('-');

                    if (values.Length == 2)
                    {
                        objModuleController.UpdateTabModuleSetting(this.TabModuleId, ArticleConstants.LATEST_COMMENTS_TAB_ID, values[0]);
                        objModuleController.UpdateTabModuleSetting(this.TabModuleId, ArticleConstants.LATEST_COMMENTS_MODULE_ID, values[1]);
                    }

                }
                objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_COMMENTS_COUNT, txtCommentCount.Text);

                objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_COMMENTS_HTML_HEADER, txtHtmlHeader.Text);
                objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_COMMENTS_HTML_BODY, txtHtmlBody.Text);
                objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_COMMENTS_HTML_FOOTER, txtHtmlFooter.Text);
                objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_COMMENTS_HTML_NO_COMMENTS, txtHtmlNoComments.Text);
                objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_COMMENTS_INCLUDE_STYLESHEET, chkIncludeStylesheet.Checked.ToString());
            }
            catch (Exception exc)  //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}