using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Security;

using GcDesign.NewsArticles.Base;
using System.Collections;
using DotNetNuke.Security.Permissions;

namespace GcDesign.NewsArticles.Controls
{
    public partial class UploadFiles : NewsArticleControlBase
    {
        #region " private Members "

        private int _articleID = Null.NullInteger;

        private bool _filesInit = false;
        private List<FileInfo> _objFiles;

        #endregion

        #region " private Properties "

        private NewsArticleModuleBase ArticleModuleBase
        {
            get
            {
                return (NewsArticleModuleBase)Parent.Parent.Parent.Parent.Parent;
            }
        }

        private ArticleSettings ArticleSettings
        {
            get
            {
                return ArticleModuleBase.ArticleSettings;
            }
        }

        #endregion

        #region " public Properties "

        public int ArticleGuid
        {
            get
            {
                if (_articleID == Null.NullInteger)
                {
                    if (litArticleGuid.Text == Null.NullString)
                    {
                        litArticleGuid.Text = (GetRandom(1, 100000) * -1).ToString();
                    }
                    return Convert.ToInt32(litArticleGuid.Text);
                }
                return _articleID;
            }
            set
            {
                litArticleGuid.Text = value.ToString();
            }
        }

        public List<FileInfo> AttachedFiles
        {
            get
            {

                if (!_filesInit)
                {
                    //_objFiles = objFileController.GetFileList(_articleID, ArticleGuid)
                    if (_articleID == Null.NullInteger)
                    {
                        _objFiles = FileProvider.Instance().GetFiles(ArticleGuid);
                    }
                    else
                    {
                        _objFiles = FileProvider.Instance().GetFiles(_articleID);
                    }
                    _filesInit = true;
                }

                return _objFiles;

            }
        }

        #endregion

        #region " private Methods "

        private void BindFiles()
        {

            dlFiles.DataSource = AttachedFiles;
            dlFiles.DataBind();

            dlFiles.Visible = (dlFiles.Items.Count > 0);
            lblNoFiles.Visible = (dlFiles.Items.Count == 0);

        }

        private void BindFolders()
        {

            string ReadRoles = Null.NullString;
            string WriteRoles = Null.NullString;

            drpUploadFilesFolder.Items.Clear();

            var folderManager = FolderManager.Instance;
            IEnumerable<IFolderInfo> folders = folderManager.GetFolders(ArticleModuleBase.PortalId);
            foreach (FolderInfo folder in folders)
            {
                ListItem FolderItem = new ListItem();
                if (folder.FolderPath == Null.NullString)
                {
                    FolderItem.Text = ArticleModuleBase.GetSharedResource("Root");
                    ReadRoles = FolderPermissionController.GetFolderPermissionsCollectionByFolder(ArticleModuleBase.PortalId, "").ToString("READ");//FileSystemUtils.GetRoles("", ArticleModuleBase.PortalId, "READ");
                    WriteRoles = FileSystemUtils.GetRoles("", ArticleModuleBase.PortalId, "WRITE");
                }
                else
                {
                    FolderItem.Text = folder.FolderPath;
                    ReadRoles = FileSystemUtils.GetRoles(FolderItem.Text, ArticleModuleBase.PortalId, "READ");
                    WriteRoles = FileSystemUtils.GetRoles(FolderItem.Text, ArticleModuleBase.PortalId, "WRITE");
                }
                FolderItem.Value = folder.FolderID.ToString();

                if (PortalSecurity.IsInRoles(ReadRoles) || PortalSecurity.IsInRoles(WriteRoles))
                {
                    drpUploadFilesFolder.Items.Add(FolderItem);
                }
            }

            if (drpUploadFilesFolder.Items.FindByValue(ArticleSettings.DefaultFilesFolder.ToString()) != null)
            {
                drpUploadFilesFolder.SelectedValue = ArticleSettings.DefaultFilesFolder.ToString();
            }

        }

        protected string GetArticleID()
        {

            return _articleID.ToString();

        }

        protected string GetMaximumFileSize()
        {

            return "20480";

        }

        protected string GetPostBackReference()
        {

            return Page.ClientScript.GetPostBackEventReference(cmdRefreshFiles, "Refresh");

        }

        private int GetRandom(int Min, int Max)
        {
            Random Generator = new Random();
            return Generator.Next(Min, Max);
        }

        public string GetResourceKey(string key)
        {

            string path = @"~/DesktopModules/GcDesign-NewsArticles/App_LocalResources/UploadFiles.ascx.resx";
            return DotNetNuke.Services.Localization.Localization.GetString(key, path);

        }

        protected string GetUploadUrl()
        {

            string link = Page.ResolveUrl(@"~/DesktopModules/GcDesign%20-%20Searches/Controls/SWFUploaderFiles.ashx?PortalID=" + ArticleModuleBase.PortalId.ToString());

            if (link.ToLower().StartsWith("http"))
            {
                return link;
            }
            else
            {
                if (Request.Url.Port == 80)
                {
                    return DotNetNuke.Common.Globals.AddHTTP(Request.Url.Host + link);
                }
                else
                {
                    return DotNetNuke.Common.Globals.AddHTTP(Request.Url.Host + ":" + Request.Url.Port.ToString() + link);
                }
            }

        }

