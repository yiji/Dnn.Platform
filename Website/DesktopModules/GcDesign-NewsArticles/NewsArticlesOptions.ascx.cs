using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using GcDesign.NewsArticles.Components.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;

using DotNetNuke.Common;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using GcDesign.NewsArticles.Components.CustomFields;
using System.IO;
using System.Collections;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;

namespace GcDesign.NewsArticles
{
    public partial class NewsArticlesOptions : ModuleSettingsBase
    {

        private void BindModules()
        {

            DesktopModuleInfo objDesktopModuleInfo = DesktopModuleController.GetDesktopModuleByModuleName("GcDesign-NewsArticles",PortalId);

            if (objDesktopModuleInfo != null)
            {

                TabController objTabController = new TabController();
                TabCollection objTabs = TabController.Instance.GetTabsByPortal(PortalId);
                foreach (DotNetNuke.Entities.Tabs.TabInfo objTab in objTabs.AsList())
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
                                            TabInfo objTabSelected = objTab;
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

            if (drpModuleID.Items.Count > 0)
            {
                drpModuleID.Items.Insert(0, new ListItem(""));
            }

        }

        protected void InitializeComponent()
        {
            //drpModuleID.SelectedIndexChanged += drpModuleID_SelectedIndexChanged;
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        //private void drpModuleID_SelectedIndexChanged(object sender, EventArgs e)
        //{

        //    try
        //    {

        //    }
        //    catch (Exception exc) //Module failed to load
        //    {
        //        Exceptions.ProcessModuleLoadException(this, exc);
        //    }

        //}

        #region " Base Method Implementations "

        public override void LoadSettings()
        {

            if (!IsPostBack)
            {
                BindModules();
                BindSettings();
            }

        }

        public override void UpdateSettings()
        {

            try
            {

                SaveSettings();

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion


        private void SaveSettings()
        {

            ModuleController objModuleController = new ModuleController();

            if (drpModuleID.Items.Count > 0)
            {

                string[] values = drpModuleID.SelectedValue.Split('-');

                if (drpModuleID.SelectedValue == "")
                {
                    objModuleController.UpdateTabModuleSetting(this.TabModuleId, ArticleConstants.News_ARTICLES_TAB_ID, this.TabId.ToString());
                    objModuleController.UpdateTabModuleSetting(this.TabModuleId, ArticleConstants.NEWS_ARTICLES_MODULE_ID, this.TabModuleId.ToString());
                }
                if (values.Length == 2)
                {
                    objModuleController.UpdateTabModuleSetting(this.TabModuleId, ArticleConstants.News_ARTICLES_TAB_ID, values[0]);
                    objModuleController.UpdateTabModuleSetting(this.TabModuleId, ArticleConstants.NEWS_ARTICLES_MODULE_ID, values[1]);
                }

            }

        }

        private void BindSettings()
        {
            if (ModuleConfiguration.TabModuleSettings.Contains(ArticleConstants.NEWS_ARTICLES_MODULE_ID) && ModuleConfiguration.TabModuleSettings.Contains(ArticleConstants.News_ARTICLES_TAB_ID))
            {
                if (drpModuleID.Items.FindByValue(ModuleConfiguration.TabModuleSettings[ArticleConstants.News_ARTICLES_TAB_ID].ToString() + "-" + ModuleConfiguration.TabModuleSettings[ArticleConstants.NEWS_ARTICLES_MODULE_ID].ToString()) != null)
                {
                    drpModuleID.SelectedValue = ModuleConfiguration.TabModuleSettings[ArticleConstants.News_ARTICLES_TAB_ID].ToString() + "-" + ModuleConfiguration.TabModuleSettings[ArticleConstants.NEWS_ARTICLES_MODULE_ID].ToString();
                }
            }


        }



    }
}