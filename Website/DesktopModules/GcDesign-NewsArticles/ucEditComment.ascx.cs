using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Security;

using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class ucEditComment : NewsArticleModuleBase
    {
        #region " private Members "

        private int _commentID = Null.NullInteger;
        private string _returnUrl = Null.NullString;

        #endregion

        #region " private Methods "

        private void BindComment()
        {

            if (_commentID == Null.NullInteger)
            {
                Response.Redirect(Globals.NavigateURL(), true);
            }

            CommentController objCommentController = new CommentController();
            CommentInfo objComment = objCommentController.GetComment(_commentID);

            if (!ArticleSettings.IsAdmin && !ArticleSettings.IsApprover)
            {

                ArticleController objArticleController = new ArticleController();
                ArticleInfo objArticle = objArticleController.GetArticle(objComment.ArticleID);

                if (objArticle == null)
                {
                    Response.Redirect(Globals.NavigateURL(), true);
                }

                if (objArticle.AuthorID != this.UserId)
                {
                    Response.Redirect(Globals.NavigateURL(), true);
                }

            }

            if (objComment.UserID != Null.NullInteger)
            {
                trName.Visible = false;
                trEmail.Visible = false;
                trUrl.Visible = false;
            }
            else
            {
                txtName.Text = objComment.AnonymousName;
                txtEmail.Text = objComment.AnonymousEmail;
                txtURL.Text = objComment.AnonymousURL;
            }

            txtComment.Text = objComment.Comment.Replace("<br />", "\r\n");

        }

        private string FilterInput(string stringToFilter)
        {

            PortalSecurity objPortalSecurity = new PortalSecurity();

            stringToFilter = objPortalSecurity.InputFilter(stringToFilter, PortalSecurity.FilterFlag.NoScripting);

            stringToFilter = stringToFilter.Replace(((char)13).ToString(), "");
            stringToFilter = stringToFilter.Replace("\r\n", "<br />");

            return stringToFilter;

        }

        private void ReadQueryString()
        {

            if (Numeric.IsNumeric(Request["CommentID"]))
            {
                _commentID = Convert.ToInt32(Request["CommentID"]);
            }

            if (Request["ReturnUrl"] != null)
            {
                _returnUrl = Request["ReturnUrl"];
            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            cmdUpdate.Click+=cmdUpdate_Click;
            cmdCancel.Click += cmdCancel_Click;
            cmdDelete.Click += cmdDelete_Click;
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

                if (!IsPostBack)
                {
                    BindComment();
                }
            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void cmdUpdate_Click(object sender, EventArgs e)
        {

            try
            {

                if (Page.IsValid)
                {


                    CommentController objCommentController = new CommentController();
                    CommentInfo objComment = objCommentController.GetComment(_commentID);

                    if (objComment != null)
                    {

                        if (objComment.UserID == Null.NullInteger)
                        {
                            objComment.AnonymousName = txtName.Text;
                            objComment.AnonymousEmail = txtEmail.Text;
                            objComment.AnonymousURL = txtURL.Text;
                        }

                        objComment.Comment = FilterInput(txtComment.Text);
                        objCommentController.UpdateComment(objComment);

                    }

                    if (_returnUrl != "")
                    {
                        Response.Redirect(_returnUrl, true);
                    }
                    else
                    {
                        Response.Redirect(Globals.NavigateURL(), true);
                    }

                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {

            try
            {

                if (_returnUrl != "")
                {
                    Response.Redirect(_returnUrl, true);
                }
                else
                {
                    Response.Redirect(Globals.NavigateURL(), true);
                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdDelete_Click(object sender, EventArgs e)
        {

            try
            {

                CommentController objCommentController = new CommentController();
                CommentInfo objComment = objCommentController.GetComment(_commentID);

                if (objComment != null)
                {
                    objCommentController.DeleteComment(_commentID, objComment.ArticleID);
                }

                if (_returnUrl != "")
                {
                    Response.Redirect(_returnUrl, true);
                }
                else
                {
                    Response.Redirect(Globals.NavigateURL(), true);
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