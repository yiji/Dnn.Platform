using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using GcDesign.NewsArticles.Components.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;

using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class GcSearchForm : NewsArticleModuleBase
    {
        #region " Private Members "

        private string m_controlToLoad;

        #endregion

        #region " Private Methods "

        private void FindSettings()
        {
            //Request["searchForm"] 要加载的控件名 没有.ascx后缀名
            if (Settings[ArticleConstants.News_Search_Window] != null)
            {

                // Load the appropriate Control加载合适的控件

                m_controlToLoad = Settings[ArticleConstants.News_Search_Window].ToString() + ".ascx";
                lblNotConfigured.Visible = false;

            }
            else
            {

                lblNotConfigured.Visible = true;

            }

        }

        private void LoadControlType()
        {

            if (m_controlToLoad != "")
            {
                PortalModuleBase objPortalModuleBase = (PortalModuleBase)this.LoadControl("Search Windows\\"+m_controlToLoad);

                if (objPortalModuleBase != null)
                {

                    objPortalModuleBase.ModuleConfiguration = this.ModuleConfiguration;
                    objPortalModuleBase.ID = System.IO.Path.GetFileNameWithoutExtension(m_controlToLoad);
                    plhControls.Controls.Add(objPortalModuleBase);

                }
            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Init += new EventHandler(this.Page_Init);
            this.PreRender += new EventHandler(Page_PreRender);
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Init(object sender, EventArgs e)
        {

            try
            {
                FindSettings();
                LoadControlType();
            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }


        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            try
            {

                jQuery.RegisterJQuery(Page);

            }
            catch (Exception exc)    //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion
    }
}