using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Common.Utilities;

namespace GcDesign.NewsArticles
{
    public class FileController
    {
        public static void ClearCache(int articleID)
        {

            //List<string> itemsToRemove = new List<string>();

            //IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();
            //while (enumerator.MoveNext())
            //{
            //    if (enumerator.Key.ToString().ToLower().Contains("gcdesign-MultiConditionSearches-files-" + articleID.ToString()))
            //    {
            //        itemsToRemove.Add(enumerator.Key.ToString());
            //    }
            //}

            //foreach (string itemToRemove in itemsToRemove)
            //{
            //    DataCache.RemoveCache(itemToRemove.Replace("DNN_", ""));
            //}

            if (DataCache.GetCache("gcdesign-MultiConditionSearches-files-" + articleID.ToString()) != null)
            {
                DataCache.RemoveCache("gcdesign-MultiConditionSearches-files-" + articleID.ToString());
            }

        }

        #region " public Methods "

        public FileInfo Get(int fileID)
        {

            return (FileInfo)CBO.FillObject<FileInfo>(DataProvider.Instance().GetFile(fileID));

        }

        public List<FileInfo> GetFileList(int articleID, string fileGuid)
        {

            if (articleID == Null.NullInteger)
            {
                return CBO.FillCollection<FileInfo>(DataProvider.Instance().GetFileList(articleID, fileGuid));
            }
            else
            {
                //string cacheKey = "gcdesign-MultiConditionSearches-files-" + articleID.ToString() + "-" + fileGuid.ToString();
                string cacheKey = "gcdesign-MultiConditionSearches-files-" + articleID.ToString();

                List<FileInfo> objFiles = (List<FileInfo>)DataCache.GetCache(cacheKey);

                if (objFiles == null)
                {
                    objFiles = CBO.FillCollection<FileInfo>(DataProvider.Instance().GetFileList(articleID, fileGuid));
                    DataCache.SetCache(cacheKey, objFiles);
                }

                return objFiles;
            }

        }

        public int Add(FileInfo objFile)
        {

            int fileID = (int)DataProvider.Instance().AddFile(objFile.ArticleID, objFile.Title, objFile.FileName, objFile.Extension, objFile.Size, objFile.ContentType, objFile.Folder, objFile.SortOrder, objFile.FileGuid);

            FileController.ClearCache(objFile.ArticleID);
            ArticleController.ClearArticleCache(objFile.ArticleID);

            return fileID;

        }

        public void Update(FileInfo objFile)
        {

            DataProvider.Instance().UpdateFile(objFile.FileID, objFile.ArticleID, objFile.Title, objFile.FileName, objFile.Extension, objFile.Size, objFile.ContentType, objFile.Folder, objFile.SortOrder, objFile.FileGuid);

            FileController.ClearCache(objFile.ArticleID);

        }

        public void Delete(int fileID)
        {

            FileInfo objFile = Get(fileID);

            if (objFile != null)
            {
                DataProvider.Instance().DeleteFile(fileID);
                FileController.ClearCache(objFile.ArticleID);
            }

        }

        #endregion
    }
}