using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Globalization;
using System.Threading;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Modules;

using GcDesign.NewsArticles.Components.Utility;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Host;
using DotNetNuke.Security;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Collections;

namespace GcDesign.NewsArticles
{
    public class Common
    {
        #region " Friend static Methods "

        internal static void BindEnum(ListControl objListControl, Type enumType, string resourceFile)
        {

            foreach (int value in Enum.GetValues(enumType))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(enumType, value);
                li.Text = Localization.GetString(System.Enum.GetName(enumType, value), resourceFile);
                objListControl.Items.Add(li);
            }

        }

        #endregion

        #region " public static Methods "


        public static string FormatTitle(string title)
        {

            if (title == "")
            {
                return "Default.aspx";
            }

            string returnTitle = OnlyAlphaNumericChars(title, TitleReplacementType.Dash);
            if (returnTitle == "")
            {
                return "Default.aspx";
            }

            if (returnTitle.Replace("-", "").Replace("_", "") == "")
            {
                return "Default.aspx";
            }

            return returnTitle.Replace("--", "-") + ".aspx";

        }

        public static string FormatTitle(string title, ArticleSettings objArticleSettings)
        {

            if (title == "")
            {
                return "Default.aspx";
            }

            string returnTitle = OnlyAlphaNumericChars(title, objArticleSettings.TitleReplacement);
            if (returnTitle == "")
            {
                return "Default.aspx";
            }

            if (returnTitle.Replace("-", "").Replace("_", "") == "")
            {
                return "Default.aspx";
            }

            return returnTitle.Replace("--", "-") + ".aspx";

        }

        public static string OnlyAlphaNumericChars(string OrigString, TitleReplacementType objReplacementType)
        {

            //***********************************************************
            //INPUT:  Any String
            //OUTPUT: 输出字符串（为删除所有非字母，数字的字符）
            //EXAMPLE Debug.Print OnlyAlphaNumericChars("Hello World!")
            //output = "HelloWorld")
            //NOTES:  Not optimized for speed and will run slow on long
            //       strings.  if you plan on using long strings, consider 
            //        using alternative method of appending to output string,
            //        such as the method at
            //        http://www.freevbcode.com/ShowCode.Asp?ID=154
            //***********************************************************
            int lLen;
            string sAns = "";
            int lCtr;
            string sChar;

            OrigString = RemoveDiacritics(OrigString.Trim());

            lLen = OrigString.Length;
            for (lCtr = 1; lCtr < lLen; lCtr++)
            {
                sChar = OrigString.Substring(lCtr, 1);
                if (IsAlphaNumeric(OrigString.Substring(lCtr, 1)) || OrigString.Substring(lCtr, 1) == "-" || OrigString.Substring(lCtr, 1) == "_")
                {
                    sAns = sAns + sChar;
                }
            }

            if (objReplacementType == TitleReplacementType.Dash)
            {
                return sAns.Replace(" ", "-");
            }
            else
            {
                return sAns.Replace(" ", "-");
            }

        }

        public static string RemoveDiacritics(string s)
        {
            s = s.Normalize(System.Text.NormalizationForm.FormD);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int i;
            for (i = 0; i < s.Length; i++)
            {
                if (s[i] == Convert.ToChar(305))
                {
                    sb.Append('i');
                }
                else
                {
                    if (CharUnicodeInfo.GetUnicodeCategory(s[i]) != UnicodeCategory.NonSpacingMark)
                    {
                        sb.Append(s[i]);
                    }
                }
            }
            return sb.ToString();
        }

        public static bool IsAlphaNumeric(string sChr)
        {
            Regex reg = new Regex("[0-9A-Za-z ]");
            return reg.IsMatch(sChr);
        }

        public static string GetAdjustedUserTime(string dateString, string format, int timeZone)
        {

            DateTime dateToFormat = DateTime.Parse(dateString);

            return dateToFormat.ToString(format);

        }

        public static string GetAdjustedUserTime(ArticleSettings articleSettings, string dateString, string format)
        {

            return GetAdjustedUserTime(dateString, format, articleSettings.ServerTimeZone);

        }

        public static DateTime GetAdjustedUserTime(ArticleSettings articleSettings, DateTime objDateTime)
        {

            return objDateTime;

        }

        public static DateTime GetAdjustedServerTime(ArticleSettings articleSettings, DateTime objDateTime)
        {

            return objDateTime;

        }

