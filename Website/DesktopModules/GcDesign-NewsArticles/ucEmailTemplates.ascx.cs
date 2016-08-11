using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Services.Exceptions;

using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class ucEmailTemplates : NewsArticleModuleBase
    {

        #region " private Methods "

        private void BindTemplateTypes()
        {

            drpTemplate.DataSource = System.Enum.GetValues(typeof(EmailTemplateType));
            drpTemplate.DataBind();

        }

        private void BindTemplate()
        {

            EmailTemplateController objTemplateController = new EmailTemplateController();

            EmailTemplateInfo objTemplate = objTemplateController.Get(this.ModuleId, (EmailTemplateType)System.Enum.Parse(typeof(EmailTemplateType), drpTemplate.SelectedValue));

            if (objTemplate != null)
            {

                txtSubject.Text = objTemplate.Subject;
                txtTemplate.Text = objTemplate.Template;

            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            cmdUpdate.Click += cmdUpdate_Click;
            cmdCancel.Click += cmdCancel_Click;
            drpTemplate.SelectedIndexChanged+=drpTemplate_SelectedIndexChanged;
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

                if (!IsPostBack)
                {
                    BindTemplateTypes();
                    BindTemplate();
                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void drpTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {

                BindTemplate();

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {

            try
            {

                EmailTemplateController objTemplateController = new EmailTemplateController();

                EmailTemplateInfo objTemplate = objTemplateController.Get(this.ModuleId, (EmailTemplateType)System.Enum.Parse(typeof(EmailTemplateType), drpTemplate.SelectedValue));

                objTemplate.Subject = txtSubject.Text;
                objTemplate.Template = txtTemplate.Text;

                objTemplateController.Update(objTemplate);

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {

            try
            {

                Response.Redirect(EditArticleUrl("AdminOptions"), true);

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}