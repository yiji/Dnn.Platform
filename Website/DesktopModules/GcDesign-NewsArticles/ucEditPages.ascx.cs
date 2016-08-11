using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;

using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class ucEditPages : NewsArticleModuleBase
    {

        #region " private Members "

        private int _articleID;

        #endregion

        #region " private Methods "

        private void ReadQueryString()
        {

            if (Numeric.IsNumeric(Request["ArticleID"]))
            {
                _articleID = Convert.ToInt32(Request["ArticleID"]);
            }


        }

        private void CheckSecurity()
        {

            if (!HasEditRights(_articleID, this.ModuleId, this.TabId))
            {
                Response.Redirect(Common.GetModuleLink(this.TabId, this.ModuleId, "NotAuthorized", ArticleSettings), true);
            }

        }

        private void BindArticle()
        {

            if (_articleID == Null.NullInteger)
            {
                Response.Redirect(Common.GetModuleLink(this.TabId, this.ModuleId, "NotAuthorized", ArticleSettings), true);
            }

            ArticleController objArticleController = new ArticleController();

            ArticleInfo objArticle = objArticleController.GetArticle(_articleID);

            if (objArticle == null)
            {
                Response.Redirect(Common.GetModuleLink(this.TabId, this.ModuleId, "NotAuthorized", ArticleSettings), true);
            }

            lblTitle.Text = String.Format(DotNetNuke.Services.Localization.Localization.GetString("EditPages.Text", LocalResourceFile), objArticle.Title);

            if (objArticle.IsDraft)
            {
                cmdSubmitApproval.Visible = true;
                cmdSubmitApproval.Attributes.Add("onClick", "javascript:return confirm('" + DotNetNuke.Services.Localization.Localization.GetString("SubmitApproval.Text", LocalResourceFile) + "');");
            }
            else
            {
                cmdSubmitApproval.Visible = false;
            }

        }

        private void BindPages()
        {

            PageController objPageController = new PageController();

            DotNetNuke.Services.Localization.Localization.LocalizeDataGrid(ref grdPages, this.LocalResourceFile);

            grdPages.DataSource = objPageController.GetPageList(_articleID);
            grdPages.DataBind();

            if (grdPages.Items.Count > 0)
            {
                grdPages.Visible = true;
                lblNoPages.Visible = false;
            }
            else
            {
                grdPages.Visible = false;
                lblNoPages.Visible = true;
                lblNoPages.Text = DotNetNuke.Services.Localization.Localization.GetString("NoPagesMessage.Text", LocalResourceFile);
            }

        }

        #endregion

        #region " protected Methods "

        protected string GetEditPageUrl(string articleID, string pageID)
        {

            return Common.GetModuleLink(TabId, ModuleId, "EditPage", ArticleSettings, "ArticleID=" + articleID, "PageID=" + pageID);

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            this.Init += new EventHandler(this.Page_Init);
            cmdAddPage.Click+=cmdAddPage_Click;
            cmdSortOrder.Click+=cmdSortOrder_Click;
            cmdSummary.Click+=cmdSummary_Click;
            cmdSubmitApproval.Click+=cmdSubmitApproval_Click;
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Init(object sender, EventArgs e)
        {

            try
            {

                ReadQueryString();
            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void Page_Load(object sender, EventArgs e)
        {

            try
            {

                CheckSecurity();
                BindPages();

                if (!IsPostBack)
                {
                    BindArticle();
                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdAddPage_Click(object sender, EventArgs e)
        {

            try
            {
                Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "EditPage", ArticleSettings, "ArticleID=" + _articleID.ToString()), true);

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdSortOrder_Click(object sender, EventArgs e)
        {

            try
            {

                Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "EditSortOrder", ArticleSettings, "ArticleID=" + _articleID.ToString()), true);

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdSummary_Click(object sender, EventArgs e)
        {

            try
            {

                Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "SubmitNews", ArticleSettings, "ArticleID=" + _articleID.ToString()), true);

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdSubmitApproval_Click(object sender, EventArgs e)
        {

            try
            {

                DotNetNuke.Services.Log.EventLog.EventLogController objLogController = new DotNetNuke.Services.Log.EventLog.EventLogController();

                ArticleController objController = new ArticleController();
                ArticleInfo objArticle = objController.GetArticle(_articleID);

                if (objArticle != null)
                {
                    objArticle.Status = StatusType.AwaitingApproval;
                    objController.UpdateArticle(objArticle);

                    if (Settings.Contains(ArticleConstants.NOTIFY_SUBMISSION_SETTING))
                    {
                        if (Convert.ToBoolean(Settings[ArticleConstants.NOTIFY_SUBMISSION_SETTING]))
                        {
                            EmailTemplateController objEmailTemplateController = new EmailTemplateController();
                            string emails = objEmailTemplateController.GetApproverDistributionList(ModuleId);

                            foreach (string email in emails.Split(';'))
                            {
                                if (email != "")
                                {
                                    objEmailTemplateController.SendFormattedEmail(this.ModuleId, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false), objArticle, EmailTemplateType.ArticleSubmission, email, ArticleSettings);
                                }
                            }
                        }
                    }

                    if (Settings.Contains(ArticleConstants.NOTIFY_SUBMISSION_SETTING_EMAIL))
                    {
                        if (Settings[ArticleConstants.NOTIFY_SUBMISSION_SETTING_EMAIL].ToString() != "")
                        {
                            EmailTemplateController objEmailTemplateController = new EmailTemplateController();
                            objEmailTemplateController.SendFormattedEmail(this.ModuleId, Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false), objArticle, EmailTemplateType.ArticleSubmission, Settings[ArticleConstants.NOTIFY_SUBMISSION_SETTING_EMAIL].ToString(), ArticleSettings);
                        }
                    }

                    Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "SubmitNewsComplete", ArticleSettings, "ArticleID=" + _articleID.ToString()), true);

                }
                else
                {
                    Exceptions.ProcessModuleLoadException(this, new Exception("Unable to Retrieve Article to Submit for Approval"));
                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}