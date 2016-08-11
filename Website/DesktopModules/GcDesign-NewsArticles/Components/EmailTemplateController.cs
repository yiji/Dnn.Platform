using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security.Roles;
using DotNetNuke.Entities.Modules;

namespace GcDesign.NewsArticles
{
    public class EmailTemplateController
    {
        #region " Private Methods "

        public string FormatArticleEmail(string template, string link, ArticleInfo objArticle, ArticleSettings articleSettings)
        {

            string formatted = template;

            formatted = formatted.Replace("[USERNAME]", objArticle.AuthorUserName);
            formatted = formatted.Replace("[FIRSTNAME]", objArticle.AuthorFirstName);
            formatted = formatted.Replace("[LASTNAME]", objArticle.AuthorLastName);
            formatted = formatted.Replace("[FULLNAME]", objArticle.AuthorFullName);
            formatted = formatted.Replace("[EMAIL]", objArticle.AuthorEmail);
            formatted = formatted.Replace("[DISPLAYNAME]", objArticle.AuthorDisplayName);

            PortalSettings settings = PortalController.Instance.GetCurrentPortalSettings();

            formatted = formatted.Replace("[PORTALNAME]", settings.PortalName);
            formatted = formatted.Replace("[CREATEDATE]", objArticle.CreatedDate.ToString("d") + " " + objArticle.CreatedDate.ToString("t"));
            formatted = formatted.Replace("[POSTDATE]", objArticle.StartDate.ToString("d") + " " + objArticle.CreatedDate.ToString("t"));

            formatted = formatted.Replace("[TITLE]", objArticle.Title);
            formatted = formatted.Replace("[SUMMARY]", System.Web.HttpContext.Current.Server.HtmlDecode(objArticle.Summary));
            formatted = formatted.Replace("[LINK]", link);

            return formatted;

        }

        /// <summary>
        /// 替换标签，返回内容字符串
        /// </summary>
        /// <param name="template"></param>
        /// <param name="link"></param>
        /// <param name="objArticle"></param>
        /// <param name="objComment"></param>
        /// <param name="articleSettings"></param>
        /// <returns></returns>
        public string FormatCommentEmail(string template, string link, ArticleInfo objArticle, CommentInfo objComment, ArticleSettings articleSettings)
        {

            string formatted = template;

            if (objComment.UserID == Null.NullInteger)
            {
                // Anonymous Comment匿名评论
                formatted = formatted.Replace("[USERNAME]", objComment.AnonymousName);
                formatted = formatted.Replace("[EMAIL]", objComment.AnonymousEmail);
                formatted = formatted.Replace("[FIRSTNAME]", objComment.AnonymousName);
                formatted = formatted.Replace("[LASTNAME]", objComment.AnonymousName);
                formatted = formatted.Replace("[FULLNAME]", objComment.AnonymousName);
                formatted = formatted.Replace("[DISPLAYNAME]", objComment.AnonymousName);
            }
            else
            {
                // Authenticated Comment
                formatted = formatted.Replace("[USERNAME]", objComment.AuthorUserName);
                formatted = formatted.Replace("[EMAIL]", objComment.AuthorEmail);
                formatted = formatted.Replace("[FIRSTNAME]", objComment.AuthorFirstName);
                formatted = formatted.Replace("[LASTNAME]", objComment.AuthorLastName);
                formatted = formatted.Replace("[FULLNAME]", objComment.AuthorFirstName + " " + objComment.AuthorLastName);
                formatted = formatted.Replace("[DISPLAYNAME]", objComment.AuthorDisplayName);
            }

            PortalSettings settings = PortalController.Instance.GetCurrentPortalSettings();

            formatted = formatted.Replace("[PORTALNAME]", settings.PortalName);
            formatted = formatted.Replace("[POSTDATE]", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());

            formatted = formatted.Replace("[TITLE]", objArticle.Title);
            formatted = formatted.Replace("[COMMENT]", objComment.Comment.Replace("<br />", "\r\n"));
            formatted = formatted.Replace("[LINK]", link);
            formatted = formatted.Replace("[APPROVELINK]", Common.GetModuleLink(settings.ActiveTab.TabID, objArticle.ModuleID, "ApproveComments", articleSettings));

            return formatted;

        }

