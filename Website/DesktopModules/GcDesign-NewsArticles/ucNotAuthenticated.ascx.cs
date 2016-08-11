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
    public partial class ucNotAuthenticated : NewsArticleModuleBase
    {

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Load(object sender, EventArgs e)
        {


        }

        #endregion

        #region " Public Methods "

        protected string GetLoginUrl()
        {

            try
            {

                if (PortalSettings.LoginTabId != Null.NullInteger)
                {

                    // User Defined Tab
                    //
                    return Page.ResolveUrl("~/Default.aspx?tabid=" + PortalSettings.LoginTabId.ToString());

                }
                else
                {

                    // Admin Tab
                    //
                    return Page.ResolveUrl("~/Default.aspx?tabid=" + PortalSettings.ActiveTab.TabID + "&ctl=Login");

                }

            }
            catch (Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

            return "";

        }

        #endregion
    }
}