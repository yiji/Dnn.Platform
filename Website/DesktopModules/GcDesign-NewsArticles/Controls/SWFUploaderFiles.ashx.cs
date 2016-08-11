using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Services;

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
    /// SWFUploaderFiles 的摘要说明
    /// </summary>
    public class SWFUploaderFiles : IHttpHandler
    {

        #region " private Members "

        private int _articleID = Null.NullInteger;
        private int _moduleID = Null.NullInteger;
        private int _tabID = Null.NullInteger;
        private int _tabModuleID = Null.NullInteger;
        private int _portalID = Null.NullInteger;
        private string _ticket = Null.NullString;
        private int _userID = Null.NullInteger;
        private string _fileGuid = Null.NullString;

        private GcDesign.NewsArticles.ArticleSettings _articleSettings;
        private Hashtable _settings;
        private HttpContext _context;

        #endregion

        #region " private Properties "

        private GcDesign.NewsArticles.ArticleSettings ArticleSettings
        {
            get
            {
                if (_articleSettings == null)
                {
                    ModuleController objModuleController = new ModuleController();
                    ModuleInfo objModule = objModuleController.GetModule(_moduleID, _tabID);

                    _articleSettings = new GcDesign.NewsArticles.ArticleSettings(Settings, PortalController.Instance.GetCurrentPortalSettings(), objModule);
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
                    _fileGuid = _context.Request["ArticleGuid"];
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

            FileController objFileController = new FileController();

            HttpPostedFile objFilePosted = _context.Request.Files["Filedata"];

            if (objFilePosted != null)
            {

                PortalController objPortalController = new PortalController();
                if (!objPortalController.HasSpaceAvailable(_portalID, objFilePosted.ContentLength))
                {
                    _context.Response.Write("-1");
                    _context.Response.End();
                }

                string username = _context.User.Identity.Name;

                if (_articleID != Null.NullInteger)
                {
                    FileProvider.Instance().AddFile(_articleID, _moduleID, objFilePosted);
                }
                else
                {
                    FileProvider.Instance().AddFile(Convert.ToInt32(_fileGuid), _moduleID, objFilePosted);
                }

                //'Dim objFile As new FileInfo

                //'objFile.ArticleID = _articleID
                //'if (_articleID = Null.NullInteger) {
                //'    objFile.FileGuid = _fileGuid
                //'}
                //'objFile.FileName = objFilePosted.FileName
                //'objFile.SortOrder = 0

                //'Dim filesList As List(Of FileInfo) = objFileController.GetFileList(_articleID, _fileGuid)

                //'if (filesList.Count > 0) {
                //'    objFile.SortOrder = CType(filesList(filesList.Count - 1), FileInfo).SortOrder + 1
                //'}

                //'Dim objPortalSettings As PortalSettings = PortalController.Instance.GetCurrentPortalSettings()

                //'Dim folder As String = ""
                //'if (ArticleSettings.DefaultFilesFolder != Null.NullInteger) {
                //'    Dim objFolderController As new FolderController
                //'    Dim objFolder As FolderInfo = objFolderController.GetFolderInfo(_portalID, ArticleSettings.DefaultFilesFolder)
                //'    if (objFolder IsNot Nothing) {
                //'        folder = objFolder.FolderPath
                //'    }
                //'}

                //'objFile.Folder = folder
                //'objFile.ContentType = objFilePosted.ContentType

                //'if (objFile.FileName.Split("."c).Length > 0) {
                //'    objFile.Extension = objFile.FileName.Split("."c)(objFile.FileName.Split("."c).Length - 1)

                //'    if (objFile.Extension.ToLower() = "jpg") {
                //'        objFile.ContentType = "image/jpeg"
                //'    }
                //'    if (objFile.Extension.ToLower() = "gif") {
                //'        objFile.ContentType = "image/gif"
                //'    }
                //'    if (objFile.Extension.ToLower() = "txt") {
                //'        objFile.ContentType = "text/plain"
                //'    }
                //'    if (objFile.Extension.ToLower() = "html") {
                //'        objFile.ContentType = "text/html"
                //'    }
                //'    if (objFile.Extension.ToLower() = "mp3") {
                //'        objFile.ContentType = "audio/mpeg"
                //'    }

                //'}
                //'objFile.Title = objFile.FileName.Replace("." & objFile.Extension, "")

                //'Dim filePath As String = objPortalSettings.HomeDirectoryMapPath & folder.Replace("/", "\")

                //'if Not (Directory.Exists(filePath)) {
                //'    Directory.CreateDirectory(filePath)
                //'}

                //'if (File.Exists(filePath & objFile.FileName)) {
                //'    For i As Integer = 1 To 100
                //'        if (File.Exists(filePath & i.ToString() & "_" & objFile.FileName) = false) {
                //'            objFile.FileName = i.ToString() & "_" & objFile.FileName
                //'            Exit For
                //'        }
                //'    Next
                //'}

                //'objFile.Size = objFilePosted.ContentLength
                //'objFilePosted.SaveAs(filePath & objFile.FileName)

                //'objFile.FileID = objFileController.Add(objFile)

                //'if (_articleID != Null.NullInteger) {
                //'    Dim objArticleController As new ArticleController
                //'    Dim objArticle As ArticleInfo = objArticleController.GetArticle(_articleID)
                //'    if (objArticle IsNot Nothing) {
                //'        objArticle.FileCount = objArticle.FileCount + 1
                //'        objArticleController.UpdateArticle(objArticle)
                //'    }
                //'}

            }

            _context.Response.Write("0");
            _context.Response.End();

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #endregion



    }
}