        public static string GetArticleLink(ArticleInfo objArticle, DotNetNuke.Entities.Tabs.TabInfo objTab, ArticleSettings articleSettings, bool includeCategory)
        {
            return GetArticleLink(objArticle, objTab, articleSettings, includeCategory, Null.NullInteger);
        }

        public static string GetArticleLink(ArticleInfo objArticle, DotNetNuke.Entities.Tabs.TabInfo objTab, ArticleSettings articleSettings, bool includeCategory, int pageID)
        {

            if (objTab == null)
            {
                return "";
            }

            if (Host.UseFriendlyUrls)
            {
                string strURL = Globals.ApplicationURL(objTab.TabID);
                ModuleController mc = new ModuleController();
                Hashtable tabmoduleSettings = mc.GetTabModule(mc.GetModule(objArticle.ModuleID, objTab.TabID).TabModuleID).TabModuleSettings;
                if (tabmoduleSettings.Contains(ArticleConstants.News_ARTICLES_TAB_ID) && tabmoduleSettings[ArticleConstants.News_ARTICLES_TAB_ID].ToString() != "")
                {
                    strURL = Globals.ApplicationURL(Convert.ToInt32(tabmoduleSettings[ArticleConstants.News_ARTICLES_TAB_ID]));
                }
                PortalSettings settings = PortalController.Instance.GetCurrentPortalSettings();

                if (articleSettings.LaunchLinks)
                {
                    strURL = strURL + "&ctl=ArticleView";
                    strURL = strURL + "&mid=" + objArticle.ModuleID.ToString();
                    strURL = strURL + "&articleId=" + objArticle.ArticleID;
                }
                else
                {
                    if (articleSettings.UrlModeType == Components.Types.UrlModeType.Classic)
                    {
                        strURL = strURL + "&articleType=ArticleView";
                        strURL = strURL + "&articleId=" + objArticle.ArticleID;
                    }
                    else
                    {
                        strURL = strURL + "&" + articleSettings.ShortenedID + "=" + objArticle.ArticleID;
                    }
                }

                if (articleSettings.AlwaysShowPageID)
                {
                    if (pageID != Null.NullInteger)
                    {
                        strURL = strURL + "&PageID=" + pageID.ToString();
                    }
                }

                if (includeCategory)
                {
                    if (HttpContext.Current.Request["CategoryID"] != null)
                    {//""
                        strURL = strURL + "&categoryId=" + HttpContext.Current.Request["CategoryID"];
                    }
                }

                if (articleSettings.AuthorUserIDFilter)
                {
                    if (articleSettings.AuthorUserIDParam != "")
                    {
                        if (HttpContext.Current.Request[articleSettings.AuthorUserIDParam] != "")
                        {
                            strURL = strURL + "&" + articleSettings.AuthorUserIDParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUserIDParam];
                        }
                    }
                }

                if (articleSettings.AuthorUsernameFilter)
                {
                    if (articleSettings.AuthorUsernameParam != "")
                    {
                        if (HttpContext.Current.Request[articleSettings.AuthorUsernameParam] != "")
                        {
                            strURL = strURL + "&" + articleSettings.AuthorUsernameParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUsernameParam];
                        }
                    }
                }

                string title = FormatTitle(objArticle.Title, articleSettings);

                string link = Globals.FriendlyUrl(objTab, strURL, title, settings);

