using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class ucEditTags : NewsArticleModuleBase
    {
        #region " private Methods "

        private void BindTags()
        {

            TagController objTagController = new TagController();

            Localization.LocalizeDataGrid(ref grdTags, this.LocalResourceFile);

            grdTags.DataSource = objTagController.List(this.ModuleId, Null.NullInteger);
            grdTags.DataBind();

            if (grdTags.Items.Count > 0)
            {
                grdTags.Visible = true;
                lblNoTags.Visible = false;
            }
            else
            {
                grdTags.Visible = false;
                lblNoTags.Visible = true;
                lblNoTags.Text = Localization.GetString("NoTagsMessage.Text", LocalResourceFile);
            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            cmdAddTag.Click+=cmdAddTag_Click;
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

                BindTags();

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdAddTag_Click(object sender, EventArgs e)
        {

            Response.Redirect(EditUrl("EditTag"), true);

        }

        #endregion
    }
}