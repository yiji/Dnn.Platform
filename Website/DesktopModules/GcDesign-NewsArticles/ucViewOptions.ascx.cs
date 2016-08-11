using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

using GcDesign.NewsArticles.Components.Types;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Entities.Portals;

using GcDesign.NewsArticles.Base;
using System.Collections;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Tabs;

namespace GcDesign.NewsArticles
{
    public partial class ucViewOptions : NewsArticleModuleBase
    {

        #region " private Methods "

        public class ListItemComparer : IComparer<ListItem>
        {


            public int Compare(ListItem x, ListItem y)
            {


                CaseInsensitiveComparer c = new CaseInsensitiveComparer();
                return c.Compare(x.Text, y.Text);
            }
        }

        public static void SortDropDown(DropDownList cbo)
        {
            List<ListItem> lstListItems = new List<ListItem>();
            foreach (ListItem li in cbo.Items)
            {
                lstListItems.Add(li);
            }
            lstListItems.Sort(new ListItemComparer());
            cbo.Items.Clear();
            cbo.Items.AddRange(lstListItems.ToArray());
        }

        #region " private Methods - Load Types/Dropdowns "

        private void BindRoleGroups()
        {

            RoleController objRole = new RoleController();

            drpSecurityRoleGroups.DataSource = RoleController.GetRoleGroups(PortalId);
            drpSecurityRoleGroups.DataBind();
            drpSecurityRoleGroups.Items.Insert(0, new ListItem(Localization.GetString("AllGroups", this.LocalResourceFile), "-1"));

        }

        private void BindRoles()
        {

            RoleController objRole = new RoleController();

            ArrayList availableRoles = new ArrayList();
            ArrayList roles;
            if (drpSecurityRoleGroups.SelectedValue == "-1")
            {
                roles = objRole.GetPortalRoles(PortalId);
            }
            else
            {
                roles = objRole.GetRolesByGroup(PortalId, Convert.ToInt32(drpSecurityRoleGroups.SelectedValue));
            }

            if (roles != null)
            {
                foreach (RoleInfo Role in roles)
                {
                    availableRoles.Add(new ListItem(Role.RoleName, Role.RoleName));
                }
            }

            grdBasicPermissions.DataSource = availableRoles;
            grdBasicPermissions.DataBind();

            grdFormPermissions.DataSource = availableRoles;
            grdFormPermissions.DataBind();

            grdAdminPermissions.DataSource = availableRoles;
            grdAdminPermissions.DataBind();

        }

