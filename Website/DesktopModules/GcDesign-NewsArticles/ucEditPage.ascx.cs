using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.UserControls;

using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class ucEditPage : NewsArticleModuleBase
    {

        #region " private Properties "

        private TextEditor Summary
        {
            get
            {
                return (TextEditor)txtSummary;
            }
        }

        #endregion

        #region " private Members "

        private int _pageID = Null.NullInteger;
        private int _articleID = Null.NullInteger;

        #endregion

        #region " private Methods "

        private void ReadQueryString()
        {

            if (Numeric.IsNumeric(Request["ArticleID"]))
            {
                _articleID = Convert.ToInt32(Request["ArticleID"]);
            }

            if (Numeric.IsNumeric(Request["PageID"]))
            {
                _pageID = Convert.ToInt32(Request["PageID"]);
            }


        }

        private void CheckSecurity()
        {

            if (!HasEditRights(_articleID, this.ModuleId, this.TabId))
            {
                Response.Redirect(Common.GetModuleLink(this.TabId, this.ModuleId, "NotAuthorized", ArticleSettings), true);
            }

        }

        private void BindPage()
        {

            if (_pageID == Null.NullInteger)
            {

                cmdDelete.Visible = false;
            }
            else
            {

                cmdDelete.Visible = true;
                cmdDelete.Attributes.Add("onClick", "javascript:return confirm('Are You Sure You Wish To Delete This Item ?');");

                PageController objPageController = new PageController();
                PageInfo objPage = objPageController.GetPage(_pageID);

                if (objPage != null)
                {
                    txtTitle.Text = objPage.Title;
                    Summary.Text = objPage.PageText;
                }

            }

        }

        private void SetTextEditor()
        {

            Summary.Width = Unit.Parse(ArticleSettings.TextEditorWidth);
            Summary.Height = Unit.Parse(ArticleSettings.TextEditorHeight);

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
                CheckSecurity();

                if (!IsPostBack)
                {

                    BindPage();
                    SetTextEditor();

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

                    PageController objPageController = new PageController();

                    PageInfo objPage = new PageInfo();

                    if (_pageID != Null.NullInteger)
                    {

                        objPage = objPageController.GetPage(_pageID);

                    }
                    else
                    {

                        objPage = (PageInfo)CBO.InitializeObject(objPage, typeof(PageInfo));

                    }

                    objPage.Title = txtTitle.Text;
                    objPage.PageText = Summary.Text;
                    objPage.ArticleID = _articleID;

                    if (_pageID == Null.NullInteger)
                    {

                        objPageController.AddPage(objPage);

                    }
                    else
                    {

                        if (objPage.SortOrder == 0)
                        {
                            ArticleController objArticleController = new ArticleController();
                            ArticleInfo objArticle = objArticleController.GetArticle(_articleID);

                            if (objArticle != null)
                            {
                                if (objArticle.Title != objPage.Title)
                                {
                                    objArticle.Title = objPage.Title;
                                    objArticleController.UpdateArticle(objArticle);
                                }
                            }
                        }

                        objPageController.UpdatePage(objPage);

                    }

                    Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "EditPages", ArticleSettings, "ArticleID=" + _articleID.ToString()), true);

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

                PageController objPageController = new PageController();
                objPageController.DeletePage(_pageID);

                Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "EditPages", ArticleSettings, "ArticleID=" + _articleID.ToString()), true);

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

                Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "EditPages", ArticleSettings, "ArticleID=" + _articleID.ToString()), true);

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}