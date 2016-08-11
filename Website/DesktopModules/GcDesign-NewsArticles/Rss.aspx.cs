using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;
using System.Xml;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections;
using System.Text;

namespace GcDesign.NewsArticles
{
    public partial class Rss : System.Web.UI.Page
    {
        #region " private Members "

        private string m_articleIDs = Null.NullString;
        private int m_count = Null.NullInteger;
        private int[] m_categoryID = null;
        private int[] m_categoryIDExclude = null;
        private int m_tabID = Null.NullInteger;
        private DotNetNuke.Entities.Tabs.TabInfo m_TabInfo;
        private int m_moduleID = Null.NullInteger;
        private int m_authorID = Null.NullInteger;
        private bool m_featuredOnly = false;
        private bool m_matchAll = false;
        private bool m_notFeaturedOnly = false;
        private bool m_securedOnly = false;
        private bool m_notSecuredOnly = false;
        private bool m_showExpired = false;
        private string m_sortBy = ArticleConstants.DEFAULT_SORT_BY;
        private string m_sortDirection = ArticleConstants.DEFAULT_SORT_DIRECTION;
        private int[] m_tagID = null;
        private bool m_tagMatch = false;

        private int m_month = Null.NullInteger;
        private int m_year = Null.NullInteger;

        private string _template = "Standard";

        private bool _enableSyndicationEnclosures = true;
        private bool _enableSyndicationHtml = false;
        private SyndicationEnclosureType _enclosureType = SyndicationEnclosureType.Attachment;
        private int _syndicationSummaryLength = Null.NullInteger;

        #endregion

        #region " private Methods "

        private void ReadQueryString()
        {

            if (Request["TabID"] != null)
            {

                if (Numeric.IsNumeric(Request["TabID"]))
                {

                    m_tabID = Convert.ToInt32(Request["TabID"]);

                    TabController objTabController = new TabController();
                    m_TabInfo = objTabController.GetTab(m_tabID, Globals.GetPortalSettings().PortalId, false);

                }

            }

            if (Request["ModuleID"] != null)
            {

                if (Numeric.IsNumeric(Request["ModuleID"]))
                {

                    m_moduleID = Convert.ToInt32(Request["ModuleID"]);

                }

            }

            if (Request["CategoryID"] != null)
            {

                string[] categories = Request["CategoryID"].ToString().Split(',');

                if (categories.Length > 0)
                {
                    int[] m_categoryID = new int[categories.Length];
                    for (int i = 0; i < categories.Length; i++)
                    {
                        m_categoryID[i] = Convert.ToInt32(categories[i]);
                    }
                }

            }

            if (Request["CategoryIDExclude"] != null)
            {

                string[] categories = Request["CategoryIDExclude"].ToString().Split(',');

                if (categories.Length > 0)
                {
                    int[] m_categoryIDExclude = new int[categories.Length];
                    for (int i = 0; i < categories.Length; i++)
                    {
                        m_categoryIDExclude[i] = Convert.ToInt32(categories[i]);
                    }
                }

            }

            if (Request["MaxCount"] != null)
            {

                if (Numeric.IsNumeric(Request["MaxCount"]))
                {

                    m_count = Convert.ToInt32(Request["MaxCount"]);

                }

            }

            if (Request["AuthorID"] != null)
            {

                if (Numeric.IsNumeric(Request["AuthorID"]))
                {

                    m_authorID = Convert.ToInt32(Request["AuthorID"]);

                }

            }

            if (Request["FeaturedOnly"] != null)
            {

                m_featuredOnly = Convert.ToBoolean(Request["FeaturedOnly"]);

            }

            if (Request["NotFeaturedOnly"] != null)
            {

                m_notFeaturedOnly = Convert.ToBoolean(Request["NotFeaturedOnly"]);

            }

            if (Request["ShowExpired"] != null)
            {
                m_showExpired = Convert.ToBoolean(Request["ShowExpired"]);
            }

            if (Request["SecuredOnly"] != null)
            {
                m_securedOnly = Convert.ToBoolean(Request["SecuredOnly"]);
            }

            if (Request["NotSecuredOnly"] != null)
            {
                m_notSecuredOnly = Convert.ToBoolean(Request["NotSecuredOnly"]);
            }

            if (Request["ArticleIDs"] != null)
            {
                m_articleIDs = Request["ArticleIDs"];
            }

            if (Request["SortBy"] != null)
            {

                m_sortBy = Request["SortBy"].ToString();

            }

            if (Request["SortDirection"] != null)
            {

                m_sortDirection = Request["SortDirection"].ToString();

            }

            if (Request["Month"] != null)
            {
                if (Numeric.IsNumeric(Request["Month"]))
                {
                    m_month = Convert.ToInt32(Request["Month"]);
                }
            }

            if (Request["Year"] != null)
            {
                if (Numeric.IsNumeric(Request["Year"]))
                {
                    m_year = Convert.ToInt32(Request["Year"]);
                }
            }

            if (Request["MatchTag"] != null)
            {
                if (Request["MatchTag"] == "1")
                {
                    m_tagMatch = true;
                }
            }

            if (Request["TagIDs"] != null)
            {
                string[] tagIDs = Request["TagIDs"].Split(',');
                if (tagIDs.Length > 0)
                {
                    List<int> tags = new List<int>();
                    foreach (string tag in tagIDs)
                    {
                        if (Numeric.IsNumeric(tag))
                        {
                            tags.Add(Convert.ToInt32(tag));
                        }
                    }
                    m_tagID = tags.ToArray();
                }
            }

            if (Request["Tags"] != null)
            {
                List<int> tags = new List<int>();
                foreach (string tag in Request["Tags"].Split('|'))
                {
                    if (tag != "")
                    {
                        TagController objTagController = new TagController();
                        TagInfo objTag = objTagController.Get(m_moduleID, Server.UrlDecode(tag).ToLower());
                        if (objTag != null)
                        {
                            tags.Add(objTag.TagID);
                        }
                        else
                        {
                            if (m_tagMatch)
                            {
                                tags.Add(Null.NullInteger);
                            }
                        }
                    }
                }
                if (tags.Count > 0)
                {
                    m_tagID = tags.ToArray();
                }
            }

        }

