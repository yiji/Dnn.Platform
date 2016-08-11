using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.FileSystem;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections;
using System.Web.Security;
using System.Data;

namespace GcDesign.NewsArticles.Controls
{
    /// <summary>
    /// SWFUploader 的摘要说明
    /// </summary>
    public class SWFUploader : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #region " private Members "

        private int _articleID = Null.NullInteger;
        private int _moduleID = Null.NullInteger;
        private int _tabID = Null.NullInteger;
        private int _tabModuleID = Null.NullInteger;
        private int _portalID = Null.NullInteger;
        private string _ticket = Null.NullString;
        private int _userID = Null.NullInteger;
        private string _imageGuid = Null.NullString;

        private GcDesign.NewsArticles.ArticleSettings _articleSettings;
        private Hashtable _settings;
        private HttpContext _context;

        #endregion

        #region " private Properties "

        private ArticleSettings ArticleSettings
        {
            get
            {
                if (_articleSettings == null)
                {
                    ModuleController objModuleController = new ModuleController();
                    ModuleInfo objModule = objModuleController.GetModule(_moduleID, _tabID);

                    _articleSettings = new ArticleSettings(Settings, PortalController.Instance.GetCurrentPortalSettings(), objModule);
                }
                return _articleSettings;
            }
        }

        private Hashtable Settings
        {
            get
            {
                if (_settings == null)
                {
                    ModuleController objModuleController = new ModuleController();
                    _settings = objModuleController.GetModuleSettings(_moduleID);
                    _settings = GetTabModuleSettings(_tabModuleID, _settings);
                }
                return _settings;
            }
        }

        #endregion

        #region " private Methods "

        private void AuthenticateUserFromTicket()
        {

            if (_ticket != "")
            {

                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(_ticket);
                FormsIdentity fi = new FormsIdentity(ticket);

                string[] roles = null;
                HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(fi, roles);

                UserInfo objUser = UserController.GetUserByName(_portalID, HttpContext.Current.User.Identity.Name);

                if (objUser != null)
                {
                    _userID = objUser.UserID;
                    HttpContext.Current.Items["UserInfo"] = objUser;

                    RoleController objRoleController = new RoleController();
                    roles = objRoleController.GetRolesByUser(_userID, _portalID);

                    string strPortalRoles = string.Join(";", roles);
                    _context.Items.Add("UserRoles", ";" + strPortalRoles + ";");
                }

            }

        }

        private Hashtable GetTabModuleSettings(int TabModuleId, Hashtable settings)
        {

            IDataReader dr = DotNetNuke.Data.DataProvider.Instance().GetTabModuleSettings(TabModuleId);

            while (dr.Read())
            {

                if (!dr.IsDBNull(1))
                {
                    settings[dr.GetString(0)] = dr.GetString(1);
                }
                else
                {
                    settings[dr.GetString(0)] = "";
                }

            }

            dr.Close();

            return settings;

        }

        private void ReadQueryString()
        {

            if (_context.Request["ModuleID"] != null)
            {
                _moduleID = Convert.ToInt32(_context.Request["ModuleID"]);
            }

            if (_context.Request["PortalID"] != null)
            {
                _portalID = Convert.ToInt32(_context.Request["PortalID"]);
            }

            if (_context.Request["ArticleID"] != null)
            {
                _articleID = Convert.ToInt32(_context.Request["ArticleID"]);
            }

            if (_context.Request["TabModuleID"] != null)
            {
                _tabModuleID = Convert.ToInt32(_context.Request["TabModuleID"]);
            }

            if (_context.Request["TabID"] != null)
            {
                _tabID = Convert.ToInt32(_context.Request["TabID"]);
            }

            if (_context.Request["Ticket"] != null)
            {
                _ticket = _context.Request["Ticket"];
            }

            if (_articleID == Null.NullInteger)
            {
                if (_context.Request["ArticleGuid"] != null)
                {
                    _imageGuid = _context.Request["ArticleGuid"];
                }
            }

        }

        #endregion

        #region " Interface Methods "

