using System.Collections;
using DotNetNuke.Common.Utilities;

namespace GcDesign.NewsArticles.Components.CustomFields
{
    public class CustomFieldController
    {
        #region " Private Members "

        private const string CACHE_KEY = "-NewsArticles-CustomFields-All";

        #endregion

        #region " Public Methods "

        public CustomFieldInfo Get(int customFieldID)
        {

            return CBO.FillObject<CustomFieldInfo>(DataProvider.Instance().GetCustomField(customFieldID));

        }

        public ArrayList List(int moduleID)
        {

            string key = moduleID.ToString() + CACHE_KEY;

            ArrayList objCustomFields = (ArrayList)DataCache.GetCache(key);

            if (objCustomFields == null)
            {
                objCustomFields = CBO.FillCollection(DataProvider.Instance().GetCustomFieldList(moduleID), typeof(CustomFieldInfo));
                DataCache.SetCache(key, objCustomFields);
            }

            return objCustomFields;

        }

        public void Delete(int moduleID, int customFieldID)
        {
            DataCache.RemoveCache(moduleID.ToString() + CACHE_KEY);
            DataProvider.Instance().DeleteCustomField(customFieldID);
        }

        public int Add(CustomFieldInfo objCustomField)
        {
            DataCache.RemoveCache(objCustomField.ModuleID.ToString() + CACHE_KEY);
            return (int)DataProvider.Instance().AddCustomField(objCustomField.ModuleID, objCustomField.Name, (int)objCustomField.FieldType, objCustomField.FieldElements, objCustomField.DefaultValue, objCustomField.Caption, objCustomField.CaptionHelp, objCustomField.IsRequired, objCustomField.IsVisible, objCustomField.SortOrder, (int)objCustomField.ValidationType, objCustomField.Length, objCustomField.RegularExpression);
        }
        public void Update(CustomFieldInfo objCustomField)
        {
            DataCache.RemoveCache(objCustomField.ModuleID.ToString() + CACHE_KEY);
            DataProvider.Instance().UpdateCustomField(objCustomField.CustomFieldID, objCustomField.ModuleID, objCustomField.Name, (int)objCustomField.FieldType, objCustomField.FieldElements, objCustomField.DefaultValue, objCustomField.Caption, objCustomField.CaptionHelp, objCustomField.IsRequired, objCustomField.IsVisible, objCustomField.SortOrder, (int)objCustomField.ValidationType, objCustomField.Length, objCustomField.RegularExpression);
        }

        #endregion
    }
}