using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

using DotNetNuke.Framework;
using DotNetNuke;
using DotNetNuke.Common.Utilities;

namespace GcDesign.NewsArticles
{
    public abstract class FileProvider
    {

        #region " Shared/Static Methods "

        // singleton reference to the instantiated object 
        private static FileProvider objProvider = null;

        // constructor
        static FileProvider()
        {
            CreateProvider();
        }

        // 动态实例化对象
        private static void CreateProvider()
        {
            if (ConfigurationManager.AppSettings["MultiConditionSearchesFileProvider"] != null)
            {
                objProvider = (FileProvider)DotNetNuke.Framework.Reflection.CreateObject(ConfigurationManager.AppSettings["MultiConditionSearchesFileProvider"].ToString(), "MultiConditionSearches_NaFileProvider");
            }
            else
            {
                objProvider = (FileProvider)DotNetNuke.Framework.Reflection.CreateObject("GcDesign.NewsArticles.CoreFileProvider", "MultiConditionSearches_NaFileProvider");
            }
        }

        //返回提供程序
        public static FileProvider Instance()
        {
            return objProvider;
        }

        #endregion

        #region " Abstract methods "

        public abstract int AddFile(int articleID, int moduleID, System.Web.HttpPostedFile objPostedFile);
        public abstract int AddFile(int articleID, int moduleID, System.Web.HttpPostedFile objPostedFile, object providerParams);
        public abstract int AddExistingFile(int articleID, int moduleID, object providerParams);
        public abstract void DeleteFile(int articleID, int fileID);
        public abstract FileInfo GetFile(int fileID);
        public abstract List<FileInfo> GetFiles(int articleID);
        public abstract void UpdateFile(FileInfo objFile);


        #endregion

    }
}