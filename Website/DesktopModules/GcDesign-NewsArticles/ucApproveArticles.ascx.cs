using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Security.Roles;
using GcDesign.NewsArticles.Components.Social;

using GcDesign.NewsArticles.Base;
using System.Collections;
using System.IO;

namespace GcDesign.NewsArticles
{
    public partial class ucApproveArticles : NewsArticleModuleBase
    {
        #region " private Properties "

        private int CurrentPage
        {
            get
            {
                if (Request["Page"] == null && Request["CurrentPage"] == null)
                {
                    return 1;
                }
                else
                {
                    try
                    {
                        if (Request["Page"] != null)
                        {
                            return Convert.ToInt32(Request["Page"]);
                        }
                        else
                        {
                            return Convert.ToInt32(Request["CurrentPage"]);
                        }
                    }
                    catch
                    {
                        return 1;
                    }
                }
            }
        }

        #endregion

        #region " private Methods "

        private bool IsInRole(string roleName, string[] roles)
        {

            foreach (string role in roles)
            {
                if (roleName == role)
                {
                    return true;
                }
            }

            return false;

        }

        private void BindArticles()
        {

            int count = 0;

            ArticleController objArticleController = new ArticleController();

            DotNetNuke.Services.Localization.Localization.LocalizeDataGrid(ref grdArticles, this.LocalResourceFile);

            grdArticles.DataSource = objArticleController.GetArticleList(this.ModuleId, Null.NullDate, Null.NullDate, null, Null.NullBoolean, null, Null.NullInteger, CurrentPage, 20, "StartDate", "DESC", false, Null.NullBoolean, Null.NullString, Null.NullInteger, true, true, Null.NullBoolean, Null.NullBoolean, false, false, Null.NullString, null, false, Null.NullString, Null.NullInteger, Null.NullString, Null.NullString, ref count);
            grdArticles.DataBind();

            if (grdArticles.Items.Count == 0)
            {
                lblNoArticles.Visible = true;
                lblNoArticles.Text = DotNetNuke.Services.Localization.Localization.GetString("NoArticlesMessage.Text", LocalResourceFile);
                grdArticles.Visible = false;

                ctlPagingControl.Visible = false;
            }
            else
            {
                lblNoArticles.Visible = false;
                grdArticles.Visible = true;

                ctlPagingControl.Visible = true;
                ctlPagingControl.TotalRecords = count;
                ctlPagingControl.PageSize = 20;
                ctlPagingControl.CurrentPage = CurrentPage;
                ctlPagingControl.QuerystringParams = GetParams();
                ctlPagingControl.TabID = TabId;
                ctlPagingControl.EnableViewState = false;
            }

        }

        private string GetParams()
        {

            string param = "";

            if (Request["ctl"] != null)
            {
                if (Request["ctl"].ToLower() == "approvearticles")
                {
                    param += "ctl=" + Request["ctl"] + "&mid=" + ModuleId.ToString();
                }
            }

            if (Request["articleType"] != null)
            {
                if (Request["articleType"].ToString().ToLower() == "approvearticles")
                {
                    param += "articleType=" + Request["articleType"];
                }
            }

            return param;

        }

        private void NotifyAuthor(ArticleInfo objArticle)
        {

            if (Settings.Contains(ArticleConstants.NOTIFY_APPROVAL_SETTING))
            {
                if (Convert.ToBoolean(Settings[ArticleConstants.NOTIFY_APPROVAL_SETTING]))
                {
                    UserController objUserController = new UserController();
                    UserInfo objUser = objUserController.GetUser(this.PortalId, objArticle.AuthorID);

                    EmailTemplateController objEmailTemplateController = new EmailTemplateController();
                    if (objUser != null)
                    {
                        objEmailTemplateController.SendFormattedEmail(this.ModuleId, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false), objArticle, EmailTemplateType.ArticleApproved, objUser.Membership.Email, ArticleSettings);
                    }

                }
            }

        }

        private void CheckSecurity()
        {

            if (!Request.IsAuthenticated)
            {
                Response.Redirect(Common.GetModuleLink(this.TabId, this.ModuleId, "NotAuthenticated", ArticleSettings), true);
            }

            if (ArticleSettings.IsApprover)
            {
                return;
            }

            Response.Redirect(Common.GetModuleLink(this.TabId, this.ModuleId, "NotAuthorized", ArticleSettings), true);

        }

