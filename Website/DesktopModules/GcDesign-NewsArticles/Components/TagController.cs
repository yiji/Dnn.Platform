using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Common.Utilities;

namespace GcDesign.NewsArticles
{
    public class TagController
    {
        #region " private Methods "

        private void RemoveCache(int tagID)
        {

            TagInfo objTag = Get(tagID);

            if (objTag != null)
            {
                RemoveCache(objTag.ModuleID, objTag.TagID.ToString());
            }

        }

        private void RemoveCache(int moduleID, string nameLowered)
        {

            if (DataCache.GetCache("Tag-" + moduleID.ToString() + "-" + nameLowered) != null)
            {
                DataCache.RemoveCache("Tag-" + moduleID.ToString() + "-" + nameLowered);
            }

        }

        #endregion

        #region " public Methods "

        public TagInfo Get(int tagID)
        {

            return (TagInfo)CBO.FillObject<TagInfo>(DataProvider.Instance().GetTag(tagID));

        }

        public TagInfo Get(int moduleID, string nameLowered){

            TagInfo objTag = (TagInfo)DataCache.GetCache("Tag-" + moduleID.ToString() + "-" + nameLowered);

            if (objTag == null) {
                objTag = (TagInfo)CBO.FillObject<TagInfo>(DataProvider.Instance().GetTagByName(moduleID, nameLowered));
                if (objTag != null) {
                    DataCache.SetCache("Tag-" + moduleID.ToString() + "-" + nameLowered, objTag);
                }
            }

            return objTag;

        }

        public ArrayList List(int moduleID, int maxCount)
        {

            return CBO.FillCollection(DataProvider.Instance().ListTag(moduleID, maxCount), typeof(TagInfo));

        }

        public int Add(TagInfo objTag)
        {

            return (int)DataProvider.Instance().AddTag(objTag.ModuleID, objTag.Name, objTag.NameLowered);

        }

        public void Update(TagInfo objTag)
        {

            RemoveCache(objTag.ModuleID, objTag.NameLowered);
            DataProvider.Instance().UpdateTag(objTag.TagID, objTag.ModuleID, objTag.Name, objTag.NameLowered, objTag.Usages);

        }

        public void Delete(int tagID)
        {

            RemoveCache(tagID);
            DataProvider.Instance().DeleteTag(tagID);

        }

        public void DeleteArticleTag(int articleID)
        {

            ArticleController objArticleController = new ArticleController();
            ArticleInfo objArticle = objArticleController.GetArticle(articleID);

            if (objArticle != null)
            {
                if (objArticle.Tags.Trim() != "")
                {
                    foreach (string tag in objArticle.Tags.Split('.'))
                    {
                        RemoveCache(objArticle.ModuleID, tag.ToLower());
                    }
                }
            }
            DataProvider.Instance().DeleteArticleTag(articleID);

        }

        public void DeleteArticleTagByTag(int tagID)
        {

            RemoveCache(tagID);
            DataProvider.Instance().DeleteArticleTag(tagID);

        }

        public void Add(int articleID, int tagID)
        {

            RemoveCache(tagID);
            DataProvider.Instance().AddArticleTag(articleID, tagID);

        }

        #endregion
    }
}