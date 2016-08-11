using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Security;
using GcDesign.NewsArticles.Base;
using System.Collections;

namespace GcDesign.NewsArticles.Controls
{
    public partial class UploadImages : NewsArticleControlBase
    {
        #region " private Members "

        private int _articleID = Null.NullInteger;

        private bool _imagesInit = false;
        private List<ImageInfo> _objImages;

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

        public string ArticleGuid
        {
            get
            {
                if (_articleID == Null.NullInteger)
                {
                    if (litArticleGuid.Text == Null.NullString)
                    {
                        litArticleGuid.Text = Guid.NewGuid().ToString();
                    }
                }
                return litArticleGuid.Text;
            }
            set
            {
                litArticleGuid.Text = value;
            }
        }

        public List<ImageInfo> AttachedImages
        {
            get
            {

                if (!_imagesInit)
                {
                    ImageController objImageController = new ImageController();
                    _objImages = objImageController.GetImageList(_articleID, ArticleGuid);
                    _imagesInit = true;
                }

                return _objImages;

            }
        }

        public string ImageExternalUrl
        {
            get
            {
                return txtImageExternal.Text;
            }
            set
            {
                txtImageExternal.Text = value;
            }
        }

        #endregion

        #region " private Methods "

        private void BindFolders()
        {

            string ReadRoles = Null.NullString;
            string WriteRoles = Null.NullString;

            drpUploadImageFolder.Items.Clear();

            ArrayList folders = FileSystemUtils.GetFolders(ArticleModuleBase.PortalId);
            foreach (FolderInfo folder in folders)
            {
                ListItem FolderItem = new ListItem();
                if (folder.FolderPath == Null.NullString)
                {
                    FolderItem.Text = ArticleModuleBase.GetSharedResource("Root");
                    ReadRoles = FileSystemUtils.GetRoles("", ArticleModuleBase.PortalId, "READ");
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
                    drpUploadImageFolder.Items.Add(FolderItem);
                }
            }

            if (drpUploadImageFolder.Items.FindByValue(ArticleSettings.DefaultImagesFolder.ToString()) != null)
            {
                drpUploadImageFolder.SelectedValue = ArticleSettings.DefaultImagesFolder.ToString();
            }

        }

        private void BindImages()
        {

            ImageController objImageController = new ImageController();

            dlImages.DataSource = AttachedImages;
            dlImages.DataBind();

            dlImages.Visible = (dlImages.Items.Count > 0);
            lblNoImages.Visible = (dlImages.Items.Count == 0);

        }

        protected string GetArticleID()
        {

            return _articleID.ToString();

        }

        protected string GetImageUrl(ImageInfo objImage)
        {

            int thumbWidth = 150;
            int thumbHeight = 150;

            int width;
            if (objImage.Width > thumbWidth)
            {
                width = thumbWidth;
            }
            else
            {
                width = objImage.Width;
            }

            int height = Convert.ToInt32(objImage.Height / (objImage.Width / width));
            if (height > thumbHeight)
            {
                height = thumbHeight;
                width = Convert.ToInt32(objImage.Width / (objImage.Height / height));
            }

            PortalSettings settings = PortalController.Instance.GetCurrentPortalSettings();

            return Page.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(settings.HomeDirectory) + "&FileName=" + Server.UrlEncode(objImage.Folder + objImage.FileName) + "&PortalID=" + settings.PortalId.ToString() + "&q=1");

        }

        protected string GetMaximumFileSize()
        {

            return "20480";

        }

        protected string GetPostBackReference()
        {

            return Page.ClientScript.GetPostBackEventReference(cmdRefreshPhotos, "Refresh");

        }

        public string GetResourceKey(string key)
        {

            string path = "~/DesktopModules/GcDesign-NewsArticles/App_LocalResources/UploadImages.ascx.resx";
            return DotNetNuke.Services.Localization.Localization.GetString(key, path);

        }

        protected string GetUploadUrl()
        {

            string link = Page.ResolveUrl(@"~/DesktopModules/GcDesign%20-%20Searches/Controls/SWFUploader.ashx?PortalID=" + ArticleModuleBase.PortalId.ToString());

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

            dshImages.Text = GetResourceKey("Images");
            lblImagesHelp.Text = GetResourceKey("ImagesHelp");

            dshExistingImages.Text = GetResourceKey("SelectExisting");
            dshUploadImages.Text = GetResourceKey("UploadImages");
            dshSelectedImages.Text = GetResourceKey("SelectedImages");
            dshExternalImage.Text = GetResourceKey("ExternalImage");

            lblNoImages.Text = GetResourceKey("NoImages");

        }

        #endregion

        #region " public Methods "

