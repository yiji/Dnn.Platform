using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Services.Exceptions;
using GcDesign.NewsArticles.Components.Types;
using GcDesign.NewsArticles.Base;

namespace GcDesign.NewsArticles
{
    public partial class ucHeader : NewsArticleControlBase
    {
        #region " private Members "

        private MenuOptionType _selectedMenu = MenuOptionType.CurrentArticles;
        private MenuPositionType _menuPosition = MenuPositionType.Top;
        private bool _processMenu = false;

        #endregion

        #region " public Properties "

        public string MenuPosition
        {
            set
            {
                switch (value.ToLower())
                {

                    case "top":
                        _menuPosition = MenuPositionType.Top;
                        return;

                    case "bottom":
                        _menuPosition = MenuPositionType.Bottom;
                        return;

                }
            }
        }

        public string SelectedMenu
        {
            set
            {
                switch (value.ToLower())
                {

                    case "adminoptions":
                        _selectedMenu = MenuOptionType.AdminOptions;
                        return;

                    case "approvearticles":
                        _selectedMenu = MenuOptionType.ApproveArticles;
                        return;

                    case "approvecomments":
                        _selectedMenu = MenuOptionType.ApproveComments;
                        return;

                    case "categories":
                        _selectedMenu = MenuOptionType.Categories;
                        return;

                    case "currentarticles":
                        _selectedMenu = MenuOptionType.CurrentArticles;
                        return;

                    case "myarticles":
                        _selectedMenu = MenuOptionType.MyArticles;
                        return;

                    case "search":
                        _selectedMenu = MenuOptionType.Search;
                        return;

                    case "syndication":
                        _selectedMenu = MenuOptionType.Syndication;
                        return;

                    case "submitarticle":
                        _selectedMenu = MenuOptionType.SubmitArticle;
                        return;

                }
            }
        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.PreRender += new EventHandler(Page_PreRender);
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {

                if (!_processMenu && _menuPosition == ArticleModuleBase.ArticleSettings.MenuPosition)
                {
                    TokenProcessor.ProcessMenu(plhControls.Controls, ArticleModuleBase, _selectedMenu);
                    _processMenu = true;
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        #endregion
    }
}