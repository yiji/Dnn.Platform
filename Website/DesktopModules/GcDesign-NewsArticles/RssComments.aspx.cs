using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Text;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using System.IO;
using System.Collections;

namespace GcDesign.NewsArticles
{
    public partial class RssComments : System.Web.UI.Page
    {
#region " private Members "

        private int m_articleID = Null.NullInteger;
        private int m_tabID = Null.NullInteger;
        private DotNetNuke.Entities.Tabs.TabInfo m_TabInfo;
        private int m_moduleID = Null.NullInteger;

#endregion

#region " private Methods "

        private void ProcessHeaderFooter(ControlCollection objPlaceHolder , string[] templateArray){

            Literal objLiteral;
            for(int iPtr = 0 ;iPtr< templateArray.Length;iPtr+=2){

                objPlaceHolder.Add(new LiteralControl(templateArray[iPtr].ToString()));

                if (iPtr < templateArray.Length - 1) {

                    switch (templateArray[iPtr + 1]){

                        case "PORTALEMAIL":
                            objLiteral =new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + iPtr.ToString());
                            objLiteral.Text = PortalController.Instance.GetCurrentPortalSettings().Email;
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "PORTALNAME":
                            objLiteral =new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + iPtr.ToString());
                            objLiteral.Text = Server.HtmlEncode(PortalController.Instance.GetCurrentPortalSettings().PortalName);
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "PORTALURL":
                            objLiteral =new Literal();
                            objLiteral.ID = Globals.CreateValidID("Rss-" + iPtr.ToString());
                            objLiteral.Text = Server.HtmlEncode( Globals.AddHTTP(PortalController.Instance.GetCurrentPortalSettings().PortalAlias.HTTPAlias));
                            objPlaceHolder.Add(objLiteral);
                            break;
                }

            }

        }

        }

        private void ProcessItem(ControlCollection objPlaceHolder , string[] templateArray , ArticleInfo objArticle  , CommentInfo objComment , ArticleSettings articleSettings )
        {

            PortalSettings portalSettings = PortalController.Instance.GetCurrentPortalSettings();
            Literal objLiteral;
            for(int iPtr = 0 ;iPtr< templateArray.Length ;iPtr+=2){

                objPlaceHolder.Add(new LiteralControl(templateArray[iPtr].ToString()));

                if (iPtr < templateArray.Length - 1) {

                    switch(templateArray[iPtr + 1]){

                        case "CREATEDATE":

                            objLiteral =new Literal();
                            objLiteral.ID = Globals.CreateValidID("RssComment-" + objComment.CommentID.ToString() + iPtr.ToString());
                            objLiteral.Text = objComment.CreatedDate.ToUniversalTime().ToString("r");
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "DESCRIPTION":

                            objLiteral =new Literal();
                            objLiteral.ID = Globals.CreateValidID("RssComment-" + objComment.CommentID.ToString() + iPtr.ToString());
                            objLiteral.Text = Server.HtmlEncode(objComment.Comment);
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "GUID":

                            objLiteral =new Literal();
                            objLiteral.ID = Globals.CreateValidID("RssComment-" + objComment.CommentID.ToString() + iPtr.ToString());
                            objLiteral.Text = "f1397696-738c-4295-afcd-943feb885714:" + objComment.CommentID.ToString();
                            objPlaceHolder.Add(objLiteral);
                            break;
                        case "TITLE":

                            objLiteral =new Literal();
                            objLiteral.ID = Globals.CreateValidID("RssComment-" + objComment.CommentID.ToString() + iPtr.ToString());
                            objLiteral.Text = Server.HtmlEncode(objArticle.Title);
                            objPlaceHolder.Add(objLiteral);
                            break;
                        default:

                            Literal objLiteralOther = new Literal();
                            objLiteralOther.ID = Globals.CreateValidID("RssComment-" + objComment.CommentID.ToString() + iPtr.ToString());
                            objLiteralOther.Text = "[" + templateArray[iPtr + 1] + "]";
                            objLiteralOther.EnableViewState = false;
                            objPlaceHolder.Add(objLiteralOther);
                            break;
                    }

                }

            }

        }

        private void ReadQueryString(){

            if (Request["ArticleID"] != null)
            {
                if (Numeric.IsNumeric(Request["ArticleID"])) {
                    m_articleID = Convert.ToInt32(Request["ArticleID"]);
                }
            }

            if (Request["TabID"] != null)
            {
                if (Numeric.IsNumeric(Request["TabID"])) {
                    m_tabID = Convert.ToInt32(Request["TabID"]);
                    DotNetNuke.Entities.Tabs.TabController objTabController = new DotNetNuke.Entities.Tabs.TabController();
                    m_TabInfo = objTabController.GetTab(m_tabID, Globals.GetPortalSettings().PortalId, false);
                }
            }

            if (Request["ModuleID"] != null)
            {
                if (Numeric.IsNumeric(Request["ModuleID"])) {
                    m_moduleID = Convert.ToInt32(Request["ModuleID"]);
                }
            }

        }

        private string RenderControlToString(Control ctrl ){

            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);

            ctrl.RenderControl(hw);

            return sb.ToString();

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

        protected void Page_Load(object sender , EventArgs e ) {

            ReadQueryString();

            bool launchLinks = false;
            bool enableSyndicationHtml = false;

            PortalSettings _portalSettings = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            ModuleController objModuleController = new ModuleController();
            ModuleInfo objModule = objModuleController.GetModule(m_moduleID, m_tabID);
            ArticleSettings articleSettings;

            if  (objModule != null) {
                Hashtable settings = objModuleController.GetTabModuleSettings(objModule.TabModuleID);
                articleSettings = new ArticleSettings(settings, _portalSettings, objModule);
                if (settings.Contains(ArticleConstants.LAUNCH_LINKS)) {
                    launchLinks = Convert.ToBoolean(settings[ArticleConstants.LAUNCH_LINKS].ToString());
                }
                if (settings.Contains(ArticleConstants.ENABLE_SYNDICATION_HTML_SETTING)) {
                    enableSyndicationHtml = Convert.ToBoolean(settings[ArticleConstants.ENABLE_SYNDICATION_HTML_SETTING].ToString());
                }
                if (settings.Contains(ArticleConstants.DISPLAY_MODE)) {
                }

                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;

                ArticleController objArticleController = new ArticleController();
                ArticleInfo objArticle = objArticleController.GetArticle(m_articleID);

                LayoutController objLayoutController = new LayoutController(_portalSettings, articleSettings, objModule, Page);
                // Dim objLayoutController As new LayoutController(_portalSettings, articleSettings, Me, False, m_tabID, m_moduleID, objModule.TabModuleID, _portalSettings.PortalId, Null.NullInteger, Null.NullInteger, "RssComment-" + m_tabID.ToString())

                LayoutInfo layoutHeader = LayoutController.GetLayout(articleSettings, objModule, Page, LayoutType.Rss_Comment_Header_Html);
                LayoutInfo layoutItem = LayoutController.GetLayout(articleSettings, objModule, Page, LayoutType.Rss_Comment_Item_Html);
                LayoutInfo layoutFooter = LayoutController.GetLayout(articleSettings, objModule, Page, (LayoutType.Rss_Comment_Footer_Html));

                PlaceHolder phRSS = new PlaceHolder();

                ProcessHeaderFooter(phRSS.Controls, layoutHeader.Tokens);

                CommentController objCommentController = new CommentController();
                List<CommentInfo> commentList = objCommentController.GetCommentList(m_moduleID, m_articleID, true, SortDirection.Ascending, Null.NullInteger);

                foreach(CommentInfo objComment in commentList){

                    string delimStr = "[]";
                    char[] delimiter = delimStr.ToCharArray();

                    PlaceHolder phItem = new PlaceHolder();
                    ProcessItem(phItem.Controls, layoutItem.Tokens, objArticle, objComment, articleSettings);

                    objLayoutController.ProcessComment(phRSS.Controls, objArticle, objComment, RenderControlToString(phItem).Split(delimiter));
                }

                ProcessHeaderFooter(phRSS.Controls, layoutFooter.Tokens);

                Response.Write(RenderControlToString(phRSS));

            }

            Response.End();

        }

#endregion
    }
}