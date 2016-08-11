using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class ViewAuthor : NewsArticleModuleBase
    {

        #region " Constants "

        private const string PARAM_AUTHOR_ID = "AuthorID";

        #endregion

        #region " private Members "

        private int _authorID = Null.NullInteger;

        #endregion

        #region " private Methods "

        private void BindAuthorName()
        {

            if (_authorID == Null.NullInteger)
            {
                // Author not specified
                Response.Redirect(Globals.NavigateURL(), true);
            }

            UserController objUserController = new UserController();
            UserInfo objUser = objUserController.GetUser(this.PortalId, _authorID);

            if (objUser != null)
            {

                if (objUser.PortalID != this.PortalId)
                {
                    //Author does not belong to this portal
                    Response.Redirect(Globals.NavigateURL(), true);
                }

                string name = "";
                switch (ArticleSettings.DisplayMode)
                {
                    case DisplayType.FirstName:
                        name = objUser.FirstName;
                        break;

                    case DisplayType.FullName:
                        name = objUser.DisplayName;
                        break;

                    case DisplayType.LastName:
                        name = objUser.LastName;
                        break;

                    case DisplayType.UserName:
                        name = objUser.Username;
                        break;
                }

                string entriesFrom = Localization.GetString("AuthorEntries", LocalResourceFile);

                if (entriesFrom.Contains("{0}"))
                {
                    lblAuthor.Text = String.Format(entriesFrom, name);
                }
                else
                {
                    lblAuthor.Text = name;
                }

                this.BasePage.Title = name + " | " + PortalSettings.PortalName;
                this.BasePage.Description = lblAuthor.Text;

            }
            else
            {

                // Author not found.
                Response.Redirect(Globals.NavigateURL(), true);

            }

        }

        private void ReadQueryString()
        {

            if (Request[PARAM_AUTHOR_ID] != "" && Numeric.IsNumeric(Request[PARAM_AUTHOR_ID]))
            {
                _authorID = Convert.ToInt32(Request[PARAM_AUTHOR_ID]);
            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Init += new EventHandler(this.Page_Init);
            
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Init(object sender, EventArgs e)
        {

            try
            {

                ReadQueryString();
                BindAuthorName();

                Listing1.Author = _authorID;
                Listing1.ShowExpired = true;
                Listing1.MaxArticles = Null.NullInteger;
                Listing1.IsIndexed = false;

                Listing1.BindListing();

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}