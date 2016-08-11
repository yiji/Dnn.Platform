using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.UserControls;
using DotNetNuke.Security.Roles;
using GcDesign.NewsArticles.Components.Social;
using GcDesign.NewsArticles.Components.CustomFields;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Security.Permissions;
using System.IO;
using GcDesign.NewsArticles.Base;
using System.Collections.Specialized;
using System.Collections;
using DotNetNuke.Services.Log.EventLog;
using System.Web.UI.HtmlControls;

namespace GcDesign.NewsArticles
{
    public partial class ucSubmitNews : NewsArticleModuleBase
    {

        #region " private Members "

        private int _articleID = Null.NullInteger;
        private string _returnUrl = Null.NullString;
        private NameValueCollection _richTextValues = new NameValueCollection();

        #endregion

        #region " private Properties "

        private DateTime PublishDate
        {
            set
            {
                txtPublishHour.Text = value.Hour.ToString();
                txtPublishMinute.Text = value.Minute.ToString();
                txtPublishDate.Text = new DateTime(value.Year, value.Month, value.Day).ToShortDateString();
            }
        }

        private DateTime PublishDate_m(int defaultHour, int defaultMinute)
        {
            int year = Convert.ToDateTime(txtPublishDate.Text).Year;
            int month = Convert.ToDateTime(txtPublishDate.Text).Month;
            int day = Convert.ToDateTime(txtPublishDate.Text).Day;

            int hour = defaultHour;
            if (Numeric.IsNumeric(txtPublishHour.Text))
            {
                if (hour >= 0 && hour <= 23)
                {
                    hour = Convert.ToInt32(txtPublishHour.Text);
                }
            }

            int minute = defaultMinute;
            if (Numeric.IsNumeric(txtPublishMinute.Text))
            {
                if (minute >= 0 && minute <= 60)
                {
                    minute = Convert.ToInt32(txtPublishMinute.Text);
                }
            }

            return new DateTime(year, month, day, hour, minute, 0);
        }

        private DateTime ExpiryDate
        {

            set
            {
                if (value == Null.NullDate)
                {
                    txtExpiryDate.Text = "";
                    return;
                }
                txtExpiryHour.Text = value.Hour.ToString();
                txtExpiryMinute.Text = value.Minute.ToString();
                txtExpiryDate.Text = new DateTime(value.Year, value.Month, value.Day).ToShortDateString();
            }
        }

        private DateTime ExpiryDate_m(int defaultHour, int defaultMinute)
        {

            if (txtExpiryDate.Text.Length == 0)
            {
                return Null.NullDate;
            }

            int year = Convert.ToDateTime(txtExpiryDate.Text).Year;
            int month = Convert.ToDateTime(txtExpiryDate.Text).Month;
            int day = Convert.ToDateTime(txtExpiryDate.Text).Day;

            int hour = defaultHour;
            if (Numeric.IsNumeric(txtExpiryHour.Text))
            {
                if (hour >= 0 && hour <= 23)
                {
                    hour = Convert.ToInt32(txtExpiryHour.Text);
                }
            }

            int minute = defaultMinute;
            if (Numeric.IsNumeric(txtExpiryMinute.Text))
            {
                if (minute >= 0 && minute <= 60)
                {
                    minute = Convert.ToInt32(txtExpiryMinute.Text);
                }
            }

            return new DateTime(year, month, day, hour, minute, 0);

        }

        private TextEditor Details
        {
            get
            {
                return (TextEditor)txtDetails;
            }
        }

        private UrlControl UrlLink
        {
            get
            {
                return (UrlControl)ctlUrlLink;
            }
        }

        private DotNetNuke.UI.UserControls.TextEditor ExcerptRich
        {
            get
            {
                return (TextEditor)txtExcerptRich;
            }
        }

        #endregion

        #region " private Methods "

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

        private void BindArticle()
        {

            if (_articleID != Null.NullInteger)
            {

                ArticleController objArticleController = new ArticleController();

                ArticleInfo objArticle = objArticleController.GetArticle(_articleID);

                lblAuthor.Text = objArticle.AuthorDisplayName;
                txtTitle.Text = objArticle.Title;
                if (ArticleSettings.TextEditorSummaryMode == Components.Types.TextEditorModeType.Basic)
                {
                    txtExcerptBasic.Text = objArticle.Summary.Replace("&lt;br /&gt;", "\r\n");
                }
                else
                {
                    ExcerptRich.Text = objArticle.Summary;
                }
                chkFeatured.Checked = objArticle.IsFeatured;
                chkSecure.Checked = objArticle.IsSecure;

                txtMetaTitle.Text = objArticle.MetaTitle;
                txtMetaDescription.Text = objArticle.MetaDescription;
                txtMetaKeyWords.Text = objArticle.MetaKeywords;
                txtPageHeadText.Text = objArticle.PageHeadText;

                if (drpStatus.Items.FindByValue(objArticle.Status.ToString()) != null)
                {
                    drpStatus.SelectedValue = objArticle.Status.ToString();
                }

                PublishDate = objArticle.StartDate;

                if (objArticle.EndDate != Null.NullDate)
                {
                    ExpiryDate = objArticle.EndDate;
                }

                if (ArticleSettings.IsImagesEnabled)
                {
                    if (objArticle.ImageUrl != Null.NullString)
                    {
                        if (objArticle.ImageUrl.ToLower().StartsWith("http://") || objArticle.ImageUrl.ToLower().StartsWith("https://"))
                        {
                            if (ArticleSettings.EnableExternalImages)
                            {
                                ucUploadImages.ImageExternalUrl = objArticle.ImageUrl;
                            }
                        }
                    }
                }

                UrlLink.Url = objArticle.Url;
                chkNewWindow.Checked = objArticle.IsNewWindow;

                ArrayList categories = objArticleController.GetArticleCategories(_articleID);
                foreach (CategoryInfo category in categories)
                {
                    ListItem li = lstCategories.Items.FindByValue(category.CategoryID.ToString());
                    if (li != null)
                    {
                        li.Selected = true;
                    }
                }
                txtTags.Text = objArticle.Tags;

                PageController objPageController = new PageController();
                ArrayList pages = objPageController.GetPageList(_articleID);
                if (pages.Count > 0)
                {
                    Details.Text = ((PageInfo)pages[0]).PageText;
                }

                cmdDelete.Visible = true;
                cmdDelete.OnClientClick = "return confirm('" + DotNetNuke.Services.Localization.Localization.GetString("Delete.Text", LocalResourceFile) + "');";

                MirrorArticleController objMirrorArticleController = new MirrorArticleController();
                MirrorArticleInfo objMirrorArticleInfo = objMirrorArticleController.GetMirrorArticle(_articleID);

                if (objMirrorArticleInfo != null)
                {

                    phMirrorText.Visible = true;
                    if (objMirrorArticleInfo.AutoUpdate)
                    {
                        lblMirrorText.Text = Localization.GetString("MirrorTextUpdate", this.LocalResourceFile);

                        if (lblMirrorText.Text.IndexOf("{0}") != -1)
                        {
                            lblMirrorText.Text = lblMirrorText.Text.Replace("{0}", objMirrorArticleInfo.PortalName);
                        }

                        phCreate.Visible = false;
                        phOrganize.Visible = false;
                        phPublish.Visible = false;

                        cmdPublishArticle.Visible = false;
                        cmdAddEditPages.Visible = false;
                    }
                    else
                    {
                        lblMirrorText.Text = Localization.GetString("MirrorText", this.LocalResourceFile);

                        if (lblMirrorText.Text.IndexOf("{0}") != -1)
                        {
                            lblMirrorText.Text = lblMirrorText.Text.Replace("{0}", objMirrorArticleInfo.PortalName);
                        }
                    }

                }

                ArrayList objMirrorArticleLinked = objMirrorArticleController.GetMirrorArticleList(_articleID);

                if (objMirrorArticleLinked.Count > 0)
                {

                    phMirrorText.Visible = true;
                    lblMirrorText.Text = Localization.GetString("MirrorTextLinked", this.LocalResourceFile);

                    if (lblMirrorText.Text.IndexOf("{0}") != -1)
                    {
                        lblMirrorText.Text = lblMirrorText.Text.Replace("{0}", objMirrorArticleLinked.Count.ToString());
                    }

                }

            }
            else
            {

                chkFeatured.Checked = ArticleSettings.IsAutoFeatured;
                chkSecure.Checked = ArticleSettings.IsAutoSecured;
                if (ArticleSettings.AuthorDefault != Null.NullInteger)
                {
                    UserInfo objUser = UserController.Instance.GetUser(PortalId, ArticleSettings.AuthorDefault);

                    if (objUser != null)
                    {
                        lblAuthor.Text = objUser.Username;
                    }
                    else
                    {
                        lblAuthor.Text = this.UserInfo.Username;
                    }
                }
                else
                {
                    lblAuthor.Text = this.UserInfo.Username;
                }
                PublishDate = DateTime.Now;
                cmdDelete.Visible = false;

                if (Settings.Contains(ArticleConstants.DEFAULT_CATEGORIES_SETTING))
                {
                    if (Settings[ArticleConstants.DEFAULT_CATEGORIES_SETTING].ToString() != Null.NullString)
                    {
                        string[] categories = Settings[ArticleConstants.DEFAULT_CATEGORIES_SETTING].ToString().Split(',');

                        foreach (string category in categories)
                        {
                            if (lstCategories.Items.FindByValue(category) != null)
                            {
                                lstCategories.Items.FindByValue(category).Selected = true;
                            }
                        }
                    }
                }

            }

        }


        private void BindCategories()
        {

            CategoryController objCategoryController = new CategoryController();

            List<CategoryInfo> objCategoriesSelected = new List<CategoryInfo>();
            List<CategoryInfo> objCategories = objCategoryController.GetCategoriesAll(ModuleId, Null.NullInteger, ArticleSettings.CategorySortType);

            if (ArticleSettings.CategoryFilterSubmit)
            {
                if (ArticleSettings.FilterSingleCategory != Null.NullInteger)
                {
                    List<CategoryInfo> objSelectedCategories = new List<CategoryInfo>();
                    foreach (CategoryInfo objCategory in objCategories)
                    {
                        if (objCategory.CategoryID == ArticleSettings.FilterSingleCategory)
                        {
                            objSelectedCategories.Add(objCategory);
                            break;
                        }
                    }
                    objCategories = objSelectedCategories;
                }

                if (ArticleSettings.FilterCategories != null)
                {
                    if (ArticleSettings.FilterCategories.Length > 0)
                    {
                        List<CategoryInfo> objSelectedCategories = new List<CategoryInfo>();
                        foreach (int i in ArticleSettings.FilterCategories)
                        {
                            foreach (CategoryInfo objCategory in objCategories)
                            {
                                if (objCategory.CategoryID == i)
                                {
                                    objSelectedCategories.Add(objCategory);
                                    break;
                                }
                            }
                        }
                        objCategories = objSelectedCategories;
                    }
                }
            }

            foreach (CategoryInfo objCategory in objCategories)
            {
                if (objCategory.InheritSecurity)
                {
                    objCategoriesSelected.Add(objCategory);
                }
                else
                {
                    if (Request.IsAuthenticated)
                    {
                        if (Settings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_SUBMIT_SETTING))
                        {
                            if (PortalSecurity.IsInRoles(Settings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_SUBMIT_SETTING].ToString()))
                            {
                                objCategoriesSelected.Add(objCategory);
                            }
                        }
                    }
                }
            }