        public void UpdateImages(int articleID)
        {

            ImageController objImageController = new ImageController();
            foreach (ImageInfo objImage in AttachedImages)
            {
                objImage.ArticleID = articleID;
                objImageController.Update(objImage);
            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            this.PreRender += new EventHandler(Page_PreRender);
            cmdRefreshPhotos.Click += cmdRefreshPhotos_Click;
            dlImages.ItemDataBound += dlImages_OnItemDataBound;
            dlImages.ItemCommand += dlImages_OnItemCommand;
            cmdAddExistingImage.Click += cmdAddExistingImage_Click;
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            cmdRefreshPhotos.Click += cmdRefreshPhotos_Click;

            try
            {

                ReadQueryString();
                SetLocalization();

                trUpload.Visible = ArticleSettings.EnableImagesUpload;
                trExisting.Visible = ArticleSettings.EnablePortalImages;

                phImages.Visible = (trUpload.Visible || trExisting.Visible);

                phExternalImage.Visible = ArticleSettings.EnableExternalImages;

                if (!ArticleSettings.IsImagesEnabled || (!trUpload.Visible && !trExisting.Visible && !phExternalImage.Visible))
                {
                    this.Visible = false;
                    return;
                }

                if (!IsPostBack)
                {
                    lblSelectImages.Text = GetResourceKey("SelectImages");
                    litModuleID.Text = this.ArticleModuleBase.ModuleId.ToString();
                    litTabModuleID.Text = this.ArticleModuleBase.TabModuleId.ToString();

                    if (Request.IsAuthenticated)
                    {
                        litTicketID.Text = Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName].Value;
                    }
                    litArticleGuid.Text = ArticleGuid.ToString();

                    BindFolders();
                    BindImages();
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

        protected void cmdRefreshPhotos_Click(object sender, EventArgs e)
        {

            try
            {

                BindImages();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }


        private void dlImages_OnItemDataBound(object sender, DataListItemEventArgs e)
        {

            try
            {

                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {

                    ImageInfo objImage = (ImageInfo)e.Item.DataItem;

                    ImageButton btnEdit = (ImageButton)e.Item.FindControl("btnEdit");
                    ImageButton btnDelete = (ImageButton)e.Item.FindControl("btnDelete");
                    ImageButton btnTop = (ImageButton)e.Item.FindControl("btnTop");
                    ImageButton btnUp = (ImageButton)e.Item.FindControl("btnUp");
                    ImageButton btnDown = (ImageButton)e.Item.FindControl("btnDown");
                    ImageButton btnBottom = (ImageButton)e.Item.FindControl("btnBottom");

                    if (btnDelete != null)
                    {
                        btnDelete.Attributes.Add("onClick", "javascript:return confirm('" + GetResourceKey("DeleteImage") + "');");

                        if (objImage != null)
                        {
                            btnDelete.CommandArgument = objImage.ImageID.ToString();
                        }

                    }

                    if (btnEdit != null)
                    {

                        if (objImage != null)
                        {
                            btnEdit.CommandArgument = objImage.ImageID.ToString();
                        }

                    }

                    if (btnUp != null && btnDown != null)
                    {

                        if (objImage.ImageID == ((ImageInfo)AttachedImages[0]).ImageID)
                        {
                            btnUp.Visible = false;
                            btnTop.Visible = false;
                        }

                        if (objImage.ImageID == ((ImageInfo)AttachedImages[AttachedImages.Count - 1]).ImageID)
                        {
                            btnDown.Visible = false;
                            btnBottom.Visible = false;
                        }

                        btnTop.CommandArgument = objImage.ImageID.ToString();
                        btnTop.CommandName = "Top";
                        btnTop.CausesValidation = false;

                        btnUp.CommandArgument = objImage.ImageID.ToString();
                        btnUp.CommandName = "Up";
                        btnUp.CausesValidation = false;

                        btnDown.CommandArgument = objImage.ImageID.ToString();
                        btnDown.CommandName = "Down";
                        btnDown.CausesValidation = false;

                        btnBottom.CommandArgument = objImage.ImageID.ToString();
                        btnBottom.CommandName = "Bottom";
                        btnBottom.CausesValidation = false;

                    }

                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }


        }

        private void dlImages_OnItemCommand(object sender, DataListCommandEventArgs e)
        {

            try
            {

                ImageController objImageController = new ImageController();

                if (e.CommandName == "Delete")
                {

                    ImageInfo objImage = objImageController.Get(Convert.ToInt32(e.CommandArgument));

                    if (objImage != null)
                    {
                        objImageController.Delete(Convert.ToInt32(e.CommandArgument), _articleID, objImage.ImageGuid);
                    }

                }

                if (e.CommandName == "Edit")
                {

                    dlImages.EditItemIndex = e.Item.ItemIndex;

                }

                if (e.CommandName == "Top")
                {

                    int imageID = Convert.ToInt32(e.CommandArgument);

                    List<ImageInfo> objImagesSorted = new List<ImageInfo>();

                    for (int i = 0; i < AttachedImages.Count; i++)
                    {
                        ImageInfo objImage = (ImageInfo)AttachedImages[i];
                        if (imageID == objImage.ImageID)
                        {
                            objImagesSorted.Insert(0, objImage);
                        }
                        else
                        {
                            objImagesSorted.Add(objImage);
                        }
                    }

                    int sortOrder = 0;
                    foreach (ImageInfo objImage in objImagesSorted)
                    {
                        objImage.SortOrder = sortOrder;
                        objImageController.Update(objImage);
                        sortOrder = sortOrder + 1;
                    }

                }

                if (e.CommandName == "Up")
                {

                    int imageID = Convert.ToInt32(e.CommandArgument);

                    for (int i = 0; i < AttachedImages.Count; i++)
                    {
                        ImageInfo objImage = (ImageInfo)AttachedImages[i];
                        if (imageID == objImage.ImageID)
                        {

                            ImageInfo objImageToSwap = (ImageInfo)AttachedImages[i - 1];

                            int sortOrder = objImage.SortOrder;
                            int sortOrderPrevious = objImageToSwap.SortOrder;

                            objImage.SortOrder = sortOrderPrevious;
                            objImageToSwap.SortOrder = sortOrder;

                            objImageController.Update(objImage);
                            objImageController.Update(objImageToSwap);

                        }
                    }

                }

                if (e.CommandName == "Down")
                {

                    int imageID = Convert.ToInt32(e.CommandArgument);

                    for (int i = 0; i < AttachedImages.Count; i++)
                    {
                        ImageInfo objImage = (ImageInfo)AttachedImages[i];
                        if (imageID == objImage.ImageID)
                        {
                            ImageInfo objImageToSwap = (ImageInfo)AttachedImages[i - 1];

                            int sortOrder = objImage.SortOrder;
                            int sortOrderNext = objImageToSwap.SortOrder;

                            objImage.SortOrder = sortOrderNext;
                            objImageToSwap.SortOrder = sortOrder;

                            objImageController.Update(objImage);
                            objImageController.Update(objImageToSwap);
                        }
                    }

                }

                if (e.CommandName == "Bottom")
                {

                    int imageID = Convert.ToInt32(e.CommandArgument);

                    ImageInfo objImageEnd = null;
                    List<ImageInfo> objImagesSorted = new List<ImageInfo>();

                    for (int i = 0; i < AttachedImages.Count; i++)
                    {
                        ImageInfo objImage = (ImageInfo)AttachedImages[i];
                        if (imageID == objImage.ImageID)
                        {
                            objImageEnd = objImage;
                        }
                        else
                        {
                            objImagesSorted.Add(objImage);
                        }
                    }

                    if (objImageEnd != null)
                    {
                        objImagesSorted.Add(objImageEnd);

                        int sortOrder = 0;
                        foreach (ImageInfo objImage in objImagesSorted)
                        {
                            objImage.SortOrder = sortOrder;
                            objImageController.Update(objImage);
                            sortOrder = sortOrder + 1;
                        }
                    }

                }

                if (e.CommandName == "Cancel")
                {

                    dlImages.EditItemIndex = -1;

                }

                if (e.CommandName == "Update")
                {

                    TextBox txtTitle = (TextBox)e.Item.FindControl("txtTitle");
                    TextBox txtDescription = (TextBox)e.Item.FindControl("txtDescription");

                    ImageInfo objImage = objImageController.Get(Convert.ToInt32(dlImages.DataKeys[e.Item.ItemIndex]));

                    if (objImage != null)
                    {
                        objImage.Title = txtTitle.Text;
                        objImage.Description = txtDescription.Text;
                        objImageController.Update(objImage);
                    }

                    dlImages.EditItemIndex = -1;

                }

                _imagesInit = false;
                BindImages();

            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void cmdAddExistingImage_Click(object sender, EventArgs e)
        {

            try
            {

                if (ctlImage.Url != "")
                {
                    if (ctlImage.Url.ToLower().StartsWith("fileid="))
                    {
                        if (Numeric.IsNumeric(ctlImage.Url.ToLower().Replace("fileid=", "")))
                        {
                            int fileID = Convert.ToInt32(ctlImage.Url.ToLower().Replace("fileid=", ""));
                            DotNetNuke.Services.FileSystem.FileController objDnnFileController = new DotNetNuke.Services.FileSystem.FileController();
                            DotNetNuke.Services.FileSystem.FileInfo objFile = objDnnFileController.GetFileById(fileID, ArticleModuleBase.PortalId);
                            if (objFile != null)
                            {

                                ImageController objImageController = new ImageController();
                                ImageInfo objImage = new ImageInfo();

                                objImage.ArticleID = _articleID;
                                if (_articleID == Null.NullInteger)
                                {
                                    objImage.ImageGuid = ArticleGuid;
                                }
                                objImage.FileName = objFile.FileName;
                                objImage.ContentType = objFile.ContentType;
                                objImage.Width = objFile.Width;
                                objImage.Height = objFile.Height;
                                objImage.SortOrder = 0;
                                List<ImageInfo> imagesList = objImageController.GetImageList(_articleID, ArticleGuid);
                                if (imagesList.Count > 0)
                                {
                                    objImage.SortOrder = ((ImageInfo)imagesList[imagesList.Count - 1]).SortOrder + 1;
                                }
                                objImage.Folder = objFile.Folder;
                                objImage.Extension = objFile.Extension;
                                objImage.Title = objFile.FileName.Replace("." + objImage.Extension, "");
                                objImage.Size = objFile.Size;
                                objImage.Description = "";

                                objImageController.Add(objImage);
                                BindImages();

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