                if (link.ToLower().StartsWith("http://") || link.ToLower().StartsWith("https://"))
                {
                    return link;
                }
                else
                {
                    if (System.Web.HttpContext.Current.Request.Url.Port == 80)
                    {
                        return Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + link);
                    }
                    else
                    {
                        return Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port.ToString() + link);
                    }
                }
            }
            else
            {
                if (articleSettings.LaunchLinks)
                {
                    List<string> parameters = new List<string>();
                    parameters.Add("mid=" + objArticle.ModuleID.ToString());
                    parameters.Add("ctl=ArticleView");
                    parameters.Add("articleId=" + objArticle.ArticleID);

                    if (articleSettings.AlwaysShowPageID)
                    {
                        if (pageID != Null.NullInteger)
                        {
                            parameters.Add("PageID=" + pageID.ToString());
                        }
                    }

                    if (articleSettings.AuthorUserIDFilter)
                    {
                        if (articleSettings.AuthorUserIDParam != "")
                        {
                            if (HttpContext.Current.Request[articleSettings.AuthorUserIDParam] != "")
                            {
                                parameters.Add(articleSettings.AuthorUserIDParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUserIDParam]);
                            }
                        }
                    }

                    if (articleSettings.AuthorUsernameFilter)
                    {
                        if (articleSettings.AuthorUsernameParam != "")
                        {
                            if (HttpContext.Current.Request[articleSettings.AuthorUsernameParam] != "")
                            {
                                parameters.Add(articleSettings.AuthorUsernameParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUsernameParam]);
                            }
                        }
                    }

                    if (System.Web.HttpContext.Current.Request.Url.Port == 80)
                    {
                        return Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + Globals.NavigateURL(objTab.TabID, Null.NullString, parameters.ToArray()).Replace("&", "&amp;"));
                    }
                    else
                    {
                        return Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port.ToString() + Globals.NavigateURL(objTab.TabID, Null.NullString, parameters.ToArray()).Replace("&", "&amp;"));
                    }
                }
                else
                {
                    List<string> parameters = new List<string>();

                    if (articleSettings.UrlModeType == Components.Types.UrlModeType.Classic)
                    {
                        parameters.Add("articleType=ArticleView");
                        parameters.Add("articleId=" + objArticle.ArticleID);
                    }
                    else
                    {
                        parameters.Add(articleSettings.ShortenedID + "=" + objArticle.ArticleID);
                    }

                    if (articleSettings.AlwaysShowPageID)
                    {
                        if (pageID != Null.NullInteger)
                        {
                            parameters.Add("PageID=" + pageID.ToString());
                        }
                    }

                    if (articleSettings.AuthorUserIDFilter)
                    {
                        if (articleSettings.AuthorUserIDParam != "")
                        {
                            if (HttpContext.Current.Request[articleSettings.AuthorUserIDParam] != "")
                            {
                                parameters.Add(articleSettings.AuthorUserIDParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUserIDParam]);
                            }
                        }
                    }

                    if (articleSettings.AuthorUsernameFilter)
                    {
                        if (articleSettings.AuthorUsernameParam != "")
                        {
                            if (HttpContext.Current.Request[articleSettings.AuthorUsernameParam] != "")
                            {
                                parameters.Add(articleSettings.AuthorUsernameParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUsernameParam]);
                            }
                        }
                    }

                    if (System.Web.HttpContext.Current.Request.Url.Port == 80)
                    {
                        return Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + Globals.NavigateURL(objTab.TabID, Null.NullString, parameters.ToArray()));
                    }
                    else
                    {
                        return Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port.ToString() + Globals.NavigateURL(objTab.TabID, Null.NullString, parameters.ToArray()));
                    }
                }

            }

        }

        public static string GetArticleLink(ArticleInfo objArticle, DotNetNuke.Entities.Tabs.TabInfo objTab, ArticleSettings articleSettings, bool includeCategory, params string[] additionalParameters)
        {
            return GetArticleLink(objArticle, objTab, articleSettings, includeCategory, Null.NullInteger, additionalParameters);
        }

        public static string GetArticleLink(ArticleInfo objArticle, DotNetNuke.Entities.Tabs.TabInfo objTab, ArticleSettings articleSettings, bool includeCategory, int pageID, params string[] additionalParameters)
        {

            if (HostController.Instance.GetString("UseFriendlyUrls") == "Y")
            {

                string strURL = Globals.ApplicationURL(objTab.TabID);
                PortalSettings settings = PortalController.Instance.GetCurrentPortalSettings();

                if (articleSettings.LaunchLinks)
                {
                    strURL = strURL + "&ctl=ArticleView";
                    strURL = strURL + "&mid=" + objArticle.ModuleID.ToString();
                    strURL = strURL + "&articleId=" + objArticle.ArticleID;

                }
                else
                {
                    if (articleSettings.UrlModeType == Components.Types.UrlModeType.Classic)
                    {
                        strURL = strURL + "&articleType=ArticleView";
                        strURL = strURL + "&articleId=" + objArticle.ArticleID;
                    }
                    else
                    {
                        strURL = strURL + "&" + articleSettings.ShortenedID + "=" + objArticle.ArticleID;
                    }
                }

                if (articleSettings.AlwaysShowPageID)
                {
                    if (pageID != Null.NullInteger)
                    {

                        bool found = false;
                        foreach (string param in additionalParameters)
                        {
                            if (param.ToLower().StartsWith("pageid"))
                            {
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            strURL = strURL + "&PageID=" + pageID.ToString();
                        }

                    }
                }

                foreach (string param in additionalParameters)
                {
                    strURL = strURL + "&" + param;
                }

                if (articleSettings.AuthorUserIDFilter)
                {
                    if (articleSettings.AuthorUserIDParam != "")
                    {
                        if (HttpContext.Current.Request[articleSettings.AuthorUserIDParam] != "")
                        {
                            strURL = strURL + "&" + articleSettings.AuthorUserIDParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUserIDParam];
                        }
                    }
                }

                if (articleSettings.AuthorUsernameFilter)
                {
                    if (articleSettings.AuthorUsernameParam != "")
                    {
                        if (HttpContext.Current.Request[articleSettings.AuthorUsernameParam] != "")
                        {
                            strURL = strURL + "&" + articleSettings.AuthorUsernameParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUsernameParam];
                        }
                    }
                }

                return Globals.FriendlyUrl(objTab, strURL, FormatTitle(objArticle.Title, articleSettings), settings);


            }
            else
            {

                if (articleSettings.LaunchLinks)
                {
                    List<string> parameters = new List<string>();
                    bool pageFound = false;
                    for (int i = 0; i < additionalParameters.Length; i++)
                    {
                        parameters.Add(additionalParameters[i]);
                        if (additionalParameters[i].ToLower().StartsWith("pageid="))
                        {
                            pageFound = true;
                        }
                    }
                    parameters.Add("mid=" + objArticle.ModuleID.ToString());
                    parameters.Add("ctl=ArticleView");
                    parameters.Add("articleId=" + objArticle.ArticleID);

                    if (articleSettings.AlwaysShowPageID)
                    {
                        if (pageID != Null.NullInteger)
                        {
                            if (!pageFound)
                            {
                                parameters.Add("PageID=" + pageID.ToString());
                            }
                        }
                    }

                    if (articleSettings.AuthorUserIDFilter)
                    {
                        if (articleSettings.AuthorUserIDParam != "")
                        {
                            if (HttpContext.Current.Request[articleSettings.AuthorUserIDParam] != "")
                            {
                                parameters.Add(articleSettings.AuthorUserIDParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUserIDParam]);
                            }
                        }
                    }

                    if (articleSettings.AuthorUsernameFilter)
                    {
                        if (articleSettings.AuthorUsernameParam != "")
                        {
                            if (HttpContext.Current.Request[articleSettings.AuthorUsernameParam] != "")
                            {
                                parameters.Add(articleSettings.AuthorUsernameParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUsernameParam]);
                            }
                        }
                    }

                    if (System.Web.HttpContext.Current.Request.Url.Port == 80)
                    {
                        return Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + Globals.NavigateURL(objTab.TabID, Null.NullString, parameters.ToArray()));
                    }
                    else
                    {
                        return Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port.ToString() + Globals.NavigateURL(objTab.TabID, Null.NullString, parameters.ToArray()));
                    }
                }
                else
                {
                    List<string> parameters = new List<string>();
                    bool pageFound = false;
                    for (int i = 0; i < additionalParameters.Length; i++)
                    {
                        parameters.Add(additionalParameters[i]);
                        if (additionalParameters[i].ToLower().StartsWith("pageid="))
                        {
                            pageFound = true;
                        }
                    }

                    if (articleSettings.UrlModeType == Components.Types.UrlModeType.Classic)
                    {
                        parameters.Add("articleType=ArticleView");
                        parameters.Add("articleId=" + objArticle.ArticleID);
                    }
                    else
                    {
                        parameters.Add(articleSettings.ShortenedID + "=" + objArticle.ArticleID);
                    }

                    if (articleSettings.AlwaysShowPageID)
                    {
                        if (pageID != Null.NullInteger)
                        {
                            if (!pageFound)
                            {
                                parameters.Add("PageID=" + pageID.ToString());
                            }
                        }
                    }

                    if (articleSettings.AuthorUserIDFilter)
                    {
                        if (articleSettings.AuthorUserIDParam != "")
                        {
                            if (HttpContext.Current.Request[articleSettings.AuthorUserIDParam] != "")
                            {
                                parameters.Add(articleSettings.AuthorUserIDParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUserIDParam]);
                            }
                        }
                    }

                    if (articleSettings.AuthorUsernameFilter)
                    {
                        if (articleSettings.AuthorUsernameParam != "")
                        {
                            if (HttpContext.Current.Request[articleSettings.AuthorUsernameParam] != "")
                            {
                                parameters.Add(articleSettings.AuthorUsernameParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUsernameParam]);
                            }
                        }
                    }

                    if (System.Web.HttpContext.Current.Request.Url.Port == 80)
                    {
                        return Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + Globals.NavigateURL(objTab.TabID, Null.NullString, parameters.ToArray()));
                    }
                    else
                    {
                        return Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port.ToString() + Globals.NavigateURL(objTab.TabID, Null.NullString, parameters.ToArray()));
                    }
                }

            }

        }

        public static List<ModuleInfo> GetArticleModules(int portalID)
        {

            List<ModuleInfo> objModulesFound = new List<ModuleInfo>();

            DesktopModuleInfo objDesktopModuleInfo = DesktopModuleController.GetDesktopModuleByModuleName("GcDesign-NewsArticles", portalID);

            if (objDesktopModuleInfo != null)
            {
                TabCollection objTabs = TabController.Instance.GetTabsByPortal(portalID);
                foreach (DotNetNuke.Entities.Tabs.TabInfo objTab in objTabs.Values)
                {
                    if (objTab != null)
                    {
                        if (!objTab.IsDeleted)
                        {
                            ModuleController objModules = new ModuleController();
                            foreach (KeyValuePair<int, ModuleInfo> pair in objModules.GetTabModules(objTab.TabID))
                            {
                                ModuleInfo objModule = pair.Value;
                                if (!objModule.IsDeleted)
                                {
                                    if (objModule.DesktopModuleID == objDesktopModuleInfo.DesktopModuleID)
                                    {
                                        if (!objModule.IsDeleted)
                                        {
                                            string strPath = objTab.TabName;
                                            TabInfo objTabSelected = objTab;
                                            while (objTabSelected.ParentId != Null.NullInteger)
                                            {
                                                objTabSelected = TabController.Instance.GetTab(objTabSelected.ParentId, objTab.PortalID, false);
                                                if (objTabSelected == null)
                                                {
                                                    break;
                                                }
                                                strPath = objTabSelected.TabName + " -> " + strPath;//可能就是导航
                                            }

                                            objModulesFound.Add(objModule);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            return objModulesFound;

        }

        public static string GetAuthorLink(int tabID, int moduleID, int authorID, string username, bool launchLinks, ArticleSettings articleSettings)
        {

            TabInfo objTab = PortalController.Instance.GetCurrentPortalSettings().ActiveTab;
            if (tabID != PortalController.Instance.GetCurrentPortalSettings().ActiveTab.TabID)
            {
                TabController objTabController = new TabController();
                objTab = objTabController.GetTab(tabID, PortalController.Instance.GetCurrentPortalSettings().PortalId, false);
            }
            return GetAuthorLink(tabID, moduleID, authorID, username, launchLinks, objTab, articleSettings);

        }

        public static string GetAuthorLink(int tabID, int moduleID, int authorID, string username, bool launchLinks, TabInfo targetTab, ArticleSettings articleSettings)
        {

            if (Host.UseFriendlyUrls)
            {

                string strURL = Globals.ApplicationURL(tabID);

                if (launchLinks)
                {
                    strURL = strURL + "&ctl=AuthorView";
                    strURL = strURL + "&mid=" + moduleID.ToString();
                }
                else
                {
                    strURL = strURL + "&articleType=AuthorView";
                }
                strURL = strURL + "&authorID=" + authorID.ToString();

                //TODO: Remove at a later date when minimum version raised.
                if (LocaleController.Instance.GetLocales(PortalController.Instance.GetCurrentPortalSettings().PortalId).Count > 1 && LocalizationUtil.UseLanguageInUrl())
                {
                    strURL += "&language=" + Thread.CurrentThread.CurrentCulture.Name;
                }

                if (articleSettings.AuthorUserIDFilter)
                {
                    if (articleSettings.AuthorUserIDParam != "")
                    {
                        if (HttpContext.Current.Request[articleSettings.AuthorUserIDParam] != "")
                        {
                            strURL = strURL + "&" + articleSettings.AuthorUserIDParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUserIDParam];
                        }
                    }
                }

                if (articleSettings.AuthorUsernameFilter)
                {
                    if (articleSettings.AuthorUsernameParam != "")
                    {
                        if (HttpContext.Current.Request[articleSettings.AuthorUsernameParam] != "")
                        {
                            strURL = strURL + "&" + articleSettings.AuthorUsernameParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUsernameParam];
                        }
                    }
                }

                return Globals.FriendlyUrl(targetTab, strURL, Common.FormatTitle("", articleSettings), PortalController.Instance.GetCurrentPortalSettings());

            }
            else
            {

                return Common.GetModuleLink(tabID, moduleID, "AuthorView", articleSettings, "AuthorID=" + authorID.ToString());

            }

        }

        public static string GetCategoryLink(int tabID, int moduleID, string categoryID, string title, bool launchLinks, ArticleSettings articleSettings)
        {

            TabInfo objTab = PortalController.Instance.GetCurrentPortalSettings().ActiveTab;
            if (tabID != PortalController.Instance.GetCurrentPortalSettings().ActiveTab.TabID)
            {
                TabController objTabController = new TabController();
                objTab = objTabController.GetTab(tabID, PortalController.Instance.GetCurrentPortalSettings().PortalId, false);
            }
            return GetCategoryLink(tabID, moduleID, categoryID, title, launchLinks, objTab, articleSettings);

        }

        public static string GetCategoryLink(int tabID, int moduleID, string categoryID, string title, bool launchLinks, TabInfo targetTab, ArticleSettings articleSettings)
        {

            if (HostSettings.GetHostSetting("UseFriendlyUrls") == "Y")
            {

                string strURL = Globals.ApplicationURL(tabID);
                PortalSettings settings = PortalController.Instance.GetCurrentPortalSettings();

                if (launchLinks)
                {
                    strURL = strURL + "&ctl=CategoryView";
                    strURL = strURL + "&mid=" + moduleID.ToString();
                }
                else
                {
                    strURL = strURL + "&articleType=CategoryView";
                }
                strURL = strURL + "&categoryId=" + categoryID;

                // TODO: Remove at a later date when minimum version raised.
                if (LocaleController.Instance.GetLocales(PortalController.Instance.GetCurrentPortalSettings().PortalId).Count > 1 && LocalizationUtil.UseLanguageInUrl())
                {
                    strURL += "&language=" + Thread.CurrentThread.CurrentCulture.Name;
                }

                if (articleSettings.AuthorUserIDFilter)
                {
                    if (articleSettings.AuthorUserIDParam != "")
                    {
                        if (HttpContext.Current.Request[articleSettings.AuthorUserIDParam] != "")
                        {
                            strURL = strURL + "&" + articleSettings.AuthorUserIDParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUserIDParam];
                        }
                    }
                }

                if (articleSettings.AuthorUsernameFilter)
                {
                    if (articleSettings.AuthorUsernameParam != "")
                    {
                        if (HttpContext.Current.Request[articleSettings.AuthorUsernameParam] != "")
                        {
                            strURL = strURL + "&" + articleSettings.AuthorUsernameParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUsernameParam];
                        }
                    }
                }

                return Globals.FriendlyUrl(targetTab, strURL, Common.FormatTitle(title, articleSettings), settings);

            }
            else
            {

                return Common.GetModuleLink(tabID, moduleID, "CategoryView", articleSettings, "categoryId=" + categoryID);

            }

        }

        public static string GetModuleLink(int tabID, int moduleID, string ctl, ArticleSettings articleSettings, params string[] additionalParameters)
        {

            List<string> parameters = new List<string>();
            foreach (string item in additionalParameters)
            {
                parameters.Add(item);
            }

            if (articleSettings.AuthorUserIDFilter)
            {
                if (articleSettings.AuthorUserIDParam != "")
                {
                    if (HttpContext.Current.Request[articleSettings.AuthorUserIDParam] != "")
                    {
                        parameters.Add(articleSettings.AuthorUserIDParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUserIDParam]);
                    }
                }
            }

            if (articleSettings.AuthorUsernameFilter)
            {
                if (articleSettings.AuthorUsernameParam != "")
                {
                    if (HttpContext.Current.Request[articleSettings.AuthorUsernameParam] != "")
                    {
                        parameters.Add(articleSettings.AuthorUsernameParam + "=" + HttpContext.Current.Request[articleSettings.AuthorUsernameParam]);
                    }
                }
            }

            string link = "";

            if (ctl != "")
            {

                if (articleSettings.LaunchLinks)
                {
                    parameters.Insert(0, "mid=" + moduleID.ToString());
                    if (ctl.ToLower() == "submitnews")
                    {
                        link = Globals.NavigateURL(tabID, "edit", parameters.ToArray());
                    }
                    else
                    {
                        link = Globals.NavigateURL(tabID, ctl, parameters.ToArray());
                    }
                }
                else
                {
                    parameters.Insert(0, "articleType=" + ctl);
                    link = Globals.NavigateURL(tabID, Null.NullString, parameters.ToArray());
                }

            }
            else
            {

                link = Globals.NavigateURL(tabID, Null.NullString, parameters.ToArray());

            }

            if (!(link.ToLower().StartsWith("http://") || link.ToLower().StartsWith("https://")))
            {

                if (System.Web.HttpContext.Current.Request.Url.Port == 80)
                {
                    link = Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + link);
                }
                else
                {
                    link = Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port.ToString() + link);
                }

            }

            return link;

        }

        public static void NotifyAuthor(ArticleInfo objArticle, Hashtable settings, int moduleID, DotNetNuke.Entities.Tabs.TabInfo objTab, int portalID, ArticleSettings articleSettings)
        {

            if (settings.Contains(ArticleConstants.NOTIFY_APPROVAL_SETTING))
            {
                if (Convert.ToBoolean(settings[ArticleConstants.NOTIFY_APPROVAL_SETTING]))
                {

                    UserController objUserController = new UserController();
                    UserInfo objUser = objUserController.GetUser(portalID, objArticle.AuthorID);

                    EmailTemplateController objEmailTemplateController = new EmailTemplateController();
                    if (objUser != null)
                    {
                        objEmailTemplateController.SendFormattedEmail(moduleID, Common.GetArticleLink(objArticle, objTab, articleSettings, false), objArticle, EmailTemplateType.ArticleApproved, objUser.Email, articleSettings);
                    }

                }
            }

        }

        public static string HtmlEncode(string text)
        {

            return System.Web.HttpUtility.HtmlEncode(text);

        }

        public static string HtmlDecode(string text)
        {

            return System.Web.HttpUtility.HtmlDecode(text);

        }

        public static string ProcessPostTokens(string content, TabInfo objTab, ArticleSettings objArticleSettings)
        {

            if (!objArticleSettings.ProcessPostTokens)
            {
                return content;
            }

            string delimStr = "[]";
            char[] delimiter = delimStr.ToCharArray();

            string[] layoutArray = content.Split(delimiter);
            string formattedContent = "";

            for (int iPtr = 0; iPtr < layoutArray.Length; iPtr = iPtr + 2)
            {

                formattedContent += layoutArray[iPtr].ToString();

                if (iPtr < layoutArray.Length - 1)
                {
                    switch (layoutArray[iPtr + 1])
                    {
                        default:
                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("ARTICLELINK:"))
                            {
                                if (Numeric.IsNumeric(layoutArray[iPtr + 1].Substring(12, layoutArray[iPtr + 1].Length - 12)))
                                {
                                    int articleID = Convert.ToInt32(layoutArray[iPtr + 1].Substring(12, layoutArray[iPtr + 1].Length - 12));

                                    ArticleController objArticleController = new ArticleController();
                                    ArticleInfo objArticle = objArticleController.GetArticle(articleID);

                                    if (objArticle != null)
                                    {
                                        string link = Common.GetArticleLink(objArticle, objTab, objArticleSettings, false);
                                        formattedContent += "<a href=\"" + link + "\" rel=\"nofollow\">" + objArticle.Title + "</a>";
                                    }
                                }
                                break;
                            }

                            formattedContent += "[" + layoutArray[iPtr + 1] + "]";
                            break;
                    }
                }

            }

            return formattedContent;

        }

        public static string StripTags(string HTML)
        {
            if (HTML == null || HTML.Trim() == "")
            {
                return "";
            }
            string pattern = "<(.|\n)*?>";
            return Regex.Replace(HTML, pattern, String.Empty);
        }

        #endregion
    }
}