            lstCategories.DataSource = objCategoriesSelected;
            lstCategories.DataBind();

            if (Settings.Contains(ArticleConstants.REQUIRE_CATEGORY))
            {
                valCategory.Enabled = Convert.ToBoolean(Settings[ArticleConstants.REQUIRE_CATEGORY].ToString());
            }

        }

        private void BindCustomFields()
        {

            CustomFieldController objCustomFieldController = new CustomFieldController();
            ArrayList objCustomFields = objCustomFieldController.List(this.ModuleId);

            if (objCustomFields.Count == 0)
            {
                phCustomFields.Visible = false;
            }
            else
            {
                phCustomFields.Visible = true;
                rptCustomFields.DataSource = objCustomFields;
                rptCustomFields.DataBind();
            }

        }

        private void BindStatus()
        {

            foreach (int value in System.Enum.GetValues(typeof(StatusType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(StatusType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(StatusType), value), this.LocalResourceFile);
                drpStatus.Items.Add(li);
            }

        }

        private void CheckSecurity()
        {

            if (!HasEditRights(_articleID, this.ModuleId, this.TabId))
            {
                Response.Redirect(Common.GetModuleLink(this.TabId, this.ModuleId, "NotAuthorized", ArticleSettings), true);
            }

        }

        public ArrayList GetAuthorList(int moduleID)
        {
            ModuleInfo module = ModuleController.Instance.GetModule(moduleID, PortalId, false);
            Hashtable moduleSettings = module.ModuleSettings;
            //string distributionList = "";
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
                            //获取某个角色中的人员
                            //objRoleController.GetUserRolesByRoleName(PortalSettings.PortalId, objRole.RoleName);
                            List<UserInfo> objUsers = RoleController.Instance.GetUsersByRole(PortalId,objRole.RoleName).ToList();
                            foreach (UserInfo objUser in objUsers)
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

        private void PopulateAuthorList()
        {

            ddlAuthor.DataSource = GetAuthorList(this.ModuleId);
            ddlAuthor.DataBind();
            ddlAuthor.Items.Insert(0, new ListItem(Localization.GetString("SelectAuthor.Text", this.LocalResourceFile), "-1"));

        }

        private void ReadQueryString()
        {

            if (Request["ArticleID"] != null)
            {
                if (Numeric.IsNumeric(Request["ArticleID"]))
                {
                    _articleID = Convert.ToInt32(Request["ArticleID"]);
                }
            }

            if (Request["ReturnUrl"] != null)
            {
                _returnUrl = Request["ReturnUrl"];
            }

        }

        private int SaveArticle()
        {

            if (_articleID != Null.NullInteger)
            {
                ArticleController objArticleController = new ArticleController();
                ArticleInfo objArticle = objArticleController.GetArticle(_articleID);

                if (objArticle != null)
                {
                    return SaveArticle(objArticle.Status);
                }
                return SaveArticle(StatusType.Draft);
            }
            else
            {
                return SaveArticle(StatusType.Draft);
            }

        }

        private int SaveArticle(StatusType status)
        {

            ArticleController objArticleController = new ArticleController();
            ArticleInfo objArticle = new ArticleInfo();

            bool statusChanged = false;
            bool publishArticle = false;

            if (_articleID == Null.NullInteger)
            {
                if (!pnlAuthor.Visible)
                {
                    UserInfo objUser = UserController.GetUserByName(this.PortalId, lblAuthor.Text);

                    if (objUser != null)
                    {
                        objArticle.AuthorID = objUser.UserID;
                    }
                    else
                    {
                        objArticle.AuthorID = this.UserId;
                    }
                }
                else
                {
                    UserInfo objUser = UserController.GetUserByName(this.PortalId, txtAuthor.Text);

                    if (objUser != null)
                    {
                        objArticle.AuthorID = objUser.UserID;
                    }
                    else
                    {
                        objArticle.AuthorID = this.UserId;
                    }
                }

                if (ddlAuthor.Visible)
                {
                    objArticle.AuthorID = Convert.ToInt32(ddlAuthor.SelectedValue);
                }
                objArticle.CreatedDate = DateTime.Now;
                objArticle.Status = StatusType.Draft;
                objArticle.CommentCount = 0;
                objArticle.RatingCount = 0;
                objArticle.Rating = 0;
                objArticle.ShortUrl = "";
            }
            else
            {
                objArticle = objArticleController.GetArticle(_articleID);
                objArticleController.DeleteArticleCategories(_articleID);

                if (objArticle.Status != StatusType.Published && status == StatusType.Published)
                {
                    // Article now approved, notify if not an Approver.
                    if (objArticle.AuthorID != this.UserId)
                    {
                        statusChanged = true;
                    }
                }

                if (pnlAuthor.Visible)
                {
                    UserInfo objUser = UserController.GetUserByName(this.PortalId, txtAuthor.Text);

                    if (objUser != null)
                    {
                        objArticle.AuthorID = objUser.UserID;
                    }
                }

                if (ddlAuthor.Visible)
                {
                    objArticle.AuthorID = Convert.ToInt32(ddlAuthor.SelectedValue);
                }
            }

            objArticle.MetaTitle = txtMetaTitle.Text.Trim();
            objArticle.MetaDescription = txtMetaDescription.Text.Trim();
            objArticle.MetaKeywords = txtMetaKeyWords.Text.Trim();
            objArticle.PageHeadText = txtPageHeadText.Text.Trim();

            if (chkMirrorArticle.Checked && drpMirrorArticle.Items.Count > 0)
            {
                ArticleInfo objLinkedArticle = objArticleController.GetArticle(Convert.ToInt32(drpMirrorArticle.SelectedValue));

                if (objLinkedArticle != null)
                {
                    objArticle.Title = objLinkedArticle.Title;
                    objArticle.Summary = objLinkedArticle.Summary;
                    objArticle.Url = objLinkedArticle.Url;
                    objArticle.IsNewWindow = objLinkedArticle.IsNewWindow;
                    objArticle.ImageUrl = objLinkedArticle.ImageUrl;

                    objArticle.MetaTitle = objLinkedArticle.MetaTitle;
                    objArticle.MetaDescription = objLinkedArticle.MetaDescription;
                    objArticle.MetaKeywords = objLinkedArticle.MetaKeywords;
                    objArticle.PageHeadText = objLinkedArticle.PageHeadText;
                }
            }
            else
            {
                objArticle.Title = txtTitle.Text;
                if (ArticleSettings.TextEditorSummaryMode == Components.Types.TextEditorModeType.Basic)
                {
                    objArticle.Summary = txtExcerptBasic.Text.Replace("\r\n", "&lt;br /&gt;");
                }
                else
                {
                    if (ExcerptRich.Text != "" && StripHtml(ExcerptRich.Text).Length > 0 && ExcerptRich.Text != "&lt;p&gt;&amp;#160;&lt;/p&gt;")
                    {
                        objArticle.Summary = ExcerptRich.Text;
                    }
                    else
                    {
                        objArticle.Summary = Null.NullString;
                    }
                }
                objArticle.Url = UrlLink.Url;
                objArticle.IsNewWindow = chkNewWindow.Checked;
                if (ArticleSettings.IsImagesEnabled)
                {
                    objArticle.ImageUrl = ucUploadImages.ImageExternalUrl;
                }
            }

            objArticle.IsFeatured = chkFeatured.Checked;
            objArticle.IsSecure = chkSecure.Checked;
            objArticle.LastUpdate = DateTime.Now;
            objArticle.LastUpdateID = this.UserId;
            objArticle.ModuleID = this.ModuleId;

            objArticle.Status = status;

            if (objArticle.StartDate != Null.NullDate)
            {
                objArticle.StartDate = PublishDate_m(objArticle.StartDate.Hour, objArticle.StartDate.Minute);
            }
            else
            {
                objArticle.StartDate = PublishDate_m(DateTime.Now.Hour, DateTime.Now.Minute);
            }

            if (ExpiryDate_m(0, 0) == Null.NullDate)
            {
                objArticle.EndDate = Null.NullDate;
            }
            else
            {
                if (objArticle.EndDate != Null.NullDate)
                {
                    objArticle.EndDate = ExpiryDate_m(objArticle.EndDate.Hour, objArticle.EndDate.Minute);
                }
                else
                {
                    objArticle.EndDate = ExpiryDate_m(0, 0);
                }
            }

            if (_articleID == Null.NullInteger)
            {
                objArticle.ArticleID = objArticleController.AddArticle(objArticle);
                ucUploadImages.UpdateImages(objArticle.ArticleID);
                ucUploadFiles.UpdateFiles(objArticle.ArticleID);
                objArticleController.UpdateArticle(objArticle);

                if (chkMirrorArticle.Checked && drpMirrorArticle.Items.Count > 0)
                {

                    // Mirrored Article
                    MirrorArticleInfo objMirrorArticleInfo = new MirrorArticleInfo();

                    objMirrorArticleInfo.ArticleID = objArticle.ArticleID;
                    objMirrorArticleInfo.LinkedArticleID = Convert.ToInt32(drpMirrorArticle.SelectedValue);
                    objMirrorArticleInfo.LinkedPortalID = Convert.ToInt32(drpMirrorModule.SelectedValue.Split('-')[0]);
                    objMirrorArticleInfo.AutoUpdate = chkMirrorAutoUpdate.Checked;

                    MirrorArticleController objMirrorArticleController = new MirrorArticleController();
                    objMirrorArticleController.AddMirrorArticle(objMirrorArticleInfo);

                    //Copy Files
                    string folderLinked = "";

                    ModuleController objModuleController = new ModuleController();
                    Hashtable objSettingsLinked = objModuleController.GetModule(Convert.ToInt32((drpMirrorModule.SelectedValue.Split('-')[1]))).ModuleSettings;

                    if (objSettingsLinked.ContainsKey(ArticleConstants.DEFAULT_FILES_FOLDER_SETTING))
                    {
                        if (Numeric.IsNumeric(objSettingsLinked[ArticleConstants.DEFAULT_FILES_FOLDER_SETTING].ToString()))
                        {
                            int folderID = Convert.ToInt32(objSettingsLinked[ArticleConstants.DEFAULT_FILES_FOLDER_SETTING]);
                            FolderInfo objFolder = (FolderInfo)FolderManager.Instance.GetFolder(folderID);
                            if (objFolder != null)
                            {
                                folderLinked = objFolder.FolderPath;
                            }
                        }
                    }

                    PortalController objPortalController = new PortalController();
                    PortalInfo objPortalLinked = objPortalController.GetPortal(Convert.ToInt32(drpMirrorModule.SelectedValue.Split('-')[0]));

                    string filePathLinked = objPortalLinked.HomeDirectoryMapPath + folderLinked.Replace("/", @"\");

                    string folder = "";

                    Hashtable objSettings = ModuleConfiguration.ModuleSettings;

                    if (objSettings.ContainsKey(ArticleConstants.DEFAULT_FILES_FOLDER_SETTING))
                    {
                        if (Numeric.IsNumeric(objSettings[ArticleConstants.DEFAULT_FILES_FOLDER_SETTING].ToString()))
                        {
                            int folderID = Convert.ToInt32(objSettings[ArticleConstants.DEFAULT_FILES_FOLDER_SETTING]);
                            FolderInfo objFolder = (FolderInfo)FolderManager.Instance.GetFolder(folderID);
                            if (objFolder != null)
                            {
                                folder = objFolder.FolderPath;
                            }
                        }
                    }

                    FileController objFileController = new FileController();
                    List<FileInfo> objFiles = objFileController.GetFileList(objMirrorArticleInfo.LinkedArticleID, Null.NullString);

                    foreach (FileInfo objFile in objFiles)
                    {

                        if (File.Exists(filePathLinked + objFile.FileName))
                        {

                            string finalCopyPath = filePathLinked + objFile.FileName;

                            string filePath = PortalSettings.HomeDirectoryMapPath + folder.Replace("/", @"\");

                            if (!Directory.Exists(filePath))
                            {
                                Directory.CreateDirectory(filePath);
                            }

                            if (File.Exists(filePath + objFile.FileName))
                            {
                                for (int i = 1; i <= 100; i++)
                                {
                                    if (!File.Exists(filePath + i.ToString() + "_" + objFile.FileName))
                                    {
                                        objFile.FileName = i.ToString() + "_" + objFile.FileName;
                                        break;
                                    }
                                }
                            }

                            File.Copy(finalCopyPath, filePath + objFile.FileName);

                            objFile.ArticleID = objArticle.ArticleID;
                            objFileController.Add(objFile);

                        }

                    }

                    //Copy Images

                    string folderImagesLinked = "";

                    if (objSettingsLinked.ContainsKey(ArticleConstants.DEFAULT_IMAGES_FOLDER_SETTING))
                    {
                        if (Numeric.IsNumeric(objSettingsLinked[ArticleConstants.DEFAULT_IMAGES_FOLDER_SETTING].ToString()))
                        {
                            int folderID = Convert.ToInt32(objSettingsLinked[ArticleConstants.DEFAULT_IMAGES_FOLDER_SETTING]);
                            FolderInfo objFolder = (FolderInfo)FolderManager.Instance.GetFolder(folderID);
                            if (objFolder != null)
                            {
                                folderImagesLinked = objFolder.FolderPath;
                            }
                        }
                    }

                    string filePathImagesLinked = objPortalLinked.HomeDirectoryMapPath + folderImagesLinked.Replace("/", @"\");

                    string folderImages = "";

                    if (objSettings.ContainsKey(ArticleConstants.DEFAULT_IMAGES_FOLDER_SETTING))
                    {
                        if (Numeric.IsNumeric(objSettings[ArticleConstants.DEFAULT_IMAGES_FOLDER_SETTING].ToString()))
                        {
                            int folderID = Convert.ToInt32(objSettings[ArticleConstants.DEFAULT_IMAGES_FOLDER_SETTING]);
                            FolderInfo objFolder = (FolderInfo)FolderManager.Instance.GetFolder(folderID);
                            if (objFolder != null)
                            {
                                folderImages = objFolder.FolderPath;
                            }
                        }
                    }

                    ImageController objImageController = new ImageController();
                    List<ImageInfo> objImages = objImageController.GetImageList(objMirrorArticleInfo.LinkedArticleID, Null.NullString);

                    foreach (ImageInfo objImage in objImages)
                    {

                        if (File.Exists(filePathImagesLinked + objImage.FileName))
                        {

                            string finalCopyPath = filePathImagesLinked + objImage.FileName;

                            string filePath = PortalSettings.HomeDirectoryMapPath + folderImages.Replace("/", @"\");

                            if (!Directory.Exists(filePath))
                            {
                                Directory.CreateDirectory(filePath);
                            }

                            if (File.Exists(filePath + objImage.FileName))
                            {
                                for (int i = 1; i <= 100; i++)
                                {
                                    if (!File.Exists(filePath + i.ToString() + "_" + objImage.FileName))
                                    {
                                        objImage.FileName = i.ToString() + "_" + objImage.FileName;
                                        break;
                                    }
                                }
                            }

                            File.Copy(finalCopyPath, filePath + objImage.FileName);

                            objImage.Folder = folderImages;
                            objImage.ArticleID = objArticle.ArticleID;
                            objImageController.Add(objImage);

                        }

                    }

                }

                objArticleController.UpdateArticle(objArticle);
                if (objArticle.Status == StatusType.Published)
                {
                    publishArticle = true;
                }
            }
            else
            {
                objArticleController.UpdateArticle(objArticle);
                if (statusChanged)
                {
                    publishArticle = true;
                }
            }

            SaveCategories(objArticle.ArticleID);
            SaveTags(objArticle.ArticleID);
            SaveDetails(objArticle.ArticleID, objArticle.Title);
            SaveCustomFields(objArticle.ArticleID);

            // Re-init.
            objArticle = objArticleController.GetArticle(objArticle.ArticleID);

            if (publishArticle)
            {
                if (objArticle.IsApproved)
                {
                    if (ArticleSettings.JournalIntegration)
                    {
                        Journal objJournal = new Journal();
                        objJournal.AddArticleToJournal(objArticle, PortalId, TabId, this.UserId, Null.NullInteger, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false));
                    }

                    if (ArticleSettings.JournalIntegrationGroups)
                    {

                        ArrayList objCategories = objArticleController.GetArticleCategories(objArticle.ArticleID);

                        if (objCategories.Count > 0)
                        {

                            RoleController objRoleController = new RoleController();

                            IList<RoleInfo> objRoles = objRoleController.GetRoles(PortalId);
                            foreach (RoleInfo objRole in objRoles)
                            {
                                bool roleAccess = false;

                                if (objRole.SecurityMode == SecurityMode.SocialGroup || objRole.SecurityMode == SecurityMode.Both)
                                {

                                    foreach (CategoryInfo objCategory in objCategories)
                                    {

                                        if (!objCategory.InheritSecurity)
                                        {

                                            if (objCategory.CategorySecurityType == CategorySecurityType.Loose)
                                            {
                                                roleAccess = false;
                                                break;
                                            }
                                            else
                                            {
                                                if (Settings.Contains(objCategory.CategoryID.ToString() + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                                                {
                                                    if (IsInRole(objRole.RoleName, Settings[objCategory.CategoryID.ToString() + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString().Split(';')))
                                                    {
                                                        roleAccess = true;
                                                    }
                                                }
                                            }

                                        }

                                    }

                                }

                                if (roleAccess)
                                {
                                    Journal objJournal = new Journal();
                                    objJournal.AddArticleToJournal(objArticle, PortalId, TabId, this.UserId, objRole.RoleID, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false));
                                }

                            }

                        }

                    }

                    // Notify Smart Thinker

                    if (ArticleSettings.EnableSmartThinkerStoryFeed)
                    {
                        //Dim objStoryFeed = new wsStoryFeed.StoryFeedWS
                        //objStoryFeed.Url = AddHTTP(Request.ServerVariables("HTTP_HOST") + this.ResolveUrl("~/DesktopModules/Smart-Thinker%20-%20UserProfile/StoryFeed.asmx"))

                        string val = GetSharedResource("StoryFeed-AddArticle");

                        val = val.Replace("[AUTHOR]", objArticle.AuthorDisplayName);
                        val = val.Replace("[AUTHORID]", objArticle.AuthorID.ToString());
                        val = val.Replace("[ARTICLELINK]", Common.GetArticleLink(objArticle, this.PortalSettings.ActiveTab, ArticleSettings, false));
                        val = val.Replace("[ARTICLETITLE]", objArticle.Title);

                        try
                        {
                            //objStoryFeed.AddAction(80, _articleID, val, objArticle.AuthorID, "VE6457624576460436531768");
                        }
                        catch
                        {

                        }

                    }

                    if (ArticleSettings.EnableActiveSocialFeed)
                    {
                        if (ArticleSettings.ActiveSocialSubmitKey != "")
                        {
                            if (File.Exists(HttpContext.Current.Server.MapPath(@"~/bin/active.modules.social.dll")))
                            {
                                object ai = null;
                                System.Reflection.Assembly asm;
                                object ac = null;
                                try
                                {
                                    asm = System.Reflection.Assembly.Load("Active.Modules.Social");
                                    ac = asm.CreateInstance("Active.Modules.Social.API.Journal");
                                    if (ac != null)
                                    {
                                        //ac.AddProfileItem(new Guid(ArticleSettings.ActiveSocialSubmitKey), objArticle.AuthorID, Common.GetArticleLink(objArticle, this.PortalSettings.ActiveTab, ArticleSettings, false), objArticle.Title, objArticle.Summary, objArticle.Body, 1, "");
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                    }

                }
            }

            if (_articleID != Null.NullInteger && !statusChanged)
            {

                if (objArticle.Status == StatusType.Published)
                {
                    //Check to see if any articles have been linked

                    EmailTemplateController objEmailTemplateController = new EmailTemplateController();

                    MirrorArticleController objMirrorArticleController = new MirrorArticleController();
                    ArrayList objMirrorArticleLinked = objMirrorArticleController.GetMirrorArticleList(_articleID);

                    EventLogController objEventLog = new DotNetNuke.Services.Log.EventLog.EventLogController();
                    objEventLog.AddLog("Article Linked Update", objMirrorArticleLinked.Count.ToString(), PortalSettings, -1, DotNetNuke.Services.Log.EventLog.EventLogController.EventLogType.ADMIN_ALERT);

                    if (objMirrorArticleLinked.Count > 0)
                    {

                        foreach (MirrorArticleInfo objMirrorArticleInfo in objMirrorArticleLinked)
                        {

                            ArticleInfo objArticleMirrored = objArticleController.GetArticle(objMirrorArticleInfo.ArticleID);

                            if (objArticleMirrored != null)
                            {

                                if (objArticleMirrored.AuthorEmail != "")
                                {
                                    objEmailTemplateController.SendFormattedEmail(this.ModuleId, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false), objArticle, EmailTemplateType.ArticleUpdateMirrored, objArticleMirrored.AuthorEmail, ArticleSettings);
                                }

                                if (objMirrorArticleInfo.AutoUpdate)
                                {

                                    objArticleMirrored.Title = objArticle.Title;
                                    objArticleMirrored.Summary = objArticle.Summary;

                                    objArticleMirrored.Url = objArticle.Url;
                                    objArticleMirrored.IsNewWindow = objArticle.IsNewWindow;
                                    objArticleMirrored.ImageUrl = objArticle.ImageUrl;

                                    objArticleMirrored.MetaTitle = objArticle.MetaTitle;
                                    objArticleMirrored.MetaDescription = objArticle.MetaDescription;
                                    objArticleMirrored.MetaKeywords = objArticle.MetaKeywords;
                                    objArticleMirrored.PageHeadText = objArticle.PageHeadText;

                                    // Save Custom Fields
                                    Hashtable fieldsToUpdate = new Hashtable();

                                    CustomValueController objCustomValueController = new CustomValueController();
                                    List<CustomValueInfo> objCustomValues = objCustomValueController.List(objArticleMirrored.ArticleID);

                                    foreach (CustomValueInfo objCustomValue in objCustomValues)
                                    {
                                        objCustomValueController.Delete(objArticleMirrored.ArticleID, objCustomValue.CustomFieldID);
                                    }

                                    CustomFieldController objCustomFieldController = new CustomFieldController();
                                    ArrayList objCustomFields = objCustomFieldController.List(ModuleId);
                                    ArrayList objCustomFieldsLinked = objCustomFieldController.List(objArticleMirrored.ModuleID);

                                    foreach (CustomFieldInfo objCustomFieldLinked in objCustomFieldsLinked)
                                    {
                                        foreach (CustomFieldInfo objCustomField in objCustomFields)
                                        {

                                            if (objCustomFieldLinked.Name.ToLower() == objCustomField.Name.ToLower())
                                            {

                                                if (objArticle.CustomList.Contains(objCustomField.CustomFieldID))
                                                {
                                                    fieldsToUpdate.Add(objCustomFieldLinked.CustomFieldID.ToString(), objArticle.CustomList[objCustomField.CustomFieldID].ToString());
                                                }

                                            }

                                        }
                                    }

                                    foreach (string key in fieldsToUpdate.Keys)
                                    {
                                        string val = fieldsToUpdate[key].ToString();
                                        CustomValueInfo objCustomValue = new CustomValueInfo();
                                        objCustomValue.CustomFieldID = Convert.ToInt32(key);
                                        objCustomValue.CustomValue = val;
                                        objCustomValue.ArticleID = objArticleMirrored.ArticleID;
                                        objCustomValueController.Add(objCustomValue);
                                    }

                                    // Details

                                    PageController objPageController = new PageController();
                                    ArrayList currentPages = objPageController.GetPageList(objArticleMirrored.ArticleID);

                                    foreach (PageInfo objPage in currentPages)
                                    {
                                        objPageController.DeletePage(objPage.PageID);
                                    }

                                    ArrayList pages = objPageController.GetPageList(objArticle.ArticleID);

                                    foreach (PageInfo objPage in pages)
                                    {
                                        objPage.ArticleID = objArticleMirrored.ArticleID;
                                        objPage.PageID = Null.NullInteger;
                                        objPageController.AddPage(objPage);
                                    }

                                    // Save Tags

                                    TagController objTagController = new TagController();
                                    objTagController.DeleteArticleTag(objArticleMirrored.ArticleID);

                                    string tagsCurrent = objArticle.Tags;

                                    if (tagsCurrent != "")
                                    {
                                        string[] tags = tagsCurrent.Split(',');
                                        foreach (string tag in tags)
                                        {
                                            if (tag != "")
                                            {
                                                TagInfo objTag = objTagController.Get(objArticleMirrored.ModuleID, tag);

                                                if (objTag == null)
                                                {
                                                    objTag = new TagInfo();
                                                    objTag.Name = tag;
                                                    objTag.NameLowered = tag.ToLower();
                                                    objTag.ModuleID = objArticleMirrored.ModuleID;
                                                    objTag.TagID = objTagController.Add(objTag);
                                                }

                                                objTagController.Add(objArticleMirrored.ArticleID, objTag.TagID);
                                            }
                                        }
                                    }

                                    //Copy Files
                                    string folderLinked = "";

                                    ModuleController objModuleController = new ModuleController();
                                    Hashtable objSettingsLinked =objModuleController.GetModule(objArticleMirrored.ModuleID).ModuleSettings;

                                    if (objSettingsLinked.ContainsKey(ArticleConstants.DEFAULT_FILES_FOLDER_SETTING))
                                    {
                                        if (Numeric.IsNumeric(objSettingsLinked[ArticleConstants.DEFAULT_FILES_FOLDER_SETTING].ToString()))
                                        {
                                            int folderID = Convert.ToInt32(objSettingsLinked[ArticleConstants.DEFAULT_FILES_FOLDER_SETTING]);
                                            FolderInfo objFolder = (FolderInfo)FolderManager.Instance.GetFolder(folderID);
                                            if (objFolder != null)
                                            {
                                                folderLinked = objFolder.FolderPath;
                                            }
                                        }
                                    }

                                    PortalController objPortalController = new PortalController();
                                    PortalInfo objPortalLinked = objPortalController.GetPortal(objMirrorArticleInfo.PortalID);

                                    string filePathLinked = objPortalLinked.HomeDirectoryMapPath + folderLinked.Replace("/", @"\");

                                    string folder = "";

                                    if (ModuleConfiguration.ModuleSettings.ContainsKey(ArticleConstants.DEFAULT_FILES_FOLDER_SETTING))
                                    {
                                        if (Numeric.IsNumeric(ModuleConfiguration.ModuleSettings[ArticleConstants.DEFAULT_FILES_FOLDER_SETTING].ToString()))
                                        {
                                            int folderID = Convert.ToInt32(ModuleConfiguration.ModuleSettings[ArticleConstants.DEFAULT_FILES_FOLDER_SETTING]);
                                            FolderInfo objFolder = (FolderInfo)FolderManager.Instance.GetFolder(folderID);
                                            if (objFolder != null)
                                            {
                                                folder = objFolder.FolderPath;
                                            }
                                        }
                                    }

                                    FileController objFileController = new FileController();
                                    List<FileInfo> objFilesCurrent = objFileController.GetFileList(objArticleMirrored.ArticleID, Null.NullString);

                                    foreach (FileInfo objFile in objFilesCurrent)
                                    {
                                        objFileController.Delete(objFile.FileID);
                                    }

                                    List<FileInfo> objFiles = objFileController.GetFileList(objArticle.ArticleID, Null.NullString);

                                    foreach (FileInfo objFile in objFiles)
                                    {

                                        string finalCopyPath = PortalSettings.HomeDirectoryMapPath + folder.Replace("/", @"\") + objFile.FileName;
                                        string filePath = objPortalLinked.HomeDirectoryMapPath + folderLinked.Replace("/", @"\") + objFile.FileName;

                                        if (File.Exists(finalCopyPath))
                                        {

                                            if (!Directory.Exists(objPortalLinked.HomeDirectoryMapPath + folderLinked.Replace("/", @"\")))
                                            {
                                                Directory.CreateDirectory(objPortalLinked.HomeDirectoryMapPath + folderLinked.Replace("/", @"\"));
                                            }

                                            if (!File.Exists(filePath))
                                            {
                                                File.Copy(finalCopyPath, filePath);
                                            }

                                            objFile.FileID = Null.NullInteger;
                                            objFile.ArticleID = objArticleMirrored.ArticleID;
                                            objFileController.Add(objFile);

                                        }

                                    }

                                    //Copy Images

                                    string folderImagesLinked = "";

                                    if (objSettingsLinked.ContainsKey(ArticleConstants.DEFAULT_IMAGES_FOLDER_SETTING))
                                    {
                                        if (Numeric.IsNumeric(objSettingsLinked[ArticleConstants.DEFAULT_IMAGES_FOLDER_SETTING].ToString()))
                                        {
                                            int folderID = Convert.ToInt32(objSettingsLinked[ArticleConstants.DEFAULT_IMAGES_FOLDER_SETTING]);
                                            FolderInfo objFolder = (FolderInfo)FolderManager.Instance.GetFolder(folderID);
                                            if (objFolder != null)
                                            {
                                                folderImagesLinked = objFolder.FolderPath;
                                            }
                                        }
                                    }

                                    string filePathImagesLinked = objPortalLinked.HomeDirectoryMapPath + folderImagesLinked.Replace("/", @"\");

                                    string folderImages = "";

                                    if (ModuleConfiguration.ModuleSettings.ContainsKey(ArticleConstants.DEFAULT_IMAGES_FOLDER_SETTING))
                                    {
                                        if (Numeric.IsNumeric(ModuleConfiguration.ModuleSettings[ArticleConstants.DEFAULT_IMAGES_FOLDER_SETTING].ToString()))
                                        {
                                            int folderID = Convert.ToInt32(ModuleConfiguration.ModuleSettings[ArticleConstants.DEFAULT_IMAGES_FOLDER_SETTING]);
                                            FolderInfo objFolder = (FolderInfo)FolderManager.Instance.GetFolder(folderID);
                                            if (objFolder != null)
                                            {
                                                folderImages = objFolder.FolderPath;
                                            }
                                        }
                                    }

                                    ImageController objImageController = new ImageController();
                                    List<ImageInfo> objImagesCurrent = objImageController.GetImageList(objArticleMirrored.ArticleID, Null.NullString);

                                    foreach (ImageInfo objImage in objImagesCurrent)
                                    {
                                        objImageController.Delete(objImage.ImageID, _articleID, objImage.ImageGuid);
                                    }

                                    List<ImageInfo> objImages = objImageController.GetImageList(objArticle.ArticleID, Null.NullString);

                                    foreach (ImageInfo objImage in objImages)
                                    {

                                        string finalCopyPath = PortalSettings.HomeDirectoryMapPath + folderImages.Replace("/", @"\") + objImage.FileName;
                                        string filePath = objPortalLinked.HomeDirectoryMapPath + folderImagesLinked.Replace("/", @"\") + objImage.FileName;

                                        if (File.Exists(finalCopyPath))
                                        {

                                            if (!Directory.Exists(objPortalLinked.HomeDirectoryMapPath + folderImages.Replace("/", @"\")))
                                            {
                                                Directory.CreateDirectory(objPortalLinked.HomeDirectoryMapPath + folderImages.Replace("/", @"\"));
                                            }

                                            if (!File.Exists(filePath))
                                            {
                                                File.Copy(finalCopyPath, filePath);
                                            }

                                            objImage.Folder = folderImages;
                                            objImage.ImageID = Null.NullInteger;
                                            objImage.ArticleID = objArticleMirrored.ArticleID;
                                            objImageController.Add(objImage);

                                        }

                                    }


                                    // Save

                                    objArticleController.UpdateArticle(objArticleMirrored);

                                }


                            }

                        }

                    }

                }

            }

            if (statusChanged)
            {
                Common.NotifyAuthor(objArticle, this.Settings, this.ModuleId, PortalSettings.ActiveTab, this.PortalId, ArticleSettings);
            }

            ArticleController.ClearArticleCache(objArticle.ArticleID);

            return objArticle.ArticleID;

        }

        private void SaveCategories(int articleID)
        {

            ArticleController objArticleController = new ArticleController();

            if (chkMirrorArticle.Checked && drpMirrorArticle.Items.Count > 0)
            {
                ArrayList objCategories = objArticleController.GetArticleCategories(Convert.ToInt32(drpMirrorArticle.SelectedValue));

                foreach (CategoryInfo objCategory in objCategories)
                {
                    foreach (ListItem item in lstCategories.Items)
                    {
                        if (objCategory.Name.ToLower() == item.Text.TrimStart('.').ToLower())
                        {
                            item.Selected = true;
                        }
                    }
                }
            }

            foreach (ListItem item in lstCategories.Items)
            {
                if (item.Selected)
                {
                    objArticleController.AddArticleCategory(articleID, Int32.Parse(item.Value));
                }
            }

            DataCache.RemoveCache(ArticleConstants.CACHE_CATEGORY_ARTICLE + articleID.ToString());
            DataCache.RemoveCache(ArticleConstants.CACHE_CATEGORY_ARTICLE_NO_LINK + articleID.ToString());

        }

        private void SaveTags(int articleID)
        {

            if (chkMirrorArticle.Checked && drpMirrorArticle.Items.Count > 0)
            {
                ArticleController objArticleController = new ArticleController();
                ArticleInfo objLinkedArticle = objArticleController.GetArticle(Convert.ToInt32(drpMirrorArticle.SelectedValue));

                if (objLinkedArticle != null)
                {
                    txtTags.Text = objLinkedArticle.Tags;
                }
            }

            TagController objTagController = new TagController();
            objTagController.DeleteArticleTag(articleID);

            if (txtTags.Text != "")
            {
                string[] tags = txtTags.Text.Split(',');
                foreach (string tag in tags)
                {
                    if (tag != "")
                    {
                        TagInfo objTag = objTagController.Get(ModuleId, tag);

                        if (objTag == null)
                        {
                            objTag = new TagInfo();
                            objTag.Name = tag;
                            objTag.NameLowered = tag.ToLower();
                            objTag.ModuleID = ModuleId;
                            objTag.TagID = objTagController.Add(objTag);
                        }

                        objTagController.Add(articleID, objTag.TagID);
                    }
                }
            }

        }

        private void SaveDetails(int articleID, string title)
        {

            if (chkMirrorArticle.Checked && drpMirrorArticle.Items.Count > 0)
            {
                PageController objPageController = new PageController();
                ArrayList pages = objPageController.GetPageList(Convert.ToInt32(drpMirrorArticle.SelectedValue));

                foreach (PageInfo objPage in pages)
                {
                    objPage.ArticleID = articleID;
                    objPageController.AddPage(objPage);
                }
            }
            else
            {
                bool doUpdate = true;
                if (phMirrorText.Visible)
                {
                    MirrorArticleController objMirrorArticleController = new MirrorArticleController();
                    MirrorArticleInfo objMirrorArticleInfo = objMirrorArticleController.GetMirrorArticle(_articleID);

                    if (objMirrorArticleInfo != null)
                    {

                        if (objMirrorArticleInfo.AutoUpdate)
                        {
                            doUpdate = false;
                        }
                    }
                }

                if (doUpdate)
                {
                    if (Details.Text.Trim() != "")
                    {
                        PageController objPageController = new PageController();
                        ArrayList pages = objPageController.GetPageList(articleID);

                        if (pages.Count > 0)
                        {
                            PageInfo objPage = (PageInfo)pages[0];
                            objPage.PageText = Details.Text;
                            objPageController.UpdatePage(objPage);
                        }
                        else
                        {
                            PageInfo objPage = new PageInfo();
                            objPage.PageText = Details.Text;
                            objPage.ArticleID = articleID;
                            objPage.Title = txtTitle.Text;
                            objPageController.AddPage(objPage);
                        }
                    }
                    else
                    {
                        PageController objPageController = new PageController();
                        ArrayList pages = objPageController.GetPageList(articleID);

                        if (pages.Count == 1)
                        {
                            objPageController.DeletePage(((PageInfo)pages[0]).PageID);
                        }
                    }
                }
            }

        }

        private void SaveCustomFields(int articleID)
        {

            Hashtable fieldsToUpdate = new Hashtable();

            CustomFieldController objCustomFieldController = new CustomFieldController();
            ArrayList objCustomFields = objCustomFieldController.List(this.ModuleId);

            if (phCustomFields.Visible)
            {


                foreach (RepeaterItem item in rptCustomFields.Items)
                {
                    PlaceHolder phValue = (PlaceHolder)item.FindControl("phValue");

                    if (phValue != null)
                    {
                        if (phValue.Controls.Count > 0)
                        {

                            System.Web.UI.Control objControl = phValue.Controls[0];
                            int customFieldID = Convert.ToInt32(objControl.ID);

                            foreach (CustomFieldInfo objCustomField in objCustomFields)
                            {
                                if (objCustomField.CustomFieldID == customFieldID)
                                {
                                    switch (objCustomField.FieldType)
                                    {

                                        case CustomFieldType.OneLineTextBox:
                                            TextBox objTextBox = (TextBox)objControl;
                                            fieldsToUpdate.Add(customFieldID.ToString(), objTextBox.Text);
                                            break;
                                        case CustomFieldType.MultiLineTextBox:
                                            TextBox objTextBox2 = (TextBox)objControl;
                                            fieldsToUpdate.Add(customFieldID.ToString(), objTextBox2.Text);
                                            break;
                                        case CustomFieldType.RichTextBox:
                                            TextEditor objTextBox1 = (TextEditor)objControl;
                                            fieldsToUpdate.Add(customFieldID.ToString(), objTextBox1.Text);
                                            break;
                                        case CustomFieldType.DropDownList:
                                            DropDownList objDropDownList = (DropDownList)objControl;
                                            if (objDropDownList.SelectedValue == "-1")
                                            {
                                                fieldsToUpdate.Add(customFieldID.ToString(), "");
                                            }
                                            else
                                            {
                                                fieldsToUpdate.Add(customFieldID.ToString(), objDropDownList.SelectedValue);
                                            }
                                            break;
                                        case CustomFieldType.CheckBox:
                                            CheckBox objCheckBox = (CheckBox)objControl;
                                            fieldsToUpdate.Add(customFieldID.ToString(), objCheckBox.Checked.ToString());
                                            break;
                                        case CustomFieldType.MultiCheckBox:
                                            CheckBoxList objCheckBoxList = (CheckBoxList)objControl;
                                            string values = "";
                                            foreach (ListItem objCheckBox1 in objCheckBoxList.Items)
                                            {
                                                if (objCheckBox1.Selected)
                                                {
                                                    if (values == "")
                                                    {
                                                        values = objCheckBox1.Value;
                                                    }
                                                    else
                                                    {
                                                        values = values + "|" + objCheckBox1.Value;
                                                    }
                                                }
                                            }
                                            fieldsToUpdate.Add(customFieldID.ToString(), values);
                                            break;
                                        case CustomFieldType.RadioButton:
                                            RadioButtonList objRadioButtonList = (RadioButtonList)objControl;
                                            fieldsToUpdate.Add(customFieldID.ToString(), objRadioButtonList.SelectedValue);
                                            break;
                                        case CustomFieldType.ColorPicker:
                                            TextBox objTextBox3 = (TextBox)objControl;
                                            fieldsToUpdate.Add(customFieldID.ToString(), objTextBox3.Text);
                                            break;
                                    }

                                    break;
                                }
                            }

                        }
                    }
                }

            }

            if (chkMirrorArticle.Checked && drpMirrorArticle.Items.Count > 0)
            {

                ArticleController objArticleController = new ArticleController();
                ArticleInfo objLinkedArticle = objArticleController.GetArticle(Convert.ToInt32(drpMirrorArticle.SelectedValue));

                if (objLinkedArticle != null)
                {

                    ArrayList objCustomFieldsLinked = objCustomFieldController.List(Convert.ToInt32(drpMirrorModule.SelectedValue.Split('-')[1]));

                    foreach (CustomFieldInfo objCustomFieldLinked in objCustomFieldsLinked)
                    {
                        foreach (CustomFieldInfo objCustomField in objCustomFields)
                        {

                            if (objCustomFieldLinked.Name.ToLower() == objCustomField.Name.ToLower())
                            {

                                if (objLinkedArticle.CustomList.Contains(objCustomFieldLinked.CustomFieldID))
                                {
                                    fieldsToUpdate.Add(objCustomField.CustomFieldID.ToString(), objLinkedArticle.CustomList[objCustomField.CustomFieldID].ToString());
                                }

                            }

                        }
                    }

                }

            }

            foreach (string key in fieldsToUpdate.Keys)
            {
                string val = fieldsToUpdate[key].ToString();

                CustomValueController objCustomValueController = new CustomValueController();
                CustomValueInfo objCustomValue = objCustomValueController.GetByCustomField(articleID, Convert.ToInt32(key));

                if (objCustomValue != null)
                {
                    objCustomValueController.Delete(articleID, Convert.ToInt32(key));
                }

                objCustomValue = new CustomValueInfo();
                objCustomValue.CustomFieldID = Convert.ToInt32(key);
                objCustomValue.CustomValue = val;
                objCustomValue.ArticleID = articleID;
                objCustomValueController.Add(objCustomValue);
            }

        }

        private void SetVisibility()
        {

            trCategories.Visible = ArticleSettings.IsCategoriesEnabled;

            if (lstCategories.Items.Count == 0 || !trCategories.Visible)
            {
                TagController objTagController = new TagController();
                ArrayList objTags = objTagController.List(this.ModuleId, 10);
                if (objTags.Count == 0)
                {
                    phOrganize.Visible = false;
                }
            }

            if (lstCategories.Items.Count == 0)
            {
                trCategories.Visible = false;
            }

            phExcerpt.Visible = ArticleSettings.IsExcerptEnabled;
            phMeta.Visible = ArticleSettings.IsMetaEnabled;
            if (phCustomFields.Visible)
            {
                phCustomFields.Visible = ArticleSettings.IsCustomEnabled;
            }

            trLink.Visible = ArticleSettings.IsLinkEnabled;
            trNewWindow.Visible = ArticleSettings.IsLinkEnabled;

            phAttachment.Visible = (trLink.Visible || trNewWindow.Visible);

            trFeatured.Visible = ArticleSettings.IsFeaturedEnabled;
            trSecure.Visible = ArticleSettings.IsSecureEnabled;
            trPublish.Visible = ArticleSettings.IsPublishEnabled;
            trExpiry.Visible = ArticleSettings.IsExpiryEnabled;

            phPublish.Visible = (trAuthor.Visible || trFeatured.Visible || trSecure.Visible || trPublish.Visible || trExpiry.Visible);

            drpStatus.Enabled = ArticleSettings.IsApprover;

            if (_articleID != Null.NullInteger)
            {
                ArticleController objArticleController = new ArticleController();
                ArticleInfo objArticle = objArticleController.GetArticle(_articleID);

                if (objArticle != null)
                {
                    switch (objArticle.Status)
                    {
                        case StatusType.Draft:
                            cmdSaveArticle.Visible = true;
                            cmdPublishArticle.Visible = true;
                            cmdAddEditPages.Visible = true;
                            cmdDelete.Visible = true;
                            return;

                        case StatusType.AwaitingApproval:
                            cmdSaveArticle.Visible = true;
                            cmdPublishArticle.Visible = false;
                            cmdAddEditPages.Visible = true;
                            cmdDelete.Visible = true;
                            return;

                        case StatusType.Published:
                            cmdSaveArticle.Visible = true;
                            cmdPublishArticle.Visible = false;
                            cmdAddEditPages.Visible = true;
                            cmdDelete.Visible = true;
                            return;
                    }
                }
            }
            else
            {
                cmdSaveArticle.Visible = true;
                cmdPublishArticle.Visible = true;
                cmdAddEditPages.Visible = true;
                cmdDelete.Visible = false;
            }

        }

        private void SetTextEditor()
        {

            this.txtExcerptBasic.Visible = (ArticleSettings.TextEditorSummaryMode == Components.Types.TextEditorModeType.Basic);
            this.txtExcerptRich.Visible = (ArticleSettings.TextEditorSummaryMode == Components.Types.TextEditorModeType.Rich);

            //dshExcerpt.IsExpanded = ArticleSettings.ExpandSummary
            //dshMeta.IsExpanded = ArticleSettings.ExpandMetaInformation

            txtExcerptBasic.Width = Unit.Parse(ArticleSettings.TextEditorSummaryWidth);
            this.txtExcerptBasic.Height = Unit.Parse(ArticleSettings.TextEditorSummaryHeight);
            this.ExcerptRich.Width = Unit.Parse(ArticleSettings.TextEditorSummaryWidth);
            this.ExcerptRich.Height = Unit.Parse(ArticleSettings.TextEditorSummaryHeight);

            this.Details.Width = Unit.Parse(ArticleSettings.TextEditorWidth);
            this.Details.Height = Unit.Parse(ArticleSettings.TextEditorHeight);

            this.lstCategories.Height = Unit.Parse(ArticleSettings.CategorySelectionHeight.ToString());

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Init += new EventHandler(this.Page_Init);
            this.Load += new EventHandler(this.Page_Load);
            this.PreRender += new EventHandler(Page_PreRender);
            rptCustomFields.ItemDataBound += rptCustomFields_OnItemDataBound;
            cmdSaveArticle.Click += cmdSaveArticle_Click;
            cmdPublishArticle.Click += cmdPublishArticle_Click;
            cmdDelete.Click += cmdDelete_Click;
            cmdAddEditPages.Click += cmdAddEditPages_Click;
            cmdSelectAuthor.Click += cmdSelectAuthor_Click;
            cmdCancel.Click += cmdCancel_Click;
            valAuthor.ServerValidate += valAuthor_ServerValidate;
            chkMirrorArticle.CheckedChanged += chkMirrorArticle_CheckedChanged;
            drpMirrorModule.SelectedIndexChanged += drpMirrorModule_SelectedIndexChanged;
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

                string script = "" + "\r\n"
                    + "<script type=\"text/javascript\" src='" + this.ResolveUrl("Includes/ColorPicker/ColorPicker.js") + "'></script>" + "\r\n";

                ClientScriptManager CSM = Page.ClientScript;
                CSM.RegisterClientScriptBlock(this.GetType(), "ColorPicker", script);

                BindCustomFields();

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {

                ModuleController objModuleController = new ModuleController();
                ModuleInfo objModule = objModuleController.GetModule(this.ModuleId, this.TabId);

                if (objModule != null)
                {
                    trAuthor.Visible = (ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EDIT", ModuleConfiguration) || ArticleSettings.IsApprover);
                }

                cmdExpiryCalendar.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtExpiryDate);
                cmdPublishCalendar.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtPublishDate);

                CheckSecurity();
                SetTextEditor();

                if (_articleID != Null.NullInteger)
                {
                    foreach (string key in _richTextValues)
                    {
                        foreach (RepeaterItem item in rptCustomFields.Items)
                        {
                            if (item.FindControl(key) != null)
                            {
                                TextEditor objTextBox = (TextEditor)item.FindControl(key);
                                objTextBox.Text = _richTextValues[key];
                                break;
                            }
                        }
                    }
                }

                if (!IsPostBack)
                {

                    BindStatus();
                    BindCategories();
                    SetVisibility();
                    BindArticle();

                    if (ArticleSettings.ContentSharingPortals == "" || _articleID != Null.NullInteger)
                    {
                        phMirror.Visible = false;
                    }

                    Page.SetFocus(txtTitle);

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

                trNewWindow.Visible = (trLink.Visible && (UrlLink.UrlType != "N"));

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void rptCustomFields_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                ArticleController objArticleController = new ArticleController();
                ArticleInfo objArticle = null;
                if (_articleID != Null.NullInteger)
                {
                    objArticle = objArticleController.GetArticle(_articleID);
                }

                CustomFieldInfo objCustomField = (CustomFieldInfo)e.Item.DataItem;
                PlaceHolder phValue = (PlaceHolder)e.Item.FindControl("phValue");
                PlaceHolder phLabel = (PlaceHolder)e.Item.FindControl("phLabel");

                LinkButton cmdHelp = (LinkButton)e.Item.FindControl("cmdHelp");
                Panel pnlHelp = (Panel)e.Item.FindControl("pnlHelp");
                Label lblLabel = (Label)e.Item.FindControl("lblLabel");
                Label lblHelp = (Label)e.Item.FindControl("lblHelp");
                Image imgHelp = (Image)e.Item.FindControl("imgHelp");

                HtmlTableRow trItem = (HtmlTableRow)e.Item.FindControl("trItem");

                if (phValue != null)
                {

                    DotNetNuke.UI.Utilities.DNNClientAPI.EnableMinMax(cmdHelp, pnlHelp, true, DotNetNuke.UI.Utilities.DNNClientAPI.MinMaxPersistanceType.None);

                    if (objCustomField.IsRequired)
                    {
                        lblLabel.Text = objCustomField.Caption + "*:";
                    }
                    else
                    {
                        lblLabel.Text = objCustomField.Caption + ":";
                    }
                    lblHelp.Text = objCustomField.CaptionHelp;
                    imgHelp.AlternateText = objCustomField.CaptionHelp;

                    switch (objCustomField.FieldType)
                    {

                        case CustomFieldType.OneLineTextBox:

                            TextBox objTextBox = new TextBox();
                            objTextBox.CssClass = "NormalTextBox";
                            objTextBox.ID = objCustomField.CustomFieldID.ToString();
                            if (objCustomField.Length != Null.NullInteger && objCustomField.Length > 0)
                            {
                                objTextBox.MaxLength = objCustomField.Length;
                            }
                            if (objCustomField.DefaultValue != "")
                            {
                                objTextBox.Text = objCustomField.DefaultValue;
                            }
                            if (objArticle != null)
                            {
                                if (objArticle.CustomList.Contains(objCustomField.CustomFieldID) && (Page.IsPostBack == false || objTextBox.Enabled == false))
                                {
                                    objTextBox.Text = objArticle.CustomList[objCustomField.CustomFieldID].ToString();
                                }
                            }
                            objTextBox.Width = Unit.Pixel(300);
                            phValue.Controls.Add(objTextBox);

                            if (objCustomField.IsRequired)
                            {
                                RequiredFieldValidator valRequired = new RequiredFieldValidator();
                                valRequired.ControlToValidate = objTextBox.ID;
                                valRequired.ErrorMessage = Localization.GetString("valFieldRequired", this.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name);
                                valRequired.CssClass = "NormalRed";
                                valRequired.Display = ValidatorDisplay.None;
                                valRequired.SetFocusOnError = true;
                                phValue.Controls.Add(valRequired);
                            }

                            if (objCustomField.ValidationType != CustomFieldValidationType.None)
                            {
                                CompareValidator valCompare = new CompareValidator();
                                valCompare.ControlToValidate = objTextBox.ID;
                                valCompare.CssClass = "NormalRed";
                                valCompare.Display = ValidatorDisplay.None;
                                valCompare.SetFocusOnError = true;
                                switch (objCustomField.ValidationType)
                                {

                                    case CustomFieldValidationType.Currency:
                                        valCompare.Type = ValidationDataType.Double;
                                        valCompare.Operator = ValidationCompareOperator.DataTypeCheck;
                                        valCompare.ErrorMessage = Localization.GetString("valFieldCurrency", this.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name);
                                        phValue.Controls.Add(valCompare);
                                        break;
                                    case CustomFieldValidationType.Date:
                                        valCompare.Type = ValidationDataType.Date;
                                        valCompare.Operator = ValidationCompareOperator.DataTypeCheck;
                                        valCompare.ErrorMessage = Localization.GetString("valFieldDate", this.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name);
                                        phValue.Controls.Add(valCompare);

                                        HyperLink objCalendar = new HyperLink();
                                        objCalendar.CssClass = "CommandButton";
                                        objCalendar.Text = Localization.GetString("Calendar", this.LocalResourceFile);
                                        objCalendar.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(objTextBox);
                                        phValue.Controls.Add(objCalendar);
                                        break;
                                    case CustomFieldValidationType.Double:
                                        valCompare.Type = ValidationDataType.Double;
                                        valCompare.Operator = ValidationCompareOperator.DataTypeCheck;
                                        valCompare.ErrorMessage = Localization.GetString("valFieldDecimal", this.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name);
                                        phValue.Controls.Add(valCompare);
                                        break;
                                    case CustomFieldValidationType.Integer:
                                        valCompare.Type = ValidationDataType.Integer;
                                        valCompare.Operator = ValidationCompareOperator.DataTypeCheck;
                                        valCompare.ErrorMessage = Localization.GetString("valFieldNumber", this.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name);
                                        phValue.Controls.Add(valCompare);
                                        break;
                                    case CustomFieldValidationType.Email:
                                        RegularExpressionValidator valRegular = new RegularExpressionValidator();
                                        valRegular.ControlToValidate = objTextBox.ID;
                                        valRegular.ValidationExpression = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
                                        valRegular.ErrorMessage = Localization.GetString("valFieldEmail", this.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name);
                                        valRegular.CssClass = "NormalRed";
                                        valRegular.Display = ValidatorDisplay.None;
                                        phValue.Controls.Add(valRegular);
                                        break;
                                    case CustomFieldValidationType.Regex:
                                        if (objCustomField.RegularExpression != "")
                                        {
                                            RegularExpressionValidator valRegular1 = new RegularExpressionValidator();
                                            valRegular1.ControlToValidate = objTextBox.ID;
                                            valRegular1.ValidationExpression = objCustomField.RegularExpression;
                                            valRegular1.ErrorMessage = Localization.GetString("valFieldRegex", this.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name);
                                            valRegular1.CssClass = "NormalRed";
                                            valRegular1.Display = ValidatorDisplay.None;
                                            phValue.Controls.Add(valRegular1);
                                        }
                                        break;
                                }
                            }
                            break;
                        case CustomFieldType.MultiLineTextBox:

                            TextBox objTextBox1 = new TextBox();
                            objTextBox1.TextMode = TextBoxMode.MultiLine;
                            objTextBox1.CssClass = "NormalTextBox";
                            objTextBox1.ID = objCustomField.CustomFieldID.ToString();
                            objTextBox1.Rows = 4;
                            if (objCustomField.Length != Null.NullInteger && objCustomField.Length > 0)
                            {
                                objTextBox1.MaxLength = objCustomField.Length;
                            }
                            if (objCustomField.DefaultValue != "")
                            {
                                objTextBox1.Text = objCustomField.DefaultValue;
                            }
                            if (objArticle != null)
                            {
                                if (objArticle.CustomList.Contains(objCustomField.CustomFieldID) && (Page.IsPostBack == false || objTextBox1.Enabled == false))
                                {
                                    objTextBox1.Text = objArticle.CustomList[objCustomField.CustomFieldID].ToString();
                                }
                            }
                            objTextBox1.Width = Unit.Pixel(300);
                            phValue.Controls.Add(objTextBox1);

                            if (objCustomField.IsRequired)
                            {
                                RequiredFieldValidator valRequired = new RequiredFieldValidator();
                                valRequired.ControlToValidate = objTextBox1.ID;
                                valRequired.ErrorMessage = Localization.GetString("valFieldRequired", this.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name);
                                valRequired.CssClass = "NormalRed";
                                valRequired.Display = ValidatorDisplay.None;
                                valRequired.SetFocusOnError = true;
                                phValue.Controls.Add(valRequired);
                            }
                            break;
                        case CustomFieldType.RichTextBox:

                            TextEditor objTextBox2 = (TextEditor)this.LoadControl("~/controls/TextEditor.ascx");
                            objTextBox2.ID = objCustomField.CustomFieldID.ToString();
                            if (objCustomField.DefaultValue != "")
                            {
                                // objTextBox2.Text = objCustomField.DefaultValue
                            }
                            if (objArticle != null)
                            {
                                if (objArticle.CustomList.Contains(objCustomField.CustomFieldID) && Page.IsPostBack == false)
                                {
                                    // There is a problem assigned values at init with the RTE, using ArrayList to assign later.
                                    _richTextValues.Add(objCustomField.CustomFieldID.ToString(), objArticle.CustomList[objCustomField.CustomFieldID].ToString());
                                }
                            }
                            objTextBox2.Width = Unit.Pixel(300);
                            objTextBox2.Height = Unit.Pixel(400);

                            phValue.Controls.Add(objTextBox2);

                            if (objCustomField.IsRequired)
                            {
                                RequiredFieldValidator valRequired = new RequiredFieldValidator();
                                valRequired.ControlToValidate = objTextBox2.ID;
                                valRequired.ErrorMessage = Localization.GetString("valFieldRequired", this.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name);
                                valRequired.CssClass = "NormalRed";
                                valRequired.SetFocusOnError = true;
                                phValue.Controls.Add(valRequired);
                            }
                            break;
                        case CustomFieldType.DropDownList:

                            DropDownList objDropDownList = new DropDownList();
                            objDropDownList.CssClass = "NormalTextBox";
                            objDropDownList.ID = objCustomField.CustomFieldID.ToString();

                            string[] values = objCustomField.FieldElements.Split('|');
                            foreach (string value in values)
                            {
                                if (value != "")
                                {
                                    objDropDownList.Items.Add(value);
                                }
                            }

                            string selectText = Localization.GetString("SelectValue", this.LocalResourceFile);
                            selectText = selectText.Replace("[VALUE]", objCustomField.Caption);
                            objDropDownList.Items.Insert(0, new ListItem(selectText, "-1"));

                            if (objCustomField.DefaultValue != "")
                            {
                                if (objDropDownList.Items.FindByValue(objCustomField.DefaultValue) != null)
                                {
                                    objDropDownList.SelectedValue = objCustomField.DefaultValue;
                                }
                            }

                            objDropDownList.Width = Unit.Pixel(300);

                            if (objArticle != null)
                            {
                                if (objArticle.CustomList.Contains(objCustomField.CustomFieldID) && (Page.IsPostBack == false || objDropDownList.Enabled == false))
                                {
                                    if (objDropDownList.Items.FindByValue(objArticle.CustomList[objCustomField.CustomFieldID].ToString()) != null)
                                    {
                                        objDropDownList.SelectedValue = objArticle.CustomList[objCustomField.CustomFieldID].ToString();
                                    }
                                }
                            }
                            phValue.Controls.Add(objDropDownList);

                            if (objCustomField.IsRequired)
                            {
                                RequiredFieldValidator valRequired = new RequiredFieldValidator();
                                valRequired.ControlToValidate = objDropDownList.ID;
                                valRequired.ErrorMessage = Localization.GetString("valFieldRequired", this.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name);
                                valRequired.CssClass = "NormalRed";
                                valRequired.Display = ValidatorDisplay.None;
                                valRequired.SetFocusOnError = true;
                                valRequired.InitialValue = "-1";
                                phValue.Controls.Add(valRequired);
                            }
                            break;
                        case CustomFieldType.CheckBox:

                            CheckBox objCheckBox = new CheckBox();
                            objCheckBox.CssClass = "Normal";
                            objCheckBox.ID = objCustomField.CustomFieldID.ToString();
                            if (objCustomField.DefaultValue != "")
                            {
                                try
                                {
                                    objCheckBox.Checked = Convert.ToBoolean(objCustomField.DefaultValue);
                                }
                                catch
                                { }
                            }

                            if (objArticle != null)
                            {
                                if (objArticle.CustomList.Contains(objCustomField.CustomFieldID) && (Page.IsPostBack == false || objCheckBox.Enabled == false))
                                {
                                    if (objArticle.CustomList[objCustomField.CustomFieldID].ToString() != "")
                                    {
                                        try
                                        {
                                            objCheckBox.Checked = Convert.ToBoolean(objArticle.CustomList[objCustomField.CustomFieldID].ToString());
                                        }
                                        catch
                                        { }
                                    }
                                }
                            }
                            phValue.Controls.Add(objCheckBox);
                            break;
                        case CustomFieldType.MultiCheckBox:

                            CheckBoxList objCheckBoxList = new CheckBoxList();
                            objCheckBoxList.CssClass = "Normal";
                            objCheckBoxList.ID = objCustomField.CustomFieldID.ToString();
                            objCheckBoxList.RepeatColumns = 4;
                            objCheckBoxList.RepeatDirection = RepeatDirection.Horizontal;
                            objCheckBoxList.RepeatLayout = RepeatLayout.Table;

                            string[] values1 = objCustomField.FieldElements.Split('|');
                            foreach (string value in values1)
                            {
                                objCheckBoxList.Items.Add(value);
                            }

                            if (objCustomField.DefaultValue != "")
                            {
                                string[] vals = objCustomField.DefaultValue.Split('|');
                                foreach (string val in vals)
                                {
                                    foreach (ListItem item in objCheckBoxList.Items)
                                    {
                                        if (item.Value == val)
                                        {
                                            item.Selected = true;
                                        }
                                    }
                                }
                            }

                            if (objArticle != null)
                            {
                                if (objArticle.CustomList.Contains(objCustomField.CustomFieldID) && (Page.IsPostBack == false || objCheckBoxList.Enabled == false))
                                {
                                    string[] vals = objArticle.CustomList[objCustomField.CustomFieldID].ToString().Split('|');
                                    foreach (string val in vals)
                                    {
                                        foreach (ListItem item in objCheckBoxList.Items)
                                        {
                                            if (item.Value == val)
                                            {
                                                item.Selected = true;
                                            }
                                        }
                                    }
                                }
                            }

                            phValue.Controls.Add(objCheckBoxList);
                            break;
                        case CustomFieldType.RadioButton:

                            RadioButtonList objRadioButtonList = new RadioButtonList();
                            objRadioButtonList.CssClass = "Normal";
                            objRadioButtonList.ID = objCustomField.CustomFieldID.ToString();
                            objRadioButtonList.RepeatDirection = RepeatDirection.Horizontal;
                            objRadioButtonList.RepeatLayout = RepeatLayout.Table;
                            objRadioButtonList.RepeatColumns = 4;

                            string[] values2 = objCustomField.FieldElements.Split('|');
                            foreach (string value in values2)
                            {
                                objRadioButtonList.Items.Add(value);
                            }

                            if (objCustomField.DefaultValue != "")
                            {
                                if (objRadioButtonList.Items.FindByValue(objCustomField.DefaultValue) != null)
                                {
                                    objRadioButtonList.SelectedValue = objCustomField.DefaultValue;
                                }
                            }

                            if (objArticle != null)
                            {
                                if (objArticle.CustomList.Contains(objCustomField.CustomFieldID) && (Page.IsPostBack == false || objRadioButtonList.Enabled == false))
                                {
                                    if (objRadioButtonList.Items.FindByValue(objArticle.CustomList[objCustomField.CustomFieldID].ToString()) != null)
                                    {
                                        objRadioButtonList.SelectedValue = objArticle.CustomList[objCustomField.CustomFieldID].ToString();
                                    }
                                }
                            }

                            phValue.Controls.Add(objRadioButtonList);

                            if (objCustomField.IsRequired)
                            {
                                RequiredFieldValidator valRequired = new RequiredFieldValidator();
                                valRequired.ControlToValidate = objRadioButtonList.ID;
                                valRequired.ErrorMessage = Localization.GetString("valFieldRequired", this.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name);
                                valRequired.CssClass = "NormalRed";
                                valRequired.Display = ValidatorDisplay.None;
                                valRequired.SetFocusOnError = true;
                                phValue.Controls.Add(valRequired);
                            }
                            break;
                        case CustomFieldType.ColorPicker:

                            TextBox objTextBox3 = new TextBox();
                            objTextBox3.CssClass = "NormalTextBox";
                            objTextBox3.ID = objCustomField.CustomFieldID.ToString();
                            if (objCustomField.Length != Null.NullInteger && objCustomField.Length > 0)
                            {
                                objTextBox3.MaxLength = objCustomField.Length;
                            }
                            if (objCustomField.DefaultValue != "")
                            {
                                objTextBox3.Text = objCustomField.DefaultValue;
                            }
                            if (objArticle != null)
                            {
                                if (objArticle.CustomList.Contains(objCustomField.CustomFieldID) && (Page.IsPostBack == false || objTextBox3.Enabled == false))
                                {
                                    objTextBox3.Text = objArticle.CustomList[objCustomField.CustomFieldID].ToString();
                                }
                            }
                            phValue.Controls.Add(objTextBox3);

                            if (objCustomField.IsRequired)
                            {
                                RequiredFieldValidator valRequired = new RequiredFieldValidator();
                                valRequired.ControlToValidate = objTextBox3.ID;
                                valRequired.ErrorMessage = Localization.GetString("valFieldRequired", this.LocalResourceFile).Replace("[CUSTOMFIELD]", objCustomField.Name);
                                valRequired.CssClass = "NormalRed";
                                valRequired.Display = ValidatorDisplay.None;
                                valRequired.SetFocusOnError = true;
                                phValue.Controls.Add(valRequired);
                            }

                            string script = ""
                                + "<script type=\"text/javascript\" charset=\"utf-8\">"
                                + "jQuery(function($)"
                                + "{"
                                + "     $(\"#" + objTextBox3.ClientID + "\").attachColorPicker();"
                                + "});"
                                + "</script>";

                            ClientScriptManager CSM = Page.ClientScript;
                            CSM.RegisterClientScriptBlock(this.GetType(), "Picker" + objTextBox3.ID, script);
                            break;
                    }

                }

            }

        }

        private void cmdSaveArticle_Click(object sender, EventArgs e)
        {

            try
            {

                if (Page.IsValid)
                {
                    StatusType objStatusType = (StatusType)System.Enum.Parse(typeof(StatusType), drpStatus.SelectedValue);
                    int articleID = SaveArticle(objStatusType);

                    if (objStatusType == StatusType.Draft)
                    {
                        Response.Redirect(Common.GetModuleLink(this.TabId, this.ModuleId, "MyArticles", ArticleSettings), true);
                    }

                    if (objStatusType == StatusType.AwaitingApproval)
                    {

                    }

                    if (objStatusType == StatusType.Published)
                    {
                        ArticleController objArticleController = new ArticleController();
                        ArticleInfo objArticle = objArticleController.GetArticle(articleID);

                        if (objArticle != null)
                        {
                            if (_returnUrl != null)
                            {
                                Response.Redirect(_returnUrl, true);
                            }
                            else
                            {
                                Response.Redirect(Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false), true);
                            }
                        }
                    }

                    if (_returnUrl != "")
                    {
                        Response.Redirect(_returnUrl, true);
                    }
                    else
                    {
                        Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "", ArticleSettings), true);
                    }
                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdPublishArticle_Click(object sender, EventArgs e)
        {

            try
            {

                if (Page.IsValid)
                {
                    if (ArticleSettings.IsApprover || ArticleSettings.IsAutoApprover)
                    {
                        int articleID = SaveArticle(StatusType.Published);
                        ArticleController objArticleController = new ArticleController();
                        ArticleInfo objArticle = objArticleController.GetArticle(articleID);

                        if (objArticle != null)
                        {
                            Response.Redirect(Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false), true);
                        }
                        else
                        {
                            Response.Redirect(Globals.NavigateURL(), true);
                        }
                    }
                    else
                    {
                        int articleID = SaveArticle(StatusType.AwaitingApproval);

                        ArticleController objArticleController = new ArticleController();
                        ArticleInfo objArticle = objArticleController.GetArticle(articleID);

                        if (objArticle != null)
                        {
                            if (Settings.Contains(ArticleConstants.NOTIFY_SUBMISSION_SETTING))
                            {
                                if (Convert.ToBoolean(Settings[ArticleConstants.NOTIFY_SUBMISSION_SETTING]))
                                {
                                    EmailTemplateController objEmailTemplateController = new EmailTemplateController();
                                    string emails = objEmailTemplateController.GetApproverDistributionList(ModuleId);

                                    foreach (string email in emails.Split(';'))
                                    {
                                        if (email != "")
                                        {
                                            objEmailTemplateController.SendFormattedEmail(this.ModuleId, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false), objArticle, EmailTemplateType.ArticleSubmission, email, ArticleSettings);
                                        }
                                    }
                                }
                            }

                            if (Settings.Contains(ArticleConstants.NOTIFY_SUBMISSION_SETTING_EMAIL))
                            {
                                if (Settings[ArticleConstants.NOTIFY_SUBMISSION_SETTING_EMAIL].ToString() != "")
                                {
                                    EmailTemplateController objEmailTemplateController = new EmailTemplateController();
                                    foreach (string email in Settings[ArticleConstants.NOTIFY_SUBMISSION_SETTING_EMAIL].ToString().Split(','))
                                    {
                                        objEmailTemplateController.SendFormattedEmail(this.ModuleId, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false), objArticle, EmailTemplateType.ArticleSubmission, email, ArticleSettings);
                                    }
                                }
                            }
                        }

                        Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "SubmitNewsComplete", ArticleSettings), true);
                    }
                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {

            try
            {

                ArticleController objArticleController = new ArticleController();
                objArticleController.DeleteArticle(_articleID, ModuleId);
                if (_returnUrl != "")
                {
                    Response.Redirect(_returnUrl, true);
                }
                else
                {
                    Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "", ArticleSettings), true);
                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdAddEditPages_Click(object sender, EventArgs e)
        {

            try
            {

                if (Page.IsValid)
                {
                    int articleID = SaveArticle();
                    Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "EditPages", ArticleSettings, "ArticleID=" + articleID.ToString()), true);
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

                cmdSelectAuthor.Visible = false;
                lblAuthor.Visible = false;

                if (ArticleSettings.AuthorSelect == Components.Types.AuthorSelectType.ByDropdown)
                {
                    PopulateAuthorList();
                    ddlAuthor.Visible = true;

                    if (_articleID != Null.NullInteger)
                    {

                        ArticleController objArticleController = new ArticleController();
                        ArticleInfo objArticle = objArticleController.GetArticle(_articleID);

                        if (objArticle != null)
                        {
                            if (ddlAuthor.Items.FindByValue(objArticle.AuthorID.ToString()) != null)
                            {
                                ddlAuthor.SelectedValue = objArticle.AuthorID.ToString();
                            }
                        }
                    }
                    else
                    {
                        if (ArticleSettings.AuthorDefault != Null.NullInteger)
                        {
                            if (ddlAuthor.Items.FindByValue(ArticleSettings.AuthorDefault.ToString()) != null)
                            {
                                ddlAuthor.SelectedValue = ArticleSettings.AuthorDefault.ToString();
                            }
                        }
                    }
                }
                else
                {
                    pnlAuthor.Visible = true;
                    txtAuthor.Text = lblAuthor.Text;
                    txtAuthor.Focus();
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
                if (_returnUrl != null)//_returnUrl != null
                {
                    Response.Redirect(_returnUrl, true);
                }
                else
                {
                    Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "", ArticleSettings), true);
                }
            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void valAuthor_ServerValidate(object source, ServerValidateEventArgs args)
        {

            try
            {

                args.IsValid = false;

                if (txtAuthor.Text != "")
                {
                    UserInfo objUser = UserController.GetUserByName(this.PortalId, txtAuthor.Text);

                    if (objUser != null)
                    {
                        args.IsValid = true;
                    }
                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void chkMirrorArticle_CheckedChanged(object sender, EventArgs e)
        {

            try
            {

                phCreate.Visible = !chkMirrorArticle.Checked;
                trMirrorModule.Visible = chkMirrorArticle.Checked;
                trMirrorArticle.Visible = chkMirrorArticle.Checked;
                trMirrorAutoUpdate.Visible = chkMirrorArticle.Checked;

                if (chkMirrorArticle.Checked)
                {

                    drpMirrorModule.Items.Clear();
                    drpMirrorModule.DataSource = GetContentSharingPortals(ArticleSettings.ContentSharingPortals);
                    drpMirrorModule.DataBind();

                    if (drpMirrorModule.Items.Count > 0)
                    {

                        ArticleController objArticleController = new ArticleController();
                        drpMirrorArticle.DataSource = objArticleController.GetArticleList(Convert.ToInt32(drpMirrorModule.SelectedValue.Split('-')[1]), true, "StartDate");
                        drpMirrorArticle.DataBind();

                    }

                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void drpMirrorModule_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {

                ArticleController objArticleController = new ArticleController();
                drpMirrorArticle.DataSource = objArticleController.GetArticleList(Convert.ToInt32(drpMirrorModule.SelectedValue.Split('-')[1]), true, "StartDate");
                drpMirrorArticle.DataBind();

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
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

                    TabController objTabController = new TabController();
                    TabInfo objTab = objTabController.GetTab(objContentSharing.LinkedTabID, objContentSharing.LinkedPortalID, false);

                    if (objTab != null)
                    {
                        objContentSharing.TabTitle = objTab.TabName;
                    }

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
    }
}