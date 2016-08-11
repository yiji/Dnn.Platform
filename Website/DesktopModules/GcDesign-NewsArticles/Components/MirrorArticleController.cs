using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke;
using DotNetNuke.Common.Utilities;

namespace GcDesign.NewsArticles
{
    public class MirrorArticleController
    {
        #region " public Methods "

        public void AddMirrorArticle(MirrorArticleInfo objMirrorArticle){

            DataProvider.Instance().AddMirrorArticle(objMirrorArticle.ArticleID, objMirrorArticle.LinkedArticleID, objMirrorArticle.LinkedPortalID, objMirrorArticle.AutoUpdate);

        }

        public MirrorArticleInfo GetMirrorArticle(int articleID) {

            return (MirrorArticleInfo)CBO.FillObject<MirrorArticleInfo>(DataProvider.Instance().GetMirrorArticle(articleID));

        }

        public ArrayList GetMirrorArticleList(int linkedArticleID){

            return CBO.FillCollection(DataProvider.Instance().GetMirrorArticleList(linkedArticleID), typeof(MirrorArticleInfo));

        }

#endregion
    }
}