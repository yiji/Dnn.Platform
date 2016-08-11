using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke;
using DotNetNuke.Common.Utilities;

namespace GcDesign.NewsArticles
{
    public class CategoryController
    {
        #region " Public Methods "

        public static void ClearCache(int moduleID)
        {

            List<string> itemsToRemove = new List<string>();

            if (HttpContext.Current != null)
            {
                IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Key.ToString().ToLower().Contains("gcdesign-newsarticles-categories-" + moduleID.ToString()))
                    {
                        itemsToRemove.Add(enumerator.Key.ToString());
                    }
                }

                foreach (string itemToRemove in itemsToRemove)
                {
                    DataCache.RemoveCache(itemToRemove.Replace("DNN_", ""));
                }
            }

        }

        public List<CategoryInfo> GetCategories(int moduleID, int parentID)
        {

            return GetCategoriesAll(moduleID, parentID, null, Null.NullInteger, 1, Null.NullBoolean, CategorySortType.Name);

        }

        public List<CategoryInfo> GetCategoriesAll(int moduleID)
        {

            return GetCategoriesAll(moduleID, Null.NullInteger, null, Null.NullInteger, Null.NullInteger, Null.NullBoolean, CategorySortType.Name);

        }

        public List<CategoryInfo> GetCategoriesAll(int moduleID, int parentID)
        {

            return GetCategoriesAll(moduleID, parentID, CategorySortType.Name);

        }

        public List<CategoryInfo> GetCategoriesAll(int moduleID, int parentID, CategorySortType sortType)
        {

            return GetCategoriesAll(moduleID, parentID, null, Null.NullInteger, Null.NullInteger, false, sortType);

        }

        public List<CategoryInfo> GetCategoriesAll(int moduleID, int parentID, int[] categoryIDFilter, int authorID, int maxDepth, bool showPending, CategorySortType sortType)
        {

            string cacheKey = "GcDesign-NewsArticles-Categories-" + moduleID.ToString() + "-" + sortType.ToString();

            if (authorID != Null.NullInteger)
            {
                cacheKey = cacheKey + "-a-" + authorID.ToString();
            }

            if (showPending != Null.NullBoolean)
            {
                cacheKey = cacheKey + "-sp-" + showPending.ToString();
            }

            List<CategoryInfo> objCategories = (List<CategoryInfo>)DataCache.GetCache(cacheKey);

            if (objCategories == null)
            {
                objCategories = CBO.FillCollection<CategoryInfo>(DataProvider.Instance().GetCategoryListAll(moduleID, authorID, showPending, (int)sortType));
                DataCache.SetCache(cacheKey, objCategories);
            }

            if (categoryIDFilter != null)
            {
                List<CategoryInfo> objNewCategories = new List<CategoryInfo>();
                foreach (int id in categoryIDFilter)
                {
                    foreach (CategoryInfo objCategory in objCategories)
                    {
                        if (objCategory.CategoryID == id)
                        {
                            objNewCategories.Add(objCategory);
                        }
                    }
                }
                objCategories = objNewCategories;
            }

            List<CategoryInfo> objCategoriesCopy = new List<CategoryInfo>(objCategories);

            if (parentID != Null.NullInteger)
            {
                int level = Null.NullInteger;
                List<int> objParentIDs = new List<int>();
                objParentIDs.Add(parentID);
                List<CategoryInfo> objNewCategories = new List<CategoryInfo>();
                foreach (CategoryInfo objCategory in objCategoriesCopy)
                {
                    foreach (int id in objParentIDs.ToArray())
                    {
                        if (objCategory.ParentID == id)
                        {
                            if (level == Null.NullInteger)
                            {
                                level = objCategory.Level;
                            }

                            if (maxDepth == Null.NullInteger || objCategory.Level < (level + maxDepth))
                            {
                                CategoryInfo objCategoryNew = objCategory.Clone();
                                objCategoryNew.Level = objCategory.Level - level + 1;
                                objNewCategories.Add(objCategoryNew);
                                if (!objParentIDs.Contains(objCategory.CategoryID))
                                {
                                    objParentIDs.Add(objCategory.CategoryID);
                                }
                                break;
                            }
                        }
                    }
                }
                objCategoriesCopy = objNewCategories;
            }
            else
            {
                if (maxDepth != Null.NullInteger)
                {
                    List<CategoryInfo> objNewCategories = new List<CategoryInfo>();
                    foreach (CategoryInfo objCategory in objCategoriesCopy)
                    {
                        if (objCategory.Level <= maxDepth)
                        {
                            objNewCategories.Add(objCategory);
                        }
                    }
                    objCategoriesCopy = objNewCategories;
                }
            }

            return objCategoriesCopy;

        }

        public void MoveNoddeBefore(CategoryInfo source, CategoryInfo target)
        {
            DataProvider.Instance().MoveOrgBefore(source.CategoryID, target.CategoryID, target.ParentID, target.SortOrder);
            CategoryController.ClearCache(source.ModuleID);
        }


        [Obsolete("This method is deprecated, use GetCategories instead.")]
        public ArrayList GetCategoryList(int moduleID, int parentID)
        {

            List<CategoryInfo> objCategories = GetCategories(moduleID, parentID);
            ArrayList objCategoriesToReturn = new ArrayList();

            foreach (CategoryInfo objCategory in objCategories)
            {
                objCategoriesToReturn.Add(objCategory);
            }

            return objCategoriesToReturn;

        }

        [Obsolete("This method is deprecated, use GetCategoriesAll instead.")]
        public ArrayList GetCategoryListAll(int moduleID)
        {

            return GetCategoryListAll(moduleID, Null.NullInteger, null, Null.NullInteger, Null.NullInteger, Null.NullBoolean, CategorySortType.Name);

        }

        [Obsolete("This method is deprecated, use GetCategoriesAll instead.")]
        public ArrayList GetCategoryListAll(int moduleID, int parentID)
        {

            return GetCategoryListAll(moduleID, parentID, CategorySortType.Name);

        }

        [Obsolete("This method is deprecated, use GetCategoriesAll instead.")]
        public ArrayList GetCategoryListAll(int moduleID, int parentID, CategorySortType sortType)
        {

            return GetCategoryListAll(moduleID, parentID, null, Null.NullInteger, Null.NullInteger, false, sortType);

        }

        [Obsolete("This method is deprecated, use GetCategoriesAll instead.")]
        public ArrayList GetCategoryListAll(int moduleID, int parentID, int[] categoryIDFilter, int authorID, int maxDepth, bool showPending, CategorySortType sortType)
        {

            List<CategoryInfo> objCategories = GetCategoriesAll(moduleID, parentID, categoryIDFilter, authorID, maxDepth, showPending, sortType);
            ArrayList objCategoriesToReturn = new ArrayList();

            foreach (CategoryInfo objCategory in objCategories)
            {
                objCategoriesToReturn.Add(objCategory);
            }

            return objCategoriesToReturn;

        }

        public CategoryInfo GetCategory(int categoryID, int moduleID)
        {

            List<CategoryInfo> objCategories = GetCategoriesAll(moduleID);

            foreach (CategoryInfo objCategory in objCategories)
            {
                if (objCategory.CategoryID == categoryID)
                {
                    return objCategory;
                }
            }

            return (CategoryInfo)CBO.FillObject<CategoryInfo>(DataProvider.Instance().GetCategory(categoryID));

        }

        public void DeleteCategory(int categoryID, int moduleID)
        {

            DataProvider.Instance().DeleteCategory(categoryID);
            CategoryController.ClearCache(moduleID);

        }

        public int AddCategory(CategoryInfo objCategory)
        {

            int categoryID = (int)DataProvider.Instance().AddCategory(objCategory.ModuleID, objCategory.ParentID, objCategory.Name, objCategory.Image, objCategory.Description, objCategory.SortOrder, objCategory.InheritSecurity, (int)objCategory.CategorySecurityType, objCategory.MetaTitle, objCategory.MetaDescription, objCategory.MetaKeywords);
            CategoryController.ClearCache(objCategory.ModuleID);
            return categoryID;

        }

        public void UpdateCategory(CategoryInfo objCategory)
        {

            DataProvider.Instance().UpdateCategory(objCategory.CategoryID, objCategory.ModuleID, objCategory.ParentID, objCategory.Name, objCategory.Image, objCategory.Description, objCategory.SortOrder, objCategory.InheritSecurity, (int)objCategory.CategorySecurityType, objCategory.MetaTitle, objCategory.MetaDescription, objCategory.MetaKeywords);
            CategoryController.ClearCache(objCategory.ModuleID);

        }

        #endregion
    }
}