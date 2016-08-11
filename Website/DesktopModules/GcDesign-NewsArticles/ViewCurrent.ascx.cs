using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Services.Exceptions;

using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class ViewCurrent : NewsArticleModuleBase
    {

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}