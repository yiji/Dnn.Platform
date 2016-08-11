using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Common.Utilities;

namespace GcDesign.NewsArticles
{
    public class ImageController
    {
        #region " public Methods "

        public ImageInfo Get(int imageID)
        {

            return (ImageInfo)CBO.FillObject<ImageInfo>(DataProvider.Instance().GetImage(imageID));

        }

        public List<ImageInfo> GetImageList(int articleID, string imageGuid)
        {

            List<ImageInfo> objImages = (List<ImageInfo>)DataCache.GetCache(ArticleConstants.CACHE_IMAGE_ARTICLE + articleID.ToString());

            if (objImages == null)
            {
                objImages = CBO.FillCollection<ImageInfo>(DataProvider.Instance().GetImageList(articleID, imageGuid));
                DataCache.SetCache(ArticleConstants.CACHE_IMAGE_ARTICLE + articleID.ToString() + imageGuid, objImages);
            }
            return objImages;

        }

        public int Add(ImageInfo objImage)
        {

            DataCache.RemoveCache(ArticleConstants.CACHE_IMAGE_ARTICLE + objImage.ArticleID.ToString() + objImage.ImageGuid);
            int imageID = (int)DataProvider.Instance().AddImage(objImage.ArticleID, objImage.Title, objImage.FileName, objImage.Extension, objImage.Size, objImage.Width, objImage.Height, objImage.ContentType, objImage.Folder, objImage.SortOrder, objImage.ImageGuid, objImage.Description);
            ArticleController.ClearArticleCache(objImage.ArticleID);
            return imageID;

        }

        public void Update(ImageInfo objImage){

            DataProvider.Instance().UpdateImage(objImage.ImageID, objImage.ArticleID, objImage.Title, objImage.FileName, objImage.Extension, objImage.Size, objImage.Width, objImage.Height, objImage.ContentType, objImage.Folder, objImage.SortOrder, objImage.ImageGuid, objImage.Description);
            DataCache.RemoveCache(ArticleConstants.CACHE_IMAGE_ARTICLE + objImage.ArticleID.ToString() + objImage.ImageGuid);
            DataCache.RemoveCache(ArticleConstants.CACHE_IMAGE_ARTICLE + objImage.ArticleID.ToString());
            ArticleController.ClearArticleCache(objImage.ArticleID);

        }

        public void Delete(int imageID, int articleID, string imageGuid)
        {

            DataProvider.Instance().DeleteImage(imageID);
            DataCache.RemoveCache(ArticleConstants.CACHE_IMAGE_ARTICLE + articleID.ToString() + imageGuid);
            ArticleController.ClearArticleCache(articleID);

        }

        #endregion
    }
}