        private string GetParentPortal(string sportalalias)
        {
            if (sportalalias.IndexOf("localhost") < 0)
            {
                if (sportalalias.IndexOf("/") > 0)
                {
                    sportalalias = sportalalias.Substring(0, sportalalias.IndexOf("/"));
                }
            }

            return sportalalias;
        }

        private string FormatTitle(string title)
        {

            return OnlyAlphaNumericChars(title) + ".aspx";

        }

        public string OnlyAlphaNumericChars(string OrigString)
        {
            //'***********************************************************
            //'INPUT:  Any string
            //'OUTPUT: The Input string with all non-alphanumeric characters 
            //'        removed
            //'EXAMPLE Debug.Print OnlyAlphaNumericChars("Hello World!")
            //'output = "HelloWorld")
            //'NOTES:  Not optimized for speed and will run slow on long
            //'        strings.  if you plan on using long strings, consider 
            //'        using alternative method of appending to output string,
            //'        such as the method at
            //'        http://www.freevbcode.com/ShowCode.Asp?ID=154
            //'***********************************************************
            int lLen;
            string sAns = "";
            int lCtr;
            string sChar;

            OrigString = OrigString.Trim();
            lLen = OrigString.Length;
            for (lCtr = 1; lCtr <= lLen; lCtr++)
            {
                sChar = OrigString.Substring(lCtr, 1);
                if (IsAlphaNumeric(OrigString.Substring(lCtr, 1)))
                {
                    sAns = sAns + sChar;
                }
            }

            return sAns;

        }

        private bool IsAlphaNumeric(string sChr)
        {
            Regex reg = new Regex("[0-9A-Za-z ]");
            return reg.IsMatch(sChr);
        }