        private void ReadQueryString()
        {

            if (ArticleSettings.UrlModeType == Components.Types.UrlModeType.Shorterned)
            {
                try
                {
                    if (Numeric.IsNumeric(Request[ArticleSettings.ShortenedID]))
                    {
                        _articleID = Convert.ToInt32(Request[ArticleSettings.ShortenedID]);
                    }
                }
                catch
                { }
            }

            if (Numeric.IsNumeric(Request["ArticleID"]))
            {
                _articleID = Convert.ToInt32(Request["ArticleID"]);
            }

        }


        private void SetLocalization()
        {

            dshFiles.Text = GetResourceKey("Files");
            lblFilesHelp.Text = GetResourceKey("FilesHelp");

            dshExistingFiles.Text = GetResourceKey("SelectExisting");
            dshUploadFiles.Text = GetResourceKey("UploadFiles");
            dshSelectedFiles.Text = GetResourceKey("SelectedFiles");

            lblNoFiles.Text = GetResourceKey("NoFiles");

            cmdAddExistingFile.Text = GetResourceKey("cmdAddExistingFile");

        }

        #endregion

        #region " public Methods "

        public void UpdateFiles(int articleID)
        {

            foreach (FileInfo objFile in AttachedFiles)
            {
                objFile.ArticleID = articleID;
                FileProvider.Instance().UpdateFile(objFile);
                // objFileController.Update(objFile)
            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            this.PreRender += new EventHandler(Page_PreRender);
            cmdRefreshFiles.Click += cmdRefreshFiles_Click;
            dlFiles.ItemDataBound += dlFiles_OnItemDataBound;
            dlFiles.ItemCommand += dlFiles_OnItemCommand;
            cmdAddExistingFile.Click += cmdAddExistingFile_Click;
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            cmdRefreshFiles.Click += cmdRefreshFiles_Click;
            try
            {

                ReadQueryString();
                SetLocalization();

                //trUpload.Visible = ArticleSettings.EnableImagesUpload
                //trExisting.Visible = ArticleSettings.EnablePortalImages

                //phFiles.Visible = (trUpload.Visible Or trExisting.Visible)

                if (!ArticleSettings.IsFilesEnabled)
                {
                    this.Visible = false;
                    return;
                }

                if (!IsPostBack)
                {

                    lblSelectFiles.Text = GetResourceKey("SelectFiles");
                    litModuleID.Text = this.ArticleModuleBase.ModuleId.ToString();
                    litTabModuleID.Text = this.ArticleModuleBase.TabModuleId.ToString();

                    if (Request.IsAuthenticated)
                    {
                        litTicketID.Text = Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName].Value;
                    }
                    litArticleGuid.Text = ArticleGuid.ToString();

                    BindFolders();
                    BindFiles();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            try
            {

            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void cmdRefreshFiles_Click(object sender, EventArgs e)
        {

            try
            {

                BindFiles();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }


        private void dlFiles_OnItemDataBound(object sender, DataListItemEventArgs e)
        {

            try
            {

                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {

                    FileInfo objFile = (FileInfo)e.Item.DataItem;

                    ImageButton btnEdit = (ImageButton)e.Item.FindControl("btnEdit");
                    ImageButton btnDelete = (ImageButton)e.Item.FindControl("btnDelete");
                    ImageButton btnUp = (ImageButton)e.Item.FindControl("btnUp");
                    ImageButton btnDown = (ImageButton)e.Item.FindControl("btnDown");

                    if (btnDelete != null)
                    {
                        btnDelete.Attributes.Add("onClick", "javascript:return confirm('" + GetResourceKey("DeleteFile") + "');");

                        if (objFile != null)
                        {
                            btnDelete.CommandArgument = objFile.FileID.ToString();
                        }

                    }

                    if (btnEdit != null)
                    {

                        if (objFile != null)
                        {
                            btnEdit.CommandArgument = objFile.FileID.ToString();
                        }

                    }

                    if (btnUp != null && btnDown != null)
                    {

                        if (objFile.FileID == ((FileInfo)AttachedFiles[0]).FileID)
                        {
                            btnUp.Visible = false;
                        }

                        if (objFile.FileID == ((FileInfo)AttachedFiles[AttachedFiles.Count - 1]).FileID)
                        {
                            btnDown.Visible = false;
                        }

                        btnUp.CommandArgument = objFile.FileID.ToString();
                        btnUp.CommandName = "Up";
                        btnUp.CausesValidation = false;

                        btnDown.CommandArgument = objFile.FileID.ToString();
                        btnDown.CommandName = "Down";
                        btnDown.CausesValidation = false;

                    }

                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void dlFiles_OnItemCommand(object sender, DataListCommandEventArgs e)
        {

            try
            {

                if (e.CommandName == "Delete")
                {

                    FileInfo objFile = FileProvider.Instance().GetFile(Convert.ToInt32(e.CommandArgument));

                    if (objFile != null)
                    {
                        //'Dim objPortalSettings As PortalSettings = PortalController.Instance.GetCurrentPortalSettings()
                        //'Dim filePath As String = objPortalSettings.HomeDirectoryMapPath & objFile.Folder & objFile.FileName
                        //'if (File.Exists(filePath)) {
                        //'    File.Delete(filePath)
                        //'}
                        FileProvider.Instance().DeleteFile(_articleID, Convert.ToInt32(e.CommandArgument));
                        // objFileController.Delete(Convert.ToInt32(e.CommandArgument))
                    }

                }

                if (e.CommandName == "Edit")
                {

                    dlFiles.EditItemIndex = e.Item.ItemIndex;

                }

                if (e.CommandName == "Up")
                {

                    int fileID = Convert.ToInt32(e.CommandArgument);

                    for (int i = 0; i < AttachedFiles.Count; i++)
                    {
                        FileInfo objFile = (FileInfo)AttachedFiles[i];
                        if (fileID == objFile.FileID)
                        {

                            FileInfo objFileToSwap = (FileInfo)AttachedFiles[i - 1];

                            int sortOrder = objFile.SortOrder;
                            int sortOrderPrevious = objFileToSwap.SortOrder;

                            objFile.SortOrder = sortOrderPrevious;
                            objFileToSwap.SortOrder = sortOrder;

                            FileProvider.Instance().UpdateFile(objFile);
                            FileProvider.Instance().UpdateFile(objFileToSwap);

                        }
                    }

                }

                if (e.CommandName == "Down")
                {

                    int fileID = Convert.ToInt32(e.CommandArgument);

                    for (int i = 0; i < AttachedFiles.Count; i++)
                    {
                        FileInfo objFile = (FileInfo)AttachedFiles[i];
                        if (fileID == objFile.FileID)
                        {
                            FileInfo objFileToSwap = (FileInfo)AttachedFiles[i + 1];

                            int sortOrder = objFile.SortOrder;
                            int sortOrderNext = objFileToSwap.SortOrder;

                            objFile.SortOrder = sortOrderNext;
                            objFileToSwap.SortOrder = sortOrder;

                            FileProvider.Instance().UpdateFile(objFile);
                            FileProvider.Instance().UpdateFile(objFileToSwap);
                        }
                    }

                }

                if (e.CommandName == "Cancel")
                {

                    dlFiles.EditItemIndex = -1;

                }

                if (e.CommandName == "Update")
                {

                    TextBox txtTitle = (TextBox)e.Item.FindControl("txtTitle");

                    FileInfo objFile = FileProvider.Instance().GetFile(Convert.ToInt32(dlFiles.DataKeys[e.Item.ItemIndex]));

                    if (objFile != null)
                    {
                        objFile.Title = txtTitle.Text;
                        FileProvider.Instance().UpdateFile(objFile);
                    }

                    dlFiles.EditItemIndex = -1;

                }

                _filesInit = false;
                BindFiles();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdAddExistingFile_Click(object sender, EventArgs e)
        {

            try
            {

                if (ctlFile.Url != "")
                {
                    if (ctlFile.Url.ToLower().StartsWith("fileid="))
                    {
                        if (Numeric.IsNumeric(ctlFile.Url.ToLower().Replace("fileid=", "")))
                        {
                            int fileID = Convert.ToInt32(ctlFile.Url.ToLower().Replace("fileid=", ""));
                            DotNetNuke.Services.FileSystem.FileController objDnnFileController = new DotNetNuke.Services.FileSystem.FileController();
                            DotNetNuke.Services.FileSystem.FileInfo objDnnFile = objDnnFileController.GetFileById(fileID, ArticleModuleBase.PortalId);
                            if (objDnnFile != null)
                            {

                                FileController objFileController = new FileController();

                                FileInfo objFile = new FileInfo();

                                objFile.ArticleID = _articleID;
                                if (_articleID == Null.NullInteger)
                                {
                                    objFile.ArticleID = ArticleGuid;
                                }
                                objFile.FileName = objDnnFile.FileName;
                                objFile.ContentType = objDnnFile.ContentType;
                                objFile.SortOrder = 0;
                                List<FileInfo> filesList = objFileController.GetFileList(_articleID, ArticleGuid.ToString());
                                if (filesList.Count > 0)
                                {
                                    objFile.SortOrder = ((FileInfo)filesList[filesList.Count - 1]).SortOrder + 1;
                                }
                                objFile.Folder = objDnnFile.Folder;
                                objFile.Extension = objDnnFile.Extension;
                                objFile.Size = objDnnFile.Size;
                                objFile.Title = objFile.FileName.Replace("." + objFile.Extension, "");

                                objFileController.Add(objFile);
                                BindFiles();

                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}