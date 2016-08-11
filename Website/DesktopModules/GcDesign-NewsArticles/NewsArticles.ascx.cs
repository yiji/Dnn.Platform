using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using GcDesign.NewsArticles.Components.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;

using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class NewsArticles : NewsArticleModuleBase
    {
        #region " Private Members "

        private string m_controlToLoad;

        #endregion

        #region " Private Methods "

        private void ReadQueryString()
        {

            if (Request["articleType"] != null)
            {

                // Load the appropriate Control

                switch (Request["articleType"].ToLower())
                {

                    case "archives":
                        m_controlToLoad = "Archives.ascx";
                        break;
                    case "approvearticles":
                        m_controlToLoad = "ucApproveArticles.ascx";
                        break;
                    case "approvecomments":
                        m_controlToLoad = "ucApproveComments.ascx";
                        break;
                    case "articleview":
                        m_controlToLoad = "ViewArticle.ascx";
                        break;
                    case "archiveview":
                        m_controlToLoad = "ViewArchive.ascx";
                        break;
                    case "authorview":
                        m_controlToLoad = "ViewAuthor.ascx";
                        break;
                    case "categories":
                        Response.Status = "301 Moved Permanently";
                        Response.AddHeader("Location", Common.GetModuleLink(this.TabId, this.ModuleId, "Archives", ArticleSettings));
                        Response.End();
                        break;
                    case "categoryview":
                        m_controlToLoad = "ViewCategory.ascx";
                        break;
                    case "editcomment":
                        m_controlToLoad = "ucEditComment.ascx";
                        break;
                    case "editpage":
                        m_controlToLoad = "ucEditPage.ascx";
                        break;
                    case "editpages":
                        m_controlToLoad = "ucEditPages.ascx";
                        break;
                    case "editsortorder":
                        m_controlToLoad = "ucEditPageSortOrder.ascx";
                        break;
                    case "myarticles":
                        m_controlToLoad = "ucMyArticles.ascx";
                        break;
                    case "notauthenticated":
                        m_controlToLoad = "ucNotAuthenticated.ascx";
                        break;
                    case "notauthorized":
                        m_controlToLoad = "ucNotAuthorized.ascx";
                        break;
                    case "search":
                        m_controlToLoad = "ViewSearch.ascx";
                        break;
                    case "submitnews":
                        m_controlToLoad = "ucSubmitNews.ascx";
                        break;
                    case "submitnewscomplete":
                        m_controlToLoad = "ucSubmitNewsComplete.ascx";
                        break;
                    case "syndication":
                        Response.Status = "301 Moved Permanently";
                        Response.AddHeader("Location", Common.GetModuleLink(this.TabId, this.ModuleId, "Archives", ArticleSettings));
                        Response.End();
                        break;
                    case "tagview":
                        m_controlToLoad = "ViewTag.ascx";
                        break;
                    default:
                        m_controlToLoad = "ViewCurrent.ascx";
                        break;
                }

            }
            else
            {

                // Type parameter not found
                //
                if (ArticleSettings.FilterSingleCategory != Null.NullInteger)
                {
                    m_controlToLoad = "ViewCategory.ascx";
                }
                else
                {
                    if (ArticleSettings.UrlModeType == Components.Types.UrlModeType.Classic)
                    {
                        m_controlToLoad = "ViewCurrent.ascx";
                    }
                    else
                    {
                        if (Request[ArticleSettings.ShortenedID] != null)//if (Request[ArticleSettings.ShortenedID] != "")
                        {
                            m_controlToLoad = "ViewArticle.ascx";
                        }
                        else
                        {
                            m_controlToLoad = "ViewCurrent.ascx";
                        }
                    }
                }

            }

        }

        private void LoadControlType()
        {

            if (m_controlToLoad != "")
            {
                PortalModuleBase objPortalModuleBase = (PortalModuleBase)this.LoadControl(m_controlToLoad);

                if (objPortalModuleBase != null)
                {

                    objPortalModuleBase.ModuleConfiguration = this.ModuleConfiguration;
                    objPortalModuleBase.ID = System.IO.Path.GetFileNameWithoutExtension(m_controlToLoad);
                    plhControls.Controls.Add(objPortalModuleBase);

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

        protected void Page_Init(object sender, EventArgs e)
        {

            try
            {

                ReadQueryString();
                LoadControlType();

                Literal litLinkLiveWriter = new Literal();
                litLinkLiveWriter.Text = "" + "\r\n" +
                    "<link rel=\"wlwmanifest\" type=\"application/wlwmanifest+xml\" title=\"windows livewriter manifest\" href=\"" + ArticleUtilities.ToAbsoluteUrl(@"~/desktopmodules/GcDesign%20-%20Searches/api/metaweblog/wlwmanifest.xml") + "\" />" + "\r\n";
                Page.Header.Controls.Add(litLinkLiveWriter);

                Literal litLinkRsd = new Literal();
                litLinkRsd.Text = "" + "\r\n" +
                    "<link type=\"application/rsd+xml\" rel=\"EditURI\" title=\"RSD\" href=\"" + ArticleUtilities.ToAbsoluteUrl(@"~/desktopmodules/GcDesign%20-%20Searches/api/rsd.ashx") + "?id=" + TabModuleId + "&url=" + (DotNetNuke.Common.Globals.NavigateURL(TabId)) + "\" />" + "\r\n";
                Page.Header.Controls.Add(litLinkRsd);
            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }


        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            try
            {

                jQuery.RegisterJQuery(Page);

            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion
    }
}