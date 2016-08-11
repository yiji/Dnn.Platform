using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using GcDesign.NewsArticles.Components.Social;

using GcDesign.NewsArticles.Base;
using DotNetNuke.Entities.Users;
using System.IO;
using System.Collections;
using DotNetNuke.Services.Log.EventLog;

namespace GcDesign.NewsArticles
{
    public partial class ucApproveComments : NewsArticleModuleBase
    {
        #region " private Methods "

        private void BindComments()
        {

            CommentController objCommentController = new CommentController();
            rptApproveComments.DataSource = objCommentController.GetCommentList(this.ModuleId, Null.NullInteger, false, SortDirection.Ascending, Null.NullInteger);
            rptApproveComments.DataBind();

            if (rptApproveComments.Items.Count == 0)
            {
                rptApproveComments.Visible = false;
                lblNoComments.Visible = true;
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

        private void NotifyAuthor(CommentInfo objComment)
        {

            ArticleController objArticleController = new ArticleController();
            ArticleInfo objArticle = objArticleController.GetArticle(objComment.ArticleID);

            if (objArticle != null)
            {
                EmailTemplateController objEmailTemplateController = new EmailTemplateController();

                try
                {
                    // Don't send it to the author if it's their own comment.
                    if (objArticle.AuthorID != objComment.UserID)
                    {
                        objEmailTemplateController.SendFormattedEmail(this.ModuleId, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false), objArticle, objComment, EmailTemplateType.CommentNotification, ArticleSettings);
                    }
                }
                catch (Exception ex)
                {
                    DotNetNuke.Services.Log.EventLog.EventLogController objEventLog = new DotNetNuke.Services.Log.EventLog.EventLogController();

                    UserController objUserController = new DotNetNuke.Entities.Users.UserController();
                    UserInfo objUser = objUserController.GetUser(this.PortalId, objArticle.AuthorID);

                    string sendTo = "";
                    if (objUser != null)
                    {
                        sendTo = objUser.Membership.Email;
                    }
                    objEventLog.AddLog("News Articles Email Failure", "Failure to send [Author Comment] to '" + sendTo + "' from '" + this.PortalSettings.Email, PortalSettings, -1, DotNetNuke.Services.Log.EventLog.EventLogController.EventLogType.ADMIN_ALERT);
                }

            }

        }

        #endregion

        #region " protected Methods "

        protected string GetAuthor(object obj)
        {

            CommentInfo objComment = (CommentInfo)obj;
            if (objComment != null)
            {
                if (objComment.UserID != Null.NullInteger)
                {
                    return objComment.AuthorUserName;
                }
                else
                {
                    return objComment.AnonymousName;
                }
            }
            else
            {
                return "";
            }

        }

        protected string GetArticleUrl(object obj)
        {

            CommentInfo objComment = (CommentInfo)obj;
            if (objComment != null)
            {
                ArticleController objArticleController = new ArticleController();
                ArticleInfo objArticle = objArticleController.GetArticle(objComment.ArticleID);

                if (objArticle != null)
                {
                    return Common.GetArticleLink(objArticle, this.PortalSettings.ActiveTab, this.ArticleSettings, false);
                }
            }

            return "";

        }

        protected string GetEditCommentUrl(string commentID)
        {

            return Common.GetModuleLink(TabId, ModuleId, "EditComment", ArticleSettings, "CommentID=" + commentID, "ReturnUrl=" + Server.UrlEncode(Request.RawUrl));

        }

        protected string GetTitle(object obj)
        {

            CommentInfo objComment = (CommentInfo)obj;
            if (objComment != null)
            {

                ArticleController objArticleController = new ArticleController();
                ArticleInfo objArticle = objArticleController.GetArticle(objComment.ArticleID);

                if (objArticle != null)
                {
                    return objArticle.Title;
                }
            }

            return "";

        }

        protected string GetWebsite(object obj)
        {

            CommentInfo objComment = (CommentInfo)obj;
            if (objComment != null)
            {
                if (objComment.AnonymousURL != "")
                {
                    return "<a href='" + DotNetNuke.Common.Globals.AddHTTP(objComment.AnonymousURL) + "' target='_blank'>" + objComment.AnonymousURL + "</a>";
                }
            }
            return "";

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            rptApproveComments.ItemDataBound += rptApproveComments_OnItemDataBound;
            cmdApprove.Click+=cmdApprove_Click;
            cmdReject.Click+=cmdReject_Click;
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {

                CheckSecurity();

                if (!Page.IsPostBack)
                {
                    BindComments();
                }
            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void rptApproveComments_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                CommentInfo objComment = (CommentInfo)e.Item.DataItem;
                if (objComment != null)
                {
                    CheckBox chkSelected = (CheckBox)e.Item.FindControl("chkSelected");
                    chkSelected.Attributes.Add("CommentID", objComment.CommentID.ToString());
                }

            }

        }

        protected void cmdApprove_Click(object sender, EventArgs e)
        {

            foreach (RepeaterItem item in rptApproveComments.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    CheckBox chkSelected = (CheckBox)item.FindControl("chkSelected");
                    if (chkSelected != null)
                    {
                        if (chkSelected.Checked)
                        {
                            int commentID = Convert.ToInt32(chkSelected.Attributes["CommentID"].ToString());
                            CommentController objCommentController = new CommentController();
                            CommentInfo objComment = objCommentController.GetComment(commentID);
                            if (objComment != null)
                            {
                                objComment.IsApproved = true;
                                objComment.ApprovedBy = this.UserId;
                                objCommentController.UpdateComment(objComment);

                                EmailTemplateController objEmailTemplateController = new EmailTemplateController();
                                if (ArticleSettings.NotifyAuthorOnApproval)
                                {
                                    NotifyAuthor(objComment);
                                }

                                ArticleController objArticleController = new ArticleController();
                                ArticleInfo objArticle = objArticleController.GetArticle(objComment.ArticleID);

                                if (objArticle != null)
                                {

                                    if (ArticleSettings.EnableActiveSocialFeed && objComment.UserID != Null.NullInteger)
                                    {
                                        if (ArticleSettings.ActiveSocialCommentKey != "")
                                        {
                                            if (File.Exists(HttpContext.Current.Server.MapPath("~/bin/active.modules.social.dll")))
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
                                                        //ac.AddProfileItem(new Guid(ArticleSettings.ActiveSocialCommentKey), objComment.UserID, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false), objArticle.Title, objComment.Comment, objComment.Comment, 1, "");
                                                    }
                                                }
                                                catch (Exception ex)
                                                { }
                                            }
                                        }
                                    }

                                    if (Request.IsAuthenticated)
                                    {
                                        if (ArticleSettings.JournalIntegration)
                                        {
                                            Journal objJournal = new Journal();
                                            objJournal.AddCommentToJournal(objArticle, objComment, PortalId, TabId, UserId, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false));
                                        }
                                    }

                                    if (ArticleSettings.EnableSmartThinkerStoryFeed && objComment.UserID != Null.NullInteger)
                                    {
                                        //Dim objStoryFeed = new wsStoryFeed.StoryFeedWS
                                        //objStoryFeed.Url = DotNetNuke.Common.Globals.AddHTTP(Request.ServerVariables("HTTP_HOST") + this.ResolveUrl("~/DesktopModules/Smart-Thinker%20-%20UserProfile/StoryFeed.asmx"))

                                        string val = GetSharedResource("StoryFeed-AddComment");

                                        string delimStr = "[]";
                                        char[] delimiter = delimStr.ToCharArray();
                                        string[] layoutArray = val.Split(delimiter);

                                        string valResult = "";

                                        for (int iPtr = 0; iPtr < layoutArray.Length; iPtr += 2)
                                        {

                                            valResult = valResult + layoutArray[iPtr];

                                            if (iPtr < layoutArray.Length - 1)
                                            {
                                                switch (layoutArray[iPtr + 1])
                                                {

                                                    case "ARTICLEID":
                                                        valResult = valResult + objComment.ArticleID.ToString();
                                                        break;
                                                    case "AUTHORID":
                                                        valResult = valResult + objComment.UserID.ToString();
                                                        break;
                                                    case "AUTHOR":
                                                        if (objComment.UserID == Null.NullInteger)
                                                        {
                                                            valResult = valResult + objComment.AnonymousName;
                                                        }
                                                        else
                                                        {
                                                            valResult = valResult + objComment.AuthorDisplayName;
                                                        }
                                                        break;
                                                    case "ARTICLELINK":
                                                        valResult = valResult + Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false);
                                                        break;
                                                    case "ARTICLETITLE":
                                                        valResult = valResult + objArticle.Title;
                                                        break;
                                                    case "ISANONYMOUS":
                                                        if (objComment.UserID != Null.NullInteger)
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
                                                        // Do null
                                                        break;
                                                    case "ISNOTANONYMOUS":
                                                        if (objComment.UserID != Null.NullInteger)
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
                                                        // Do null
                                                        break;
                                                }
                                            }
                                        }

                                        try
                                        {
                                            //objStoryFeed.AddAction(81, objComment.CommentID, valResult, objComment.UserID, "VE6457624576460436531768");
                                        }
                                        catch
                                        { }

                                    }

                                    if (ArticleSettings.NotifyEmailOnComment != "")
                                    {
                                        foreach (string email in ArticleSettings.NotifyEmailOnComment.Split(';'))
                                        {
                                            if (email != "")
                                            {
                                                objEmailTemplateController.SendFormattedEmail(this.ModuleId, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false), objArticle, objComment, EmailTemplateType.CommentNotification, ArticleSettings, email);
                                            }
                                        }
                                    }

                                    if (ArticleSettings.NotifyAuthorOnApproval)
                                    {
                                        objEmailTemplateController.SendFormattedEmail(this.ModuleId, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false), objArticle, objComment, EmailTemplateType.CommentApproved, ArticleSettings);
                                    }

