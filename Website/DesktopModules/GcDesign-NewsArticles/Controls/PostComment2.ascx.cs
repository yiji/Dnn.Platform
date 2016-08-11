using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using Joel.Net;
using GcDesign.NewsArticles.Components.Social;
using GcDesign.NewsArticles.Base;
using System.Collections;
using DotNetNuke.Common;

namespace GcDesign.NewsArticles.Controls
{
    public partial class PostComment2 : NewsArticleControlBase
    {
        #region " private Properties "

        private NewsArticleModuleBase ArticleModuleBase
        {
            get
            {
                if (Parent.Parent.GetType().BaseType.BaseType == typeof(NewsArticleModuleBase))
                {
                    return (NewsArticleModuleBase)Parent.Parent;
                }
                else
                {
                    return (NewsArticleModuleBase)Parent.Parent.Parent.Parent;
                }
            }
        }

        private ArticleSettings ArticleSettings
        {
            get
            {
                return ArticleModuleBase.ArticleSettings;
            }
        }

        #endregion

        #region " private Methods "

        /// <summary>
        /// 本地化
        /// </summary>
        private void AssignLocalization()
        {


            valName.ErrorMessage = GetResourceKey("valName.ErrorMessage");

            if (!ArticleSettings.CommentRequireName)
            {
                valName.Enabled = false;
            }

            lblEmail.Text = GetResourceKey("Email");
            valEmail.ErrorMessage = GetResourceKey("valEmail.ErrorMessage");
            valEmailIsValid.ErrorMessage = GetResourceKey("valEmailIsValid.ErrorMessage");

            if (!ArticleSettings.CommentRequireEmail)
            {
                valEmail.Enabled = false;
                valEmailIsValid.Enabled = false;
                lblEmail.Text = GetResourceKey("EmailNotRequired");
            }

            lblUrl.Text = GetResourceKey("Website");

            ctlCaptcha.Text = GetResourceKey("ctlCaptcha.Text");
            ctlCaptcha.ErrorMessage = GetResourceKey("ctlCaptcha.ErrorMessage");

            btnAddComment.Text = GetResourceKey("AddComment");
            lblRequiresApproval.Text = GetResourceKey("RequiresApproval");

            lblRequiresAccess.Text = GetResourceKey("RequiresAccess");

        }

        /// <summary>
        /// 检查权限 不允许评论和未登录时评论表单不出现
        /// </summary>
        private void CheckSecurity()
        {

            if (!ArticleSettings.IsCommentsAnonymous && !Request.IsAuthenticated)
            {
                phCommentForm.Visible = false;
                phCommentAnonymous.Visible = true;
            }

        }

        private string FilterInput(string stringToFilter)
        {

            PortalSecurity objPortalSecurity = new PortalSecurity();

            stringToFilter = objPortalSecurity.InputFilter(stringToFilter, PortalSecurity.FilterFlag.NoScripting);

            stringToFilter = stringToFilter.Replace(Convert.ToChar(13).ToString(), "");
            stringToFilter = stringToFilter.Replace("\r\n", "<br />");

            return stringToFilter;

        }

        /// <summary>
        /// 获取Cookie 当未验证时获取写入到Cookie中的name，email，url
        /// </summary>
        private void GetCookie()
        {

            if (!Request.IsAuthenticated)
            {
                HttpCookie cookie = Request.Cookies["comment"];

                if (cookie != null)
                {
                    txtEmail.Text = cookie.Values["email"];
                    txtURL.Text = cookie.Values["url"];
                }
            }


        }

        public string GetResourceKey(string key)
        {

            string path = @"~/DesktopModules/GcDesign-NewsArticles/" + Localization.LocalResourceDirectory + @"/PostComment2.ascx.zh-CN.resx";
            return DotNetNuke.Services.Localization.Localization.GetString(key, path);

        }

        private void NotifyAuthor(CommentInfo objComment, ArticleInfo objArticle)
        {

            EmailTemplateController objEmailTemplateController = new EmailTemplateController();
            objEmailTemplateController.SendFormattedEmail(ArticleModuleBase.ModuleId, Common.GetArticleLink(objArticle, ArticleModuleBase.PortalSettings.ActiveTab, ArticleSettings, false), objArticle, objComment, EmailTemplateType.CommentNotification, ArticleSettings);

        }

