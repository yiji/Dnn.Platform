using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Security;
using DotNetNuke.Entities.Users;

using GcDesign.NewsArticles.Base;
using System.Collections;

namespace GcDesign.NewsArticles
{
    public partial class NewsArchives : NewsArticleModuleBase
    {

        #region " private Members "

        private ArchiveSettings _archiveSettings;
        private ArticleSettings _articleSettings;
        private List<CategoryInfo> _categories;

        #endregion

        #region " private Property "

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

        public new ArticleSettings ArticleSettings
        {
            get
            {
                if (_articleSettings == null)
                {

                    ModuleController objModuleController = new ModuleController();
                    Hashtable _settings = objModuleController.GetModuleSettings(ArchiveSettings.ModuleId);    //ArchiveSettings.ModuleId 分类模块对应的文章模块

                    ModuleInfo objModule = objModuleController.GetModule(ArchiveSettings.ModuleId, ArchiveSettings.TabId);
                    if (objModule != null)
                    {
                        Hashtable tabModuleSettings = objModuleController.GetTabModuleSettings(objModule.TabModuleID);
                        foreach (string strKey in tabModuleSettings.Keys)
                        {
                            _settings[strKey] = tabModuleSettings[strKey];
                        }
                    }

                    _articleSettings = new ArticleSettings(_settings, PortalSettings, objModule);

                }
                return _articleSettings;
            }
        }

        private List<CategoryInfo> Categories
        {
            get
            {
                if (_categories == null)
                {
                    CategoryController objCategoryController = new CategoryController();
                    _categories = objCategoryController.GetCategoriesAll(ArchiveSettings.ModuleId, Null.NullInteger, _articleSettings.CategorySortType);
                }

                return _categories;
            }
        }

        #endregion

        #region " private Methods "

        private void BindArchive()
        {

            switch (ArchiveSettings.Mode)
            {

                case ArchiveModeType.Date:
                    BindDateArchive();
                    break;
                case ArchiveModeType.Category:
                    BindCategoryArchive();
                    break;
                case ArchiveModeType.Author:
                    BindAuthorArchive();
                    break;
            }

        }

        private bool FindSettings()
        {

            if (ArchiveSettings.ModuleId != Null.NullInteger)
            {
                return true;
            }

            if (!Settings.Contains("na_StartupCheck"))
            {

                ModuleController objModuleController = new ModuleController();
                List<ModuleInfo> objModules = Common.GetArticleModules(PortalId);

                foreach (ModuleInfo objModule in objModules)
                {
                    if (objModule.TabID == TabId)
                    {

                        objModuleController.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_TAB_ID, objModule.TabID.ToString());
                        objModuleController.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_MODULE_ID, objModule.ModuleID.ToString());
                        objModuleController.UpdateModuleSetting(ModuleId, "na_StartupCheck", "1");

                        _archiveSettings = null;

                        return true;
                    }
                }

                foreach (ModuleInfo objModule in objModules)
                {

                    objModuleController.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_TAB_ID, objModule.TabID.ToString());
                    objModuleController.UpdateTabModuleSetting(TabModuleId, ArticleConstants.NEWS_ARCHIVES_MODULE_ID, objModule.ModuleID.ToString());
                    objModuleController.UpdateModuleSetting(ModuleId, "na_StartupCheck", "1");

                    _archiveSettings = null;

                    return true;
                }

