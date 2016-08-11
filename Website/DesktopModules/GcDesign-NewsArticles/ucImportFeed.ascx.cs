using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

using DotNetNuke.Entities.Users;


using GcDesign.NewsArticles.Base;
using GcDesign.NewsArticles.Import;

namespace GcDesign.NewsArticles
{
    public partial class ucImportFeed : NewsArticleModuleBase
    {

        #region " private Members "

        private int _feedID = Null.NullInteger;

        #endregion

        #region " private Methods "

        private void BindAutoExpiry()
        {

            foreach (int value in System.Enum.GetValues(typeof(FeedExpiryType)))
            {
                ListItem li = new ListItem();
                li.Value = value.ToString();
                li.Text = Localization.GetString(System.Enum.GetName(typeof(FeedExpiryType), value), this.LocalResourceFile);
                drpAutoExpire.Items.Add(li);
            }

        }

        private void BindCategories()
        {

            CategoryController objCategoryController = new CategoryController();

            lstCategories.DataSource = objCategoryController.GetCategoriesAll(ModuleId, Null.NullInteger, ArticleSettings.CategorySortType);
            lstCategories.DataBind();

            this.lstCategories.Height = Unit.Parse(ArticleSettings.CategorySelectionHeight.ToString());

        }

        private void BindDateModes()
        {

            foreach (int value in System.Enum.GetValues(typeof(FeedDateMode)))
            {
                ListItem li = new ListItem();
                li.Value = value.ToString();
                li.Text = Localization.GetString(System.Enum.GetName(typeof(FeedDateMode), value), this.LocalResourceFile);
                lstDateMode.Items.Add(li);
            }

        }

        private void BindFeed()
        {

            if (_feedID == Null.NullInteger)
            {

                chkIsActive.Checked = true;
                lblAuthor.Text = this.UserInfo.Username;
                lstDateMode.SelectedValue = Convert.ToInt32(FeedDateMode.ImportDate).ToString();
                drpAutoExpire.SelectedValue = Null.NullInteger.ToString();
                cmdDelete.Visible = false;

            }
            else
            {

                cmdDelete.Visible = true;
                cmdDelete.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("Confirmation", LocalResourceFile) + "');");

                FeedController objFeedController = new FeedController();
                FeedInfo objFeed = objFeedController.Get(_feedID);

                if (objFeed != null)
                {
                    txtTitle.Text = objFeed.Title;
                    txtUrl.Text = objFeed.Url;
                    chkAutoFeature.Checked = objFeed.AutoFeature;
                    chkIsActive.Checked = objFeed.IsActive;
                    lstDateMode.SelectedValue = Convert.ToInt32(objFeed.DateMode).ToString();
                    if (objFeed.AutoExpire != Null.NullInteger)
                    {
                        txtAutoExpire.Text = objFeed.AutoExpire.ToString();
                    }
                    drpAutoExpire.SelectedValue = Convert.ToInt32(objFeed.AutoExpireUnit).ToString();

                    UserInfo objUser = UserController.GetUser(PortalId, objFeed.UserID, true);
                    if (objUser != null)
                    {
                        lblAuthor.Text = objUser.Username;
                    }
                    else
                    {
                        lblAuthor.Text = this.UserInfo.Username;
                    }

                    foreach (CategoryInfo objCategory in objFeed.Categories)
                    {
                        foreach (ListItem li in lstCategories.Items)
                        {
                            if (li.Value == objCategory.CategoryID.ToString())
                            {
                                li.Selected = true;
                            }
                        }
                    }
                }
                else
                {
                    Response.Redirect(EditUrl("ImportFeeds"), true);
                }

            }

        }

        private void ReadQueryString()
        {

            if (Request["FeedID"] != null)
            {
                _feedID = Convert.ToInt32(Request["FeedID"]);
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
            cmdSelectAuthor.Click+=cmdSelectAuthor_Click;
            valAuthor.ServerValidate+=valAuthor_ServerValidate;
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

                    BindAutoExpiry();
                    BindCategories();
                    BindDateModes();
                    BindFeed();

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

                    FeedController objFeedController = new FeedController();

                    FeedInfo objFeed = new FeedInfo();

                    if (_feedID != Null.NullInteger)
                    {
                        objFeed = objFeedController.Get(_feedID);
                    }
                    else
                    {
                        objFeed = (FeedInfo)CBO.InitializeObject(objFeed, typeof(FeedInfo));
                    }

                    objFeed.ModuleID = this.ModuleId;
                    objFeed.Title = txtTitle.Text;
                    objFeed.Url = txtUrl.Text;
                    objFeed.AutoFeature = chkAutoFeature.Checked;
                    objFeed.IsActive = chkIsActive.Checked;
                    objFeed.DateMode = (FeedDateMode)System.Enum.Parse(typeof(FeedDateMode), lstDateMode.SelectedValue);
                    objFeed.AutoExpire = Null.NullInteger;
                    if (txtAutoExpire.Text != "")
                    {
                        if (Numeric.IsNumeric(txtAutoExpire.Text))
                        {
                            if (Convert.ToInt32(txtAutoExpire.Text) > 0)
                            {
                                objFeed.AutoExpire = Convert.ToInt32(txtAutoExpire.Text);
                            }
                        }
                    }
                    objFeed.AutoExpireUnit = (FeedExpiryType)System.Enum.Parse(typeof(FeedExpiryType), drpAutoExpire.SelectedValue);

                    if (pnlAuthor.Visible)
                    {
                        UserInfo objUser = UserController.GetUserByName(this.PortalId, txtAuthor.Text);

                        if (objUser != null)
                        {
                            objFeed.UserID = objUser.UserID;
                        }
                        else
                        {
                            objFeed.UserID = this.UserId;
                        }
                    }
                    else
                    {
                        UserInfo objUser = UserController.GetUserByName(this.PortalId, lblAuthor.Text);

                        if (objUser != null)
                        {
                            objFeed.UserID = objUser.UserID;
                        }
                        else
                        {
                            objFeed.UserID = this.UserId;
                        }
                    }

                    List<CategoryInfo> objCategories = new List<CategoryInfo>();
                    foreach (ListItem li in lstCategories.Items)
                    {
                        if (li.Selected)
                        {
                            CategoryInfo objCategory = new CategoryInfo();
                            objCategory.CategoryID = Convert.ToInt32(li.Value);
                            objCategories.Add(objCategory);
                        }
                    }
                    objFeed.Categories = objCategories;

                    if (_feedID == Null.NullInteger)
                    {
                        objFeedController.Add(objFeed);
                    }
                    else
                    {
                        objFeedController.Update(objFeed);
                    }

                    Response.Redirect(EditUrl("ImportFeeds"), true);

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

                FeedController objFeedController = new FeedController();
                objFeedController.Delete(_feedID);

                Response.Redirect(EditUrl("ImportFeeds"), true);

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

                Response.Redirect(EditUrl("ImportFeeds"), true);

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void cmdSelectAuthor_Click(object sender, EventArgs e)
        {

            try
            {

                cmdSelectAuthor.Visible = false;
                lblAuthor.Visible = false;

                pnlAuthor.Visible = true;
                txtAuthor.Text = lblAuthor.Text;
                txtAuthor.Focus();

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void valAuthor_ServerValidate(object source, ServerValidateEventArgs args)
        {

            try
            {

                args.IsValid = false;

                if (txtAuthor.Text != "")
                {
                    UserInfo objUser = UserController.GetUserByName(this.PortalId, txtAuthor.Text);

                    if (objUser != null)
                    {
                        args.IsValid = true;
                    }
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