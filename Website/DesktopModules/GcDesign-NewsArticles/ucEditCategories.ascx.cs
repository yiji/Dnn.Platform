using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.UI.WebControls;

using DotNetNuke.Security;
using System.Globalization;
using DotNetNuke.UI.Skins;
using DotNetNuke.Security.Roles;
using System.Collections;
using DotNetNuke.UI.Skins.Controls;
using Telerik.Web.UI;
using GcDesign.NewsArticles.Base;
using DotNetNuke.Entities.Modules;
using System.Drawing;
using System.IO;
using DotNetNuke.Common;

namespace GcDesign.NewsArticles
{
    public enum Position
    {
        Child,
        Below,
        Above
    }

    public partial class ucEditCategories : NewsArticleModuleBase
    {

        #region " Property "
        protected List<CategoryInfo> Categories
        {
            get
            {
                return (new CategoryController()).GetCategoriesAll(ModuleId,Null.NullInteger,CategorySortType.SortOrder);
            }
        }

        private string IconPortal
        {
            get
            {
                return TemplateSourceDirectory + "/images/Icon_Portal.png";
            }
        }

        private string SelectedNode
        {
            get
            {
                return (string)ViewState["SelectedNode"];
            }
            set
            {
                ViewState["SelectedNode"] = value;
            }
        }

        private bool IsEditMode
        {
            get
            {
                return ViewState["IsEditMode"] == null ? false : (bool)ViewState["IsEditMode"];
            }
            set
            {
                ViewState["IsEditMode"] = value;
            }
        }

        private bool IsAddMode
        {
            get
            {
                return Request["IsAddMode"] == null ? false : bool.Parse(Request["IsAddMode"].ToString());
            }
        }

        private string AddCategoryParentId
        {
            get
            {
                return Request["AddCategoryParentId"] == null ? Null.NullInteger.ToString() : Request["AddCategoryParentId"].ToString();
            }
        }

        private int EditCategoryId
        {
            get
            {
                return ViewState["EditCategoryId"] == null ? Null.NullInteger : (int)ViewState["EditCategoryId"];
            }
            set
            {
                ViewState["EditCategoryId"] = value;
            }
        }

        #endregion



        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            cmdUpdate.Click += cmdUpdate_Click;
            cmdDelete.Click += cmdDelete_Click;
            chkInheritSecurity.CheckedChanged += chkInheritSecurity_CheckedChanged;
            grdCategoryPermissions.ItemDataBound += grdCategoryPermissions_ItemDataBound;
            valInvalidParentCategory.ServerValidate += valInvalidParentCategory_ServerValidate;
            btnAdd.Click += btnAdd_Click;

            //初始化 组织架构树形图
            ctlCategory.NodeClick += CtlCategoryNodeClick;
            ctlCategory.ContextMenuItemClick += CtlCategoryContextMenuItemClick;
            ctlCategory.NodeEdit += CtlCategoryNodeEdit;
            if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
            {
                ctlCategory.EnableDragAndDrop = true;
                ctlCategory.EnableDragAndDropBetweenNodes = true;
                ctlCategory.NodeDrop += CtlCategoryNodeDrop;
            }
            else
            {
                ctlCategory.EnableDragAndDrop = false;
                ctlCategory.EnableDragAndDropBetweenNodes = false;
            }
            ctlCategory.NodeExpand += CtlCategoryNodeExpand;
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
                    //绑定组织架构树形图
                    BindTree();

                    BindSecurityMode();
                    BindRoles();
                    BindParentCategories();
                }

                if (IsAddMode)
                {
                    editBtnGroup.Visible = true;
                    categoryStatus.InnerText = "Add Category";
                    drpParentCategory.SelectedValue = AddCategoryParentId;
                    if (!IsPostBack)
                    {
                        chkInheritSecurity.Checked = true;
                        trPermissions.Visible = !chkInheritSecurity.Checked;
                        trSecurityMode.Visible = !chkInheritSecurity.Checked;
                        BindCategory(Null.NullInteger);
                    }
                }
                else if (IsEditMode)
                {
                    editBtnGroup.Visible = true;
                }
                else
                {
                    editBtnGroup.Visible = false;
                    categoryStatus.InnerText = "Category Details";
                }

            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void chkInheritSecurity_CheckedChanged(object sender, EventArgs e)
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

        private void CtlCategoryNodeExpand(object sender, RadTreeNodeEventArgs e)
        {
            AddChildNodes(e.Node);
        }

