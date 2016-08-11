using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Exceptions;
using GcDesign.NewsArticles.Components.CustomFields;
using System.IO;
using System.Collections;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;

namespace GcDesign.NewsArticles
{
    public partial class LatestArticlesOptions : ModuleSettingsBase
    {
        #region " private Methods "

        private void BindMatchOperators()
        {

            foreach (int value in System.Enum.GetValues(typeof(MatchOperatorType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(MatchOperatorType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(MatchOperatorType), value), this.LocalResourceFile);
                rdoMatchOperator.Items.Add(li);
            }

            foreach (int value in System.Enum.GetValues(typeof(MatchOperatorType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(MatchOperatorType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(MatchOperatorType), value), this.LocalResourceFile);
                rdoTagsMatchOperator.Items.Add(li);
            }

        }

        private void BindLayoutMode()
        {

            foreach (int value in System.Enum.GetValues(typeof(LayoutModeType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(LayoutModeType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(LayoutModeType), value), this.LocalResourceFile);
                lstLayoutMode.Items.Add(li);
            }

        }

        private void BindPageMode()
        {

            foreach (int value in System.Enum.GetValues(typeof(LinkFilterType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(LinkFilterType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(LinkFilterType), value), this.LocalResourceFile);
                rdoLinkFilter.Items.Add(li);
            }

            rdoLinkFilter.SelectedIndex = 0;

        }

        private void BindPages()
        {

            TabController objTabController = new TabController();

            ArrayList objTabs = objTabController.GetTabs(PortalId);
            foreach (DotNetNuke.Entities.Tabs.TabInfo objTab in objTabs)
            {
                drpPageFilter.Items.Add(new ListItem(objTab.TabPath.Replace("//", "/").TrimStart('/'), objTab.TabID.ToString()));
            }


        }

        private void BindSortBy()
        {

            drpSortBy.Items.Add(new ListItem(Localization.GetString("PublishDate.Text", this.LocalResourceFile), "StartDate"));
            drpSortBy.Items.Add(new ListItem(Localization.GetString("ExpiryDate.Text", this.LocalResourceFile), "EndDate"));
            drpSortBy.Items.Add(new ListItem(Localization.GetString("LastUpdate.Text", this.LocalResourceFile), "LastUpdate"));
            drpSortBy.Items.Add(new ListItem(Localization.GetString("HighestRated.Text", this.LocalResourceFile), "Rating"));
            drpSortBy.Items.Add(new ListItem(Localization.GetString("MostCommented.Text", this.LocalResourceFile), "CommentCount"));
            drpSortBy.Items.Add(new ListItem(Localization.GetString("MostViewed.Text", this.LocalResourceFile), "NumberOfViews"));
            drpSortBy.Items.Add(new ListItem(Localization.GetString("Random.Text", this.LocalResourceFile), "NewID()"));
            drpSortBy.Items.Add(new ListItem(Localization.GetString("SortTitle.Text", this.LocalResourceFile), "Title"));

        }

        private void BindSortDirection()
        {

            drpSortDirection.Items.Add(new ListItem(Localization.GetString("Ascending.Text", this.LocalResourceFile), "ASC"));
            drpSortDirection.Items.Add(new ListItem(Localization.GetString("Descending.Text", this.LocalResourceFile), "DESC"));

        }

        private void BindCustomFields()
        {

            if (drpModuleID.Items.Count > 0)
            {
                CustomFieldController objCustomFieldController = new CustomFieldController();
                drpCustomField.DataSource = objCustomFieldController.List(Convert.ToInt32(drpModuleID.SelectedValue.Split('-')[1]));
                drpCustomField.DataBind();
                drpCustomField.Items.Insert(0, new ListItem(Localization.GetString("SelectCustomField", this.LocalResourceFile), "-1"));
            }

        }

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

            if (drpModuleID.Items.Count > 0)
            {
                BindCategories();
                BindCustomFields();
            }

        }

        private void BindCategories()
        {

            if (drpModuleID.Items.Count > 0)
            {
                CategoryController objCategoryController = new CategoryController();
                List<CategoryInfo> objCategories = objCategoryController.GetCategoriesAll(Convert.ToInt32(drpModuleID.SelectedValue.Split('-')[1]), Null.NullInteger, CategorySortType.Name);

                lstCategories.DataSource = objCategories;
                lstCategories.DataBind();

                lstCategoriesExclude.DataSource = objCategories;
                lstCategoriesExclude.DataBind();
            }

        }

        private void BindSettings()
        {

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_MODULE_ID) && Settings.Contains(ArticleConstants.LATEST_ARTICLES_TAB_ID))
            {
                if (drpModuleID.Items.FindByValue(Settings[ArticleConstants.LATEST_ARTICLES_TAB_ID].ToString() + "-" + Settings[ArticleConstants.LATEST_ARTICLES_MODULE_ID].ToString()) != null)
                {
                    drpModuleID.SelectedValue = Settings[ArticleConstants.LATEST_ARTICLES_TAB_ID].ToString() + "-" + Settings[ArticleConstants.LATEST_ARTICLES_MODULE_ID].ToString();
                }
                BindCategories();
                BindCustomFields();
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_SORT_BY))
            {
                if (drpSortBy.Items.FindByValue(Settings[ArticleConstants.LATEST_ARTICLES_SORT_BY].ToString()) != null)
                {
                    drpSortBy.SelectedValue = Settings[ArticleConstants.LATEST_ARTICLES_SORT_BY].ToString();
                }
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_SORT_DIRECTION))
            {
                if (drpSortDirection.Items.FindByValue(Settings[ArticleConstants.LATEST_ARTICLES_SORT_DIRECTION].ToString()) != null)
                {
                    drpSortDirection.SelectedValue = Settings[ArticleConstants.LATEST_ARTICLES_SORT_DIRECTION].ToString();
                }
            }
            else
            {
                if (drpSortDirection.Items.FindByValue(ArticleConstants.DEFAULT_SORT_DIRECTION) != null)
                {
                    drpSortDirection.SelectedValue = ArticleConstants.DEFAULT_SORT_DIRECTION;
                }
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_CATEGORIES))
            {
                if (Settings[ArticleConstants.LATEST_ARTICLES_CATEGORIES].ToString() != Null.NullString && Settings[ArticleConstants.LATEST_ARTICLES_CATEGORIES].ToString() != "-1")
                {
                    string[] categories = Settings[ArticleConstants.LATEST_ARTICLES_CATEGORIES].ToString().Split(',');

                    foreach (string category in categories)
                    {
                        if (lstCategories.Items.FindByValue(category) != null)
                        {
                            lstCategories.Items.FindByValue(category).Selected = true;
                        }
                    }

                    rdoMatchOperator.Enabled = true;
                    lstCategories.Enabled = true;
                }
                else
                {
                    rdoMatchOperator.Enabled = false;
                    chkAllCategories.Checked = true;
                    lstCategories.Enabled = false;
                }
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_CATEGORIES_EXCLUDE))
            {
                if (Settings[ArticleConstants.LATEST_ARTICLES_CATEGORIES_EXCLUDE].ToString() != Null.NullString && Settings[ArticleConstants.LATEST_ARTICLES_CATEGORIES_EXCLUDE].ToString() != "-1")
                {
                    string[] categories = Settings[ArticleConstants.LATEST_ARTICLES_CATEGORIES_EXCLUDE].ToString().Split(',');

                    foreach (string category in categories)
                    {
                        if (lstCategoriesExclude.Items.FindByValue(category) != null)
                        {
                            lstCategoriesExclude.Items.FindByValue(category).Selected = true;
                        }
                    }
                }
            }

            if (rdoMatchOperator.Items.Count > 0)
            {
                rdoMatchOperator.Items[0].Selected = true;
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_MATCH_OPERATOR))
            {
                if (rdoMatchOperator.Items.FindByValue(Settings[ArticleConstants.LATEST_ARTICLES_MATCH_OPERATOR].ToString()) != null)
                {
                    rdoMatchOperator.SelectedValue = Settings[ArticleConstants.LATEST_ARTICLES_MATCH_OPERATOR].ToString();
                }
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_TAGS))
            {
                TagController objTagController = new TagController();
                string tags = Settings[ArticleConstants.LATEST_ARTICLES_TAGS].ToString();
                if (tags != "" && drpModuleID.Items.Count > 0)
                {
                    foreach (string tag in tags.Split(','))
                    {
                        TagInfo objTag = objTagController.Get(Convert.ToInt32(tag));
                        if (objTag != null)
                        {
                            if (txtTags.Text != "")
                            {
                                txtTags.Text = txtTags.Text + "," + objTag.Name;
                            }
                            else
                            {
                                txtTags.Text = objTag.Name;
                            }
                        }
                    }
                }
            }

            if (rdoTagsMatchOperator.Items.Count > 0)
            {
                rdoTagsMatchOperator.Items[0].Selected = true;
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_TAGS_MATCH_OPERATOR))
            {
                if (rdoTagsMatchOperator.Items.FindByValue(Settings[ArticleConstants.LATEST_ARTICLES_TAGS_MATCH_OPERATOR].ToString()) != null)
                {
                    rdoTagsMatchOperator.SelectedValue = Settings[ArticleConstants.LATEST_ARTICLES_TAGS_MATCH_OPERATOR].ToString();
                }
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_AUTHOR))
            {
                int authorID = Convert.ToInt32(Settings[ArticleConstants.LATEST_ARTICLES_AUTHOR]);
                if (authorID != Null.NullInteger)
                {
                    UserController objUserController = new UserController();
                    UserInfo objUser = objUserController.GetUser(this.PortalId, authorID);

                    if (objUser != null)
                    {
                        lblAuthorFilter.Text = objUser.Username;
                    }
                }
                else
                {
                    lblAuthorFilter.Text = Localization.GetString("AllAuthors.Text", this.LocalResourceFile);
                }
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_LAYOUT_MODE))
            {
                if (lstLayoutMode.Items.FindByValue(Settings[ArticleConstants.LATEST_ARTICLES_LAYOUT_MODE].ToString()) != null)
                {
                    lstLayoutMode.SelectedValue = Settings[ArticleConstants.LATEST_ARTICLES_LAYOUT_MODE].ToString();
                }
                else
                {
                    lstLayoutMode.SelectedIndex = 0;
                }
            }
            else
            {
                if (lstLayoutMode.Items.FindByValue(ArticleConstants.LATEST_ARTICLES_LAYOUT_MODE_DEFAULT.ToString()) != null)
                {
                    lstLayoutMode.SelectedValue = ArticleConstants.LATEST_ARTICLES_LAYOUT_MODE_DEFAULT.ToString();
                }
                else
                {
                    lstLayoutMode.SelectedIndex = 0;
                }
            }

            BindHtml();

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_ITEMS_PER_ROW))
            {
                txtItemsPerRow.Text = Settings[ArticleConstants.LATEST_ARTICLES_ITEMS_PER_ROW].ToString();
            }
            else
            {
                txtItemsPerRow.Text = ArticleConstants.LATEST_ARTICLES_ITEMS_PER_ROW_DEFAULT.ToString();
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_IDS))
            {
                txtArticleIDs.Text = Settings[ArticleConstants.LATEST_ARTICLES_IDS].ToString();
            }
            else
            {
                txtArticleIDs.Text = "";
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_COUNT))
            {
                txtArticleCount.Text = Settings[ArticleConstants.LATEST_ARTICLES_COUNT].ToString();
            }
            else
            {
                txtArticleCount.Text = "10";
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_START_POINT))
            {
                txtStartPoint.Text = Settings[ArticleConstants.LATEST_ARTICLES_START_POINT].ToString();
            }
            else
            {
                txtStartPoint.Text = "0";
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_MAX_AGE))
            {
                txtMaxAge.Text = Settings[ArticleConstants.LATEST_ARTICLES_MAX_AGE].ToString();
            }
            else
            {
                txtMaxAge.Text = "";
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_START_DATE))
            {
                if (Settings[ArticleConstants.LATEST_ARTICLES_START_DATE].ToString() != "")
                {
                    DateTime objStartDate = DateTime.Parse(Settings[ArticleConstants.LATEST_ARTICLES_START_DATE].ToString());
                    txtStartDate.Text = objStartDate.ToShortDateString();
                }
            }
            else
            {
                txtStartDate.Text = "";
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_QUERY_STRING_FILTER))
            {
                chkQueryStringFilter.Checked = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_QUERY_STRING_FILTER].ToString());
            }
            else
            {
                chkQueryStringFilter.Checked = ArticleConstants.LATEST_ARTICLES_QUERY_STRING_FILTER_DEFAULT;
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_QUERY_STRING_PARAM))
            {
                txtQueryStringParam.Text = Settings[ArticleConstants.LATEST_ARTICLES_QUERY_STRING_PARAM].ToString();
            }
            else
            {
                txtQueryStringParam.Text = ArticleConstants.LATEST_ARTICLES_QUERY_STRING_PARAM_DEFAULT;
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_USERNAME_FILTER))
            {
                chkUsernameFilter.Checked = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_USERNAME_FILTER].ToString());
            }
            else
            {
                chkUsernameFilter.Checked = ArticleConstants.LATEST_ARTICLES_USERNAME_FILTER_DEFAULT;
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_USERNAME_PARAM))
            {
                txtUsernameParam.Text = Settings[ArticleConstants.LATEST_ARTICLES_USERNAME_PARAM].ToString();
            }
            else
            {
                txtUsernameParam.Text = ArticleConstants.LATEST_ARTICLES_USERNAME_PARAM_DEFAULT;
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_LOGGED_IN_USER_FILTER))
            {
                chkLoggedInUser.Checked = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_LOGGED_IN_USER_FILTER].ToString());
            }
            else
            {
                chkLoggedInUser.Checked = ArticleConstants.LATEST_ARTICLES_LOGGED_IN_USER_FILTER_DEFAULT;
            }

            if (Settings.Contains(ArticleConstants.LAUNCH_LINKS))
            {
                chkLaunchLinks.Checked = Convert.ToBoolean(Settings[ArticleConstants.LAUNCH_LINKS].ToString());
            }
            else
            {
                chkLaunchLinks.Checked = false;
            }

            if (Settings.Contains(ArticleConstants.BUBBLE_FEATURED_ARTICLES))
            {
                chkBubbleFeatured.Checked = Convert.ToBoolean(Settings[ArticleConstants.BUBBLE_FEATURED_ARTICLES].ToString());
            }
            else
            {
                chkBubbleFeatured.Checked = false;
            }

            if (Settings.Contains(ArticleConstants.LATEST_ENABLE_PAGER))
            {
                chkEnablePager.Checked = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ENABLE_PAGER].ToString());
            }
            else
            {
                chkEnablePager.Checked = false;
            }

            if (Settings.Contains(ArticleConstants.LATEST_PAGE_SIZE))
            {
                txtPageSize.Text = Settings[ArticleConstants.LATEST_PAGE_SIZE].ToString();
            }
            else
            {
                txtPageSize.Text = ArticleConstants.LATEST_PAGE_SIZE_DEFAULT.ToString();
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_SHOW_PENDING))
            {
                chkShowPending.Checked = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_SHOW_PENDING].ToString());
            }
            else
            {
                chkShowPending.Checked = false;
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_SHOW_RELATED))
            {
                chkShowRelated.Checked = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_SHOW_RELATED].ToString());
            }
            else
            {
                chkShowRelated.Checked = false;
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_FEATURED_ONLY))
            {
                chkFeaturedOnly.Checked = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_FEATURED_ONLY].ToString());
            }
            else
            {
                chkFeaturedOnly.Checked = false;
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_NOT_FEATURED_ONLY))
            {
                chkNotFeaturedOnly.Checked = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_NOT_FEATURED_ONLY].ToString());
            }
            else
            {
                chkNotFeaturedOnly.Checked = false;
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_SECURED_ONLY))
            {
                chkSecureOnly.Checked = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_SECURED_ONLY].ToString());
            }
            else
            {
                chkSecureOnly.Checked = false;
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_NOT_SECURED_ONLY))
            {
                chkNotSecureOnly.Checked = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_NOT_SECURED_ONLY].ToString());
            }
            else
            {
                chkNotSecureOnly.Checked = false;
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_CUSTOM_FIELD_FILTER))
            {
                if (drpCustomField.Items.FindByValue(Settings[ArticleConstants.LATEST_ARTICLES_CUSTOM_FIELD_FILTER].ToString()) != null)
                {
                    drpCustomField.SelectedValue = Settings[ArticleConstants.LATEST_ARTICLES_CUSTOM_FIELD_FILTER].ToString();
                }
            }
            else
            {
                drpCustomField.SelectedValue = "-1";
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_CUSTOM_FIELD_VALUE))
            {
                txtCustomFieldValue.Text = Settings[ArticleConstants.LATEST_ARTICLES_CUSTOM_FIELD_VALUE].ToString();
            }

            drpPageFilter.Visible = false;
            txtUrlFilter.Visible = false;

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_LINK_FILTER))
            {
                string linkFilter = Settings[ArticleConstants.LATEST_ARTICLES_LINK_FILTER].ToString().ToLower();
                if (Numeric.IsNumeric(linkFilter))
                {
                    if (drpPageFilter.Items.FindByValue(linkFilter) != null)
                    {
                        drpPageFilter.Visible = true;
                        drpPageFilter.SelectedValue = linkFilter;
                        rdoLinkFilter.SelectedIndex = 2;
                    }
                }
                else
                {
                    txtUrlFilter.Visible = true;
                    txtUrlFilter.Text = linkFilter;
                    rdoLinkFilter.SelectedIndex = 1;
                }
            }

            if (Settings.Contains(ArticleConstants.CATEGORY_SELECTION_HEIGHT_SETTING))
            {
                lstCategories.Height = Unit.Parse(Settings[ArticleConstants.CATEGORY_SELECTION_HEIGHT_SETTING].ToString());
            }
            else
            {
                lstCategories.Height = ArticleConstants.CATEGORY_SELECTION_HEIGHT_DEFAULT;
            }

            if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_INCLUDE_STYLESHEET))
            {
                chkIncludeStylesheet.Checked = Convert.ToBoolean(Settings[ArticleConstants.LATEST_ARTICLES_INCLUDE_STYLESHEET].ToString());
            }
            else
            {
                chkIncludeStylesheet.Checked = ArticleConstants.LATEST_ARTICLES_INCLUDE_STYLESHEET_DEFAULT;
            }

        }

        private void BindHtml()
        {

            LatestLayoutController objLatestLayoutController = new LatestLayoutController();

            LayoutInfo objLayoutHeader = objLatestLayoutController.GetLayout(LatestLayoutType.Listing_Header_Html, ModuleId, Settings);
            LayoutInfo objLayoutItem = objLatestLayoutController.GetLayout(LatestLayoutType.Listing_Item_Html, ModuleId, Settings);
            LayoutInfo objLayoutFooter = objLatestLayoutController.GetLayout(LatestLayoutType.Listing_Footer_Html, ModuleId, Settings);
            LayoutInfo objLayoutEmpty = objLatestLayoutController.GetLayout(LatestLayoutType.Listing_Empty_Html, ModuleId, Settings);

            txtHtmlHeader.Text = objLayoutHeader.Template;
            txtHtmlBody.Text = objLayoutItem.Template;
            txtHtmlFooter.Text = objLayoutFooter.Template;
            txtHtmlNoArticles.Text = objLayoutEmpty.Template;

        }

        private void SaveSettings()
        {

            ModuleController objModuleController = new ModuleController();

            if (drpModuleID.Items.Count > 0)
            {

                string[] values = drpModuleID.SelectedValue.Split('-');

                if (values.Length == 2)
                {
                    objModuleController.UpdateTabModuleSetting(this.TabModuleId, ArticleConstants.LATEST_ARTICLES_TAB_ID, values[0]);
                    objModuleController.UpdateTabModuleSetting(this.TabModuleId, ArticleConstants.LATEST_ARTICLES_MODULE_ID, values[1]);
                }

            }

            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_SORT_BY, drpSortBy.SelectedValue);
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_SORT_DIRECTION, drpSortDirection.SelectedValue);
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_LAYOUT_MODE, lstLayoutMode.SelectedValue);

            LatestLayoutController objLatestLayoutController = new LatestLayoutController();

            objLatestLayoutController.UpdateLayout(LatestLayoutType.Listing_Header_Html, ModuleId, txtHtmlHeader.Text);
            objLatestLayoutController.UpdateLayout(LatestLayoutType.Listing_Item_Html, ModuleId, txtHtmlBody.Text);
            objLatestLayoutController.UpdateLayout(LatestLayoutType.Listing_Footer_Html, ModuleId, txtHtmlFooter.Text);
            objLatestLayoutController.UpdateLayout(LatestLayoutType.Listing_Empty_Html, ModuleId, txtHtmlNoArticles.Text);

            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_ITEMS_PER_ROW, txtItemsPerRow.Text);
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LAUNCH_LINKS, chkLaunchLinks.Checked.ToString());
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.BUBBLE_FEATURED_ARTICLES, chkBubbleFeatured.Checked.ToString());
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ENABLE_PAGER, chkEnablePager.Checked.ToString());
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_PAGE_SIZE, txtPageSize.Text);

            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_SHOW_PENDING, chkShowPending.Checked.ToString());
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_SHOW_RELATED, chkShowRelated.Checked.ToString());
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_FEATURED_ONLY, chkFeaturedOnly.Checked.ToString());
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_NOT_FEATURED_ONLY, chkNotFeaturedOnly.Checked.ToString());
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_SECURED_ONLY, chkSecureOnly.Checked.ToString());
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_NOT_SECURED_ONLY, chkNotSecureOnly.Checked.ToString());
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_CUSTOM_FIELD_FILTER, drpCustomField.SelectedValue);
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_CUSTOM_FIELD_VALUE, txtCustomFieldValue.Text);
            //objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_LINK_FILTER, ctlLinkFilter.Url)

            if (rdoLinkFilter.SelectedIndex == 0)
            {
                objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_LINK_FILTER, "");
            }
            if (rdoLinkFilter.SelectedIndex == 1)
            {
                objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_LINK_FILTER, Globals.AddHTTP(txtUrlFilter.Text));
            }
            if (rdoLinkFilter.SelectedIndex == 2)
            {
                objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_LINK_FILTER, drpPageFilter.SelectedValue);
            }

            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_COUNT, txtArticleCount.Text);
            if (Numeric.IsNumeric(txtStartPoint.Text) && Convert.ToInt32(txtStartPoint.Text) >= 0)
            {
                objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_START_POINT, txtStartPoint.Text);
            }

            if (txtArticleIDs.Text != "")
            {
                objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_IDS, txtArticleIDs.Text);
                foreach (string ID in txtArticleIDs.Text.Split(','))
                {
                    if (!Numeric.IsNumeric(ID))
                    {
                        objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_IDS, "");
                    }
                }
            }
            else
            {
                objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_IDS, txtArticleIDs.Text);
            }
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_MAX_AGE, txtMaxAge.Text);
            if (txtStartDate.Text != "")
            {
                DateTime objStartDate = DateTime.Parse(txtStartDate.Text);
                objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_START_DATE, objStartDate.Year.ToString() + "-" + objStartDate.Month.ToString() + "-" + objStartDate.Day.ToString());
            }
            else
            {
                objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_START_DATE, Null.NullString);
            }
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_QUERY_STRING_FILTER, chkQueryStringFilter.Checked.ToString());
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_QUERY_STRING_PARAM, txtQueryStringParam.Text);
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_USERNAME_FILTER, chkUsernameFilter.Checked.ToString());
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_USERNAME_PARAM, txtUsernameParam.Text);
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_LOGGED_IN_USER_FILTER, chkLoggedInUser.Checked.ToString());

            if (ddlAuthor.Items.Count > 0)
            {
                objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_AUTHOR, ddlAuthor.SelectedValue);
            }

            if (chkAllCategories.Checked)
            {
                objModuleController.UpdateModuleSetting(ModuleId, ArticleConstants.LATEST_ARTICLES_CATEGORIES, Null.NullInteger.ToString());
            }
            else
            {
                string categories = "";
                foreach (ListItem item in lstCategories.Items)
                {
                    if (item.Selected)
                    {
                        if (categories.Length > 0)
                        {
                            categories = categories + ",";
                        }
                        categories = categories + item.Value;
                    }
                }
                objModuleController.UpdateModuleSetting(ModuleId, ArticleConstants.LATEST_ARTICLES_CATEGORIES, categories);
            }

            string categoriesExclude = "";
            foreach (ListItem item in lstCategoriesExclude.Items)
            {
                if (item.Selected)
                {
                    if (categoriesExclude.Length > 0)
                    {
                        categoriesExclude = categoriesExclude + ",";
                    }
                    categoriesExclude = categoriesExclude + item.Value;
                }
            }
            objModuleController.UpdateModuleSetting(ModuleId, ArticleConstants.LATEST_ARTICLES_CATEGORIES_EXCLUDE, categoriesExclude);

            string tags = "";
            if (drpModuleID.Items.Count > 0)
            {
                foreach (string tag in txtTags.Text.Split(','))
                {
                    if (tag != "")
                    {
                        TagController objTagController = new TagController();
                        TagInfo objTag = objTagController.Get(Convert.ToInt32(drpModuleID.SelectedValue.Split('-')[1]), tag.ToLower());
                        if (objTag != null)
                        {
                            if (tags == "")
                            {
                                tags = objTag.TagID.ToString();
                            }
                            else
                            {
                                tags = tags + "," + objTag.TagID.ToString();
                            }
                        }
                        else
                        {
                            objTag = new TagInfo();
                            objTag.ModuleID = Convert.ToInt32(drpModuleID.SelectedValue.Split('-')[1]);
                            objTag.Name = tag;
                            objTag.NameLowered = tag.ToLower();
                            objTag.Usages = 0;
                            objTag.TagID = objTagController.Add(objTag);
                            if (tags == "")
                            {
                                tags = objTag.TagID.ToString();
                            }
                            else
                            {
                                tags = tags + "," + objTag.TagID.ToString();
                            }
                        }
                    }
                }
            }
            objModuleController.UpdateModuleSetting(ModuleId, ArticleConstants.LATEST_ARTICLES_TAGS, tags);
            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_TAGS_MATCH_OPERATOR, rdoTagsMatchOperator.SelectedValue);

            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_MATCH_OPERATOR, rdoMatchOperator.SelectedValue);

            objModuleController.UpdateModuleSetting(this.ModuleId, ArticleConstants.LATEST_ARTICLES_INCLUDE_STYLESHEET, chkIncludeStylesheet.Checked.ToString());

        }

        private void PopulateAuthorList()
        {

            if (drpModuleID.Items.Count > 0)
            {
                ddlAuthor.DataSource = GetAuthorList(Convert.ToInt32(drpModuleID.SelectedValue.Split('-')[1]));
                ddlAuthor.DataBind();
                ddlAuthor.Items.Insert(0, new ListItem(Localization.GetString("AllAuthors.Text", this.LocalResourceFile), "-1"));

                if (Settings.Contains(ArticleConstants.LATEST_ARTICLES_AUTHOR))
                {
                    int authorID = Convert.ToInt32(Settings[ArticleConstants.LATEST_ARTICLES_AUTHOR]);

                    if (ddlAuthor.Items.FindByValue(authorID.ToString()) != null)
                    {
                        ddlAuthor.SelectedValue = authorID.ToString();
                    }

                }
            }
            else
            {
                ddlAuthor.Items.Clear();
            }

        }

        public ArrayList GetAuthorList(int moduleID)
        {

            Hashtable moduleSettings = DotNetNuke.Entities.Portals.PortalSettings.GetModuleSettings(moduleID);
            string distributionList = "";
            ArrayList userList = new ArrayList();

            if (moduleSettings.Contains(ArticleConstants.PERMISSION_SUBMISSION_SETTING))
            {

                string roles = moduleSettings[ArticleConstants.PERMISSION_SUBMISSION_SETTING].ToString();
                string[] rolesArray = roles.Split(';');
                Hashtable userIDs = new Hashtable();

                foreach (string role in rolesArray)
                {
                    if (role.Length > 0)
                    {

                        RoleController objRoleController = new RoleController();
                        RoleInfo objRole = objRoleController.GetRoleByName(PortalSettings.PortalId, role);

                        if (objRole != null)
                        {
                            ArrayList objUsers = objRoleController.GetUserRolesByRoleName(PortalSettings.PortalId, objRole.RoleName);
                            foreach (UserRoleInfo objUser in objUsers)
                            {
                                if (!userIDs.Contains(objUser.UserID))
                                {
                                    UserController objUserController = new DotNetNuke.Entities.Users.UserController();
                                    UserInfo objSelectedUser = objUserController.GetUser(PortalSettings.PortalId, objUser.UserID);
                                    if (objSelectedUser != null)
                                    {
                                        if (objSelectedUser.Membership.Email.Length > 0)
                                        {
                                            userIDs.Add(objUser.UserID, objUser.UserID);
                                            userList.Add(objSelectedUser);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            return userList;

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.PreRender += new EventHandler(Page_PreRender);
            drpModuleID.SelectedIndexChanged += drpModuleID_SelectedIndexChanged;
            chkAllCategories.CheckedChanged += chkAllCategories_CheckedChanged;
            lstLayoutMode.SelectedIndexChanged += lstLayoutMode_SelectedIndexChanged;
            rdoLinkFilter.SelectedIndexChanged += rdoLinkFilter_SelectedIndexChanged;
            cmdSelectAuthor.Click += cmdSelectAuthor_Click;
            cmdSaveTemplate.Click += cmdSaveTemplate_Click;
            cmdLoadTemplate.Click += cmdLoadTemplate_Click;
            drpLoadFromTemplate.SelectedIndexChanged += drpLoadFromTemplate_SelectedIndexChanged;
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }


        private void Page_PreRender(object sender, EventArgs e)
        {

            try
            {
                trItemsPerRow.Visible = (lstLayoutMode.SelectedValue == LayoutModeType.Advanced.ToString());
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }


        }

        private void drpModuleID_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                BindCategories();
                BindCustomFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void chkAllCategories_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                rdoMatchOperator.Enabled = !chkAllCategories.Checked;
                lstCategories.Enabled = !chkAllCategories.Checked;
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void lstLayoutMode_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                //BindHtml()
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void rdoLinkFilter_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {

                if (rdoLinkFilter.SelectedIndex == 0)
                {
                    drpPageFilter.Visible = false;
                    txtUrlFilter.Visible = false;
                }

                if (rdoLinkFilter.SelectedIndex == 1)
                {
                    drpPageFilter.Visible = false;
                    txtUrlFilter.Visible = true;
                }

                if (rdoLinkFilter.SelectedIndex == 2)
                {
                    drpPageFilter.Visible = true;
                    txtUrlFilter.Visible = false;
                }

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdSelectAuthor_Click(object sender, EventArgs e)
        {

            try
            {

                PopulateAuthorList();
                ddlAuthor.Visible = true;
                cmdSelectAuthor.Visible = false;
                lblAuthorFilter.Visible = false;

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdSaveTemplate_Click(object sender, EventArgs e)
        {

            try
            {

                if (txtSaveTemplate.Text != "")
                {

                    string pathToTemplate = this.MapPath("Templates/Portals/" + PortalId.ToString() + "/" + txtSaveTemplate.Text + "/");

                    if (!Directory.Exists(pathToTemplate))
                    {
                        Directory.CreateDirectory(pathToTemplate);
                    }

                    StreamWriter sw = new StreamWriter(pathToTemplate + "header.html");
                    try
                    {
                        sw.Write(txtHtmlHeader.Text);
                    }
                    catch { }
                    finally
                    {
                        if (sw != null) { sw.Close(); }
                    }

                    sw = new StreamWriter(pathToTemplate + "body.html");
                    try
                    {
                        sw.Write(txtHtmlBody.Text);
                    }
                    catch { }
                    finally
                    {
                        if (sw != null) { sw.Close(); }
                    }


                    sw = new StreamWriter(pathToTemplate + "footer.html");
                    try
                    {
                        sw.Write(txtHtmlFooter.Text);
                    }
                    catch { }
                    finally
                    {
                        if (sw != null) { sw.Close(); }
                    }

                    sw = new StreamWriter(pathToTemplate + "empty.html");
                    try
                    {
                        sw.Write(txtHtmlNoArticles.Text);
                    }
                    catch { }
                    finally
                    {
                        if (sw != null) { sw.Close(); }
                    }

                    BindTemplateSaves(txtSaveTemplate.Text);

                }

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdLoadTemplate_Click(object sender, EventArgs e)
        {

            try
            {

                if (txtSaveTemplate.Text != "")
                {

                    string pathToTemplate = this.MapPath("Templates/Portals/" + PortalId.ToString() + "/" + drpLoadFromTemplate.SelectedValue + "/");

                    if (File.Exists(pathToTemplate + "header.html"))
                    {
                        StreamReader sr = new StreamReader(pathToTemplate + "header.html");
                        try
                        {
                            txtHtmlHeader.Text = sr.ReadToEnd();
                        }
                        catch (Exception ex)
                        { }
                        finally
                        {
                            if (sr != null) { sr.Close(); }
                        }
                    }

                    if (File.Exists(pathToTemplate + "body.html"))
                    {
                        StreamReader sr = new StreamReader(pathToTemplate + "body.html");
                        try
                        {
                            txtHtmlBody.Text = sr.ReadToEnd();
                        }
                        catch (Exception ex)
                        { }
                        finally
                        {
                            if (sr != null) { sr.Close(); }
                        }
                    }

                    if (File.Exists(pathToTemplate + "footer.html"))
                    {
                        StreamReader sr = new StreamReader(pathToTemplate + "footer.html");
                        try
                        {
                            txtHtmlFooter.Text = sr.ReadToEnd();
                        }
                        catch (Exception ex)
                        { }
                        finally
                        {
                            if (sr != null) { sr.Close(); }
                        }
                    }

                    if (File.Exists(pathToTemplate + "empty.html"))
                    {
                        StreamReader sr = new StreamReader(pathToTemplate + "empty.html");
                        try
                        {
                            txtHtmlNoArticles.Text = sr.ReadToEnd();
                        }
                        catch (Exception ex)
                        { }
                        finally
                        {
                            if (sr != null) { sr.Close(); }
                        }
                    }

                }

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void drpLoadFromTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                txtSaveTemplate.Text = drpLoadFromTemplate.SelectedValue;
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void BindTemplateSaves(string template)
        {

            drpLoadFromTemplate.Items.Clear();

            string templateRoot = this.MapPath("Templates/Portals/" + PortalId.ToString() + "/");
            if (Directory.Exists(templateRoot))
            {
                string[] arrFolders = Directory.GetDirectories(templateRoot);
                foreach (string folder in arrFolders)
                {
                    string folderName = folder.Substring(folder.LastIndexOf(@"\") + 1);
                    ListItem objListItem = new ListItem();
                    objListItem.Text = folderName;
                    objListItem.Value = folderName;
                    drpLoadFromTemplate.Items.Add(objListItem);
                }
            }

            if (drpLoadFromTemplate.Items.Count == 0)
            {
                trLoadFromTemplate.Visible = false;
            }
            else
            {
                trLoadFromTemplate.Visible = true;

                if (template != "")
                {
                    if (drpLoadFromTemplate.Items.FindByValue(template) != null)
                    {
                        drpLoadFromTemplate.SelectedValue = template;
                    }
                }

                txtSaveTemplate.Text = drpLoadFromTemplate.SelectedValue;
            }

        }

        #endregion

        #region " Base Method Implementations "

        public override void LoadSettings()
        {

            cmdStartDate.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtStartDate);

            if (!IsPostBack)
            {
                BindModules();
                BindLayoutMode();
                BindMatchOperators();
                BindPageMode();
                BindPages();
                BindSortBy();
                BindSortDirection();
                BindSettings();
                BindTemplateSaves("");
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
    }
}