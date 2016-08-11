using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Common.Utilities;
using System.Security.Cryptography;
using System.Collections;
using System.Text;

namespace GcDesign.NewsArticles
{
    public class AuthorController
    {
         public static void ClearCache(int moduleID){

            List<string> itemsToRemove =new List<string>();

            if (HttpContext.Current != null) {
                IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();
                while( enumerator.MoveNext()){
                    if( enumerator.Key.ToString().ToLower().Contains("gcdesign-newsarticles-authors-" + moduleID.ToString())) {
                        itemsToRemove.Add(enumerator.Key.ToString());
                    }
            }

                 foreach(string itemToRemove in itemsToRemove){
                    DataCache.RemoveCache(itemToRemove.Replace("DNN_", ""));
                }
            }

       }

        public List<AuthorInfo> GetAuthorList(int moduleID){

            string cacheKey = "gcdesign-newsarticles-authors-" + moduleID.ToString();

            List<AuthorInfo> objAuthors = (List<AuthorInfo>)DataCache.GetCache(cacheKey);

            if (objAuthors == null) {
                objAuthors = CBO.FillCollection<AuthorInfo>(DataProvider.Instance().GetAuthorList(moduleID));
                DataCache.SetCache(cacheKey, objAuthors);
            }

            return objAuthors;

        }

        public List<AuthorInfo> GetAuthorStatistics(int moduleID , int[] categoryID  , int[] categoryIDExclude , int authorID , string sortBy , bool showPending ){

            string categories = Null.NullString;

            if (categoryID != null) {
                foreach(int category in categoryID){
                    if (categories != Null.NullString) {
                        categories = categories + ",";
                    }
                    categories = categories + category.ToString();
                }
            }

            string categoriesExclude = Null.NullString;

            if (categoryIDExclude != null) {
                foreach(int category in categoryIDExclude){
                    if (categoriesExclude != Null.NullString) {
                        categoriesExclude = categoriesExclude + ",";
                    }
                    categoriesExclude = categoriesExclude + category.ToString();
                }
            }

            string hashCategories = "";
            if (categories != "" || categoriesExclude != "") {
                UnicodeEncoding Ue = new UnicodeEncoding();
                byte[] ByteSourceText = Ue.GetBytes(categories + categoriesExclude);
                MD5CryptoServiceProvider Md5 =new MD5CryptoServiceProvider();
                byte[] ByteHash = Md5.ComputeHash(ByteSourceText);
                hashCategories = Convert.ToBase64String(ByteHash);
            }

            if (sortBy == "ArticleCount") {
                sortBy = "ArticleCount DESC";
            }

            string cacheKey = "gcdesign-newsarticles-authors-" + moduleID.ToString() + "-" + hashCategories + "-" + authorID.ToString() + "-" + sortBy.ToString() + "-" + showPending.ToString();

            List<AuthorInfo> objAuthorStatistics  = (List<AuthorInfo>)DataCache.GetCache(cacheKey);

            if (objAuthorStatistics == null) {
                objAuthorStatistics = CBO.FillCollection<AuthorInfo>(DataProvider.Instance().GetAuthorStatistics(moduleID, categoryID, categoryIDExclude, authorID, sortBy, showPending));
                DataCache.SetCache(cacheKey, objAuthorStatistics);
            }

            return objAuthorStatistics;

        }

    }
}