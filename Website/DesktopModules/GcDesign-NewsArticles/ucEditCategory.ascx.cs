using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Security.Roles;
using DotNetNuke.Entities.Modules;

using GcDesign.NewsArticles.Base;
using DotNetNuke.UI.UserControls;
using System.Collections;

namespace GcDesign.NewsArticles
{
    public partial class ucEditCategory : NewsArticleModuleBase
    {
        #region " private Members "

        private int _categoryID = Null.NullInteger;

        #endregion

        #region " private Properties "

        #endregion


        #region " private Methods "

        private void ReadQueryString()
        {

            if (Numeric.IsNumeric(Request["CategoryID"]))
            {
                _categoryID = Convert.ToInt32(Request["CategoryID"]);
            }

        }

        private void BindCategory()
        {

            if (_categoryID == Null.NullInteger)
            {
                cmdDelete.Visible = false;
                trPermissions.Visible = false;
                trSecurityMode.Visible = false;
                chkInheritSecurity.Checked = true;
                lstSecurityMode.SelectedIndex = 0;
                return;
            }

            CategoryController objCategoryController = new CategoryController();
            CategoryInfo objCategoryInfo = objCategoryController.GetCategory(_categoryID, ModuleId);

            if (objCategoryInfo != null)
            {
                if (drpParentCategory.Items.FindByValue(objCategoryInfo.ParentID.ToString()) != null)
                {
                    drpParentCategory.SelectedValue = objCategoryInfo.ParentID.ToString();
                }
                txtName.Text = objCategoryInfo.Name;
                txtDescription.Text = objCategoryInfo.Description;
                ctlIcon.Url = objCategoryInfo.Image;
                cmdDelete.Visible = true;
                chkInheritSecurity.Checked = objCategoryInfo.InheritSecurity;
                trPermissions.Visible = !chkInheritSecurity.Checked;
                trSecurityMode.Visible = !chkInheritSecurity.Checked;
                lstSecurityMode.SelectedValue = Convert.ToInt32(objCategoryInfo.CategorySecurityType).ToString();

                txtMetaTitle.Text = objCategoryInfo.MetaTitle;
                txtMetaDescription.Text = objCategoryInfo.MetaDescription;
                txtMetaKeyWords.Text = objCategoryInfo.MetaKeywords;
            }

        }

        private void BindParentCategories()
        {

            CategoryController objCategoryController = new CategoryController();
            drpParentCategory.DataSource = objCategoryController.GetCategoriesAll(this.ModuleId, Null.NullInteger, ArticleSettings.CategorySortType);
            drpParentCategory.DataBind();

            drpParentCategory.Items.Insert(0, new ListItem(Localization.GetString("NoParentCategory", this.LocalResourceFile), Null.NullInteger.ToString()));

            if (Request["ParentID"] != null)
            {
                if (drpParentCategory.Items.FindByValue(Request["ParentID"]) != null)
                {
                    drpParentCategory.SelectedValue = Request["ParentID"];
                }
            }

        }

        private void BindRoles()
        {

            RoleController objRole = new RoleController();
            ArrayList availableRoles = new ArrayList();
            ArrayList roles = objRole.GetPortalRoles(PortalId);

            if (roles != null)
            {
                foreach (RoleInfo Role in roles)
                {
                    availableRoles.Add(new ListItem(Role.RoleName, Role.RoleName));
                }
            }

            grdCategoryPermissions.DataSource = availableRoles;
            grdCategoryPermissions.DataBind();

        }

        private void BindSecurityMode()
        {

            foreach (int value in System.Enum.GetValues(typeof(CategorySecurityType)))
            {
                ListItem li = new ListItem();
                li.Value = value.ToString();
                li.Text = Localization.GetString(System.Enum.GetName(typeof(CategorySecurityType), value), this.LocalResourceFile);
                lstSecurityMode.Items.Add(li);
            }

        }

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

