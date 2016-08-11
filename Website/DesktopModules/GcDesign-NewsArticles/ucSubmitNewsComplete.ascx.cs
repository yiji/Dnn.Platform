using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class ucSubmitNewsComplete : NewsArticleModuleBase
    {

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            cmdSubmitArticle.Click+=cmdSubmitArticle_Click;
            cmdViewMyArticles.Click+=cmdViewMyArticles_Click;
            cmdCurrentArticles.Click+=cmdCurrentArticles_Click;
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void cmdSubmitArticle_Click(object sender, EventArgs e)
        {

            Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "SubmitNews", ArticleSettings), true);

        }

        private void cmdViewMyArticles_Click(object sender, EventArgs e)
        {

            Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "MyArticles", ArticleSettings), true);

        }

        private void cmdCurrentArticles_Click(object sender, EventArgs e)
        {

            Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "", ArticleSettings), true);

        }

        #endregion
    }
}