        void IHttpHandler.ProcessRequest(HttpContext context)
        {

            _context = context;
            context.Response.ContentType = "text/plain";

            ReadQueryString();
            AuthenticateUserFromTicket();

            if (!_context.Request.IsAuthenticated)
            {
                _context.Response.Write("-2");
                _context.Response.End();
            }

            ImageController objImageController = new ImageController();
            HttpPostedFile objFile = _context.Request.Files["Filedata"];

            if (objFile != null)
            {

                PortalController objPortalController = new PortalController();
                if (!objPortalController.HasSpaceAvailable(_portalID, objFile.ContentLength))
                {
                    _context.Response.Write("-1");
                    _context.Response.End();
                }

                string username = _context.User.Identity.Name;

                ImageInfo objImage = new ImageInfo();

                objImage.ArticleID = _articleID;
                if (_articleID == Null.NullInteger)
                {
                    objImage.ImageGuid = _imageGuid;
                }
                objImage.FileName = objFile.FileName;

                if (objFile.FileName.ToLower().EndsWith(".jpg"))
                {
                    objImage.ContentType = "image/jpeg";
                }

                if (objFile.FileName.ToLower().EndsWith(".gif"))
                {
                    objImage.ContentType = "image/gif";
                }

                if (objFile.FileName.ToLower().EndsWith(".png"))
                {
                    objImage.ContentType = "image/png";
                }

                int maxWidth = ArticleSettings.MaxImageWidth;
                int maxHeight = ArticleSettings.MaxImageHeight;

                System.Drawing.Image photo = System.Drawing.Image.FromStream(objFile.InputStream);

                objImage.Width = photo.Width;
                objImage.Height = photo.Height;

                if (objImage.Width > maxWidth)
                {
                    objImage.Width = maxWidth;
                    objImage.Height = Convert.ToInt32(objImage.Height / (photo.Width / maxWidth));
                }

                if (objImage.Height > maxHeight)
                {
                    objImage.Height = maxHeight;
                    objImage.Width = Convert.ToInt32(photo.Width / (photo.Height / maxHeight));
                }

                objImage.SortOrder = 0;

                List<ImageInfo> imagesList = objImageController.GetImageList(_articleID, _imageGuid);

                if (imagesList.Count > 0)
                {
                    objImage.SortOrder = ((ImageInfo)imagesList[imagesList.Count - 1]).SortOrder + 1;
                }

                PortalSettings objPortalSettings = PortalController.Instance.GetCurrentPortalSettings();

                string folder = "";
                int folderID = Null.NullInteger;
                if (Numeric.IsNumeric(context.Request.Form["FolderID"]))
                {
                    folderID = Convert.ToInt32(context.Request.Form["FolderID"]);
                }

                if (folderID != Null.NullInteger)
                {

                    FolderInfo objFolder = (FolderInfo)FolderManager.Instance.GetFolder(folderID);
                    if (objFolder != null)
                    {
                        folder = objFolder.FolderPath;
                    }
                }

                objImage.Folder = folder;

                switch (objImage.ContentType.ToLower())
                {
                    case "image/jpeg":
                        objImage.Extension = "jpg";
                        break;
                    case "image/gif":
                        objImage.Extension = "gif";
                        break;
                    case "image/png":
                        objImage.Extension = "png";
                        break;
                }

                objImage.Title = objFile.FileName.Replace("." + objImage.Extension, "");

                string filePath = objPortalSettings.HomeDirectoryMapPath + folder.Replace("/", @"\");

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

                objImage.Size = objFile.ContentLength;
                if ((photo.Width < maxWidth && photo.Height < maxHeight) || !ArticleSettings.ResizeImages)
                {
                    objFile.SaveAs(filePath + objImage.FileName);
                }
                else
                {
                    Bitmap bmp = new Bitmap(objImage.Width, objImage.Height);
                    Graphics g = Graphics.FromImage((System.Drawing.Image)bmp);

                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    g.DrawImage(photo, 0, 0, objImage.Width, objImage.Height);

                    if (ArticleSettings.WatermarkEnabled && ArticleSettings.WatermarkText != "")
                    {
                        SizeF crSize = new SizeF();
                        Brush brushColor = Brushes.Yellow;
                        Font fnt = new Font("Verdana", 11, FontStyle.Bold);
                        StringFormat strDirection = new StringFormat();

                        strDirection.Alignment = StringAlignment.Center;
                        crSize = g.MeasureString(ArticleSettings.WatermarkText, fnt);

                        int yPixelsFromBottom = Convert.ToInt32(Convert.ToDouble(objImage.Height) * 0.05);
                        Single yPosFromBottom = Convert.ToSingle((objImage.Height - yPixelsFromBottom) - (crSize.Height / 2));
                        Single xCenterOfImage = Convert.ToSingle((objImage.Width / 2));

                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        SolidBrush semiTransBrush2 = new SolidBrush(Color.FromArgb(153, 0, 0, 0));
                        g.DrawString(ArticleSettings.WatermarkText, fnt, semiTransBrush2, new PointF(xCenterOfImage + 1, yPosFromBottom + 1), strDirection);

                        SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(153, 255, 255, 255));
                        g.DrawString(ArticleSettings.WatermarkText, fnt, semiTransBrush, new PointF(xCenterOfImage, yPosFromBottom), strDirection);
                    }

                    if (ArticleSettings.WatermarkEnabled && ArticleSettings.WatermarkImage != "")
                    {
                        string watermark = objPortalSettings.HomeDirectoryMapPath + ArticleSettings.WatermarkImage;
                        if (File.Exists(watermark))
                        {
                            Image imgWatermark = new Bitmap(watermark);
                            int wmWidth = imgWatermark.Width;
                            int wmHeight = imgWatermark.Height;

                            ImageAttributes objImageAttributes = new ImageAttributes();
                            ColorMap objColorMap = new ColorMap();
                            objColorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                            objColorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                            ColorMap[] remapTable = { objColorMap };
                            objImageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                            Single[][] colorMatrixElements = { new Single[] { 1.0F, 0.0F, 0.0F, 0.0F, 0.0F }, new Single[] { 0.0F, 1.0F, 0.0F, 0.0F, 0.0F }, new Single[] { 0.0F, 0.0F, 1.0F, 0.0F, 0.0F }, new Single[] { 0.0F, 0.0F, 0.0F, 0.3F, 0.0F }, new Single[] { 0.0F, 0.0F, 0.0F, 0.0F, 1.0F } };
                            ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
                            objImageAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                            int xPosOfWm = ((objImage.Width - wmWidth) - 10);
                            int yPosOfWm = 10;

                            switch (ArticleSettings.WatermarkPosition)
                            {
                                case WatermarkPosition.TopLeft:
                                    xPosOfWm = 10;
                                    yPosOfWm = 10;
                                    break;

                                case WatermarkPosition.TopRight:
                                    xPosOfWm = ((objImage.Width - wmWidth) - 10);
                                    yPosOfWm = 10;
                                    break;

                                case WatermarkPosition.BottomLeft:
                                    xPosOfWm = 10;
                                    yPosOfWm = ((objImage.Height - wmHeight) - 10);
                                    break;
                                case WatermarkPosition.BottomRight:
                                    xPosOfWm = ((objImage.Width - wmWidth) - 10);
                                    yPosOfWm = ((objImage.Height - wmHeight) - 10);
                                    break;
                            }

                            g.DrawImage(imgWatermark, new Rectangle(xPosOfWm, yPosOfWm, wmWidth, wmHeight), 0, 0, wmWidth, wmHeight, GraphicsUnit.Pixel, objImageAttributes);
                            imgWatermark.Dispose();
                        }
                    }

                    photo.Dispose();
                    ImageCodecInfo[] info;
                    EncoderParameters encoderParameters;
                    switch (objFile.ContentType.ToLower())
                    {
                        case "image/jpeg":
                            info = ImageCodecInfo.GetImageEncoders();
                            encoderParameters = new EncoderParameters(1);
                            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                            bmp.Save(filePath + objImage.FileName, info[1], encoderParameters);
                            break;
                        case "image/gif":
                            //Dim quantizer As new ImageQuantization.OctreeQuantizer(255, 8)
                            //Dim bmpQuantized As Bitmap = quantizer.Quantize(bmp)
                            //bmpQuantized.Save(filePath & objPhoto.Filename, ImageFormat.Gif)
                            // Not working in medium trust.
                            bmp.Save(filePath + objImage.FileName, ImageFormat.Gif);
                            break;
                        default:
                            //Shouldn't get to here because of validators.                                
                            info = ImageCodecInfo.GetImageEncoders();
                            encoderParameters = new EncoderParameters(1);
                            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                            bmp.Save(filePath + objImage.FileName, info[1], encoderParameters);
                            break;
                    }

                    bmp.Dispose();

                    if (File.Exists(filePath + objImage.FileName))
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(filePath + objImage.FileName);
                        if (fi != null)
                        {
                            objImage.Size = Convert.ToInt32(fi.Length);
                        }
                    }
                }

                objImage.ImageID = objImageController.Add(objImage);

                if (_articleID != Null.NullInteger)
                {
                    ArticleController objArticleController = new ArticleController();
                    ArticleInfo objArticle = objArticleController.GetArticle(_articleID);
                    if (objArticle != null)
                    {
                        objArticle.ImageCount = objArticle.ImageCount + 1;
                        objArticleController.UpdateArticle(objArticle);
                    }
                }

            }

            _context.Response.Write("0");
            _context.Response.End();

        }

        //public bool IsReusable
        //{
        //    get
        //    {
        //        return false;
        //    }
        //}

        #endregion
    }
}