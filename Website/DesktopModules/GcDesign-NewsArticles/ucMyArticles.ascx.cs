using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class ucMyArticles : NewsArticleModuleBase
    {

        public enum StatusType
        {

            Draft = 1,
            Unapproved = 2,
            Approved = 3,

        }

        private StatusType _status = StatusType.Draft;

        #region " private Properties "

        private int CurrentPage
        {
            get
            {
                if (Request["Page"] == null && Request["CurrentPage"] == null)
                {
                    return 1;
                }
                else
                {
                    try
                    {
                        if (Request["Page"] != null)
                        {
                            return Convert.ToInt32(Request["Page"]);
                        }
                        else
                        {
                            return Convert.ToInt32(Request["CurrentPage"]);
                        }
                    }
                    catch
                    {
                        return 1;
                    }

                }
            }
        }

        #endregion

        #region " private Methods "

        private void ReadQueryString()
        {

            if (Request["Status"] != null)
            {
                switch (Request["Status"].ToLower())
                {
                    case "2":
                        _status = StatusType.Unapproved;
                        break;
                    case "3":
                        _status = StatusType.Approved;
                        break;
                }
            }

        }

        private void BindSelection()
        {

            if (ArticleSettings.IsApprover || ArticleSettings.IsAdmin)
            {
                if (Request["ShowAll"] != null)
                {
                    chkShowAll.Checked = true;
                }
            }
            else
            {
                chkShowAll.Visible = false;
            }

        }

        private void BindArticles()
        {

            ArticleController objArticleController = new ArticleController();

            Localization.LocalizeDataGrid(ref grdMyArticles, this.LocalResourceFile);

            int count = 0;
            int authorID = this.UserId;
            if (chkShowAll.Checked)
            {
                authorID = Null.NullInteger;
            }

            grdMyArticles.DataSource = objArticleController.GetArticleList(this.ModuleId, Null.NullDate, Null.NullDate, null, Null.NullBoolean, null, Null.NullInteger, CurrentPage, 10, ArticleSettings.SortBy, ArticleSettings.SortDirection, false, true, Null.NullString, authorID, true, true, Null.NullBoolean, Null.NullBoolean, false, false, Null.NullString, null, false, Null.NullString, Null.NullInteger, Null.NullString, Null.NullString, ref count);

            switch (_status)
            {

                case StatusType.Draft:
                    grdMyArticles.DataSource = objArticleController.GetArticleList(this.ModuleId, Null.NullDate, Null.NullDate, null, Null.NullBoolean, null, Null.NullInteger, CurrentPage, 10, ArticleSettings.SortBy, ArticleSettings.SortDirection, false, true, Null.NullString, authorID, true, true, Null.NullBoolean, Null.NullBoolean, false, false, Null.NullString, null, false, Null.NullString, Null.NullInteger, Null.NullString, Null.NullString, ref count);
                    break;
                case StatusType.Unapproved:
                    grdMyArticles.DataSource = objArticleController.GetArticleList(this.ModuleId, Null.NullDate, Null.NullDate, null, Null.NullBoolean, null, Null.NullInteger, CurrentPage, 10, ArticleSettings.SortBy, ArticleSettings.SortDirection, false, false, Null.NullString, authorID, true, true, Null.NullBoolean, Null.NullBoolean, false, false, Null.NullString, null, false, Null.NullString, Null.NullInteger, Null.NullString, Null.NullString, ref count);
                    break;
                case StatusType.Approved:
                    grdMyArticles.DataSource = objArticleController.GetArticleList(this.ModuleId, Null.NullDate, Null.NullDate, null, Null.NullBoolean, null, Null.NullInteger, CurrentPage, 10, ArticleSettings.SortBy, ArticleSettings.SortDirection, true, false, Null.NullString, authorID, true, true, Null.NullBoolean, Null.NullBoolean, false, false, Null.NullString, null, false, Null.NullString, Null.NullInteger, Null.NullString, Null.NullString, ref count);
                    break;

            }

            grdMyArticles.DataBind();

            if (grdMyArticles.Items.Count == 0)
            {
                phNoArticles.Visible = true;
                grdMyArticles.Visible = false;

                ctlPagingControl.Visible = false;
            }
            else
            {
                phNoArticles.Visible = false;
                grdMyArticles.Visible = true;

                if (count > 10)
                {
                    ctlPagingControl.Visible = true;
                    ctlPagingControl.TotalRecords = count;
                    ctlPagingControl.PageSize = 10;
                    ctlPagingControl.CurrentPage = CurrentPage;
                    ctlPagingControl.QuerystringParams = GetParams();
                    ctlPagingControl.TabID = TabId;
                    ctlPagingControl.EnableViewState = false;
                }

                grdMyArticles.Columns[0].Visible = IsEditable;
            }

        }

        private void CheckSecurity()
        {

            if (!ArticleSettings.IsSubmitter)
            {
                Response.Redirect(Globals.NavigateURL(), true);
            }

            if (Request["ShowAll"] != null)
            {
                if (!(ArticleSettings.IsApprover || ArticleSettings.IsAdmin))
                {
                    Response.Redirect(Globals.NavigateURL(), true);
                }
            }

        }

        private string GetParams()
        {

            string param = "";

            if (Request["ctl"] != null)
            {
                if (Request["ctl"].ToLower() == "myarticles")
                {
                    param += "ctl=" + Request["ctl"] + "&mid=" + ModuleId.ToString();
                }
            }

            if (Request["articleType"] != null)
            {
                if (Request["articleType"].ToString().ToLower() == "myarticles")
                {
                    param += "articleType=" + Request["articleType"];
                }
            }

            if (Request["Status"] != null)
            {
                param += "&Status=" + Convert.ToInt32(_status).ToString();
            }

            if (Request["ShowAll"] != null)
            {
                param += "&ShowAll=" + Request["ShowAll"];
            }

            return param;

        }

        #endregion

        #region " protected Methods "

        protected string GetAdjustedCreateDate(object objItem)
        {

            ArticleInfo objArticle = (ArticleInfo)objItem;
            return objArticle.CreatedDate.ToString("d") + " " + objArticle.CreatedDate.ToString("t");

        }

        protected string GetAdjustedPublishDate(object objItem)
        {

            ArticleInfo objArticle = (ArticleInfo)objItem;
            return objArticle.StartDate.ToString("d") + " " + objArticle.StartDate.ToString("t");

        }

        protected string GetArticleLink(object objItem)
        {

            ArticleInfo objArticle = (ArticleInfo)objItem;
            return Common.GetArticleLink(objArticle, PortalSettings.ActiveTab, ArticleSettings, false);

        }

        protected string GetEditUrl(string articleID)
        {
            if (ArticleSettings.LaunchLinks)
            {
                return Common.GetModuleLink(this.TabId, this.ModuleId, "Edit", ArticleSettings, "ArticleID=" + articleID);
            }
            else
            {
                return Common.GetModuleLink(this.TabId, this.ModuleId, "SubmitNews", ArticleSettings, "ArticleID=" + articleID);
            }
        }

        protected string GetModuleLink(string key, int status)
        {

            if (status == 1)
            {
                return Common.GetModuleLink(TabId, ModuleId, "MyArticles", ArticleSettings);
            }
            else
            {
                return Common.GetModuleLink(TabId, ModuleId, "MyArticles", ArticleSettings, "Status=" + status.ToString());
            }

        }

        public new bool IsEditable
        {
            get
            {

                if (_status == StatusType.Draft)
                {
                    return true;
                }
                else
                {
                    if (ArticleSettings.IsApprover || ArticleSettings.IsAutoApprover)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        protected string IsSelected(int status)
        {

            if (status == (int)_status)
            {
                return "ui-tabs-selected ui-state-active";
            }
            else
            {
                return "";
            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            chkShowAll.CheckedChanged += chkShowAll_CheckedChanged;
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

                CheckSecurity();
                ReadQueryString();

                if (!IsPostBack)
                {
                    BindSelection();
                    BindArticles();
                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkShowAll_CheckedChanged(object sender, EventArgs e)
        {

            try
            {

                if (chkShowAll.Checked)
                {
                    if (_status != StatusType.Draft)
                    {
                        Response.Redirect(Common.GetModuleLink(this.TabId, this.ModuleId, "MyArticles", ArticleSettings, "ShowAll=1", "Status=" + Convert.ToInt32(_status).ToString()), true);
                    }
                    else
                    {
                        Response.Redirect(Common.GetModuleLink(this.TabId, this.ModuleId, "MyArticles", ArticleSettings, "ShowAll=1"), true);
                    }
                }
                else
                {
                    if (_status != StatusType.Draft)
                    {
                        Response.Redirect(Common.GetModuleLink(this.TabId, this.ModuleId, "MyArticles", ArticleSettings, "Status=" + Convert.ToInt32(_status).ToString()), true);
                    }
                    else
                    {
                        Response.Redirect(Common.GetModuleLink(this.TabId, this.ModuleId, "MyArticles", ArticleSettings), true);
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