                                    Hashtable objMailList = new Hashtable();

                                    List<CommentInfo> objComments = objCommentController.GetCommentList(this.ModuleId, objComment.ArticleID, true, SortDirection.Ascending, Null.NullInteger);

                                    foreach (CommentInfo objNotifyComment in objComments)
                                    {
                                        if (objNotifyComment.CommentID != objComment.CommentID && objNotifyComment.NotifyMe)
                                        {
                                            if (objNotifyComment.UserID == Null.NullInteger)
                                            {
                                                if (objNotifyComment.AnonymousEmail != "")
                                                {
                                                    try
                                                    {
                                                        if (!objMailList.Contains(objNotifyComment.AnonymousEmail))
                                                        {
                                                            objEmailTemplateController.SendFormattedEmail(this.ModuleId, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false), objArticle, objComment, EmailTemplateType.CommentNotification, ArticleSettings, objNotifyComment.AnonymousEmail);
                                                            objMailList.Add(objNotifyComment.AnonymousEmail, objNotifyComment.AnonymousEmail);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        EventLogController objEventLog = new EventLogController();
                                                        objEventLog.AddLog("News Articles Email Failure", "Failure to send [Anon Comment] to '" + objNotifyComment.AnonymousEmail + "' from '" + this.PortalSettings.Email, PortalSettings, -1, DotNetNuke.Services.Log.EventLog.EventLogController.EventLogType.ADMIN_ALERT);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (objNotifyComment.AuthorEmail != "")
                                                {
                                                    try
                                                    {
                                                        if (objNotifyComment.UserID != objComment.UserID)
                                                        {
                                                            if (!objMailList.Contains(objNotifyComment.UserID.ToString()))
                                                            {
                                                                objEmailTemplateController.SendFormattedEmail(this.ModuleId, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false), objArticle, objComment, EmailTemplateType.CommentNotification, ArticleSettings, objNotifyComment.AuthorEmail);
                                                                objMailList.Add(objNotifyComment.UserID.ToString(), objNotifyComment.UserID.ToString());
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        EventLogController objEventLog = new EventLogController();
                                                        objEventLog.AddLog("News Articles Email Failure", "Failure to send [Author Comment] to '" + objNotifyComment.AuthorEmail + "' from '" + this.PortalSettings.Email, PortalSettings, -1, DotNetNuke.Services.Log.EventLog.EventLogController.EventLogType.ADMIN_ALERT);
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }

            Response.Redirect(Request.RawUrl, true);

        }

        protected void cmdReject_Click(object sender, EventArgs e)
        {

            foreach (RepeaterItem item in rptApproveComments.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    CheckBox chkSelected = (CheckBox)item.FindControl("chkSelected");
                    if (chkSelected != null)
                    {
                        if (chkSelected.Checked)
                        {
                            int commentID = Convert.ToInt32(chkSelected.Attributes["CommentID"].ToString());
                            CommentController objCommentController = new CommentController();
                            CommentInfo objComment = objCommentController.GetComment(commentID);
                            if (objComment != null)
                            {
                                objCommentController.DeleteComment(objComment.CommentID, objComment.ArticleID);
                            }
                        }
                    }
                }
            }

            Response.Redirect(Request.RawUrl, true);

        }

        #endregion
    }
}