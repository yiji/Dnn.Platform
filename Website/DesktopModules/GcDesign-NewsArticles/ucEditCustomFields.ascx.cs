using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;

using GcDesign.NewsArticles.Components.CustomFields;

using GcDesign.NewsArticles.Base;
using System.Collections;

namespace GcDesign.NewsArticles
{
    public partial class ucEditCustomFields : NewsArticleModuleBase
    {

        #region " private Members "

        ArrayList _customFields;

        #endregion

        #region " private Methods "

        private void BindCustomFields()
        {

            Localization.LocalizeDataGrid(ref grdCustomFields, this.LocalResourceFile);

            CustomFieldController objCustomFieldController = new CustomFieldController();

            _customFields = objCustomFieldController.List(this.ModuleId);
            grdCustomFields.DataSource = _customFields;

            grdCustomFields.DataBind();

            if (grdCustomFields.Items.Count == 0)
            {
                grdCustomFields.Visible = false;
                lblNoCustomFields.Visible = true;
            }
            else
            {
                grdCustomFields.Visible = true;
                lblNoCustomFields.Visible = false;
            }

        }

        #endregion

        #region " protected Methods "

        protected string GetCustomFieldEditUrl(string customFieldID)
        {

            return EditArticleUrl("EditCustomField", new string[] { "CustomFieldID=" + customFieldID });

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            cmdAddCustomField.Click+=cmdAddCustomField_Click;
            grdCustomFields.ItemDataBound+=grdCustomFields_ItemDataBound;
            grdCustomFields.ItemCommand+=grdCustomFields_ItemCommand;
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                BindCustomFields();
            }

        }

        protected void cmdAddCustomField_Click(object sender, EventArgs e)
        {

            Response.Redirect(EditArticleUrl("EditCustomField"), true);

        }

        private void grdCustomFields_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                ImageButton btnUp = (ImageButton)e.Item.FindControl("btnUp");
                ImageButton btnDown = (ImageButton)e.Item.FindControl("btnDown");

                CustomFieldInfo objCustomField = (CustomFieldInfo)e.Item.DataItem;

                if (btnUp != null && btnDown != null)
                {

                    if (objCustomField.CustomFieldID == ((CustomFieldInfo)_customFields[0]).CustomFieldID)
                    {
                        btnUp.Visible = false;
                    }

                    if (objCustomField.CustomFieldID == ((CustomFieldInfo)_customFields[_customFields.Count - 1]).CustomFieldID)
                    {
                        btnDown.Visible = false;
                    }

                    btnUp.CommandArgument = objCustomField.CustomFieldID.ToString();
                    btnUp.CommandName = "Up";

                    btnDown.CommandArgument = objCustomField.CustomFieldID.ToString();
                    btnDown.CommandName = "Down";

                }

            }

        }

        private void grdCustomFields_ItemCommand(object sender, DataGridCommandEventArgs e)
        {

            CustomFieldController objCustomFieldController = new CustomFieldController();
            _customFields = objCustomFieldController.List(this.ModuleId);

            int customFieldID = Convert.ToInt32(e.CommandArgument);

            for (int i = 0; i < _customFields.Count; i++)
            {

                CustomFieldInfo objCustomField = (CustomFieldInfo)_customFields[i];

                if (customFieldID == objCustomField.CustomFieldID)
                {

                    if (e.CommandName == "Up")
                    {

                        CustomFieldInfo objCustomFieldToSwap = (CustomFieldInfo)_customFields[i - 1];

                        int sortOrder = objCustomField.SortOrder;
                        int sortOrderPrevious = objCustomFieldToSwap.SortOrder;

                        objCustomField.SortOrder = sortOrderPrevious;
                        objCustomFieldToSwap.SortOrder = sortOrder;

                        objCustomFieldController.Update(objCustomField);
                        objCustomFieldController.Update(objCustomFieldToSwap);

                    }


                    if (e.CommandName == "Down")
                    {

                        CustomFieldInfo objCustomFieldToSwap = (CustomFieldInfo)_customFields[i + 1];

                        int sortOrder = objCustomField.SortOrder;
                        int sortOrderNext = objCustomFieldToSwap.SortOrder;

                        objCustomField.SortOrder = sortOrderNext;
                        objCustomFieldToSwap.SortOrder = sortOrder;

                        objCustomFieldController.Update(objCustomField);
                        objCustomFieldController.Update(objCustomFieldToSwap);

                    }

                }

            }

            Response.Redirect(Request.RawUrl, true);

        }

        #endregion
    }
}