        private void BindAuthorSelection()
        {

            foreach (int value in System.Enum.GetValues(typeof(AuthorSelectType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(AuthorSelectType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(AuthorSelectType), value), this.LocalResourceFile);
                lstAuthorSelection.Items.Add(li);
            }

        }

        private void BindCategorySortOrder()
        {

            foreach (int value in System.Enum.GetValues(typeof(CategorySortType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(CategorySortType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(CategorySortType), value), this.LocalResourceFile);
                lstCategorySortOrder.Items.Add(li);
            }

        }

        private void BindDisplayTypes()
        {

            foreach (int value in System.Enum.GetValues(typeof(DisplayType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(DisplayType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(DisplayType), value), this.LocalResourceFile);
                drpDisplayType.Items.Add(li);
            }

        }

        private void BindRelatedTypes()
        {

            foreach (int value in System.Enum.GetValues(typeof(RelatedType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(RelatedType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(RelatedType), value), this.LocalResourceFile);
                lstRelatedMode.Items.Add(li);
            }

        }

        private void BindFolders()
        {

            List<IFolderInfo> folders = (List<IFolderInfo>)FolderManager.Instance.GetFolders(PortalId);
            foreach (FolderInfo folder in folders)
            {
                ListItem FolderItem = new ListItem();
                if (folder.FolderPath == Null.NullString)
                {
                    FolderItem.Text = Localization.GetString("Root", this.LocalResourceFile);
                }
                else
                {
                    FolderItem.Text = folder.FolderPath;
                }
                FolderItem.Value = folder.FolderID.ToString();
                drpDefaultImageFolder.Items.Add(FolderItem);
                drpDefaultFileFolder.Items.Add(new ListItem(FolderItem.Text, FolderItem.Value));
            }

        }

        private void BindTextEditorMode()
        {

            foreach (int value in System.Enum.GetValues(typeof(TextEditorModeType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(TextEditorModeType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(TextEditorModeType), value), this.LocalResourceFile);
                lstTextEditorSummaryMode.Items.Add(li);
            }

        }

        private void BindTitleReplacement()
        {

            foreach (int value in System.Enum.GetValues(typeof(TitleReplacementType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(TitleReplacementType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(TitleReplacementType), value), this.LocalResourceFile);
                lstTitleReplacement.Items.Add(li);
            }

        }

        private void BindUrlMode()
        {

            foreach (int value in System.Enum.GetValues(typeof(UrlModeType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(UrlModeType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(UrlModeType), value), this.LocalResourceFile);
                lstUrlMode.Items.Add(li);
            }

        }

        private void BindPageSize()
        {

            drpNumber.Items.Add(new ListItem(Localization.GetString("NoRestriction", this.LocalResourceFile), "-1"));
            drpNumber.Items.Add(new ListItem(Localization.GetString("1", this.LocalResourceFile), "1"));
            drpNumber.Items.Add(new ListItem(Localization.GetString("2", this.LocalResourceFile), "2"));
            drpNumber.Items.Add(new ListItem(Localization.GetString("3", this.LocalResourceFile), "3"));
            drpNumber.Items.Add(new ListItem(Localization.GetString("4", this.LocalResourceFile), "4"));
            drpNumber.Items.Add(new ListItem(Localization.GetString("5", this.LocalResourceFile), "5"));
            drpNumber.Items.Add(new ListItem(Localization.GetString("6", this.LocalResourceFile), "6"));
            drpNumber.Items.Add(new ListItem(Localization.GetString("7", this.LocalResourceFile), "7"));
            drpNumber.Items.Add(new ListItem(Localization.GetString("8", this.LocalResourceFile), "8"));
            drpNumber.Items.Add(new ListItem(Localization.GetString("9", this.LocalResourceFile), "9"));
            drpNumber.Items.Add(new ListItem(Localization.GetString("10", this.LocalResourceFile), "10"));
            drpNumber.Items.Add(new ListItem(Localization.GetString("15", this.LocalResourceFile), "15"));
            drpNumber.Items.Add(new ListItem(Localization.GetString("20", this.LocalResourceFile), "20"));
            drpNumber.Items.Add(new ListItem(Localization.GetString("50", this.LocalResourceFile), "50"));

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

            drpSortDirectionComments.Items.Add(new ListItem(Localization.GetString("Ascending.Text", this.LocalResourceFile), "0"));
            drpSortDirectionComments.Items.Add(new ListItem(Localization.GetString("Descending.Text", this.LocalResourceFile), "1"));

        }

        private void BindSyndicationLinkMode()
        {

            foreach (int value in System.Enum.GetValues(typeof(SyndicationLinkType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(SyndicationLinkType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(SyndicationLinkType), value), this.LocalResourceFile);
                lstSyndicationLinkMode.Items.Add(li);
            }

        }

        private void BindSyndicationEnclosureType()
        {

            foreach (int value in System.Enum.GetValues(typeof(SyndicationEnclosureType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(SyndicationEnclosureType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(SyndicationEnclosureType), value) + "-Enclosure", this.LocalResourceFile);
                lstSyndicationEnclosureType.Items.Add(li);
            }

        }

        private void BindMenuPositionType()
        {

            foreach (int value in System.Enum.GetValues(typeof(MenuPositionType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(MenuPositionType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(MenuPositionType), value), this.LocalResourceFile);
                lstMenuPosition.Items.Add(li);
            }

        }

        private void BindTemplates(){

            string templateRoot = this.MapPath("Templates");
            if (Directory.Exists(templateRoot)) {
                string[] arrFolders= Directory.GetDirectories(templateRoot);
                foreach(string folder in arrFolders){
                    string folderName = folder.Substring(folder.LastIndexOf(@"\") + 1);
                    ListItem objListItem = new ListItem();
                    objListItem.Text = folderName;
                    objListItem.Value = folderName;
                    drpTemplates.Items.Add(objListItem);
                }
            }

        }

        private void BindCategories()
        {

            CategoryController objCategoryController = new CategoryController();
            List<CategoryInfo> objCategories = objCategoryController.GetCategoriesAll(this.ModuleId, Null.NullInteger, ArticleSettings.CategorySortType);

            lstCategories.DataSource = objCategories;
            lstCategories.DataBind();

            lstDefaultCategories.DataSource = objCategories;
            lstDefaultCategories.DataBind();

            drpCategories.DataSource = objCategories;
            drpCategories.DataBind();

        }

        private void BindTimeZone()
        {

            DotNetNuke.Services.Localization.Localization.LoadTimeZoneDropDownList(drpTimeZone, BasePage.PageCulture.Name, Convert.ToString(PortalSettings.TimeZoneOffset));

        }

        private void BindThumbnailType()
        {

            foreach (int value in System.Enum.GetValues(typeof(ThumbnailType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(ThumbnailType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(ThumbnailType), value), this.LocalResourceFile);
                rdoThumbnailType.Items.Add(li);
            }

        }

        private void BindWatermarkPosition()
        {

            foreach (int value in System.Enum.GetValues(typeof(WatermarkPosition)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(WatermarkPosition), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(WatermarkPosition), value), this.LocalResourceFile);
                drpWatermarkPosition.Items.Add(li);
            }

        }

        #endregion

        private void BindSettings()
        {

            BindBasicSettings();
            BindArchiveSettings();
            BindCategorySettings();
            BindCommentSettings();
            BindContentSharingSettings();
            BindFilterSettings();
            BindFormSettings();
            BindImageSettings();
            BindFileSettings();
            BindNotificationSettings();
            BindRelatedSettings();
            BindSecuritySettings();
            BindSEOSettings();
            BindSyndicationSettings();
            BindTwitterSettings();
            BindThirdPartySettings();

        }

        #region " private Methods - Bind/Save Basic Settings "

        private void BindBasicSettings(){

            chkEnableRatingsAuthenticated.Checked = ArticleSettings.EnableRatingsAuthenticated;
            chkEnableRatingsAnonymous.Checked = ArticleSettings.EnableRatingsAnonymous;
            chkEnableCoreSearch.Checked = ArticleSettings.EnableCoreSearch;

            chkEnableNotificationPing.Checked = ArticleSettings.EnableNotificationPing;
            chkEnableAutoTrackback.Checked = ArticleSettings.EnableAutoTrackback;

            chkProcessPostTokens.Checked = ArticleSettings.ProcessPostTokens;

            if (Settings.Contains(ArticleConstants.ENABLE_INCOMING_TRACKBACK_SETTING)) {
                chkEnableIncomingTrackback.Checked = Convert.ToBoolean(Settings[ArticleConstants.ENABLE_INCOMING_TRACKBACK_SETTING].ToString());
            }else{
                chkEnableIncomingTrackback.Checked = true;
            }

            if (Settings.Contains(ArticleConstants.LAUNCH_LINKS)) {
                chkLaunchLinks.Checked = Convert.ToBoolean(Settings[ArticleConstants.LAUNCH_LINKS].ToString());
            }else{
                chkLaunchLinks.Checked = ArticleSettings.LaunchLinks;
            }

            if (Settings.Contains(ArticleConstants.BUBBLE_FEATURED_ARTICLES)) {
                chkBubbleFeaturedArticles.Checked = Convert.ToBoolean(Settings[ArticleConstants.BUBBLE_FEATURED_ARTICLES].ToString());
            }else{
                chkBubbleFeaturedArticles.Checked = false;
            }

            if (Settings.Contains(ArticleConstants.REQUIRE_CATEGORY)) {
                chkRequireCategory.Checked = Convert.ToBoolean(Settings[ArticleConstants.REQUIRE_CATEGORY].ToString());
            }else{
                chkRequireCategory.Checked = false;
            }

            if  (drpDisplayType.Items.FindByValue(ArticleSettings.DisplayMode.ToString()) != null) {
                drpDisplayType.SelectedValue = ArticleSettings.DisplayMode.ToString();
            }

            if  (drpTemplates.Items.FindByValue(ArticleSettings.Template) != null) {
                drpTemplates.SelectedValue = ArticleSettings.Template;
            }

            if (drpNumber.Items.FindByValue(ArticleSettings.PageSize.ToString()) != null) {
                drpNumber.SelectedValue = ArticleSettings.PageSize.ToString();
            }

            if (drpTimeZone.Items.FindByValue(ArticleSettings.ServerTimeZone.ToString()) != null) {
                drpTimeZone.SelectedValue = ArticleSettings.ServerTimeZone.ToString();
            }

            if (drpSortBy.Items.FindByValue(ArticleSettings.SortBy.ToString()) != null) {
                drpSortBy.SelectedValue = ArticleSettings.SortBy.ToString();
            }

            if (drpSortDirection.Items.FindByValue(ArticleSettings.SortDirection.ToString()) != null) {
                drpSortDirection.SelectedValue = ArticleSettings.SortDirection.ToString();
            }

            if (lstMenuPosition.Items.FindByValue(ArticleSettings.MenuPosition.ToString()) != null) {
                lstMenuPosition.SelectedValue = ArticleSettings.MenuPosition.ToString();
            }

        }

        private void SaveBasicSettings()
        {

            ModuleController objModules = new ModuleController();

            // General Configuration
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.ENABLE_RATINGS_AUTHENTICATED_SETTING, chkEnableRatingsAuthenticated.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.ENABLE_RATINGS_ANONYMOUS_SETTING, chkEnableRatingsAnonymous.Checked.ToString());

            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.ENABLE_CORE_SEARCH_SETTING, chkEnableCoreSearch.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PROCESS_POST_TOKENS, chkProcessPostTokens.Checked.ToString());

            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.ENABLE_NOTIFICATION_PING_SETTING, chkEnableNotificationPing.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.ENABLE_AUTO_TRACKBACK_SETTING, chkEnableAutoTrackback.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.ENABLE_INCOMING_TRACKBACK_SETTING, chkEnableIncomingTrackback.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.LAUNCH_LINKS, chkLaunchLinks.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.BUBBLE_FEATURED_ARTICLES, chkBubbleFeaturedArticles.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.REQUIRE_CATEGORY, chkRequireCategory.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.DISPLAY_MODE, ((DisplayType)System.Enum.Parse(typeof(DisplayType), drpDisplayType.SelectedValue)).ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.PAGE_SIZE_SETTING, drpNumber.SelectedValue);
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.TEMPLATE_SETTING, drpTemplates.SelectedValue);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.SERVER_TIMEZONE, drpTimeZone.SelectedValue);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.SORT_BY, drpSortBy.SelectedValue);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.SORT_DIRECTION, drpSortDirection.SelectedValue);
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.MENU_POSITION_TYPE, lstMenuPosition.SelectedValue);

            // Clear Cache
            DataCache.RemoveCache(TabModuleId.ToString() + TemplateConstants.LISTING_ITEM);
            DataCache.RemoveCache(TabModuleId.ToString() + TemplateConstants.LISTING_FEATURED);
            DataCache.RemoveCache(TabModuleId.ToString() + TemplateConstants.LISTING_HEADER);
            DataCache.RemoveCache(TabModuleId.ToString() + TemplateConstants.LISTING_FOOTER);

            DataCache.RemoveCache(TabModuleId.ToString() + TemplateConstants.VIEW_ITEM);
            DataCache.RemoveCache(TabModuleId.ToString() + TemplateConstants.VIEW_HEADER);
            DataCache.RemoveCache(TabModuleId.ToString() + TemplateConstants.VIEW_FOOTER);

            DataCache.RemoveCache(TabModuleId.ToString() + TemplateConstants.COMMENT_ITEM);
            DataCache.RemoveCache(TabModuleId.ToString() + TemplateConstants.COMMENT_HEADER);
            DataCache.RemoveCache(TabModuleId.ToString() + TemplateConstants.COMMENT_FOOTER);

            DataCache.RemoveCache(TabModuleId.ToString() + TemplateConstants.MENU_ITEM);

        }

        #endregion

        #region " private Methods - Bind/Save Archive Settings "

        private void BindArchiveSettings()
        {

            chkArchiveCurrentArticles.Checked = ArticleSettings.ArchiveCurrentArticles;
            chkArchiveCategories.Checked = ArticleSettings.ArchiveCategories;
            chkArchiveAuthor.Checked = ArticleSettings.ArchiveAuthor;
            chkArchiveMonth.Checked = ArticleSettings.ArchiveMonth;

        }

        private void SaveArchiveSettings()
        {

            ModuleController objModules = new ModuleController();

            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.ARCHIVE_CURRENT_ARTICLES_SETTING, chkArchiveCurrentArticles.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.ARCHIVE_CATEGORIES_SETTING, chkArchiveCategories.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.ARCHIVE_AUTHOR_SETTING, chkArchiveAuthor.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.ARCHIVE_MONTH_SETTING, chkArchiveMonth.Checked.ToString());

        }


        #endregion

        #region " private Methods - Bind/Save Category Settings "

        private void BindCategorySettings()
        {

            if (Settings.Contains(ArticleConstants.DEFAULT_CATEGORIES_SETTING))
            {
                if (Settings[ArticleConstants.DEFAULT_CATEGORIES_SETTING].ToString() != Null.NullString)
                {
                    string[] categories = Settings[ArticleConstants.DEFAULT_CATEGORIES_SETTING].ToString().Split(',');

                    foreach (string category in categories)
                    {
                        if (lstDefaultCategories.Items.FindByValue(category) != null)
                        {
                            lstDefaultCategories.Items.FindByValue(category).Selected = true;
                        }
                    }
                }
            }

            txtCategorySelectionHeight.Text = ArticleSettings.CategorySelectionHeight.ToString();
            chkCategoryBreadcrumb.Checked = ArticleSettings.CategoryBreadcrumb;
            chkArticleBreadcrumb.Checked = ArticleSettings.ArticleBreadcrumb;
            chkCategoryName.Checked = ArticleSettings.IncludeInPageName;
            chkCategoryFilterSubmit.Checked = ArticleSettings.CategoryFilterSubmit;

            if (lstCategorySortOrder.Items.FindByValue(ArticleSettings.CategorySortType.ToString()) != null)
            {
                lstCategorySortOrder.SelectedValue = ArticleSettings.CategorySortType.ToString();
            }

        }

        private void SaveCategorySettings()
        {

            ModuleController objModules = new ModuleController();

            string categories = "";
            foreach (ListItem item in lstDefaultCategories.Items)
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

            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.DEFAULT_CATEGORIES_SETTING, categories);

            if (Numeric.IsNumeric(txtCategorySelectionHeight.Text))
            {
                objModules.UpdateModuleSetting(ModuleId, ArticleConstants.CATEGORY_SELECTION_HEIGHT_SETTING, txtCategorySelectionHeight.Text);
            }

            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.CATEGORY_BREADCRUMB_SETTING, chkCategoryBreadcrumb.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.ARTICLE_BREADCRUMB_SETTING, chkArticleBreadcrumb.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.CATEGORY_NAME_SETTING, chkCategoryName.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.CATEGORY_FILTER_SUBMIT_SETTING, chkCategoryFilterSubmit.Checked.ToString());

            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.CATEGORY_SORT_SETTING, lstCategorySortOrder.SelectedValue);

        }