        /// <summary>
        /// 通知别的所有留过邮箱的评论者们有新的评论
        /// </summary>
        /// <param name="objComment"></param>
        /// <param name="objArticle"></param>
        private void NotifyComments(CommentInfo objComment, ArticleInfo objArticle)
        {

            EmailTemplateController objEmailTemplateController = new EmailTemplateController();
            Hashtable objMailList = new Hashtable();

            CommentController objCommentController = new CommentController();
            List<CommentInfo> objComments = objCommentController.GetCommentList(ArticleModuleBase.ModuleId, ArticleID, true, SortDirection.Ascending, Null.NullInteger);

            foreach (CommentInfo objNotifyComment in objComments)
            {
                //已存在的评论且勾选通知我
                if (objNotifyComment.CommentID != objComment.CommentID && objNotifyComment.NotifyMe)
                {
                    //先前的评论者是匿名用户
                    if (objNotifyComment.UserID == Null.NullInteger)
                    {
                        //匿名时邮箱不为空
                        if (objNotifyComment.AnonymousEmail != "")
                        {
                            //匿名邮箱与当前评论邮箱地址不一样
                            if (objNotifyComment.AnonymousEmail != objComment.AnonymousEmail)
                            {
                                //objMailList不包含该匿名邮箱
                                if (!objMailList.Contains(objNotifyComment.AnonymousEmail))
                                {
                                    objEmailTemplateController.SendFormattedEmail(ArticleModuleBase.ModuleId, Common.GetArticleLink(objArticle, ArticleModuleBase.PortalSettings.ActiveTab, ArticleSettings, false), objArticle, objComment, EmailTemplateType.CommentNotification, ArticleSettings, objNotifyComment.AnonymousEmail);
                                    objMailList.Add(objNotifyComment.AnonymousEmail, objNotifyComment.AnonymousEmail);
                                }
                            }
                        }
                    }
                    else//先前评论者是登陆用户
                    {
                        //评论者邮箱不为空
                        if (objNotifyComment.AuthorEmail != "")
                        {
                            //先前评论者和当前评论者不是同一人
                            if (objNotifyComment.UserID != objComment.UserID)
                            {
                                if (!objMailList.Contains(objNotifyComment.AuthorEmail.ToString()))
                                {
                                    objEmailTemplateController.SendFormattedEmail(ArticleModuleBase.ModuleId, Common.GetArticleLink(objArticle, ArticleModuleBase.PortalSettings.ActiveTab, ArticleSettings, false), objArticle, objComment, EmailTemplateType.CommentNotification, ArticleSettings, objNotifyComment.AuthorEmail);
                                    objMailList.Add(objNotifyComment.AuthorEmail, objNotifyComment.AuthorEmail);
                                }
                            }
                        }
                    }

                }
            }

        }

        /// <summary>
        /// 写入Cookie 当未验证时将name，email，url写入到Cookie中
        /// </summary>
        private void SetCookie()
        {

            if (!Request.IsAuthenticated)
            {
                HttpCookie objCookie = new HttpCookie("comment");

                objCookie.Expires = DateTime.Now.AddMonths(24);
                objCookie.Values.Add("email", txtEmail.Text);
                objCookie.Values.Add("url", txtURL.Text);

                Response.Cookies.Add(objCookie);
            }

        }