                objModuleController.UpdateModuleSetting(ModuleId, "na_StartupCheck", "1");

            }

            return false;

        }

        private void BindDateArchive(){

            ArticleController objArticleController = new ArticleController();
            ModuleController objModuleSettingController = new ModuleController();

            ModuleInfo mi = objModuleSettingController.GetModule(ArchiveSettings.ModuleId, ArchiveSettings.TabId);
            if (mi != null) {

                int authorId = Null.NullInteger;
                if (ArticleSettings.AuthorLoggedInUserFilter) {
                    authorId = -100;
                    if (Request.IsAuthenticated) {
                        authorId = UserId;
                    }
                }

                if (ArticleSettings.AuthorUserIDFilter) {
                    if (ArticleSettings.AuthorUserIDParam != "") {
                        if (Request[ArticleSettings.AuthorUserIDParam] != null)
                        {
                            if (Numeric.IsNumeric(Request[ArticleSettings.AuthorUserIDParam])) {
                                authorId = Convert.ToInt32(Request[ArticleSettings.AuthorUserIDParam]);
                            }
                        }
                    }
                }

                if (ArticleSettings.AuthorUsernameFilter) {
                    if (ArticleSettings.AuthorUsernameParam != "") {
                        if (Request[ArticleSettings.AuthorUsernameParam] != null)
                        {
                            UserInfo objUser = UserController.GetUserByName(PortalId, Request[ArticleSettings.AuthorUsernameParam]);
                            if (objUser != null) {
                                authorId = objUser.UserID;
                            }
                        }
                    }
                }

                if (ArticleSettings.FilterSingleCategory != Null.NullInteger) {
                    int[] categoriesToDisplay = new int[1];
                    categoriesToDisplay[0] = ArticleSettings.FilterSingleCategory;

                    if (ArchiveSettings.LayoutMode == LayoutModeType.Simple) {
                        rptNewsArchives.DataSource = objArticleController.GetNewsArchive(ArchiveSettings.ModuleId, categoriesToDisplay, null, authorId, ArchiveSettings.GroupBy, ArticleSettings.ShowPending);
                        rptNewsArchives.DataBind();
                    }
                    else
                    {
                        dlNewsArchives.RepeatColumns = ArchiveSettings.ItemsPerRow;
                        dlNewsArchives.DataSource = objArticleController.GetNewsArchive(ArchiveSettings.ModuleId, categoriesToDisplay, null, authorId, ArchiveSettings.GroupBy, ArticleSettings.ShowPending);
                        dlNewsArchives.DataBind();
                    }
                    return;
                }

                if (ArticleSettings.FilterCategories != null) {
                    if (ArchiveSettings.LayoutMode == LayoutModeType.Simple) {
                        rptNewsArchives.DataSource = objArticleController.GetNewsArchive(ArchiveSettings.ModuleId, ArticleSettings.FilterCategories, null, authorId, ArchiveSettings.GroupBy, ArticleSettings.ShowPending);
                        rptNewsArchives.DataBind();
                    }
                    else
                    {
                        dlNewsArchives.RepeatColumns = ArchiveSettings.ItemsPerRow;
                        dlNewsArchives.DataSource = objArticleController.GetNewsArchive(ArchiveSettings.ModuleId, ArticleSettings.FilterCategories, null, authorId, ArchiveSettings.GroupBy, ArticleSettings.ShowPending);
                        dlNewsArchives.DataBind();
                    }
                    return;
                }

                Hashtable newsSettings = objModuleSettingController.GetTabModuleSettings(mi.TabModuleID);
                List<int> excludeCategoriesRestrictive = new List<int>();

                foreach(CategoryInfo objCategory  in Categories){
                    if (!objCategory.InheritSecurity && objCategory.CategorySecurityType == CategorySecurityType.Restrict) {
                        if (Request.IsAuthenticated) {
                            if (newsSettings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING)) {
                                if (!PortalSecurity.IsInRoles(newsSettings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString())) {
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

                foreach(CategoryInfo objCategory in Categories){
                    if (!objCategory.InheritSecurity && objCategory.CategorySecurityType == CategorySecurityType.Loose) {
                        if (Request.IsAuthenticated) {
                            if (newsSettings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING)) {
                                if (!PortalSecurity.IsInRoles(newsSettings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString())) {
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

                List<int> includeCategories = new List<int>();

                if (excludeCategories.Count > 0) {

                    foreach(CategoryInfo objCategoryToInclude in Categories){

                        bool includeCategory = true;

                        foreach(int exclCategory in excludeCategories){
                            if (exclCategory == objCategoryToInclude.CategoryID) {
                                includeCategory = false;
                            }
                        }

                        if (includeCategory) {
                            includeCategories.Add(objCategoryToInclude.CategoryID);
                        }

                    }

                    if (includeCategories.Count > 0) {
                        includeCategories.Add(-1);
                    }

                }

                if (ArchiveSettings.LayoutMode == LayoutModeType.Simple) {
                    rptNewsArchives.DataSource = objArticleController.GetNewsArchive(ArchiveSettings.ModuleId, includeCategories.ToArray(), excludeCategoriesRestrictive.ToArray(), authorId, ArchiveSettings.GroupBy, ArticleSettings.ShowPending);
                    rptNewsArchives.DataBind();
                }
                else
                {
                    dlNewsArchives.RepeatColumns = ArchiveSettings.ItemsPerRow;
                    dlNewsArchives.DataSource = objArticleController.GetNewsArchive(ArchiveSettings.ModuleId, includeCategories.ToArray(), excludeCategoriesRestrictive.ToArray(), authorId, ArchiveSettings.GroupBy, ArticleSettings.ShowPending);
                    dlNewsArchives.DataBind();
                }
            }

        }

        private void BindCategoryArchive(){

            CategoryController objCategoryController = new CategoryController();
            ModuleController objModuleSettingController = new ModuleController();

            int parentID = ArticleConstants.NEWS_ARCHIVES_PARENT_CATEGORY_DEFAULT;

            if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_PARENT_CATEGORY)) {
                parentID = Convert.ToInt32(Settings[ArticleConstants.NEWS_ARCHIVES_PARENT_CATEGORY]);
            }

            ModuleInfo mi = objModuleSettingController.GetModule(ArchiveSettings.ModuleId, ArchiveSettings.TabId);//不是栏目id是文章模块ID
            if  (mi != null) {
                
                int authorId = Null.NullInteger;
                if (ArticleSettings.AuthorLoggedInUserFilter) {
                    authorId = -100;
                    if (Request.IsAuthenticated) {
                        authorId = UserId;
                    }
                }
                
                if (ArticleSettings.AuthorUserIDFilter) {
                    if (ArticleSettings.AuthorUserIDParam != "") {
                        if (Request[ArticleSettings.AuthorUserIDParam] != null)
                        {
                            if (Numeric.IsNumeric(Request[ArticleSettings.AuthorUserIDParam])) {
                                authorId = Convert.ToInt32(Request[ArticleSettings.AuthorUserIDParam]);
                            }
                        }
                    }
                }

                if (ArticleSettings.AuthorUsernameFilter) {
                    if (ArticleSettings.AuthorUsernameParam != "") {
                        if (Request[ArticleSettings.AuthorUsernameParam] != null)
                        {
                            UserInfo objUser = UserController.GetUserByName(PortalId, Request[ArticleSettings.AuthorUsernameParam]);
                            if (objUser != null) {
                                authorId = objUser.UserID;
                            }
                        }
                    }
                }

                //2013-06-24 蒋国祥 栏目绑定用了文章模块的设置  导致文章模块决定显示那些栏目文章  栏目模块也跟着显示那些模块
                if (ArticleSettings.FilterSingleCategory != Null.NullInteger) {  //Single Category 单独一个栏目
                    int[] categoriesToDisplay = new int[1];
                    categoriesToDisplay[0] = ArticleSettings.FilterSingleCategory;

                    if (ArchiveSettings.LayoutMode == LayoutModeType.Simple) {
                        rptNewsArchives.DataSource = objCategoryController.GetCategoriesAll(ArchiveSettings.ModuleId, parentID, categoriesToDisplay, authorId, ArchiveSettings.CategoryMaxDepth, ArticleSettings.ShowPending, ArticleSettings.CategorySortType);
                        rptNewsArchives.DataBind();
                    }
                    else
                    {
                        dlNewsArchives.RepeatColumns = ArchiveSettings.ItemsPerRow;
                        dlNewsArchives.DataSource = objCategoryController.GetCategoriesAll(ArchiveSettings.ModuleId, parentID, categoriesToDisplay, authorId, ArchiveSettings.CategoryMaxDepth, ArticleSettings.ShowPending, ArticleSettings.CategorySortType);
                        dlNewsArchives.DataBind();
                    }
                    return;
                }


                //2013-06-24 蒋国祥 绑定match any设置中的栏目
                if (ArticleSettings.FilterCategories != null) {
                    if (ArchiveSettings.LayoutMode == LayoutModeType.Simple) {
                        rptNewsArchives.DataSource = objCategoryController.GetCategoriesAll(ArchiveSettings.ModuleId, parentID, ArticleSettings.FilterCategories, authorId, ArchiveSettings.CategoryMaxDepth, ArticleSettings.ShowPending, ArticleSettings.CategorySortType);
                        rptNewsArchives.DataBind();
                    }else{
                        dlNewsArchives.RepeatColumns = ArchiveSettings.ItemsPerRow;
                        dlNewsArchives.DataSource = objCategoryController.GetCategoriesAll(ArchiveSettings.ModuleId, parentID, ArticleSettings.FilterCategories, authorId, ArchiveSettings.CategoryMaxDepth, ArticleSettings.ShowPending, ArticleSettings.CategorySortType);
                        dlNewsArchives.DataBind();
                    }
                    return;
                }

                Hashtable moduleSettings = objModuleSettingController.GetModuleSettings(mi.ModuleID);//获取文章模块的设置

                List<CategoryInfo> objCategoriesSelected = new List<CategoryInfo>();
                //不过滤
                List<CategoryInfo> objCategories = objCategoryController.GetCategoriesAll(ArchiveSettings.ModuleId, parentID, null, authorId, ArchiveSettings.CategoryMaxDepth, ArticleSettings.ShowPending, ArticleSettings.CategorySortType);
                //取所有栏目 根据角色安全显示
                foreach(CategoryInfo objCategory in objCategories){
                    if (objCategory.InheritSecurity) {
                        objCategoriesSelected.Add(objCategory);
                    }else{
                        if (Request.IsAuthenticated) {
                            if (moduleSettings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING)) {
                                if (PortalSecurity.IsInRoles(moduleSettings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString())) {
                                    objCategoriesSelected.Add(objCategory);
                                }
                            }
                        }
                    }
                }

                if (ArchiveSettings.LayoutMode == LayoutModeType.Simple) {
                    rptNewsArchives.DataSource = objCategoriesSelected;
                    rptNewsArchives.DataBind();
                }else{
                    dlNewsArchives.RepeatColumns = ArchiveSettings.ItemsPerRow;
                    dlNewsArchives.DataSource = objCategoriesSelected;
                    dlNewsArchives.DataBind();
                }
            }

        }

        private void BindAuthorArchive()
        {

            string sortBy = ArticleConstants.NEWS_ARCHIVES_AUTHOR_SORT_BY_DEFAULT.ToString();
            if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_AUTHOR_SORT_BY))
            {
                sortBy = ((AuthorSortByType)System.Enum.Parse(typeof(AuthorSortByType), Settings[ArticleConstants.NEWS_ARCHIVES_AUTHOR_SORT_BY].ToString())).ToString();
            }

            AuthorController objAuthorController = new AuthorController();
            ModuleController objModuleSettingController = new ModuleController();

            ModuleInfo mi = objModuleSettingController.GetModule(ArchiveSettings.ModuleId, ArchiveSettings.TabId);
            if (mi != null)
            {
                Hashtable newsSettings = objModuleSettingController.GetTabModuleSettings(mi.TabModuleID);

                int authorId = Null.NullInteger;
                if (ArticleSettings.AuthorLoggedInUserFilter)
                {
                    authorId = -100;
                    if (Request.IsAuthenticated)
                    {
                        authorId = UserId;
                    }
                }

                if (ArticleSettings.AuthorUserIDFilter)
                {
                    if (ArticleSettings.AuthorUserIDParam != "")
                    {
                        if (Request[ArticleSettings.AuthorUserIDParam] != null)
                        {
                            if (Numeric.IsNumeric(Request[ArticleSettings.AuthorUserIDParam]))
                            {
                                authorId = Convert.ToInt32(Request[ArticleSettings.AuthorUserIDParam]);
                            }
                        }
                    }
                }


                if (ArticleSettings.AuthorUsernameFilter)
                {
                    if (ArticleSettings.AuthorUsernameParam != "")
                    {
                        if (Request[ArticleSettings.AuthorUsernameParam] != null)
                        {
                            UserInfo objUser = UserController.GetUserByName(PortalId, Request[ArticleSettings.AuthorUsernameParam]);
                            if (objUser != null)
                            {
                                authorId = objUser.UserID;
                            }
                        }
                    }
                }

                if (ArticleSettings.FilterSingleCategory != Null.NullInteger)
                {
                    int[] categoriesToDisplay = new int[1];
                    categoriesToDisplay[0] = ArticleSettings.FilterSingleCategory;

                    if (ArchiveSettings.LayoutMode == LayoutModeType.Simple)
                    {
                        rptNewsArchives.DataSource = objAuthorController.GetAuthorStatistics(ArchiveSettings.ModuleId, categoriesToDisplay, null, authorId, sortBy, ArticleSettings.ShowPending);
                        rptNewsArchives.DataBind();
                    }
                    else
                    {
                        dlNewsArchives.RepeatColumns = ArchiveSettings.ItemsPerRow;
                        dlNewsArchives.DataSource = objAuthorController.GetAuthorStatistics(ArchiveSettings.ModuleId, categoriesToDisplay, null, authorId, sortBy, ArticleSettings.ShowPending);
                        dlNewsArchives.DataBind();
                    }
                    return;
                }

                if (ArticleSettings.FilterCategories != null)
                {
                    if (ArchiveSettings.LayoutMode == LayoutModeType.Simple)
                    {
                        rptNewsArchives.DataSource = objAuthorController.GetAuthorStatistics(ArchiveSettings.ModuleId, ArticleSettings.FilterCategories, null, authorId, sortBy, ArticleSettings.ShowPending);
                        rptNewsArchives.DataBind();
                    }
                    else
                    {
                        dlNewsArchives.RepeatColumns = ArchiveSettings.ItemsPerRow;
                        dlNewsArchives.DataSource = objAuthorController.GetAuthorStatistics(ArchiveSettings.ModuleId, ArticleSettings.FilterCategories, null, authorId, sortBy, ArticleSettings.ShowPending);
                        dlNewsArchives.DataBind();
                    }
                    return;
                }

                CategoryController objCategoryController = new CategoryController();
                List<CategoryInfo> objCategories = objCategoryController.GetCategoriesAll(ArchiveSettings.ModuleId, Null.NullInteger);

                List<int> excludeCategoriesRestrictive = new List<int>();

                foreach (CategoryInfo objCategory in objCategories)
                {
                    if (!objCategory.InheritSecurity && objCategory.CategorySecurityType == CategorySecurityType.Restrict)
                    {
                        if (Request.IsAuthenticated)
                        {
                            if (newsSettings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                            {
                                if (!PortalSecurity.IsInRoles(newsSettings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString()))
                                {
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

                foreach (CategoryInfo objCategory in objCategories)
                {
                    if (!objCategory.InheritSecurity && objCategory.CategorySecurityType == CategorySecurityType.Loose)
                    {
                        if (Request.IsAuthenticated)
                        {
                            if (newsSettings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                            {
                                if (!PortalSecurity.IsInRoles(newsSettings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString()))
                                {
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

                List<int> includeCategories = new List<int>();

                if (excludeCategories.Count > 0)
                {

                    foreach (CategoryInfo objCategoryToInclude in objCategories)
                    {

                        bool includeCategory = true;

                        foreach (int exclCategory in excludeCategories)
                        {
                            if (exclCategory == objCategoryToInclude.CategoryID)
                            {
                                includeCategory = false;
                            }
                        }

                        if (includeCategory)
                        {
                            includeCategories.Add(objCategoryToInclude.CategoryID);
                        }

                    }

                    if (includeCategories.Count > 0)
                    {
                        includeCategories.Add(-1);
                    }

                }

                if (ArchiveSettings.LayoutMode == LayoutModeType.Simple)
                {
                    rptNewsArchives.DataSource = objAuthorController.GetAuthorStatistics(ArchiveSettings.ModuleId, includeCategories.ToArray(), excludeCategoriesRestrictive.ToArray(), authorId, sortBy, ArticleSettings.ShowPending);
                    rptNewsArchives.DataBind();
                }
                else
                {
                    dlNewsArchives.RepeatColumns = ArchiveSettings.ItemsPerRow;
                    dlNewsArchives.DataSource = objAuthorController.GetAuthorStatistics(ArchiveSettings.ModuleId, includeCategories.ToArray(), excludeCategoriesRestrictive.ToArray(), authorId, sortBy, ArticleSettings.ShowPending);
                    dlNewsArchives.DataBind();
                }
            }

        }

        private void ProcessBody(ControlCollection dataControls, ArchiveInfo objArchive)
        {

            Literal literal = new Literal();

            if (ArchiveSettings.LayoutMode == LayoutModeType.Simple)
            {
                literal.Text = ArchiveSettings.TemplateDateBody.ToString();
            }
            else
            {
                literal.Text = ArchiveSettings.TemplateDateAdvancedBody.ToString();
            }

            string delimStr = "[]";
            char[] delimiter = delimStr.ToCharArray();
            ProcessArchive(dataControls, literal.Text.Split(delimiter), objArchive);

        }

        private void ProcessArchive(ControlCollection dataControls, string[] layoutArray, ArchiveInfo objArchive)
        {

            DateTime archiveDate = new DateTime(objArchive.Year, objArchive.Month, objArchive.Day);
            Literal objLiteral;
            bool isValid;

            for (int iPtr = 0; iPtr < layoutArray.Length; iPtr += 2)
            {
                dataControls.Add(new LiteralControl(layoutArray[iPtr].ToString()));

                if (iPtr < layoutArray.Length - 1)
                {
                    switch (layoutArray[iPtr + 1])
                    {

                        case "COUNT":
                            objLiteral = new Literal();
                            objLiteral.Text = objArchive.Count.ToString();
                            dataControls.Add(objLiteral);
                            break;
                        case "ISSELECTEDMONTH":
                            isValid = false;
                            if (ArchiveSettings.GroupBy == GroupByType.Month)
                            {
                                if (Request["month"] != null)
                                {
                                    if (Request["month"] == objArchive.Month.ToString())
                                    {
                                        isValid = true;
                                    }
                                }
                            }
                            if (!isValid)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISSELECTEDMONTH")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "ISNOTSELECTEDMONTH":
                            isValid = false;
                            if (ArchiveSettings.GroupBy == GroupByType.Month)
                            {
                                if (Request["month"] != null)
                                {
                                    if (Request["month"] == objArchive.Month.ToString())
                                    {
                                        isValid = true;
                                    }
                                }
                            }
                            if (isValid)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISNOTSELECTEDMONTH")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "ISSELECTEDYEAR":
                            isValid = false;
                            if (ArchiveSettings.GroupBy == GroupByType.Year || ArchiveSettings.GroupBy == GroupByType.Month)
                            {
                                if (Request["year"] != null)
                                {
                                    if (Request["year"] == objArchive.Year.ToString())
                                    {
                                        isValid = true;
                                    }
                                }
                            }
                            if (!isValid)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISSELECTEDYEAR")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "ISNOTSELECTEDYEAR":
                            isValid = false;
                            if (Request["year"] != null)
                            {
                                if (Request["year"] == objArchive.Year.ToString())
                                {
                                    isValid = true;
                                }
                            }
                            if (isValid)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISNOTSELECTEDYEAR")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "LINK":
                            objLiteral = new Literal();
                            if (ArchiveSettings.GroupBy == GroupByType.Month)
                            {
                                objLiteral.Text = Common.GetModuleLink(ArchiveSettings.TabId, ArchiveSettings.ModuleId, "ArchiveView", ArticleSettings, "month=" + objArchive.Month.ToString(), "year=" + objArchive.Year.ToString());
                            }
                            else
                            {
                                objLiteral.Text = Common.GetModuleLink(ArchiveSettings.TabId, ArchiveSettings.ModuleId, "ArchiveView", ArticleSettings, "year=" + objArchive.Year.ToString());
                            }
                            dataControls.Add(objLiteral);
                            break;
                        case "MONTH":
                            objLiteral = new Literal();
                            if (ArchiveSettings.GroupBy == GroupByType.Month)
                            {
                                objLiteral.Text = archiveDate.ToString("MMMM");
                            }
                            else
                            {
                                objLiteral.Text = "";
                            }
                            dataControls.Add(objLiteral);
                            break;
                        case "YEAR":
                            objLiteral = new Literal();
                            objLiteral.Text = archiveDate.Year.ToString();
                            dataControls.Add(objLiteral);
                            break;
                    }
                }

            }

        }

        private void ProcessBody(ControlCollection dataControls, CategoryInfo objCategory)
        {

            Literal literal = new Literal();

            if (ArchiveSettings.LayoutMode == LayoutModeType.Simple)
            {
                literal.Text = ArchiveSettings.TemplateCategoryBody.ToString();
            }
            else
            {
                literal.Text = ArchiveSettings.TemplateCategoryAdvancedBody.ToString();
            }

            if (literal.Text.Contains("[DEPTHABS]"))
            {

                foreach (CategoryInfo objCategorySelected in Categories)
                {
                    if (objCategorySelected.CategoryID == objCategory.CategoryID)
                    {
                        literal.Text = literal.Text.Replace("[DEPTHABS]", objCategorySelected.Level.ToString());
                    }
                }

            }

            literal.Text = literal.Text.Replace("[CATEGORYID]", objCategory.CategoryID.ToString());
            literal.Text = literal.Text.Replace("[CATEGORY]", objCategory.NameIndented.ToString());
            literal.Text = literal.Text.Replace("[CATEGORYNOTINDENTED]", objCategory.Name.ToString());
            literal.Text = literal.Text.Replace("[COUNT]", objCategory.NumberOfArticles.ToString());
            literal.Text = literal.Text.Replace("[DEPTHREL]", objCategory.Level.ToString());
            literal.Text = literal.Text.Replace("[DESCRIPTION]", Server.HtmlDecode(objCategory.Description));

            literal.Text = literal.Text.Replace("[LINK]", Common.GetCategoryLink(ArchiveSettings.TabId, ArchiveSettings.ModuleId, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings));

            if (ArchiveSettings.CategoryHideZeroCategories && objCategory.NumberOfArticles == 0)
            {
                return;
            }

            dataControls.Add(literal);

        }

        private void ProcessBody(ControlCollection dataControls, AuthorInfo objAuthor)
        {

            Literal literal = new Literal();

            if (ArchiveSettings.LayoutMode == LayoutModeType.Simple)
            {
                literal.Text = ArchiveSettings.TemplateAuthorBody.ToString();
            }
            else
            {
                literal.Text = ArchiveSettings.TemplateAuthorAdvancedBody.ToString();
            }


            literal.Text = literal.Text.Replace("[AUTHORID]", objAuthor.UserID.ToString());
            literal.Text = literal.Text.Replace("[AUTHORUSERNAME]", objAuthor.UserName.ToString());
            literal.Text = literal.Text.Replace("[AUTHORDISPLAYNAME]", objAuthor.DisplayName.ToString());
            literal.Text = literal.Text.Replace("[AUTHORFIRSTNAME]", objAuthor.FirstName.ToString());
            literal.Text = literal.Text.Replace("[AUTHORLASTNAME]", objAuthor.LastName.ToString());
            literal.Text = literal.Text.Replace("[AUTHORFULLNAME]", objAuthor.FullName.ToString());
            literal.Text = literal.Text.Replace("[COUNT]", objAuthor.ArticleCount.ToString());
            literal.Text = literal.Text.Replace("[LINK]", Common.GetAuthorLink(ArchiveSettings.TabId, ArchiveSettings.ModuleId, objAuthor.UserID, objAuthor.UserName, ArticleSettings.LaunchLinks, ArticleSettings));

            dataControls.Add(literal);

        }

        private void ProcessTemplate(ControlCollection listControls, ListItemType li, object obj)
        {

            if (li == ListItemType.Header)
            {

                Literal objLiteral = new Literal();

                switch (ArchiveSettings.Mode)
                {

                    case ArchiveModeType.Date:
                        if (ArchiveSettings.LayoutMode == LayoutModeType.Simple)
                        {
                            objLiteral.Text = ArchiveSettings.TemplateDateHeader.ToString();
                        }
                        else
                        {
                            objLiteral.Text = ArchiveSettings.TemplateDateAdvancedHeader.ToString();
                        }
                        break;
                    case ArchiveModeType.Category:
                        if (ArchiveSettings.LayoutMode == LayoutModeType.Simple)
                        {
                            objLiteral.Text = ArchiveSettings.TemplateCategoryHeader.ToString();
                        }
                        else
                        {
                            objLiteral.Text = ArchiveSettings.TemplateCategoryAdvancedHeader.ToString();
                        }
                        break;
                    case ArchiveModeType.Author:
                        if (ArchiveSettings.LayoutMode == LayoutModeType.Simple)
                        {
                            objLiteral.Text = ArchiveSettings.TemplateAuthorHeader.ToString();
                        }
                        else
                        {
                            objLiteral.Text = ArchiveSettings.TemplateAuthorAdvancedHeader.ToString();
                        }
                        break;
                }

                listControls.Add(objLiteral);

            }

            if (li == ListItemType.Item || li == ListItemType.AlternatingItem)
            {

                switch (ArchiveSettings.Mode)
                {

                    case ArchiveModeType.Date:
                        ProcessBody(listControls, (ArchiveInfo)obj);
                        break;
                    case ArchiveModeType.Category:
                        ProcessBody(listControls, (CategoryInfo)obj);
                        break;
                    case ArchiveModeType.Author:
                        ProcessBody(listControls, (AuthorInfo)obj);
                        break;
                }

            }

            if (li == ListItemType.Footer)
            {

                Literal objLiteral = new Literal();

                switch (ArchiveSettings.Mode)
                {

                    case ArchiveModeType.Date:
                        if (ArchiveSettings.LayoutMode == LayoutModeType.Simple)
                        {
                            objLiteral.Text = ArchiveSettings.TemplateDateFooter.ToString();
                        }
                        else
                        {
                            objLiteral.Text = ArchiveSettings.TemplateDateAdvancedFooter.ToString();
                        }
                        break;
                    case ArchiveModeType.Category:
                        if (ArchiveSettings.LayoutMode == LayoutModeType.Simple)
                        {
                            objLiteral.Text = ArchiveSettings.TemplateCategoryFooter.ToString();
                        }
                        else
                        {
                            objLiteral.Text = ArchiveSettings.TemplateCategoryAdvancedFooter.ToString();
                        }
                        break;
                    case ArchiveModeType.Author:
                        if (ArchiveSettings.LayoutMode == LayoutModeType.Simple)
                        {
                            objLiteral.Text = ArchiveSettings.TemplateAuthorFooter.ToString();
                        }
                        else
                        {
                            objLiteral.Text = ArchiveSettings.TemplateAuthorAdvancedFooter.ToString();
                        }
                        break;
                }

                listControls.Add(objLiteral);

            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            rptNewsArchives.ItemDataBound += rptNewsArchives_OnItemDataBound;
            dlNewsArchives.ItemDataBound += dlNewsArchives_OnItemDataBound;
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
                    BindArchive();
                }
                else
                {
                    divNotConfigured.Visible = true;
                    rptNewsArchives.Visible = false;
                }
            }
            catch (Exception exc)  //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void rptNewsArchives_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            try
            {

                ProcessTemplate(e.Item.Controls, e.Item.ItemType, e.Item.DataItem);

            }
            catch (Exception exc)  //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void dlNewsArchives_OnItemDataBound(object sender, DataListItemEventArgs e)
        {

            try
            {

                ProcessTemplate(e.Item.Controls, e.Item.ItemType, e.Item.DataItem);

            }
            catch (Exception exc)  //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion

    }
}