        #endregion

        private void BindCommentSettings()
        {

            if (Settings.Contains(ArticleConstants.ENABLE_COMMENTS_SETTING))
            {
                chkEnableComments.Checked = Convert.ToBoolean(Settings[ArticleConstants.ENABLE_COMMENTS_SETTING].ToString());
            }
            else
            {
                chkEnableComments.Checked = true;
            }

            if (Settings.Contains(ArticleConstants.ENABLE_ANONYMOUS_COMMENTS_SETTING))
            {
                chkEnableAnonymousComments.Checked = Convert.ToBoolean(Settings[ArticleConstants.ENABLE_ANONYMOUS_COMMENTS_SETTING].ToString());
            }
            else
            {
                chkEnableAnonymousComments.Checked = true;
            }

            if (Settings.Contains(ArticleConstants.ENABLE_COMMENT_MODERATION_SETTING))
            {
                chkEnableCommentModeration.Checked = Convert.ToBoolean(Settings[ArticleConstants.ENABLE_COMMENT_MODERATION_SETTING].ToString());
            }
            else
            {
                chkEnableCommentModeration.Checked = false;
            }

            if (Settings.Contains(ArticleConstants.COMMENT_HIDE_WEBSITE_SETTING))
            {
                chkHideWebsite.Checked = Convert.ToBoolean(Settings[ArticleConstants.COMMENT_HIDE_WEBSITE_SETTING].ToString());
            }
            else
            {
                chkHideWebsite.Checked = false;
            }

            if (Settings.Contains(ArticleConstants.COMMENT_REQUIRE_NAME_SETTING))
            {
                chkRequireName.Checked = Convert.ToBoolean(Settings[ArticleConstants.COMMENT_REQUIRE_NAME_SETTING].ToString());
            }
            else
            {
                chkRequireName.Checked = true;
            }

            if (Settings.Contains(ArticleConstants.COMMENT_REQUIRE_EMAIL_SETTING))
            {
                chkRequireEmail.Checked = Convert.ToBoolean(Settings[ArticleConstants.COMMENT_REQUIRE_EMAIL_SETTING].ToString());
            }
            else
            {
                chkRequireEmail.Checked = true;
            }

            if (Settings.Contains(ArticleConstants.USE_CAPTCHA_SETTING))
            {
                chkUseCaptcha.Checked = Convert.ToBoolean(Settings[ArticleConstants.USE_CAPTCHA_SETTING].ToString());
            }
            else
            {
                chkUseCaptcha.Checked = false;
            }

            if (Settings.Contains(ArticleConstants.NOTIFY_DEFAULT_SETTING))
            {
                chkNotifyDefault.Checked = Convert.ToBoolean(Settings[ArticleConstants.NOTIFY_DEFAULT_SETTING].ToString());
            }
            else
            {
                chkNotifyDefault.Checked = false;
            }

            if (drpSortDirectionComments.Items.FindByValue(ArticleSettings.SortDirectionComments.ToString()) != null)
            {
                drpSortDirectionComments.SelectedValue = ArticleSettings.SortDirectionComments.ToString();
            }

            if (Settings.Contains(ArticleConstants.COMMENT_AKISMET_SETTING))
            {
                txtAkismetKey.Text = Settings[ArticleConstants.COMMENT_AKISMET_SETTING].ToString();
            }

        }