        /// <summary>
        /// 设置可见性
        /// </summary>
        private void SetVisibility()
        {
            pEmail.Visible = Request.IsAuthenticated;
            pUrl.Visible = !Request.IsAuthenticated;
            ctlCaptcha.Visible = (ArticleSettings.UseCaptcha && Request.IsAuthenticated);

            //if (!Request.IsAuthenticated)
            //{
            //    pUrl.Visible = !ArticleSettings.IsCommentWebsiteHidden;
            //}

            phCommentAnonymous.Visible = !Request.IsAuthenticated;
            phCommentForm.Visible = Request.IsAuthenticated;

            if (!Request.IsAuthenticated)
            {
                LoginPopup();
            }
        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Init += new EventHandler(this.Page_Init);
            this.PreRender += new EventHandler(Page_PreRender);
            btnAddComment.Click += btnAddComment_Click;
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Init(object sender, EventArgs e)
        {

            if (Parent.Parent.GetType().BaseType.BaseType != typeof(NewsArticleModuleBase) && Parent.Parent.Parent.Parent.GetType().BaseType.BaseType != typeof(NewsArticleModuleBase))
            {
                Visible = false;
                return;
            }

            CheckSecurity();
            AssignLocalization();
            SetVisibility();

            valName.ValidationGroup = "PostComment-" + ArticleID.ToString();
            valEmail.ValidationGroup = "PostComment-" + ArticleID.ToString();
            valEmailIsValid.ValidationGroup = "PostComment-" + ArticleID.ToString();
            valphone.ValidationGroup = "PostComment-" + ArticleID.ToString();
            btnAddComment.ValidationGroup = "PostComment-" + ArticleID.ToString();

            if (!Page.IsPostBack)
            {
                GetCookie();
                //赋予课程名 文章标题 蒋国祥 2014-11-20
                ArticleController articleController = new ArticleController();
                txtCourse.Text = articleController.GetArticle(ArticleID).Title;
                txtCourse.Enabled = false;
                //end
            }

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            if (HttpContext.Current.Items.Contains("IgnoreCaptcha"))
            {
                ctlCaptcha.ErrorMessage = "";
            }

        }

        protected void btnAddComment_Click(object sender, EventArgs e)
        {

            if (Page.IsValid)
            {

                if (ctlCaptcha.Visible && !ctlCaptcha.IsValid)
                {
                    txtComment.Focus();
                    return;
                }

                ArticleController objController = new ArticleController();
                ArticleInfo objArticle = objController.GetArticle(ArticleID);

                //获取本篇文章评论
                CommentController objCommentController = new CommentController();
                List<CommentInfo> objComments = objCommentController.GetCommentList(ArticleModuleBase.ModuleId, ArticleID, true, SortDirection.Ascending, Null.NullInteger);

                //遍历每条评论
                foreach (CommentInfo objArticleComment in objComments)
                {
                    //如果评论是一分钟之内发的会判断是否有重复评论，
                    //早于一分钟前发的评论不去对比会生成新的评论
                    if (objArticleComment.CreatedDate > DateTime.Now.AddMinutes(-1))
                    {
                        int id = Null.NullInteger;
                        if (Request.IsAuthenticated)
                        {
                            id = ArticleModuleBase.UserId;
                        }
                        //如果评论内容和当前文本框里一样且评论者与当前用户一致，直接跳转到该条评论
                        if (objArticleComment.Comment == FilterInput(txtComment.Text) && objArticleComment.UserID == id)
                        {

                            if (Request["articleType"] != "ArticleView")
                            {
                                Response.Redirect(Request.RawUrl + "#Comment" + objArticleComment.CommentID.ToString(), true);
                            }
                            else
                            {
                                Response.Redirect(Common.GetArticleLink(objArticle, ArticleModuleBase.PortalSettings.ActiveTab, ArticleSettings, false) + "#Comment" + objArticleComment.CommentID.ToString(), true);
                            }
                            return;
                        }
                    }
                }

                //生成新的评论
                CommentInfo objComment = new CommentInfo();
                objComment.ArticleID = ArticleID;
                objComment.CreatedDate = DateTime.Now;
                if (Request.IsAuthenticated)
                {
                    objComment.UserID = ArticleModuleBase.UserId;
                }
                else
                {
                    //允许匿名评论时
                    objComment.UserID = Null.NullInteger;
                    objComment.AnonymousName = txtCustomerName.Text;
                    objComment.AnonymousEmail = txtEmail.Text;
                    objComment.AnonymousURL = txtURL.Text;
                    SetCookie();
                }


                //评论内容 邮件发送

                //获取文章所在的栏目
                ArrayList categories = objController.GetArticleCategories(ArticleID);
                string strCategories = "";
                foreach (object obj in categories)
                {
                    strCategories += ((CategoryInfo)obj).Name + ",";
                }
                //objComment.Comment = FilterInput(txtComment.Text)
                string emailContent = "课程名:" + txtCourse.Text + "\r\n" +
                                        "<br />时间:" + objArticle.StartDate.ToString("yyyy-MM-dd") + "--" + objArticle.EndDate.AddDays(-1).ToString("yyyy-MM-dd") + "\r\n" +
                                        "<br />课程分类:" + strCategories.Substring(0, strCategories.LastIndexOf(',')) + "\r\n" +
                                        "<br />姓    名:" + txtCustomerName.Text + "\r\n" +
                                        "<br />联系电话:" + txtPhone.Text + "\r\n" +
                                        "<br />QQ:" + txtQQ.Text + "\r\n" +
                                        "<br />Email:" + txtEmail.Text + "\r\n" +
                                        "<br />备 注:" + txtComment.Text;
                objComment.Comment = FilterInput(emailContent);
                objComment.RemoteAddress = Request.UserHostAddress;
                objComment.NotifyMe = false;
                objComment.Type = 0;

                //模块设置允许评论且自动通过审核
                if (ArticleSettings.IsApprover || ArticleSettings.IsAutoApproverComment)
                {
                    objComment.IsApproved = true;
                    objComment.ApprovedBy = ArticleModuleBase.UserId;
                }
                else
                {
                    //评论需要审核
                    if (ArticleSettings.CommentModeration)
                    {
                        objComment.IsApproved = false;
                        objComment.ApprovedBy = Null.NullInteger;
                    }
                    else
                    {
                        objComment.IsApproved = true;
                        objComment.ApprovedBy = Null.NullInteger;
                    }
                }

                //Akismet 防止垃圾邮件
                //if (ArticleSettings.CommentAkismetKey != "")
                //{
                //    Akismet api = new Akismet(ArticleSettings.CommentAkismetKey, DotNetNuke.Common.Globals.NavigateURL(((DotNetNuke.Entities.Modules.PortalModuleBase)Parent.Parent).TabId), "Test/1.0");
                //    if (api.VerifyKey())
                //    {
                //        AkismetComment comment = new AkismetComment();

                //        comment.Blog = DotNetNuke.Common.Globals.NavigateURL(((DotNetNuke.Entities.Modules.PortalModuleBase)Parent.Parent).TabId);
                //        comment.UserIp = objComment.RemoteAddress;
                //        comment.UserAgent = Request.UserAgent;
                //        comment.CommentContent = objComment.Comment;
                //        comment.CommentType = "comment";

                //        if (Request.IsAuthenticated)
                //        {
                //            comment.CommentAuthor = ((DotNetNuke.Entities.Modules.PortalModuleBase)Parent.Parent).UserInfo.DisplayName;
                //            comment.CommentAuthorEmail = ((DotNetNuke.Entities.Modules.PortalModuleBase)Parent.Parent).UserInfo.Email;
                //            comment.CommentAuthorUrl = "";
                //        }
                //        else
                //        {
                //            comment.CommentAuthor = objComment.AnonymousName;
                //            comment.CommentAuthorEmail = objComment.AnonymousEmail;
                //            comment.CommentAuthorUrl = objComment.AnonymousURL;
                //        }

                //        if (api.CommentCheck(comment))
                //        {
                //            txtComment.Focus();
                //            return;
                //        }
                //    }
                //}

                //向数据库中添加评论
                objComment.CommentID = objCommentController.AddComment(objComment);

                //重获刚插入的评论
                objComment = objCommentController.GetComment(objComment.CommentID);

                if (objArticle != null)
                {
                    //jiang
                    EmailTemplateController objEmailTemplateController = new EmailTemplateController();
                    //jiang 通知评论者以成功报名
                    if (txtEmail.Text != "")
                    {
                        objEmailTemplateController.SendFormattedEmail(ArticleModuleBase.ModuleId, Common.GetArticleLink(objArticle, ArticleModuleBase.PortalSettings.ActiveTab, ArticleSettings, false), objArticle, objComment, EmailTemplateType.CommentNotification, ArticleSettings, txtEmail.Text);
                    }


                    // 通知 已审核通过
                    if (objComment.IsApproved)
                    {
                        //设置了有评论时通知作者
                        if (ArticleSettings.NotifyAuthorOnComment)
                        {
                            NotifyAuthor(objComment, objArticle);
                        }
                        //NotifyComments(objComment, objArticle);

                        //模块设置了有评论时发通知的邮箱（以‘;’分号分隔）
                        if (ArticleSettings.NotifyEmailOnComment != "")
                        {
                            //EmailTemplateController objEmailTemplateController = new EmailTemplateController();
                            foreach (string email in ArticleSettings.NotifyEmailOnComment.Split(';'))
                            {
                                if (email != "")
                                {
                                    objEmailTemplateController.SendFormattedEmail(ArticleModuleBase.ModuleId, Common.GetArticleLink(objArticle, ArticleModuleBase.PortalSettings.ActiveTab, ArticleSettings, false), objArticle, objComment, EmailTemplateType.CommentNotification, ArticleSettings, email);
                                }
                            }
                        }

                        //if (ArticleSettings.EnableActiveSocialFeed && Request.IsAuthenticated)
                        //{
                        //    if (ArticleSettings.ActiveSocialCommentKey != "")
                        //    {
                        //        if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/bin/active.modules.social.dll")))
                        //        {
                        //            object ai = null;
                        //            System.Reflection.Assembly asm;
                        //            object ac = null;
                        //            try
                        //            {
                        //                asm = System.Reflection.Assembly.Load("Active.Modules.Social");
                        //                ac = asm.CreateInstance("Active.Modules.Social.API.Journal");
                        //                if (ac != null)
                        //                {
                        //                    //ac.AddProfileItem(new Guid(ArticleSettings.ActiveSocialCommentKey), objComment.UserID, Common.GetArticleLink(objArticle, ArticleModuleBase.PortalSettings.ActiveTab, ArticleSettings, false), objArticle.Title, objComment.Comment, objComment.Comment, 1, "");
                        //                }
                        //            }
                        //            catch (Exception ex)
                        //            {

                        //            }
                        //        }
                        //    }
                        //}

                        //添加到日志中
                        if (Request.IsAuthenticated)
                        {
                            if (ArticleSettings.JournalIntegration)
                            {
                                Journal objJournal = new Journal();
                                objJournal.AddCommentToJournal(objArticle, objComment, ArticleModuleBase.PortalId, ArticleModuleBase.TabId, ArticleModuleBase.UserId, Common.GetArticleLink(objArticle, ArticleModuleBase.PortalSettings.ActiveTab, ArticleSettings, false));
                            }
                        }

                    }
                    else//未审核
                    {
                        //设置了通知审核员
                        if (ArticleSettings.NotifyApproverForCommentApproval)
                        {

                            // 通知审核员
                            //EmailTemplateController objEmailTemplateController = new EmailTemplateController();

                            //通知审核角色中的人
                            string emails = objEmailTemplateController.GetApproverDistributionList(ArticleModuleBase.ModuleId);

                            foreach (string email in emails.Split(';'))
                            {
                                if (email != "")
                                {
                                    try
                                    {
                                        //通知审核员
                                        objEmailTemplateController.SendFormattedEmail(ArticleModuleBase.ModuleId, Common.GetArticleLink(objArticle, ArticleModuleBase.PortalSettings.ActiveTab, ArticleSettings, false), objArticle, objComment, EmailTemplateType.CommentRequiringApproval, ArticleSettings, email);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }

                        }
                        //通知模块设置中的人
                        if (ArticleSettings.NotifyEmailForCommentApproval != "")
                        {

                            //EmailTemplateController objEmailTemplateController = new EmailTemplateController();

                            foreach (string email in ArticleSettings.NotifyEmailForCommentApproval.Split(';'))
                            {
                                if (email != "")
                                {
                                    try
                                    {
                                        objEmailTemplateController.SendFormattedEmail(ArticleModuleBase.ModuleId, Common.GetArticleLink(objArticle, ArticleModuleBase.PortalSettings.ActiveTab, ArticleSettings, false), objArticle, objComment, EmailTemplateType.CommentRequiringApproval, ArticleSettings, email);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }

                        }

                    }

                    // 重定向
                    if (objComment.IsApproved)
                    {
                        //审核通过
                        if (Request["articleType"] != "ArticleView")
                        {
                            Response.Redirect(Request.RawUrl + "#Comment" + objComment.CommentID.ToString(), true);
                        }
                        else
                        {
                            Response.Redirect(Common.GetArticleLink(objArticle, ArticleModuleBase.PortalSettings.ActiveTab, ArticleSettings, false) + "#Comment" + objComment.CommentID.ToString(), true);
                        }
                    }
                    else
                    {
                        //审核未通过显示待审核提示
                        phCommentForm.Visible = false;
                        phCommentPosted.Visible = true;
                    }

                }
                else
                {

                    // Should never be here.
                    Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);

                }

            }

        }

        private void LoginPopup()
        {
            string returnUrl = HttpContext.Current.Request.RawUrl;
            if (returnUrl.IndexOf("?returnurl=") != -1)
            {
                returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("?returnurl="));
            }
            returnUrl = HttpUtility.UrlEncode(returnUrl);

            loginLink.NavigateUrl = Globals.LoginURL(returnUrl, (Request.QueryString["override"] != null));

            //avoid issues caused by multiple clicks of login link
            var oneclick = "this.disabled=true;";
            loginLink.Attributes.Add("onclick", oneclick);


            if (PortalSettings.EnablePopUps && PortalSettings.LoginTabId == Null.NullInteger)
            {
                //To avoid duplicated encodes of URL
                var clickEvent = "return " + UrlUtils.PopUpUrl(HttpUtility.UrlDecode(loginLink.NavigateUrl), this, PortalSettings, true, false, 300, 650);
                loginLink.Attributes.Add("onclick", clickEvent);
            }
        }

        #endregion
    }
}