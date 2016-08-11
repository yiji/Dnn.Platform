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
    public partial class ucEditTag : NewsArticleModuleBase
    {


        #region " private Members "

        private int _tagID = Null.NullInteger;
        private int _photoCount = 0;
        private int _albumCount = 0;

        #endregion

        #region " private Methods "

        private void ReadQueryString()
        {

            if (Request["TagID"] != null)
            {
                _tagID = Convert.ToInt32(Request["TagID"]);
            }

        }

        private void BindTag()
        {

            if (_tagID == Null.NullInteger)
            {

                cmdDelete.Visible = false;

            }
            else
            {

                cmdDelete.Visible = true;
                cmdDelete.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("Confirmation", LocalResourceFile) + "');");

                TagController objTagController = new TagController();
                TagInfo objTag = objTagController.Get(_tagID);

                if (objTag != null)
                {
                    txtName.Text = objTag.Name;
                }

            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            cmdUpdate.Click += cmdUpdate_Click;
            cmdCancel.Click += cmdCancel_Click;
            cmdDelete.Click += cmdDelete_Click;
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

                ReadQueryString();

                if (!IsPostBack)
                {

                    BindTag();
                    txtName.Focus();

                }

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

                if (Page.IsValid)
                {

                    TagController objTagController = new TagController();

                    TagInfo objTag = objTagController.Get(ModuleId, txtName.Text);
                    if (objTag == null)
                    {
                        objTag = new TagInfo();

                        if (_tagID != Null.NullInteger)
                        {
                            objTag = objTagController.Get(_tagID);
                        }
                        else
                        {
                            objTag = (TagInfo)CBO.InitializeObject(objTag, typeof(TagInfo));
                        }

                        objTag.ModuleID = this.ModuleId;
                        objTag.Name = txtName.Text;
                        objTag.NameLowered = txtName.Text.ToLower();

                        if (_tagID == Null.NullInteger)
                        {
                            objTagController.Add(objTag);
                        }
                        else
                        {
                            objTagController.Update(objTag);
                        }
                    }

                    Response.Redirect(EditUrl("EditTags"), true);

                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {

            try
            {

                TagController objTagController = new TagController();
                objTagController.DeleteArticleTagByTag(_tagID);
                objTagController.Delete(_tagID);

                Response.Redirect(EditUrl("EditTags"), true);

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

                Response.Redirect(EditUrl("EditTags"), true);

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}