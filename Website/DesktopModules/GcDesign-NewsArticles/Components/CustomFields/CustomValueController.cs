using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Common.Utilities;

namespace GcDesign.NewsArticles.Components.CustomFields
{
    public class CustomValueController
    {
        #region " Private Members "

        private const string CACHE_KEY = "-NewsArticles-CustomValues-All";

        #endregion

        #region " Public Methods "

        public CustomValueInfo GetByCustomField(int articleID, int customFieldID)
        {
            List<CustomValueInfo> objCustomValues = List(articleID);
            foreach (CustomValueInfo objCustomValue in objCustomValues)
            {
                if (objCustomValue.CustomFieldID == customFieldID)
                {
                    return objCustomValue;
                }
            }
            return null;
        }

        public List<CustomValueInfo> List(int articleID)
        {
            string key = articleID.ToString() + CACHE_KEY;
            List<CustomValueInfo> objCustomValues = (List<CustomValueInfo>)DataCache.GetCache(key);
            if (objCustomValues == null)
            {
                objCustomValues = CBO.FillCollection<CustomValueInfo>(DataProvider.Instance().GetCustomValueList(articleID));
                DataCache.SetCache(key, objCustomValues);
            }
            return objCustomValues;
        }

        public int Add(CustomValueInfo objCustomValue)
        {
            DataCache.RemoveCache(objCustomValue.ArticleID.ToString() + CACHE_KEY);
            return DataProvider.Instance().AddCustomValue(objCustomValue.ArticleID, objCustomValue.CustomFieldID, objCustomValue.CustomValue);
        }

        public void Update(CustomValueInfo objCustomValue)
        {
            DataCache.RemoveCache(objCustomValue.ArticleID.ToString() + CACHE_KEY);
            DataProvider.Instance().UpdateCustomValue(objCustomValue.CustomValueID, objCustomValue.ArticleID, objCustomValue.CustomFieldID, objCustomValue.CustomValue);
        }

        public void Delete(int articleID, int customFieldID)
        {
            DataCache.RemoveCache(articleID.ToString() + CACHE_KEY);
            DataProvider.Instance().DeleteCustomValue(articleID, customFieldID);
        }

        #endregion

    }
}