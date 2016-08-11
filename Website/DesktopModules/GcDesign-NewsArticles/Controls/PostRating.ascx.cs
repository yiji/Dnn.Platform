using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;
using DotNetNuke.Framework;
using GcDesign.NewsArticles.Components.Social;
using GcDesign.NewsArticles.Base;


namespace GcDesign.NewsArticles.Controls
{
    public partial class PostRating : System.Web.UI.UserControl
    {
        #region " private Members "

        private int _articleID = Null.NullInteger;

        #endregion

        #region " private Properties "

        private NewsArticleModuleBase ArticleModuleBase
        {
            get
            {
                return (NewsArticleModuleBase)Parent.Parent;
            }
        }

        private ArticleSettings ArticleSettings
        {
            get
            {
                return ArticleModuleBase.ArticleSettings;
            }
        }

        #endregion

        #region " private Methods "

        private void AssignLocalization()
        {

            lblRatingSaved.Text = GetResourceKey("RatingSaved");

        }

        public string GetResourceKey(string key)
        {

            string path = @"~/DesktopModules/GcDesign-NewsArticles/" + Localization.LocalResourceDirectory + @"/PostRating.ascx.resx";
            return DotNetNuke.Services.Localization.Localization.GetString(key, path);

        }

        private void ReadQueryString()
        {

            if (ArticleSettings.UrlModeType == Components.Types.UrlModeType.Shorterned)
            {
                try
                {
                    if (Numeric.IsNumeric(Request[ArticleSettings.ShortenedID]))
                    {
                        _articleID = Convert.ToInt32(Request[ArticleSettings.ShortenedID]);
                    }
                }
                catch
                { }
            }

            if (Numeric.IsNumeric(Request["ArticleID"]))
            {
                _articleID = Convert.ToInt32(Request["ArticleID"]);
            }

        }

        #endregion

        #region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            lstRating.SelectedIndexChanged+=lstRating_SelectedIndexChanged;
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Parent.Parent.GetType().BaseType.BaseType != typeof(NewsArticleModuleBase))
            {
                Visible = false;
                return;
            }

            ReadQueryString();
            AssignLocalization();

            if (!ArticleSettings.IsRateable)
            {
                this.Visible = false;
                return;
            }

