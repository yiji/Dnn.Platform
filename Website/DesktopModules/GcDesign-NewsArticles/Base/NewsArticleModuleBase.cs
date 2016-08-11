using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using System.Collections;
using DotNetNuke.Security.Permissions;

namespace GcDesign.NewsArticles.Base
{
    public class NewsArticleModuleBase : PortalModuleBase
    {
        #region " Private Members "

        private ArticleSettings _articleSettings;

        #endregion

        #region " Public Properties "

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DotNetNuke.Framework.CDefault BasePage
        {
            get
            {
                return (DotNetNuke.Framework.CDefault)Page;
            }
        }

        /// <summary>
        /// 文章设置 module设置 站点设置 tabmodule设置
        /// </summary>
        public ArticleSettings ArticleSettings
        {
            get
            {
                if (_articleSettings == null)
                {
                    try
                    {
                        _articleSettings = new ArticleSettings(Settings, PortalSettings, ModuleConfiguration);
                    }
                    catch
                    {
                        ModuleController objModuleController = new ModuleController();

                        foreach (DictionaryEntry item in ModuleConfiguration.TabModuleSettings)
                        {
                            if (ModuleConfiguration.ModuleSettings.ContainsKey(item.Key) == false)
                            {
                                ModuleConfiguration.ModuleSettings.Add(item.Key, item.Value);
                            }
                        }

                        _articleSettings = new ArticleSettings(ModuleConfiguration.ModuleSettings, PortalSettings, ModuleConfiguration);
                        objModuleController.UpdateModuleSetting(ModuleId, "ResetArticleSettings", "true");
                    }
                }
                return _articleSettings;
            }
        }

        public string ModuleKey
        {
            get
            {
                return "NewsArticles-" + TabModuleId;
            }
        }

        #endregion

        #region " protected Methods "

        protected string EditArticleUrl(string ctl)
        {

            if (ArticleSettings.AuthorUserIDFilter)
            {
                if (ArticleSettings.AuthorUserIDParam != "")
                {
                    if (HttpContext.Current.Request[ArticleSettings.AuthorUserIDParam] != "")
                    {
                        return EditUrl(ArticleSettings.AuthorUserIDParam, HttpContext.Current.Request[ArticleSettings.AuthorUserIDParam], ctl);
                    }
                }
            }

            if (ArticleSettings.AuthorUsernameFilter)
            {
                if (ArticleSettings.AuthorUsernameParam != "")
                {
                    if (HttpContext.Current.Request[ArticleSettings.AuthorUsernameParam] != "")
                    {
                        return EditUrl(ArticleSettings.AuthorUsernameParam, HttpContext.Current.Request[ArticleSettings.AuthorUsernameParam], ctl);
                    }
                }
            }

            return EditUrl(ctl);

        }

        protected string EditArticleUrl(string ctl, string[] ParamArray)
        {

            List<string> parameters = new List<string>();

            parameters.Add("mid=" + ModuleId.ToString());
            parameters.AddRange(ParamArray);

            if (ArticleSettings.AuthorUserIDFilter)
            {
                if (ArticleSettings.AuthorUserIDParam != "")
                {
                    if (HttpContext.Current.Request[ArticleSettings.AuthorUserIDParam] != "")
                    {
                        parameters.Add(ArticleSettings.AuthorUserIDParam + "=" + HttpContext.Current.Request[ArticleSettings.AuthorUserIDParam]);
                    }
                }
            }

            if (ArticleSettings.AuthorUsernameFilter)
            {
                if (ArticleSettings.AuthorUsernameParam != "")
                {
                    if (HttpContext.Current.Request[ArticleSettings.AuthorUsernameParam] != "")
                    {
                        parameters.Add(ArticleSettings.AuthorUsernameParam + "=" + HttpContext.Current.Request[ArticleSettings.AuthorUsernameParam]);
                    }
                }
            }

            return Globals.NavigateURL(TabId, ctl, parameters.ToArray());

        }

        public void LoadStyleSheet()
        {


            Control objCSS = BasePage.FindControl("CSS");

            if (objCSS != null)
            {
                HtmlLink objLink = new HtmlLink();
                objLink.ID = "Template_" + ModuleId.ToString();
                objLink.Attributes["rel"] = "stylesheet";
                objLink.Attributes["type"] = "text/css";
                objLink.Href = Page.ResolveUrl("~/DesktopModules/GcDesign-NewsArticles/Templates/" + _articleSettings.Template + "/Template.css");

                objCSS.Controls.AddAt(0, objLink);
            }

        }

        public string GetSkinAttribute(XmlDocument xDoc, string tag, string attrib, string defaultValue)
        {
            string retValue = defaultValue;
            XmlNode xmlSkinAttributeRoot = xDoc.SelectSingleNode("descendant::Object[Token='[" + tag + "]']");
            // 如果发现了token
            if (xmlSkinAttributeRoot != null)
            {
                // process each token attribute
                foreach (XmlNode xmlSkinAttribute in xmlSkinAttributeRoot.SelectNodes(".//Settings/Setting"))
                {
                    if (xmlSkinAttribute.SelectSingleNode("Value").InnerText != "")
                    {
                        // append the formatted attribute to the inner contents of the control statement
                        if (xmlSkinAttribute.SelectSingleNode("Name").InnerText == attrib)
                        {
                            retValue = xmlSkinAttribute.SelectSingleNode("Value").InnerText;
                        }
                    }
                }
            }
            return retValue;
        }


        protected string FormatImageUrl(string imageUrlResolved)
        {

            return PortalSettings.HomeDirectory + imageUrlResolved;

        }

