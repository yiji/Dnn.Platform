using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

using GcDesign.NewsArticles.Components.CustomFields;

using GcDesign.NewsArticles.Base;
using System.Collections;

namespace GcDesign.NewsArticles
{
    public partial class ucEditCustomField : NewsArticleModuleBase
    {
        #region " private Members "

        private int _customFieldID = Null.NullInteger;

        #endregion

        #region " private Methods "

        private void AdjustFieldElements()
        {

            CustomFieldType objFieldType = (CustomFieldType)System.Enum.Parse(typeof(CustomFieldValidationType), drpFieldType.SelectedIndex.ToString());

            switch (objFieldType)
            {

                case CustomFieldType.CheckBox:
                    phRequired.Visible = true;
                    trMaximumLength.Visible = false;
                    trFieldElements.Visible = false;
                    break;
                case CustomFieldType.DropDownList:
                    phRequired.Visible = true;
                    trMaximumLength.Visible = false;
                    trFieldElements.Visible = true;
                    break;
                case CustomFieldType.MultiCheckBox:
                    phRequired.Visible = true;
                    trMaximumLength.Visible = false;
                    trFieldElements.Visible = true;
                    break;
                case CustomFieldType.MultiLineTextBox:
                    phRequired.Visible = true;
                    trMaximumLength.Visible = true;
                    trFieldElements.Visible = false;
                    break;
                case CustomFieldType.OneLineTextBox:
                    phRequired.Visible = true;
                    trMaximumLength.Visible = true;
                    trFieldElements.Visible = false;
                    break;
                case CustomFieldType.RadioButton:
                    phRequired.Visible = true;
                    trMaximumLength.Visible = false;
                    trFieldElements.Visible = true;
                    break;
                case CustomFieldType.RichTextBox:
                    phRequired.Visible = true;
                    trMaximumLength.Visible = false;
                    trFieldElements.Visible = false;
                    break;
            }

        }

        private void AdjustValidationType()
        {

            if (drpValidationType.SelectedValue == ((int)CustomFieldValidationType.Regex).ToString())
            {
                trRegex.Visible = true;
            }
            else
            {
                trRegex.Visible = false;
            }

        }

        private void BindCustomField()
        {

            if (_customFieldID == Null.NullInteger)
            {

                AdjustFieldElements();
                AdjustValidationType();
                cmdDelete.Visible = false;

            }
            else
            {

                CustomFieldController objCustomFieldController = new CustomFieldController();
                CustomFieldInfo objCustomFieldInfo = objCustomFieldController.Get(_customFieldID);

                if (objCustomFieldInfo != null)
                {
                    FieldID.Visible = true;//编辑模式时显示字段节点  显示字段ID
                    txtFieldID.Text = objCustomFieldInfo.CustomFieldID.ToString();
                    txtName.Text = objCustomFieldInfo.Name;
                    txtCaption.Text = objCustomFieldInfo.Caption;
                    txtCaptionHelp.Text = objCustomFieldInfo.CaptionHelp;
                    if (drpFieldType.Items.FindByValue(objCustomFieldInfo.FieldType.ToString()) != null)
                    {
                        drpFieldType.SelectedValue = objCustomFieldInfo.FieldType.ToString();
                    }
                    txtFieldElements.Text = objCustomFieldInfo.FieldElements;
                    AdjustFieldElements();

                    txtDefaultValue.Text = objCustomFieldInfo.DefaultValue;
                    chkVisible.Checked = objCustomFieldInfo.IsVisible;
                    if (objCustomFieldInfo.Length != Null.NullInteger)
                    {
                        txtMaximumLength.Text = objCustomFieldInfo.Length.ToString();
                    }

                    chkRequired.Checked = objCustomFieldInfo.IsRequired;
                    if (drpValidationType.Items.FindByValue(((int)objCustomFieldInfo.ValidationType).ToString()) != null)
                    {
                        drpValidationType.SelectedValue = ((int)objCustomFieldInfo.ValidationType).ToString();
                    }
                    txtRegex.Text = objCustomFieldInfo.RegularExpression;
                    AdjustValidationType();

                }

            }


        }

        private void BindFieldTypes()
        {

            foreach (int value in System.Enum.GetValues(typeof(CustomFieldType)))
            {
                ListItem li = new ListItem();
                li.Value = System.Enum.GetName(typeof(CustomFieldType), value);
                li.Text = Localization.GetString(System.Enum.GetName(typeof(CustomFieldType), value), this.LocalResourceFile);
                drpFieldType.Items.Add(li);
            }

        }

