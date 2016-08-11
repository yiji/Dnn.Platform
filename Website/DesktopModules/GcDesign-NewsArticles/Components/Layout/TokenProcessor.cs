using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using GcDesign.NewsArticles.Components.Common;

using DotNetNuke;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;

using GcDesign.NewsArticles.Base;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GcDesign.NewsArticles
{
    public class TokenProcessor
    {
        #region " private Methods "

        private static string GetModuleLink(string key, NewsArticleModuleBase moduleContext)
        {
            return Common.GetModuleLink(moduleContext.TabId, moduleContext.ModuleId, key, moduleContext.ArticleSettings);
        }

        #endregion

        #region " Process Menu "

        public static void ProcessMenu(ControlCollection placeHolder, NewsArticleModuleBase moduleContext, MenuOptionType selectedMenu)
        {

            LayoutController objLayoutController = new LayoutController(moduleContext);
            LayoutInfo objLayout = LayoutController.GetLayout(moduleContext, LayoutType.Menu_Item_Html);

            for (int iPtr = 0; iPtr < objLayout.Tokens.Length; iPtr = iPtr + 2)
            {

                placeHolder.Add(new LiteralControl(objLayoutController.ProcessImages(objLayout.Tokens[iPtr].ToString())));

                if (iPtr < objLayout.Tokens.Length - 1)
                {
                    ProcessMenuItem(objLayout.Tokens[iPtr + 1], placeHolder, objLayoutController, moduleContext, ref iPtr, objLayout.Tokens, selectedMenu);
                }

            }

        }

        public static void ProcessMenuItem(string token, ControlCollection objPlaceHolder, LayoutController objLayoutController, NewsArticleModuleBase moduleContext, ref int iPtr, string[] templateArray, MenuOptionType selectedMenu)
        {

            //Dim path As String = objPage.TemplateSourceDirectory + "/DesktopModules/GcDesign-NewsArticles/" + Localization.LocalResourceDirectory + "/" + Localization.LocalSharedResourceFile
            //path = "~" + path.Substring(path.IndexOf("/DesktopModules/"), path.Length - path.IndexOf("/DesktopModules/"))

            string path = @"~/DesktopModules/GcDesign-NewsArticles/" + Localization.LocalResourceDirectory + "/" + Localization.LocalSharedResourceFile;

            Literal objLiteral;

            switch (token)
            {

                case "ADMINLINK":
                    objLiteral = new Literal();

                    List<string> parameters = new List<string>();
                    parameters.Add("mid=" + moduleContext.ModuleId);

                    if (moduleContext.ArticleSettings.AuthorUserIDFilter)
                    {
                        if (moduleContext.ArticleSettings.AuthorUserIDParam != "")
                        {
                            if (HttpContext.Current.Request[moduleContext.ArticleSettings.AuthorUserIDParam] != "")
                            {
                                parameters.Add(moduleContext.ArticleSettings.AuthorUserIDParam + "=" + HttpContext.Current.Request[moduleContext.ArticleSettings.AuthorUserIDParam]);
                            }
                        }
                    }

                    if (moduleContext.ArticleSettings.AuthorUsernameFilter)
                    {
                        if (moduleContext.ArticleSettings.AuthorUsernameParam != "")
                        {
                            if (HttpContext.Current.Request[moduleContext.ArticleSettings.AuthorUsernameParam] != "")
                            {
                                parameters.Add(moduleContext.ArticleSettings.AuthorUsernameParam + "=" + HttpContext.Current.Request[moduleContext.ArticleSettings.AuthorUsernameParam]);
                            }
                        }
                    }

                    objLiteral.Text = Globals.NavigateURL(moduleContext.TabId, "AdminOptions", parameters.ToArray());
                    objPlaceHolder.Add(objLiteral);
                    break;
                case "ARCHIVESLINK":
                    objLiteral = new Literal();
                    objLiteral.Text = GetModuleLink("Archives", moduleContext);
                    objPlaceHolder.Add(objLiteral);
                    break;
                case "APPROVEARTICLESLINK":
                    objLiteral = new Literal();
                    objLiteral.Text = GetModuleLink("ApproveArticles", moduleContext);
                    objPlaceHolder.Add(objLiteral);
                    break;
                case "APPROVECOMMENTSLINK":
                    objLiteral = new Literal();
                    objLiteral.Text = GetModuleLink("ApproveComments", moduleContext);
                    objPlaceHolder.Add(objLiteral);
                    break;
                case "CATEGORIESLINK":
                    objLiteral = new Literal();
                    objLiteral.Text = GetModuleLink("Archives", moduleContext);
                    objPlaceHolder.Add(objLiteral);
                    break;
                case "CURRENTARTICLESLINK":
                    objLiteral = new Literal();
                    objLiteral.Text = GetModuleLink("", moduleContext);
                    objPlaceHolder.Add(objLiteral);
                    break;
                case "HASCOMMENTSENABLED":
                    if (moduleContext.ArticleSettings.IsCommentsEnabled == false || moduleContext.ArticleSettings.IsCommentModerationEnabled == false)
                    {
                        while (iPtr < templateArray.Length - 1)
                        {
                            if (templateArray[iPtr + 1] == "/HASCOMMENTSENABLED")
                            {
                                break;
                            }
                            iPtr = iPtr + 1;
                        }
                    }
                    break;
                case "/HASCOMMENTSENABLED":
                // Do Nothing
                    break;
                case "ISADMIN":
                    if (moduleContext.ArticleSettings.IsAdmin == false)
                    {
                        while (iPtr < templateArray.Length - 1)
                        {
                            if (templateArray[iPtr + 1] == "/ISADMIN")
                            {
                                break;
                            }
                            iPtr = iPtr + 1;
                        }
                    }
                    break;
                case "/ISADMIN":
                // Do Nothing
                    break;
                case "ISAPPROVER":
                    if (!moduleContext.ArticleSettings.IsApprover)
                    {
                        while (iPtr < templateArray.Length - 1)
                        {
                            if (templateArray[iPtr + 1] == "/ISAPPROVER")
                            {
                                break;
                            }
                            iPtr = iPtr + 1;
                        }
                    }
                    break;
                case "/ISAPPROVER":
                // Do Nothing
                    break;
                case "ISSELECTEDADMIN":
                    if (selectedMenu != MenuOptionType.AdminOptions)
                    {
                        while (iPtr < templateArray.Length - 1)
                        {
                            if (templateArray[iPtr + 1] == "/ISSELECTEDADMIN")
                            {
                                break;
                            }
                            iPtr = iPtr + 1;
                        }
                    }
                    break;
                case "/ISSELECTEDADMIN":
                // Do Nothing
                    break;
                case "ISSELECTEDAPPROVEARTICLES":
                    if (selectedMenu != MenuOptionType.ApproveArticles)
                    {
                        while (iPtr < templateArray.Length - 1)
                        {
                            if (templateArray[iPtr + 1] == "/ISSELECTEDAPPROVEARTICLES")
                            {
                                break;
                            }
                            iPtr = iPtr + 1;
                        }
                    }
                    break;
                case "/ISSELECTEDAPPROVEARTICLES":
                    // Do Nothing
                    break;
                case "ISSELECTEDAPPROVECOMMENTS":
                    if (selectedMenu != MenuOptionType.ApproveComments)
                    {
                        while (iPtr < templateArray.Length - 1)
                        {
                            if (templateArray[iPtr + 1] == "/ISSELECTEDAPPROVECOMMENTS")
                            {
                                break;
                            }
                            iPtr = iPtr + 1;
                        }
                    }
                    break;
                case "/ISSELECTEDAPPROVECOMMENTS":
                    // Do Nothing
                    break;
                case "ISSELECTEDCATEGORIES":
                    if (selectedMenu != MenuOptionType.Categories)
                    {
                        while (iPtr < templateArray.Length - 1)
                        {
                            if (templateArray[iPtr + 1] == "/ISSELECTEDCATEGORIES")
                            {
                                break;
                            }
                            iPtr = iPtr + 1;
                        }
                    }
                    break;
                case "/ISSELECTEDCATEGORIES":
                    // Do Nothing
                    break;
                case "ISSELECTEDCURRENTARTICLES":
                    if (selectedMenu != MenuOptionType.CurrentArticles)
                    {
                        while (iPtr < templateArray.Length - 1)
                        {
                            if (templateArray[iPtr + 1] == "/ISSELECTEDCURRENTARTICLES")
                            {
                                break;
                            }
                            iPtr = iPtr + 1;
                        }
                    }
                    break;
                case "/ISSELECTEDCURRENTARTICLES":
                    // Do Nothing
                    break;
                case "ISSELECTEDMYARTICLES":
                    if (selectedMenu != MenuOptionType.MyArticles)
                    {
                        while (iPtr < templateArray.Length - 1)
                        {
                            if (templateArray[iPtr + 1] == "/ISSELECTEDMYARTICLES")
                            {
                                break;
                            }
                            iPtr = iPtr + 1;
                        }
                    }
                    break;
                case "/ISSELECTEDMYARTICLES":
                    // Do Nothing
                    break;
                case "ISSELECTEDSEARCH":
                    if (selectedMenu != MenuOptionType.Search)
                    {
                        while (iPtr < templateArray.Length - 1)
                        {
                            if (templateArray[iPtr + 1] == "/ISSELECTEDSEARCH")
                            {
                                break;
                            }
                            iPtr = iPtr + 1;
                        }
                    }
                    break;
                case "/ISSELECTEDSEARCH":
                    // Do Nothing
                    break;
                case "ISSELECTEDSYNDICATION":
                    if (selectedMenu != MenuOptionType.Syndication)
                    {
                        while (iPtr < templateArray.Length - 1)
                        {
                            if (templateArray[iPtr + 1] == "/ISSELECTEDSYNDICATION")
                            {
                                break;
                            }
                            iPtr = iPtr + 1;
                        }
                    }
                    break;
                case "/ISSELECTEDSYNDICATION":
                    // Do Nothing
                    break;
                case "ISSELECTEDSUBMITARTICLE":
                    if (selectedMenu != MenuOptionType.SubmitArticle)
                    {
                        while (iPtr < templateArray.Length - 1)
                        {
                            if (templateArray[iPtr + 1] == "/ISSELECTEDSUBMITARTICLE")
                            {
                                break;
                            }
                            iPtr = iPtr + 1;
                        }
                    }
                    break;
                case "/ISSELECTEDSUBMITARTICLE":
                    // Do Nothing
                    break;
                case "ISSYNDICATIONENABLED":
                    if (!moduleContext.ArticleSettings.IsSyndicationEnabled)
                    {
                        while (iPtr < templateArray.Length - 1)
                        {
                            if (templateArray[iPtr + 1] == "/ISSYNDICATIONENABLED")
                            {
                                break;
                            }
                            iPtr = iPtr + 1;
                        }
                    }
                    break;
                case "/ISSYNDICATIONENABLED":
                    // Do Nothing
                    break;
                case "ISSUBMITTER":
                    if (!moduleContext.ArticleSettings.IsSubmitter)
                    {
                        while (iPtr < templateArray.Length - 1)
                        {
                            if (templateArray[iPtr + 1] == "/ISSUBMITTER")
                            {
                                break;
                            }
                            iPtr = iPtr + 1;
                        }
                    }
                    break;
                case "/ISSUBMITTER":
                    // Do Nothing
                    break;
                case "MYARTICLESLINK":
                    objLiteral = new Literal();
                    objLiteral.Text = GetModuleLink("MyArticles", moduleContext);
                    objPlaceHolder.Add(objLiteral);
                    break;
                case "RSSLATESTLINK":
                    objLiteral = new Literal();
                    string authorIDParam = "";
                    if (moduleContext.ArticleSettings.AuthorUserIDFilter)
                    {
                        if (moduleContext.ArticleSettings.AuthorUserIDParam != "")
                        {
                            if (HttpContext.Current.Request[moduleContext.ArticleSettings.AuthorUserIDParam] != "")
                            {
                                authorIDParam = "&amp;AuthorID=" + HttpContext.Current.Request[moduleContext.ArticleSettings.AuthorUserIDParam];
                            }
                        }
                    }

                    if (moduleContext.ArticleSettings.AuthorUsernameFilter)
                    {
                        if (moduleContext.ArticleSettings.AuthorUsernameParam != "")
                        {
                            if (HttpContext.Current.Request[moduleContext.ArticleSettings.AuthorUsernameParam] != null)
                            {
                                try
                                {
                                    DotNetNuke.Entities.Users.UserInfo objUser = DotNetNuke.Entities.Users.UserController.GetUserByName(PortalController.Instance.GetCurrentPortalSettings().PortalId, HttpContext.Current.Request.QueryString[moduleContext.ArticleSettings.AuthorUsernameParam]);
                                    if (objUser != null)
                                    {
                                        authorIDParam = "&amp;AuthorID=" + objUser.UserID.ToString();
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                    objLiteral.Text = ArticleUtilities.ResolveUrl(@"~/DesktopModules/GcDesign%20-%20Searches/Rss.aspx") + "?TabID=" + moduleContext.TabId + "&amp;ModuleID=" + moduleContext.ModuleId + "&amp;MaxCount=25" + authorIDParam;
                    objPlaceHolder.Add(objLiteral);
                    break;
                case "SEARCHLINK":
                    objLiteral = new Literal();
                    objLiteral.Text = GetModuleLink("Search", moduleContext);
                    objPlaceHolder.Add(objLiteral);
                    break;
                case "SUBMITARTICLELINK":
                    objLiteral = new Literal();
                    if (moduleContext.ArticleSettings.LaunchLinks)
                    {
                        objLiteral.Text = GetModuleLink("Edit", moduleContext);
                    }
                    else
                    {
                        objLiteral.Text = GetModuleLink("SubmitNews", moduleContext);
                    }
                    objPlaceHolder.Add(objLiteral);
                    break;
                case "SYNDICATIONLINK":
                    objLiteral = new Literal();
                    objLiteral.Text = GetModuleLink("Syndication", moduleContext);
                    objPlaceHolder.Add(objLiteral);
                    break;
                default:
                    bool isRendered = false;

                    if (templateArray[iPtr + 1].ToUpper().StartsWith("RESX:"))
                    {
                        string key = templateArray[iPtr + 1].Substring(5, templateArray[iPtr + 1].Length - 5);
                        objLiteral = new Literal();
                        try
                        {
                            objLiteral.Text = Localization.GetString(key + ".Text", path);
                            if (objLiteral.Text == "")
                            {
                                objLiteral.Text = templateArray[iPtr + 1].Substring(5, templateArray[iPtr + 1].Length - 5);
                            }
                        }
                        catch
                        {
                            objLiteral.Text = templateArray[iPtr + 1].Substring(5, templateArray[iPtr + 1].Length - 5);
                        }
                        objLiteral.EnableViewState = false;
                        objPlaceHolder.Add(objLiteral);
                        isRendered = true;
                    }

                    if (!isRendered)
                    {
                        Literal objLiteralOther = new Literal();
                        objLiteralOther.Text = "[" + templateArray[iPtr + 1] + "]";
                        objLiteralOther.EnableViewState = false;
                        objPlaceHolder.Add(objLiteralOther);
                    }
                    break;
            }

        }


        #endregion
    }
}