        private void SaveCategory()
        {

            CategoryInfo objCategoryInfo = new CategoryInfo();

            objCategoryInfo.CategoryID = _categoryID;
            objCategoryInfo.ModuleID = ModuleId;
            objCategoryInfo.ParentID = Convert.ToInt32(drpParentCategory.SelectedValue);
            objCategoryInfo.Name = txtName.Text;
            objCategoryInfo.Description = txtDescription.Text;
            objCategoryInfo.Image = ctlIcon.Url;
            objCategoryInfo.InheritSecurity = chkInheritSecurity.Checked;
            objCategoryInfo.CategorySecurityType = (CategorySecurityType)Convert.ToInt32(lstSecurityMode.SelectedValue);

            objCategoryInfo.MetaTitle = txtMetaTitle.Text;
            objCategoryInfo.MetaDescription = txtMetaDescription.Text;
            objCategoryInfo.MetaKeywords = txtMetaKeyWords.Text;



            CategoryController objCategoryController = new CategoryController();

            if (objCategoryInfo.CategoryID == Null.NullInteger)
            {
                List<CategoryInfo> objCategories = objCategoryController.GetCategories(ModuleId, objCategoryInfo.ParentID);

                objCategoryInfo.SortOrder = 0;
                if (objCategories.Count > 0)
                {
                    objCategoryInfo.SortOrder = ((CategoryInfo)objCategories[objCategories.Count - 1]).SortOrder + 1;
                }
                objCategoryInfo.CategoryID = objCategoryController.AddCategory(objCategoryInfo);
            }
            else
            {
                CategoryInfo objCategoryOld = objCategoryController.GetCategory(objCategoryInfo.CategoryID, ModuleId);

                if (objCategoryOld != null)
                {
                    objCategoryInfo.SortOrder = objCategoryOld.SortOrder;
                    if (objCategoryInfo.ParentID != objCategoryOld.ParentID)
                    {
                        List<CategoryInfo> objCategories = objCategoryController.GetCategories(ModuleId, objCategoryInfo.ParentID);
                        if (objCategories.Count > 0)
                        {
                            objCategoryInfo.SortOrder = ((CategoryInfo)objCategories[objCategories.Count - 1]).SortOrder + 1;
                        }
                    }
                }
                objCategoryController.UpdateCategory(objCategoryInfo);
            }

            string viewRoles = "";
            string submitRoles = "";

            foreach (DataGridItem item in grdCategoryPermissions.Items)
            {
                string role = grdCategoryPermissions.DataKeys[item.ItemIndex].ToString();

                CheckBox chkView = (CheckBox)item.FindControl("chkView");
                if (chkView.Checked)
                {
                    if (viewRoles == "")
                    {
                        viewRoles = role;
                    }
                    else
                    {
                        viewRoles = viewRoles + ";" + role;
                    }
                }

                CheckBox chkSubmit = (CheckBox)item.FindControl("chkSubmit");
                if (chkSubmit.Checked)
                {
                    if (submitRoles == "")
                    {
                        submitRoles = role;
                    }
                    else
                    {
                        submitRoles = submitRoles + ";" + role;
                    }
                }
            }

            ModuleController objModuleController = new ModuleController();
            objModuleController.UpdateModuleSetting(this.ModuleId, objCategoryInfo.CategoryID.ToString() + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING, viewRoles);
            objModuleController.UpdateModuleSetting(this.ModuleId, objCategoryInfo.CategoryID.ToString() + "-" + ArticleConstants.PERMISSION_CATEGORY_SUBMIT_SETTING, submitRoles);

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            cmdUpdate.Click+=cmdUpdate_Click;
            cmdCancel.Click+=cmdCancel_Click;
            cmdDelete.Click+=cmdDelete_Click;
            valInvalidParentCategory.ServerValidate+=valInvalidParentCategory_ServerValidate;
            grdCategoryPermissions.ItemDataBound+=grdCategoryPermissions_ItemDataBound;
            chkInheritSecurity.CheckedChanged+=chkInheritSecurity_CheckedChanged;
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
                    BindSecurityMode();
                    BindRoles();
                    BindParentCategories();
                    BindCategory();
                    Page.SetFocus(txtName);
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

                    SaveCategory();

                    Response.Redirect(EditArticleUrl("EditCategories"), true);

                }

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

                Response.Redirect(EditArticleUrl("EditCategories"), true);

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

                CategoryController objCategoryController = new CategoryController();
                CategoryInfo objCategory = objCategoryController.GetCategory(_categoryID, ModuleId);

                if (objCategory != null)
                {

                    List<CategoryInfo> objChildCategories = objCategoryController.GetCategories(this.ModuleId, _categoryID);

                    foreach (CategoryInfo objChildCategory in objChildCategories)
                    {
                        objChildCategory.ParentID = objCategory.ParentID;
                        objCategoryController.UpdateCategory(objChildCategory);
                    }

                    objCategoryController.DeleteCategory(_categoryID, ModuleId);

                }

                Response.Redirect(EditArticleUrl("EditCategories"), true);

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void valInvalidParentCategory_ServerValidate(object source, ServerValidateEventArgs args)
        {

            try
            {

                if (_categoryID == Null.NullInteger || drpParentCategory.SelectedValue == "-1")
                {
                    args.IsValid = true;
                    return;
                }

                CategoryController objCategoryController = new CategoryController();
                CategoryInfo objCategory = objCategoryController.GetCategory(Convert.ToInt32(drpParentCategory.SelectedValue), ModuleId);

                while (objCategory != null)
                {
                    if (_categoryID == objCategory.CategoryID)
                    {
                        args.IsValid = false;
                        return;
                    }
                    objCategory = objCategoryController.GetCategory(objCategory.ParentID, objCategory.ModuleID);
                }

                args.IsValid = true;

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void grdCategoryPermissions_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

            try
            {

                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    ListItem objListItem = (ListItem)e.Item.DataItem;

                    if (objListItem != null)
                    {

                        string role = ((ListItem)e.Item.DataItem).Value;

                        CheckBox chkView = (CheckBox)e.Item.FindControl("chkView");
                        CheckBox chkSubmit = (CheckBox)e.Item.FindControl("chkSubmit");

                        if (objListItem.Value == PortalSettings.AdministratorRoleName.ToString())
                        {
                            chkView.Enabled = false;
                            chkView.Checked = true;
                            chkSubmit.Enabled = false;
                            chkSubmit.Checked = true;
                        }
                        else
                        {
                            if (_categoryID != Null.NullInteger)
                            {
                                if (Settings.Contains(_categoryID.ToString() + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                                {
                                    chkView.Checked = IsInRole(role, Settings[_categoryID.ToString() + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString().Split(';'));
                                }
                                if (Settings.Contains(_categoryID.ToString() + "-" + ArticleConstants.PERMISSION_CATEGORY_SUBMIT_SETTING))
                                {
                                    chkSubmit.Checked = IsInRole(role, Settings[_categoryID.ToString() + "-" + ArticleConstants.PERMISSION_CATEGORY_SUBMIT_SETTING].ToString().Split(';'));
                                }
                            }
                        }

                    }

                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void chkInheritSecurity_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                trPermissions.Visible = !chkInheritSecurity.Checked;
                trSecurityMode.Visible = !chkInheritSecurity.Checked;

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}