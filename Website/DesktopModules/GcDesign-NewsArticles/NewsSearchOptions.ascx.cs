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
    public partial class NewsSearchOptions : ModuleSettingsBase
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

        }

        #endregion

        #region " Base Method Implementations "

        public override void LoadSettings()
        {
            try
            {

                if (!Page.IsPostBack)
                {

                    BindModules();

                    if (Settings.Contains(ArticleConstants.NEWS_SEARCH_MODULE_ID) && Settings.Contains(ArticleConstants.NEWS_SEARCH_TAB_ID))
                    {
                        if (drpModuleID.Items.FindByValue(Settings[ArticleConstants.NEWS_SEARCH_TAB_ID].ToString() + "-" + Settings[ArticleConstants.NEWS_SEARCH_MODULE_ID].ToString()) != null)
                        {
                            drpModuleID.SelectedValue = Settings[ArticleConstants.NEWS_SEARCH_TAB_ID].ToString() + "-" + Settings[ArticleConstants.NEWS_SEARCH_MODULE_ID].ToString();
                        }
                    }

                }
            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public override void UpdateSettings()
        {
            try
            {

                ModuleController objModules = new ModuleController();

                if (drpModuleID.Items.Count > 0)
                {

                    string[] values = drpModuleID.SelectedValue.Split('-');

                    if (values.Length == 2)
                    {
                        objModules.UpdateTabModuleSetting(this.TabModuleId, ArticleConstants.NEWS_SEARCH_TAB_ID, values[0]);
                        objModules.UpdateTabModuleSetting(this.TabModuleId, ArticleConstants.NEWS_SEARCH_MODULE_ID, values[1]);
                    }

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