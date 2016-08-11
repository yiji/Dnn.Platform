using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using System.Xml;

using GcDesign.NewsArticles.Base;
using System.Collections;

namespace GcDesign.NewsArticles
{
    public partial class ucAdminOptions : NewsArticleModuleBase
    {
#region " Event Handlers "

        protected void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
            cmdImport.Click+=cmdImport_Click;
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender , EventArgs e ){

            try{

                trSiteTemplates.Visible = this.UserInfo.IsSuperUser;
                if (Settings.Contains(ArticleConstants.PERMISSION_SITE_TEMPLATES_SETTING)) {
                    if (Settings[ArticleConstants.PERMISSION_SITE_TEMPLATES_SETTING].ToString() != "") {
                        trSiteTemplates.Visible = PortalSecurity.IsInRoles(Settings[ArticleConstants.PERMISSION_SITE_TEMPLATES_SETTING].ToString());
                    }
                }
            }
            catch(Exception exc)     //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }



        protected void cmdImport_Click(object sender , EventArgs e ){

            string file = PortalSettings.HomeDirectoryMapPath + "import.xml";

            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            XmlNamespaceManager nsMgr = new XmlNamespaceManager(doc.NameTable);
            nsMgr.AddNamespace("wp", "http://wordpress.org/export/1.2/");
            nsMgr.AddNamespace("content", "http://purl.org/rss/1.0/modules/content/");

            XmlNode nodeRoot = doc.DocumentElement;

            XmlNodeList objCategoriesNodes = nodeRoot.SelectNodes("/rss/channel/wp:category", nsMgr);

            CategoryController objCategoryController = new CategoryController();

            foreach(XmlNode objCategoryNode in objCategoriesNodes){

                XmlNode objName = objCategoryNode.SelectSingleNode("wp:cat_name", nsMgr);

                CategoryInfo objCategoryInfo = new CategoryInfo();

                objCategoryInfo.CategoryID = Null.NullInteger;
                objCategoryInfo.ModuleID = ModuleId;
                objCategoryInfo.ParentID = Null.NullInteger;
                objCategoryInfo.Name = objName.InnerText;
                objCategoryInfo.Description = "";
                objCategoryInfo.InheritSecurity = true;
                objCategoryInfo.CategorySecurityType = 0;

                objCategoryInfo.MetaTitle = "";
                objCategoryInfo.MetaDescription = "";
                objCategoryInfo.MetaKeywords = "";

                objCategoryController.AddCategory(objCategoryInfo);

            }

            List<CategoryInfo> objCategories = objCategoryController.GetCategoriesAll(ModuleId, Null.NullInteger);

            XmlNodeList objTagNodes = nodeRoot.SelectNodes("/rss/channel/wp:tag", nsMgr);

            TagController objTagController = new TagController();

            foreach(XmlNode objTagNode in objTagNodes){

                XmlNode objName = objTagNode.SelectSingleNode("wp:tag_name", nsMgr);

                TagInfo objTag = new TagInfo();

                objTag.ModuleID = this.ModuleId;
                objTag.Name = objName.InnerText;
                objTag.NameLowered = objName.InnerText.ToLower();

                objTagController.Add(objTag);

            }

            ArrayList objTags = objTagController.List(ModuleId, Null.NullInteger);

            XmlNodeList objArticleNodes = nodeRoot.SelectNodes("/rss/channel/item", nsMgr);

            ArticleController objArticleController = new ArticleController();

            foreach(XmlNode objArticleNode in objArticleNodes){

                XmlNode objPostType = objArticleNode.SelectSingleNode("wp:post_type", nsMgr);
                XmlNode objStatus = objArticleNode.SelectSingleNode("wp:status", nsMgr);

                if (objPostType.InnerText == "post" && objStatus.InnerText != "draft") {

                    XmlNode objTitle = objArticleNode.SelectSingleNode("title", nsMgr);

                    XmlNode objContent = objArticleNode.SelectSingleNode("content:encoded", nsMgr);

                    XmlNode objPostID = objArticleNode.SelectSingleNode("wp:post_id", nsMgr);
                    XmlNode objPostDate = objArticleNode.SelectSingleNode("wp:post_date", nsMgr);
                    DateTime dt = DateTime.Parse(objPostDate.InnerText);

                    ArticleInfo objArticle = new ArticleInfo();

                    objArticle.Title = objTitle.InnerText;
                    objArticle.CreatedDate = dt;
                    objArticle.StartDate = dt;

                    objArticle.Status = StatusType.Published;
                    objArticle.CommentCount = 0;
                    objArticle.FileCount = 0;
                    objArticle.RatingCount = 0;
                    objArticle.Rating = 0;
                    objArticle.ShortUrl = "";
                    objArticle.MetaTitle = "";
                    objArticle.MetaDescription = "";
                    objArticle.MetaKeywords = "";
                    objArticle.PageHeadText = "";
                    objArticle.IsFeatured = false;
                    objArticle.IsSecure = false;
                    objArticle.LastUpdate = DateTime.Now;
                    objArticle.LastUpdateID = this.UserId;
                    objArticle.ModuleID = this.ModuleId;
                    objArticle.AuthorID = this.UserId;

                    objArticle.ArticleID = objArticleController.AddArticle(objArticle);

                    PageController objPageController = new PageController();
                    PageInfo objPage = new PageInfo();
                    objPage.PageText = objContent.InnerText;
                    objPage.ArticleID = objArticle.ArticleID;
                    objPage.Title = objArticle.Title;
                    objPageController.AddPage(objPage);

                    XmlNodeList objCategoryNodes = objArticleNode.SelectNodes("category", nsMgr);

                    foreach(XmlNode objCategoryNode in objCategoryNodes){
                        switch (objCategoryNode.Attributes["domain"].InnerText){

                            case "post_tag":

                                foreach(TagInfo objTag in objTags){
                                    if (objTag.Name.ToLower() == objCategoryNode.InnerText.ToLower()) {
                                        objTagController.Add(objArticle.ArticleID, objTag.TagID);
                                        break;
                                    }
                                }
                                break;

                            case "category":
                                foreach(CategoryInfo objCategory  in objCategories){
                                    if (objCategory.Name.ToLower() == objCategoryNode.InnerText.ToLower()) {
                                        objArticleController.AddArticleCategory(objArticle.ArticleID, objCategory.CategoryID);
                                        break;
                                    }
                                }
                                break;

                        }
                    }

                    XmlNodeList objCommentNodes = objArticleNode.SelectNodes("wp:comment", nsMgr);

                    foreach(XmlNode objCommentNode in objCommentNodes){

                        XmlNode objAuthor = objCommentNode.SelectSingleNode("wp:comment_author", nsMgr);
                        XmlNode objAuthorEmail = objCommentNode.SelectSingleNode("wp:comment_author_email", nsMgr);
                        XmlNode objAuthorUrl = objCommentNode.SelectSingleNode("wp:comment_author_url", nsMgr);
                        XmlNode objAuthorIP = objCommentNode.SelectSingleNode("wp:comment_author_IP", nsMgr);
                        XmlNode objCommentContent = objCommentNode.SelectSingleNode("wp:comment_content", nsMgr);
                        XmlNode objCommentDate = objCommentNode.SelectSingleNode("wp:comment_date", nsMgr);
                        DateTime dtComment = DateTime.Parse(objCommentDate.InnerText);

                        CommentInfo objComment = new CommentInfo();
                        objComment.ArticleID = objArticle.ArticleID;
                        objComment.UserID = Null.NullInteger;
                        objComment.AnonymousName = objAuthor.InnerText;
                        objComment.AnonymousEmail = objAuthorEmail.InnerText;
                        objComment.AnonymousURL = objAuthorUrl.InnerText;
                        objComment.Comment = FilterInput(objCommentContent.InnerText);
                        objComment.RemoteAddress = objAuthorIP.InnerText;
                        objComment.NotifyMe = false;
                        objComment.Type = 0;
                        objComment.IsApproved = true;
                        objComment.ApprovedBy = this.UserId;
                        objComment.CreatedDate = dtComment;

                        CommentController objCommentController = new CommentController();
                        objComment.CommentID = objCommentController.AddComment(objComment);

                        objArticle.CommentCount = objArticle.CommentCount + 1;
                        objArticleController.UpdateArticle(objArticle);
                    }

                }

            }

        }

#endregion

        private string FilterInput(string stringToFilter)
        {

            PortalSecurity objPortalSecurity = new PortalSecurity();

            stringToFilter = objPortalSecurity.InputFilter(stringToFilter, PortalSecurity.FilterFlag.NoScripting);

            stringToFilter = stringToFilter.Replace(Convert.ToChar(13).ToString(), "");
            stringToFilter = stringToFilter.Replace("\r\n", "<br />");

            return stringToFilter;

        }
    }
}