            if (!IsPostBack)
            {

                RatingController objRatingController = new RatingController();
                if (Request.IsAuthenticated)
                {
                    RatingInfo objRating = objRatingController.Get(_articleID, ArticleModuleBase.UserId, ArticleModuleBase.ModuleId);

                    if (objRating != null)
                    {
                        if (objRating.RatingID != Null.NullInteger)
                        {
                            if (lstRating.Items.FindByValue(Convert.ToDouble(objRating.Rating).ToString()) != null)
                            {
                                lstRating.SelectedValue = Convert.ToDouble(objRating.Rating).ToString();
                            }
                        }
                    }
                }
                else
                {
                    HttpCookie cookie = Request.Cookies["ArticleRating" + _articleID.ToString()];
                    if (cookie != null)
                    {
                        RatingInfo objRating = objRatingController.GetByID(Convert.ToInt32(cookie.Value), _articleID, ArticleModuleBase.ModuleId);

                        if (objRating != null)
                        {
                            if (lstRating.Items.FindByValue(Convert.ToDouble(objRating.Rating).ToString()) != null)
                            {
                                lstRating.SelectedValue = Convert.ToDouble(objRating.Rating).ToString();
                            }
                        }
                    }
                }

            }

        }

        protected void lstRating_SelectedIndexChanged(object sender, EventArgs e) {

            if (Request.IsAuthenticated) {

                RatingController objRatingController = new RatingController();

                RatingInfo objRatingExists = objRatingController.Get(_articleID, ArticleModuleBase.UserId, ArticleModuleBase.ModuleId);

                if (objRatingExists.RatingID != Null.NullInteger) {
                    objRatingController.Delete(objRatingExists.RatingID, _articleID, ArticleModuleBase.ModuleId);
                }

                RatingInfo objRating = new RatingInfo();

                objRating.ArticleID = _articleID;
                objRating.CreatedDate = DateTime.Now;
                objRating.Rating = Convert.ToDouble(lstRating.SelectedValue);
                objRating.UserID = ArticleModuleBase.UserId;

                objRating.RatingID = objRatingController.Add(objRating, ArticleModuleBase.ModuleId);

                ArticleController objArticleController = new ArticleController();
                ArticleInfo objArticle = objArticleController.GetArticle(_articleID);

                if (ArticleSettings.EnableActiveSocialFeed && Request.IsAuthenticated) {
                    if (ArticleSettings.ActiveSocialRateKey != "") {
                        if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(@"~/bin/active.modules.social.dll"))) {
                            object ai = null;
                            System.Reflection.Assembly asm;
                            object ac = null;
                            try
                            {
                                asm = System.Reflection.Assembly.Load("Active.Modules.Social");
                                ac = asm.CreateInstance("Active.Modules.Social.API.Journal");
                                if (ac != null) {
                                    //ac.AddProfileItem(new Guid(ArticleSettings.ActiveSocialRateKey), objRating.UserID, Common.GetArticleLink(objArticle, ArticleModuleBase.PortalSettings.ActiveTab, ArticleSettings, false), objArticle.Title, objRating.Rating.ToString(), objRating.Rating.ToString(), 1, "");
                                }
                            }
                            catch(Exception ex)
                            {}

                        }
                    }
                }

                if (ArticleSettings.JournalIntegration) {
                    Journal objJournal = new Journal();
                    objJournal.AddRatingToJournal(objArticle, objRating, ArticleModuleBase.PortalId, ArticleModuleBase.TabId, ArticleModuleBase.UserId, Common.GetArticleLink(objArticle, ArticleModuleBase.PortalSettings.ActiveTab, ArticleSettings, false));
                }

                if (ArticleSettings.EnableSmartThinkerStoryFeed) {
                    //StoryFeedWS objStoryFeed As new wsStoryFeed.StoryFeedWS
                    //objStoryFeed.Url = DotNetNuke.Common.Globals.AddHTTP(Request.ServerVariables("HTTP_HOST") + this.ResolveUrl("~/DesktopModules/Smart-Thinker%20-%20UserProfile/StoryFeed.asmx"))

                    string val = ArticleModuleBase.GetSharedResource("StoryFeed-AddRating");

                    string delimStr = "[]";
                    char[] delimiter = delimStr.ToCharArray();
                    string[] layoutArray = val.Split(delimiter);

                    string valResult = "";

                    for (int iPtr = 0; iPtr < layoutArray.Length; iPtr = iPtr + 2){

                        valResult = valResult + layoutArray[iPtr];

                                if (iPtr < layoutArray.Length - 1)
                                {
                                    switch (layoutArray[iPtr + 1])
                                    {

                                        case "ARTICLEID":
                                    valResult = valResult + objRating.ArticleID.ToString();
                                            break;
                                        case "AUTHORID":
                                    valResult = valResult + objRating.UserID.ToString();
                                            break;
                                        case "AUTHOR":
                                    valResult = valResult + ArticleModuleBase.UserInfo.DisplayName;
                                            break;
                                        case "ARTICLELINK":
                                    valResult = valResult + Common.GetArticleLink(objArticle, ArticleModuleBase.PortalSettings.ActiveTab, ArticleSettings, false);
                                            break;
                                        case "ARTICLETITLE":
                                    valResult = valResult + objArticle.Title;
break;
                                }
                        }
                }

                    try
                    {
                        //objStoryFeed.AddAction(82, objRating.RatingID, valResult, objRating.UserID, "VE6457624576460436531768")
                    
                            }
                    catch
                    {
                    
                    }
                }

            }
            else
            {

                RatingController objRatingController = new RatingController();
                RatingInfo objRating = new RatingInfo();
                HttpCookie cookie = Request.Cookies["ArticleRating" + _articleID.ToString()];
                if (cookie != null) {
                    objRating = objRatingController.GetByID(Convert.ToInt32(cookie.Value), _articleID, ArticleModuleBase.ModuleId);
                    if  (objRating != null) {
                        if (objRating.ArticleID != Null.NullInteger) {
                            objRatingController.Delete(objRating.RatingID, _articleID, ArticleModuleBase.ModuleId);
                        }
                    }
                }

                objRating = new RatingInfo();

                objRating.ArticleID = _articleID;
                objRating.CreatedDate = DateTime.Now;
                objRating.Rating = Convert.ToDouble(lstRating.SelectedValue);
                objRating.UserID = -1;

                int ratingID = objRatingController.Add(objRating, ArticleModuleBase.ModuleId);

                cookie = new HttpCookie("ArticleRating" + _articleID.ToString());
                cookie.Value = ratingID.ToString();
                cookie.Expires = DateTime.Now.AddDays(7);
                Context.Response.Cookies.Add(cookie);

            }

            lblRatingSaved.Visible = true;

            HttpContext.Current.Items.Add("IgnoreCaptcha", "True");
            Response.Redirect(Request.RawUrl, true);

        }

        #endregion
    }
}