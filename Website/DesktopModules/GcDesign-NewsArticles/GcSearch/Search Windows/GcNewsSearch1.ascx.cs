using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;

using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class GcNewsSearch1 : NewsArticleModuleBase
    {
        #region " Constants "

        private const string PARAM_SEARCH_ID = "Search";

        #endregion

        #region " private Members "

        private int _articleTabID = Null.NullInteger;
        private int _articleModuleID = Null.NullInteger;

        #endregion

        #region " private Methods "

        private bool FindSettings()
        {

            if (Settings.Contains(ArticleConstants.NEWS_SEARCH_TAB_ID))
            {
                if (Numeric.IsNumeric(Settings[ArticleConstants.NEWS_SEARCH_TAB_ID].ToString()))
                {
                    _articleTabID = Convert.ToInt32(Settings[ArticleConstants.NEWS_SEARCH_TAB_ID].ToString());
                }
            }

            if (Settings.Contains(ArticleConstants.NEWS_SEARCH_MODULE_ID))
            {
                if (Numeric.IsNumeric(Settings[ArticleConstants.NEWS_SEARCH_MODULE_ID].ToString()))
                {
                    _articleModuleID = Convert.ToInt32(Settings[ArticleConstants.NEWS_SEARCH_MODULE_ID].ToString());
                    if (_articleModuleID != Null.NullInteger)
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        private void ReadQueryString()
        {

            if (Request[PARAM_SEARCH_ID] != "")
            {
                txtSearch.Text = Server.UrlDecode(Request[PARAM_SEARCH_ID]);
            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            btnSearch.Click+=btnSearch_Click;
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
                if (!IsPostBack)
                {
                    ReadQueryString();
                }

                if (FindSettings())
                {
                    phSearchForm.Visible = true;
                    lblNotConfigured.Visible = false;
                }
                else
                {
                    lblNotConfigured.Visible = true;
                    phSearchForm.Visible = false;
                }
            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

            try
            {

                if (txtSearch.Text.Trim() != "")
                {
                    Response.Redirect(Common.GetModuleLink(_articleTabID, _articleModuleID, "Search", ArticleSettings, "Search=" + Server.UrlEncode(txtSearch.Text)), true);
                }
            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}