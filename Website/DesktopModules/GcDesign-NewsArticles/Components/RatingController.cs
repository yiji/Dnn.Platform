using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework;

namespace GcDesign.NewsArticles
{
    public class RatingController
    {
        #region " Static Methods "

        public static void ClearCache(int moduleID)
        {

            List<string> itemsToRemove = new List<string>();

            IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().ToLower().StartsWith("gcdesign-newsarticles-rating-" + moduleID.ToString()))
                {
                    itemsToRemove.Add(enumerator.Key.ToString());
                }
            }

            foreach (string itemToRemove in itemsToRemove)
            {
                DataCache.RemoveCache(itemToRemove.Replace("DNN_", ""));
            }

        }

        #endregion

        #region " public Methods "

        public int Add(RatingInfo objRating, int moduleID)
        {

            int ratingID = (int)DataProvider.Instance().AddRating(objRating.ArticleID, objRating.UserID, objRating.CreatedDate, objRating.Rating);

            ArticleController.ClearArticleCache(objRating.ArticleID);

            string cacheKey = "gcdesign-newsarticles-rating-" + moduleID.ToString() + "-aid-" + objRating.ArticleID.ToString() + "-" + objRating.UserID.ToString();
            DataCache.RemoveCache(cacheKey);

            return ratingID;

        }

        public RatingInfo Get(int articleID, int userID, int moduleID)
        {

            string cacheKey = "gcdesign-newsarticles-rating-" + moduleID.ToString() + "-aid-" + articleID.ToString() + "-" + userID.ToString();

            RatingInfo objRating = (RatingInfo)DataCache.GetCache(cacheKey);

            if (objRating == null)
            {
                objRating = (RatingInfo)CBO.FillObject<RatingInfo>(DataProvider.Instance().GetRating(articleID, userID));
                if (objRating != null)
                {
                    DataCache.SetCache(cacheKey, objRating);
                }
                else
                {
                    objRating = new RatingInfo();
                    objRating.RatingID = Null.NullInteger;
                    DataCache.SetCache(cacheKey, objRating);
                }
            }

            return objRating;

        }

        public RatingInfo GetByID(int ratingID, int articleID, int moduleID)
        {

            string cacheKey = "gcdesign-newsarticles-rating-" + moduleID.ToString() + "-id-" + ratingID.ToString();

            RatingInfo objRating = (RatingInfo)DataCache.GetCache(cacheKey);

            if (objRating == null)
            {
                objRating = (RatingInfo)CBO.FillObject<RatingInfo>(DataProvider.Instance().GetRatingByID(ratingID));
                DataCache.SetCache(cacheKey, objRating);
            }

            return objRating;

        }

        public void Delete(int ratingID, int articleID, int moduleID){

            DataProvider.Instance().DeleteRating(ratingID);

            ArticleController.ClearArticleCache(articleID);

            string cacheKey = "gcdesign-newsarticles-rating-" + moduleID.ToString() + "-id-" + ratingID.ToString();
            DataCache.RemoveCache(cacheKey);

        }

        #endregion
    }
}