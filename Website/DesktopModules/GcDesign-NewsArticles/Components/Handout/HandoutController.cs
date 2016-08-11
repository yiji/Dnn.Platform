using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Common.Utilities;

namespace GcDesign.NewsArticles
{
    public class HandoutController
    {
        #region " public Methods "

        public void AddHandout(HandoutInfo objHandout)
        {

            int handoutID = (int)DataProvider.Instance().AddHandout(objHandout.ModuleID, objHandout.UserID, objHandout.Name, objHandout.Description);

            int i = 0;
            foreach (HandoutArticle objHandoutArticle in objHandout.Articles)
            {
                DataProvider.Instance().AddHandoutArticle(handoutID, objHandoutArticle.ArticleID, i);
                i = i + 1;
            }

        }

        public void DeleteHandout(int handoutID)
        {

            DataProvider.Instance().DeleteHandout(handoutID);
            DataProvider.Instance().DeleteHandoutArticleList(handoutID);

        }

        public HandoutInfo GetHandout(int handoutID)
        {

            HandoutInfo objHandout = (HandoutInfo)CBO.FillObject<HandoutInfo>(DataProvider.Instance().GetHandout(handoutID));

            objHandout.Articles = CBO.FillCollection<HandoutArticle>(DataProvider.Instance().GetHandoutArticleList(handoutID));

            return objHandout;

        }

        public List<HandoutInfo> ListHandout(int userID)
        {

            return CBO.FillCollection<HandoutInfo>(DataProvider.Instance().GetHandoutList(userID));

        }

        public void UpdateHandout(HandoutInfo objHandout)
        {

            DataProvider.Instance().UpdateHandout(objHandout.HandoutID, objHandout.ModuleID, objHandout.UserID, objHandout.Name, objHandout.Description);
            DataProvider.Instance().DeleteHandoutArticleList(objHandout.HandoutID);

            int i = 0;
            foreach (HandoutArticle objHandoutArticle in objHandout.Articles)
            {
                DataProvider.Instance().AddHandoutArticle(objHandout.HandoutID, objHandoutArticle.ArticleID, i);
                i = i + 1;
            }

        }

        #endregion
    }
}