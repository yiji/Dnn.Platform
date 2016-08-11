using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Common.Utilities;

namespace GcDesign.NewsArticles.Import
{
    public class FeedController
    {
        #region " public Methods "

        public FeedInfo Get(int feedID)
        {

            return (FeedInfo)CBO.FillObject<FeedInfo>(DataProvider.Instance().GetFeed(feedID));

        }

        public List<FeedInfo> GetFeedList(int moduleID, bool showActiveOnly)
        {

            return CBO.FillCollection<FeedInfo>(DataProvider.Instance().GetFeedList(moduleID, showActiveOnly));

        }

        public int Add(FeedInfo objFeed)
        {

            int feedID = (int)DataProvider.Instance().AddFeed(objFeed.ModuleID, objFeed.Title, objFeed.Url, objFeed.UserID, objFeed.AutoFeature, objFeed.IsActive, (int)objFeed.DateMode, objFeed.AutoExpire, (int)objFeed.AutoExpireUnit);

            foreach (CategoryInfo objCategory in objFeed.Categories)
            {
                AddFeedCategory(feedID, objCategory.CategoryID);
            }

            return feedID;

        }

        public void Update(FeedInfo objFeed)
        {

            DataProvider.Instance().UpdateFeed(objFeed.FeedID, objFeed.ModuleID, objFeed.Title, objFeed.Url, objFeed.UserID, objFeed.AutoFeature, objFeed.IsActive, (int)objFeed.DateMode, objFeed.AutoExpire, (int)objFeed.AutoExpireUnit);

            DeleteFeedCategory(objFeed.FeedID);
            foreach (CategoryInfo objCategory in objFeed.Categories)
            {
                AddFeedCategory(objFeed.FeedID, objCategory.CategoryID);
            }

        }

        public void Delete(int feedID)
        {

            DataProvider.Instance().DeleteFeed(feedID);

        }

        public void AddFeedCategory(int feedID, int categoryID)
        {

            DataProvider.Instance().AddFeedCategory(feedID, categoryID);

        }

        public List<CategoryInfo> GetFeedCategoryList(int feedID)
        {

            return CBO.FillCollection<CategoryInfo>(DataProvider.Instance().GetFeedCategoryList(feedID));

        }

        public void DeleteFeedCategory(int feedID)
        {

            DataProvider.Instance().DeleteFeedCategory(feedID);

        }

        #endregion
    }
}