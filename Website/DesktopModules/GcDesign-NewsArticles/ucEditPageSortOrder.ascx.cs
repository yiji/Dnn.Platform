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
    public partial class ucEditPageSortOrder : NewsArticleModuleBase
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

        private void BindOrder()
        {

            PageController objPageController = new PageController();

            lstSortOrder.DataSource = objPageController.GetPageList(_articleID);
            lstSortOrder.DataBind();

            if (lstSortOrder.Items.Count == 0)
            {
                Response.Redirect(Common.GetModuleLink(this.TabId, this.ModuleId, "EditPages", ArticleSettings, "ArticleID=" + _articleID.ToString()), true);
            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            cmdUpdate.Click += cmdUpdate_Click;
            cmdCancel.Click += cmdCancel_Click;
            upBtn.Click+=upBtn_Click;
            downBtn.Click+=downBtn_Click;
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

                    CheckSecurity();
                    BindOrder();

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

                PageController objPageController = new PageController();

                for (int i = 0; i < lstSortOrder.Items.Count; i++)
                {

                    PageInfo objPage = objPageController.GetPage(Convert.ToInt32(lstSortOrder.Items[i].Value));
                    objPage.SortOrder = i;
                    objPageController.UpdatePage(objPage);

                }

                Response.Redirect(Common.GetModuleLink(this.TabId, this.ModuleId, "EditPages", ArticleSettings, "ArticleID=" + _articleID.ToString()), true);

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

                Response.Redirect(Common.GetModuleLink(this.TabId, this.ModuleId, "EditPages", ArticleSettings, "ArticleID=" + _articleID.ToString()), true);

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void upBtn_Click(object sender, ImageClickEventArgs e)
        {

            try
            {

                if (lstSortOrder.SelectedIndex != -1)
                {

                    if (lstSortOrder.SelectedIndex != 0)
                    {

                        int tempIndex = lstSortOrder.SelectedIndex;

                        ListItem newListItem = new ListItem();

                        newListItem.Text = lstSortOrder.SelectedItem.Text;
                        newListItem.Value = lstSortOrder.SelectedItem.Value;

                        lstSortOrder.Items.RemoveAt(tempIndex);

                        lstSortOrder.Items.Insert(tempIndex - 1, newListItem);
                        lstSortOrder.SelectedIndex = tempIndex - 1;

                    }

                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        private void downBtn_Click(object sender, ImageClickEventArgs e)
        {

            try
            {

                if (lstSortOrder.SelectedIndex != -1)
                {

                    if (lstSortOrder.SelectedIndex != lstSortOrder.Items.Count - 1)
                    {

                        int tempIndex = lstSortOrder.SelectedIndex;

                        ListItem newListItem = new ListItem();

                        newListItem.Text = lstSortOrder.SelectedItem.Text;
                        newListItem.Value = lstSortOrder.SelectedItem.Value;

                        lstSortOrder.Items.RemoveAt(tempIndex);

                        lstSortOrder.Items.Insert(tempIndex + 1, newListItem);
                        lstSortOrder.SelectedIndex = tempIndex + 1;

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