        private void CtlCategoryNodeDrop(object sender, RadTreeNodeDragDropEventArgs e)
        {
            if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
            {
                var sourceNode = e.SourceDragNode;
                var destNode = e.DestDragNode;
                var dropPosition = e.DropPosition;
                if (destNode != null && destNode.Level != 0)//destNode.Level=0 根节点，国泰中会出错
                {
                    if (sourceNode.TreeView.SelectedNodes.Count <= 1)
                    {
                        PerformDragAndDrop(dropPosition, sourceNode, destNode);
                    }
                    else if (sourceNode.TreeView.SelectedNodes.Count > 1)
                    {
                        foreach (var node in sourceNode.TreeView.SelectedNodes)
                        {
                            PerformDragAndDrop(dropPosition, node, destNode);
                        }
                    }

                    destNode.Expanded = true;

                    foreach (var node in ctlCategory.GetAllNodes())
                    {
                        node.Selected = node.Value == e.SourceDragNode.Value;
                    }
                }
            }
        }

        private void CtlCategoryNodeEdit(object sender, RadTreeNodeEditEventArgs e)
        {
            int categoryId = Convert.ToInt32(e.Node.Value);
            CategoryController objController = new CategoryController();
            CategoryInfo category = objController.GetCategory(categoryId, ModuleId);
            category.Name = e.Text;
            objController.UpdateCategory(category);
            BindTree();
        }

        private void CtlCategoryContextMenuItemClick(object sender, RadTreeViewContextMenuEventArgs e)
        {
            //获取选中节点的Value 即 部门OrganizationID
            int id = Convert.ToInt32(e.Node.Value);
            var objController = new CategoryController();
            CategoryInfo obj = objController.GetCategory(id, ModuleId);

            switch (e.MenuItem.Value.ToLower())
            {
                case "view":
                    break;
                case "edit":
                    categoryStatus.InnerText = "Edit Category";
                    IsEditMode = true;
                    EditCategoryId = int.Parse(e.Node.Value);
                    editBtnGroup.Visible = true;
                    BindCategory(EditCategoryId);
                    break;
                case "delete":
                    if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
                    {
                        List<CategoryInfo> lst = objController.GetCategoriesAll(ModuleId, obj.CategoryID);
                        foreach (CategoryInfo category in lst)//删除子节点
                        {
                            objController.DeleteCategory(category.CategoryID, ModuleId);
                        }
                        objController.DeleteCategory(ModuleId, obj.CategoryID);

                        BindTree();
                        //keep the parent tab selected
                        if (obj.ParentID != Null.NullInteger)
                        {
                            SelectedNode = obj.ParentID.ToString(CultureInfo.InvariantCulture);
                            ctlCategory.FindNodeByValue(SelectedNode).Selected = true;
                            ctlCategory.FindNodeByValue(SelectedNode).ExpandParentNodes();
                        }
                        Skin.AddModuleMessage(this, string.Format(Localization.GetString("NodeDeleted", LocalResourceFile), obj.Name), ModuleMessage.ModuleMessageType.GreenSuccess);
                    }
                    break;
                case "add":
                    //IsEditMode = true;
                    //editBtnGroup.Visible = true;
                    //categoryStatus.InnerText = "添加栏目";

                    EditCategoryId = int.Parse(e.Node.Value);
                    string url = EditArticleUrl("EditCategories", new string[] { "IsAddMode=" + true, "AddCategoryParentId=" + EditCategoryId });
                    Response.Redirect(url);
                    break;
            }
        }