        #endregion

        #region " protected Methods "

        protected string GetAdjustedCreateDate(object objItem)
        {

            ArticleInfo objArticle = (ArticleInfo)objItem;
            return objArticle.CreatedDate.ToString("d") + " " + objArticle.CreatedDate.ToString("t");

        }

        protected string GetAdjustedPublishDate(object objItem)
        {

            ArticleInfo objArticle = (ArticleInfo)objItem;
            return objArticle.StartDate.ToString("d") + " " + objArticle.StartDate.ToString("t");

        }

        protected string GetArticleLink(object objItem)
        {

            ArticleInfo objArticle = (ArticleInfo)objItem;
            return Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false);

        }

        protected string GetEditUrl(string articleID)
        {
            if (ArticleSettings.LaunchLinks)
            {
                return Common.GetModuleLink(this.TabId, this.ModuleId, "Edit", ArticleSettings, "ArticleID=" + articleID, "returnurl=" + Server.UrlEncode(Request.Url.ToString()));
            }
            else
            {
                return Common.GetModuleLink(this.TabId, this.ModuleId, "SubmitNews", ArticleSettings, "ArticleID=" + articleID, "returnurl=" + Server.UrlEncode(Request.Url.ToString()));
            }
        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            cmdApproveAll.Click+=cmdApproveAll_Click;
            cmdApproveSelected.Click += cmdApproveSelected_OnClick;
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Load(object sender, EventArgs e)
        {

            try
            {

                CheckSecurity();

                if (!IsPostBack)
                {
                    BindArticles();
                }
            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdApproveSelected_OnClick(object sender, EventArgs e)
        {

            try
            {

                ArticleController objArticleController = new ArticleController();

                for (int i = 0; i < grdArticles.Items.Count; i++)
                {

                    DataGridItem currentItem = grdArticles.Items[i];

                    if (currentItem.FindControl("chkArticle") != null)
                    {
                        CheckBox chkArticle = (CheckBox)currentItem.FindControl("chkArticle");

                        if (chkArticle.Checked)
                        {
                            ArticleInfo objArticle = objArticleController.GetArticle(Convert.ToInt32(grdArticles.DataKeys[i]));

                            objArticle.Status = StatusType.Published;
                            objArticleController.UpdateArticle(objArticle);

                            NotifyAuthor(objArticle);

                            if (ArticleSettings.EnableAutoTrackback)
                            {
                                GcDesign.NewsArticles.Tracking.Notification objNotifications = new Tracking.Notification();
                                objNotifications.NotifyExternalSites(objArticle, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false), this.PortalSettings.PortalName);
                            }

                            if (ArticleSettings.EnableNotificationPing)
                            {
                                GcDesign.NewsArticles.Tracking.Notification objNotifications = new Tracking.Notification();
                                objNotifications.NotifyWeblogs(Globals.AddHTTP(Globals.NavigateURL()), this.PortalSettings.PortalName);
                            }

                            if (ArticleSettings.JournalIntegration)
                            {
                                Journal objJournal = new Journal();
                                objJournal.AddArticleToJournal(objArticle, PortalId, TabId, this.UserId, Null.NullInteger, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false));
                            }

                            if (ArticleSettings.JournalIntegrationGroups)
                            {

                                ArrayList objCategories = objArticleController.GetArticleCategories(objArticle.ArticleID);

                                if (objCategories.Count > 0)
                                {

                                    RoleController objRoleController = new RoleController();

                                    ArrayList objRoles = objRoleController.GetRoles();
                                    foreach (RoleInfo objRole in objRoles)
                                    {
                                        bool roleAccess = false;

                                        if (objRole.SecurityMode == SecurityMode.SocialGroup || objRole.SecurityMode == SecurityMode.Both)
                                        {

                                            foreach (CategoryInfo objCategory in objCategories)
                                            {

                                                if (objCategory.InheritSecurity = false)
                                                {

                                                    if (objCategory.CategorySecurityType == CategorySecurityType.Loose)
                                                    {
                                                        roleAccess = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        if (Settings.Contains(objCategory.CategoryID.ToString() + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                                                        {
                                                            if (IsInRole(objRole.RoleName, Settings[objCategory.CategoryID.ToString() + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString().Split(';')))
                                                            {
                                                                roleAccess = true;
                                                            }
                                                        }
                                                    }

                                                }

                                            }

                                        }

                                        if (roleAccess)
                                        {
                                            Journal objJournal = new Journal();
                                            objJournal.AddArticleToJournal(objArticle, PortalId, TabId, this.UserId, objRole.RoleID, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false));
                                        }

                                    }

                                }

                            }

                            if (ArticleSettings.EnableSmartThinkerStoryFeed)
                            {
                                //StoryFeedWS objStoryFeed = new wsStoryFeed.StoryFeedWS
                                //objStoryFeed.Url = DotNetNuke.Common.Globals.AddHTTP(Request.ServerVariables("HTTP_HOST") + this.ResolveUrl("~/DesktopModules/Smart-Thinker%20-%20UserProfile/StoryFeed.asmx"))

                                string val = GetSharedResource("StoryFeed-AddArticle");

                                val = val.Replace("[AUTHOR]", objArticle.AuthorDisplayName);
                                val = val.Replace("[AUTHORID]", objArticle.AuthorID.ToString());
                                val = val.Replace("[ARTICLELINK]", Common.GetArticleLink(objArticle, this.PortalSettings.ActiveTab, ArticleSettings, false));
                                val = val.Replace("[ARTICLETITLE]", objArticle.Title);

                                try
                                {
                                    //objStoryFeed.AddAction(80, objArticle.ArticleID, val, objArticle.AuthorID, "VE6457624576460436531768")
                                }
                                catch { }
                            }

                            if (ArticleSettings.EnableActiveSocialFeed)
                            {
                                if (ArticleSettings.ActiveSocialSubmitKey != "")
                                {
                                    if (File.Exists(HttpContext.Current.Server.MapPath(@"~/bin/active.modules.social.dll")))
                                    {
                                        object ai = null;
                                        System.Reflection.Assembly asm;
                                        object ac = null;
                                        try
                                        {
                                            asm = System.Reflection.Assembly.Load("Active.Modules.Social");
                                            ac = asm.CreateInstance("Active.Modules.Social.API.Journal");
                                            if (ac != null)
                                            {
                                                //ac.AddProfileItem(new Guid(ArticleSettings.ActiveSocialSubmitKey), objArticle.AuthorID, Common.GetArticleLink(objArticle, this.PortalSettings.ActiveTab, ArticleSettings, false), objArticle.Title, objArticle.Summary, objArticle.Body, 1, "")
                                            }
                                        }
                                        catch (Exception ex)
                                        { }
                                    }
                                }
                            }

                        }

                    }

                }

                Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "ApproveArticles", ArticleSettings), true);

            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdApproveAll_Click(object sender, EventArgs e)
        {

            try
            {

                ArticleController objArticleController = new ArticleController();

                for (int i = 0; i < grdArticles.Items.Count; i++)
                {

                    DataGridItem currentItem = grdArticles.Items[i];
                    ArticleInfo objArticle = objArticleController.GetArticle(Convert.ToInt32(grdArticles.DataKeys[i]));

                    objArticle.Status = StatusType.Published;
                    objArticleController.UpdateArticle(objArticle);

                    NotifyAuthor(objArticle);

                    if (ArticleSettings.EnableAutoTrackback)
                    {
                        GcDesign.NewsArticles.Tracking.Notification objNotifications = new Tracking.Notification();
                        objNotifications.NotifyExternalSites(objArticle, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false), this.PortalSettings.PortalName);
                    }

                    if (ArticleSettings.EnableNotificationPing)
                    {
                        GcDesign.NewsArticles.Tracking.Notification objNotifications = new Tracking.Notification();
                        objNotifications.NotifyWeblogs(Globals.AddHTTP(Globals.NavigateURL()), this.PortalSettings.PortalName);
                    }

                }

                Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "ApproveArticles", ArticleSettings), true);

            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}