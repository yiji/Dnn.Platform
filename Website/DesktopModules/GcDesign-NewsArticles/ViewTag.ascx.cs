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
    public partial class ViewTag : NewsArticleModuleBase
    {

        #region " Constants "

        private const string PARAM_TAG = "Tag";

        #endregion

        #region " private Members "

        private string _tag = Null.NullString;

        #endregion

        #region " private Methods "

        private void BindTag()
        {

            if (_tag == Null.NullString)
            {
                // Author not specified
                Response.Redirect(Globals.NavigateURL(), true);
            }

            TagController objTagController = new TagController();
            TagInfo objTag = objTagController.Get(this.ModuleId, _tag.ToLower());

            if (objTag != null)
            {

                string entriesFrom = Localization.GetString("TagEntries", LocalResourceFile);

                if (entriesFrom.Contains("{0}"))
                {
                    lblTag.Text = String.Format(entriesFrom, _tag);
                }
                else
                {
                    lblTag.Text = _tag;
                }

                this.BasePage.Title = _tag + " | " + PortalSettings.PortalName;
                this.BasePage.Description = entriesFrom;

                // We never want to index the tag pages. 


            }
            else
            {

                // Author not found.
                Response.Redirect(Globals.NavigateURL(), true);

            }

        }

        private void ReadQueryString()
        {

            if (Request[PARAM_TAG] != "")
            {
                _tag = Server.UrlDecode(Request[PARAM_TAG]);
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
                BindTag();

                Listing1.Tag = _tag;
                Listing1.ShowExpired = true;
                Listing1.MaxArticles = Null.NullInteger;
                Listing1.IsIndexed = false;

                Listing1.BindListing();

            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}