        private void CtlCategoryNodeClick(object sender, RadTreeNodeEventArgs e)
        {
            IsEditMode = false;
            editBtnGroup.Visible = false;
            categoryStatus.InnerText = "Category Details";
            if (e.Node.Attributes["isRootCategory"] != null && Boolean.Parse(e.Node.Attributes["isRootCategory"]))
            {
                pnlDetails.Visible = false;
            }
            else
            {
                var categoryId = Convert.ToInt32(e.Node.Value);
                BindCategory(categoryId);


                ctlCategory.FindNode(node => node.Value == e.Node.Value).ExpandParentNodes();
                ctlCategory.FindNode(node => node.Value == e.Node.Value).Selected = true;
                ctlCategory.FindNode(node => node.Value == e.Node.Value).Expanded = true;
                ctlCategory.FindNodeByValue(e.Node.Value).ExpandChildNodes();
            }
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            try
            {

                if (Page.IsValid)
                {

                    SaveCategory();
                    BindTree();
                    Response.Redirect(Globals.NavigateURL("EditCategories","mid",ModuleId.ToString()),false);
                    BindParentCategories();
                    categoryStatus.InnerText = "Category Details";
                    editBtnGroup.Visible = false;
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
                if (!IsEditMode || EditCategoryId == Null.NullInteger)
                {
                    return;
                }

                CategoryController objCategoryController = new CategoryController();
                CategoryInfo objCategory = objCategoryController.GetCategory(EditCategoryId, ModuleId);

                if (objCategory != null)
                {

                    List<CategoryInfo> objChildCategories = objCategoryController.GetCategories(this.ModuleId, EditCategoryId);

                    foreach (CategoryInfo objChildCategory in objChildCategories)
                    {
                        objChildCategory.ParentID = objCategory.ParentID;
                        objCategoryController.UpdateCategory(objChildCategory);
                    }

                    objCategoryController.DeleteCategory(EditCategoryId, ModuleId);
                    BindTree();
                }

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
                            if (EditCategoryId != Null.NullInteger)
                            {
                                if (Settings.Contains(EditCategoryId.ToString() + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING))
                                {
                                    chkView.Checked = IsInRole(role, Settings[EditCategoryId.ToString() + "-" + ArticleConstants.PERMISSION_CATEGORY_VIEW_SETTING].ToString().Split(';'));
                                }
                                if (Settings.Contains(EditCategoryId.ToString() + "-" + ArticleConstants.PERMISSION_CATEGORY_SUBMIT_SETTING))
                                {
                                    chkSubmit.Checked = IsInRole(role, Settings[EditCategoryId.ToString() + "-" + ArticleConstants.PERMISSION_CATEGORY_SUBMIT_SETTING].ToString().Split(';'));
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

        void valInvalidParentCategory_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {

                if (EditCategoryId == Null.NullInteger || drpParentCategory.SelectedValue == "-1")
                {
                    args.IsValid = true;
                    return;
                }

                CategoryController objCategoryController = new CategoryController();
                CategoryInfo objCategory = objCategoryController.GetCategory(Convert.ToInt32(drpParentCategory.SelectedValue), ModuleId);

                while (objCategory != null)
                {
                    if (EditCategoryId == objCategory.CategoryID)
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

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL("EditCategories", "mid", ModuleId.ToString(), "IsAddMode","True"), false);
        }
        #endregion

        #region " Private Method "
        private void BindCategory(int categoryId)
        {
            BindCategoryInfo(categoryId);
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


        private void AddAttributes(ref RadTreeNode node, CategoryInfo category)
        {
            var canView = true;
            bool canEdit;
            bool canAdd;
            bool canDelete;

            if (node.Attributes["isCategoryRoot"] != null && Boolean.Parse(node.Attributes["isCategoryRoot"]))
            {
                canAdd = PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName);
                canView = false;
                canEdit = false;
                canDelete = false;
            }
            else if (category == null)
            {
                canView = false;
                canEdit = false;
                canAdd = false;
                canDelete = false;
            }
            else
            {
                canAdd = PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName);
                canEdit = PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName);
                canDelete = PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName);
            }

            node.Attributes.Add("CanView", canView.ToString(CultureInfo.InvariantCulture));
            node.Attributes.Add("CanEdit", canEdit.ToString(CultureInfo.InvariantCulture));
            node.Attributes.Add("CanAdd", canAdd.ToString(CultureInfo.InvariantCulture));
            node.Attributes.Add("CanDelete", canDelete.ToString(CultureInfo.InvariantCulture));

            node.AllowEdit = canEdit;
        }


        /// <summary>
        /// 绑定树形控件
        /// </summary>
        private void BindTree()
        {
            ctlCategory.Nodes.Clear();

            //var rootNode = new RadTreeNode();
            var strParent = "-1";

            if (strParent == "-1")
            {
                //rootNode.Text = Localization.GetString("RootNode", LocalResourceFile);
                //rootNode.ImageUrl = IconPortal;
                //rootNode.Value = Null.NullInteger.ToString(CultureInfo.InvariantCulture);
                //rootNode.Expanded = true;
                //rootNode.AllowEdit = false;
                //rootNode.EnableContextMenu = true;
                //rootNode.Attributes.Add("isRootCategory", "True");
                //AddAttributes(ref rootNode, null);
            }


            foreach (var category in Categories)
            {
                if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
                {
                    if (strParent != "-1")
                    {
                        if (category.ParentID == Convert.ToInt32(strParent))
                        {
                            var node = new RadTreeNode
                            {
                                Text = string.Format("{0} {1}", category.Name, CreateCategoryIDImage(category.CategoryID)),
                                Value = category.CategoryID.ToString(CultureInfo.InvariantCulture),
                                AllowEdit = true
                            };
                            AddAttributes(ref node, category);

                            AddChildNodes(node);
                            ctlCategory.Nodes.Add(node);
                            //rootNode.Nodes.Add(node);
                        }
                    }
                    else
                    {
                        if (category.Level == 1)
                        {
                            var node = new RadTreeNode
                            {
                                Text = string.Format("{0} {1}", category.Name, CreateCategoryIDImage(category.CategoryID)),
                                Value = category.CategoryID.ToString(CultureInfo.InvariantCulture),
                                AllowEdit = true
                            };
                            AddAttributes(ref node, category);

                            AddChildNodes(node);
                            ctlCategory.Nodes.Add(node);
                            //rootNode.Nodes.Add(node);
                        }
                    }
                }
            }

            if (ctlCategory.Nodes.Count == 0)
            {
                btnAdd.Visible = true;
            }
            //ctlCategory.Nodes.Add(rootNode);
            //AttachContextMenu(ctlPages)

        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="parentNode"></param>
        private void AddChildNodes(RadTreeNode parentNode)
        {
            parentNode.Nodes.Clear();

            var parentId = int.Parse(parentNode.Value);

            foreach (var objCategory in Categories)
            {
                if (objCategory.ParentID == parentId)
                {
                    var node = new RadTreeNode
                    {
                        Text = string.Format("{0} {1}", objCategory.Name, CreateCategoryIDImage(objCategory.CategoryID)),
                        Value = objCategory.CategoryID.ToString(CultureInfo.InvariantCulture),
                        AllowEdit = true
                    };
                    AddAttributes(ref node, objCategory);
                    //If objTab.HasChildren Then
                    //    node.ExpandMode = TreeNodeExpandMode.ServerSide
                    //End If

                    AddChildNodes(node);
                    parentNode.Nodes.Add(node);
                }
            }
        }

        private void BindParentCategories()
        {

            CategoryController objCategoryController = new CategoryController();

            drpParentCategory.DataSource = objCategoryController.GetCategoriesAll(ModuleId, Null.NullInteger, ArticleSettings.CategorySortType);
            drpParentCategory.DataBind();

            drpParentCategory.Items.Insert(0, new ListItem(Localization.GetString("NoParentCategory", this.LocalResourceFile), "-1"));

        }

        private void BindCategoryInfo(int categoryId)
        {

            if (categoryId == Null.NullInteger)
            {
                cmdDelete.Visible = false;
                trPermissions.Visible = false;
                trSecurityMode.Visible = false;
                chkInheritSecurity.Checked = true;
                lstSecurityMode.SelectedIndex = 0;
                return;
            }

            CategoryController objCategoryController = new CategoryController();
            CategoryInfo objCategoryInfo = objCategoryController.GetCategory(categoryId, ModuleId);

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

        /// <summary>
        /// 执行拖放
        /// </summary>
        /// <param name="dropPosition"></param>
        /// <param name="sourceNode">被拖拽的节点</param>
        /// <param name="destNode">目的节点</param>
        private void PerformDragAndDrop(RadTreeViewDropPosition dropPosition, RadTreeNode sourceNode, RadTreeNode destNode)
        {
            var objController = new CategoryController();
            var sourceOrg = objController.GetCategory(ModuleId, int.Parse(sourceNode.Value));
            var targetOrg = objController.GetCategory(ModuleId, int.Parse(destNode.Value));

            switch (dropPosition)
            {
                case RadTreeViewDropPosition.Over:
                    if (!(sourceNode.IsAncestorOf(destNode)))//sourceNode是否是destNode上级，是 为 true
                    {
                        if (MoveOrganizationToParent(sourceOrg, targetOrg))
                        {
                            sourceNode.Owner.Nodes.Remove(sourceNode);
                            destNode.Nodes.Add(sourceNode);
                        }
                    }
                    break;
                case RadTreeViewDropPosition.Above:
                    if (MoveOrganization(sourceOrg, targetOrg, Position.Above))
                    {
                        sourceNode.Owner.Nodes.Remove(sourceNode);
                        destNode.InsertBefore(sourceNode);
                    }
                    break;
                case RadTreeViewDropPosition.Below:
                    if (MoveOrganization(sourceOrg, targetOrg, Position.Below))
                    {
                        sourceNode.Owner.Nodes.Remove(sourceNode);
                        destNode.InsertAfter(sourceNode);
                    }
                    break;
            }
        }

        /// <summary>
        /// 移动节点到父节点
        /// </summary>
        /// <param name="sourceOrg">要移动的节点</param>
        /// <param name="targetOrg">Over 移动到targetOrg节点下最后一个位置</param>
        /// <returns></returns>
        private bool MoveOrganizationToParent(CategoryInfo sourceOrg, CategoryInfo targetOrg)
        {
            //Validate Tab Path
            if (IsExistSameNameOrg(sourceOrg, targetOrg.CategoryID))
            {
                return false;
            }

            //将当前节点移到目标节点下
            var objController = new CategoryController();
            List<CategoryInfo> lst = objController.GetCategoriesAll(ModuleId, targetOrg.CategoryID);

            sourceOrg.ParentID = targetOrg.CategoryID;

            //根据栏目SortOrder排序
            var categoryOrder = from category in lst orderby category.SortOrder select category;
            if (categoryOrder.Count() > 0)
            {
                int i = 0;
                foreach (var category in categoryOrder)
                {
                    if (category.SortOrder == i)
                    {
                        continue;
                    }
                    else
                    {
                        category.SortOrder = i;
                        objController.UpdateCategory(category);
                    }
                    i += 1;
                }
                sourceOrg.SortOrder = lst.Count;
            }
            else
            {
                sourceOrg.SortOrder = 0;
            }
            objController.UpdateCategory(sourceOrg);


            Skin.AddModuleMessage(this, string.Format(Localization.GetString("CategoryMoved", LocalResourceFile), sourceOrg.Name), ModuleMessage.ModuleMessageType.GreenSuccess);
            return true;
        }

        private bool MoveOrganization(CategoryInfo sourceOrg, CategoryInfo targetOrg, Position position)
        {
            //Validate Tab Path
            if (sourceOrg == null || IsExistSameNameOrg(sourceOrg, targetOrg.CategoryID))
            {
                return false;
            }

            var objController = new CategoryController();
            switch (position)
            {
                case Position.Above:
                    objController.MoveNoddeBefore(sourceOrg, targetOrg);
                    break;
                case Position.Below:
                    var newobj = sourceOrg.Clone();
                    newobj.ParentID = targetOrg.ParentID;
                    newobj.SortOrder = targetOrg.SortOrder + 1;
                    objController.UpdateCategory(newobj);
                    break;
            }

            Skin.AddModuleMessage(this, string.Format(Localization.GetString("NodeMoved", LocalResourceFile), sourceOrg.Name), ModuleMessage.ModuleMessageType.GreenSuccess);
            return true;
        }

        /// <summary>
        /// 判断当前层次是否有同名组织架构 存在返回true 不存在返回false
        /// </summary>
        /// <param name="orgSource"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        private bool IsExistSameNameOrg(CategoryInfo orgSource, int parentID)
        {
            CategoryController objController = new CategoryController();
            List<CategoryInfo> lst = objController.GetCategoriesAll(ModuleId, parentID);
            var obj = from c in lst where c.Name == orgSource.Name select c;
            if (obj.Count<CategoryInfo>() > 0)
            {
                Skin.AddModuleMessage(this, Localization.GetString("CategoryHasExist", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
                return true;
            }
            return false;
        }

        private void SaveCategory()
        {
            if (!IsEditMode)
            {
                if (!IsAddMode)
                {
                    return;
                }
            }
            CategoryInfo objCategoryInfo = new CategoryInfo();

            objCategoryInfo.CategoryID = EditCategoryId;
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

            BindTree();
            ctlCategory.FindNode(delegate(RadTreeNode node){ return node.Value == objCategoryInfo.CategoryID.ToString(); }).ExpandParentNodes();
            ctlCategory.FindNode(node =>node.Value == objCategoryInfo.CategoryID.ToString()).Selected = true;

            IsEditMode = false;
            EditCategoryId = Null.NullInteger;

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

        /// <summary>
        /// 输出栏目Id
        /// </summary>
        /// <param name="tabID"></param>
        /// <returns></returns>
        private string CreateCategoryIDImage(int categoryId)
        {
            Bitmap bitmap = new Bitmap(60, 16);
            Graphics g = Graphics.FromImage(bitmap);
            Font font = new Font("宋体", 10, FontStyle.Regular);//Font font = new Font("Arial", 10,FontStyle.Regular); Arial 字体粗
            System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(Color.Black);

            g.DrawString(string.Format("[{0}]", categoryId.ToString()), font, brush, 1, 0);

            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            byte[] arr = new byte[stream.Length];
            stream.Position = 0;//重要
            stream.Read(arr, 0, (int)stream.Length);
            stream.Close();
            g.Dispose();
            bitmap.Dispose();
            string img = string.Format("<img src=\"data:image/png;base64,{0}\" alt=\"{1}\" class=\"imgtabid\" />", Convert.ToBase64String(arr), categoryId);
            return img;

        }

        #endregion

        public string GetConfirmString()
        {
            return Localization.GetSafeJSString(Localization.GetString("ConfirmDelete", LocalResourceFile));
        }

    }
}