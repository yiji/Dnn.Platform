using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using GcDesign.NewsArticles.Components.Common;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using DotNetNuke.Security;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Portals;
using GcDesign.NewsArticles.Components.CustomFields;

using GcDesign.NewsArticles.Base;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Client;
using DotNetNuke.Services.Cache;
using System.Collections;
using System.Text;
using System.Security.Cryptography;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;

namespace GcDesign.NewsArticles
{
    public class LayoutController
    {
        #region " Constructors "

        public LayoutController(PortalSettings portalSettings, ArticleSettings articleSettings, ModuleInfo objModule, Page objPage)
        {

            _portalSettings = portalSettings;
            _articleSettings = articleSettings;
            _articleModule = objModule;
            _page = objPage;

        }

        public LayoutController(NewsArticleModuleBase moduleContext)
        {

            _portalSettings = moduleContext.PortalSettings;
            _articleSettings = moduleContext.ArticleSettings;
            _articleModule = moduleContext.ModuleContext.Configuration;
            _page = moduleContext.Page;

        }

        public LayoutController(NewsArticleModuleBase moduleContext, int pageId)
        {

            _portalSettings = moduleContext.PortalSettings;
            _articleSettings = moduleContext.ArticleSettings;
            _articleModule = moduleContext.ModuleContext.Configuration;
            _page = moduleContext.Page;
            _pageId = pageId;

        }

        #endregion

        #region " private Members "

        private PortalSettings _portalSettings;
        private ArticleSettings _articleSettings;
        private ModuleInfo _articleModule;
        private Page _page;

        private int _pageId = Null.NullInteger;
        private DotNetNuke.Entities.Tabs.TabInfo _tab;

        private ArrayList _objPages;
        private List<ArticleInfo> _objRelatedArticles;

        UserInfo _author;

        bool _includeCategory = false;

        ProfilePropertyDefinitionCollection _profileProperties;

        #endregion

        #region " private Properties "

        private PortalSettings PortalSettings
        {
            get
            {
                return _portalSettings;
            }
        }

        private ArticleSettings ArticleSettings
        {
            get
            {
                return _articleSettings;
            }
        }

        private ModuleInfo ArticleModule
        {
            get
            {
                return _articleModule;
            }
        }

        private Page Page
        {
            get
            {
                return _page;
            }
        }

        private ArrayList Pages(int articleId)
        {

            if (_objPages == null)
            {
                PageController objPageController = new PageController();
                _objPages = objPageController.GetPageList(articleId);
            }
            return _objPages;

        }

        private HttpServerUtility Server
        {
            get
            {
                return _page.Server;
            }
        }

        private HttpRequest Request
        {
            get
            {
                return _page.Request;
            }
        }

        private int UserId
        {
            get
            {
                return UserController.Instance.GetCurrentUserInfo().UserID;
            }
        }

        private DotNetNuke.Entities.Tabs.TabInfo Tab
        {
            get
            {
                if (_tab == null)
                {
                    DotNetNuke.Entities.Tabs.TabController objTabController = new DotNetNuke.Entities.Tabs.TabController();
                    _tab = objTabController.GetTab(ArticleModule.TabID, PortalSettings.PortalId, false);
                    //_tab = objTabController.GetTab(112, PortalSettings.PortalId, false)
                }

                return _tab;
            }
        }

