using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace GcDesign.NewsArticles
{
    public partial class NewsArchivesOptions : ModuleSettingsBase
    {
        #region " private Members "

        private ArchiveSettings _archiveSettings;

        #endregion

        #region " private Property "

        private ArchiveModeType ArchiveMode
        {
            get
            {
                return (ArchiveModeType)System.Enum.Parse(typeof(ArchiveModeType), drpMode.SelectedValue);
            }
        }

        private ArchiveSettings ArchiveSettings
        {
            get
            {
                if (_archiveSettings == null)
                {
                    _archiveSettings = new ArchiveSettings(Settings);
                }
                return _archiveSettings;
            }
        }

        private LayoutModeType LayoutMode
        {
            get
            {
                return (LayoutModeType)System.Enum.Parse(typeof(LayoutModeType), rdoLayoutMode.SelectedValue);
            }
        }

        #endregion

        #region " private Methods "

        private void BindLookups()
        {

            Common.BindEnum(drpAuthorSortBy, typeof(AuthorSortByType), LocalResourceFile);
            Common.BindEnum(rdoLayoutMode, typeof(LayoutModeType), LocalResourceFile);
            Common.BindEnum(drpGroupBy, typeof(GroupByType), LocalResourceFile);
            Common.BindEnum(drpMode, typeof(ArchiveModeType), LocalResourceFile);

        }

        private void BindModules()
        {

            List<ModuleInfo> objModules = Common.GetArticleModules(PortalId);

            foreach (ModuleInfo objModule in objModules)
            {
                ListItem objListItem = new ListItem();

                objListItem.Value = objModule.TabID.ToString() + "-" + objModule.ModuleID.ToString();
                objListItem.Text = objModule.ParentTab.TabName + " -> " + objModule.ModuleTitle;

                drpModuleID.Items.Add(objListItem);
            }

        }

        private void BindParentCategories()
        {

            drpParentCategory.Items.Clear();

            if (drpModuleID.Items.Count > 0)
            {

                string[] values = drpModuleID.SelectedValue.Split('-');

                if (values.Length == 2)
                {
                    CategoryController objCategoryController = new CategoryController();
                    drpParentCategory.DataSource = objCategoryController.GetCategoriesAll(Convert.ToInt32(values[1]), Null.NullInteger, CategorySortType.Name);
                    drpParentCategory.DataBind();
                }

            }

            drpParentCategory.Items.Insert(0, new ListItem(Localization.GetString("NoParentCategory", LocalResourceFile), "-1"));

        }

        private void BindTemplates()
        {

            switch (ArchiveMode)
            {
                case ArchiveModeType.Date:

                    if (LayoutMode == LayoutModeType.Simple)
                    {

                        txtHtmlHeader.Text = ArchiveSettings.TemplateDateHeader.ToString();
                        txtHtmlBody.Text = ArchiveSettings.TemplateDateBody.ToString();
                        txtHtmlFooter.Text = ArchiveSettings.TemplateDateFooter.ToString();

                    }
                    else
                    {

                        txtHtmlHeader.Text = ArchiveSettings.TemplateDateAdvancedHeader.ToString();
                        txtHtmlBody.Text = ArchiveSettings.TemplateDateAdvancedBody.ToString();
                        txtHtmlFooter.Text = ArchiveSettings.TemplateDateAdvancedFooter.ToString();

                    }
                    break;
                case ArchiveModeType.Category:

                    if (LayoutMode == LayoutModeType.Simple)
                    {

                        txtHtmlHeader.Text = ArchiveSettings.TemplateCategoryHeader.ToString();
                        txtHtmlBody.Text = ArchiveSettings.TemplateCategoryBody.ToString();
                        txtHtmlFooter.Text = ArchiveSettings.TemplateCategoryFooter.ToString();

                    }
                    else
                    {

                        txtHtmlHeader.Text = ArchiveSettings.TemplateCategoryAdvancedHeader.ToString();
                        txtHtmlBody.Text = ArchiveSettings.TemplateCategoryAdvancedBody.ToString();
                        txtHtmlFooter.Text = ArchiveSettings.TemplateCategoryAdvancedFooter.ToString();

                    }
                    break;
                case ArchiveModeType.Author:

                    if (LayoutMode == LayoutModeType.Simple)
                    {

                        txtHtmlHeader.Text = ArchiveSettings.TemplateAuthorHeader.ToString();
                        txtHtmlBody.Text = ArchiveSettings.TemplateAuthorBody.ToString();
                        txtHtmlFooter.Text = ArchiveSettings.TemplateAuthorFooter.ToString();

                    }
                    else
                    {

                        txtHtmlHeader.Text = ArchiveSettings.TemplateAuthorAdvancedHeader.ToString();
                        txtHtmlBody.Text = ArchiveSettings.TemplateAuthorAdvancedBody.ToString();
                        txtHtmlFooter.Text = ArchiveSettings.TemplateAuthorAdvancedFooter.ToString();

                    }
                    break;
            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.PreRender += new EventHandler(Page_PreRender);
            drpMode.SelectedIndexChanged += drpMode_SelectedIndexChanged;
            rdoLayoutMode.SelectedIndexChanged += rdoLayoutMode_SelectedIndexChanged;
            drpModuleID.SelectedIndexChanged += drpModuleID_SelectedIndexChanged;
            
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            try
            {

                // Date Settings
                divGroupBy.Visible = (drpMode.SelectedValue == ArchiveModeType.Date.ToString());

                // Category Settings
                divHideZeroCategories.Visible = (ArchiveMode == ArchiveModeType.Category);
                divParentCategory.Visible = (ArchiveMode == ArchiveModeType.Category);
                divMaxDepth.Visible = (ArchiveMode == ArchiveModeType.Category);

                // Author Settings
                divAuthorSortBy.Visible = (drpMode.SelectedValue == ArchiveModeType.Author.ToString());

                // Template Settings
                divItemsPerRow.Visible = (LayoutMode == LayoutModeType.Advanced);
            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void drpMode_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {

                BindTemplates();

            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void rdoLayoutMode_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {

                BindTemplates();

            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void drpModuleID_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {

                BindParentCategories();

            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
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
                    BindLookups();

                    if (drpMode.Items.FindByValue(ArchiveSettings.Mode.ToString()) != null)
                    {
                        drpMode.SelectedValue = ArchiveSettings.Mode.ToString();
                    }

                    if (drpModuleID.Items.FindByValue(ArchiveSettings.TabId.ToString() + "-" + ArchiveSettings.ModuleId.ToString()) != null)
                    {
                        drpModuleID.SelectedValue = ArchiveSettings.TabId.ToString() + "-" + ArchiveSettings.ModuleId.ToString();
                    }

                    BindParentCategories();

                    // Date Settings 
                    if (drpGroupBy.Items.FindByValue(ArchiveSettings.GroupBy.ToString()) != null)
                    {
                        drpGroupBy.SelectedValue = ArchiveSettings.GroupBy.ToString();
                    }

                    // Category Settings
                    if (ArchiveSettings.CategoryMaxDepth != Null.NullInteger)
                    {
                        txtMaxDepth.Text = ArchiveSettings.CategoryMaxDepth.ToString();
                    }
                    if (drpParentCategory.Items.FindByValue(ArchiveSettings.CategoryParent.ToString()) != null)
                    {
                        drpParentCategory.SelectedValue = ArchiveSettings.CategoryParent.ToString();
                    }
                    chkHideZeroCategories.Checked = ArchiveSettings.CategoryHideZeroCategories;

                    // Author Settings
                    if (drpAuthorSortBy.Items.FindByValue(ArchiveSettings.AuthorSortBy.ToString()) != null)
                    {
                        drpAuthorSortBy.SelectedValue = ArchiveSettings.AuthorSortBy.ToString();
                    }

                    // Template settings
                    rdoLayoutMode.SelectedValue = ArchiveSettings.LayoutMode.ToString();
                    BindTemplates();
                    txtItemsPerRow.Text = ArchiveSettings.ItemsPerRow.ToString();

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

                objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_MODE, drpMode.SelectedValue);

                if (drpModuleID.Items.Count > 0)
                {

                    string[] values = drpModuleID.SelectedValue.Split('-');

                    if (values.Length == 2)
                    {
                        objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_TAB_ID, values[0]);
                        objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_MODULE_ID, values[1]);
                    }

                }

                objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_GROUP_BY, drpGroupBy.SelectedValue);
                objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_LAYOUT_MODE, rdoLayoutMode.SelectedValue);
                objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_ITEMS_PER_ROW, txtItemsPerRow.Text);

                ArchiveModeType currentType = (ArchiveModeType)System.Enum.Parse(typeof(ArchiveModeType), drpMode.SelectedValue);

                switch (currentType)
                {

                    case ArchiveModeType.Date:

                        if (LayoutMode == LayoutModeType.Simple)
                        {
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_HEADER, txtHtmlHeader.Text);
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_BODY, txtHtmlBody.Text);
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_FOOTER, txtHtmlFooter.Text);
                        }
                        else
                        {
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_HEADER_ADVANCED, txtHtmlHeader.Text);
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_BODY_ADVANCED, txtHtmlBody.Text);
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_FOOTER_ADVANCED, txtHtmlFooter.Text);
                        }
                        break;
                    case ArchiveModeType.Category:

                        if (LayoutMode == LayoutModeType.Simple)
                        {
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_HEADER, txtHtmlHeader.Text);
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_BODY, txtHtmlBody.Text);
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_FOOTER, txtHtmlFooter.Text);
                        }
                        else
                        {
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_HEADER_ADVANCED, txtHtmlHeader.Text);
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_BODY_ADVANCED, txtHtmlBody.Text);
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_FOOTER_ADVANCED, txtHtmlFooter.Text);
                        }
                        objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_HIDE_ZERO_CATEGORIES, chkHideZeroCategories.Checked.ToString());
                        objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_PARENT_CATEGORY, drpParentCategory.SelectedValue);
                        if (txtMaxDepth.Text != "")
                        {
                            if (Convert.ToInt32(txtMaxDepth.Text) > 0)
                            {
                                objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_MAX_DEPTH, txtMaxDepth.Text);
                            }
                        }
                        else
                        {
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_MAX_DEPTH, "-1");
                        }
                        break;
                    case ArchiveModeType.Author:

                        if (LayoutMode == LayoutModeType.Simple)
                        {
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_HEADER, txtHtmlHeader.Text);
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_BODY, txtHtmlBody.Text);
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_FOOTER, txtHtmlFooter.Text);
                        }
                        else
                        {
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_HEADER_ADVANCED, txtHtmlHeader.Text);
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_BODY_ADVANCED, txtHtmlBody.Text);
                            objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_FOOTER_ADVANCED, txtHtmlFooter.Text);
                        }
                        objModules.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_AUTHOR_SORT_BY, drpAuthorSortBy.SelectedValue);
                        break;
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