        private void ProcessHeaderFooter(ControlCollection objPlaceHolder, string[] templateArray)
        {
            Literal objLiteral;

            for (int iPtr = 0; iPtr < templateArray.Length; iPtr += 2)
            {

                objPlaceHolder.Add(new LiteralControl(templateArray[iPtr].ToString()));

                if (iPtr < templateArray.Length - 1)
                {

                    switch (templateArray[iPtr + 1])
                    {

                        case "PORTALNAME":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + iPtr.ToString());
                            objLiteral.Text = Server.HtmlEncode(PortalController.Instance.GetCurrentPortalSettings().PortalName);
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "PORTALURL":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + iPtr.ToString());
                            objLiteral.Text = Server.HtmlEncode(Globals.AddHTTP(PortalController.Instance.GetCurrentPortalSettings().PortalAlias.HTTPAlias));
                            objPlaceHolder.Add(objLiteral);
                            break;
                    }

                }

            }

        }

        private string ProcessItem(string item)
        {

            if (item.Contains("&lt;"))
            {
                // already encoded?
                return item;
            }
            return Server.HtmlEncode(item);

        }

        private void ProcessItem(ControlCollection objPlaceHolder, string[] templateArray, ArticleInfo objArticle, ArticleSettings articleSettings, TabInfo objTab)
        {

            PortalSettings portalSettings = PortalController.Instance.GetCurrentPortalSettings();

            string enclosureLink = "";
            string enclosureType = "";
            string enclosureLength = "";

            if (_enableSyndicationEnclosures)
            {
                if (_enclosureType == SyndicationEnclosureType.Attachment)
                {
                    if (objArticle.FileCount > 0 || objArticle.Url.ToLower().StartsWith("http://") || objArticle.Url.ToLower().StartsWith("https://"))
                    {
                        if (objArticle.FileCount > 0)
                        {

                            FileController objFileController = new FileController();
                            List<FileInfo> objFiles = objFileController.GetFileList(objArticle.ArticleID, "");

                            if (objFiles.Count > 0)
                            {
                                if (System.Web.HttpContext.Current.Request.Url.Port == 80)
                                {
                                    enclosureLink = Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + portalSettings.HomeDirectory + objFiles[0].Folder + objFiles[0].FileName);
                                }
                                else
                                {
                                    enclosureLink = Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port.ToString() + portalSettings.HomeDirectory + objFiles[0].Folder + objFiles[0].FileName);
                                }
                                enclosureType = objFiles[0].ContentType;
                                enclosureLength = objFiles[0].Size.ToString();
                            }

                        }
                        else
                        {
                            if (objArticle.Url.ToLower().StartsWith("http://") || objArticle.Url.ToLower().StartsWith("https://"))
                            {

                                Hashtable objFileInfo = (Hashtable)DataCache.GetCache("NA-" + objArticle.Url);

                                if (objFileInfo == null)
                                {

                                    objFileInfo = new Hashtable();

                                    try
                                    {

                                        Uri Url = new Uri(objArticle.Url);

                                        HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
                                        HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

                                        objFileInfo.Add("ContentType", myHttpWebResponse.ContentType);
                                        objFileInfo.Add("ContentLength", myHttpWebResponse.ContentLength);

                                        myHttpWebResponse.Close();
                                    }
                                    catch
                                    {

                                    }

                                    DataCache.SetCache("NA-" + objArticle.Url, objFileInfo);

                                }

                                if (objFileInfo.Count > 0)
                                {
                                    enclosureLink = objArticle.Url;
                                    enclosureType = objFileInfo["ContentType"].ToString();
                                    enclosureLength = objFileInfo["ContentLength"].ToString();
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (objArticle.ImageCount > 0)
                    {
                        ImageController objImageController = new ImageController();
                        List<ImageInfo> objImages = objImageController.GetImageList(objArticle.ArticleID, Null.NullString);

                        if (objImages.Count > 0)
                        {

                            if (System.Web.HttpContext.Current.Request.Url.Port == 80)
                            {
                                enclosureLink = Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + portalSettings.HomeDirectory + objImages[0].Folder + objImages[0].FileName);
                            }
                            else
                            {
                                enclosureLink = Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port.ToString() + portalSettings.HomeDirectory + objImages[0].Folder + objImages[0].FileName);
                            }

                            enclosureType = objImages[0].ContentType;
                            enclosureLength = objImages[0].Size.ToString();
                        }
                    }
                }
            }

            bool hasEnclosure = false;

            if (enclosureLink != "")
            {
                hasEnclosure = true;
            }

            Literal objLiteral;
            for (int iPtr = 0; iPtr < templateArray.Length; iPtr += 2)
            {

                objPlaceHolder.Add(new LiteralControl(templateArray[iPtr].ToString()));

                if (iPtr < templateArray.Length - 1)
                {

                    switch (templateArray[iPtr + 1])
                    {

                        case "ARTICLELINK":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + objArticle.ArticleID.ToString() + iPtr.ToString());
                            int pageID = Null.NullInteger;
                            if (articleSettings.SyndicationLinkType == SyndicationLinkType.Attachment && (objArticle.Url != "" || objArticle.FileCount > 0))
                            {
                                if (objArticle.FileCount > 0)
                                {
                                    FileController objFileController = new FileController();
                                    List<FileInfo> objFiles = objFileController.GetFileList(objArticle.ArticleID, "");

                                    if (objFiles.Count > 0)
                                    {
                                        if (System.Web.HttpContext.Current.Request.Url.Port == 80)
                                        {
                                            objLiteral.Text = Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + portalSettings.HomeDirectory + objFiles[0].Folder + objFiles[0].FileName).Replace("&", "&amp;");
                                        }
                                        else
                                        {
                                            objLiteral.Text = Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port.ToString() + portalSettings.HomeDirectory + objFiles[0].Folder + objFiles[0].FileName).Replace("&", "&amp;");
                                        }
                                    }
                                }
                                else
                                {
                                    objLiteral.Text = DotNetNuke.Common.Globals.LinkClick(objArticle.Url, m_tabID, objArticle.ModuleID, false).Replace("&", "&amp;");
                                    if (!objLiteral.Text.ToLower().StartsWith("http"))
                                    {
                                        if (System.Web.HttpContext.Current.Request.Url.Port == 80)
                                        {
                                            objLiteral.Text = Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + objLiteral.Text);
                                        }
                                        else
                                        {
                                            objLiteral.Text = Globals.AddHTTP(System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port.ToString() + objLiteral.Text);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                objLiteral.Text = Common.GetArticleLink(objArticle, m_TabInfo, articleSettings, false).Replace("&", "&amp;");
                            }
                            objLiteral.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "COMMENTLINK":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + objArticle.ArticleID.ToString() + iPtr.ToString());
                            objLiteral.Text = Common.GetArticleLink(objArticle, m_TabInfo, articleSettings, false).Replace("&", "&amp;") + "#Comments";
                            objLiteral.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "DESCRIPTION":
                            string description = "";
                            if (Common.StripTags(Server.HtmlDecode(objArticle.Summary)) != "")
                            {
                                if (_enableSyndicationHtml)
                                {
                                    description = ProcessItem(Common.ProcessPostTokens(Server.HtmlDecode(objArticle.Summary), m_TabInfo, articleSettings));
                                }
                                else
                                {
                                    if (_syndicationSummaryLength != Null.NullInteger)
                                    {
                                        string summary = Common.ProcessPostTokens(Common.StripTags(Server.HtmlDecode(objArticle.Summary)), m_TabInfo, articleSettings);
                                        if (summary.Length > _syndicationSummaryLength)
                                        {
                                            description = ProcessItem(Common.ProcessPostTokens(Common.StripTags(Server.HtmlDecode(objArticle.Summary)), m_TabInfo, articleSettings).Substring(0, _syndicationSummaryLength) + "...");
                                        }
                                        else
                                        {
                                            description = ProcessItem(Common.ProcessPostTokens(Common.StripTags(Server.HtmlDecode(objArticle.Summary)), m_TabInfo, articleSettings));
                                        }
                                    }
                                    else
                                    {
                                        description = ProcessItem(Common.ProcessPostTokens(Common.StripTags(Server.HtmlDecode(objArticle.Summary)), m_TabInfo, articleSettings));
                                    }
                                }
                            }
                            else
                            {
                                if (_enableSyndicationHtml)
                                {
                                    description = ProcessItem(Common.ProcessPostTokens(Server.HtmlDecode(objArticle.Body), m_TabInfo, articleSettings));
                                }
                                else
                                {
                                    if (_syndicationSummaryLength != Null.NullInteger)
                                    {
                                        string summary = ProcessItem(Common.ProcessPostTokens(Common.StripTags(Server.HtmlDecode(objArticle.Body)), m_TabInfo, articleSettings));
                                        if (summary.Length > _syndicationSummaryLength)
                                        {
                                            description = ProcessItem(Common.ProcessPostTokens(Common.StripTags(Server.HtmlDecode(objArticle.Body)), m_TabInfo, articleSettings).Substring(0, _syndicationSummaryLength) + "...");
                                        }
                                        else
                                        {
                                            description = ProcessItem(Common.ProcessPostTokens(Common.StripTags(Server.HtmlDecode(objArticle.Body)), m_TabInfo, articleSettings));
                                        }
                                    }
                                    else
                                    {
                                        description = ProcessItem(Common.ProcessPostTokens(Common.StripTags(Server.HtmlDecode(objArticle.Body)), m_TabInfo, articleSettings));
                                    }
                                }
                            }

                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + objArticle.ArticleID.ToString() + iPtr.ToString());
                            objLiteral.Text = description;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "DETAILS":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + objArticle.ArticleID.ToString() + iPtr.ToString());
                            if (objArticle.PageCount > 0)
                            {
                                pageID = Null.NullInteger;
                                if (Numeric.IsNumeric(Request["PageID"]))
                                {
                                    pageID = Convert.ToInt32(Request["PageID"]);
                                }
                                if (pageID == Null.NullInteger)
                                {
                                    objLiteral.Text = ProcessItem(Common.ProcessPostTokens(objArticle.Body, objTab, articleSettings));
                                }
                                else
                                {
                                    PageController pageController = new PageController();
                                    ArrayList pageList = pageController.GetPageList(objArticle.ArticleID);
                                    foreach (PageInfo objPage in pageList)
                                    {
                                        if (objPage.PageID == pageID)
                                        {
                                            objLiteral.Text = ProcessItem(Common.ProcessPostTokens(objPage.PageText, objTab, articleSettings));
                                            break;
                                        }
                                    }
                                    if (objLiteral.Text == Null.NullString)
                                    {
                                        objLiteral.Text = ProcessItem(Common.ProcessPostTokens(objArticle.Body, objTab, articleSettings));
                                    }
                                }
                            }
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "ENCLOSURELENGTH":

                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + objArticle.ArticleID.ToString() + iPtr.ToString());
                            objLiteral.Text = enclosureLength;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "ENCLOSURELINK":

                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + objArticle.ArticleID.ToString() + iPtr.ToString());
                            objLiteral.Text = enclosureLink.Replace("&amp;", "&").Replace("&", "&amp;").Replace(" ", "%20");
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "ENCLOSURETYPE":

                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + objArticle.ArticleID.ToString() + iPtr.ToString());
                            objLiteral.Text = enclosureType;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "GUID":

                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + objArticle.ArticleID.ToString() + iPtr.ToString());
                            objLiteral.Text = "f1397696-738c-4295-afcd-943feb885714:" + objArticle.ArticleID.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "HASENCLOSURE":
                            if (!hasEnclosure)
                            {
                                while (iPtr < templateArray.Length - 1)
                                {
                                    if (templateArray[iPtr + 1] == "/HASENCLOSURE")
                                    {
                                        break;
                                    }
                                    iPtr = iPtr + 1;
                                }
                            }
                            break;
                        case "/HASENCLOSURE":
                            // Do null
                            break;
                        case "IMAGELINK":
                            if (objArticle.ImageUrl != "")
                            {
                                objLiteral = new Literal();
                                objLiteral.Text = objArticle.ImageUrl;
                                objPlaceHolder.Add(objLiteral);
                            }
                            else
                            {
                                ImageController objImageController = new ImageController();
                                List<ImageInfo> objImages = objImageController.GetImageList(objArticle.ArticleID, Null.NullString);

                                if (objImages.Count > 0)
                                {
                                    objLiteral = new Literal();
                                    objLiteral.Text = Globals.AddHTTP(Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port.ToString() + portalSettings.HomeDirectory + objImages[0].Folder + objImages[0].FileName);
                                    objPlaceHolder.Add(objLiteral);
                                }
                            }
                            break;
                        case "PUBLISHDATE":

                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + objArticle.ArticleID.ToString() + iPtr.ToString());
                            objLiteral.Text = objArticle.StartDate.ToUniversalTime().ToString("r");
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "SUMMARY":
                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + objArticle.ArticleID.ToString() + iPtr.ToString());
                            objLiteral.Text = ProcessItem(Common.ProcessPostTokens(objArticle.Summary, objTab, articleSettings));
                            objLiteral.EnableViewState = false;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TITLE":

                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + objArticle.ArticleID.ToString() + iPtr.ToString());
                            objLiteral.Text = Server.HtmlEncode(objArticle.Title);
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TITLEURL":

                            string title = Common.FormatTitle(objArticle.Title, articleSettings);

                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + objArticle.ArticleID.ToString() + iPtr.ToString());
                            objLiteral.Text = Server.HtmlEncode(title);
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TRACKBACKLINK":

                            string link = "";
                            if (System.Web.HttpContext.Current.Request.Url.Port == 80)
                            {
                                link = Globals.AddHTTP(Request.Url.Host + this.ResolveUrl("Tracking/Trackback.aspx?ArticleID=" + objArticle.ArticleID.ToString() + "&amp;PortalID=" + portalSettings.PortalId.ToString() + "&amp;TabID=" + portalSettings.ActiveTab.TabID.ToString()).Replace(" ", "%20"));
                            }
                            else
                            {
                                link = Globals.AddHTTP(Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port.ToString() + this.ResolveUrl("Tracking/Trackback.aspx?ArticleID=" + objArticle.ArticleID.ToString() + "&amp;PortalID=" + portalSettings.PortalId.ToString() + "&amp;TabID=" + portalSettings.ActiveTab.TabID.ToString()).Replace(" ", "%20"));
                            }

                            objLiteral = new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + objArticle.ArticleID.ToString() + iPtr.ToString());
                            objLiteral.Text = link;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        default:

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("DETAILS:"))
                            {
                                int length = Convert.ToInt32(templateArray[iPtr + 1].Substring(8, templateArray[iPtr + 1].Length - 8));

                                objLiteral = new Literal();
                                objLiteral.ID = Globals.CreateValidID("Rss-" + objArticle.ArticleID.ToString() + iPtr.ToString());
                                if (StripHtml(Server.HtmlDecode(objArticle.Body)).TrimStart().Length > length)
                                {
                                    objLiteral.Text = ProcessItem(Common.ProcessPostTokens(StripHtml(Server.HtmlDecode(objArticle.Body)).TrimStart().Substring(0, length), objTab, articleSettings) + "...");
                                }
                                else
                                {
                                    objLiteral.Text = ProcessItem(Common.ProcessPostTokens(StripHtml(Server.HtmlDecode(objArticle.Body)).TrimStart().Substring(0, StripHtml(Server.HtmlDecode(objArticle.Body)).TrimStart().Length), objTab, articleSettings));
                                }

                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                                break;
                            }

                            if (templateArray[iPtr + 1].ToUpper().StartsWith("SUMMARY:"))
                            {
                                string summary = objArticle.Summary;
                                if (Numeric.IsNumeric(templateArray[iPtr + 1].Substring(8, templateArray[iPtr + 1].Length - 8)))
                                {
                                    int length = Convert.ToInt32(templateArray[iPtr + 1].Substring(8, templateArray[iPtr + 1].Length - 8));
                                    if (StripHtml(Server.HtmlDecode(objArticle.Summary)).TrimStart().Length > length)
                                    {
                                        summary = ProcessItem(Common.ProcessPostTokens(StripHtml(Server.HtmlDecode(objArticle.Summary)).TrimStart().Substring(0, length), objTab, articleSettings) + "...");
                                    }
                                    else
                                    {
                                        summary = ProcessItem(Common.ProcessPostTokens(StripHtml(Server.HtmlDecode(objArticle.Summary)).TrimStart().Substring(0, StripHtml(Server.HtmlDecode(objArticle.Summary)).TrimStart().Length), objTab, articleSettings));
                                    }
                                }

                                objLiteral = new Literal();
                                objLiteral.ID = Globals.CreateValidID("Rss-" + objArticle.ArticleID.ToString() + iPtr.ToString());
                                objLiteral.Text = summary;
                                objLiteral.EnableViewState = false;
                                objPlaceHolder.Add(objLiteral);
                                break;
                            }

                            Literal objLiteralOther = new Literal();
                            objLiteralOther.ID = Globals.CreateValidID("Rss-" + objArticle.ArticleID.ToString() + iPtr.ToString());
                            objLiteralOther.Text = "[" + templateArray[iPtr + 1] + "]";
                            objLiteralOther.EnableViewState = false;
                            objPlaceHolder.Add(objLiteralOther);
                            break;
                    }

                }

            }

        }

        private string RenderControlToString(Control ctrl)
        {

            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);

            ctrl.RenderControl(hw);

            return sb.ToString();

        }

        private string StripHtml(string html)
        {

            string pattern = "<(.|\n)*?>";
            return Regex.Replace(html, pattern, string.Empty);

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Load(object sender, EventArgs e)
        {

            ReadQueryString();

            DisplayType displayType = DisplayType.UserName;
            bool launchLinks = false;
            bool showPending = false;

            PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            ModuleController objModuleController = new ModuleController();
            ModuleInfo objModule = objModuleController.GetModule(m_moduleID, m_tabID);
            ArticleSettings articleSettings;

            if (objModule != null)
            {
                TabController objTabController = new TabController();
                TabInfo objTab = objTabController.GetTab(objModule.TabID, _portalSettings.PortalId, false);

                Hashtable settings = objModuleController.GetModuleSettings(objModule.ModuleID);
                settings = PortalSettings.GetTabModuleSettings(objModule.TabModuleID, settings);
                articleSettings = new ArticleSettings(settings, _portalSettings, objModule);
                if (settings.Contains(ArticleConstants.LAUNCH_LINKS))
                {
                    launchLinks = Convert.ToBoolean(settings[ArticleConstants.LAUNCH_LINKS].ToString());
                }
                if (settings.Contains(ArticleConstants.TEMPLATE_SETTING))
                {
                    _template = settings[ArticleConstants.TEMPLATE_SETTING].ToString();
                }
                if (settings.Contains(ArticleConstants.ENABLE_SYNDICATION_ENCLOSURES_SETTING))
                {
                    _enableSyndicationEnclosures = Convert.ToBoolean(settings[ArticleConstants.ENABLE_SYNDICATION_ENCLOSURES_SETTING].ToString());
                }
                if (settings.Contains(ArticleConstants.SYNDICATION_ENCLOSURE_TYPE))
                {
                    _enclosureType = (SyndicationEnclosureType)System.Enum.Parse(typeof(SyndicationEnclosureType), settings[ArticleConstants.SYNDICATION_ENCLOSURE_TYPE].ToString());
                }
                if (settings.Contains(ArticleConstants.ENABLE_SYNDICATION_HTML_SETTING))
                {
                    _enableSyndicationHtml = Convert.ToBoolean(settings[ArticleConstants.ENABLE_SYNDICATION_HTML_SETTING].ToString());
                }
                Hashtable settingsModule = objModuleController.GetModuleSettings(objModule.ModuleID);
                if (settingsModule.Contains(ArticleConstants.SYNDICATION_SUMMARY_LENGTH))
                {
                    _syndicationSummaryLength = Convert.ToInt32(settingsModule[ArticleConstants.SYNDICATION_SUMMARY_LENGTH].ToString());
                }
                if (settings.Contains(ArticleConstants.SHOW_PENDING_SETTING))
                {
                    showPending = Convert.ToBoolean(settings[ArticleConstants.SHOW_PENDING_SETTING].ToString());
                }
                if (settings.Contains(ArticleConstants.DISPLAY_MODE))
                {
                    displayType = (DisplayType)System.Enum.Parse(typeof(DisplayType), settings[ArticleConstants.DISPLAY_MODE].ToString());
                }
                if (m_count == Null.NullInteger)
                {
                    if (settings.Contains(ArticleConstants.SYNDICATION_MAX_COUNT))
                    {
                        try
                        {
                            m_count = Convert.ToInt32(settings[ArticleConstants.SYNDICATION_MAX_COUNT].ToString());
                        }
                        catch
                        {
                            m_count = 50;
                        }
                    }
                    else
                    {
                        m_count = 50;
                    }
                }
                if (m_categoryID == null)
                {
                    if (settings.Contains(ArticleConstants.CATEGORIES_SETTING + m_tabID.ToString()))
                    {
                        if (settings[ArticleConstants.CATEGORIES_SETTING + m_tabID.ToString()].ToString() != Null.NullString && settings[ArticleConstants.CATEGORIES_SETTING + m_tabID.ToString()].ToString() != "-1")
                        {
                            string[] categories = settings[ArticleConstants.CATEGORIES_SETTING + m_tabID.ToString()].ToString().Split(',');
                            List<int> cats = new List<int>();

                            foreach (string category in categories)
                            {
                                if (Numeric.IsNumeric(category))
                                {
                                    cats.Add(Convert.ToInt32(category));
                                }
                            }

                            m_categoryID = cats.ToArray();
                        }
                    }
                }

                if (m_categoryID != null)
                {
                    if (m_categoryID.Length > 0)
                    {
                        if (settings.Contains(ArticleConstants.MATCH_OPERATOR_SETTING))
                        {
                            MatchOperatorType objMatchOperator = (MatchOperatorType)System.Enum.Parse(typeof(MatchOperatorType), settings[ArticleConstants.MATCH_OPERATOR_SETTING].ToString());
                            if (objMatchOperator == MatchOperatorType.MatchAll)
                            {
                                m_matchAll = true;
                            }
                        }

                        if (Request["MatchCat"] != null && Request["CategoryID"] != null)
                        {
                            m_matchAll = true;
                        }
                    }
                }

                LayoutController objLayoutController = new LayoutController(_portalSettings, articleSettings, objModule, Page);
                //Dim objLayoutController As new LayoutController(_portalSettings, articleSettings, this, false, m_tabID, m_moduleID, objModule.TabModuleID, _portalSettings.PortalId, Null.NullInteger, Null.NullInteger, "Rss-" + m_tabID.ToString())

                LayoutInfo layoutHeader = LayoutController.GetLayout(articleSettings, objModule, Page, LayoutType.Rss_Header_Html);
                LayoutInfo layoutItem = LayoutController.GetLayout(articleSettings, objModule, Page, LayoutType.Rss_Item_Html);
                LayoutInfo layoutFooter = LayoutController.GetLayout(articleSettings, objModule, Page, LayoutType.Rss_Footer_Html);

                PlaceHolder phRSS = new PlaceHolder();

                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;

                ProcessHeaderFooter(phRSS.Controls, layoutHeader.Tokens);

                DateTime agedDate = Null.NullDate;
                DateTime startDate = DateTime.Now.AddMinutes(1);
                if (m_year != Null.NullInteger && m_month != Null.NullInteger)
                {
                    agedDate = new DateTime(m_year, m_month, 1);
                    startDate = agedDate.AddMonths(1).AddSeconds(-1);
                }

                if (m_categoryID == null)
                {

                    // Permission to view category?
                    CategoryController objCategoryController = new CategoryController();
                    List<CategoryInfo> objCategories = objCategoryController.GetCategoriesAll(m_moduleID, Null.NullInteger);
                    bool checkCategory = false;

                    List<int> excludeCategories = new List<int>();
                    foreach (CategoryInfo objCategory in objCategories)
                    {
                        if (!objCategory.InheritSecurity && objCategory.CategorySecurityType == CategorySecurityType.Restrict)
                        {
                            excludeCategories.Add(objCategory.CategoryID);
                        }
                    }
                    if (excludeCategories.Count > 0)
                    {
                        m_categoryIDExclude = excludeCategories.ToArray();
                    }

                    foreach (CategoryInfo objCategory in objCategories)
                    {
                        if (!objCategory.InheritSecurity && objCategory.CategorySecurityType == CategorySecurityType.Loose)
                        {
                            checkCategory = true;
                        }
                    }

                    if (checkCategory)
                    {
                        if (m_categoryID == null)
                        {
                            List<int> includeCategories = new List<int>();

                            foreach (CategoryInfo objCategory in objCategories)
                            {
                                if (objCategory.InheritSecurity)
                                {
                                    includeCategories.Add(objCategory.CategoryID);
                                }
                            }

                            if (includeCategories.Count > 0)
                            {
                                includeCategories.Add(-1);
                            }

                            m_categoryID = includeCategories.ToArray();
                        }
                        else
                        {
                            List<int> includeCategories = new List<int>();

                            foreach (int i in m_categoryID)
                            {
                                foreach (CategoryInfo objCategory in objCategories)
                                {
                                    if (i == objCategory.CategoryID)
                                    {
                                        if (objCategory.InheritSecurity)
                                        {
                                            includeCategories.Add(objCategory.CategoryID);
                                        }
                                    }
                                }
                            }

                            m_categoryID = includeCategories.ToArray();
                        }
                    }

                }

                ArticleController objArticleController = new ArticleController();
                int total = Null.NullInteger;
                List<ArticleInfo> articleList = objArticleController.GetArticleList(m_moduleID, startDate, agedDate, m_categoryID, m_matchAll, m_categoryIDExclude, m_count, 1, m_count, m_sortBy, m_sortDirection, true, false, Null.NullString, m_authorID, showPending, m_showExpired, m_featuredOnly, m_notFeaturedOnly, m_securedOnly, m_notSecuredOnly, m_articleIDs, m_tagID, m_tagMatch, Null.NullString, Null.NullInteger, Null.NullString, Null.NullString, ref total);

                foreach (ArticleInfo objArticle in articleList)
                {

                    string delimStr = "[]";
                    char[] delimiter = delimStr.ToCharArray();

                    PlaceHolder phItem = new PlaceHolder();
                    ProcessItem(phItem.Controls, layoutItem.Tokens, objArticle, articleSettings, objTab);
                    objLayoutController.ProcessArticleItem(phRSS.Controls, RenderControlToString(phItem).Split(delimiter), objArticle);

                }

                ProcessHeaderFooter(phRSS.Controls, layoutFooter.Tokens);

                Response.Write(RenderControlToString(phRSS));

            }

            Response.End();

        }

        #endregion
    }
}