        public string GetApproverDistributionList(int moduleID)
        {

            PortalSettings settings = PortalController.Instance.GetCurrentPortalSettings();
            ModuleController objModuleController = new ModuleController();
            Hashtable moduleSettings = objModuleController.GetModule(moduleID).ModuleSettings;
            string distributionList = "";

            if (moduleSettings.Contains(ArticleConstants.PERMISSION_APPROVAL_SETTING))
            {

                string roles = moduleSettings[ArticleConstants.PERMISSION_APPROVAL_SETTING].ToString();
                string[] rolesArray = roles.Split(';');
                Hashtable userList = new Hashtable();

                foreach (string role in rolesArray)
                {
                    if (role.Length > 0)
                    {
                        RoleInfo objRole = RoleController.Instance.GetRoleByName(settings.PortalId, role);

                        if (objRole != null)
                        {
                            IList<UserInfo> objUsers =RoleController.Instance.GetUsersByRole(settings.PortalId, objRole.RoleName);
                            foreach (UserInfo objUser in objUsers)
                            {
                                if (!userList.Contains(objUser.UserID))
                                {
                                    UserController objUserController = new UserController();
                                    UserInfo objSelectedUser = objUserController.GetUser(settings.PortalId, objUser.UserID);
                                    if (objSelectedUser != null)
                                    {
                                        if (objSelectedUser.Email.Length > 0)
                                        {
                                            userList.Add(objUser.UserID, objSelectedUser.Email);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (DictionaryEntry email in userList)
                {
                    if (distributionList.Length > 0)
                    {
                        distributionList += "; ";
                    }
                    distributionList += email.Value.ToString();
                }

            }

            return distributionList;

        }


        #endregion

        #region " public Methods "

        public EmailTemplateInfo Get(int templateID)
        {

            return (EmailTemplateInfo)CBO.FillObject<EmailTemplateInfo>(DataProvider.Instance().GetEmailTemplate(templateID));

        }

        /// <summary>
        /// 获取邮件模板
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="type">邮件类型ArticleApproved，ArticleSubmission等</param>
        /// <returns></returns>
        public EmailTemplateInfo Get(int moduleID, EmailTemplateType type)
        {
            //获取邮件模板
            EmailTemplateInfo objEmailTemplate = CBO.FillObject <EmailTemplateInfo>(DataProvider.Instance().GetEmailTemplateByName(moduleID, type.ToString()));

            if (objEmailTemplate == null)
            {

                objEmailTemplate = new EmailTemplateInfo();
                objEmailTemplate.ModuleID = moduleID;
                objEmailTemplate.Name = type.ToString();

                switch (type)
                {

                    case EmailTemplateType.ArticleApproved:
                        objEmailTemplate.Subject = "[PORTALNAME]: 你的文章已审核通过";
                        objEmailTemplate.Template = ""
                                + "你发表的文章 [TITLE] 已通过审核." + "\r\n" + "\r\n"
                                + "参观文章，请访问：" + "\r\n"
                                + "[LINK]" + "\r\n" + "\r\n"
                                + "谢谢," + "\r\n"
                                + "[PORTALNAME]";
                        break;

                    case EmailTemplateType.ArticleSubmission:
                        objEmailTemplate.Subject = "[PORTALNAME]: 文章请求审核";
                        objEmailTemplate.Template = ""
                                + " [POSTDATE] 文章 [TITLE] 已提交等待审核." + "\r\n" + "\r\n"
                                + "[SUMMARY]" + "\r\n" + "\r\n"
                                + "要查看完整的文章和批准，请访问：" + "\r\n"
                                + "[LINK]" + "\r\n" + "\r\n"
                                + "谢谢," + "\r\n"
                                + "[PORTALNAME]";
                        break;

                    case EmailTemplateType.ArticleUpdateMirrored:
                        objEmailTemplate.Subject = "[PORTALNAME]: 文章更新";
                        objEmailTemplate.Template = ""
                                + " [POSTDATE] 文章 '[TITLE]' 已更新." + "\r\n" + "\r\n"
                                + "要查看文章，请访问：" + "\r\n"
                                + "[LINK]" + "\r\n" + "\r\n"
                                + "谢谢," + "\r\n"
                                + "[PORTALNAME]";
                        break;

                    case EmailTemplateType.CommentNotification:
                        objEmailTemplate.Subject = "[PORTALNAME]: 评论通知";
                        objEmailTemplate.Template = ""
                                + " [POSTDATE]  [TITLE] 有条新评论." + "\r\n" + "\r\n"
                                + "[COMMENT]" + "\r\n" + "\r\n"
                                + "To view the complete article and reply, please visit:" + "\r\n"
                                + "[LINK]" + "\r\n" + "\r\n"
                                + "谢谢," + "\r\n"
                                + "[PORTALNAME]";
                        break;

                    case EmailTemplateType.CommentRequiringApproval:
                        objEmailTemplate.Subject = "[PORTALNAME]: 评论等待审核";
                        objEmailTemplate.Template = ""
                                + "[POSTDATE], [TITLE] 有条评论等待你审核." + "\r\n" + "\r\n"
                                + "[COMMENT]" + "\r\n" + "\r\n"
                                + "审核评论请访问:" + "\r\n"
                                + "[APPROVELINK]" + "\r\n" + "\r\n"
                                + "谢谢," + "\r\n"
                                + "[PORTALNAME]";
                        break;

                    case EmailTemplateType.CommentApproved:
                        objEmailTemplate.Subject = "[PORTALNAME]: 你的评论已通过审核";
                        objEmailTemplate.Template = ""
                                + "你发表的关于 [TITLE] 的评论已通过审核." + "\r\n" + "\r\n"
                                + "查看文章请访问:" + "\r\n"
                                + "[LINK]" + "\r\n" + "\r\n"
                                + "谢谢," + "\r\n"
                                + "[PORTALNAME]";
                        break;

                }

                objEmailTemplate.TemplateID = Add(objEmailTemplate);

            }

            return objEmailTemplate;

        }

        public ArrayList List(int moduleID)
        {

            return CBO.FillCollection(DataProvider.Instance().ListEmailTemplate(moduleID), typeof(EmailTemplateInfo));

        }

        public int Add(EmailTemplateInfo objEmailTemplate)
        {

            return (int)DataProvider.Instance().AddEmailTemplate(objEmailTemplate.ModuleID, objEmailTemplate.Name, objEmailTemplate.Subject, objEmailTemplate.Template);

        }

        public void Update(EmailTemplateInfo objEmailTemplate)
        {

            DataProvider.Instance().UpdateEmailTemplate(objEmailTemplate.TemplateID, objEmailTemplate.ModuleID, objEmailTemplate.Name, objEmailTemplate.Subject, objEmailTemplate.Template);

        }

        public void Delete(int templateID)
        {

            DataProvider.Instance().DeleteEmailTemplate(templateID);

        }

        public void SendFormattedEmail(int moduleID, string link, ArticleInfo objArticle, EmailTemplateType type, string sendTo, ArticleSettings articleSettings)
        {

            PortalSettings settings = PortalController.Instance.GetCurrentPortalSettings();

            string subject = "";
            string template = "";

            EmailTemplateInfo objTemplate;
            switch (type)
            {

                case EmailTemplateType.ArticleSubmission:
                    objTemplate = Get(moduleID, EmailTemplateType.ArticleSubmission);
                    subject = objTemplate.Subject;
                    template = objTemplate.Template;

                    break;

                case EmailTemplateType.ArticleApproved:
                    objTemplate = Get(moduleID, EmailTemplateType.ArticleApproved);
                    subject = objTemplate.Subject;
                    template = objTemplate.Template;

                    break;

                case EmailTemplateType.ArticleUpdateMirrored:
                    objTemplate = Get(moduleID, EmailTemplateType.ArticleUpdateMirrored);
                    subject = objTemplate.Subject;
                    template = objTemplate.Template;

                    break;

                default:
                    return;

            }

            subject = FormatArticleEmail(subject, link, objArticle, articleSettings);
            template = FormatArticleEmail(template, link, objArticle, articleSettings);

            // SendNotification(settings.Email, sendTo, Null.NullString, subject, template)
            try
            {
                DotNetNuke.Services.Mail.Mail.SendMail(settings.Email, sendTo, "", subject, template, "", "", "", "", "", "");
            }
            catch
            { }

        }

        public void SendFormattedEmail(int moduleID, string link, ArticleInfo objArticle, CommentInfo objComment, EmailTemplateType type, ArticleSettings articleSettings)
        {

            PortalSettings settings = PortalController.Instance.GetCurrentPortalSettings();

            string sendTo = "";
            UserController objUserController;
            UserInfo objUser;

            switch (type)
            {

                case EmailTemplateType.CommentNotification:

                    objUserController = new UserController();
                    objUser = objUserController.GetUser(settings.PortalId, objArticle.AuthorID);

                    if (objUser != null)
                    {
                        sendTo = objUser.Email;
                        SendFormattedEmail(moduleID, link, objArticle, objComment, EmailTemplateType.CommentNotification, articleSettings, sendTo);
                    }

                    break;

                case EmailTemplateType.CommentApproved:

                    if (objComment.UserID != Null.NullInteger)
                    {
                        objUserController = new UserController();
                        objUser = objUserController.GetUser(settings.PortalId, objComment.UserID);

                        if (objUser != null)
                        {
                            sendTo = objUser.Email;
                            SendFormattedEmail(moduleID, link, objArticle, objComment, EmailTemplateType.CommentApproved, articleSettings, sendTo);
                        }
                    }
                    else
                    {
                        SendFormattedEmail(moduleID, link, objArticle, objComment, EmailTemplateType.CommentApproved, articleSettings, objComment.AnonymousEmail);
                    }

                    break;

                case EmailTemplateType.CommentRequiringApproval:

                    objUserController = new UserController();
                    objUser = objUserController.GetUser(settings.PortalId, objArticle.AuthorID);

                    if (objUser != null)
                    {
                        sendTo = objUser.Email;
                        SendFormattedEmail(moduleID, link, objArticle, objComment, EmailTemplateType.CommentRequiringApproval, articleSettings, sendTo);
                    }

                    break;

                default:
                    return;

            }

        }

        /// <summary>
        /// 获取邮件模板，组织邮件内容并发送
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="link"></param>
        /// <param name="objArticle"></param>
        /// <param name="objComment"></param>
        /// <param name="type"></param>
        /// <param name="articleSettings"></param>
        /// <param name="email"></param>
        public void SendFormattedEmail(int moduleID, string link, ArticleInfo objArticle, CommentInfo objComment, EmailTemplateType type, ArticleSettings articleSettings, string email)
        {

            PortalSettings settings = PortalController.Instance.GetCurrentPortalSettings();

            string subject = "";
            string template = "";
            string sendTo = email;
            EmailTemplateInfo objTemplate;

            switch (type)
            {

                case EmailTemplateType.CommentNotification:
                    objTemplate = Get(moduleID, EmailTemplateType.CommentNotification);
                    subject = objTemplate.Subject;
                    template = objTemplate.Template;

                    break;

                case EmailTemplateType.CommentApproved:
                    objTemplate = Get(moduleID, EmailTemplateType.CommentApproved);
                    subject = objTemplate.Subject;
                    template = objTemplate.Template;

                    break;

                case EmailTemplateType.CommentRequiringApproval:
                    objTemplate = Get(moduleID, EmailTemplateType.CommentRequiringApproval);
                    subject = objTemplate.Subject;
                    template = objTemplate.Template;

                    break;

                default:
                    return;

            }
            //subject主题
            subject = FormatCommentEmail(subject, link, objArticle, objComment, articleSettings);
            //内容
            template = FormatCommentEmail(template, link, objArticle, objComment, articleSettings);

            try
            {
                //发送邮件
                DotNetNuke.Services.Mail.Mail.SendMail(settings.Email, sendTo, "", subject, template, "", "", "", "", "", "");
            }
            catch
            {

            }

        }

        #endregion
    }
}