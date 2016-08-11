using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Security;

using GcDesign.NewsArticles.Base;
using System.Text.RegularExpressions;
using System.Collections;

namespace GcDesign.NewsArticles
{
    public partial class ViewSearch : NewsArticleModuleBase
    {

#region " Constants "

        private const string PARAM_SEARCH_ID = "Search";

#endregion

#region " private Members "

        private string _searchText = Null.NullString;

#endregion

#region " private Methods "

        private void BindSearch(){

            this.BasePage.Title = "Search | " + this.BasePage.Title;

            if (_searchText == "") {
                lblSearch.Text = Localization.GetString("SearchArticles", this.LocalResourceFile);
                Listing1.BindArticles = false;
                return;
            }else{
                string articlesFor = Localization.GetString("ArticlesFor", this.LocalResourceFile);
                if (articlesFor.Contains("{0}")) {
                    lblSearch.Text = string.Format(articlesFor, _searchText);
                }else{
                    lblSearch.Text = articlesFor;
                }
                txtSearch.Text = _searchText;
                Listing1.SearchText = _searchText;
                Listing1.BindArticles = true;
                Listing1.BindListing();
                Listing1.BindArticles = false;
                return;
            }

        }

        private void ReadQueryString(){

            if (Request[PARAM_SEARCH_ID] != "") {
                _searchText = Server.UrlDecode(Request[PARAM_SEARCH_ID]);
                PortalSecurity objSecurity = new PortalSecurity();
                _searchText = objSecurity.InputFilter(_searchText, PortalSecurity.FilterFlag.NoScripting);
                _searchText = StripTags(_searchText);
            }

        }

        string StripTags(string html ){
            // Remove HTML tags.
            return Regex.Replace(html, "<.*?>", "");
        }


#endregion

#region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Init += new EventHandler(this.Page_Init);
            btnSearch.Click+=btnSearch_Click; ;
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void Page_Init(object sender, EventArgs e ){

            try{

                IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();
                List<string> itemsToRemove = new List<string>();

                while (enumerator.MoveNext()){
                    if (enumerator.Key.ToString().ToLower().Contains("gcdesign-newsarticles-categories-" + ModuleId.ToString())) {
                        itemsToRemove.Add(enumerator.Key.ToString());
                    }
                }

                foreach(string itemToRemove in itemsToRemove){
                    DataCache.RemoveCache(itemToRemove.Replace("DNN_", ""));
                }

                enumerator = HttpContext.Current.Cache.GetEnumerator();
                while(enumerator.MoveNext()){
                    if (enumerator.Key.ToString().ToLower().Contains("gcdesign-newsarticles-categories-" + ModuleId.ToString())) {
                        Response.Write(enumerator.Key.ToString() + "<BR>");
                    }
                }
                ReadQueryString();
                Listing1.ShowExpired = true;
                Listing1.MaxArticles = Null.NullInteger;
                Listing1.IsIndexed = false;

                BindSearch();
                Page.SetFocus(txtSearch);

            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        protected void btnSearch_Click(object sender , EventArgs e ){

            try{

                if (txtSearch.Text.Trim() != "") {
                    PortalSecurity objSecurity = new PortalSecurity();
                    Response.Redirect(Common.GetModuleLink(TabId, ModuleId, "Search", ArticleSettings, "Search=" + Server.UrlEncode(objSecurity.InputFilter(txtSearch.Text, PortalSecurity.FilterFlag.NoScripting))), true);
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