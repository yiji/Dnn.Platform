using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.FileSystem;
using System.IO;
using DotNetNuke.Entities.Modules;
using System.Web;
using System.Collections.Generic;
using System.Collections;
using System;

namespace GcDesign.NewsArticles
{

    public class CoreFileProvider2 : FileProvider
    {

        #region " Public Methods "

        public override int AddFile(int articleID, int moduleID, HttpPostedFile objPostedFile)
        {

            FileInfo objFile = new FileInfo();

            objFile.ArticleID = articleID;
            objFile.FileName = objPostedFile.FileName;
            objFile.SortOrder = 0;

            List<FileInfo> filesList = GetFiles(articleID);

            if (filesList.Count > 0)
            {
                objFile.SortOrder = ((FileInfo)filesList[filesList.Count - 1]).SortOrder + 1;
            }

            PortalSettings objPortalSettings = PortalController.Instance.GetCurrentPortalSettings();

            string folder = "";

            ModuleController objModuleController = new ModuleController();
            Hashtable objSettings = objModuleController.GetModule(moduleID).ModuleSettings;

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

            objFile.Folder = folder;
            objFile.ContentType = objPostedFile.ContentType;

            if (objFile.FileName.Split('.').Length > 0)
            {
                objFile.Extension = objFile.FileName.Split('.')[objFile.FileName.Split('.').Length - 1];

                if (objFile.Extension.ToLower() == "jpg")
                {
                    objFile.ContentType = "image/jpeg";
                }
                if (objFile.Extension.ToLower() == "gif")
                {
                    objFile.ContentType = "image/gif";
                }
                if (objFile.Extension.ToLower() == "txt")
                {
                    objFile.ContentType = "text/plain";
                }
                if (objFile.Extension.ToLower() == "html")
                {
                    objFile.ContentType = "text/html";
                }
                if (objFile.Extension.ToLower() == "mp3")
                {
                    objFile.ContentType = "audio/mpeg";
                }

            }
            objFile.Title = objFile.FileName.Replace("." + objFile.Extension, "");

            string filePath = objPortalSettings.HomeDirectoryMapPath + folder.Replace("/", @"\");

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

            objFile.Size = objPostedFile.ContentLength;
            objPostedFile.SaveAs(filePath + objFile.FileName);

            FileController objFileController = new FileController();
            objFile.FileID = objFileController.Add(objFile);

            if (articleID != Null.NullInteger)
            {
                ArticleController objArticleController = new ArticleController();
                ArticleInfo objArticle = objArticleController.GetArticle(articleID);
                if (objArticle != null)
                {
                    objArticle.FileCount = objArticle.FileCount + 1;
                    objArticleController.UpdateArticle(objArticle);
                }
            }

            return objFile.FileID;

        }

        public override int AddFile(int articleID, int moduleID, HttpPostedFile objPostedFile, object providerParams)
        {
            return AddFile(articleID, moduleID, objPostedFile);
        }

        public override int AddExistingFile(int articleID, int moduleID, object providerParams)
        {
            throw new NotImplementedException();
        }

        public override void DeleteFile(int articleID, int fileID)
        {
            FileController objFileController = new FileController();
            objFileController.Delete(fileID);
        }

        public override FileInfo GetFile(int fileID)
        {
            FileController objFileController = new FileController();
            FileInfo objFile = objFileController.Get(fileID);
            objFile.Link = PortalController.Instance.GetCurrentPortalSettings().HomeDirectory + objFile.Folder + objFile.FileName + "1";
            return objFile;
        }

        public override List<FileInfo> GetFiles(int articleID)
        {
            FileController objFileController = new FileController();
            List<FileInfo> objFiles = objFileController.GetFileList(articleID, Null.NullString);
            foreach (FileInfo objFile in objFiles)
            {
                objFile.Link = PortalController.Instance.GetCurrentPortalSettings().HomeDirectory + objFile.Folder + objFile.FileName + "1";
            }
            return objFiles;
        }

        public override void UpdateFile(FileInfo objFile)
        {
            FileController objFileController = new FileController();
            objFileController.Update(objFile);
        }

        #endregion

    }

}