        private void SaveCommentSettings()
        {

            ModuleController objModules = new ModuleController();

            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.ENABLE_COMMENTS_SETTING, chkEnableComments.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.ENABLE_ANONYMOUS_COMMENTS_SETTING, chkEnableAnonymousComments.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.ENABLE_COMMENT_MODERATION_SETTING, chkEnableCommentModeration.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.COMMENT_HIDE_WEBSITE_SETTING, chkHideWebsite.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.COMMENT_REQUIRE_NAME_SETTING, chkRequireName.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.COMMENT_REQUIRE_EMAIL_SETTING, chkRequireEmail.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.USE_CAPTCHA_SETTING, chkUseCaptcha.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NOTIFY_DEFAULT_SETTING, chkNotifyDefault.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.COMMENT_SORT_DIRECTION_SETTING, drpSortDirectionComments.SelectedValue);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.COMMENT_AKISMET_SETTING, txtAkismetKey.Text);

        }

        private void BindContentSharingSettings()
        {

            if (!this.UserInfo.IsSuperUser)
            {
                phContentSharing.Visible = false;
            }
            else
            {
                BindAvailableContentSharingPortals();
                BindSelectedContentSharingPortals();
            }

        }

        private void SaveContentSharingSettings()
        {

        }

        private void BindFilterSettings()
        {

            if (ArticleSettings.MaxArticles != Null.NullInteger)
            {
                txtMaxArticles.Text = ArticleSettings.MaxArticles.ToString();
            }

            if (ArticleSettings.MaxAge != Null.NullInteger)
            {
                txtMaxAge.Text = ArticleSettings.MaxAge.ToString();
            }

            if (ArticleSettings.FilterSingleCategory == Null.NullInteger)
            {
                if (ArticleSettings.FilterCategories != null)
                {
                    foreach (int category in ArticleSettings.FilterCategories)
                    {
                        if (lstCategories.Items.FindByValue(category.ToString()) != null)
                        {
                            lstCategories.Items.FindByValue(category.ToString()).Selected = true;
                        }
                    }

                    if (ArticleSettings.MatchCategories == MatchOperatorType.MatchAny)
                    {
                        rdoMatchAny.Checked = true;
                    }

                    if (ArticleSettings.MatchCategories == MatchOperatorType.MatchAll)
                    {
                        rdoMatchAll.Checked = true;
                    }
                }
                else
                {
                    rdoAllCategories.Checked = true;
                }
            }
            else
            {
                rdoSingleCategory.Checked = true;

                if (drpCategories.Items.FindByValue(ArticleSettings.FilterSingleCategory.ToString()) != null)
                {
                    drpCategories.SelectedValue = ArticleSettings.FilterSingleCategory.ToString();
                }
            }

            chkShowPending.Checked = ArticleSettings.ShowPending;

            chkShowFeaturedOnly.Checked = ArticleSettings.FeaturedOnly;
            chkShowNotFeaturedOnly.Checked = ArticleSettings.NotFeaturedOnly;

            chkShowSecuredOnly.Checked = ArticleSettings.SecuredOnly;
            chkShowNotSecuredOnly.Checked = ArticleSettings.NotSecuredOnly;

            if (ArticleSettings.Author != Null.NullInteger)
            {
                UserController objUserController = new UserController();
                UserInfo objUser = objUserController.GetUser(this.PortalId, ArticleSettings.Author);

                if (objUser != null)
                {
                    lblAuthorFilter.Text = objUser.Username;
                }
            }
            else
            {
                lblAuthorFilter.Text = Localization.GetString("SelectAuthor.Text", this.LocalResourceFile);
            }

            chkQueryStringFilter.Checked = ArticleSettings.AuthorUserIDFilter;
            txtQueryStringParam.Text = ArticleSettings.AuthorUserIDParam;
            chkUsernameFilter.Checked = ArticleSettings.AuthorUsernameFilter;
            txtUsernameParam.Text = ArticleSettings.AuthorUsernameParam;
            chkLoggedInUser.Checked = ArticleSettings.AuthorLoggedInUserFilter;

        }

        private void SaveFilterSettings()
        {

            ModuleController objModules = new ModuleController();

            if (txtMaxArticles.Text != "")
            {
                objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.MAX_ARTICLES_SETTING, txtMaxArticles.Text);
            }
            else
            {
                objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.MAX_ARTICLES_SETTING, Null.NullInteger.ToString());
            }

            if (txtMaxAge.Text != "")
            {
                if (Numeric.IsNumeric(txtMaxAge.Text))
                {
                    objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.MAX_AGE_SETTING, txtMaxAge.Text);
                }
                else
                {
                    objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.MAX_AGE_SETTING, Null.NullInteger.ToString());
                }
            }
            else
            {
                objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.MAX_AGE_SETTING, Null.NullInteger.ToString());
            }

            if (rdoAllCategories.Checked)
            {
                objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.CATEGORIES_SETTING + this.TabId.ToString(), Null.NullInteger.ToString());
            }

            if (rdoSingleCategory.Checked)
            {
                if (drpCategories.SelectedValue != "")
                {
                    objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.CATEGORIES_FILTER_SINGLE_SETTING, drpCategories.SelectedValue);
                }
                else
                {
                    objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.CATEGORIES_FILTER_SINGLE_SETTING, Null.NullInteger.ToString());
                }
            }
            else
            {
                objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.CATEGORIES_FILTER_SINGLE_SETTING, Null.NullInteger.ToString());
            }

            if (rdoMatchAny.Checked || rdoMatchAll.Checked)
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
                objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.CATEGORIES_SETTING + this.TabId.ToString(), categories);

                if (rdoMatchAny.Checked)
                {
                    objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.MATCH_OPERATOR_SETTING, MatchOperatorType.MatchAny.ToString());
                }

                if (rdoMatchAll.Checked)
                {
                    objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.MATCH_OPERATOR_SETTING, MatchOperatorType.MatchAll.ToString());
                }
            }

            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.SHOW_PENDING_SETTING, chkShowPending.Checked.ToString());

            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.SHOW_FEATURED_ONLY_SETTING, chkShowFeaturedOnly.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.SHOW_NOT_FEATURED_ONLY_SETTING, chkShowNotFeaturedOnly.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.SHOW_SECURED_ONLY_SETTING, chkShowSecuredOnly.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.SHOW_NOT_SECURED_ONLY_SETTING, chkShowNotSecuredOnly.Checked.ToString());

            if (ddlAuthor.Visible)
            {
                if (ddlAuthor.Items.Count > 0)
                {
                    objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.AUTHOR_SETTING, ddlAuthor.SelectedValue);
                }
            }

            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.AUTHOR_USERID_FILTER_SETTING, chkQueryStringFilter.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.AUTHOR_USERID_PARAM_SETTING, txtQueryStringParam.Text);
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.AUTHOR_USERNAME_FILTER_SETTING, chkUsernameFilter.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.AUTHOR_USERNAME_PARAM_SETTING, txtUsernameParam.Text);
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.AUTHOR_LOGGED_IN_USER_FILTER_SETTING, chkLoggedInUser.Checked.ToString());

        }


        private void BindFormSettings()
        {

            if (lstAuthorSelection.Items.FindByValue(ArticleSettings.AuthorSelect.ToString()) != null)
            {
                lstAuthorSelection.SelectedValue = ArticleSettings.AuthorSelect.ToString();
            }

            if (ArticleSettings.AuthorDefault != Null.NullInteger)
            {
                UserController objUserController = new DotNetNuke.Entities.Users.UserController();
                UserInfo objUser = objUserController.GetUser(this.PortalId, ArticleSettings.AuthorDefault);

                if (objUser != null)
                {
                    lblAuthorDefault.Text = objUser.Username;
                }
            }

            chkExpandSummary.Checked = ArticleSettings.ExpandSummary;
            txtTextEditorWidth.Text = ArticleSettings.TextEditorWidth;
            txtTextEditorHeight.Text = ArticleSettings.TextEditorHeight;
            if (lstTextEditorSummaryMode.Items.FindByValue(ArticleSettings.TextEditorSummaryMode.ToString()) != null)
            {
                lstTextEditorSummaryMode.SelectedValue = ArticleSettings.TextEditorSummaryMode.ToString();
            }
            txtTextEditorSummaryWidth.Text = ArticleSettings.TextEditorSummaryWidth;
            txtTextEditorSummaryHeight.Text = ArticleSettings.TextEditorSummaryHeight;

        }

        private void SaveFormSettings()
        {

            ModuleController objModules = new ModuleController();

            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.AUTHOR_SELECT_TYPE, lstAuthorSelection.SelectedValue);
            if (drpAuthorDefault.Visible)
            {
                if (drpAuthorDefault.Items.Count > 0)
                {
                    objModules.UpdateModuleSetting(ModuleId, ArticleConstants.AUTHOR_DEFAULT_SETTING, drpAuthorDefault.SelectedValue);
                }
            }
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.EXPAND_SUMMARY_SETTING, chkExpandSummary.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.TEXT_EDITOR_WIDTH, txtTextEditorWidth.Text);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.TEXT_EDITOR_HEIGHT, txtTextEditorHeight.Text);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.TEXT_EDITOR_SUMMARY_MODE, lstTextEditorSummaryMode.SelectedValue);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.TEXT_EDITOR_SUMMARY_WIDTH, txtTextEditorSummaryWidth.Text);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.TEXT_EDITOR_SUMMARY_HEIGHT, txtTextEditorSummaryHeight.Text);

        }

        private void BindImageSettings()
        {

            chkIncludeJQuery.Checked = ArticleSettings.IncludeJQuery;
            txtJQueryPath.Text = this.ArticleSettings.ImageJQueryPath;
            chkEnableImagesUpload.Checked = ArticleSettings.EnableImagesUpload;
            chkEnablePortalImages.Checked = ArticleSettings.EnablePortalImages;
            chkEnableExternalImages.Checked = ArticleSettings.EnableExternalImages;
            if (drpDefaultImageFolder.Items.FindByValue(ArticleSettings.DefaultImagesFolder.ToString()) != null)
            {
                drpDefaultImageFolder.SelectedValue = ArticleSettings.DefaultImagesFolder.ToString();
            }
            chkResizeImages.Checked = ArticleSettings.ResizeImages;
            if (rdoThumbnailType.Items.FindByValue(ArticleSettings.ImageThumbnailType.ToString()) != null)
            {
                rdoThumbnailType.SelectedValue = ArticleSettings.ImageThumbnailType.ToString();
            }
            else
            {
                rdoThumbnailType.SelectedIndex = 0;
            }
            txtImageMaxWidth.Text = ArticleSettings.MaxImageWidth.ToString();
            txtImageMaxHeight.Text = ArticleSettings.MaxImageHeight.ToString();

            chkUseWatermark.Checked = this.ArticleSettings.WatermarkEnabled;
            txtWatermarkText.Text = this.ArticleSettings.WatermarkText;
            ctlWatermarkImage.Url = this.ArticleSettings.WatermarkImage;
            if (drpWatermarkPosition.Items.FindByValue(ArticleSettings.WatermarkPosition.ToString()) != null)
            {
                drpWatermarkPosition.SelectedValue = ArticleSettings.WatermarkPosition.ToString();
            }

        }

        private void SaveImageSettings()
        {

            ModuleController objModules = new ModuleController();

            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.INCLUDE_JQUERY_SETTING, chkIncludeJQuery.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.IMAGE_JQUERY_PATH, txtJQueryPath.Text);
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.ENABLE_UPLOAD_IMAGES_SETTING, chkEnableImagesUpload.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.ENABLE_PORTAL_IMAGES_SETTING, chkEnablePortalImages.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.ENABLE_EXTERNAL_IMAGES_SETTING, chkEnableExternalImages.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.DEFAULT_IMAGES_FOLDER_SETTING, drpDefaultImageFolder.SelectedValue);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.IMAGE_RESIZE_SETTING, chkResizeImages.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.IMAGE_THUMBNAIL_SETTING, rdoThumbnailType.SelectedValue);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.IMAGE_MAX_WIDTH_SETTING, txtImageMaxWidth.Text);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.IMAGE_MAX_HEIGHT_SETTING, txtImageMaxHeight.Text);

            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.IMAGE_WATERMARK_ENABLED_SETTING, chkUseWatermark.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.IMAGE_WATERMARK_TEXT_SETTING, txtWatermarkText.Text);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.IMAGE_WATERMARK_IMAGE_SETTING, ctlWatermarkImage.Url);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.IMAGE_WATERMARK_IMAGE_POSITION_SETTING, drpWatermarkPosition.SelectedValue);

        }

        private void BindFileSettings()
        {

            if (drpDefaultFileFolder.Items.FindByValue(ArticleSettings.DefaultFilesFolder.ToString()) != null)
            {
                drpDefaultFileFolder.SelectedValue = ArticleSettings.DefaultFilesFolder.ToString();
            }

        }

        private void SaveFileSettings()
        {

            ModuleController objModules = new ModuleController();
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.DEFAULT_FILES_FOLDER_SETTING, drpDefaultFileFolder.SelectedValue);

        }

        private void BindSecuritySettings()
        {

            txtSecureUrl.Text = ArticleSettings.SecureUrl.ToString();
            if (ArticleSettings.RoleGroupID != Null.NullInteger)
            {
                if (drpSecurityRoleGroups.Items.FindByValue(ArticleSettings.RoleGroupID.ToString()) != null)
                {
                    drpSecurityRoleGroups.SelectedValue = ArticleSettings.RoleGroupID.ToString();
                }
            }

            BindRoles();

        }

        private void SaveSecuritySettings()
        {

            ModuleController objModules = new ModuleController();

            string submitRoles = "";
            string secureRoles = "";
            string autoSecureRoles = "";
            string approveRoles = "";
            string autoApproveRoles = "";
            string autoApproveCommentRoles = "";
            string featureRoles = "";
            string autoFeatureRoles = "";
            foreach (DataGridItem item in grdBasicPermissions.Items)
            {
                string role = grdBasicPermissions.DataKeys[item.ItemIndex].ToString();

                CheckBox chkSubmit = (CheckBox)item.FindControl("chkSubmit");
                if (chkSubmit.Checked)
                {
                    if (submitRoles == "")
                    {
                        submitRoles = role;
                    }
                    else
                    {
                        submitRoles = submitRoles + ";" + role;
                    }
                }

                CheckBox chkSecure = (CheckBox)item.FindControl("chkSecure");
                if (chkSecure.Checked)
                {
                    if (secureRoles == "")
                    {
                        secureRoles = role;
                    }
                    else
                    {
                        secureRoles = secureRoles + ";" + role;
                    }
                }

                CheckBox chkAutoSecure = (CheckBox)item.FindControl("chkAutoSecure");
                if (chkAutoSecure.Checked)
                {
                    if (autoSecureRoles == "")
                    {
                        autoSecureRoles = role;
                    }
                    else
                    {
                        autoSecureRoles = autoSecureRoles + ";" + role;
                    }
                }

                CheckBox chkApprove = (CheckBox)item.FindControl("chkApprove");
                if (chkApprove.Checked)
                {
                    if (approveRoles == "")
                    {
                        approveRoles = role;
                    }
                    else
                    {
                        approveRoles = approveRoles + ";" + role;
                    }
                }

                CheckBox chkAutoApproveArticle = (CheckBox)item.FindControl("chkAutoApproveArticle");
                if (chkAutoApproveArticle.Checked)
                {
                    if (autoApproveRoles == "")
                    {
                        autoApproveRoles = role;
                    }
                    else
                    {
                        autoApproveRoles = autoApproveRoles + ";" + role;
                    }
                }

                CheckBox chkAutoApproveComment = (CheckBox)item.FindControl("chkAutoApproveComment");
                if (chkAutoApproveComment.Checked)
                {
                    if (autoApproveCommentRoles == "")
                    {
                        autoApproveCommentRoles = role;
                    }
                    else
                    {
                        autoApproveCommentRoles = autoApproveCommentRoles + ";" + role;
                    }
                }

                CheckBox chkFeature = (CheckBox)item.FindControl("chkFeature");
                if (chkFeature.Checked)
                {
                    if (featureRoles == "")
                    {
                        featureRoles = role;
                    }
                    else
                    {
                        featureRoles = featureRoles + ";" + role;
                    }
                }

                CheckBox chkAutoFeature = (CheckBox)item.FindControl("chkAutoFeature");
                if (chkAutoFeature.Checked)
                {
                    if (autoFeatureRoles == "")
                    {
                        autoFeatureRoles = role;
                    }
                    else
                    {
                        autoFeatureRoles = autoFeatureRoles + ";" + role;
                    }
                }
            }
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_SUBMISSION_SETTING, submitRoles);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_SECURE_SETTING, secureRoles);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_AUTO_SECURE_SETTING, autoSecureRoles);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_APPROVAL_SETTING, approveRoles);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_AUTO_APPROVAL_SETTING, autoApproveRoles);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_AUTO_APPROVAL_COMMENT_SETTING, autoApproveCommentRoles);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_FEATURE_SETTING, featureRoles);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_AUTO_FEATURE_SETTING, autoFeatureRoles);

            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_ROLE_GROUP_ID, drpSecurityRoleGroups.SelectedValue);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_SECURE_URL_SETTING, txtSecureUrl.Text);

            string categoriesRoles = "";
            string excerptRoles = "";
            string imageRoles = "";
            string fileRoles = "";
            string linkRoles = "";
            string publishRoles = "";
            string expiryRoles = "";
            string metaRoles = "";
            string customRoles = "";

            foreach (DataGridItem item in grdFormPermissions.Items)
            {
                string role = grdFormPermissions.DataKeys[item.ItemIndex].ToString();

                CheckBox chkCategories = (CheckBox)item.FindControl("chkCategories");
                if (chkCategories.Checked)
                {
                    if (categoriesRoles == "")
                    {
                        categoriesRoles = role;
                    }
                    else
                    {
                        categoriesRoles = categoriesRoles + ";" + role;
                    }
                }

                CheckBox chkExcerpt = (CheckBox)item.FindControl("chkExcerpt");
                if (chkExcerpt.Checked)
                {
                    if (excerptRoles == "")
                    {
                        excerptRoles = role;
                    }
                    else
                    {
                        excerptRoles = excerptRoles + ";" + role;
                    }
                }

                CheckBox chkImage = (CheckBox)item.FindControl("chkImage");
                if (chkImage.Checked)
                {
                    if (imageRoles == "")
                    {
                        imageRoles = role;
                    }
                    else
                    {
                        imageRoles = imageRoles + ";" + role;
                    }
                }

                CheckBox chkFile = (CheckBox)item.FindControl("chkFile");
                if (chkFile.Checked)
                {
                    if (fileRoles == "")
                    {
                        fileRoles = role;
                    }
                    else
                    {
                        fileRoles = fileRoles + ";" + role;
                    }
                }

                CheckBox chkLink = (CheckBox)item.FindControl("chkLink");
                if (chkLink.Checked)
                {
                    if (linkRoles == "")
                    {
                        linkRoles = role;
                    }
                    else
                    {
                        linkRoles = linkRoles + ";" + role;
                    }
                }

                CheckBox chkPublishDate = (CheckBox)item.FindControl("chkPublishDate");
                if (chkPublishDate.Checked)
                {
                    if (publishRoles == "")
                    {
                        publishRoles = role;
                    }
                    else
                    {
                        publishRoles = publishRoles + ";" + role;
                    }
                }

                CheckBox chkExpiryDate = (CheckBox)item.FindControl("chkExpiryDate");
                if (chkExpiryDate.Checked)
                {
                    if (expiryRoles == "")
                    {
                        expiryRoles = role;
                    }
                    else
                    {
                        expiryRoles = expiryRoles + ";" + role;
                    }
                }

                CheckBox chkMeta = (CheckBox)item.FindControl("chkMeta");
                if (chkMeta.Checked)
                {
                    if (metaRoles == "")
                    {
                        metaRoles = role;
                    }
                    else
                    {
                        metaRoles = metaRoles + ";" + role;
                    }
                }

                CheckBox chkCustom = (CheckBox)item.FindControl("chkCustom");
                if (chkCustom.Checked)
                {
                    if (customRoles == "")
                    {
                        customRoles = role;
                    }
                    else
                    {
                        customRoles = customRoles + ";" + role;
                    }
                }
            }
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_CATEGORIES_SETTING, categoriesRoles);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_EXCERPT_SETTING, excerptRoles);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_IMAGE_SETTING, imageRoles);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_FILE_SETTING, fileRoles);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_LINK_SETTING, linkRoles);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_PUBLISH_SETTING, publishRoles);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_EXPIRY_SETTING, expiryRoles);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_META_SETTING, metaRoles);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_CUSTOM_SETTING, customRoles);

            string siteTemplatesRoles = "";
            foreach (DataGridItem item in grdAdminPermissions.Items)
            {
                string role = grdAdminPermissions.DataKeys[item.ItemIndex].ToString();

                CheckBox chkSiteTemplates = (CheckBox)item.FindControl("chkSiteTemplates");
                if (chkSiteTemplates.Checked)
                {
                    if (siteTemplatesRoles == "")
                    {
                        siteTemplatesRoles = role;
                    }
                    else
                    {
                        siteTemplatesRoles = siteTemplatesRoles + ";" + role;
                    }
                }
            }
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.PERMISSION_SITE_TEMPLATES_SETTING, siteTemplatesRoles);

        }

        private void BindNotificationSettings()
        {

            if (Settings.Contains(ArticleConstants.NOTIFY_SUBMISSION_SETTING))
            {
                chkNotifySubmission.Checked = Convert.ToBoolean(Settings[ArticleConstants.NOTIFY_SUBMISSION_SETTING].ToString());
            }
            else
            {
                chkNotifySubmission.Checked = true;
            }

            if (Settings.Contains(ArticleConstants.NOTIFY_SUBMISSION_SETTING_EMAIL))
            {
                txtSubmissionEmail.Text = Settings[ArticleConstants.NOTIFY_SUBMISSION_SETTING_EMAIL].ToString();
            }

            if (Settings.Contains(ArticleConstants.NOTIFY_APPROVAL_SETTING))
            {
                chkNotifyApproval.Checked = Convert.ToBoolean(Settings[ArticleConstants.NOTIFY_APPROVAL_SETTING].ToString());
            }
            else
            {
                chkNotifyApproval.Checked = true;
            }

            if (Settings.Contains(ArticleConstants.NOTIFY_COMMENT_SETTING))
            {
                chkNotifyComment.Checked = Convert.ToBoolean(Settings[ArticleConstants.NOTIFY_COMMENT_SETTING].ToString());
            }
            else
            {
                chkNotifyComment.Checked = true;
            }

            if (Settings.Contains(ArticleConstants.NOTIFY_COMMENT_SETTING_EMAIL))
            {
                txtNotifyCommentEmail.Text = Settings[ArticleConstants.NOTIFY_COMMENT_SETTING_EMAIL].ToString();
            }

            if (Settings.Contains(ArticleConstants.NOTIFY_COMMENT_APPROVAL_SETTING))
            {
                chkNotifyCommentApproval.Checked = Convert.ToBoolean(Settings[ArticleConstants.NOTIFY_COMMENT_APPROVAL_SETTING].ToString());
            }
            else
            {
                chkNotifyCommentApproval.Checked = true;
            }

            if (Settings.Contains(ArticleConstants.NOTIFY_COMMENT_APPROVAL_EMAIL_SETTING))
            {
                txtNotifyCommentApprovalEmail.Text = Settings[ArticleConstants.NOTIFY_COMMENT_APPROVAL_EMAIL_SETTING].ToString();
            }

        }

        private void SaveNotificationSettings()
        {

            ModuleController objModules = new ModuleController();

            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.NOTIFY_SUBMISSION_SETTING, chkNotifySubmission.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.NOTIFY_SUBMISSION_SETTING_EMAIL, txtSubmissionEmail.Text.Trim().ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.NOTIFY_APPROVAL_SETTING, chkNotifyApproval.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.NOTIFY_COMMENT_SETTING, chkNotifyComment.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.NOTIFY_COMMENT_SETTING_EMAIL, txtNotifyCommentEmail.Text.Trim().ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.NOTIFY_COMMENT_APPROVAL_SETTING, chkNotifyCommentApproval.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.NOTIFY_COMMENT_APPROVAL_EMAIL_SETTING, txtNotifyCommentApprovalEmail.Text.Trim().ToString());

        }

        private void BindRelatedSettings()
        {

            if (lstRelatedMode.Items.FindByValue(RelatedType.MatchCategoriesAnyTagsAny.ToString()) != null)
            {
                lstRelatedMode.SelectedValue = RelatedType.MatchCategoriesAnyTagsAny.ToString();
            }

            if (Settings.Contains(ArticleConstants.RELATED_MODE))
            {
                if (lstRelatedMode.Items.FindByValue(Settings[ArticleConstants.RELATED_MODE].ToString()) != null)
                {
                    lstRelatedMode.SelectedValue = Settings[ArticleConstants.RELATED_MODE].ToString();
                }
            }

        }

        private void SaveRelatedSettings()
        {

            ModuleController objModules = new ModuleController();
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.RELATED_MODE, lstRelatedMode.SelectedValue.ToString());

        }

        private void BindSEOSettings()
        {

            if (lstTitleReplacement.Items.FindByValue(ArticleSettings.TitleReplacement.ToString()) != null)
            {
                lstTitleReplacement.SelectedValue = ArticleSettings.TitleReplacement.ToString();
            }

            chkAlwaysShowPageID.Checked = ArticleSettings.AlwaysShowPageID;

            if (lstUrlMode.Items.FindByValue(ArticleSettings.UrlModeType.ToString()) != null)
            {
                lstUrlMode.SelectedValue = ArticleSettings.UrlModeType.ToString();
            }

            txtShorternID.Text = ArticleSettings.ShortenedID;

            chkUseCanonicalLink.Checked = ArticleSettings.UseCanonicalLink;
            chkExpandMetaInformation.Checked = ArticleSettings.ExpandMetaInformation;
            chkUniquePageTitles.Checked = ArticleSettings.UniquePageTitles;

        }

        private void SaveSEOSettings()
        {

            ModuleController objModules = new ModuleController();
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.TITLE_REPLACEMENT_TYPE, lstTitleReplacement.SelectedValue);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.SEO_ALWAYS_SHOW_PAGEID_SETTING, chkAlwaysShowPageID.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.SEO_URL_MODE_SETTING, lstUrlMode.SelectedValue);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.SEO_SHORTEN_ID_SETTING, txtShorternID.Text);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.SEO_USE_CANONICAL_LINK_SETTING, chkUseCanonicalLink.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.SEO_EXPAND_META_INFORMATION_SETTING, chkExpandMetaInformation.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.SEO_UNIQUE_PAGE_TITLES_SETTING, chkUniquePageTitles.Checked.ToString());

        }

        private void BindSyndicationSettings()
        {

            chkEnableSyndication.Checked = ArticleSettings.EnableSyndication;
            chkEnableSyndicationEnclosures.Checked = ArticleSettings.EnableSyndicationEnclosures;
            chkEnableSyndicationHtml.Checked = ArticleSettings.EnableSyndicationHtml;

            if (lstSyndicationLinkMode.Items.FindByValue(ArticleSettings.SyndicationLinkType.ToString()) != null)
            {
                lstSyndicationLinkMode.SelectedValue = ArticleSettings.SyndicationLinkType.ToString();
            }

            if (lstSyndicationEnclosureType.Items.FindByValue(ArticleSettings.SyndicationEnclosureType.ToString()) != null)
            {
                lstSyndicationEnclosureType.SelectedValue = ArticleSettings.SyndicationEnclosureType.ToString();
            }

            if (ArticleSettings.SyndicationSummaryLength != Null.NullInteger)
            {
                txtSyndicationSummaryLength.Text = ArticleSettings.SyndicationSummaryLength.ToString();
            }

            txtSyndicationMaxCount.Text = ArticleSettings.SyndicationMaxCount.ToString();
            txtSyndicationImagePath.Text = ArticleSettings.SyndicationImagePath;

        }

        private void SaveSyndicationSettings()
        {

            ModuleController objModules = new ModuleController();

            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.ENABLE_SYNDICATION_SETTING, chkEnableSyndication.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.ENABLE_SYNDICATION_ENCLOSURES_SETTING, chkEnableSyndicationEnclosures.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.ENABLE_SYNDICATION_HTML_SETTING, chkEnableSyndicationHtml.Checked.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.SYNDICATION_LINK_TYPE, lstSyndicationLinkMode.SelectedValue.ToString());
            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.SYNDICATION_ENCLOSURE_TYPE, lstSyndicationEnclosureType.SelectedValue.ToString());

            objModules.DeleteModuleSetting(ModuleId, ArticleConstants.SYNDICATION_SUMMARY_LENGTH);
            if (txtSyndicationSummaryLength.Text != "")
            {
                if (Convert.ToInt32(txtSyndicationSummaryLength.Text) > 0)
                {
                    objModules.UpdateModuleSetting(ModuleId, ArticleConstants.SYNDICATION_SUMMARY_LENGTH, txtSyndicationSummaryLength.Text);
                }
            }

            try
            {
                objModules.UpdateModuleSetting(ModuleId, ArticleConstants.SYNDICATION_MAX_COUNT, txtSyndicationMaxCount.Text);
            }

            catch { }
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.SYNDICATION_IMAGE_PATH, txtSyndicationImagePath.Text);

        }

        private void BindTwitterSettings()
        {

            if (Settings.Contains(ArticleConstants.TWITTER_USERNAME))
            {
                txtTwitterName.Text = Settings[ArticleConstants.TWITTER_USERNAME].ToString();
            }

            if (Settings.Contains(ArticleConstants.TWITTER_BITLY_LOGIN))
            {
                txtBitLyLogin.Text = Settings[ArticleConstants.TWITTER_BITLY_LOGIN].ToString();
            }

            if (Settings.Contains(ArticleConstants.TWITTER_BITLY_API_KEY))
            {
                txtBitLyAPIKey.Text = Settings[ArticleConstants.TWITTER_BITLY_API_KEY].ToString();
            }

        }

        private void SaveTwitterSettings()
        {

            ModuleController objModules = new ModuleController();
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.TWITTER_USERNAME, txtTwitterName.Text);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.TWITTER_BITLY_LOGIN, txtBitLyLogin.Text);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.TWITTER_BITLY_API_KEY, txtBitLyAPIKey.Text);

        }

        private void BindThirdPartySettings()
        {

            chkJournalIntegration.Checked = ArticleSettings.JournalIntegration;
            chkJournalIntegrationGroups.Checked = ArticleSettings.JournalIntegrationGroups;

            if (Settings.Contains(ArticleConstants.ACTIVE_SOCIAL_SETTING))
            {
                chkActiveSocial.Checked = Convert.ToBoolean(Settings[ArticleConstants.ACTIVE_SOCIAL_SETTING].ToString());
            }
            else
            {
                chkActiveSocial.Checked = false;
            }

            txtActiveSocialSubmissionKey.Text = ArticleSettings.ActiveSocialSubmitKey;
            txtActiveSocialCommentKey.Text = ArticleSettings.ActiveSocialCommentKey;
            txtActiveSocialRateKey.Text = ArticleSettings.ActiveSocialRateKey;

            if (Settings.Contains(ArticleConstants.SMART_THINKER_STORY_FEED_SETTING))
            {
                chkSmartThinkerStoryFeed.Checked = Convert.ToBoolean(Settings[ArticleConstants.SMART_THINKER_STORY_FEED_SETTING].ToString());
            }
            else
            {
                chkSmartThinkerStoryFeed.Checked = false;
            }

        }

        private void SaveThirdPartySettings()
        {

            ModuleController objModules = new ModuleController();

            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.JOURNAL_INTEGRATION_SETTING, chkJournalIntegration.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.JOURNAL_INTEGRATION_GROUPS_SETTING, chkJournalIntegrationGroups.Checked.ToString());

            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.ACTIVE_SOCIAL_SETTING, chkActiveSocial.Checked.ToString());
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.ACTIVE_SOCIAL_SUBMIT_SETTING, txtActiveSocialSubmissionKey.Text);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.ACTIVE_SOCIAL_COMMENT_SETTING, txtActiveSocialCommentKey.Text);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.ACTIVE_SOCIAL_RATE_SETTING, txtActiveSocialRateKey.Text);
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.SMART_THINKER_STORY_FEED_SETTING, chkSmartThinkerStoryFeed.Checked.ToString());

        }

        private bool IsInRole(string roleName, string[] roles)
        {

            foreach (string role in roles)
            {
                if (roleName == role)
                {
                    return true;
                }
            }

            return false;

        }

        private void PopulateAuthorList()
        {

            ddlAuthor.DataSource = GetAuthorList(this.ModuleId);
            ddlAuthor.DataBind();
            SortDropDown(ddlAuthor);
            ddlAuthor.Items.Insert(0, new ListItem(Localization.GetString("SelectAuthor.Text", this.LocalResourceFile), "-1"));

            if (ddlAuthor.Items.FindByValue(this.ArticleSettings.Author.ToString()) != null)
            {
                ddlAuthor.SelectedValue = this.ArticleSettings.Author.ToString();
            }

        }

        private void PopulateAuthorListDefault()
        {

            drpAuthorDefault.DataSource = GetAuthorList(this.ModuleId);
            drpAuthorDefault.DataBind();
            SortDropDown(drpAuthorDefault);
            drpAuthorDefault.Items.Insert(0, new ListItem(Localization.GetString("NoDefault.Text", this.LocalResourceFile), "-1"));

            if (drpAuthorDefault.Items.FindByValue(this.ArticleSettings.AuthorDefault.ToString()) != null)
            {
                drpAuthorDefault.SelectedValue = this.ArticleSettings.AuthorDefault.ToString();
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

                        RoleController objRoleController = new DotNetNuke.Security.Roles.RoleController();
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
                                        if (objSelectedUser.Email.Length > 0)
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

        private void BindAvailableContentSharingPortals()
        {

            drpContentSharingPortals.Items.Clear();

            List<ContentSharingInfo> objContentSharingPortals = GetContentSharingPortals(ArticleSettings.ContentSharingPortals);

            PortalController objPortalController = new PortalController();
            ArrayList objPortals = objPortalController.GetPortals();

            foreach (PortalInfo objPortal in objPortals)
            {

                if (objPortal.PortalID != this.PortalId)
                {

                    DesktopModuleController objDesktopModuleController = new DesktopModuleController();
                    DesktopModuleInfo objDesktopModuleInfo = objDesktopModuleController.GetDesktopModuleByModuleName("GcDesign-NewsArticles");

                    if (objDesktopModuleInfo != null)
                    {

                        TabController objTabController = new TabController();
                        ArrayList objTabs = objTabController.GetTabs(objPortal.PortalID);
                        foreach (TabInfo objTab in objTabs)
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
                                                if (!objModule.IsDeleted)
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

                                                    bool add = true;

                                                    foreach (ContentSharingInfo objContentSharingPortal in objContentSharingPortals)
                                                    {
                                                        if (objContentSharingPortal.LinkedModuleID == objModule.ModuleID && objContentSharingPortal.LinkedPortalID == objPortal.PortalID)
                                                        {
                                                            add = false;
                                                            break;
                                                        }
                                                    }

                                                    if (add)
                                                    {
                                                        ListItem objListItem = new ListItem();
                                                        objListItem.Value = objPortal.PortalID.ToString() + "-" + objTab.TabID.ToString() + "-" + objModule.ModuleID.ToString();
                                                        List<PortalAliasInfo> aliases = PortalAliasController.Instance.GetPortalAliasesByPortalId(objPortal.PortalID).ToList();
                                                        if (aliases.Count > 0)
                                                        {
                                                            objListItem.Text = DotNetNuke.Common.Globals.AddHTTP(((PortalAliasInfo)aliases[0]).HTTPAlias) + " -> " + strPath + " -> " + objModule.ModuleTitle;
                                                        }
                                                        else
                                                        {
                                                            objListItem.Text = objPortal.PortalName + " -> " + strPath + " -> " + objModule.ModuleTitle;
                                                        }
                                                        drpContentSharingPortals.Items.Add(objListItem);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }

                }

            }

            if (drpContentSharingPortals.Items.Count == 0)
            {
                lblContentSharingNoneAvailable.Visible = true;
                drpContentSharingPortals.Visible = false;
                cmdContentSharingAdd.Visible = false;
            }
            else
            {
                lblContentSharingNoneAvailable.Visible = false;
                drpContentSharingPortals.Visible = true;
                cmdContentSharingAdd.Visible = true;
            }

        }

        private void BindSelectedContentSharingPortals()
        {

            if (!Page.IsPostBack)
            {
                Localization.LocalizeDataGrid(ref grdContentSharing, this.LocalResourceFile);
            }

            List<ContentSharingInfo> objContentSharingPortals = GetContentSharingPortals(ArticleSettings.ContentSharingPortals);

            if (objContentSharingPortals.Count > 0)
            {
                grdContentSharing.DataSource = objContentSharingPortals;
                grdContentSharing.DataBind();
                lblNoContentSharing.Visible = false;
                grdContentSharing.Visible = true;
            }
            else
            {
                lblNoContentSharing.Visible = true;
                grdContentSharing.Visible = false;
            }

        }

        private List<ContentSharingInfo> GetContentSharingPortals(string linkedPortals)
        {

            PortalController objPortalController = new PortalController();
            List<ContentSharingInfo> objContentSharingPortals = new List<ContentSharingInfo>();

            foreach (string element in linkedPortals.Split(','))
            {

                if (element.Split('-').Length == 3)
                {

                    ContentSharingInfo objContentSharing = new ContentSharingInfo();

                    objContentSharing.LinkedPortalID = Convert.ToInt32(element.Split('-')[0]);
                    objContentSharing.LinkedTabID = Convert.ToInt32(element.Split('-')[1]);
                    objContentSharing.LinkedModuleID = Convert.ToInt32(element.Split('-')[2]);

                    ModuleController objModuleController = new ModuleController();
                    ModuleInfo objModule = objModuleController.GetModule(objContentSharing.LinkedModuleID, objContentSharing.LinkedTabID);

                    if (objModule != null)
                    {
                        objContentSharing.ModuleTitle = objModule.ModuleTitle;
                        objContentSharingPortals.Add(objContentSharing);
                    }

                }

            }

            return objContentSharingPortals;

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            cmdUpdate.Click+=cmdUpdate_Click;
            cmdCancel.Click+=cmdCancel_Click;
            valEditorWidthIsValid.ServerValidate+=valEditorWidthIsValid_ServerValidate;
            valEditorHeightIsvalid.ServerValidate+=valEditorHeightIsvalid_ServerValidate;
            cmdSelectAuthor.Click+=cmdSelectAuthor_Click;
            cmdSelectAuthorDefault.Click+=cmdSelectAuthorDefault_Click;
            grdBasicPermissions.ItemDataBound+=grdBasicPermissions_ItemDataBound;
            grdFormPermissions.ItemDataBound+=grdFormPermissions_ItemDataBound;
            grdAdminPermissions.ItemDataBound+=grdAdminPermissions_ItemDataBound;
            cmdContentSharingAdd.Click+=cmdContentSharingAdd_Click;
            grdContentSharing.ItemCommand+=grdContentSharing_ItemCommand;
            drpSecurityRoleGroups.SelectedIndexChanged+=drpSecurityRoleGroups_SelectedIndexChanged;
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Load(object sender, EventArgs e)
        {

            try
            {

                if (!Page.IsPostBack)
                {

                    BindCategorySortOrder();
                    BindDisplayTypes();
                    BindAuthorSelection();
                    BindTextEditorMode();
                    BindPageSize();
                    BindTemplates();
                    BindTitleReplacement();
                    BindTimeZone();
                    BindRoleGroups();
                    BindFolders();
                    BindCategories();
                    BindSortBy();
                    BindSortDirection();
                    BindSyndicationLinkMode();
                    BindSyndicationEnclosureType();
                    BindUrlMode();
                    BindMenuPositionType();
                    BindRelatedTypes();
                    BindThumbnailType();
                    BindWatermarkPosition();
                    BindSettings();

                    lstDefaultCategories.Height = Unit.Parse(ArticleSettings.CategorySelectionHeight.ToString());
                    lstCategories.Height = Unit.Parse(ArticleSettings.CategorySelectionHeight.ToString());

                    trAdminSettings1.Visible = this.UserInfo.IsSuperUser;
                    trAdminSettings2.Visible = this.UserInfo.IsSuperUser;

                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {

            try
            {

                if (Page.IsValid)
                {

                    SaveBasicSettings();
                    SaveArchiveSettings();
                    SaveCategorySettings();
                    SaveCommentSettings();
                    SaveFilterSettings();
                    SaveFormSettings();
                    SaveImageSettings();
                    SaveFileSettings();
                    SaveNotificationSettings();
                    SaveRelatedSettings();
                    SaveSecuritySettings();
                    SaveSEOSettings();
                    SaveSyndicationSettings();
                    SaveTwitterSettings();
                    SaveThirdPartySettings();

                    CategoryController.ClearCache(ModuleId);
                    LayoutController.ClearCache(this);

                    Response.Redirect(EditArticleUrl("AdminOptions"), true);

                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {

            try
            {

                Response.Redirect(EditArticleUrl("AdminOptions"), true);

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void valEditorWidthIsValid_ServerValidate(object source, ServerValidateEventArgs args)
        {

            try
            {

                try
                {
                    Unit.Parse(txtTextEditorWidth.Text);
                    args.IsValid = true;
                }
                catch (Exception ex)
                {
                    args.IsValid = false;
                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void valEditorHeightIsvalid_ServerValidate(object source, ServerValidateEventArgs args)
        {

            try
            {

                try
                {
                    Unit.Parse(txtTextEditorHeight.Text);
                    args.IsValid = true;
                }
                catch (Exception ex)
                {
                    args.IsValid = false;
                }

            }
            catch (Exception exc)     //Module failed to load
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
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdSelectAuthorDefault_Click(object sender, EventArgs e)
        {

            try
            {

                PopulateAuthorListDefault();
                drpAuthorDefault.Visible = true;
                cmdSelectAuthorDefault.Visible = false;
                lblAuthorDefault.Visible = false;

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void grdBasicPermissions_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ListItem objListItem = (ListItem)e.Item.DataItem;

                if (objListItem != null)
                {

                    string role = ((ListItem)e.Item.DataItem).Value;

                    CheckBox chkSubmit = (CheckBox)e.Item.FindControl("chkSubmit");
                    CheckBox chkSecure = (CheckBox)e.Item.FindControl("chkSecure");
                    CheckBox chkAutoSecure = (CheckBox)e.Item.FindControl("chkAutoSecure");
                    CheckBox chkApprove = (CheckBox)e.Item.FindControl("chkApprove");
                    CheckBox chkAutoApproveArticle = (CheckBox)e.Item.FindControl("chkAutoApproveArticle");
                    CheckBox chkAutoApproveComment = (CheckBox)e.Item.FindControl("chkAutoApproveComment");
                    CheckBox chkFeature = (CheckBox)e.Item.FindControl("chkFeature");
                    CheckBox chkAutoFeature = (CheckBox)e.Item.FindControl("chkAutoFeature");

                    if (objListItem.Value == PortalSettings.AdministratorRoleName.ToString())
                    {
                        chkSubmit.Enabled = false;
                        chkSubmit.Checked = true;
                        chkSecure.Enabled = true;
                        if (Settings.Contains(ArticleConstants.PERMISSION_SECURE_SETTING))
                        {
                            chkSecure.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_SECURE_SETTING].ToString().Split(';'));
                        }
                        chkAutoSecure.Enabled = true;
                        if (Settings.Contains(ArticleConstants.PERMISSION_AUTO_SECURE_SETTING))
                        {
                            chkAutoSecure.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_AUTO_SECURE_SETTING].ToString().Split(';'));
                        }
                        chkApprove.Enabled = false;
                        chkApprove.Checked = true;
                        chkAutoApproveArticle.Enabled = true;
                        if (Settings.Contains(ArticleConstants.PERMISSION_AUTO_APPROVAL_SETTING))
                        {
                            chkAutoApproveArticle.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_AUTO_APPROVAL_SETTING].ToString().Split(';'));
                        }
                        chkAutoApproveComment.Enabled = true;
                        if (Settings.Contains(ArticleConstants.PERMISSION_AUTO_APPROVAL_COMMENT_SETTING))
                        {
                            chkAutoApproveComment.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_AUTO_APPROVAL_COMMENT_SETTING].ToString().Split(';'));
                        }
                        chkFeature.Enabled = false;
                        chkFeature.Checked = true;
                        chkAutoFeature.Enabled = true;
                        if (Settings.Contains(ArticleConstants.PERMISSION_AUTO_FEATURE_SETTING))
                        {
                            chkAutoFeature.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_AUTO_FEATURE_SETTING].ToString().Split(';'));
                        }
                    }
                    else
                    {
                        if (Settings.Contains(ArticleConstants.PERMISSION_SUBMISSION_SETTING))
                        {
                            chkSubmit.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_SUBMISSION_SETTING].ToString().Split(';'));
                        }
                        if (Settings.Contains(ArticleConstants.PERMISSION_SECURE_SETTING))
                        {
                            chkSecure.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_SECURE_SETTING].ToString().Split(';'));
                        }
                        if (Settings.Contains(ArticleConstants.PERMISSION_AUTO_SECURE_SETTING))
                        {
                            chkAutoSecure.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_AUTO_SECURE_SETTING].ToString().Split(';'));
                        }
                        if (Settings.Contains(ArticleConstants.PERMISSION_APPROVAL_SETTING))
                        {
                            chkApprove.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_APPROVAL_SETTING].ToString().Split(';'));
                        }
                        if (Settings.Contains(ArticleConstants.PERMISSION_AUTO_APPROVAL_SETTING))
                        {
                            chkAutoApproveArticle.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_AUTO_APPROVAL_SETTING].ToString().Split(';'));
                        }
                        if (Settings.Contains(ArticleConstants.PERMISSION_AUTO_APPROVAL_COMMENT_SETTING))
                        {
                            chkAutoApproveComment.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_AUTO_APPROVAL_COMMENT_SETTING].ToString().Split(';'));
                        }
                        if (Settings.Contains(ArticleConstants.PERMISSION_FEATURE_SETTING))
                        {
                            chkFeature.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_FEATURE_SETTING].ToString().Split(';'));
                        }
                        if (Settings.Contains(ArticleConstants.PERMISSION_AUTO_FEATURE_SETTING))
                        {
                            chkAutoFeature.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_AUTO_FEATURE_SETTING].ToString().Split(';'));
                        }
                    }

                }

            }

        }

        private void grdFormPermissions_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ListItem objListItem = (ListItem)e.Item.DataItem;

                if (objListItem != null)
                {

                    string role = ((ListItem)e.Item.DataItem).Value;

                    CheckBox chkCategories = (CheckBox)e.Item.FindControl("chkCategories");
                    CheckBox chkExcerpt = (CheckBox)e.Item.FindControl("chkExcerpt");
                    CheckBox chkImage = (CheckBox)e.Item.FindControl("chkImage");
                    CheckBox chkFile = (CheckBox)e.Item.FindControl("chkFile");
                    CheckBox chkLink = (CheckBox)e.Item.FindControl("chkLink");
                    CheckBox chkPublishDate = (CheckBox)e.Item.FindControl("chkPublishDate");
                    CheckBox chkExpiryDate = (CheckBox)e.Item.FindControl("chkExpiryDate");
                    CheckBox chkMeta = (CheckBox)e.Item.FindControl("chkMeta");
                    CheckBox chkCustom = (CheckBox)e.Item.FindControl("chkCustom");

                    if (Settings.Contains(ArticleConstants.PERMISSION_CATEGORIES_SETTING))
                    {
                        chkCategories.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_CATEGORIES_SETTING].ToString().Split(';'));
                    }
                    else
                    {
                        chkCategories.Checked = true;
                    }
                    if (Settings.Contains(ArticleConstants.PERMISSION_EXCERPT_SETTING))
                    {
                        chkExcerpt.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_EXCERPT_SETTING].ToString().Split(';'));
                    }
                    else
                    {
                        chkExcerpt.Checked = true;
                    }
                    if (Settings.Contains(ArticleConstants.PERMISSION_IMAGE_SETTING))
                    {
                        chkImage.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_IMAGE_SETTING].ToString().Split(';'));
                    }
                    else
                    {
                        chkImage.Checked = true;
                    }
                    if (Settings.Contains(ArticleConstants.PERMISSION_FILE_SETTING))
                    {
                        chkFile.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_FILE_SETTING].ToString().Split(';'));
                    }
                    else
                    {
                        chkFile.Checked = true;
                    }
                    if (Settings.Contains(ArticleConstants.PERMISSION_LINK_SETTING))
                    {
                        chkLink.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_LINK_SETTING].ToString().Split(';'));
                    }
                    else
                    {
                        chkLink.Checked = true;
                    }
                    if (Settings.Contains(ArticleConstants.PERMISSION_PUBLISH_SETTING))
                    {
                        chkPublishDate.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_PUBLISH_SETTING].ToString().Split(';'));
                    }
                    else
                    {
                        chkPublishDate.Checked = true;
                    }
                    if (Settings.Contains(ArticleConstants.PERMISSION_EXPIRY_SETTING))
                    {
                        chkExpiryDate.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_EXPIRY_SETTING].ToString().Split(';'));
                    }
                    else
                    {
                        chkExpiryDate.Checked = true;
                    }
                    if (Settings.Contains(ArticleConstants.PERMISSION_META_SETTING))
                    {
                        chkMeta.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_META_SETTING].ToString().Split(';'));
                    }
                    else
                    {
                        chkMeta.Checked = true;
                    }
                    if (Settings.Contains(ArticleConstants.PERMISSION_CUSTOM_SETTING))
                    {
                        chkCustom.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_CUSTOM_SETTING].ToString().Split(';'));
                    }
                    else
                    {
                        chkCustom.Checked = true;
                    }

                }

            }

        }

        private void grdAdminPermissions_ItemDataBound(object sender, DataGridItemEventArgs e){

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {
                ListItem objListItem = (ListItem)e.Item.DataItem;

                if  (objListItem != null) {

                    string role = ((ListItem)e.Item.DataItem).Value;

                    CheckBox chkSiteTemplates = (CheckBox)e.Item.FindControl("chkSiteTemplates");

                    if (Settings.Contains(ArticleConstants.PERMISSION_SITE_TEMPLATES_SETTING)) {
                        chkSiteTemplates.Checked = IsInRole(role, Settings[ArticleConstants.PERMISSION_SITE_TEMPLATES_SETTING].ToString().Split(';'));
                    }else{
                        chkSiteTemplates.Checked = false;
                    }

                }

            }

        }

        private void cmdContentSharingAdd_Click(object sender, EventArgs e)
        {

            string currentPortals = Null.NullString;
            if (ArticleSettings.ContentSharingPortals == Null.NullString)
            {
                currentPortals = drpContentSharingPortals.SelectedValue;
            }
            else
            {
                currentPortals = ArticleSettings.ContentSharingPortals + "," + drpContentSharingPortals.SelectedValue;
            }

            ModuleController objModules = new ModuleController();
            objModules.UpdateModuleSetting(ModuleId, ArticleConstants.CONTENT_SHARING_SETTING, currentPortals);

            Settings[ArticleConstants.CONTENT_SHARING_SETTING] = currentPortals;

            BindSelectedContentSharingPortals();
            BindAvailableContentSharingPortals();

        }

        private void grdContentSharing_ItemCommand(object sender, DataGridCommandEventArgs e)
        {

            if (e.CommandName == "Delete")
            {

                int linkedModuleID = Convert.ToInt32(e.CommandArgument);

                string updateSetting = Null.NullString;
                List<ContentSharingInfo> objContentSharingPortals = GetContentSharingPortals(ArticleSettings.ContentSharingPortals);

                foreach (ContentSharingInfo objContentSharingPortal in objContentSharingPortals)
                {
                    if (objContentSharingPortal.LinkedModuleID != linkedModuleID)
                    {

                        if (updateSetting == "")
                        {
                            updateSetting = objContentSharingPortal.LinkedPortalID.ToString() + "-" + objContentSharingPortal.LinkedTabID.ToString() + "-" + objContentSharingPortal.LinkedModuleID.ToString();
                        }
                        else
                        {
                            updateSetting = updateSetting + "," + objContentSharingPortal.LinkedPortalID.ToString() + "-" + objContentSharingPortal.LinkedTabID.ToString() + "-" + objContentSharingPortal.LinkedModuleID.ToString();
                        }

                    }
                }

                ModuleController objModules = new ModuleController();
                objModules.UpdateModuleSetting(ModuleId, ArticleConstants.CONTENT_SHARING_SETTING, updateSetting);

                Settings[ArticleConstants.CONTENT_SHARING_SETTING] = updateSetting;

                BindSelectedContentSharingPortals();
                BindAvailableContentSharingPortals();

            }

        }

        private void drpSecurityRoleGroups_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {

                BindRoles();

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}