        private void BindValidationTypes()
        {

            foreach (int value in System.Enum.GetValues(typeof(CustomFieldValidationType)))
            {
                ListItem li = new ListItem();
                li.Value = value.ToString();
                li.Text = Localization.GetString(System.Enum.GetName(typeof(CustomFieldValidationType), value), this.LocalResourceFile);
                drpValidationType.Items.Add(li);
            }

        }

        private void ReadQueryString()
        {
            if (Request["CustomFieldID"] != null)
            {
                _customFieldID = Convert.ToInt32(Request["CustomFieldID"]);
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
            drpFieldType.SelectedIndexChanged+=drpFieldType_SelectedIndexChanged;
            drpValidationType.SelectedIndexChanged+=drpValidationType_SelectedIndexChanged;

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

                ReadQueryString();

                if (!Page.IsPostBack)
                {

                    BindFieldTypes();
                    BindValidationTypes();
                    BindCustomField();

                    Page.SetFocus(txtName);
                    cmdDelete.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("Confirmation", LocalResourceFile) + "');");

                }
            }
            catch (Exception exc)    //Module failed to load
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

                    CustomFieldController objCustomFieldController = new CustomFieldController();
                    CustomFieldInfo objCustomFieldInfo = new CustomFieldInfo();

                    objCustomFieldInfo.ModuleID = this.ModuleId;

                    objCustomFieldInfo.Name = txtName.Text;
                    objCustomFieldInfo.Caption = txtCaption.Text;
                    objCustomFieldInfo.CaptionHelp = txtCaptionHelp.Text;
                    objCustomFieldInfo.FieldType = (CustomFieldType)System.Enum.Parse(typeof(CustomFieldType), drpFieldType.SelectedIndex.ToString());
                    objCustomFieldInfo.FieldElements = txtFieldElements.Text;

                    objCustomFieldInfo.DefaultValue = txtDefaultValue.Text;
                    objCustomFieldInfo.IsVisible = chkVisible.Checked;
                    if (txtMaximumLength.Text.Trim() == "")
                    {
                        objCustomFieldInfo.Length = Null.NullInteger;
                    }
                    else
                    {
                        objCustomFieldInfo.Length = Convert.ToInt32(txtMaximumLength.Text);
                        if (objCustomFieldInfo.Length <= 0)
                        {
                            objCustomFieldInfo.Length = Null.NullInteger;
                        }
                    }

                    objCustomFieldInfo.IsRequired = chkRequired.Checked;
                    objCustomFieldInfo.ValidationType = (CustomFieldValidationType)System.Enum.Parse(typeof(CustomFieldValidationType), drpValidationType.SelectedIndex.ToString());
                    objCustomFieldInfo.RegularExpression = txtRegex.Text;

                    if (_customFieldID == Null.NullInteger)
                    {

                        ArrayList objCustomFields = objCustomFieldController.List(this.ModuleId);

                        if (objCustomFields.Count == 0)
                        {
                            objCustomFieldInfo.SortOrder = 0;
                        }
                        else
                        {
                            objCustomFieldInfo.SortOrder = ((CustomFieldInfo)objCustomFields[objCustomFields.Count - 1]).SortOrder + 1;
                        }

                        objCustomFieldController.Add(objCustomFieldInfo);

                    }
                    else
                    {

                        CustomFieldInfo objCustomFieldInfoOld = objCustomFieldController.Get(_customFieldID);

                        objCustomFieldInfo.SortOrder = objCustomFieldInfoOld.SortOrder;
                        objCustomFieldInfo.CustomFieldID = _customFieldID;
                        objCustomFieldController.Update(objCustomFieldInfo);

                    }

                    Response.Redirect(EditUrl("EditCustomFields"), true);

                }

            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }


        private void cmdCancel_Click(object sender, EventArgs e)
        {

            try
            {

                Response.Redirect(EditUrl("EditCustomFields"), true);

            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {

            try
            {

                CustomFieldController objCustomFieldController = new CustomFieldController();
                objCustomFieldController.Delete(this.ModuleId, _customFieldID);

                Response.Redirect(EditUrl("EditCustomFields"), true);

            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void drpFieldType_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {

                AdjustFieldElements();

            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void drpValidationType_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {

                AdjustValidationType();

            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}