        private bool IsEditable
        {
            get
            {
                if (DotNetNuke.Security.Permissions.ModulePermissionController.CanEditModuleContent(ArticleModule))
                {
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region " public Properties "


        public bool IncludeCategory
        {
            get
            {
                return _includeCategory;
            }
            set
            {
                _includeCategory = value;
            }
        }

        #endregion

        #region " private Methods "

        private UserInfo Author(int authorID)
        {

            if (authorID == Null.NullInteger)
            {
                return null;
            }

            if (_author == null)
            {
                _author = UserController.GetUserById(PortalSettings.PortalId, authorID);
            }
            else
            {
                if (_author.UserID == authorID)
                {
                    return _author;
                }
                else
                {
                    _author = UserController.GetUserById(PortalSettings.PortalId, authorID);
                }
            }

            return _author;

        }

        public string BBCode(string strTextToReplace)
        {

            //Define regex
            Regex regExp;

            //Regex for URL tag without anchor
            regExp = new Regex(@"\[url\]([^\]]+)\[\/url\]", RegexOptions.IgnoreCase);

            Match m = regExp.Match(strTextToReplace);
            while (m.Success)
            {
                strTextToReplace = strTextToReplace.Replace(m.Value, "<a href='" + Globals.AddHTTP(m.Groups[1].Value) + "' target='_blank' >" + m.Groups[1].Value + "</a>");
                m = m.NextMatch();
            }

            //Regex for URL with anchor
            regExp = new Regex(@"\[url=([^\]]+)\]([^\]]+)\[\/url\]", RegexOptions.IgnoreCase);
            m = regExp.Match(strTextToReplace);
            while (m.Success)
            {
                strTextToReplace = strTextToReplace.Replace(m.Value, "<a href='" + Globals.AddHTTP(m.Groups[1].Value) + "' target='_blank'>" + m.Groups[2].Value + "</a>");
                m = m.NextMatch();
            }

            //Quote text
            regExp = new Regex(@"\[quote\](.+?)\[\/quote\]", RegexOptions.IgnoreCase);
            strTextToReplace = regExp.Replace(strTextToReplace, "<cite title=\"Quote\">$1</cite>");

            //Bold text
            regExp = new Regex(@"\[b\](.+?)\[\/b\]", RegexOptions.IgnoreCase);
            strTextToReplace = regExp.Replace(strTextToReplace, "<b>$1</b>");

            //Italic text
            regExp = new Regex(@"\[i\](.+?)\[\/i\]", RegexOptions.IgnoreCase);
            strTextToReplace = regExp.Replace(strTextToReplace, "<i>$1</i>");

            //Underline text
            regExp = new Regex(@"\[u\](.+?)\[\/u\]", RegexOptions.IgnoreCase);
            strTextToReplace = regExp.Replace(strTextToReplace, "<u>$1</u>");

            return strTextToReplace;

        }

        // utility function to convert a byte array into a hex string
        private string ByteArrayToString(byte[] arrInput)
        {

            StringBuilder strOutput = new StringBuilder(arrInput.Length);

            for (int i = 0; i < arrInput.Length; i++)
            {
                strOutput.Append(arrInput[i].ToString("X2"));
            }

            return strOutput.ToString().ToLower();

        }

        protected string EncodeComment(CommentInfo objComment)
        {

            if (objComment.Type == 0)
            {
                string body = objComment.Comment;
                return BBCode(body);
            }
            else
            {
                return objComment.Comment;
            }

        }

        // Returns string with binary notation of b bytes,
        // rounded to 2 decimal places , eg
        // 123="123 Bytes", 2345="2.29 KB",
        // 1234567="1.18 MB", etc
        // b : double : numeric to convert
        string Numeric2Bytes(double b)
        {
            string[] bSize = new string[9];

            bSize[0] = "Bytes";
            bSize[1] = "KB"; //Kilobytes
            bSize[2] = "MB"; //Megabytes
            bSize[3] = "GB"; //Gigabytes
            bSize[4] = "TB"; //Terabytes
            bSize[5] = "PB"; //Petabytes
            bSize[6] = "EB"; //Exabytes
            bSize[7] = "ZB"; //Zettabytes
            bSize[8] = "YB"; //Yottabytes

            b = Convert.ToDouble(b); //Make sure var is a Double (not just variant)
            for (int i = bSize.Length - 1; i >= 0; i--)
            {
                if (b >= Math.Pow(1024, i))
                {
                    return ThreeNonZeroDigits(b / Math.Pow(1024, i)) + " " + bSize[i];
                }
            }

            return "";
        }

        // return the value formatted to include at most three
        // non-zero digits and at most two digits after the
        // decimal point. Examples:
        //       1
        //       123
        //       12.3
        //       1.23
        //       0.12
        private string ThreeNonZeroDigits(double value)
        {
            if (value >= 100)
            {
                // No digits after the decimal.
                return Convert.ToInt32(value).ToString();
            }
            else if (value >= 10)
            {
                // One digit after the decimal.
                return string.Format("{0:0.0}", value / 10);
            }
            else
            {
                // Two digits after the decimal.
                return string.Format("{0:0.00}", value / 100);
            }
        }

        private string FormatImageUrl(string imageUrl)
        {

            if (imageUrl.ToLower().StartsWith("http://") || imageUrl.ToLower().StartsWith("https://"))
            {
                return imageUrl;
            }
            else
            {
                if (imageUrl.ToLower().StartsWith("fileid="))
                {

                    DotNetNuke.Services.FileSystem.FileInfo objFile = (DotNetNuke.Services.FileSystem.FileInfo)FileManager.Instance.GetFile(Convert.ToInt32(UrlUtils.GetParameterValue(imageUrl)));
                    if (objFile != null)
                    {
                        if (objFile.StorageLocation == 1)
                        {
                            // Secure Url
                            string url = Globals.LinkClick(imageUrl, ArticleModule.TabID, ArticleModule.ModuleID);

                            if (HttpContext.Current.Request.Url.Port == 80)
                            {
                                return Globals.AddHTTP(HttpContext.Current.Request.Url.Host + url);
                            }
                            else
                            {
                                return Globals.AddHTTP(HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port.ToString() + url);
                            }
                        }
                        else
                        {
                            if (HttpContext.Current.Request.Url.Port == 80)
                            {
                                return Globals.AddHTTP(HttpContext.Current.Request.Url.Host + PortalSettings.HomeDirectory + objFile.Folder + objFile.FileName);
                            }
                            else
                            {
                                return Globals.AddHTTP(HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port.ToString() + PortalSettings.HomeDirectory + objFile.Folder + objFile.FileName);
                            }
                        }
                    }
                }
            }

            return "";

        }

        public string GetArticleResource(string key)
        {

            string path = @"~/DesktopModules/GcDesign-NewsArticles/" + Localization.LocalResourceDirectory + "/ViewArticle.ascx.resx";
            return Localization.GetString(key, path);

        }

        private string GetFieldValue(CustomFieldInfo objCustomField, ArticleInfo objArticle, bool showCaption)
        {

            string value = objArticle.CustomList[objCustomField.CustomFieldID].ToString();
            if (objCustomField.FieldType == CustomFieldType.RichTextBox)
            {
                value = Server.HtmlDecode(objArticle.CustomList[objCustomField.CustomFieldID].ToString());
            }
            else
            {
                if (objCustomField.FieldType == CustomFieldType.MultiCheckBox)
                {
                    value = objArticle.CustomList[objCustomField.CustomFieldID].ToString().Replace("|", ", ");
                }
                if (objCustomField.FieldType == CustomFieldType.MultiLineTextBox)
                {
                    value = objArticle.CustomList[objCustomField.CustomFieldID].ToString().Replace("\r\n", "<br />");
                }
                if (value != "" && objCustomField.ValidationType == CustomFieldValidationType.Date)
                {
                    try
                    {
                        value = DateTime.Parse(value).ToShortDateString();
                    }
                    catch
                    {
                        value = objArticle.CustomList[objCustomField.CustomFieldID].ToString();
                    }
                }

                if (value != "" && objCustomField.ValidationType == CustomFieldValidationType.Currency)
                {
                    try
                    {
                        string culture = PortalSettings.CultureCode;

                        System.Globalization.CultureInfo portalFormat = new System.Globalization.CultureInfo(culture);
                        string format = "{0:C2}";
                        value = string.Format(portalFormat.NumberFormat, format, Double.Parse(value));
                    }
                    catch
                    {
                        value = objArticle.CustomList[objCustomField.CustomFieldID].ToString();
                    }
                }
            }

            if (showCaption)
            {
                value = "<b>" + objCustomField.Caption + "</b>:&nbsp;" + value;
            }

            return value;

        }

        private List<ArticleInfo> GetRelatedArticles(ArticleInfo objArticle, int count)
        {

            if (_objRelatedArticles != null)
            {
                return _objRelatedArticles;
            }

            bool matchAllCategories = false;
            if (ArticleSettings.RelatedMode == RelatedType.MatchCategoriesAll || ArticleSettings.RelatedMode == RelatedType.MatchCategoriesAllTagsAny)
            {
                matchAllCategories = true;
            }

            bool matchAllTags = false;
            if (ArticleSettings.RelatedMode == RelatedType.MatchCategoriesAnyTagsAll || ArticleSettings.RelatedMode == RelatedType.MatchTagsAll)
            {
                matchAllTags = true;
            }

            ArticleController objArticleController = new ArticleController();

            int[] categoriesArray = null;
            if (ArticleSettings.RelatedMode == RelatedType.MatchCategoriesAll || ArticleSettings.RelatedMode == RelatedType.MatchCategoriesAllTagsAny || ArticleSettings.RelatedMode == RelatedType.MatchCategoriesAny || ArticleSettings.RelatedMode == RelatedType.MatchCategoriesAnyTagsAll || ArticleSettings.RelatedMode == RelatedType.MatchCategoriesAnyTagsAny)
            {
                ArrayList categories = objArticleController.GetArticleCategories(objArticle.ArticleID);
                List<int> categoriesRelated = new List<int>();
                foreach (CategoryInfo objCategory in categories)
                {
                    categoriesRelated.Add(objCategory.CategoryID);
                }
                if (categories.Count == 0)
                {
                    categoriesRelated.Add(-1);
                }
                categoriesArray = categoriesRelated.ToArray();
            }

            int[] tagsArray = null;
            if (ArticleSettings.RelatedMode == RelatedType.MatchCategoriesAllTagsAny || ArticleSettings.RelatedMode == RelatedType.MatchCategoriesAnyTagsAll || ArticleSettings.RelatedMode == RelatedType.MatchCategoriesAnyTagsAny || ArticleSettings.RelatedMode == RelatedType.MatchTagsAll || ArticleSettings.RelatedMode == RelatedType.MatchTagsAny)
            {
                List<int> tagsRelated = new List<int>();
                if (objArticle.Tags != "")
                {
                    TagController objTagController = new TagController();
                    foreach (string tag in objArticle.Tags.Split(','))
                    {
                        TagInfo objTag = objTagController.Get(objArticle.ModuleID, tag.ToLower().Trim());
                        if (objTag != null)
                        {
                            tagsRelated.Add(objTag.TagID);
                        }
                    }
                }
                if (tagsRelated.Count == 0)
                {
                    tagsRelated.Add(-1);
                }
                tagsArray = tagsRelated.ToArray();
            }

            int reftotal = 0;
            _objRelatedArticles = objArticleController.GetArticleList(objArticle.ModuleID, DateTime.Now, Null.NullDate, categoriesArray, matchAllCategories, null, (count + 1), 1, (count + 1), ArticleSettings.SortBy, ArticleSettings.SortDirection, true, false, Null.NullString, Null.NullInteger, ArticleSettings.ShowPending, true, false, false, false, false, Null.NullString, tagsArray, matchAllTags, Null.NullString, Null.NullInteger, Null.NullString, Null.NullString, ref reftotal);

            int positionToRemove = Null.NullInteger;

            int i = 0;
            foreach (ArticleInfo objRelatedArticle in _objRelatedArticles)
            {
                if (objArticle.ArticleID == objRelatedArticle.ArticleID)
                {
                    positionToRemove = i;
                }
                i = i + 1;
            }

            if (positionToRemove != Null.NullInteger)
            {
                _objRelatedArticles.RemoveAt(positionToRemove);
            }

            if (_objRelatedArticles.Count == (count + 1))
            {
                _objRelatedArticles.RemoveAt(count);
            }

            return _objRelatedArticles;

        }

        private string GetRatingImage(ArticleInfo objArticle)
        {

            if (objArticle.Rating == Null.NullDouble)
            {
                return Globals.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Images/Rating/stars-0-0.gif");
            }

            switch (RoundToUnit(objArticle.Rating, 0.5, false).ToString())
            {

                case "1":
                    return Globals.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Images/Rating/stars-1-0.gif");

                case "1.5":
                    return Globals.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Images/Rating/stars-1-5.gif");

                case "2":
                    return Globals.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Images/Rating/stars-2-0.gif");

                case "2.5":
                    return Globals.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Images/Rating/stars-2-5.gif");

                case "3":
                    return Globals.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Images/Rating/stars-3-0.gif");

                case "3.5":
                    return Globals.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Images/Rating/stars-3-5.gif");

                case "4":
                    return Globals.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Images/Rating/stars-4-0.gif");

                case "4.5":
                    return Globals.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Images/Rating/stars-4-5.gif");

                case "5":
                    return Globals.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Images/Rating/stars-5-0.gif");

            }

            return Globals.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Images/Rating/stars-0-0.gif");

        }


        public string GetSharedResource(string key)
        {

            string path = @"~/DesktopModules/GcDesign-NewsArticles/" + Localization.LocalResourceDirectory + "/" + Localization.LocalSharedResourceFile;
            return Localization.GetString(key, path);

        }


        //计算给定的字符串的MD5哈希, 该字符串转换为字节数组
        public string MD5CalcString(string strData)
        {

            MD5CryptoServiceProvider objMD5 = new MD5CryptoServiceProvider();
            byte[] arrData;
            byte[] arrHash;

            // 第一步将字符串转换为byte类型的直接数组 (使用 UTF8 编码字符)
            arrData = System.Text.Encoding.UTF8.GetBytes(strData);

            // 计算字节数组的hash值
            arrHash = objMD5.ComputeHash(arrData);

            //thanks objects
            objMD5 = null;

            // return formatted hash
            return ByteArrayToString(arrHash);

        }

        /// <summary>
        /// 处理标签
        /// </summary>
        /// <param name="content">含有标签的内容</param>
        /// <param name="objArticle"></param>
        /// <param name="generator"></param>
        /// <param name="objArticleSettings"></param>
        /// <returns></returns>
        private string ProcessPostTokens(string content, ArticleInfo objArticle, ref Random generator, ArticleSettings objArticleSettings)
        {
            //不处理提交的标签直接返回内容
            if (objArticleSettings.ProcessPostTokens == false)
            {
                return content;
            }

            //转换char数组‘[’‘]’
            string delimStr = "[]";
            char[] delimiter = delimStr.ToCharArray();
            //将内容以‘[’‘]’为分割符分割成数组
            string[] layoutArray = content.Split(delimiter);
            string formattedContent = "";

            for (int iPtr = 0; iPtr < layoutArray.Length; iPtr = iPtr + 2)
            {//Step 2

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
                                    ArticleInfo objArticleTarget = objArticleController.GetArticle(articleID);

                                    if (objArticleTarget != null)
                                    {
                                        string link = GcDesign.NewsArticles.Common.GetArticleLink(objArticleTarget, Tab, ArticleSettings, false);
                                        formattedContent += "<a href='" + link + "' rel='nofollow'>" + objArticleTarget.Title + "</a>";
                                    }
                                }
                                break;
                            }


                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("IMAGETHUMBRANDOM:"))
                            {

                                if (objArticle.ImageCount > 0)
                                {

                                    ImageController objImageController = new ImageController();
                                    List<ImageInfo> objImages = objImageController.GetImageList(objArticle.ArticleID, Null.NullString);
                                    if (objImages.Count > 0)
                                    {

                                        ImageInfo randomImage = objImages[generator.Next(0, objImages.Count - 1)];

                                        string val = layoutArray[iPtr + 1].Substring(17, layoutArray[iPtr + 1].Length - 17);
                                        if (val.IndexOf(':') == -1)
                                        {
                                            int length = Convert.ToInt32(val);

                                            Image objImage = new Image();
                                            objImage.ImageUrl = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + length.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(randomImage.Folder + randomImage.FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1");
                                            objImage.EnableViewState = false;
                                            objImage.AlternateText = objArticle.Title;

                                            formattedContent += RenderControlAsString(objImage);
                                        }
                                        else
                                        {

                                            string[] arr = val.Split(':');

                                            if (arr.Length == 2)
                                            {
                                                int width = Convert.ToInt32(val.Split(':')[0]);
                                                int height = Convert.ToInt32(val.Split(':')[1]);

                                                Image objImage = new Image();
                                                objImage.ImageUrl = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(randomImage.Folder + randomImage.FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1");
                                                objImage.EnableViewState = false;
                                                objImage.AlternateText = objArticle.Title;

                                                formattedContent += RenderControlAsString(objImage);
                                            }
                                        }

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

        private ProfilePropertyDefinitionCollection ProfileProperties
        {
            get
            {
                if (_profileProperties == null)
                {
                    _profileProperties = ProfileController.GetPropertyDefinitionsByPortal(PortalSettings.PortalId);
                }
                return _profileProperties;
            }
        }

        private string RenderControlAsString(Control objControl)
        {

            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);

            objControl.RenderControl(hw);

            return sb.ToString();

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

        private string StripHtml(string html)
        {

            string pattern = "<(.|\n)*?>";
            return Regex.Replace(html, pattern, string.Empty);

        }

        #endregion


        public static LayoutInfo GetLayout(NewsArticleModuleBase moduleContext, LayoutType type)
        {

            return GetLayout(moduleContext.ArticleSettings, moduleContext.ModuleConfiguration, moduleContext.Page, type);

        }

        public static LayoutInfo GetLayout(ArticleSettings articleSettings, ModuleInfo articleModule, Page page, LayoutType type)
        {

            string cacheKey = "NewsArticles-" + articleModule.TabModuleID.ToString() + type.ToString();
            LayoutInfo objLayout = (LayoutInfo)DataCache.GetCache(cacheKey);

            if (objLayout == null)
            {

                const string delimStr = "[]";
                char[] delimiter = delimStr.ToCharArray();

                objLayout = new LayoutInfo();
                string path = page.MapPath(@"~\DesktopModules\GcDesign-NewsArticles\Templates\" + articleSettings.Template + @"\" + type.ToString().Replace("_", "."));

                if (File.Exists(path) == false)
                {
                    // Need to find a default... 
                    path = page.MapPath(@"~\DesktopModules\GcDesign-NewsArticles\Templates\" + ArticleConstants.DEFAULT_TEMPLATE + @"\" + type.ToString().Replace("_", "."));
                }

                objLayout.Template = File.ReadAllText(path);
                objLayout.Tokens = objLayout.Template.Split(delimiter);

                DataCache.SetCache(cacheKey, objLayout, new DNNCacheDependency(path));

            }

            return objLayout;

        }

        public static void ClearCache(NewsArticleModuleBase moduleContext)
        {

            foreach (string type in System.Enum.GetNames(typeof(LayoutType)))
            {
                string cacheKey = "NewsArticles-" + moduleContext.TabModuleId.ToString() + type.ToString();
                DataCache.RemoveCache(cacheKey);
            }

        }

        #region " public Methods "

        public LayoutInfo GetLayoutObject(string templateData)
        {

            string delimStr = "[]";
            char[] delimiter = delimStr.ToCharArray();
            LayoutInfo objLayout = new LayoutInfo();

            objLayout.Template = templateData;
            objLayout.Tokens = objLayout.Template.Split(delimiter);

            return objLayout;

        }

        public string GetStylesheet(string template)
        {

            string value = "";

            string path = ArticleUtilities.MapPath(@"~\GcDesign-NewsArticles\Templates\" + template + @"\Template.css");

            if (File.Exists(path) == false)
            {
                // Need to find a default... 
            }

            File.ReadAllText(path);

            return value;

        }

        public void UpdateStylesheet(string template, string text)
        {

            string path = ArticleUtilities.MapPath(@"~\GcDesign-NewsArticles\Templates\" + template + @"\Template.css");
            File.WriteAllText(path, text);

        }

        public void UpdateLayout(string template, LayoutType type, string text)
        {

            string path = ArticleUtilities.MapPath(@"~\GcDesign-NewsArticles\Templates\" + template + @"\" + type.ToString().Replace("_", "."));
            File.WriteAllText(path, text);

        }

        public void LoadStyleSheet(string template)
        {

            ClientResourceManager.RegisterStyleSheet(Page, ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Templates/" + template + "/Template.css"), FileOrder.Css.ModuleCss);

        }

        public string ProcessImages(string html)
        {

            if (html.ToLower().Contains("src=\"images/") || html.ToLower().Contains("src=\"/images/"))
            {
                html = html.Replace("src=\"images/", "src=\"" + ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Templates/" + ArticleSettings.Template + "/Images/"));
                html = html.Replace("src=\"Images/", "src=\"" + ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Templates/" + ArticleSettings.Template + "/Images/"));
                html = html.Replace("src=\"/images/", "src=\"" + ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Images") + "/");
                html = html.Replace("src=\"/Images/", "src=\"" + ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Images") + "/");
            }

            return html;

        }

        #region " Process Article Item "


        public void ProcessHeaderFooter(ControlCollection objPlaceHolder, string[] layoutArray, ArticleInfo objArticle)
        {

            for (int iPtr = 0; iPtr < layoutArray.Length; iPtr = iPtr + 2)
            {
                objPlaceHolder.Add(new LiteralControl(ProcessImages(layoutArray[iPtr].ToString())));
            }

        }

        private int articleItemIndex = 0;
        public void ProcessArticleItem(ControlCollection objPlaceHolder, string[] layoutArray, ArticleInfo objArticle)
        {

            articleItemIndex = articleItemIndex + 1;
            _objRelatedArticles = null;

            System.Random Generator = new System.Random();

            Image objImage;
            Literal objLiteral;

            for (int iPtr = 0; iPtr < layoutArray.Length; iPtr = iPtr + 2)
            {

                objPlaceHolder.Add(new LiteralControl(ProcessImages(layoutArray[iPtr].ToString())));

                if (iPtr < layoutArray.Length - 1)
                {
                    switch (layoutArray[iPtr + 1])
                    {

                        case "APPROVERDISPLAYNAME"://审核者显示名
                            if (objArticle.Approver(PortalSettings.PortalId) != null)
                            {
                                objLiteral = new Literal();
                                objLiteral.Text = objArticle.Approver(PortalSettings.PortalId).DisplayName;
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "APPROVERFIRSTNAME":
                            if (objArticle.Approver(PortalSettings.PortalId) != null)
                            {
                                objLiteral = new Literal();
                                objLiteral.Text = objArticle.Approver(PortalSettings.PortalId).FirstName;
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "APPROVERLASTNAME":
                            if (objArticle.Approver(PortalSettings.PortalId) != null)
                            {
                                objLiteral = new Literal();
                                objLiteral.Text = objArticle.Approver(PortalSettings.PortalId).LastName;
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "APPROVERUSERNAME":
                            if (objArticle.Approver(PortalSettings.PortalId) != null)
                            {
                                objLiteral = new Literal();
                                objLiteral.Text = objArticle.Approver(PortalSettings.PortalId).Username;
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "ARTICLEID":
                            Literal objLiteral_articleID = new Literal();
                            objLiteral_articleID.Text = objArticle.ArticleID.ToString();
                            objPlaceHolder.Add(objLiteral_articleID);
                            break;
                        case "ARTICLELINK":
                            Literal objLiteral_link = new Literal();
                            int pageID = Null.NullInteger;
                            if (ArticleSettings.AlwaysShowPageID)
                            {
                                if (Pages(objArticle.ArticleID).Count > 0)
                                {
                                    pageID = ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID;
                                }
                            }

                            objLiteral_link.Text = Common.GetArticleLink(objArticle, Tab, ArticleSettings, IncludeCategory, pageID);
                            objLiteral_link.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral_link);
                            break;
                        case "AUTHOR":
                            Literal objLiteral_author = new Literal();
                            switch (ArticleSettings.DisplayMode)
                            {
                                case DisplayType.FirstName:
                                    objLiteral_author.Text = objArticle.AuthorFirstName;
                                    break;
                                case DisplayType.LastName:
                                    objLiteral_author.Text = objArticle.AuthorLastName;
                                    break;
                                case DisplayType.UserName:
                                    objLiteral_author.Text = objArticle.AuthorUserName;
                                    break;
                                case DisplayType.FullName:
                                    objLiteral_author.Text = objArticle.AuthorDisplayName;
                                    break;
                                default:
                                    objLiteral_author.Text = objArticle.AuthorUserName;
                                    break;
                            }
                            objPlaceHolder.Add(objLiteral_author);
                            break;
                        case "AUTHORDISPLAYNAME":
                            Literal objLiteral_displayName = new Literal();
                            objLiteral_displayName.Text = objArticle.AuthorDisplayName;
                            objPlaceHolder.Add(objLiteral_displayName);
                            break;
                        case "AUTHOREMAIL":
                            Literal objLiteral_email = new Literal();
                            objLiteral_email.Text = objArticle.AuthorEmail.ToString();
                            objPlaceHolder.Add(objLiteral_email);
                            break;
                        case "AUTHORFIRSTNAME":
                            Literal objLiteral_firstName = new Literal();
                            objLiteral_firstName.Text = objArticle.AuthorFirstName;
                            objPlaceHolder.Add(objLiteral_firstName);
                            break;
                        case "AUTHORFULLNAME":
                            Literal objLiteral_fullName = new Literal();
                            objLiteral_fullName.Text = objArticle.AuthorFullName;
                            objPlaceHolder.Add(objLiteral_fullName);
                            break;
                        case "AUTHORID":
                            Literal objLiteral_authorID = new Literal();
                            objLiteral_authorID.Text = objArticle.AuthorID.ToString();
                            objPlaceHolder.Add(objLiteral_authorID);
                            break;
                        case "AUTHORLASTNAME":
                            Literal objLiteral_lastName = new Literal();
                            objLiteral_lastName.Text = objArticle.AuthorLastName;
                            objPlaceHolder.Add(objLiteral_lastName);
                            break;
                        case "AUTHORLINK":
                            Literal objLiteral_Authorlink = new Literal();
                            objLiteral_Authorlink.Text = Common.GetAuthorLink(ArticleModule.TabID, ArticleModule.ModuleID, objArticle.AuthorID, objArticle.AuthorUserName, ArticleSettings.LaunchLinks, ArticleSettings);
                            objLiteral_Authorlink.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral_Authorlink);
                            break;
                        case "AUTHORUSERNAME":
                            Literal objLiteral_authorUserName = new Literal();
                            objLiteral_authorUserName.Text = objArticle.AuthorUserName.ToString();
                            objPlaceHolder.Add(objLiteral_authorUserName);
                            break;
                        case "CATEGORIES":
                            Literal objLiteral_categories = new Literal();

                            ArrayList objArticleCategories = (ArrayList)DataCache.GetCache(ArticleConstants.CACHE_CATEGORY_ARTICLE + objArticle.ArticleID.ToString());
                            if (objArticleCategories == null)
                            {
                                ArticleController objArticleController = new ArticleController();
                                objArticleCategories = objArticleController.GetArticleCategories(objArticle.ArticleID);
                                foreach (CategoryInfo objCategory in objArticleCategories)
                                {
                                    if (objCategory.InheritSecurity)
                                    {
                                        if (objLiteral_categories.Text != "")
                                        {
                                            objLiteral_categories.Text = objLiteral_categories.Text + ", <a href=\"" + Common.GetCategoryLink(ArticleModule.TabID, ArticleModule.ModuleID, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings) + "\" > " + objCategory.Name + "</a>";
                                        }
                                        else
                                        {
                                            objLiteral_categories.Text = "<a href=\"" + GcDesign.NewsArticles.Common.GetCategoryLink(ArticleModule.TabID, ArticleModule.ModuleID, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings) + "\" > " + objCategory.Name + "</a>";
                                        }
                                    }
                                    else
                                    {
                                        if (ArticleSettings.Settings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                                        {
                                            if (PortalSecurity.IsInRoles(ArticleSettings.Settings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString()))
                                            {
                                                if (objLiteral_categories.Text != "")
                                                {
                                                    objLiteral_categories.Text = objLiteral_categories.Text + ", <a href=\"" + Common.GetCategoryLink(ArticleModule.TabID, ArticleModule.ModuleID, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings) + "\" > " + objCategory.Name + "</a>";
                                                }
                                                else
                                                {
                                                    objLiteral_categories.Text = "<a href=\"" + Common.GetCategoryLink(ArticleModule.TabID, ArticleModule.ModuleID, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings) + "\" > " + objCategory.Name + "</a>";
                                                }
                                            }
                                        }
                                    }
                                }
                                DataCache.SetCache(ArticleConstants.CACHE_CATEGORY_ARTICLE + objArticle.ArticleID.ToString(), objArticleCategories);
                            }
                            else
                            {
                                foreach (CategoryInfo objCategory in objArticleCategories)
                                {
                                    if (objCategory.InheritSecurity)
                                    {
                                        if (objLiteral_categories.Text != "")
                                        {
                                            objLiteral_categories.Text = objLiteral_categories.Text + ", <a href=\"" + Common.GetCategoryLink(ArticleModule.TabID, ArticleModule.ModuleID, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings) + "\" > " + objCategory.Name + "</a>";
                                        }
                                        else
                                        {
                                            objLiteral_categories.Text = "<a href=\"" + Common.GetCategoryLink(ArticleModule.TabID, ArticleModule.ModuleID, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings) + "\" > " + objCategory.Name + "</a>";
                                        }
                                    }
                                    else
                                    {

                                        if (ArticleSettings.Settings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                                        {
                                            if (PortalSecurity.IsInRoles(ArticleSettings.Settings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString()))
                                            {
                                                if (objLiteral_categories.Text != "")
                                                {
                                                    objLiteral_categories.Text = objLiteral_categories.Text + ", <a href=\"" + Common.GetCategoryLink(ArticleModule.TabID, ArticleModule.ModuleID, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings) + "\" > " + objCategory.Name + "</a>";
                                                }
                                                else
                                                {
                                                    objLiteral_categories.Text = "<a href=\"" + Common.GetCategoryLink(ArticleModule.TabID, ArticleModule.ModuleID, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings) + "\" > " + objCategory.Name + "</a>";
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            objPlaceHolder.Add(objLiteral_categories);
                            break;
                        case "CATEGORIESNOLINK":
                            Literal objLiteral_categoriesNoLink = new Literal();
                            objArticleCategories = (ArrayList)DataCache.GetCache(ArticleConstants.CACHE_CATEGORY_ARTICLE + objArticle.ArticleID.ToString());
                            if (objArticleCategories == null)
                            {
                                ArticleController objArticleController = new ArticleController();
                                objArticleCategories = (objArticleController.GetArticleCategories(objArticle.ArticleID));
                                foreach (CategoryInfo objCategory in objArticleCategories)
                                {
                                    if (objCategory.InheritSecurity)
                                    {
                                        if (objLiteral_categoriesNoLink.Text != "")
                                        {
                                            objLiteral_categoriesNoLink.Text = objLiteral_categoriesNoLink.Text + ", " + objCategory.Name;
                                        }
                                        else
                                        {
                                            objLiteral_categoriesNoLink.Text = objCategory.Name;
                                        }
                                    }
                                    else
                                    {
                                        if (ArticleSettings.Settings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                                        {
                                            if (PortalSecurity.IsInRoles(ArticleSettings.Settings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString()))
                                            {

                                                if (objLiteral_categoriesNoLink.Text != "")
                                                {
                                                    objLiteral_categoriesNoLink.Text = objLiteral_categoriesNoLink.Text + ", " + objCategory.Name;
                                                }
                                                else
                                                {
                                                    objLiteral_categoriesNoLink.Text = objCategory.Name;
                                                }
                                            }
                                        }
                                    }
                                }
                                DataCache.SetCache(ArticleConstants.CACHE_CATEGORY_ARTICLE_NO_LINK + objArticle.ArticleID.ToString(), objArticleCategories);
                            }
                            else
                            {
                                foreach (CategoryInfo objCategory in objArticleCategories)
                                {
                                    if (objCategory.InheritSecurity)
                                    {
                                        if (objLiteral_categoriesNoLink.Text != "")
                                        {
                                            objLiteral_categoriesNoLink.Text = objLiteral_categoriesNoLink.Text + ", " + objCategory.Name;
                                        }
                                        else
                                        {
                                            objLiteral_categoriesNoLink.Text = objCategory.Name;
                                        }
                                    }
                                    else
                                    {
                                        if (ArticleSettings.Settings.Contains(objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                                        {
                                            if (PortalSecurity.IsInRoles(ArticleSettings.Settings[objCategory.CategoryID + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString()))
                                            {
                                                if (objLiteral_categoriesNoLink.Text != "")
                                                {
                                                    objLiteral_categoriesNoLink.Text = objLiteral_categoriesNoLink.Text + ", " + objCategory.Name;
                                                }
                                                else
                                                {
                                                    objLiteral_categoriesNoLink.Text = objCategory.Name;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            objPlaceHolder.Add(objLiteral_categoriesNoLink);
                            break;
                        case "COMMENTCOUNT":
                            Literal objLiteral_commentCount = new Literal();
                            objLiteral_commentCount.Text = objArticle.CommentCount.ToString();
                            objLiteral_commentCount.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral_commentCount);
                            break;
                        case "COMMENTLINK":
                            Literal objLiteral_commentLink = new Literal();
                            objLiteral_commentLink.Text = Common.GetArticleLink(objArticle, Tab, ArticleSettings, IncludeCategory) + "#Comments";
                            objLiteral_commentLink.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral_commentLink);
                            break;
                        case "COMMENTRSS":
                            Literal objLiteral_commentRss = new Literal();
                            objLiteral_commentRss.Text = Globals.AddHTTP(Request.Url.Host + Globals.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/RssComments.aspx?TabID=" + ArticleModule.TabID.ToString() + "&amp;ModuleID=" + ArticleModule.ModuleID.ToString() + "&amp;ArticleID=" + objArticle.ArticleID.ToString()).Replace(" ", "%20"));
                            objLiteral_commentRss.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral_commentRss);
                            break;
                        case "COMMENTS":
                            commentItemIndex = 0;

                            PlaceHolder phComments = new PlaceHolder();
                            LayoutInfo objLayoutCommentItem = GetLayout(ArticleSettings, ArticleModule, Page, LayoutType.Comment_Item_Html);

                            SortDirection direction = SortDirection.Ascending;
                            if (ArticleSettings.SortDirectionComments == 1)
                            {
                                direction = SortDirection.Descending;
                            }
                            CommentController objCommentController = new CommentController();
                            List<CommentInfo> objComments = objCommentController.GetCommentList(objArticle.ModuleID, objArticle.ArticleID, true, direction, Null.NullInteger);

                            foreach (CommentInfo objComment in objComments)
                            {
                                ProcessComment(phComments.Controls, objArticle, objComment, objLayoutCommentItem.Tokens);
                            }

                            objPlaceHolder.Add(phComments);
                            break;
                        case "CREATEDATE":
                            Literal objLiteral_createDate = new Literal();
                            objLiteral_createDate.Text = objArticle.CreatedDate.ToString("D");
                            objPlaceHolder.Add(objLiteral_createDate);
                            break;
                        case "CREATETIME":
                            Literal objLiteral_createTime = new Literal();
                            objLiteral_createTime.Text = objArticle.CreatedDate.ToString("t");
                            objPlaceHolder.Add(objLiteral_createTime);
                            break;
                        case "CURRENTPAGE":
                            Literal objLiteral_currentPage = new Literal();
                            if (objArticle.PageCount <= 1)
                            {
                                objLiteral_currentPage.Text = "1";
                            }
                            else
                            {
                                pageID = Null.NullInteger;
                                if (Request["PageID"] != null && Numeric.IsNumeric(Request["PageID"]))
                                {//""
                                    pageID = Convert.ToInt32(Request["PageID"]);
                                }
                                if (pageID == Null.NullInteger)
                                {
                                    pageID = ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID;
                                }
                                for (int i = 0; i < Pages(objArticle.ArticleID).Count; i++)
                                {
                                    PageInfo objPage = (PageInfo)Pages(objArticle.ArticleID)[i];
                                    if (pageID == objPage.PageID)
                                    {
                                        objLiteral_currentPage.Text = (i + 1).ToString();
                                        break;
                                    }
                                }
                                if (objLiteral_currentPage.Text == Null.NullString)
                                {
                                    objLiteral_currentPage.Text = "1";
                                }
                            }
                            objPlaceHolder.Add(objLiteral_currentPage);
                            break;
                        case "CUSTOMFIELDS":
                            CustomFieldController objCustomFieldController = new CustomFieldController();
                            ArrayList objCustomFields = objCustomFieldController.List(objArticle.ModuleID);
                            int i_CUSTOMFIELDS = 0;
                            foreach (CustomFieldInfo objCustomField in objCustomFields)
                            {
                                if (objCustomField.IsVisible == true)
                                {
                                    Literal objLiteral_customfields = new Literal();
                                    objLiteral_customfields.Text = GetFieldValue(objCustomField, objArticle, true) + "<br />";
                                    if (objLiteral_customfields.Text != "")
                                    {
                                        objPlaceHolder.Add(objLiteral_customfields);
                                    }
                                    i_CUSTOMFIELDS = i_CUSTOMFIELDS + 1;
                                }
                            }
                            break;
                        case "DETAILS":
                            Literal objLiteral_details = new Literal();
                            if (objArticle.PageCount > 0)
                            {
                                int pageID_details = Null.NullInteger;
                                if (Numeric.IsNumeric(Request["PageID"]))
                                {
                                    pageID_details = Convert.ToInt32(Request["PageID"]);
                                }
                                if (pageID_details == Null.NullInteger)
                                {
                                    objLiteral_details.Text = ProcessPostTokens(Server.HtmlDecode(objArticle.Body), objArticle, ref Generator, ArticleSettings);
                                }
                                else
                                {
                                    PageController pageController = new PageController();
                                    ArrayList pageList = pageController.GetPageList(objArticle.ArticleID);
                                    foreach (PageInfo objPage in pageList)
                                    {
                                        if (objPage.PageID == pageID_details)
                                        {
                                            objLiteral_details.Text = ProcessPostTokens(Server.HtmlDecode(objPage.PageText), objArticle, ref Generator, ArticleSettings);
                                            break;
                                        }
                                    }
                                    if (objLiteral_details.Text == Null.NullString)
                                    {
                                        objLiteral_details.Text = ProcessPostTokens(Server.HtmlDecode(objArticle.Body), objArticle, ref Generator, ArticleSettings);
                                    }
                                }
                            }
                            objPlaceHolder.Add(objLiteral_details);
                            break;
                        case "DETAILSDATA":
                            Literal objLiteral_detailsdata = new Literal();
                            if (objArticle.PageCount > 0)
                            {
                                int pageID_detailsdata = Null.NullInteger;
                                if (Numeric.IsNumeric(Request["PageID"]))
                                {
                                    pageID_detailsdata = Convert.ToInt32(Request["PageID"]);
                                }
                                if (pageID_detailsdata == Null.NullInteger)
                                {
                                    objLiteral_detailsdata.Text = "<![CDATA[" + ProcessPostTokens(Server.HtmlDecode(objArticle.Body), objArticle, ref Generator, ArticleSettings) + "]]>";
                                }
                                else
                                {
                                    PageController pageController = new PageController();
                                    ArrayList pageList = pageController.GetPageList(objArticle.ArticleID);
                                    foreach (PageInfo objPage in pageList)
                                    {
                                        if (objPage.PageID == pageID_detailsdata)
                                        {
                                            objLiteral_detailsdata.Text = "<![CDATA[" + ProcessPostTokens(Server.HtmlDecode(objPage.PageText), objArticle, ref Generator, ArticleSettings) + "]]>";
                                            break;
                                        }
                                    }
                                    if (objLiteral_detailsdata.Text == Null.NullString)
                                    {
                                        objLiteral_detailsdata.Text = "<![CDATA[" + ProcessPostTokens(Server.HtmlDecode(objArticle.Body), objArticle, ref Generator, ArticleSettings) + "]]>";
                                    }
                                }
                            }
                            objPlaceHolder.Add(objLiteral_detailsdata);
                            break;
                        case "EDIT":
                            if (IsEditable || (ArticleSettings.IsApprover) || (objArticle.AuthorID == UserId && ArticleSettings.IsAutoApprover))
                            {
                                HyperLink objHyperLink = new HyperLink();
                                objHyperLink.NavigateUrl = Common.GetModuleLink(ArticleModule.TabID, ArticleModule.ModuleID, "SubmitNews", ArticleSettings, "ArticleID=" + objArticle.ArticleID.ToString(), "ReturnUrl=" + Server.UrlEncode(Request.RawUrl));
                                objHyperLink.ImageUrl = "~/images/edit.gif";
                                objHyperLink.ToolTip = "Click to edit";
                                objHyperLink.EnableViewState = false;
                                objPlaceHolder.Add(objHyperLink);
                            }
                            break;
                        case "FILECOUNT":
                            Literal objLiteral_filecount = new Literal();
                            objLiteral_filecount.Text = objArticle.FileCount.ToString();
                            objLiteral_filecount.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral_filecount);
                            break;
                        case "FILELINK":
                            if (objArticle.FileCount > 0)
                            {
                                List<FileInfo> objFiles = FileProvider.Instance().GetFiles(objArticle.ArticleID);

                                if (objFiles.Count > 0)
                                {
                                    Literal objLiteral_fileLink = new Literal();
                                    objLiteral_fileLink.Text = objFiles[0].Link;
                                    objLiteral_fileLink.EnableViewState = false;
                                    objPlaceHolder.Add(objLiteral_fileLink);
                                }

                            }
                            break;
                        case "FILES":
                            // File Count Check
                            if (objArticle.FileCount > 0)
                            {
                                // Dim objFileController As new FileController
                                List<FileInfo> objFiles = FileProvider.Instance().GetFiles(objArticle.ArticleID);

                                if (objFiles.Count > 0)
                                {
                                    LayoutInfo objLayoutFileHeader = GetLayout(ArticleSettings, ArticleModule, Page, LayoutType.File_Header_Html);
                                    LayoutInfo objLayoutFileItem = GetLayout(ArticleSettings, ArticleModule, Page, LayoutType.File_Item_Html);
                                    LayoutInfo objLayoutFileFooter = GetLayout(ArticleSettings, ArticleModule, Page, LayoutType.File_Footer_Html);

                                    ProcessHeaderFooter(objPlaceHolder, objLayoutFileHeader.Tokens, objArticle);
                                    foreach (FileInfo objFile in objFiles)
                                    {
                                        ProcessFile(objPlaceHolder, objArticle, objFile, objLayoutFileItem.Tokens);
                                    }
                                    ProcessHeaderFooter(objPlaceHolder, objLayoutFileFooter.Tokens, objArticle);
                                }
                            }
                            break;
                        case "GRAVATARURL":
                            if (objArticle.AuthorEmail != "")
                            {
                                Literal objLiteral_gravatarurl = new Literal();
                                if (Request.IsSecureConnection)
                                {
                                    objLiteral_gravatarurl.Text = Globals.AddHTTP("secure.gravatar.com/avatar/" + MD5CalcString(objArticle.AuthorEmail.ToLower()));
                                }
                                else
                                {
                                    objLiteral_gravatarurl.Text = Globals.AddHTTP("www.gravatar.com/avatar/" + MD5CalcString(objArticle.AuthorEmail.ToLower()));
                                }
                                objPlaceHolder.Add(objLiteral_gravatarurl);
                            }
                            break;
                        case "HASAUTHOR":
                            if (objArticle.AuthorID == Null.NullInteger)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASAUTHOR")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASAUTHOR":
                            break;

                        case "HASNOAUTHOR":
                            if (objArticle.AuthorID != Null.NullInteger)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASNOAUTHOR")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASNOAUTHOR":
                            break;

                        case "HASCATEGORIES":
                            Literal objLiteral_hascategory = new Literal();
                            if (DataCache.GetCache(ArticleConstants.CACHE_CATEGORY_ARTICLE + objArticle.ArticleID.ToString()) == null)
                            {
                                ArticleController objArticleController = new ArticleController();
                                ArrayList objArticleCategories1 = objArticleController.GetArticleCategories(objArticle.ArticleID);
                                foreach (CategoryInfo objCategory in objArticleCategories1)
                                {
                                    if (objLiteral_hascategory.Text != "")
                                    {
                                        objLiteral_hascategory.Text = objLiteral_hascategory.Text + ", <a href=\"" + Common.GetCategoryLink(ArticleModule.TabID, ArticleModule.ModuleID, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings) + "\" > " + objCategory.Name + "</a>";
                                    }
                                    else
                                    {
                                        objLiteral_hascategory.Text = "<a href=\"" + Common.GetCategoryLink(ArticleModule.TabID, ArticleModule.ModuleID, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings) + "\" > " + objCategory.Name + "</a>";
                                    }
                                }
                                DataCache.SetCache(ArticleConstants.CACHE_CATEGORY_ARTICLE + objArticle.ArticleID.ToString(), objArticleCategories1);
                            }
                            else
                            {
                                ArrayList objArticleCategories2 = (ArrayList)DataCache.GetCache(ArticleConstants.CACHE_CATEGORY_ARTICLE + objArticle.ArticleID.ToString());
                                foreach (CategoryInfo objCategory in objArticleCategories2)
                                {
                                    if (objLiteral_hascategory.Text != "")
                                    {
                                        objLiteral_hascategory.Text = objLiteral_hascategory.Text + ", <a href=\"" + Common.GetCategoryLink(ArticleModule.TabID, ArticleModule.ModuleID, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings) + "\" > " + objCategory.Name + "</a>";
                                    }
                                    else
                                    {
                                        objLiteral_hascategory.Text = "<a href=\"" + Common.GetCategoryLink(ArticleModule.TabID, ArticleModule.ModuleID, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings) + "\" > " + objCategory.Name + "</a>";
                                    }
                                }
                            }
                            if (objLiteral_hascategory.Text == "")
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASCATEGORIES")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASCATEGORIES":
                            break;

                        case "HASCOMMENTS":
                            if (objArticle.CommentCount == 0)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASCOMMENTS")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASCOMMENTS":
                            break;

                        case "HASCOMMENTSENABLED":
                            if (ArticleSettings.IsCommentsEnabled == false)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASCOMMENTSENABLED")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASCOMMENTSENABLED":
                            break;

                        case "HASCUSTOMFIELDS":
                            objCustomFieldController = new CustomFieldController();
                            objCustomFields = objCustomFieldController.List(objArticle.ModuleID);

                            if (objCustomFields.Count == 0)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASCUSTOMFIELDS")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASCUSTOMFIELDS":
                            break;

                        case "HASDETAILS":
                            Literal objLiteral_hasDetails = new Literal();
                            objLiteral_hasDetails.Text = "";
                            if (objArticle.PageCount > 0)
                            {
                                pageID = Null.NullInteger;
                                if (Numeric.IsNumeric(Request["PageID"]))
                                {
                                    pageID = Convert.ToInt32(Request["PageID"]);
                                }
                                if (pageID == Null.NullInteger)
                                {
                                    objLiteral_hasDetails.Text = ProcessPostTokens(Server.HtmlDecode(objArticle.Body), objArticle, ref Generator, ArticleSettings);
                                }
                                else
                                {
                                    PageController pageController = new PageController();
                                    ArrayList pageList = pageController.GetPageList(objArticle.ArticleID);
                                    foreach (PageInfo objPage in pageList)
                                    {
                                        if (objPage.PageID == pageID)
                                        {
                                            objLiteral_hasDetails.Text = ProcessPostTokens(Server.HtmlDecode(objPage.PageText), objArticle, ref Generator, ArticleSettings);
                                            break;
                                        }
                                    }
                                    if (objLiteral_hasDetails.Text == Null.NullString)
                                    {
                                        objLiteral_hasDetails.Text = ProcessPostTokens(Server.HtmlDecode(objArticle.Body), objArticle, ref Generator, ArticleSettings);
                                    }
                                }
                            }

                            if (objLiteral_hasDetails.Text.Replace("<p>&#160;</p>", "").Trim() == "")
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASDETAILS")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASDETAILS":
                            break;

                        case "HASNODETAILS":
                            Literal objLiteral_hasNoDetails = new Literal();
                            objLiteral_hasNoDetails.Text = "";
                            if (objArticle.PageCount > 0)
                            {
                                int pageId = Null.NullInteger;
                                if (Numeric.IsNumeric(Request["PageID"]))
                                {
                                    pageId = Convert.ToInt32(Request["PageID"]);
                                }
                                if (pageId == Null.NullInteger)
                                {
                                    objLiteral_hasNoDetails.Text = ProcessPostTokens(Server.HtmlDecode(objArticle.Body), objArticle, ref Generator, ArticleSettings);
                                }
                                else
                                {
                                    PageController pageController = new PageController();
                                    ArrayList pageList = pageController.GetPageList(objArticle.ArticleID);
                                    foreach (PageInfo objPage in pageList)
                                    {
                                        if (objPage.PageID == pageId)
                                        {
                                            objLiteral_hasNoDetails.Text = ProcessPostTokens(Server.HtmlDecode(objPage.PageText), objArticle, ref Generator, ArticleSettings);
                                            break;
                                        }
                                    }
                                    if (objLiteral_hasNoDetails.Text == Null.NullString)
                                    {
                                        objLiteral_hasNoDetails.Text = ProcessPostTokens(Server.HtmlDecode(objArticle.Body), objArticle, ref Generator, ArticleSettings);
                                    }
                                }
                            }

                            if (objLiteral_hasNoDetails.Text.Replace("<p>&#160;</p>", "").Trim() != "")
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASNODETAILS")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASNODETAILS":
                            break;

                        case "HASFILES":
                            if (objArticle.FileCount == 0)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASFILES")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASFILES":
                            break;

                        case "HASIMAGE":
                            if (objArticle.ImageUrl == "" && objArticle.ImageCount == 0)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASIMAGE")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASIMAGE":
                            break;

                        case "HASIMAGES":
                            if (objArticle.ImageCount == 0)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASIMAGES")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASIMAGES":
                            break;

                        case "HASLINK":
                            if (objArticle.Url == Null.NullString)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASLINK")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASLINK":
                            break;

                        case "HASMOREDETAIL":
                            if (objArticle.Url == Null.NullString && StripHtml(objArticle.Summary.Trim()) == "")
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASMOREDETAIL")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASMOREDETAIL":
                            break;

                        case "HASMULTIPLEIMAGES":
                            if (objArticle.ImageCount <= 1)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASMULTIPLEIMAGES")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASMULTIPLEIMAGES":
                            break;

                        case "HASMULTIPLEPAGES":
                            if (objArticle.PageCount <= 1)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASMULTIPLEPAGES")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASMULTIPLEPAGES":
                            break;

                        case "HASNEXTPAGE":
                            if (Pages(objArticle.ArticleID).Count <= 1)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASNEXTPAGE")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            else
                            {
                                if (_pageId == Null.NullInteger)
                                {
                                    _pageId = ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID;
                                }
                                if (_pageId == ((PageInfo)Pages(objArticle.ArticleID)[Pages(objArticle.ArticleID).Count - 1]).PageID)
                                {
                                    while (iPtr < layoutArray.Length - 1)
                                    {
                                        if (layoutArray[iPtr + 1] == "/HASNEXTPAGE")
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                            }
                            break;
                        case "/HASNEXTPAGE":
                            break;

                        case "HASNOCOMMENTS":
                            if (objArticle.CommentCount > 0)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASNOCOMMENTS")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASNOCOMMENTS":
                            break;

                        case "HASNOFILES":
                            if (objArticle.FileCount > 0)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASNOFILES")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASNOFILES":
                            break;

                        case "HASNOIMAGE":
                            if (objArticle.ImageUrl != "" || objArticle.ImageCount > 0)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASNOIMAGE")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASNOIMAGE":
                            break;

                        case "HASNOIMAGES":
                            if (objArticle.ImageCount > 0)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASNOIMAGES")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASNOIMAGES":
                            break;

                        case "HASNOLINK":
                            if (objArticle.Url != Null.NullString)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASNOLINK")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASNOLINK":
                            break;

                        case "HASPREVPAGE":
                            if (objArticle.PageCount <= 1)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASPREVPAGE")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            else
                            {
                                if (_pageId == Null.NullInteger)
                                {
                                    _pageId = ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID;
                                }
                                if (_pageId == ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID)
                                {
                                    while (iPtr < layoutArray.Length - 1)
                                    {
                                        if (layoutArray[iPtr + 1] == "/HASPREVPAGE")
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                            }
                            break;
                        case "/HASPREVPAGE":
                            break;

                        case "HASRATING":
                            if (ArticleSettings.EnableRatings == false || objArticle.RatingCount == 0)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASRATING")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASRATING":
                            break;

                        case "HASRATINGSENABLED":
                            if (ArticleSettings.EnableRatings == false)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASRATINGSENABLED")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASRATINGSENABLED":
                            break;

                        case "HASRELATED":
                            List<ArticleInfo> objRelatedArticles = GetRelatedArticles(objArticle, 5);
                            if (objRelatedArticles.Count == 0)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASRELATED")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASRELATED":
                            break;

                        case "HASSUMMARY":
                            if (StripHtml(objArticle.Summary.Trim()) == "")
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASSUMMARY")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASSUMMARY":
                            break;

                        case "HASNOSUMMARY":
                            if (StripHtml(objArticle.Summary.Trim()) != "")
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASNOSUMMARY")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASNOSUMMARY":
                            break;

                        case "HASTAGS":
                            if (objArticle.Tags == "")
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASTAGS")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASTAGS":
                            break;

                        case "HASNOTAGS":
                            if (objArticle.Tags != "")
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/HASNOTAGS")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASNOTAGS":
                            break;

                        case "IMAGE":
                            if (objArticle.ImageUrl != "")
                            {
                                objImage = new Image();
                                objImage.ImageUrl = FormatImageUrl(objArticle.ImageUrl);
                                objImage.EnableViewState = false;
                                objImage.AlternateText = objArticle.Title;
                                objPlaceHolder.Add(objImage);
                            }
                            else
                            {
                                if (objArticle.ImageCount > 0)
                                {
                                    ImageController objImageController = new ImageController();
                                    List<ImageInfo> objImages = objImageController.GetImageList(objArticle.ArticleID, Null.NullString);

                                    if (objImages.Count > 0)
                                    {
                                        objImage = new Image();
                                        objImage.ImageUrl = PortalSettings.HomeDirectory + objImages[0].Folder + objImages[0].FileName;
                                        objImage.EnableViewState = false;
                                        objImage.AlternateText = objArticle.Title;
                                        objPlaceHolder.Add(objImage);
                                    }

                                }
                            }
                            break;
                        case "IMAGECOUNT":
                            Literal objLiteral_imageCount = new Literal();
                            objLiteral_imageCount.Text = objArticle.ImageCount.ToString();
                            objLiteral_imageCount.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral_imageCount);
                            break;
                        case "IMAGELINK":
                            if (objArticle.ImageUrl != "")
                            {
                                Literal objLiteral_imageLink = new Literal();
                                objLiteral_imageLink.Text = FormatImageUrl(objArticle.ImageUrl);
                                objPlaceHolder.Add(objLiteral_imageLink);
                            }
                            else
                            {
                                ImageController objImageController = new ImageController();
                                List<ImageInfo> objImages = objImageController.GetImageList(objArticle.ArticleID, Null.NullString);

                                if (objImages.Count > 0)
                                {
                                    Literal objLiteral_imageLink = new Literal();
                                    objLiteral_imageLink.Text = PortalSettings.HomeDirectory + objImages[0].Folder + objImages[0].FileName;
                                    objPlaceHolder.Add(objLiteral_imageLink);
                                }
                            }
                            break;
                        case "IMAGES":

                            // Image Count Check
                            if (objArticle.ImageCount > 0)
                            {
                                int imageIndex = 0;

                                ImageController objImageController = new ImageController();
                                List<ImageInfo> objImages = objImageController.GetImageList(objArticle.ArticleID, Null.NullString);

                                if (objImages.Count > 0)
                                {
                                    LayoutInfo objLayoutImageHeader = GetLayout(ArticleSettings, ArticleModule, Page, LayoutType.Image_Header_Html);
                                    LayoutInfo objLayoutImageItem = GetLayout(ArticleSettings, ArticleModule, Page, LayoutType.Image_Item_Html);
                                    LayoutInfo objLayoutImageFooter = GetLayout(ArticleSettings, ArticleModule, Page, LayoutType.Image_Footer_Html);

                                    ProcessHeaderFooter(objPlaceHolder, objLayoutImageHeader.Tokens, objArticle);
                                    foreach (ImageInfo objImage1 in objImages)
                                    {
                                        ProcessImage(objPlaceHolder, objArticle, objImage1, objLayoutImageItem.Tokens);
                                    }
                                    ProcessHeaderFooter(objPlaceHolder, objLayoutImageFooter.Tokens, objArticle);
                                }

                                //'Dim script As string = "" _
                                //'& "<script type=""text/javascript"">" & vbCrLf _
                                //'& "jQuery(function() {" & vbCrLf _
                                //'& "jQuery('a[rel*=lightbox" & objArticle.ArticleID.ToString() & "]').lightBox({" & vbCrLf _
                                //'& "imageLoading: '" & ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Images/Lightbox/lightbox-ico-loading.gif") & "'," & vbCrLf _
                                //'& "imageBlank: '" & ArticleUtilities.ResolveUrl("~/images/spacer.gif") & "'," & vbCrLf _
                                //'& "txtImage: '" & GetSharedResource("Image") & "'," & vbCrLf _
                                //'& "txtOf: '" & GetSharedResource("Of") & "'," & vbCrLf _
                                //'& "next: '" & GetSharedResource("Next") & "'," & vbCrLf _
                                //'& "previous: '" & GetSharedResource("Previous") & "'," & vbCrLf _
                                //'& "close: '" & GetSharedResource("Close") & "'" & vbCrLf _
                                //'& "});" & vbCrLf _
                                //'& "});" & vbCrLf _
                                //'& "</script>" & vbCrLf

                                //'Dim objScript As new Literal
                                //'objScript.Text = script
                                //'objPlaceHolder.AddAt(0, objScript)
                            }
                            break;
                        case "ISAUTHOR":
                            bool isAuthor = false;

                            if (Request.IsAuthenticated)
                            {
                                UserInfo objUser = UserController.Instance.GetCurrentUserInfo();
                                if (objUser != null)
                                {
                                    if (objUser.UserID == objArticle.AuthorID)
                                    {
                                        isAuthor = true;
                                    }
                                }
                            }

                            if (isAuthor == false)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISAUTHOR")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISAUTHOR":
                            break;

                        case "ISANONYMOUS":
                            if (Request.IsAuthenticated)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISANONYMOUS")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISANONYMOUS":
                            break;

                        case "ISDRAFT":
                            if (objArticle.Status != StatusType.Draft)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISDRAFT")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISDRAFT":
                            break;

                        case "ISFEATURED":
                            if (objArticle.IsFeatured == false)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISFEATURED")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISFEATURED":
                            break;

                        case "ISNOTFEATURED":
                            if (objArticle.IsFeatured)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISNOTFEATURED")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISNOTFEATURED":
                            break;

                        case "ISFIRST":
                            if (articleItemIndex > 1)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISFIRST")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISFIRST":
                            break;

                        case "ISFIRST2":
                            if (articleItemIndex > 1 || (Request["currentpage"] != null && Request["currentpage"] != "1"))
                            {//""
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISFIRST2")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISFIRST2":
                            break;

                        case "ISNOTFIRST":
                            if (articleItemIndex == 1)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISNOTFIRST")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISNOTFIRST":
                            break;

                        case "ISNOTFIRST2":
                            if (articleItemIndex == 1 && (Request["currentpage"] == "" || Request["currentpage"] == "1"))
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISNOTFIRST2")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISNOTFIRST2":
                            break;

                        case "ISSECOND":
                            if (articleItemIndex != 2)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISSECOND")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISSECOND":
                            break;

                        case "ISNOTSECOND":
                            if (articleItemIndex == 2)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISNOTSECOND")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISNOTSECOND":
                            break;

                        case "ISNOTANONYMOUS":
                            if (!Request.IsAuthenticated)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISNOTANONYMOUS")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISNOTANONYMOUS":
                            break;

                        case "ISNOTSECURE":
                            if (objArticle.IsSecure)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISNOTSECURE")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISNOTSECURE":
                            break;

                        case "ISPUBLISHED":
                            if (objArticle.Status != StatusType.Published)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISPUBLISHED")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISPUBLISHED":
                            break;

                        case "ISRATEABLE":
                            if (!ArticleSettings.IsRateable)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISRATEABLE")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISRATEABLE":
                            break;

                        case "ISRSSITEM":
                            if (objArticle.RssGuid == Null.NullString)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISRSSITEM")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISRSSITEM":
                            break;

                        case "ISNOTRSSITEM":
                            if (objArticle.RssGuid != Null.NullString)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISNOTRSSITEM")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISNOTRSSITEM":
                            break;

                        case "ISSECURE":
                            if (objArticle.IsSecure == false)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISSECURE")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISSECURE":
                            break;

                        case "ISSYNDICATIONENABLED":
                            if (ArticleSettings.IsSyndicationEnabled == false)
                            {
                                while (iPtr < layoutArray.Length - 1)
                                {
                                    if (layoutArray[iPtr + 1] == "/ISSYNDICATIONENABLED")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISSYNDICATIONENABLED":
                            break;

                        case "ITEMINDEX":
                            Literal objLiteral_itemIndex = new Literal();
                            objLiteral_itemIndex.Text = articleItemIndex.ToString();
                            objPlaceHolder.Add(objLiteral_itemIndex);
                            break;
                        case "LASTUPDATEDATE":
                            Literal objLiteral_lastUpdateTime = new Literal();
                            objLiteral_lastUpdateTime.Text = objArticle.LastUpdate.ToString("D");
                            objPlaceHolder.Add(objLiteral_lastUpdateTime);
                            break;
                        case "LASTUPDATEEMAIL":
                            Literal lastUpdateEmail = new Literal();
                            lastUpdateEmail.Text = objArticle.LastUpdateEmail.ToString();
                            objPlaceHolder.Add(lastUpdateEmail);
                            break;
                        case "LASTUPDATEFIRSTNAME":
                            Literal lastUpdateFirstName = new Literal();
                            lastUpdateFirstName.Text = objArticle.LastUpdateFirstName.ToString();
                            objPlaceHolder.Add(lastUpdateFirstName);
                            break;
                        case "LASTUPDATELASTNAME":
                            Literal lastUpdateLastName = new Literal();
                            lastUpdateLastName.Text = objArticle.LastUpdateLastName.ToString();
                            objPlaceHolder.Add(lastUpdateLastName);
                            break;
                        case "LASTUPDATEUSERNAME":
                            Literal lastUpdateUserName = new Literal();
                            lastUpdateUserName.Text = objArticle.LastUpdateUserName.ToString();
                            objPlaceHolder.Add(lastUpdateUserName);
                            break;
                        case "LASTUPDATEFULLNAME":
                            Literal lastUpdateFullName = new Literal();
                            lastUpdateFullName.Text = objArticle.LastUpdateFullName.ToString();
                            objPlaceHolder.Add(lastUpdateFullName);
                            break;
                        case "LASTUPDATEID":
                            Literal objLiteral_lastUpdateID = new Literal();
                            objLiteral_lastUpdateID.Text = objArticle.LastUpdateID.ToString();
                            objPlaceHolder.Add(objLiteral_lastUpdateID);
                            break;
                        case "LINK":
                            Literal objLiteral_Link = new Literal();
                            if (objArticle.Url == "")
                            {
                                pageID = Null.NullInteger;
                                if (ArticleSettings.AlwaysShowPageID)
                                {
                                    if (Pages(objArticle.ArticleID).Count > 0)
                                    {
                                        pageID = ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID;
                                    }
                                }
                                objLiteral_Link.Text = Common.GetArticleLink(objArticle, Tab, ArticleSettings, IncludeCategory, pageID);
                            }
                            else
                            {
                                objLiteral_Link.Text = Globals.LinkClick(objArticle.Url, Tab.TabID, objArticle.ModuleID, false);
                            }
                            objLiteral_Link.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral_Link);
                            break;
                        case "LINKNEXT":
                            HyperLink objLink = new HyperLink();
                            objLink.CssClass = "CommandButton";
                            if (Pages(objArticle.ArticleID).Count <= 1)
                            {
                                objLink.Enabled = false;
                            }
                            else
                            {
                                if (_pageId == Null.NullInteger)
                                {
                                    _pageId = ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID;
                                }
                                if (_pageId == ((PageInfo)Pages(objArticle.ArticleID)[Pages(objArticle.ArticleID).Count - 1]).PageID)
                                {
                                    objLink.Enabled = false;
                                }
                                else
                                {
                                    objLink.Enabled = true;
                                }
                            }
                            if (objLink.Enabled == true)
                            {
                                if (_pageId == Null.NullInteger)
                                {
                                    _pageId = ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID;
                                }
                                for (int i = 0; i < Pages(objArticle.ArticleID).Count; i++)
                                {
                                    PageInfo objPage = (PageInfo)Pages(objArticle.ArticleID)[i];
                                    if (_pageId == objPage.PageID)
                                    {
                                        objLink.NavigateUrl = Common.GetArticleLink(objArticle, Tab, ArticleSettings, IncludeCategory, "PageID=" + ((PageInfo)Pages(objArticle.ArticleID)[i + 1]).PageID.ToString());
                                    }
                                }
                            }
                            objLink.Text = GetSharedResource("NextPage");
                            objPlaceHolder.Add(objLink);
                            break;
                        case "LINKPREVIOUS":
                            objLink = new HyperLink();
                            objLink.CssClass = "CommandButton";
                            if (Pages(objArticle.ArticleID).Count <= 1)
                            {
                                objLink.Enabled = false;
                            }
                            else
                            {
                                if (_pageId == Null.NullInteger)
                                {
                                    _pageId = ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID;
                                }
                                if (_pageId == ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID)
                                {
                                    objLink.Enabled = false;
                                }
                                else
                                {
                                    objLink.Enabled = true;
                                }
                            }
                            if (objLink.Enabled == true)
                            {
                                for (int i = 0; i < Pages(objArticle.ArticleID).Count - 1; i++)
                                {
                                    PageInfo objPage = (PageInfo)Pages(objArticle.ArticleID)[i];
                                    if (_pageId == objPage.PageID)
                                    {
                                        if (((PageInfo)Pages(objArticle.ArticleID)[i - 1]).PageID.ToString() == ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID.ToString())
                                        {
                                            objLink.NavigateUrl = Common.GetArticleLink(objArticle, Tab, ArticleSettings, IncludeCategory);
                                        }
                                        else
                                        {
                                            objLink.NavigateUrl = Common.GetArticleLink(objArticle, Tab, ArticleSettings, IncludeCategory, "PageID=" + ((PageInfo)Pages(objArticle.ArticleID)[i - 1]).PageID.ToString());
                                        }
                                        break;
                                    }
                                }
                            }
                            objLink.Text = GetSharedResource("PreviousPage");
                            objPlaceHolder.Add(objLink);
                            break;
                        case "LINKTARGET":
                            if (objArticle.Url != "")
                            {
                                Literal objLiteral_linkTarget = new Literal();
                                if (objArticle.IsNewWindow)
                                {
                                    objLiteral_linkTarget.Text = "_blank";
                                }
                                else
                                {
                                    objLiteral_linkTarget.Text = "_self";
                                }
                                objPlaceHolder.Add(objLiteral_linkTarget);
                            }
                            break;
                        case "MODULEID":
                            Literal objLiteral_moduleID = new Literal();
                            objLiteral_moduleID.Text = objArticle.ModuleID.ToString();
                            objPlaceHolder.Add(objLiteral_moduleID);
                            break;
                        case "PAGECOUNT":
                            Literal objLiteral_pageCount = new Literal();
                            objLiteral_pageCount.Text = objArticle.PageCount.ToString();
                            objLiteral_pageCount.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral_pageCount);
                            break;
                        case "PAGETEXT":
                            Literal pageNext = new Literal();
                            if (objArticle.PageCount > 0)
                            {
                                pageID = Null.NullInteger;
                                if (Numeric.IsNumeric(Request["PageID"]))
                                {
                                    pageID = Convert.ToInt32(Request["PageID"]);
                                }
                                if (pageID == Null.NullInteger)
                                {
                                    pageNext.Text = ProcessPostTokens(Server.HtmlDecode(objArticle.Body), objArticle, ref Generator, ArticleSettings);
                                }
                                else
                                {
                                    PageController pageController = new PageController();
                                    ArrayList pageList = pageController.GetPageList(objArticle.ArticleID);
                                    foreach (PageInfo objPage in pageList)
                                    {
                                        if (objPage.PageID == pageID)
                                        {
                                            pageNext.Text = ProcessPostTokens(Server.HtmlDecode(objPage.PageText), objArticle, ref Generator, ArticleSettings);
                                            break;
                                        }
                                    }
                                    if (pageNext.Text == Null.NullString)
                                    {
                                        pageNext.Text = ProcessPostTokens(Server.HtmlDecode(objArticle.Body), objArticle, ref Generator, ArticleSettings);
                                    }
                                }
                            }
                            else
                            {
                                pageNext.Text = ProcessPostTokens(Server.HtmlDecode(objArticle.Summary), objArticle, ref Generator, ArticleSettings);
                            }
                            objPlaceHolder.Add(pageNext);
                            break;
                        case "PAGETITLE":
                            Literal pageTitle = new Literal();
                            if (objArticle.PageCount > 0)
                            {
                                PageController pageController = new PageController();
                                ArrayList pageList = pageController.GetPageList(objArticle.ArticleID);
                                pageID = Null.NullInteger;
                                if (Numeric.IsNumeric(Request["PageID"]))
                                {
                                    pageID = Convert.ToInt32(Request["PageID"]);
                                }
                                if (pageID == Null.NullInteger)
                                {
                                    pageTitle.Text = ((PageInfo)pageList[0]).Title;
                                }
                                else
                                {
                                    foreach (PageInfo objPage in pageList)
                                    {
                                        if (objPage.PageID == pageID)
                                        {
                                            pageTitle.Text = objPage.Title;
                                            break;
                                        }
                                    }
                                    if (pageTitle.Text == Null.NullString)
                                    {
                                        pageTitle.Text = ((PageInfo)pageList[0]).Title;
                                    }
                                }
                            }
                            else
                            {
                                pageTitle.Text = objArticle.Title;
                            }
                            objPlaceHolder.Add(pageTitle);
                            break;
                        case "PAGETITLENEXT":
                            Literal pageTitleNext = new Literal();
                            if (Pages(objArticle.ArticleID).Count <= 1)
                            {
                                pageTitleNext.Visible = false;
                            }
                            else
                            {
                                if (_pageId == Null.NullInteger)
                                {
                                    _pageId = ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID;
                                }
                                if (_pageId == ((PageInfo)Pages(objArticle.ArticleID)[Pages(objArticle.ArticleID).Count - 1]).PageID)
                                {
                                    pageTitleNext.Visible = false;
                                }
                                else
                                {
                                    pageTitleNext.Visible = true;
                                }
                            }
                            if (pageTitleNext.Visible)
                            {
                                if (_pageId == Null.NullInteger)
                                {
                                    _pageId = ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID;
                                }
                                for (int i = 0; i < Pages(objArticle.ArticleID).Count; i++)
                                {
                                    PageInfo objPage = (PageInfo)Pages(objArticle.ArticleID)[i];
                                    if (_pageId == objPage.PageID)
                                    {
                                        pageTitleNext.Text = ((PageInfo)Pages(objArticle.ArticleID)[i + 1]).Title;
                                    }
                                }
                            }
                            if (pageTitleNext.Visible = true && pageTitleNext.Text != "")
                            {
                                objPlaceHolder.Add(pageTitleNext);
                            }
                            break;
                        case "PAGETITLEPREV":
                            Literal pageTitlePre = new Literal();
                            if (Pages(objArticle.ArticleID).Count <= 1)
                            {
                                pageTitlePre.Visible = false;
                            }
                            else
                            {
                                if (_pageId == Null.NullInteger)
                                {
                                    _pageId = ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID;
                                }
                                if (_pageId == ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID)
                                {
                                    pageTitlePre.Visible = false;
                                }
                                else
                                {
                                    pageTitlePre.Visible = true;
                                }
                            }
                            if (pageTitlePre.Visible)
                            {
                                for (int i = 0; i < Pages(objArticle.ArticleID).Count; i++)
                                {
                                    PageInfo objPage = (PageInfo)Pages(objArticle.ArticleID)[i];
                                    if (_pageId == objPage.PageID)
                                    {
                                        if (((PageInfo)Pages(objArticle.ArticleID)[i - 1]).PageID.ToString() == ((PageInfo)Pages(objArticle.ArticleID)[0]).PageID.ToString())
                                        {
                                            pageTitlePre.Text = objArticle.Title;
                                        }
                                        else
                                        {
                                            pageTitlePre.Text = ((PageInfo)Pages(objArticle.ArticleID)[i - 1]).Title;
                                        }
                                        break;
                                    }
                                }
                            }
                            if (pageTitlePre.Visible && pageTitlePre.Text != "")
                            {
                                objPlaceHolder.Add(pageTitlePre);
                            }

                            break;
                        case "PAGES":
                            DropDownList drpPages = new DropDownList();
                            PageController pageController_pages = new PageController();
                            ArrayList pageList_pages = pageController_pages.GetPageList(objArticle.ArticleID);
                            drpPages.Attributes.Add("onChange", "window.location.href=this.options[this.selectedIndex].value;");
                            drpPages.CssClass = "Normal";
                            pageID = Null.NullInteger;
                            if (Numeric.IsNumeric(Request["PageID"]))
                            {
                                pageID = Convert.ToInt32(Request["PageID"]);
                            }
                            foreach (PageInfo objPage in pageList_pages)
                            {
                                ListItem item = new ListItem();

                                item.Value = Common.GetModuleLink(ArticleModule.TabID, ArticleModule.ModuleID, "ArticleView", ArticleSettings, "ArticleID=" + objArticle.ArticleID.ToString(), "PageID=" + objPage.PageID.ToString());
                                item.Text = objPage.Title;

                                if (objPage.PageID == pageID)
                                {
                                    item.Selected = true;
                                }
                                drpPages.Items.Add(item);
                            }
                            if (drpPages.Items.Count > 1)
                            {
                                objPlaceHolder.Add(drpPages);
                            }
                            break;
                        case "PAGER":

                            PageController pageController_pager = new PageController();
                            ArrayList pageList_pager = pageController_pager.GetPageList(objArticle.ArticleID);

                            pageID = Null.NullInteger;
                            if (Numeric.IsNumeric(Request["PageID"]))
                            {
                                pageID = Convert.ToInt32(Request["PageID"]);
                            }

                            string pager = "<table class=\"PagingTable\" border=\"0\">" + "<tbody>" + "<tr>";

                            int pageNo = 1;

                            int y = 1;
                            foreach (PageInfo objPage in pageList_pager)
                            {
                                if (objPage.PageID == pageID)
                                {
                                    pageNo = y;
                                    break;
                                }
                                y = y + 1;
                            }

                            pager = pager + "<td align=\"left\" style=\"width: 50%;\" class=\"Normal\">" + GetSharedResource("Page") + " " + pageNo.ToString() + " " + GetSharedResource("Of") + " " + pageList_pager.Count.ToString() + "</td>" +
                                "<td align=\"right\" style=\"width: 50%;\" class=\"Normal\">";

                            if (pageList_pager.Count > 1)
                            {
                                if (pageNo == 1)
                                {
                                    pager = pager + "<span class=\"NormalDisabled\">" + GetSharedResource("First") + "</span>&nbsp;&nbsp;";
                                }
                                else
                                {
                                    pager = pager + "<a class=\"CommandButton\" href=\"" + Common.GetModuleLink(ArticleModule.TabID, ArticleModule.ModuleID, "ArticleView", ArticleSettings, "ArticleID=" + objArticle.ArticleID.ToString(), "PageID=" + ((PageInfo)pageList_pager[0]).PageID.ToString()) + "\">" + GetSharedResource("First") + "</a>&nbsp;&nbsp;";
                                }
                            }
                            else
                            {
                                pager = pager + "<span class=\"NormalDisabled\">" + GetSharedResource("First") + "</span>&nbsp;&nbsp;";
                            }

                            if (pageList_pager.Count > 1)
                            {
                                if (pageNo == 1)
                                {
                                    pager = pager + "<span class=\"NormalDisabled\">" + GetSharedResource("Previous") + "</span>&nbsp;&nbsp;";
                                }
                                else
                                {
                                    int x = 0;
                                    foreach (PageInfo objPage in pageList_pager)
                                    {
                                        if (objPage.PageID == pageID)
                                        {
                                            pager = pager + "<a class=\"CommandButton\" href=\"" + Common.GetModuleLink(ArticleModule.TabID, ArticleModule.ModuleID, "ArticleView", ArticleSettings, "ArticleID=" + objArticle.ArticleID.ToString(), "PageID=" + ((PageInfo)pageList_pager[x - 1]).PageID.ToString()) + "\">" + GetSharedResource("Previous") + "</a>&nbsp;&nbsp;";
                                            break;
                                        }
                                        x = x + 1;
                                    }
                                }
                            }
                            else
                            {
                                pager = pager + "<span class=\"NormalDisabled\">" + GetSharedResource("Previous") + "</span>&nbsp;&nbsp;";
                            }

                            int i_pager = 1;
                            foreach (PageInfo objPage in pageList_pager)
                            {
                                if (objPage.PageID == pageID || (pageID == Null.NullInteger && i_pager == 1))
                                {
                                    pager = pager + "<span class=\"NormalDisabled\">" + i_pager.ToString() + "</span>&nbsp;&nbsp;";
                                }
                                else
                                {
                                    pager = pager + "<a class=\"CommandButton\" href=\"" + Common.GetModuleLink(ArticleModule.TabID, ArticleModule.ModuleID, "ArticleView", ArticleSettings, "ArticleID=" + objArticle.ArticleID.ToString(), "PageID=" + objPage.PageID.ToString()) + "\">" + i_pager.ToString() + "</a>&nbsp;&nbsp;";
                                }
                                i_pager = i_pager + 1;
                            }

                            if (pageList_pager.Count > 1)
                            {
                                if (pageID != Null.NullInteger)
                                {
                                    if (((PageInfo)pageList_pager[pageList_pager.Count - 1]).PageID == pageID)
                                    {
                                        pager = pager + "<span class=\"NormalDisabled\">" + GetSharedResource("Next") + "</span>&nbsp;&nbsp;";
                                    }
                                    else
                                    {
                                        int x = 0;
                                        foreach (PageInfo objPage in pageList_pager)
                                        {
                                            if (objPage.PageID == pageID)
                                            {
                                                pager = pager + "<a class=\"CommandButton\" href=\"" + Common.GetModuleLink(ArticleModule.TabID, ArticleModule.ModuleID, "ArticleView", ArticleSettings, "ArticleID=" + objArticle.ArticleID.ToString(), "PageID=" + ((PageInfo)pageList_pager[x + 1]).PageID.ToString()) + "\">" + GetSharedResource("Next") + "</a>&nbsp;&nbsp;";
                                                break;
                                            }
                                            x = x + 1;
                                        }
                                    }
                                }
                                else
                                {
                                    pager = pager + "<a class=\"CommandButton\" href=\"" + Common.GetModuleLink(ArticleModule.TabID, ArticleModule.ModuleID, "ArticleView", ArticleSettings, "ArticleID=" + objArticle.ArticleID.ToString(), "PageID=" + ((PageInfo)pageList_pager[1]).PageID.ToString()) + "\">" + GetSharedResource("Next") + "</a>&nbsp;&nbsp;";
                                }
                            }
                            else
                            {
                                pager = pager + "<span class=\"NormalDisabled\">" + GetSharedResource("Next") + "</span>&nbsp;&nbsp;";
                            }

                            if (pageList_pager.Count > 1)
                            {
                                if (((PageInfo)pageList_pager[pageList_pager.Count - 1]).PageID == pageID)
                                {
                                    pager = pager + "<span class=\"NormalDisabled\">" + GetSharedResource("Last") + "</span>&nbsp;&nbsp;";
                                }
                                else
                                {
                                    pager = pager + "<a class=\"CommandButton\" href=\"" + Common.GetModuleLink(ArticleModule.TabID, ArticleModule.ModuleID, "ArticleView", ArticleSettings, "ArticleID=" + objArticle.ArticleID.ToString(), "PageID=" + ((PageInfo)pageList_pager[pageList_pager.Count - 1]).PageID.ToString()) + "\">" + GetSharedResource("Last") + "</a>&nbsp;&nbsp;";
                                }
                            }
                            else
                            {
                                pager = pager + "<span class=\"NormalDisabled\">" + GetSharedResource("Last") + "</span>&nbsp;&nbsp;";
                            }

                            pager = pager + ""
                                + "</td>"
                                + "</tr>"
                                + "</tbody>"
                                + "</table>";

                            Literal objLiteral_pager = new Literal();
                            objLiteral_pager.Text = pager;
                            objPlaceHolder.Add(objLiteral_pager);
                            break;
                        case "PAGESLIST":
                            string pages = "";
                            PageController pageController_pagesList = new PageController();
                            ArrayList pageList_pageList = pageController_pagesList.GetPageList(objArticle.ArticleID);
                            pageID = Null.NullInteger;
                            if (Numeric.IsNumeric(Request["PageID"]))
                            {
                                pageID = Convert.ToInt32(Request["PageID"]);
                            }

                            pages = "<ul>";
                            foreach (PageInfo objPage in pageList_pageList)
                            {
                                pages = pages + "<li><a href='" + Common.GetModuleLink(ArticleModule.TabID, ArticleModule.ModuleID, "ArticleView", ArticleSettings, "ArticleID=" + objArticle.ArticleID.ToString(), "PageID=" + objPage.PageID.ToString()) + "'>" + objPage.Title + "</a></li>";
                            }
                            pages = pages + "</ul>";

                            if (pageList_pageList.Count > 1)
                            {
                                objLiteral = new Literal();
                                objLiteral.Text = pages;
                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "PAGESLIST2":
                            pages = "";
                            PageController pageController_pageList2 = new PageController();
                            ArrayList pageList2 = pageController_pageList2.GetPageList(objArticle.ArticleID);
                            pageID = Null.NullInteger;
                            if (Numeric.IsNumeric(Request["PageID"]))
                            {
                                pageID = Convert.ToInt32(Request["PageID"]);
                            }

                            pages = "<ul>";
                            bool isFirst = true;
                            foreach (PageInfo objPage in pageList2)
                            {
                                if (pageID == objPage.PageID || (pageID == Null.NullInteger && isFirst))
                                {
                                    pages = pages + "<li>" + objPage.Title + "</li>";
                                }
                                else
                                {
                                    pages = pages + "<li><a href='" + Common.GetModuleLink(ArticleModule.TabID, ArticleModule.ModuleID, "ArticleView", ArticleSettings, "ArticleID=" + objArticle.ArticleID.ToString(), "PageID=" + objPage.PageID.ToString()) + "'>" + objPage.Title + "</a></li>";
                                }
                                isFirst = false;
                            }
                            pages = pages + "</ul>";

                            if (pageList2.Count > 1)
                            {
                                objLiteral = new Literal();
                                objLiteral.Text = pages;
                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "PORTALROOT":
                            Literal objLiteral_portRoot = new Literal();
                            objLiteral_portRoot.Text = PortalSettings.HomeDirectory;
                            objPlaceHolder.Add(objLiteral_portRoot);
                            break;
                        case "POSTCOMMENT":
                            if (ArticleSettings.IsCommentsEnabled)
                            {
                                Control objControl = Page.LoadControl(@"~/DesktopModules/GcDesign-NewsArticles/Controls/PostComment.ascx");
                                ((NewsArticleControlBase)objControl).ArticleID = objArticle.ArticleID;
                                objPlaceHolder.Add(objControl);
                            }
                            break;
                        case "POSTCOMMENT1":
                            if (ArticleSettings.IsCommentsEnabled)
                            {
                                Control objControl = Page.LoadControl(@"~/DesktopModules/GcDesign-NewsArticles/Controls/PostComment1.ascx");
                                ((NewsArticleControlBase)objControl).ArticleID = objArticle.ArticleID;
                                objPlaceHolder.Add(objControl);
                            }
                            break;
                        case "POSTCOMMENT2":
                            if (ArticleSettings.IsCommentsEnabled)
                            {
                                Control objControl = Page.LoadControl(@"~/DesktopModules/GcDesign-NewsArticles/Controls/PostComment2.ascx");
                                ((NewsArticleControlBase)objControl).ArticleID = objArticle.ArticleID;
                                objPlaceHolder.Add(objControl);
                            }
                            break;
                        case "POSTCOMMENT3":
                            if (ArticleSettings.IsCommentsEnabled)
                            {
                                Control objControl = Page.LoadControl(@"~/DesktopModules/GcDesign-NewsArticles/Controls/PostComment3.ascx");
                                ((NewsArticleControlBase)objControl).ArticleID = objArticle.ArticleID;
                                objPlaceHolder.Add(objControl);
                            }
                            break;
                        case "POSTRATING":
                            Control objControl_postRating = Page.LoadControl(@"~/DesktopModules/GcDesign-NewsArticles/Controls/PostRating.ascx");
                            objPlaceHolder.Add(objControl_postRating);
                            break;
                        case "PRINT":
                            HyperLink objHyperLink_print = new HyperLink();
                            if (_pageId != Null.NullInteger)
                            {
                                objHyperLink_print.NavigateUrl = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Print.aspx?tabid=" + ArticleModule.TabID.ToString() + "&tabmoduleid=" + ArticleModule.TabModuleID.ToString() + "&articleId=" + objArticle.ArticleID.ToString() + "&moduleId=" + objArticle.ModuleID.ToString() + "&PortalID=" + PortalSettings.PortalId.ToString() + "&PageID=" + _pageId.ToString());
                            }
                            else
                            {
                                objHyperLink_print.NavigateUrl = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Print.aspx?tabid=" + ArticleModule.TabID.ToString() + "&tabmoduleid=" + ArticleModule.TabModuleID.ToString() + "&articleId=" + objArticle.ArticleID.ToString() + "&moduleId=" + objArticle.ModuleID.ToString() + "&PortalID=" + PortalSettings.PortalId.ToString());
                            }
                            objHyperLink_print.ImageUrl = "~/images/print.gif";
                            objHyperLink_print.ToolTip = GetArticleResource("ClickPrint");
                            objHyperLink_print.EnableViewState = false;
                            objHyperLink_print.Target = "_blank";
                            objHyperLink_print.Attributes.Add("rel", "nofollow");
                            objPlaceHolder.Add(objHyperLink_print);
                            break;
                        case "PRINTLINK":
                            Literal objLiteral_printLink = new Literal();
                            if (_pageId != Null.NullInteger)
                            {
                                objLiteral_printLink.Text = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Print.aspx?tabid=" + ArticleModule.TabID.ToString() + "&tabmoduleid=" + ArticleModule.TabModuleID.ToString() + "&articleId=" + objArticle.ArticleID.ToString() + "&moduleId=" + objArticle.ModuleID.ToString() + "&PortalID=" + PortalSettings.PortalId.ToString() + "&PageID=" + _pageId.ToString());
                            }
                            else
                            {
                                objLiteral_printLink.Text = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/Print.aspx?tabid=" + ArticleModule.TabID.ToString() + "&tabmoduleid=" + ArticleModule.TabModuleID.ToString() + "&articleId=" + objArticle.ArticleID.ToString() + "&moduleId=" + objArticle.ModuleID.ToString() + "&PortalID=" + PortalSettings.PortalId.ToString());
                            }
                            objLiteral_printLink.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral_printLink);
                            break;
                        case "PUBLISHDATE":
                            Literal objLiteral_publishDate = new Literal();
                            if (objArticle.StartDate == Null.NullDate)
                            {
                                objLiteral_publishDate.Text = objArticle.CreatedDate.ToString("D");
                            }
                            else
                            {
                                objLiteral_publishDate.Text = objArticle.StartDate.ToString("D");
                            }
                            objLiteral_publishDate.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral_publishDate);
                            break;
                        case "PUBLISHSTARTDATE":
                            Literal publishStartDate = new Literal();
                            if (objArticle.StartDate == Null.NullDate)
                            {
                                publishStartDate.Text = objArticle.CreatedDate.ToString("D");
                            }
                            else
                            {
                                publishStartDate.Text = objArticle.StartDate.ToString("D");
                            }
                            publishStartDate.EnableViewState = false;
                            objPlaceHolder.Add(publishStartDate);
                            break;
                        case "PUBLISHTIME":
                            Literal publishTime = new Literal();
                            if (objArticle.StartDate == Null.NullDate)
                            {
                                publishTime.Text = objArticle.CreatedDate.ToString("t");
                            }
                            else
                            {
                                publishTime.Text = objArticle.StartDate.ToString("t");
                            }
                            publishTime.EnableViewState = false;
                            break;
                        case "PUBLISHSTARTTIME":
                            Literal publishStartTime = new Literal();
                            if (objArticle.StartDate == Null.NullDate)
                            {
                                publishStartTime.Text = objArticle.CreatedDate.ToString("t");
                            }
                            else
                            {
                                publishStartTime.Text = objArticle.StartDate.ToString("t");
                            }
                            publishStartTime.EnableViewState = false;
                            objPlaceHolder.Add(publishStartTime);
                            break;
                        case "PUBLISHENDDATE":
                            Literal publishEndDate = new Literal();
                            if (objArticle.EndDate == Null.NullDate)
                            {
                                publishEndDate.Text = "";
                            }
                            else
                            {
                                publishEndDate.Text = objArticle.EndDate.ToString("D");
                            }
                            objPlaceHolder.Add(publishEndDate);
                            break;
                        case "PUBLISHENDTIME":
                            Literal publishEndTime = new Literal();
                            if (objArticle.EndDate == Null.NullDate)
                            {
                                publishEndTime.Text = "";
                            }
                            else
                            {
                                publishEndTime.Text = objArticle.EndDate.ToString("t");
                            }
                            publishEndTime.EnableViewState = false;
                            objPlaceHolder.Add(publishEndTime);
                            break;
                        case "RATING":
                            objImage = new Image();
                            objImage.ImageUrl = GetRatingImage(objArticle);
                            objImage.EnableViewState = false;
                            objImage.ToolTip = "Article Rating";
                            objImage.AlternateText = "Article Rating";
                            objPlaceHolder.Add(objImage);
                            break;
                        case "RATINGCOUNT":
                            objLiteral = new Literal();
                            objLiteral.Text = objArticle.RatingCount.ToString();
                            objLiteral.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "RATINGDETAIL":
                            if (objArticle.Rating != Null.NullDouble)
                            {
                                objLiteral = new Literal();
                                objLiteral.Text = objArticle.Rating.ToString("R1");
                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "RELATED":
                            if (ArticleSettings.RelatedMode != RelatedType.None)
                            {
                                PlaceHolder phRelated = new PlaceHolder();

                                List<ArticleInfo> objArticles = GetRelatedArticles(objArticle, 5);

                                if (objArticles.Count > 0)
                                {
                                    LayoutInfo _objLayoutRelatedHeader = GetLayout(ArticleSettings, ArticleModule, Page, LayoutType.Related_Header_Html);
                                    LayoutInfo _objLayoutRelatedItem = GetLayout(ArticleSettings, ArticleModule, Page, LayoutType.Related_Item_Html);
                                    LayoutInfo _objLayoutRelatedFooter = GetLayout(ArticleSettings, ArticleModule, Page, LayoutType.Related_Footer_Html);

                                    ProcessArticleItem(phRelated.Controls, _objLayoutRelatedHeader.Tokens, objArticle);
                                    foreach (ArticleInfo objRelatedArticle in objArticles)
                                    {
                                        ProcessArticleItem(phRelated.Controls, _objLayoutRelatedItem.Tokens, objRelatedArticle);
                                    }
                                    ProcessArticleItem(phRelated.Controls, _objLayoutRelatedFooter.Tokens, objArticle);

                                    objPlaceHolder.Add(phRelated);
                                }
                            }
                            break;
                        case "SITEROOT":
                            objLiteral = new Literal();
                            objLiteral.Text = ArticleUtilities.ResolveUrl("~/");
                            objLiteral.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "SITETITLE":
                            objLiteral = new Literal();
                            objLiteral.Text = PortalSettings.PortalName;
                            objLiteral.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "SHORTLINK":
                            if (objArticle.ShortUrl != "")
                            {
                                objLiteral = new Literal();
                                objLiteral.Text = objArticle.ShortUrl;
                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                            }
                            else
                            {
                                if (ArticleSettings.TwitterBitLyAPIKey != "" && ArticleSettings.TwitterBitLyLogin != "")
                                {
                                    string link = Common.GetArticleLink(objArticle, Tab, ArticleSettings, false);
                                    Bitly b = new Bitly(ArticleSettings.TwitterBitLyLogin, ArticleSettings.TwitterBitLyAPIKey);
                                    string shortUrl = b.Shorten(link);

                                    if (shortUrl != "")
                                    {
                                        objLiteral = new Literal();
                                        objLiteral.Text = shortUrl;
                                        objLiteral.EnableViewState = false;
                                        objPlaceHolder.Add(objLiteral);

                                        objArticle.ShortUrl = shortUrl;
                                        ArticleController objArticleController = new ArticleController();
                                        objArticleController.UpdateArticle(objArticle);
                                    }
                                }
                            }
                            break;
                        case "SUMMARY":
                            objLiteral = new Literal();
                            objLiteral.Text = ProcessPostTokens(Server.HtmlDecode(objArticle.Summary), objArticle, ref Generator, ArticleSettings);
                            objLiteral.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TABID":
                            objLiteral = new Literal();
                            objLiteral.Text = ArticleModule.TabID.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TABTITLE":
                            objLiteral = new Literal();
                            if (PortalSettings.ActiveTab.Title.Length == 0)
                            {
                                objLiteral.Text = PortalSettings.ActiveTab.TabName;
                            }
                            else
                            {
                                objLiteral.Text = PortalSettings.ActiveTab.Title;
                            }
                            objLiteral.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TAGS":
                            if (objArticle.Tags.Trim() != "")
                            {
                                objLiteral = new Literal();
                                foreach (string tag in objArticle.Tags.Split(','))
                                {
                                    if (objLiteral.Text == "")
                                    {
                                        objLiteral.Text = "<a href=\"" + Common.GetModuleLink(ArticleModule.TabID, objArticle.ModuleID, "TagView", ArticleSettings, "Tag=" + Server.UrlEncode(tag)) + "\">" + tag + "</a>";
                                    }
                                    else
                                    {
                                        objLiteral.Text = objLiteral.Text + ", " + "<a href=\"" + Common.GetModuleLink(ArticleModule.TabID, objArticle.ModuleID, "TagView", ArticleSettings, "Tag=" + Server.UrlEncode(tag)) + "\">" + tag + "</a>";
                                    }
                                }
                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "TAGSNOLINK":
                            if (objArticle.Tags.Trim() != "")
                            {
                                objLiteral = new Literal();
                                foreach (string tag in objArticle.Tags.Split('c'))
                                {
                                    if (objLiteral.Text == "")
                                    {
                                        objLiteral.Text = tag;
                                    }
                                    else
                                    {
                                        objLiteral.Text = objLiteral.Text + ", " + tag;
                                    }
                                }
                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "TEMPLATEPATH":
                            objLiteral = new Literal();
                            objLiteral.Text = ArticleUtilities.ResolveUrl("~/GcDesign-NewsArticles/Templates/" + ArticleSettings.Template + "/");
                            objLiteral.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TITLE":
                            objLiteral = new Literal();
                            objLiteral.Text = objArticle.Title;
                            objLiteral.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TITLESAFEJS":
                            if (objArticle.Title != "")
                            {
                                objLiteral = new Literal();
                                objLiteral.Text = objArticle.Title.Replace("\"\"", "");
                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                            }
                            break;
                        case "TITLEURLENCODED":
                            objLiteral = new Literal();
                            objLiteral.Text = Server.UrlEncode(objArticle.Title);
                            objLiteral.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TWITTERNAME":
                            objLiteral = new Literal();
                            objLiteral.Text = ArticleSettings.TwitterName;
                            objLiteral.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "UPDATEDATE":
                            objLiteral = new Literal();
                            objLiteral.Text = objArticle.LastUpdate.ToString("D");
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "UPDATETIME":
                            objLiteral = new Literal();
                            objLiteral.Text = objArticle.LastUpdate.ToString("t");
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "VIEWCOUNT":
                            objLiteral = new Literal();
                            objLiteral.Text = objArticle.NumberOfViews.ToString();
                            objLiteral.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        default:

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("AUTHOR:"))
                            {
                                if (Author(objArticle.AuthorID) != null)
                                {
                                    // token to be processed
                                    string field = layoutArray[iPtr + 1].Substring(7, layoutArray[iPtr + 1].Length - 7).ToLower().Trim();

                                    //Gets the DNN profile property named like the token (field)
                                    bool profilePropertyFound = false;
                                    string profilePropertyDataType = string.Empty;
                                    string profilePropertyName = string.Empty;
                                    string profilePropertyValue = string.Empty;

                                    foreach (ProfilePropertyDefinition objProfilePropertyDefinition in ProfileProperties)
                                    {
                                        if (objProfilePropertyDefinition.PropertyName.ToLower().Trim() == field)
                                        {

                                            //Gets the dnn profile property's datatype
                                            ListController objListController = new ListController();
                                            ListEntryInfo definitionEntry = objListController.GetListEntryInfo(objProfilePropertyDefinition.DataType);
                                            if (definitionEntry != null)
                                            {
                                                profilePropertyDataType = definitionEntry.Value;
                                            }
                                            else
                                            {
                                                profilePropertyDataType = "Unknown";
                                            }

                                            //Gets the dnn profile property's name and current value for the given user (Agent = AuthorID)
                                            profilePropertyName = objProfilePropertyDefinition.PropertyName;
                                            profilePropertyValue = Author(objArticle.AuthorID).Profile.GetPropertyValue(profilePropertyName);

                                            profilePropertyFound = true;

                                        }
                                    }

                                    if (profilePropertyFound)
                                    {

                                        switch (profilePropertyDataType.ToLower())
                                        {
                                            case "truefalse":
                                                CheckBox objTrueFalse = new CheckBox();
                                                if (profilePropertyValue == string.Empty)
                                                {
                                                    objTrueFalse.Checked = false;
                                                }
                                                else
                                                {
                                                    objTrueFalse.Checked = Convert.ToBoolean(profilePropertyValue);
                                                }
                                                objTrueFalse.Enabled = false;
                                                objTrueFalse.EnableViewState = false;
                                                objPlaceHolder.Add(objTrueFalse);
                                                break;
                                            case "richtext":
                                                objLiteral = new Literal();
                                                if (profilePropertyValue == string.Empty)
                                                {
                                                    objLiteral.Text = string.Empty;
                                                }
                                                else
                                                {
                                                    objLiteral.Text = Server.HtmlDecode(profilePropertyValue);
                                                }
                                                objLiteral.EnableViewState = false;
                                                objPlaceHolder.Add(objLiteral);
                                                break;
                                            case "list":
                                                objLiteral = new Literal();
                                                objLiteral.Text = profilePropertyValue;
                                                ListController objListController = new ListController();
                                                List<ListEntryInfo> objListEntryInfoCollection = objListController.GetListEntryInfoItems(profilePropertyName).ToList();
                                                foreach (ListEntryInfo objListEntryInfo in objListEntryInfoCollection)
                                                {
                                                    if (objListEntryInfo.Value == profilePropertyValue)
                                                    {
                                                        objLiteral.Text = objListEntryInfo.Text;
                                                        break;
                                                    }
                                                }
                                                objLiteral.EnableViewState = false;
                                                objPlaceHolder.Add(objLiteral);
                                                break;
                                            default:
                                                objLiteral = new Literal();
                                                if (profilePropertyValue == string.Empty)
                                                {
                                                    objLiteral.Text = string.Empty;
                                                }
                                                else
                                                {
                                                    if (profilePropertyName.ToLower() == "website")
                                                    {
                                                        string url = profilePropertyValue;
                                                        if (url.ToLower().StartsWith("http://"))
                                                        {
                                                            url = url.Substring(7); // removes the "http://"
                                                        }
                                                        objLiteral.Text = url;
                                                    }
                                                    else
                                                    {
                                                        objLiteral.Text = profilePropertyValue;
                                                    }
                                                }
                                                objLiteral.EnableViewState = false;
                                                objPlaceHolder.Add(objLiteral);
                                                break;
                                        } //profilePropertyDataType

                                    } // DNN Profile property processing
                                }
                                break;
                            } // "AUTHOR:" token

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("CAPTION:"))
                            {

                                objCustomFieldController = new CustomFieldController();
                                objCustomFields = objCustomFieldController.List(objArticle.ModuleID);

                                string field = layoutArray[iPtr + 1].Substring(8, layoutArray[iPtr + 1].Length - 8);

                                int i = 0;
                                foreach (CustomFieldInfo objCustomField in objCustomFields)
                                {
                                    if (objCustomField.Name.ToLower() == field.ToLower())
                                    {
                                        if (objArticle.CustomList.Contains(objCustomField.CustomFieldID))
                                        {
                                            objLiteral = new Literal();
                                            objLiteral.Text = objCustomField.Caption;
                                            objLiteral.EnableViewState = false;
                                            objPlaceHolder.Add(objLiteral);
                                            i = i + 1;
                                        }
                                    }
                                }

                                break;

                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("CATEGORIESSUB:"))
                            {
                                string values = layoutArray[iPtr + 1].Substring(14, layoutArray[iPtr + 1].Length - 14);

                                string[] splitValues = values.Split(':');

                                if (splitValues.Length == 2)
                                {

                                    string category = splitValues[0];
                                    string number = splitValues[1];

                                    if (Numeric.IsNumeric(number))
                                    {
                                        if (Convert.ToInt32(number) > 0)
                                        {

                                            // Find category

                                            CategoryController objCategoryController = new CategoryController();
                                            List<CategoryInfo> objCategories = objCategoryController.GetCategoriesAll(ArticleModule.ModuleID, Null.NullInteger);

                                            int categoryID = Null.NullInteger;
                                            foreach (CategoryInfo objCategory in objCategories)
                                            {

                                                if (objCategory.Name.ToLower() == category.ToLower())
                                                {
                                                    categoryID = objCategory.CategoryID;
                                                    break;
                                                }

                                            }

                                            if (categoryID != Null.NullInteger)
                                            {

                                                List<CategoryInfo> objCategoriesSelected = objCategoryController.GetCategoriesAll(ArticleModule.ModuleID, categoryID, null, Null.NullInteger, Convert.ToInt32(number), false, CategorySortType.Name);

                                                objArticleCategories = (ArrayList)DataCache.GetCache(ArticleConstants.CACHE_CATEGORY_ARTICLE + objArticle.ArticleID.ToString());
                                                if (objArticleCategories == null)
                                                {
                                                    ArticleController objArticleController = new ArticleController();
                                                    objArticleCategories = objArticleController.GetArticleCategories(objArticle.ArticleID);
                                                    DataCache.SetCache(ArticleConstants.CACHE_CATEGORY_ARTICLE + objArticle.ArticleID.ToString(), objArticleCategories);
                                                }


                                                objLiteral = new Literal();
                                                foreach (CategoryInfo objCategory in objArticleCategories)
                                                {

                                                    foreach (CategoryInfo objCategorySel in objCategoriesSelected)
                                                    {
                                                        if (objCategory.CategoryID == objCategorySel.CategoryID)
                                                        {
                                                            if (objLiteral.Text != "")
                                                            {
                                                                objLiteral.Text = objLiteral.Text + ", <a href=\"" + Common.GetCategoryLink(ArticleModule.TabID, ArticleModule.ModuleID, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings) + "\" > " + objCategory.Name + "</a>";
                                                            }
                                                            else
                                                            {
                                                                objLiteral.Text = "<a href=\"" + Common.GetCategoryLink(ArticleModule.TabID, ArticleModule.ModuleID, objCategory.CategoryID.ToString(), objCategory.Name, ArticleSettings.LaunchLinks, ArticleSettings) + "\" > " + objCategory.Name + "</a>";
                                                            }
                                                        }
                                                    }

                                                }
                                                objLiteral.EnableViewState = false;
                                                objPlaceHolder.Add(objLiteral);
                                                break;

                                            }
                                        }

                                    }

                                }

                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("CREATEDATE:"))
                            {
                                string formatExpression = layoutArray[iPtr + 1].Substring(11, layoutArray[iPtr + 1].Length - 11);

                                objLiteral = new Literal();

                                try
                                {
                                    if (objArticle.CreatedDate == Null.NullDate)
                                    {
                                        objLiteral.Text = "";
                                    }
                                    else
                                    {
                                        objLiteral.Text = objArticle.CreatedDate.ToString(formatExpression);
                                    }
                                }
                                catch
                                {
                                    if (objArticle.CreatedDate == Null.NullDate)
                                    {
                                        objLiteral.Text = "";
                                    }
                                    else
                                    {
                                        objLiteral.Text = objArticle.CreatedDate.ToString("D");
                                    }
                                }

                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("CREATEDATELESSTHAN:"))
                            {
                                int length = Convert.ToInt32(layoutArray[iPtr + 1].Substring(19, layoutArray[iPtr + 1].Length - 19));

                                if (objArticle.CreatedDate < DateTime.Now.AddDays(length * -1))
                                {
                                    string endVal = layoutArray[iPtr + 1].ToUpper();
                                    while (iPtr < layoutArray.Length - 1)
                                    {
                                        if (layoutArray[iPtr + 1] == ("/" + endVal))
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("/CREATEDATELESSTHAN:"))
                            {
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("CUSTOM:"))
                            {
                                string field = layoutArray[iPtr + 1].Substring(7, layoutArray[iPtr + 1].Length - 7).ToLower();

                                int customFieldID = Null.NullInteger;
                                CustomFieldInfo objCustomFieldSelected = new CustomFieldInfo();
                                //bool isLink = false;

                                objCustomFieldController = new CustomFieldController();
                                objCustomFields = objCustomFieldController.List(objArticle.ModuleID);

                                int maxLength = Null.NullInteger;
                                if (field.IndexOf(':') != -1)
                                {
                                    try
                                    {
                                        maxLength = Convert.ToInt32(field.Split(':')[1]);
                                    }
                                    catch
                                    {
                                        maxLength = Null.NullInteger;
                                    }
                                    field = field.Split(':')[0];
                                }
                                if (customFieldID == Null.NullInteger)
                                {
                                    foreach (CustomFieldInfo objCustomField in objCustomFields)
                                    {
                                        if (objCustomField.Name.ToLower() == field.ToLower())
                                        {
                                            customFieldID = objCustomField.CustomFieldID;
                                            objCustomFieldSelected = objCustomField;
                                        }
                                    }
                                }

                                if (customFieldID != Null.NullInteger)
                                {

                                    int i = 0;
                                    if (objArticle.CustomList.Contains(customFieldID))
                                    {
                                        objLiteral = new Literal();
                                        string fieldValue = GetFieldValue(objCustomFieldSelected, objArticle, false);
                                        if (maxLength != Null.NullInteger)
                                        {
                                            if (fieldValue.Length > maxLength)
                                            {
                                                fieldValue = fieldValue.Substring(0, maxLength);
                                            }
                                        }
                                        objLiteral.Text = fieldValue.TrimStart('#');
                                        objLiteral.EnableViewState = false;
                                        objPlaceHolder.Add(objLiteral);
                                        i = i + 1;
                                    }
                                }
                                break;
                            }


                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("DETAILS:"))
                            {
                                int length = Convert.ToInt32(layoutArray[iPtr + 1].Substring(8, layoutArray[iPtr + 1].Length - 8));

                                objLiteral = new Literal();
                                if (StripHtml(Server.HtmlDecode(objArticle.Body)).TrimStart().Length > length)
                                {
                                    objLiteral.Text = ProcessPostTokens(StripHtml(Server.HtmlDecode(objArticle.Body)).TrimStart().Substring(0, length), objArticle, ref Generator, ArticleSettings) + "...";
                                }
                                else
                                {
                                    objLiteral.Text = ProcessPostTokens(StripHtml(Server.HtmlDecode(objArticle.Body)).TrimStart().Substring(0, StripHtml(Server.HtmlDecode(objArticle.Body)).TrimStart().Length), objArticle, ref Generator, ArticleSettings);
                                }

                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("EXPRESSION:"))
                            {

                                objCustomFieldController = new CustomFieldController();
                                objCustomFields = objCustomFieldController.List(objArticle.ModuleID);

                                string field = layoutArray[iPtr + 1].Substring(11, layoutArray[iPtr + 1].Length - 11);

                                string[] paramses = field.Split(':');

                                if (paramses.Length != 3)
                                {
                                    string endToken = "/" + layoutArray[iPtr + 1];
                                    while (iPtr < layoutArray.Length - 1)
                                    {
                                        if (layoutArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                    break;
                                }

                                string customField = paramses[0];
                                string customExpression = paramses[1];
                                string customValue = paramses[2];

                                string fieldValue = "";

                                foreach (CustomFieldInfo objCustomField in objCustomFields)
                                {
                                    if (objCustomField.Name.ToLower() == customField.ToLower())
                                    {
                                        if (objArticle.CustomList.Contains(objCustomField.CustomFieldID))
                                        {
                                            fieldValue = GetFieldValue(objCustomField, objArticle, false);
                                        }
                                    }
                                }

                                bool isValid = false;
                                switch (customExpression)
                                {
                                    case "=":
                                        if (customValue.ToLower() == fieldValue.ToLower())
                                        {
                                            isValid = true;
                                        }
                                        break;

                                    case "!=":
                                        if (customValue.ToLower() != fieldValue.ToLower())
                                        {
                                            isValid = true;
                                        }
                                        break;

                                    case "<":
                                        if (Numeric.IsNumeric(customValue) && Numeric.IsNumeric(fieldValue))
                                        {
                                            if (Convert.ToInt32(fieldValue) < Convert.ToInt32(customValue))
                                            {
                                                isValid = true;
                                            }
                                        }
                                        break;

                                    case "<=":
                                        if (Numeric.IsNumeric(customValue) && Numeric.IsNumeric(fieldValue))
                                        {
                                            if (Convert.ToInt32(fieldValue) <= Convert.ToInt32(customValue))
                                            {
                                                isValid = true;
                                            }
                                        }
                                        break;

                                    case ">":
                                        if (Numeric.IsNumeric(customValue) && Numeric.IsNumeric(fieldValue))
                                        {
                                            if (Convert.ToInt32(fieldValue) > Convert.ToInt32(customValue))
                                            {
                                                isValid = true;
                                            }
                                        }
                                        break;

                                    case ">=":
                                        if (Numeric.IsNumeric(customValue) && Numeric.IsNumeric(fieldValue))
                                        {
                                            if (Convert.ToInt32(fieldValue) >= Convert.ToInt32(customValue))
                                            {
                                                isValid = true;
                                            }
                                        }
                                        break;

                                }

                                if (!isValid)
                                {
                                    string endToken = "/" + layoutArray[iPtr + 1];
                                    while (iPtr < layoutArray.Length - 1)
                                    {
                                        if (layoutArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }

                                break;

                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("/EXPRESSION:"))
                            {
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("HASIMAGES:"))
                            {

                                string field = layoutArray[iPtr + 1].Substring(10, layoutArray[iPtr + 1].Length - 10);

                                if (Numeric.IsNumeric(field))
                                {

                                    if (objArticle.ImageCount < Convert.ToInt32(field))
                                    {
                                        string endToken = "/" + layoutArray[iPtr + 1];
                                        while (iPtr < layoutArray.Length - 1)
                                        {
                                            if (layoutArray[iPtr + 1] == endToken)
                                            {
                                                break;
                                            }
                                            iPtr = iPtr + 1;
                                        }
                                    }

                                }

                                break;

                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("/HASIMAGES:"))
                            {
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("HASMOREDETAIL:"))
                            {
                                int length = Convert.ToInt32(layoutArray[iPtr + 1].Substring(14, layoutArray[iPtr + 1].Length - 14));
                                string endToken = "/" + layoutArray[iPtr + 1];

                                if (objArticle.Url == Null.NullString && StripHtml(objArticle.Summary.Trim()) == "" && StripHtml(Server.HtmlDecode(objArticle.Body)).TrimStart().Length <= length)
                                {
                                    while (iPtr < layoutArray.Length - 1)
                                    {
                                        if (layoutArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("/HASMOREDETAIL:"))
                            {
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("HASNOVALUE:"))
                            {

                                objCustomFieldController = new CustomFieldController();
                                objCustomFields = objCustomFieldController.List(objArticle.ModuleID);

                                string field = layoutArray[iPtr + 1].Substring(11, layoutArray[iPtr + 1].Length - 11);

                                foreach (CustomFieldInfo objCustomField in objCustomFields)
                                {
                                    if (objCustomField.Name.ToLower() == field.ToLower())
                                    {
                                        if (objArticle.CustomList.Contains(objCustomField.CustomFieldID))
                                        {
                                            string fieldValue = GetFieldValue(objCustomField, objArticle, false);
                                            if (fieldValue.Trim() != "")
                                            {
                                                string endToken = "/" + layoutArray[iPtr + 1];
                                                while (iPtr < layoutArray.Length - 1)
                                                {
                                                    if (layoutArray[iPtr + 1] == endToken)
                                                    {
                                                        break;
                                                    }
                                                    iPtr = iPtr + 1;
                                                }
                                            }
                                        }
                                    }
                                }

                                break;

                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("/HASNOVALUE:"))
                            {
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("HASVALUE:"))
                            {

                                objCustomFieldController = new CustomFieldController();
                                objCustomFields = objCustomFieldController.List(objArticle.ModuleID);

                                string field = layoutArray[iPtr + 1].Substring(9, layoutArray[iPtr + 1].Length - 9);

                                foreach (CustomFieldInfo objCustomField in objCustomFields)
                                {
                                    if (objCustomField.Name.ToLower() == field.ToLower())
                                    {
                                        if (objArticle.CustomList.Contains(objCustomField.CustomFieldID))
                                        {
                                            string fieldValue = GetFieldValue(objCustomField, objArticle, false);
                                            if (fieldValue.Trim() == "")
                                            {
                                                string endToken = "/" + layoutArray[iPtr + 1];
                                                while (iPtr < layoutArray.Length - 1)
                                                {
                                                    if (layoutArray[iPtr + 1] == endToken)
                                                    {
                                                        break;
                                                    }
                                                    iPtr = iPtr + 1;
                                                }
                                            }
                                        }
                                    }
                                }

                                break;

                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("/HASVALUE:"))
                            {
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("HASAUTHORVALUE:"))
                            {

                                string field = layoutArray[iPtr + 1].Substring(15, layoutArray[iPtr + 1].Length - 15).ToLower().Trim();

                                //Gets the DNN profile property named like the token (field)
                                string profilePropertyName = string.Empty;
                                string profilePropertyValue = string.Empty;

                                foreach (ProfilePropertyDefinition objProfilePropertyDefinition in ProfileProperties)
                                {
                                    if (objProfilePropertyDefinition.PropertyName.ToLower().Trim() == field)
                                    {

                                        //Gets the dnn profile property's datatype
                                        ListController objListController = new ListController();
                                        ListEntryInfo definitionEntry = objListController.GetListEntryInfo(objProfilePropertyDefinition.DataType);
                                        if (definitionEntry != null)
                                        {
                                        }
                                        else
                                        {
                                        }

                                        //Gets the dnn profile property's name and current value for the given user (Agent = AuthorID)
                                        profilePropertyName = objProfilePropertyDefinition.PropertyName;
                                        if (Author(objArticle.AuthorID) != null)
                                        {
                                            profilePropertyValue = Author(objArticle.AuthorID).Profile.GetPropertyValue(profilePropertyName);
                                            break;
                                        }

                                    }
                                }

                                if (profilePropertyValue == "")
                                {
                                    string endToken = "/" + layoutArray[iPtr + 1];
                                    while (iPtr < layoutArray.Length - 1)
                                    {
                                        if (layoutArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }

                                break;

                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("/HASAUTHORVALUE:"))
                            {
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("IMAGE:"))
                            {
                                if (objArticle.ImageCount > 0)
                                {
                                    string val = layoutArray[iPtr + 1].Substring(6, layoutArray[iPtr + 1].Length - 6);

                                    if (Numeric.IsNumeric(val))
                                    {
                                        ImageController objImageController = new ImageController();
                                        List<ImageInfo> objImages = objImageController.GetImageList(objArticle.ArticleID, Null.NullString);

                                        if (objImages.Count > 0)
                                        {
                                            int count = 1;
                                            foreach (ImageInfo objChildImage in objImages)
                                            {
                                                if (count == Convert.ToInt32(val))
                                                {
                                                    objImage = new Image();
                                                    objImage.ImageUrl = PortalSettings.HomeDirectory + objChildImage.Folder + objChildImage.FileName;
                                                    objImage.EnableViewState = false;
                                                    objImage.AlternateText = objArticle.Title;
                                                    objPlaceHolder.Add(objImage);
                                                }
                                                count = count + 1;
                                            }
                                        }
                                    }
                                }
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("IMAGETHUMB:"))
                            {

                                if (objArticle.ImageUrl != "")
                                {
                                    objImage = new Image();
                                    objImage.ImageUrl = objArticle.ImageUrl;
                                    objImage.EnableViewState = false;
                                    objImage.AlternateText = objArticle.Title;
                                    objPlaceHolder.Add(objImage);
                                }
                                else
                                {
                                    if (objArticle.ImageCount > 0)
                                    {

                                        ImageController objImageController = new ImageController();
                                        List<ImageInfo> objImages = objImageController.GetImageList(objArticle.ArticleID, Null.NullString);
                                        if (objImages.Count > 0)
                                        {

                                            string val = layoutArray[iPtr + 1].Substring(11, layoutArray[iPtr + 1].Length - 11);
                                            if (val.IndexOf(':') == -1)
                                            {
                                                int length = Convert.ToInt32(val);

                                                objImage = new Image();
                                                if (ArticleSettings.ImageThumbnailType == ThumbnailType.Proportion)
                                                {
                                                    objImage.ImageUrl = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + length.ToString() + "&Height=" + length.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(objImages[0].Folder + objImages[0].FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1");
                                                }
                                                else
                                                {
                                                    objImage.ImageUrl = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + length.ToString() + "&Height=" + length.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(objImages[0].Folder + objImages[0].FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1&s=1");
                                                }
                                                objImage.EnableViewState = false;
                                                objImage.AlternateText = objArticle.Title;
                                                objPlaceHolder.Add(objImage);
                                            }
                                            else
                                            {

                                                string[] arr = val.Split(':');

                                                if (arr.Length == 2)
                                                {
                                                    int width = Convert.ToInt32(val.Split(':')[0]);
                                                    int height = Convert.ToInt32(val.Split(':')[1]);

                                                    objImage = new Image();
                                                    if (ArticleSettings.ImageThumbnailType == ThumbnailType.Proportion)
                                                    {
                                                        objImage.ImageUrl = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(objImages[0].Folder + objImages[0].FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1");
                                                    }
                                                    else
                                                    {
                                                        objImage.ImageUrl = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(objImages[0].Folder + objImages[0].FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1&s=1");
                                                    }
                                                    objImage.EnableViewState = false;
                                                    objImage.AlternateText = objArticle.Title;
                                                    objPlaceHolder.Add(objImage);
                                                }
                                                else
                                                {
                                                    if (arr.Length == 3)
                                                    {
                                                        int width = Convert.ToInt32(val.Split(':')[0]);
                                                        int height = Convert.ToInt32(val.Split(':')[1]);
                                                        int item = Convert.ToInt32(val.Split(':')[2]);

                                                        if (objImages.Count > 0)
                                                        {
                                                            int count = 1;
                                                            foreach (ImageInfo objChildImage in objImages)
                                                            {
                                                                if (count == item)
                                                                {
                                                                    objImage = new Image();
                                                                    if (ArticleSettings.ImageThumbnailType == ThumbnailType.Proportion)
                                                                    {
                                                                        objImage.ImageUrl = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(objChildImage.Folder + objChildImage.FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1");
                                                                    }
                                                                    else
                                                                    {
                                                                        objImage.ImageUrl = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(objChildImage.Folder + objChildImage.FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1&s=1");
                                                                    }
                                                                    objImage.EnableViewState = false;
                                                                    objImage.AlternateText = objArticle.Title;
                                                                    objPlaceHolder.Add(objImage);
                                                                }
                                                                count = count + 1;
                                                            }
                                                        }

                                                    }
                                                }
                                            }

                                        }

                                    }
                                }
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("IMAGETHUMBLINK:"))
                            {

                                if (objArticle.ImageUrl != "")
                                {
                                    objImage = new Image();
                                    objImage.ImageUrl = objArticle.ImageUrl;
                                    objImage.EnableViewState = false;
                                    objImage.AlternateText = objArticle.Title;
                                    objPlaceHolder.Add(objImage);
                                }
                                else
                                {
                                    if (objArticle.ImageCount > 0)
                                    {

                                        ImageController objImageController = new ImageController();
                                        List<ImageInfo> objImages = objImageController.GetImageList(objArticle.ArticleID, Null.NullString);

                                        if (objImages.Count > 0)
                                        {

                                            string val = layoutArray[iPtr + 1].Substring(15, layoutArray[iPtr + 1].Length - 15);
                                            if (val.IndexOf(':') == -1)
                                            {
                                                int length = Convert.ToInt32(val);

                                                objLiteral = new Literal();
                                                if (ArticleSettings.ImageThumbnailType == ThumbnailType.Proportion)
                                                {
                                                    objLiteral.Text = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + length.ToString() + "&Height=" + length.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(objImages[0].Folder + objImages[0].FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1");
                                                }
                                                else
                                                {
                                                    objLiteral.Text = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + length.ToString() + "&Height=" + length.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(objImages[0].Folder + objImages[0].FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1&s=1");
                                                }
                                                objLiteral.EnableViewState = false;
                                                objPlaceHolder.Add(objLiteral);
                                            }
                                            else
                                            {
                                                string[] arr = val.Split(':');

                                                if (arr.Length == 2)
                                                {
                                                    int width = Convert.ToInt32(val.Split(':')[0]);
                                                    int height = Convert.ToInt32(val.Split(':')[1]);

                                                    objLiteral = new Literal();
                                                    if (objArticle.ImageUrl.ToLower().StartsWith("http://") || objArticle.ImageUrl.ToLower().StartsWith("https://"))
                                                    {
                                                        objLiteral.Text = objArticle.ImageUrl;
                                                    }
                                                    else
                                                    {
                                                        if (ArticleSettings.ImageThumbnailType == ThumbnailType.Proportion)
                                                        {
                                                            objLiteral.Text = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(objImages[0].Folder + objImages[0].FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1");
                                                        }
                                                        else
                                                        {
                                                            objLiteral.Text = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(objImages[0].Folder + objImages[0].FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1&s=1");
                                                        }
                                                    }
                                                    objLiteral.EnableViewState = false;
                                                    objPlaceHolder.Add(objLiteral);
                                                }
                                                else
                                                {
                                                    if (arr.Length == 3)
                                                    {
                                                        int width = Convert.ToInt32(val.Split(':')[0]);
                                                        int height = Convert.ToInt32(val.Split(':')[1]);
                                                        int item = Convert.ToInt32(val.Split(':')[2]);

                                                        if (objImages.Count > 0)
                                                        {
                                                            int count = 1;
                                                            foreach (ImageInfo objChildImage in objImages)
                                                            {
                                                                if (count == item)
                                                                {
                                                                    objLiteral = new Literal();
                                                                    if (objArticle.ImageUrl.ToLower().StartsWith("http://") || objArticle.ImageUrl.ToLower().StartsWith("https://"))
                                                                    {
                                                                        objLiteral.Text = objArticle.ImageUrl;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (ArticleSettings.ImageThumbnailType == ThumbnailType.Proportion)
                                                                        {
                                                                            objLiteral.Text = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(objChildImage.Folder + objChildImage.FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1");
                                                                        }
                                                                        else
                                                                        {
                                                                            objLiteral.Text = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(objChildImage.Folder + objChildImage.FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1&s=1");
                                                                        }
                                                                    }
                                                                    objLiteral.EnableViewState = false;
                                                                    objPlaceHolder.Add(objLiteral);
                                                                }
                                                                count = count + 1;
                                                            }
                                                        }

                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("IMAGETHUMBRANDOM:"))
                            {

                                if (objArticle.ImageCount > 0)
                                {

                                    ImageController objImageController = new ImageController();
                                    List<ImageInfo> objImages = objImageController.GetImageList(objArticle.ArticleID, Null.NullString);
                                    if (objImages.Count > 0)
                                    {

                                        ImageInfo randomImage = objImages[Generator.Next(0, objImages.Count - 1)];

                                        string val = layoutArray[iPtr + 1].Substring(17, layoutArray[iPtr + 1].Length - 17);
                                        if (val.IndexOf(':') == -1)
                                        {
                                            int length = Convert.ToInt32(val);

                                            objImage = new Image();
                                            if (ArticleSettings.ImageThumbnailType == ThumbnailType.Proportion)
                                            {
                                                objImage.ImageUrl = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + length.ToString() + "&Height=" + length.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(randomImage.Folder + randomImage.FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1");
                                            }
                                            else
                                            {
                                                objImage.ImageUrl = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + length.ToString() + "&Height=" + length.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(randomImage.Folder + randomImage.FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1&s=1");
                                            }
                                            objImage.EnableViewState = false;
                                            objImage.AlternateText = objArticle.Title;
                                            objPlaceHolder.Add(objImage);
                                        }
                                        else
                                        {

                                            string[] arr = val.Split(':');

                                            if (arr.Length == 2)
                                            {
                                                int width = Convert.ToInt32(val.Split(':')[0]);
                                                int height = Convert.ToInt32(val.Split(':')[1]);

                                                objImage = new Image();
                                                if (ArticleSettings.ImageThumbnailType == ThumbnailType.Proportion)
                                                {
                                                    objImage.ImageUrl = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(randomImage.Folder + randomImage.FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1");
                                                }
                                                else
                                                {
                                                    objImage.ImageUrl = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(randomImage.Folder + randomImage.FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1&s=1");
                                                }
                                                objImage.EnableViewState = false;
                                                objImage.AlternateText = objArticle.Title;
                                                objPlaceHolder.Add(objImage);
                                            }
                                        }

                                    }

                                }
                                break;
                            }


                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("ISINROLE:"))
                            {
                                string field = layoutArray[iPtr + 1].Substring(9, layoutArray[iPtr + 1].Length - 9);
                                if (PortalSecurity.IsInRole(field) == false)
                                {
                                    string endToken = "/" + layoutArray[iPtr + 1];
                                    while (iPtr < layoutArray.Length - 1)
                                    {
                                        if (layoutArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("/ISINROLE:"))
                            {
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("ISITEMINDEX:"))
                            {
                                string field = layoutArray[iPtr + 1].Substring(12, layoutArray[iPtr + 1].Length - 12);
                                try
                                {
                                    if (Convert.ToInt32(field) != articleItemIndex)
                                    {
                                        string endToken = "/" + layoutArray[iPtr + 1];
                                        while (iPtr < layoutArray.Length - 1)
                                        {
                                            if (layoutArray[iPtr + 1] == endToken)
                                            {
                                                break;
                                            }
                                            iPtr = iPtr + 1;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("/ISITEMINDEX:"))
                            {
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("ISPAGE:"))
                            {
                                string page = layoutArray[iPtr + 1].Substring(7, layoutArray[iPtr + 1].Length - 7);
                                if (Numeric.IsNumeric(page))
                                {
                                    if (Convert.ToInt32(page) == 1)
                                    {
                                        if (Request["PageID"] != null)
                                        {//""
                                            string endToken = "/" + layoutArray[iPtr + 1];
                                            while (iPtr < layoutArray.Length - 1)
                                            {
                                                if (layoutArray[iPtr + 1] == endToken)
                                                {
                                                    break;
                                                }
                                                iPtr = iPtr + 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        PageController objPageController = new PageController();
                                        ArrayList objPages = objPageController.GetPageList(objArticle.ArticleID);

                                        bool found = false;
                                        if (Request["PageID"] != null)
                                        {//""
                                            int pageNumber = 1;
                                            foreach (PageInfo objPage in objPages)
                                            {
                                                if (Convert.ToInt32(page) == pageNumber)
                                                {
                                                    if (Request["PageID"] == objPage.PageID.ToString())
                                                    {
                                                        found = true;
                                                    }
                                                    break;
                                                }
                                                pageNumber = pageNumber + 1;
                                            }
                                        }

                                        if (!found)
                                        {
                                            string endToken = "/" + layoutArray[iPtr + 1];
                                            while (iPtr < layoutArray.Length - 1)
                                            {
                                                if (layoutArray[iPtr + 1] == endToken)
                                                {
                                                    break;
                                                }
                                                iPtr = iPtr + 1;
                                            }
                                        }
                                    }
                                }
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("/ISPAGE:"))
                            {
                                break;

                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("ISNOTPAGE:"))
                            {
                                string page = layoutArray[iPtr + 1].Substring(10, layoutArray[iPtr + 1].Length - 10);
                                if (Numeric.IsNumeric(page))
                                {
                                    if (Convert.ToInt32(page) == 1)
                                    {
                                        if (Request["PageID"] == null)
                                        {
                                            string endToken = "/" + layoutArray[iPtr + 1];
                                            while (iPtr < layoutArray.Length - 1)
                                            {
                                                if (layoutArray[iPtr + 1] == endToken)
                                                {
                                                    break;
                                                }
                                                iPtr = iPtr + 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        PageController objPageController = new PageController();
                                        ArrayList objPages = objPageController.GetPageList(objArticle.ArticleID);

                                        bool found = false;
                                        if (Request["PageID"] != null)
                                        {
                                            int pageNumber = 1;
                                            foreach (PageInfo objPage in objPages)
                                            {
                                                if (Convert.ToInt32(page) == pageNumber)
                                                {
                                                    if (Request["PageID"] == objPage.PageID.ToString())
                                                    {
                                                        found = true;
                                                    }
                                                    break;
                                                }
                                                pageNumber = pageNumber + 1;
                                            }
                                        }

                                        if (found)
                                        {
                                            string endToken = "/" + layoutArray[iPtr + 1];
                                            while (iPtr < layoutArray.Length - 1)
                                            {
                                                if (layoutArray[iPtr + 1] == endToken)
                                                {
                                                    break;
                                                }
                                                iPtr = iPtr + 1;
                                            }
                                        }
                                    }
                                }
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("/ISNOTPAGE:"))
                            {
                                break;

                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("HASCATEGORY:"))
                            {
                                string category = layoutArray[iPtr + 1].Substring(12, layoutArray[iPtr + 1].Length - 12);

                                objArticleCategories = (ArrayList)DataCache.GetCache(ArticleConstants.CACHE_CATEGORY_ARTICLE + objArticle.ArticleID.ToString());
                                if (objArticleCategories == null)
                                {
                                    ArticleController objArticleController = new ArticleController();
                                    objArticleCategories = objArticleController.GetArticleCategories(objArticle.ArticleID);
                                    DataCache.SetCache(ArticleConstants.CACHE_CATEGORY_ARTICLE + objArticle.ArticleID.ToString(), objArticleCategories);
                                }

                                bool found = false;
                                if (category != "")
                                {
                                    foreach (CategoryInfo objCategory in objArticleCategories)
                                    {
                                        if (category.ToLower() == objCategory.Name.ToLower())
                                        {
                                            found = true;
                                        }
                                    }
                                }

                                if (!found)
                                {
                                    string endToken = "/" + layoutArray[iPtr + 1];
                                    while (iPtr < layoutArray.Length - 1)
                                    {
                                        if (layoutArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("/HASCATEGORY:"))
                            {
                                break;

                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("HASTAG:"))
                            {

                                string tagSelected = layoutArray[iPtr + 1].Substring(7, layoutArray[iPtr + 1].Length - 7);

                                bool found = false;
                                if (objArticle.Tags.Trim() != "")
                                {
                                    foreach (string tag in objArticle.Tags.Split(','))
                                    {
                                        if (tag.ToLower() == tagSelected.ToLower())
                                        {
                                            found = true;
                                        }
                                    }
                                }

                                if (!found)
                                {
                                    string endToken = "/" + layoutArray[iPtr + 1];
                                    while (iPtr < layoutArray.Length - 1)
                                    {
                                        if (layoutArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("/HASTAG:"))
                            {
                                break;

                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("ISLOCALE:"))
                            {
                                string field = layoutArray[iPtr + 1].Substring(9, layoutArray[iPtr + 1].Length - 9);

                                if (((DotNetNuke.Framework.PageBase)Page).PageCulture.Name.ToLower() != field.ToLower())
                                {
                                    string endToken = "/" + layoutArray[iPtr + 1];
                                    while (iPtr < layoutArray.Length - 1)
                                    {
                                        if (layoutArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("/ISLOCALE:"))
                            {
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("PAGE:"))
                            {
                                if (Numeric.IsNumeric(layoutArray[iPtr + 1].Substring(5, layoutArray[iPtr + 1].Length - 5)))
                                {
                                    int pageNumber = Convert.ToInt32(layoutArray[iPtr + 1].Substring(5, layoutArray[iPtr + 1].Length - 5));

                                    objLiteral = new Literal();
                                    if (pageNumber > 0)
                                    {
                                        PageController pageController = new PageController();
                                        ArrayList pageList = pageController.GetPageList(objArticle.ArticleID);

                                        if (pageList.Count >= pageNumber)
                                        {
                                            objLiteral.Text = ProcessPostTokens(Server.HtmlDecode(((PageInfo)pageList[pageNumber - 1]).PageText), objArticle, ref Generator, ArticleSettings);
                                        }
                                    }
                                    objLiteral.EnableViewState = false;
                                    objPlaceHolder.Add(objLiteral);
                                    break;
                                }
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("PUBLISHENDDATE:"))
                            {
                                string formatExpression = layoutArray[iPtr + 1].Substring(15, layoutArray[iPtr + 1].Length - 15);

                                objLiteral = new Literal();

                                try
                                {
                                    if (objArticle.EndDate == Null.NullDate)
                                    {
                                        objLiteral.Text = "";
                                    }
                                    else
                                    {
                                        objLiteral.Text = objArticle.EndDate.ToString(formatExpression);
                                    }
                                }
                                catch
                                {
                                    if (objArticle.EndDate == Null.NullDate)
                                    {
                                        objLiteral.Text = "";
                                    }
                                    else
                                    {
                                        objLiteral.Text = objArticle.EndDate.ToString("D");
                                    }
                                }

                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("PUBLISHDATE:"))
                            {
                                string formatExpression = layoutArray[iPtr + 1].Substring(12, layoutArray[iPtr + 1].Length - 12);

                                objLiteral = new Literal();

                                try
                                {
                                    if (objArticle.StartDate == Null.NullDate)
                                    {
                                        objLiteral.Text = objArticle.CreatedDate.ToString(formatExpression);
                                    }
                                    else
                                    {
                                        objLiteral.Text = objArticle.StartDate.ToString(formatExpression);
                                    }
                                }
                                catch
                                {
                                    if (objArticle.StartDate == Null.NullDate)
                                    {
                                        objLiteral.Text = objArticle.CreatedDate.ToString("D");
                                    }
                                    else
                                    {
                                        objLiteral.Text = objArticle.StartDate.ToString("D");
                                    }
                                }

                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("PUBLISHSTARTDATE:"))
                            {
                                string formatExpression = layoutArray[iPtr + 1].Substring(17, layoutArray[iPtr + 1].Length - 17);

                                objLiteral = new Literal();

                                try
                                {
                                    if (objArticle.StartDate == Null.NullDate)
                                    {
                                        objLiteral.Text = objArticle.CreatedDate.ToString(formatExpression);
                                    }
                                    else
                                    {
                                        objLiteral.Text = objArticle.StartDate.ToString(formatExpression);
                                    }
                                }
                                catch
                                {
                                    if (objArticle.StartDate == Null.NullDate)
                                    {
                                        objLiteral.Text = objArticle.CreatedDate.ToString("D");
                                    }
                                    else
                                    {
                                        objLiteral.Text = objArticle.StartDate.ToString("D");
                                    }
                                }

                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("QUERYSTRING:"))
                            {
                                string variable = layoutArray[iPtr + 1].Substring(12, layoutArray[iPtr + 1].Length - 12);

                                objLiteral = new Literal();
                                objLiteral.Text = Request.QueryString[variable];
                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("RELATED:"))
                            {
                                _objRelatedArticles = null;
                                if (Numeric.IsNumeric(layoutArray[iPtr + 1].Substring(8, layoutArray[iPtr + 1].Length - 8)))
                                {
                                    int count = Convert.ToInt32(layoutArray[iPtr + 1].Substring(8, layoutArray[iPtr + 1].Length - 8));
                                    if (count > 0)
                                    {
                                        if (ArticleSettings.RelatedMode != RelatedType.None)
                                        {
                                            PlaceHolder phRelated = new PlaceHolder();
                                            List<ArticleInfo> objArticles = GetRelatedArticles(objArticle, count);
                                            if (objArticles.Count > 0)
                                            {
                                                LayoutInfo _objLayoutRelatedHeader = GetLayout(ArticleSettings, ArticleModule, Page, LayoutType.Related_Header_Html);
                                                LayoutInfo _objLayoutRelatedItem = GetLayout(ArticleSettings, ArticleModule, Page, LayoutType.Related_Item_Html);
                                                LayoutInfo _objLayoutRelatedFooter = GetLayout(ArticleSettings, ArticleModule, Page, LayoutType.Related_Footer_Html);

                                                ProcessArticleItem(phRelated.Controls, _objLayoutRelatedHeader.Tokens, objArticle);
                                                foreach (ArticleInfo objRelatedArticle in objArticles)
                                                {
                                                    ProcessArticleItem(phRelated.Controls, _objLayoutRelatedItem.Tokens, objRelatedArticle);
                                                }
                                                ProcessArticleItem(phRelated.Controls, _objLayoutRelatedFooter.Tokens, objArticle);

                                                objPlaceHolder.Add(phRelated);
                                            }
                                        }
                                    }
                                }
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("SUMMARY:"))
                            {
                                string summary = objArticle.Summary;
                                if (Numeric.IsNumeric(layoutArray[iPtr + 1].Substring(8, layoutArray[iPtr + 1].Length - 8)))
                                {
                                    int length = Convert.ToInt32(layoutArray[iPtr + 1].Substring(8, layoutArray[iPtr + 1].Length - 8));
                                    if (StripHtml(Server.HtmlDecode(objArticle.Summary)).TrimStart().Length > length)
                                    {
                                        summary = ProcessPostTokens(StripHtml(Server.HtmlDecode(objArticle.Summary)).TrimStart().Substring(0, length), objArticle, ref Generator, ArticleSettings) + "...";
                                    }
                                    else
                                    {
                                        summary = ProcessPostTokens(StripHtml(Server.HtmlDecode(objArticle.Summary)).TrimStart().Substring(0, StripHtml(Server.HtmlDecode(objArticle.Summary)).TrimStart().Length), objArticle, ref Generator, ArticleSettings);
                                    }
                                }

                                objLiteral = new Literal();
                                objLiteral.Text = summary;
                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("TITLE:"))
                            {
                                string title = objArticle.Title;
                                if (Numeric.IsNumeric(layoutArray[iPtr + 1].Substring(6, layoutArray[iPtr + 1].Length - 6)))
                                {
                                    int length = Convert.ToInt32(layoutArray[iPtr + 1].Substring(6, layoutArray[iPtr + 1].Length - 6));
                                    if (objArticle.Title.Length > length)
                                    {
                                        title = objArticle.Title.Substring(0, length) + "...";
                                    }
                                    else
                                    {
                                        title = objArticle.Title.Substring(0, objArticle.Title.Length);
                                    }
                                }

                                objLiteral = new Literal();
                                objLiteral.Text = title;
                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("UPDATEDATE:"))
                            {
                                string formatExpression = layoutArray[iPtr + 1].Substring(11, layoutArray[iPtr + 1].Length - 11);

                                objLiteral = new Literal();

                                try
                                {
                                    if (objArticle.CreatedDate == Null.NullDate)
                                    {
                                        objLiteral.Text = "";
                                    }
                                    else
                                    {
                                        objLiteral.Text = objArticle.LastUpdate.ToString(formatExpression);
                                    }
                                }
                                catch
                                {
                                    if (objArticle.CreatedDate == Null.NullDate)
                                    {
                                        objLiteral.Text = "";
                                    }
                                    else
                                    {
                                        objLiteral.Text = objArticle.LastUpdate.ToString("D");
                                    }
                                }

                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("UPDATEDATELESSTHAN:"))
                            {
                                int length = Convert.ToInt32(layoutArray[iPtr + 1].Substring(19, layoutArray[iPtr + 1].Length - 19));

                                if (objArticle.LastUpdate < DateTime.Now.AddDays(length * -1))
                                {
                                    while (iPtr < layoutArray.Length - 1)
                                    {
                                        if (layoutArray[iPtr + 1] == "/UPDATEDATELESSTHAN")
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                                break;
                            }

                            if (layoutArray[iPtr + 1].ToUpper().StartsWith("/UPDATEDATELESSTHAN:"))
                            {
                                break;
                            }

                            Literal objLiteralOther = new Literal();
                            objLiteralOther.Text = "[" + layoutArray[iPtr + 1] + "]";
                            objLiteralOther.EnableViewState = false;
                            objPlaceHolder.Add(objLiteralOther);
                            break;
                    }
                }
            }

        }


        #endregion

        #region " Process Comment Item "

        int commentItemIndex = 0;
        public void ProcessComment(ControlCollection objPlaceHolder, ArticleInfo objArticle, CommentInfo objComment, string[] templateArray)
        {

            commentItemIndex = commentItemIndex + 1;
            bool isAnonymous = Null.IsNull(objComment.UserID);

            Literal objLiteral;

            for (int iPtr = 0; iPtr < templateArray.Length; iPtr = iPtr + 2)
            {

                objPlaceHolder.Add(new LiteralControl(ProcessImages(templateArray[iPtr].ToString())));

                if (iPtr < templateArray.Length - 1)
                {
                    switch (templateArray[iPtr + 1])
                    {
                        case "ANONYMOUSURL":
                            objLiteral = new Literal();
                            objLiteral.Text = Globals.AddHTTP(objComment.AnonymousURL);
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "ARTICLETITLE":
                            objLiteral = new Literal();
                            objLiteral.Text = objArticle.Title;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "AUTHOREMAIL":
                            objLiteral = new Literal();
                            if (objComment.UserID == Null.NullInteger)
                            {
                                objLiteral.Text = objComment.AnonymousEmail.ToString();
                            }
                            else
                            {
                                objLiteral.Text = objComment.AuthorEmail.ToString();
                            }
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "AUTHOR":
                            objLiteral = new Literal();
                            switch (ArticleSettings.DisplayMode)
                            {
                                case DisplayType.FirstName:
                                    if (isAnonymous)
                                    {
                                        if (objComment.AnonymousName != "")
                                        {
                                            objLiteral.Text = objComment.AnonymousName;
                                        }
                                        else
                                        {
                                            objLiteral.Text = GetArticleResource("AnonymousFirstName");
                                        }
                                    }
                                    else
                                    {
                                        objLiteral.Text = objComment.AuthorFirstName;
                                    }
                                    break;
                                case DisplayType.LastName:
                                    if (isAnonymous)
                                    {
                                        if (objComment.AnonymousName != "")
                                        {
                                            objLiteral.Text = objComment.AnonymousName;
                                        }
                                        else
                                        {
                                            objLiteral.Text = GetArticleResource("AnonymousLastName");
                                        }
                                    }
                                    else
                                    {
                                        objLiteral.Text = objComment.AuthorLastName;
                                    }
                                    break;
                                case DisplayType.UserName:
                                    if (isAnonymous)
                                    {
                                        if (objComment.AnonymousName != "")
                                        {
                                            objLiteral.Text = objComment.AnonymousName;
                                        }
                                        else
                                        {
                                            objLiteral.Text = GetArticleResource("AnonymousUserName");
                                        }
                                    }
                                    else
                                    {
                                        objLiteral.Text = objComment.AuthorUserName;
                                    }
                                    break;
                                case DisplayType.FullName:
                                    if (isAnonymous)
                                    {
                                        if (objComment.AnonymousName != "")
                                        {
                                            objLiteral.Text = objComment.AnonymousName;
                                        }
                                        else
                                        {
                                            objLiteral.Text = GetArticleResource("AnonymousFullName");
                                        }
                                    }
                                    else
                                    {
                                        objLiteral.Text = objComment.AuthorDisplayName;
                                    }
                                    break;
                                default:
                                    if (isAnonymous)
                                    {
                                        if (objComment.AnonymousName != "")
                                        {
                                            objLiteral.Text = objComment.AnonymousName;
                                        }
                                        else
                                        {
                                            objLiteral.Text = GetArticleResource("AnonymousUserName");
                                        }
                                    }
                                    else
                                    {
                                        objLiteral.Text = objComment.AuthorUserName;
                                    }
                                    break;
                            }
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "AUTHORID":
                            objLiteral = new Literal();
                            objLiteral.Text = objComment.UserID.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "AUTHORUSERNAME":
                            objLiteral = new Literal();
                            if (objComment.UserID == Null.NullInteger)
                            {
                                if (objComment.AnonymousName != "")
                                {
                                    objLiteral.Text = objComment.AnonymousName;
                                }
                                else
                                {
                                    objLiteral.Text = GetArticleResource("AnonymousUserName");
                                }
                            }
                            else
                            {
                                objLiteral.Text = objComment.AuthorUserName.ToString();
                            }
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "AUTHORDISPLAYNAME":
                            objLiteral = new Literal();
                            if (objComment.UserID == Null.NullInteger)
                            {
                                if (objComment.AnonymousName != "")
                                {
                                    objLiteral.Text = objComment.AnonymousName;
                                }
                                else
                                {
                                    objLiteral.Text = GetArticleResource("AnonymousFullName");
                                }
                            }
                            else
                            {
                                objLiteral.Text = objComment.AuthorDisplayName.ToString();
                            }
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "AUTHORFIRSTNAME":
                            objLiteral = new Literal();
                            if (objComment.UserID == Null.NullInteger)
                            {
                                if (objComment.AnonymousName != "")
                                {
                                    objLiteral.Text = objComment.AnonymousName;
                                }
                                else
                                {
                                    objLiteral.Text = GetArticleResource("AnonymousFirstName");
                                }
                            }
                            else
                            {
                                objLiteral.Text = objComment.AuthorFirstName.ToString();
                            }
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "AUTHORLASTNAME":
                            objLiteral = new Literal();
                            if (objComment.UserID == Null.NullInteger)
                            {
                                if (objComment.AnonymousName != "")
                                {
                                    objLiteral.Text = objComment.AnonymousName;
                                }
                                else
                                {
                                    objLiteral.Text = GetArticleResource("AnonymousLastName");
                                }
                            }
                            else
                            {
                                objLiteral.Text = objComment.AuthorLastName.ToString();
                            }
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "AUTHORFULLNAME":
                            objLiteral = new Literal();
                            if (objComment.UserID == Null.NullInteger)
                            {
                                if (objComment.AnonymousName != "")
                                {
                                    objLiteral.Text = objComment.AnonymousName;
                                }
                                else
                                {
                                    objLiteral.Text = GetArticleResource("AnonymousFullName");
                                }
                            }
                            else
                            {
                                objLiteral.Text = objComment.AuthorFirstName.ToString() + " " + objComment.AuthorLastName.ToString();
                            }
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "COMMENTID":
                            objLiteral = new Literal();
                            objLiteral.Text = objComment.CommentID.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "COMMENT":
                            objLiteral = new Literal();
                            Random gener = new Random();
                            objLiteral.Text = ProcessPostTokens(EncodeComment(objComment), objArticle, ref gener, ArticleSettings);
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "COMMENTLINK":
                            objLiteral = new Literal();
                            objLiteral.Text = Common.GetArticleLink(objArticle, Tab, ArticleSettings, IncludeCategory) + "#Comments";
                            objLiteral.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "CREATEDATE":
                            objLiteral = new Literal();
                            objLiteral.Text = objComment.CreatedDate.ToString("D");
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "CREATETIME":
                            objLiteral = new Literal();
                            objLiteral.Text = objComment.CreatedDate.ToString("t");
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "DELETE":
                            if (ArticleSettings.IsAdmin || ArticleSettings.IsApprover || (UserId == objArticle.AuthorID && objArticle.AuthorID != Null.NullInteger))
                            {
                                LinkButton cmdDelete = new LinkButton();
                                cmdDelete.CssClass = "CommandButton";
                                cmdDelete.Text = GetArticleResource("Delete");
                                cmdDelete.Attributes.Add("onClick", "javascript:return confirm('确定要删除此项？');");
                                cmdDelete.CommandArgument = objComment.CommentID.ToString();
                                cmdDelete.CommandName = "DeleteComment";
                                cmdDelete.Command += new System.Web.UI.WebControls.CommandEventHandler(Comment_Command);
                                objPlaceHolder.Add(cmdDelete);
                            }
                            break;
                        case "EDIT":
                            if (ArticleSettings.IsAdmin || ArticleSettings.IsApprover || (UserId == objArticle.AuthorID && objArticle.AuthorID != Null.NullInteger))
                            {
                                HyperLink objHyperLink = new HyperLink();
                                objHyperLink.CssClass = "CommandButton";
                                objHyperLink.Text = GetArticleResource("Edit");
                                objHyperLink.NavigateUrl = Common.GetModuleLink(ArticleModule.TabID, ArticleModule.ModuleID, "EditComment", ArticleSettings, "CommentID=" + objComment.CommentID.ToString(), "ReturnUrl=" + Server.UrlEncode(Request.RawUrl));
                                objHyperLink.EnableViewState = false;
                                objPlaceHolder.Add(objHyperLink);
                            }
                            break;
                        case "GRAVATARURL":
                            objLiteral = new Literal();
                            if (Request.IsSecureConnection)
                            {
                                if (objComment.UserID == Null.NullInteger)
                                {
                                    objLiteral.Text = Globals.AddHTTP("secure.gravatar.com/avatar/" + MD5CalcString(objComment.AnonymousEmail.ToLower()));
                                }
                                else
                                {
                                    objLiteral.Text = Globals.AddHTTP("secure.gravatar.com/avatar/" + MD5CalcString(objComment.AuthorEmail.ToLower()));
                                }
                            }
                            else
                            {
                                if (objComment.UserID == Null.NullInteger)
                                {
                                    objLiteral.Text = Globals.AddHTTP("www.gravatar.com/avatar/" + MD5CalcString(objComment.AnonymousEmail.ToLower()));
                                }
                                else
                                {
                                    objLiteral.Text = Globals.AddHTTP("www.gravatar.com/avatar/" + MD5CalcString(objComment.AuthorEmail.ToLower()));
                                }
                            }
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "HASANONYMOUSURL":
                            if (objComment.AnonymousURL == "")
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/HASANONYMOUSURL")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASANONYMOUSURL":
                            break;
                        case "IPADDRESS":
                            objLiteral = new Literal();
                            objLiteral.Text = objComment.RemoteAddress;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "ISANONYMOUS":
                            if (objComment.UserID != -1)
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/ISANONYMOUS")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISANONYMOUS":
                            break;
                        case "ISNOTANONYMOUS":
                            if (objComment.UserID == -1)
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/ISNOTANONYMOUS")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISNOTANONYMOUS":
                            break;
                        case "ISCOMMENT":
                            if (objComment.Type != 0)
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/ISCOMMENT")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISCOMMENT":
                            break;
                        case "ISPINGBACK":
                            if (objComment.Type != 2)
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/ISPINGBACK")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "ISTRACKBACK":
                            if (objComment.Type != 1)
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/ISTRACKBACK")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISTRACKBACK":
                            break;
                        case "ISAUTHOR":
                            if (objComment.UserID == Null.NullInteger || (objComment.UserID != objArticle.AuthorID))
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/ISAUTHOR")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/ISAUTHOR":
                            break;
                        case "ITEMINDEX":
                            objLiteral = new Literal();
                            objLiteral.Text = this.commentItemIndex.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "MODULEID":
                            objLiteral = new Literal();
                            objLiteral.Text = ArticleModule.ModuleID.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "PINGBACKURL":
                            objLiteral = new Literal();
                            objLiteral.Text = objComment.TrackbackUrl;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TRACKBACKBLOGNAME":
                            objLiteral = new Literal();
                            objLiteral.Text = objComment.TrackbackBlogName;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TRACKBACKEXCERPT":
                            objLiteral = new Literal();
                            objLiteral.Text = objComment.TrackbackExcerpt;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TRACKBACKTITLE":
                            objLiteral = new Literal();
                            objLiteral.Text = objComment.TrackbackTitle;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TRACKBACKURL":
                            objLiteral = new Literal();
                            objLiteral.Text = objComment.TrackbackUrl;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        default:
                            if (templateArray[iPtr + 1].ToUpper().StartsWith("AUTHOR:"))
                            {
                                if (Author(objComment.UserID) != null)
                                {
                                    // token to be processed
                                    string field = templateArray[iPtr + 1].Substring(7, templateArray[iPtr + 1].Length - 7).ToLower().Trim();

                                    //Gets the DNN profile property named like the token (field)用户信息
                                    bool profilePropertyFound = false;
                                    string profilePropertyDataType = string.Empty;
                                    string profilePropertyName = string.Empty;
                                    string profilePropertyValue = string.Empty;

                                    foreach (ProfilePropertyDefinition objProfilePropertyDefinition in ProfileProperties)
                                    {
                                        if (objProfilePropertyDefinition.PropertyName.ToLower().Trim() == field)
                                        {

                                            //Gets the dnn profile property's datatype
                                            ListController objListController = new ListController();
                                            ListEntryInfo definitionEntry = objListController.GetListEntryInfo(objProfilePropertyDefinition.DataType);
                                            if (definitionEntry != null)
                                            {
                                                profilePropertyDataType = definitionEntry.Value;
                                            }
                                            else
                                            {
                                                profilePropertyDataType = "Unknown";
                                            }

                                            //Gets the dnn profile property's name and current value for the given user (Agent = AuthorID)
                                            profilePropertyName = objProfilePropertyDefinition.PropertyName;
                                            profilePropertyValue = Author(objComment.UserID).Profile.GetPropertyValue(profilePropertyName);

                                            profilePropertyFound = true;

                                        }
                                    }

                                    if (profilePropertyFound)
                                    {

                                        switch (profilePropertyDataType.ToLower())
                                        {
                                            case "truefalse":
                                                CheckBox objTrueFalse = new CheckBox();
                                                if (profilePropertyValue == string.Empty)
                                                {
                                                    objTrueFalse.Checked = false;
                                                }
                                                else
                                                {
                                                    objTrueFalse.Checked = Convert.ToBoolean(profilePropertyValue);
                                                }
                                                objTrueFalse.Enabled = false;
                                                objTrueFalse.EnableViewState = false;
                                                objPlaceHolder.Add(objTrueFalse);
                                                break;
                                            case "richtext":
                                                objLiteral = new Literal();
                                                if (profilePropertyValue == string.Empty)
                                                {
                                                    objLiteral.Text = string.Empty;
                                                }
                                                else
                                                {
                                                    objLiteral.Text = Page.Server.HtmlDecode(profilePropertyValue);
                                                }
                                                objLiteral.EnableViewState = false;
                                                objPlaceHolder.Add(objLiteral);
                                                break;
                                            case "list":
                                                objLiteral = new Literal();
                                                objLiteral.Text = profilePropertyValue;
                                                ListController objListController = new ListController();
                                                List<ListEntryInfo> objListEntryInfoCollection = objListController.GetListEntryInfoItems(profilePropertyName).ToList();
                                                foreach (ListEntryInfo objListEntryInfo in objListEntryInfoCollection)
                                                {
                                                    if (objListEntryInfo.Value == profilePropertyValue)
                                                    {
                                                        objLiteral.Text = objListEntryInfo.Text;
                                                        break;
                                                    }
                                                }
                                                objLiteral.EnableViewState = false;
                                                objPlaceHolder.Add(objLiteral);
                                                break;
                                            default:
                                                objLiteral = new Literal();
                                                if (profilePropertyValue == string.Empty)
                                                {
                                                    objLiteral.Text = string.Empty;
                                                }
                                                else
                                                {
                                                    if (profilePropertyName.ToLower() == "website")
                                                    {
                                                        string url = profilePropertyValue;
                                                        if (url.ToLower().StartsWith("http://"))
                                                        {
                                                            url = url.Substring(7); // removes the "http://"
                                                        }
                                                        objLiteral.Text = url;
                                                    }
                                                    else
                                                    {
                                                        objLiteral.Text = profilePropertyValue;
                                                    }
                                                }
                                                objLiteral.EnableViewState = false;
                                                objPlaceHolder.Add(objLiteral);
                                                break;
                                        }//profilePropertyDataType

                                    } //DNN Profile property processing
                                }
                                break;
                            } // "AUTHOR:" token

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("CREATEDATE:"))
                            {
                                string formatExpression = templateArray[iPtr + 1].Substring(11, templateArray[iPtr + 1].Length - 11);
                                objLiteral = new Literal();
                                objLiteral.Text = objComment.CreatedDate.ToString(formatExpression);
                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("COMMENT:"))
                            {
                                string count = templateArray[iPtr + 1].Substring(8, templateArray[iPtr + 1].Length - 8);
                                if (Numeric.IsNumeric(count))
                                {
                                    string comment = objComment.Comment;
                                    if (StripHtml(objComment.Comment).TrimStart().Length > Convert.ToInt32(count))
                                    {
                                        comment = StripHtml(Server.HtmlDecode(objComment.Comment)).TrimStart().Substring(0, Convert.ToInt32(count)) + "...";
                                    }
                                    objLiteral = new Literal();
                                    objLiteral.Text = comment;
                                    objLiteral.EnableViewState = false;
                                    objPlaceHolder.Add(objLiteral);
                                }
                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("ISINROLE:"))
                            {
                                string field = templateArray[iPtr + 1].Substring(9, templateArray[iPtr + 1].Length - 9);
                                if (PortalSecurity.IsInRole(field) == false)
                                {
                                    string endToken = "/" + templateArray[iPtr + 1];
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

        public void ProcessFile(ControlCollection objPlaceHolder, ArticleInfo objArticle, FileInfo objFile, string[] templateArray)
        {

            for (int iPtr = 0; iPtr < templateArray.Length; iPtr = iPtr + 2)
            {

                objPlaceHolder.Add(new LiteralControl(ProcessImages(templateArray[iPtr].ToString())));

                Literal objLiteral;

                if (iPtr < templateArray.Length - 1)
                {
                    switch (templateArray[iPtr + 1])
                    {

                        case "ARTICLEID":
                            objLiteral = new Literal();
                            objLiteral.Text = objFile.ArticleID.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "FILEID":
                            objLiteral = new Literal();
                            objLiteral.Text = objFile.FileID.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "FILENAME":
                            objLiteral = new Literal();
                            objLiteral.Text = objFile.FileName.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "FILELINK":
                            objLiteral = new Literal();
                            objLiteral.Text = objFile.Link;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "SIZE":
                            objLiteral = new Literal();
                            objLiteral.Text = Numeric2Bytes(objFile.Size);
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "SORTORDER":
                            objLiteral = new Literal();
                            objLiteral.Text = objFile.SortOrder.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TITLE":
                            objLiteral = new Literal();
                            objLiteral.Text = objFile.Title;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        default:

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("ISEXTENSION:"))
                            {
                                string field = templateArray[iPtr + 1].Substring(12, templateArray[iPtr + 1].Length - 12);

                                if (objFile.FileName.ToUpper().EndsWith(field.ToUpper()) == false)
                                {
                                    string endToken = "/" + templateArray[iPtr + 1];
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

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("ISNOTEXTENSION:"))
                            {
                                string field = templateArray[iPtr + 1].Substring(15, templateArray[iPtr + 1].Length - 15);

                                if (objFile.FileName.ToUpper().EndsWith(field.ToUpper()))
                                {
                                    string endToken = "/" + templateArray[iPtr + 1];
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

        private int imageIndex = 0;
        public void ProcessImage(ControlCollection objPlaceHolder, ArticleInfo objArticle, ImageInfo objImage, string[] templateArray)
        {

            imageIndex = imageIndex + 1;

            Literal objLiteral;

            for (int iPtr = 0; iPtr < templateArray.Length; iPtr = iPtr + 2)
            {

                objPlaceHolder.Add(new LiteralControl(ProcessImages(templateArray[iPtr].ToString()).Replace("{|", "[").Replace("|}", "]")));

                if (iPtr < templateArray.Length - 1)
                {
                    switch (templateArray[iPtr + 1])
                    {

                        case "ARTICLEID":
                            objLiteral = new Literal();
                            objLiteral.Text = objImage.ArticleID.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;

                        case "DESCRIPTION":
                            objLiteral = new Literal();
                            objLiteral.Text = objImage.Description;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "FILENAME":
                            objLiteral = new Literal();
                            objLiteral.Text = objImage.FileName.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "HEIGHT":
                            objLiteral = new Literal();
                            objLiteral.Text = objImage.Height.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "IMAGEID":
                            objLiteral = new Literal();
                            objLiteral.Text = objImage.ImageID.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "IMAGELINK":
                            objLiteral = new Literal();
                            objLiteral.Text = PortalSettings.HomeDirectory + objImage.Folder + objImage.FileName;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "ITEMINDEX":
                            objLiteral = new Literal();
                            objLiteral.Text = imageIndex.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "SIZE":
                            objLiteral = new Literal();
                            objLiteral.Text = objImage.Size.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "SORTORDER":
                            objLiteral = new Literal();
                            objLiteral.Text = objImage.SortOrder.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TITLE":
                            objLiteral = new Literal();
                            objLiteral.Text = objImage.Title;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "WIDTH":
                            objLiteral = new Literal();
                            objLiteral.Text = objImage.Width.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        default:

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("IMAGETHUMB:"))
                            {
                                string val = templateArray[iPtr + 1].Substring(11, templateArray[iPtr + 1].Length - 11);
                                if (val.IndexOf(':') != -1)
                                {
                                    int width = Convert.ToInt32(val.Split(':')[0]);
                                    int height = Convert.ToInt32(val.Split(':')[1]);

                                    Image objImageItem = new Image();
                                    if (ArticleSettings.ImageThumbnailType == ThumbnailType.Proportion)
                                    {
                                        objImageItem.ImageUrl = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + height.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(objImage.Folder + objImage.FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1");
                                    }
                                    else
                                    {
                                        objImageItem.ImageUrl = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign-NewsArticles/ImageHandler.ashx?Width=" + width.ToString() + "&Height=" + width.ToString() + "&HomeDirectory=" + Server.UrlEncode(PortalSettings.HomeDirectory) + "&FileName=" + Server.UrlEncode(objImage.Folder + objImage.FileName) + "&PortalID=" + PortalSettings.PortalId.ToString() + "&q=1&s=1");
                                    }
                                    objImageItem.EnableViewState = false;
                                    objImageItem.AlternateText = objArticle.Title;
                                    objPlaceHolder.Add(objImageItem);
                                }
                                break;
                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("ISITEMINDEX:"))
                            {
                                string field = templateArray[iPtr + 1].Substring(12, templateArray[iPtr + 1].Length - 12);
                                if (field != imageIndex.ToString())
                                {
                                    string endToken = "/" + templateArray[iPtr + 1];
                                    while (iPtr < templateArray.Length - 1)
                                    {
                                        if (templateArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                                break;
                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("/ISITEMINDEX:"))
                            {
                                break;
                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("ISNOTITEMINDEX:"))
                            {
                                string field = templateArray[iPtr + 1].Substring(15, templateArray[iPtr + 1].Length - 15);
                                if (field == imageIndex.ToString())
                                {
                                    string endToken = "/" + templateArray[iPtr + 1];
                                    while (iPtr < templateArray.Length - 1)
                                    {
                                        if (templateArray[iPtr + 1] == endToken)
                                        {
                                            break;
                                        }
                                        iPtr = iPtr + 1;
                                    }
                                }
                                break;
                            }


                            if (templateArray[iPtr + 1].ToUpper().StartsWith("/ISNOTITEMINDEX:"))
                            {
                                break;
                            }

                            Literal objLiteralOther = new Literal();
                            objLiteralOther.Text = "[" + templateArray[iPtr + 1] + "]";
                            objLiteralOther.EnableViewState = false;
                            objPlaceHolder.Add(objLiteralOther);
                            break;
                    }
                }

            }

        }

        #endregion

        #endregion

        #region " Event Handlers "

        private void Comment_Command(object sender, CommandEventArgs e)
        {

            switch (e.CommandName.ToLower())
            {

                case "deletecomment":
                    ArticleController objArticleController = new ArticleController();
                    ArticleInfo objArticle = objArticleController.GetArticle(Convert.ToInt32(Request["ArticleID"]));

                    if (objArticle != null)
                    {
                        CommentController objCommentController = new CommentController();
                        objCommentController.DeleteComment(Convert.ToInt32(e.CommandArgument), objArticle.ArticleID);
                    }

                    HttpContext.Current.Response.Redirect(Request.RawUrl, true);
                    break;

            }

        }

        #endregion
    }
}