        protected bool IsRated(ArticleInfo objArticle)
        {

            if (objArticle.Rating == Null.NullDouble)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        protected bool IsRated(object objDataItem)
        {

            ArticleInfo objArticle = (ArticleInfo)objDataItem;

            return IsRated(objArticle);

        }

        protected string GetRatingImage(ArticleInfo objArticle)
        {

            if (objArticle.Rating == Null.NullDouble)
            {
                return ResolveUrl(@"Images\Rating\stars-0-0.gif");
            }
            else
            {

                switch (RoundToUnit(objArticle.Rating, 0.5, false).ToString())
                {

                    case "1":
                        return ResolveUrl(@"Images\Rating\stars-1-0.gif");

                    case "1.5":
                        return ResolveUrl(@"Images\Rating\stars-1-5.gif");

                    case "2":
                        return ResolveUrl(@"Images\Rating\stars-2-0.gif");

                    case "2.5":
                        return ResolveUrl(@"Images\Rating\stars-2-5.gif");

                    case "3":
                        return ResolveUrl(@"Images\Rating\stars-3-0.gif");

                    case "3.5":
                        return ResolveUrl(@"Images\Rating\stars-3-5.gif");

                    case "4":
                        return ResolveUrl(@"Images\Rating\stars-4-0.gif");

                    case "4.5":
                        return ResolveUrl(@"Images\Rating\stars-4-5.gif");

                    case "5":
                        return ResolveUrl(@"Images\Rating\stars-5-0.gif");

                }

                return ResolveUrl(@"Images\Rating\stars-0-0.gif");

            }

        }

        protected string StripHtml(string html)
        {

            const string pattern = "<(.|\n)*?>";
            return Regex.Replace(html, pattern, string.Empty);

        }


        private double RoundToUnit(double d, double unit, bool roundDown)
        {

            if (roundDown)
            {
                return Math.Round(Math.Round((d / unit) - 0.5, 0) * unit, 2);
            }
            else
            {
                return Math.Round(Math.Round((d / unit) + 0.5, 0) * unit, 2);
            }

        }

        protected string GetRatingImage(object objDataItem)
        {

            ArticleInfo objArticle = (ArticleInfo)objDataItem;

            return GetRatingImage(objArticle);

        }

        protected string GetUserName(object dataItem)
        {

            ArticleInfo objArticle = (ArticleInfo)dataItem;

            if (objArticle != null)
            {

                switch (ArticleSettings.DisplayMode)
                {

                    case DisplayType.UserName:
                        return objArticle.AuthorUserName;

                    case DisplayType.FirstName:
                        return objArticle.AuthorFirstName;

                    case DisplayType.LastName:
                        return objArticle.AuthorLastName;

                    case DisplayType.FullName:
                        return objArticle.AuthorFullName;

                }

            }

            return Null.NullString;

        }

        protected string GetUserName(object dataItem, int type)
        {

            ArticleInfo objArticle = (ArticleInfo)dataItem;

            if (objArticle != null)
            {

                switch (type)
                {

                    case 1: //Last Updated

                        switch (ArticleSettings.DisplayMode)
                        {

                            case DisplayType.UserName:
                                return objArticle.LastUpdateUserName;

                            case DisplayType.FirstName:
                                return objArticle.LastUpdateFirstName;

                            case DisplayType.LastName:
                                return objArticle.LastUpdateLastName;

                            case DisplayType.FullName:
                                return objArticle.LastUpdateDisplayName;

                        }
                        break;
                    default:

                        switch (ArticleSettings.DisplayMode)
                        {

                            case DisplayType.UserName:
                                return objArticle.AuthorUserName;

                            case DisplayType.FirstName:
                                return objArticle.AuthorFirstName;

                            case DisplayType.LastName:
                                return objArticle.AuthorLastName;

                            case DisplayType.FullName:
                                return objArticle.AuthorDisplayName;

                        }
                        break;

                }

            }

            return Null.NullString;

        }

        protected bool HasEditRights(int articleId, int moduleID, int tabID)
        {

            // Unauthenticated User

            if (Request.IsAuthenticated == false)
            {

                return false;

            }

            ModuleController objModuleController = new ModuleController();

            ModuleInfo objModule = objModuleController.GetModule(moduleID, tabID);

            if (objModule != null)
            {

                // Admin of Module


                if (ModulePermissionController.HasModulePermission(ModuleConfiguration.ModulePermissions, "edit"))
                {

                    return true;

                }

            }


            // Approver

            if (ArticleSettings.IsApprover)
            {
                return true;
            }

            // Submitter of New Article

            if (articleId == Null.NullInteger && ArticleSettings.IsSubmitter)
            {
                return true;
            }

            // Owner of Article

            ArticleController objArticleController = new ArticleController();
            ArticleInfo objArticle = objArticleController.GetArticle(articleId);
            if (objArticle != null)
            {
                if (objArticle.AuthorID == UserId && (objArticle.Status == StatusType.Draft || ArticleSettings.IsAutoApprover))
                {
                    return true;
                }
            }

            return false;

        }

        public string GetSharedResource(string key)
        {

            string path = "~/DesktopModules/GcDesign-NewsArticles/" + Localization.LocalResourceDirectory + "/" + Localization.LocalSharedResourceFile;
            return Localization.GetString(key, path);

        }

        #endregion

        #region " Shadowed Methods "

        public new string ResolveUrl(string url)
        {

            return base.ResolveUrl(url).Replace(" ", "%20");

        }

        #endregion
    }
}