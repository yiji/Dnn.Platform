using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Security;

using GcDesign.NewsArticles.Base;
using DotNetNuke.Services.FileSystem;

namespace GcDesign.NewsArticles
{
    public partial class ViewCategory : NewsArticleModuleBase
    {

        #region " Constants "

        private const string PARAM_CATEGORY_ID = "CategoryID";

        #endregion

        #region " private Members "

        private LayoutController _layoutController;
        private List<CategoryInfo> _objCategoriesAll;
        private int _categoryID = Null.NullInteger;

        #endregion

        #region " private Methods "

        private void BindCategory()
        {

            if (_categoryID == Null.NullInteger)
            {
                // Category not specified
                return;
            }

            CategoryController objCategoryController = new CategoryController();
            CategoryInfo objCategory = objCategoryController.GetCategory(_categoryID, ModuleId);

            if (objCategory != null)
            {

                if (objCategory.ModuleID != this.ModuleId)
                {
                    // Category does not belong to this module.
                    Response.Redirect(Globals.NavigateURL(), true);
                }

                ProcessCategory(objCategory, phCategory.Controls);

            }
            else
            {

                Response.Redirect(Globals.NavigateURL(), true);

            }

        }

        private void ProcessCategoryChild(CategoryInfo objCategory, ControlCollection objPlaceHolder, string moduleKey, string[] templateArray, int level)
        {

            for (int iPtr = 0; iPtr < templateArray.Length; iPtr += 2)
            {

                objPlaceHolder.Add(new LiteralControl(_layoutController.ProcessImages(templateArray[iPtr].ToString())));

                Literal objLiteral;

                if (iPtr < templateArray.Length - 1)
                {
                    switch (templateArray[iPtr + 1])
                    {

                        case "ARTICLECOUNT":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                            objLiteral.Text = objCategory.NumberOfArticles.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "CATEGORYID":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                            objLiteral.Text = objCategory.CategoryID.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "DEPTHABS":
                            foreach (CategoryInfo objCategoryItem in _objCategoriesAll)
                            {
                                if (objCategoryItem.CategoryID == objCategory.CategoryID)
                                {
                                    objLiteral = new Literal();
                                    objLiteral.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                                    objLiteral.Text = objCategoryItem.Level.ToString();
                                    objPlaceHolder.Add(objLiteral);
                                }
                            }
                            break;
                        case "DEPTHREL":
                            foreach (CategoryInfo objCategoryItem in _objCategoriesAll)
                            {
                                if (objCategoryItem.CategoryID == objCategory.CategoryID)
                                {
                                    objLiteral = new Literal();
                                    objLiteral.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                                    objLiteral.Text = (objCategoryItem.Level - level).ToString();
                                    objPlaceHolder.Add(objLiteral);
                                }
                            }
                            break;
                        case "DESCRIPTION":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                            objLiteral.Text = Server.HtmlDecode(objCategory.Description);
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "HASIMAGE":
                            if (objCategory.Image == "")
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/HASIMAGE")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASIMAGE":
                            // Do null
                            break;
                        case "HASNOIMAGE":
                            if (objCategory.Image != "")
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/HASNOIMAGE")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASNOIMAGE":
                            // Do null
                            break;
                        case "IMAGE":
                            if (objCategory.Image != "")
                            {

                                if (objCategory.Image.Split('=').Length == 2)
                                {
                                    if (Numeric.IsNumeric(objCategory.Image.Split('=')[1]))
                                    {
                                        DotNetNuke.Services.FileSystem.FileInfo objFile = (DotNetNuke.Services.FileSystem.FileInfo)FileManager.Instance.GetFile(Convert.ToInt32(objCategory.Image.Split('=')[1]));

                                        if (objFile != null)
                                        {
                                            Image objImage = new Image();
                                            objImage.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                                            objImage.ImageUrl = PortalSettings.HomeDirectory + objFile.Folder + objFile.FileName;
                                            objPlaceHolder.Add(objImage);
                                        }
                                    }

                                }

                            }
                            break;
                        case "IMAGELINK":
                            if (objCategory.Image != "")
                            {

                                if (objCategory.Image.Split('=').Length == 2)
                                {
                                    if (Numeric.IsNumeric(objCategory.Image.Split('=')[1]))
                                    {
                                        DotNetNuke.Services.FileSystem.FileInfo objFile = (DotNetNuke.Services.FileSystem.FileInfo)FileManager.Instance.GetFile(Convert.ToInt32(objCategory.Image.Split('=')[1]));

                                        if (objFile != null)
                                        {
                                            objLiteral = new Literal();
                                            objLiteral.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                                            objLiteral.Text = PortalSettings.HomeDirectory + objFile.Folder + objFile.FileName;
                                            objPlaceHolder.Add(objLiteral);
                                        }
                                    }

                                }

                            }
                            break;

                        case "LINK":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                            objLiteral.Text = Common.GetCategoryLink(TabId, ModuleId, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings);
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "METADESCRIPTION":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                            objLiteral.Text = objCategory.MetaDescription.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "NAME":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                            objLiteral.Text = objCategory.Name;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "RSSLINK":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                            objLiteral.Text = Page.ResolveUrl("~/DesktopModules/GcDesign-NewsArticles/RSS.aspx?TabID=" + TabId.ToString() + "&amp;ModuleID=" + ModuleId.ToString() + "&amp;CategoryID=" + objCategory.CategoryID.ToString());
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "ORDER":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                            objLiteral.Text = objCategory.SortOrder.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "VIEWS":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                            objLiteral.Text = objCategory.NumberOfViews.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        default:
                            if (templateArray[iPtr + 1].ToUpper().StartsWith("DESCRIPTION:"))
                            {

                                string description = Server.HtmlDecode(objCategory.Description);
                                if (Numeric.IsNumeric(templateArray[iPtr + 1].Substring(12, templateArray[iPtr + 1].Length - 12)))
                                {
                                    int length = Convert.ToInt32(templateArray[iPtr + 1].Substring(12, templateArray[iPtr + 1].Length - 12));
                                    if (StripHtml(Server.HtmlDecode(objCategory.Description)).TrimStart().Length > length)
                                    {
                                        description = StripHtml(Server.HtmlDecode(objCategory.Description)).TrimStart().Substring(0, length) + "...";
                                    }
                                    else
                                    {
                                        description = StripHtml(Server.HtmlDecode(objCategory.Description)).TrimStart().Substring(0, StripHtml(Server.HtmlDecode(objCategory.Description)).TrimStart().Length);
                                    }
                                }

                                objLiteral = new Literal();
                                objLiteral.ID = Globals.CreateValidID(moduleKey + "-" + iPtr.ToString());
                                objLiteral.Text = description;
                                objPlaceHolder.Add(objLiteral);
                                break;
                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("IFORDER:"))
                            {
                                string depth = templateArray[iPtr + 1].Substring(8, templateArray[iPtr + 1].Length - 8);
                                bool isOrder = false;
                                foreach (string item in depth.Split(','))
                                {
                                    if (Numeric.IsNumeric(item))
                                    {
                                        if (objCategory.SortOrder == Convert.ToInt32(item))
                                        {
                                            isOrder = true;
                                        }
                                    }
                                }

                                string endToken = "/" + templateArray[iPtr + 1];
                                if (!isOrder)
                                {
                                    while (iPtr < templateArray.Length - 1)
                                    {
                                        if (templateArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("IFNOTORDER:"))
                            {
                                string depth = templateArray[iPtr + 1].Substring(11, templateArray[iPtr + 1].Length - 11);
                                bool isOrder = false;
                                foreach (string item in depth.Split(','))
                                {
                                    if (Numeric.IsNumeric(item))
                                    {
                                        if (objCategory.SortOrder == Convert.ToInt32(item))
                                        {
                                            isOrder = true;
                                        }
                                    }
                                }

                                string endToken = "/" + templateArray[iPtr + 1];
                                if (isOrder == true)
                                {
                                    while (iPtr < templateArray.Length - 1)
                                    {
                                        if (templateArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                            }


                            if (templateArray[iPtr + 1].ToUpper().StartsWith("IMAGETHUMB:"))
                            {

                                if (objCategory.Image != "")
                                {

                                    if (objCategory.Image.Split('=').Length == 2)
                                    {
                                        if (Numeric.IsNumeric(objCategory.Image.Split('=')[1]))
                                        {
                                            DotNetNuke.Services.FileSystem.FileInfo objFile = (DotNetNuke.Services.FileSystem.FileInfo)FileManager.Instance.GetFile(Convert.ToInt32(objCategory.Image.Split('=')[1]));

                                            if (objFile != null)
                                            {

                                                string val = templateArray[iPtr + 1].Substring(11, templateArray[iPtr + 1].Length - 11);
                                                if (val.IndexOf(':') == -1)
                                                {
                                                    int length = Convert.ToInt32(val);

                                                    Image objImage = new Image();
                                                    if (ArticleSettings.ImageThumbnailType == ThumbnailType.Proportion)
                                                    {
                                                        objImage.ImageUrl = Page.ResolveUrl("~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + length.ToString() + "&Height=" + length.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory + objFile.Folder) + "&FileName=" + Server.UrlEncode(objFile.FileName) + "&PortalID=" + PortalId.ToString() + "&q=1");
                                                    }
                                                    else
                                                    {
                                                        objImage.ImageUrl = Page.ResolveUrl("~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + length.ToString() + "&Height=" + length.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory + objFile.Folder) + "&FileName=" + Server.UrlEncode(objFile.FileName) + "&PortalID=" + PortalId.ToString() + "&q=1&s=1");
                                                    }
                                                    objImage.EnableViewState = false;
                                                    objPlaceHolder.Add(objImage);

                                                }
                                                else
                                                {

                                                    string[] arr = val.Split(':');

                                                    int width = Convert.ToInt32(val.Split(':')[0]);
                                                    int height = Convert.ToInt32(val.Split(':')[1]);

                                                    Image objImage = new Image();
                                                    if (ArticleSettings.ImageThumbnailType == ThumbnailType.Proportion)
                                                    {
                                                        objImage.ImageUrl = Page.ResolveUrl("~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory + objFile.Folder) + "&FileName=" + Server.UrlEncode(objFile.FileName) + "&PortalID=" + PortalId.ToString() + "&q=1");
                                                    }
                                                    else
                                                    {
                                                        objImage.ImageUrl = Page.ResolveUrl("~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory + objFile.Folder) + "&FileName=" + Server.UrlEncode(objFile.FileName) + "&PortalID=" + PortalId.ToString() + "&q=1&s=1");
                                                    }
                                                    objImage.EnableViewState = false;
                                                    objPlaceHolder.Add(objImage);

                                                }

                                            }
                                        }
                                    }
                                }

                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("ISDEPTHABS:"))
                            {
                                string depth = templateArray[iPtr + 1].Substring(11, templateArray[iPtr + 1].Length - 11);
                                bool isDepth = false;
                                foreach (string item in depth.Split(','))
                                {
                                    if (Numeric.IsNumeric(item))
                                    {
                                        foreach (CategoryInfo objCategoryItem in _objCategoriesAll)
                                        {
                                            if (objCategoryItem.CategoryID == objCategory.CategoryID)
                                            {
                                                if (objCategoryItem.Level == Convert.ToInt32(item))
                                                {
                                                    isDepth = true;
                                                }
                                            }
                                        }
                                    }
                                }

                                string endToken = "/" + templateArray[iPtr + 1];
                                if (!isDepth)
                                {
                                    while (iPtr < templateArray.Length - 1)
                                    {
                                        if (templateArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("ISDEPTHREL:"))
                            {
                                string depth = templateArray[iPtr + 1].Substring(11, templateArray[iPtr + 1].Length - 11);
                                bool isDepth = false;
                                foreach (string item in depth.Split(','))
                                {
                                    if (Numeric.IsNumeric(item))
                                    {
                                        foreach (CategoryInfo objCategoryItem in _objCategoriesAll)
                                        {
                                            if (objCategoryItem.CategoryID == objCategory.CategoryID)
                                            {
                                                if ((objCategoryItem.Level - level) == Convert.ToInt32(item))
                                                {
                                                    isDepth = true;
                                                }
                                            }
                                        }
                                    }
                                }

                                string endToken = "/" + templateArray[iPtr + 1];
                                if (isDepth == false)
                                {
                                    while (iPtr < templateArray.Length - 1)
                                    {
                                        if (templateArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("ISNOTDEPTHABS:"))
                            {
                                string depth = templateArray[iPtr + 1].Substring(14, templateArray[iPtr + 1].Length - 14);
                                bool isDepth = false;
                                foreach (string item in depth.Split(','))
                                {
                                    if (Numeric.IsNumeric(item))
                                    {
                                        foreach (CategoryInfo objCategoryItem in _objCategoriesAll)
                                        {
                                            if (objCategoryItem.CategoryID == objCategory.CategoryID)
                                            {
                                                if (objCategoryItem.Level == Convert.ToInt32(item))
                                                {
                                                    isDepth = true;
                                                }
                                            }
                                        }
                                    }
                                }

                                string endToken = "/" + templateArray[iPtr + 1];
                                if (isDepth == true)
                                {
                                    while (iPtr < templateArray.Length - 1)
                                    {
                                        if (templateArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("ISNOTDEPTHREL:"))
                            {
                                string depth = templateArray[iPtr + 1].Substring(14, templateArray[iPtr + 1].Length - 14);
                                bool isDepth = false;
                                foreach (string item in depth.Split(','))
                                {
                                    if (Numeric.IsNumeric(item))
                                    {
                                        foreach (CategoryInfo objCategoryItem in _objCategoriesAll)
                                        {
                                            if (objCategoryItem.CategoryID == objCategory.CategoryID)
                                            {
                                                if ((objCategoryItem.Level - level) == Convert.ToInt32(item))
                                                {
                                                    isDepth = true;
                                                }
                                            }
                                        }
                                    }
                                }

                                string endToken = "/" + templateArray[iPtr + 1];
                                if (isDepth == true)
                                {
                                    while (iPtr < templateArray.Length - 1)
                                    {
                                        if (templateArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                            }
                            break;
                    }
                }
            }

        }

        private void ProcessCategory(CategoryInfo objCategory, ControlCollection objPlaceHolder)
        {

            LayoutController _layoutController = new LayoutController(this);
            LayoutInfo layoutCategory = LayoutController.GetLayout(this, LayoutType.Category_Html);
            LayoutInfo layoutCategoryChild = LayoutController.GetLayout(this, LayoutType.Category_Child_Html);
            string[] templateArray = layoutCategory.Tokens;

            CategoryController objCategoryController = new CategoryController();
            CategoryInfo objParentCategory = null;

            if (objCategory.ParentID != Null.NullInteger)
            {
                objParentCategory = objCategoryController.GetCategory(objCategory.ParentID, ModuleId);
            }

            List<CategoryInfo> objCategoriesChildren = objCategoryController.GetCategories(this.ModuleId, objCategory.CategoryID);

            for (int iPtr = 0; iPtr < templateArray.Length; iPtr += 2)
            {

                objPlaceHolder.Add(new LiteralControl(_layoutController.ProcessImages(templateArray[iPtr].ToString())));

                Literal objLiteral;

                if (iPtr < templateArray.Length - 1)
                {
                    switch (templateArray[iPtr + 1])
                    {

                        case "ARTICLECOUNT":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                            objLiteral.Text = objCategory.NumberOfArticles.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "CATEGORYLABEL":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                            string entriesFrom = Localization.GetString("CategoryEntries", LocalResourceFile);
                            if (entriesFrom.Contains("{0}"))
                            {
                                objLiteral.Text = String.Format(entriesFrom, objCategory.Name);
                            }
                            else
                            {
                                objLiteral.Text = objCategory.Name;
                            }
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "CATEGORYID":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                            objLiteral.Text = objCategory.CategoryID.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "CHILDCATEGORIES":
                            if (objCategoriesChildren.Count > 0)
                            {
                                foreach (CategoryInfo objCategoryItem in _objCategoriesAll)
                                {
                                    if (objCategoryItem.CategoryID == objCategory.CategoryID)
                                    {
                                        int i = 0;
                                        foreach (CategoryInfo objCategoryChild in objCategoriesChildren)
                                        {
                                            ProcessCategoryChild(objCategoryChild, objPlaceHolder, "ChildCategory-" + i.ToString() + "-" + iPtr.ToString(), layoutCategoryChild.Tokens, objCategoryItem.Level);
                                            i = i + 1;
                                        }
                                    }
                                }
                            }
                            break;
                        case "DESCRIPTION":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                            objLiteral.Text = Server.HtmlDecode(objCategory.Description);
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "HASCHILDCATEGORIES":
                            if (objCategoriesChildren.Count == 0)
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/HASCHILDCATEGORIES")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASCHILDCATEGORIES":
                            // Do null
                            break;
                        case "HASNOCHILDCATEGORIES":
                            if (objCategoriesChildren.Count > 0)
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/HASNOCHILDCATEGORIES")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASNOCHILDCATEGORIES":
                            // Do null
                            break;
                        case "HASNOPARENT":
                            if (objParentCategory != null)
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/HASNOPARENT")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASNOPARENT":
                            // Do null
                            break;
                        case "HASPARENT":
                            if (objParentCategory == null)
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/HASPARENT")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASPARENT":
                            // Do null
                            break;
                        case "HASIMAGE":
                            if (objCategory.Image == "")
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/HASIMAGE")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASIMAGE":
                            // Do null
                            break;
                        case "HASNOIMAGE":
                            if (objCategory.Image != "")
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/HASNOIMAGE")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASNOIMAGE":
                            // Do null
                            break;
                        case "IMAGE":
                            if (objCategory.Image != "")
                            {

                                if (objCategory.Image.Split('=').Length == 2)
                                {
                                    if (Numeric.IsNumeric(objCategory.Image.Split('=')[1]))
                                    {
                                        DotNetNuke.Services.FileSystem.FileInfo objFile = (DotNetNuke.Services.FileSystem.FileInfo)FileManager.Instance.GetFile(Convert.ToInt32(objCategory.Image.Split('=')[1]));

                                        if (objFile != null)
                                        {
                                            Image objImage = new Image();
                                            objImage.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                                            objImage.ImageUrl = PortalSettings.HomeDirectory + objFile.Folder + objFile.FileName;
                                            objPlaceHolder.Add(objImage);
                                        }
                                    }

                                }

                            }
                            break;
                        case "IMAGELINK":
                            if (objCategory.Image != "")
                            {

                                if (objCategory.Image.Split('=').Length == 2)
                                {
                                    if (Numeric.IsNumeric(objCategory.Image.Split('=')[1]))
                                    {
                                        DotNetNuke.Services.FileSystem.FileInfo objFile = (DotNetNuke.Services.FileSystem.FileInfo)FileManager.Instance.GetFile(Convert.ToInt32(objCategory.Image.Split('=')[1]));

                                        if (objFile != null)
                                        {
                                            objLiteral = new Literal();
                                            objLiteral.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                                            objLiteral.Text = PortalSettings.HomeDirectory + objFile.Folder + objFile.FileName;
                                            objPlaceHolder.Add(objLiteral);
                                        }
                                    }

                                }

                            }
                            break;
                        case "LINK":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                            objLiteral.Text = Common.GetCategoryLink(TabId, ModuleId, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings);
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "METADESCRIPTION":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                            objLiteral.Text = objCategory.MetaDescription.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "NAME":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                            objLiteral.Text = objCategory.Name;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "PARENTDESCRIPTION":
                            if (objParentCategory != null)
                            {
                                objLiteral = new Literal();
                                objLiteral.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                                objLiteral.Text = Server.HtmlDecode(objParentCategory.Description);
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "PARENTLINK":
                            if (objParentCategory != null)
                            {
                                objLiteral = new Literal();
                                objLiteral.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                                objLiteral.Text = Common.GetCategoryLink(TabId, ModuleId, objParentCategory.CategoryID.ToString(), objParentCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings);
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "PARENTNAME":
                            if (objParentCategory != null)
                            {
                                objLiteral = new Literal();
                                objLiteral.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                                objLiteral.Text = objParentCategory.Name;
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "RSSLINK":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                            objLiteral.Text = Page.ResolveUrl("~/DesktopModules/GcDesign-NewsArticles/RSS.aspx?TabID=" + TabId.ToString() + "&amp;ModuleID=" + ModuleId.ToString() + "&amp;CategoryID=" + objCategory.CategoryID.ToString());
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "VIEWS":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                            objLiteral.Text = objCategory.NumberOfViews.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        default:
                            if (templateArray[iPtr + 1].ToUpper().StartsWith("CHILDCATEGORIES:"))
                            {
                                string count = templateArray[iPtr + 1].Substring(16, templateArray[iPtr + 1].Length - 16);
                                if (Numeric.IsNumeric(count))
                                {
                                    int relativeLevel = Null.NullInteger;
                                    foreach (CategoryInfo objCategoryItem in _objCategoriesAll)
                                    {
                                        if (objCategoryItem.CategoryID == objCategory.CategoryID)
                                        {
                                            relativeLevel = objCategoryItem.Level;
                                        }
                                    }

                                    int level = Null.NullInteger;
                                    int i = 0;
                                    foreach (CategoryInfo objCategoryItem in _objCategoriesAll)
                                    {
                                        if (level != Null.NullInteger && objCategoryItem.Level <= level)
                                        {
                                            break;
                                        }
                                        if (objCategoryItem.CategoryID == objCategory.CategoryID)
                                        {
                                            level = objCategoryItem.Level;
                                        }
                                        else
                                        {
                                            if (level != Null.NullInteger)
                                            {
                                                if (objCategoryItem.Level > level && ((objCategoryItem.Level - relativeLevel) <= Convert.ToInt32(count)))
                                                {
                                                    ProcessCategoryChild(objCategoryItem, objPlaceHolder, "ChildCategory" + i.ToString() + "-" + iPtr.ToString(), layoutCategoryChild.Tokens, relativeLevel);
                                                    i = i + 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("DESCRIPTION:"))
                            {

                                string description = Server.HtmlDecode(objCategory.Description);
                                if (Numeric.IsNumeric(templateArray[iPtr + 1].Substring(12, templateArray[iPtr + 1].Length - 12)))
                                {
                                    int length = Convert.ToInt32(templateArray[iPtr + 1].Substring(12, templateArray[iPtr + 1].Length - 12));
                                    if (StripHtml(Server.HtmlDecode(objCategory.Description)).TrimStart().Length > length)
                                    {
                                        description = StripHtml(Server.HtmlDecode(objCategory.Description)).TrimStart().Substring(0, length) + "...";
                                    }
                                    else
                                    {
                                        description = StripHtml(Server.HtmlDecode(objCategory.Description)).TrimStart().Substring(0, StripHtml(Server.HtmlDecode(objCategory.Description)).TrimStart().Length);
                                    }
                                }

                                objLiteral = new Literal();
                                objLiteral.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                                objLiteral.Text = description;
                                objPlaceHolder.Add(objLiteral);
                                break;
                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("ISDEPTHABS:"))
                            {
                                string depth = templateArray[iPtr + 1].Substring(11, templateArray[iPtr + 1].Length - 11);
                                bool isDepth = false;
                                foreach (string item in depth.Split(','))
                                {
                                    if (Numeric.IsNumeric(item))
                                    {
                                        foreach (CategoryInfo objCategoryItem in _objCategoriesAll)
                                        {
                                            if (objCategoryItem.CategoryID == objCategory.CategoryID)
                                            {
                                                if (objCategoryItem.Level == Convert.ToInt32(item))
                                                {
                                                    isDepth = true;
                                                }
                                            }
                                        }
                                    }
                                }

                                string endToken = "/" + templateArray[iPtr + 1];
                                if (isDepth == false)
                                {
                                    while (iPtr < templateArray.Length - 1)
                                    {
                                        if (templateArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("ISNOTDEPTHABS:"))
                            {
                                string depth = templateArray[iPtr + 1].Substring(14, templateArray[iPtr + 1].Length - 14);
                                bool isDepth = false;
                                foreach (string item in depth.Split(','))
                                {
                                    if (Numeric.IsNumeric(item))
                                    {
                                        foreach (CategoryInfo objCategoryItem in _objCategoriesAll)
                                        {
                                            if (objCategoryItem.CategoryID == objCategory.CategoryID)
                                            {
                                                if (objCategoryItem.Level == Convert.ToInt32(item))
                                                {
                                                    isDepth = true;
                                                }
                                            }
                                        }
                                    }
                                }

                                string endToken = "/" + templateArray[iPtr + 1];
                                if (isDepth == true)
                                {
                                    while (iPtr < templateArray.Length - 1)
                                    {
                                        if (templateArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("IMAGETHUMB:"))
                            {

                                if (objCategory.Image != "")
                                {

                                    if (objCategory.Image.Split('=').Length == 2)
                                    {
                                        if (Numeric.IsNumeric(objCategory.Image.Split('=')[1]))
                                        {
                                            DotNetNuke.Services.FileSystem.FileInfo objFile = (DotNetNuke.Services.FileSystem.FileInfo)FileManager.Instance.GetFile(Convert.ToInt32(objCategory.Image.Split('=')[1]));

                                            if (objFile != null)
                                            {

                                                string val = templateArray[iPtr + 1].Substring(11, templateArray[iPtr + 1].Length - 11);
                                                if (val.IndexOf(':') == -1)
                                                {
                                                    int length = Convert.ToInt32(val);

                                                    Image objImage = new Image();
                                                    if (ArticleSettings.ImageThumbnailType == ThumbnailType.Proportion)
                                                    {
                                                        objImage.ImageUrl = Page.ResolveUrl("~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + length.ToString() + "&Height=" + length.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory + objFile.Folder) + "&FileName=" + Server.UrlEncode(objFile.FileName) + "&PortalID=" + PortalId.ToString() + "&q=1");
                                                    }
                                                    else
                                                    {
                                                        objImage.ImageUrl = Page.ResolveUrl("~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + length.ToString() + "&Height=" + length.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory + objFile.Folder) + "&FileName=" + Server.UrlEncode(objFile.FileName) + "&PortalID=" + PortalId.ToString() + "&q=1&s=1");
                                                    }
                                                    objImage.EnableViewState = false;
                                                    objPlaceHolder.Add(objImage);

                                                }
                                                else
                                                {

                                                    string[] arr = val.Split(':');

                                                    int width = Convert.ToInt32(val.Split(':')[0]);
                                                    int height = Convert.ToInt32(val.Split(':')[1]);

                                                    Image objImage = new Image();
                                                    if (ArticleSettings.ImageThumbnailType == ThumbnailType.Proportion)
                                                    {
                                                        objImage.ImageUrl = Page.ResolveUrl("~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory + objFile.Folder) + "&FileName=" + Server.UrlEncode(objFile.FileName) + "&PortalID=" + PortalId.ToString() + "&q=1");
                                                    }
                                                    else
                                                    {
                                                        objImage.ImageUrl = Page.ResolveUrl("~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory + objFile.Folder) + "&FileName=" + Server.UrlEncode(objFile.FileName) + "&PortalID=" + PortalId.ToString() + "&q=1&s=1");
                                                    }
                                                    objImage.EnableViewState = false;
                                                    objPlaceHolder.Add(objImage);

                                                }

                                            }
                                        }
                                    }
                                }

                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("PARENTDESCRIPTION:"))
                            {

                                if (objParentCategory != null)
                                {
                                    string description = Server.HtmlDecode(objParentCategory.Description);
                                    if (Numeric.IsNumeric(templateArray[iPtr + 1].Substring(18, templateArray[iPtr + 1].Length - 18)))
                                    {
                                        int length = Convert.ToInt32(templateArray[iPtr + 1].Substring(18, templateArray[iPtr + 1].Length - 18));
                                        if (StripHtml(Server.HtmlDecode(objParentCategory.Description)).TrimStart().Length > length)
                                        {
                                            description = StripHtml(Server.HtmlDecode(objParentCategory.Description)).TrimStart().Substring(0, length) + "...";
                                        }
                                        else
                                        {
                                            description = StripHtml(Server.HtmlDecode(objParentCategory.Description)).TrimStart().Substring(0, StripHtml(Server.HtmlDecode(objParentCategory.Description)).TrimStart().Length);
                                        }
                                    }

                                    objLiteral = new Literal();
                                    objLiteral.ID = Globals.CreateValidID("Category-" + iPtr.ToString());
                                    objLiteral.Text = description;
                                    objPlaceHolder.Add(objLiteral);
                                    break;
                                }
                            }
                            break;

                    }
                }

            }

        }

        private void ReadQueryString()
        {

            if (Request[PARAM_CATEGORY_ID] != null && Numeric.IsNumeric(Request[PARAM_CATEGORY_ID]))
            {
                _categoryID = Convert.ToInt32(Request[PARAM_CATEGORY_ID]);
            }
            else
            {
                if (ArticleSettings.FilterSingleCategory != Null.NullInteger)
                {
                    _categoryID = ArticleSettings.FilterSingleCategory;
                }
            }


        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Init += new EventHandler(this.Page_Init);
            this.PreRender += new EventHandler(Page_PreRender);
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Init(object sender, EventArgs e)
        {

            try
            {

                ReadQueryString();

                CategoryController objCategoryController = new CategoryController();
                _objCategoriesAll = objCategoryController.GetCategoriesAll(this.ModuleId, Null.NullInteger, ArticleSettings.CategorySortType);

                if (Request["articleType"] != null && Request["articleType"].ToLower() == "categoryview")
                {

                    foreach (CategoryInfo objCategory in _objCategoriesAll)
                    {
                        if (objCategory.CategoryID == _categoryID)
                        {

                            if (ArticleSettings.FilterSingleCategory == objCategory.CategoryID)
                            {
                                break;
                            }

                            string path = "";
                            if (ArticleSettings.CategoryBreadcrumb)
                            {
                                DotNetNuke.Entities.Tabs.TabInfo objTab = new DotNetNuke.Entities.Tabs.TabInfo();
                                objTab.TabName = objCategory.Name;
                                objTab.Url = Common.GetCategoryLink(TabId, ModuleId, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings);
                                PortalSettings.ActiveTab.BreadCrumbs.Add(objTab);

                                int parentID = objCategory.ParentID;
                                int parentCount = 0;

                                while (parentID != Null.NullInteger)
                                {
                                    foreach (CategoryInfo objParentCategory in _objCategoriesAll)
                                    {
                                        if (objParentCategory.CategoryID == parentID)
                                        {
                                            if (ArticleSettings.FilterSingleCategory == objParentCategory.CategoryID)
                                            {
                                                parentID = Null.NullInteger;
                                                break;
                                            }
                                            DotNetNuke.Entities.Tabs.TabInfo objParentTab = new DotNetNuke.Entities.Tabs.TabInfo();
                                            objParentTab.TabID = 10000 + objParentCategory.CategoryID;
                                            objParentTab.TabName = objParentCategory.Name;
                                            objParentTab.Url = Common.GetCategoryLink(TabId, ModuleId, objParentCategory.CategoryID.ToString(), objParentCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings);
                                            PortalSettings.ActiveTab.BreadCrumbs.Insert(PortalSettings.ActiveTab.BreadCrumbs.Count - 1 - parentCount, objParentTab);

                                            if (path.Length == 0)
                                            {
                                                path = " > " + objParentCategory.Name;
                                            }
                                            else
                                            {
                                                path = " > " + objParentCategory.Name + path;
                                            }

                                            parentCount = parentCount + 1;
                                            parentID = objParentCategory.ParentID;
                                        }
                                    }
                                }
                            }

                            if (Request["articleType"] != null && Request["articleType"].ToLower() == "categoryview")
                            {
                                if (PortalSettings.ActiveTab.Title.Length == 0)
                                {
                                    this.BasePage.Title = Server.HtmlEncode(PortalSettings.PortalName + " > " + PortalSettings.ActiveTab.TabName + path + " > " + objCategory.Name);
                                }
                                else
                                {
                                    this.BasePage.Title = Server.HtmlEncode(PortalSettings.ActiveTab.Title + path + " > " + objCategory.Name);
                                }

                                if (objCategory.MetaTitle != "")
                                {
                                    this.BasePage.Title = objCategory.MetaTitle;
                                }
                                if (objCategory.MetaDescription != "")
                                {
                                    this.BasePage.Description = objCategory.MetaDescription;
                                }
                                if (objCategory.MetaKeywords != "")
                                {
                                    this.BasePage.KeyWords = objCategory.MetaKeywords;
                                }

                            }

                            if (ArticleSettings.IncludeInPageName)
                            {
                                HttpContext.Current.Items.Add("NA-CategoryName", objCategory.Name);
                            }

                            break;
                        }
                    }

                }

                BindCategory();

                int[] categories = { _categoryID };
                Listing1.FilterCategories = categories;
                if (ArticleSettings.FilterSingleCategory != Null.NullInteger)
                {
                    Listing1.ShowExpired = false;
                }
                else
                {
                    Listing1.ShowExpired = true;
                }
                Listing1.MaxArticles = Null.NullInteger;
                Listing1.ShowMessage = false;

                if (ArticleSettings.CategoryBreadcrumb && Request["CategoryID"] != null)
                {
                    Listing1.IncludeCategory = true;
                }

                Listing1.BindListing();
                Listing1.BindArticles = false;
                Listing1.IsIndexed = false;

                //Listing1.BindListing()

                //ucHeader1.ProcessMenu()
                //ucHeader2.ProcessMenu()

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void Page_PreRender(object sender, EventArgs e)
        {

            try
            {

                if (HttpContext.Current.Items.Contains("NA-CategoryName"))
                {
                    PortalSettings.ActiveTab.TabName = HttpContext.Current.Items["NA-CategoryName"].ToString();
                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}