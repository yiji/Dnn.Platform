using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Collections;
using DotNetNuke.Common.Utilities;
using System.Web.Caching;

namespace GcDesign.NewsArticles
{
    public class LatestLayoutController
    {
        #region " Public Methods "

        public LayoutInfo GetLayout(LatestLayoutType type, int moduleID , Hashtable settings){

            string cacheKey = "LatestArticles-" + moduleID.ToString() + "-" + type.ToString();
            LayoutInfo objLayout = (LayoutInfo)DataCache.GetCache(cacheKey);

            if (objLayout == null) {

                string delimStr = "[]";
                char[] delimiter  = delimStr.ToCharArray();

                objLayout = new LayoutInfo();
                string folderPath = HttpContext.Current.Server.MapPath(@"~\DesktopModules\GcDesign-NewsArticles - LatestArticles\Templates\" + moduleID.ToString());
                string filePath = HttpContext.Current.Server.MapPath(@"~\DesktopModules\GcDesign-NewsArticles - LatestArticles\Templates\" + moduleID.ToString() + @"\" + type.ToString().Replace("_", "."));

                if (File.Exists(filePath) == false) {//文件不存在
                    // Load from settings... 

                    LayoutModeType _layoutMode = ArticleConstants.LATEST_ARTICLES_LAYOUT_MODE_DEFAULT;
                    if (settings.Contains(ArticleConstants.LATEST_ARTICLES_LAYOUT_MODE)) {
                        _layoutMode = (LayoutModeType)System.Enum.Parse(typeof(LayoutModeType), settings[ArticleConstants.LATEST_ARTICLES_LAYOUT_MODE].ToString());
                    }

                    switch(type){

                        case LatestLayoutType.Listing_Header_Html:

                            string layoutHeader;

                            if (_layoutMode == LayoutModeType.Simple) {
                                if (settings.Contains(ArticleConstants.SETTING_HTML_HEADER)) {
                                    layoutHeader = settings[ArticleConstants.SETTING_HTML_HEADER].ToString();
                                }
                                else
                                {
                                    layoutHeader = ArticleConstants.DEFAULT_HTML_HEADER;
                                }
                            }
                            else
                            {
                                if (settings.Contains(ArticleConstants.SETTING_HTML_HEADER_ADVANCED)) {
                                    layoutHeader = settings[ArticleConstants.SETTING_HTML_HEADER_ADVANCED].ToString();
                                }
                                else{
                                    layoutHeader = ArticleConstants.DEFAULT_HTML_HEADER_ADVANCED;
                               }
                            }

                            if (!Directory.Exists(folderPath)) {
                                Directory.CreateDirectory(folderPath);
                            }

                            File.WriteAllText(filePath, layoutHeader);
                            break;

                        case LatestLayoutType.Listing_Item_Html:

                            string layoutItem;

                            if (_layoutMode == LayoutModeType.Simple) {
                                if (settings.Contains(ArticleConstants.SETTING_HTML_BODY)) {
                                    layoutItem = settings[ArticleConstants.SETTING_HTML_BODY].ToString();
                                }
                                else
                                {
                                    layoutItem = ArticleConstants.DEFAULT_HTML_BODY;
                                }
                            }
                            else
                            {
                                if (settings.Contains(ArticleConstants.SETTING_HTML_BODY_ADVANCED)) {
                                    layoutItem = settings[ArticleConstants.SETTING_HTML_BODY_ADVANCED].ToString();
                                }
                                else
                                {
                                    layoutItem = ArticleConstants.DEFAULT_HTML_BODY_ADVANCED;
                                }
                            }

                            if (!Directory.Exists(folderPath)) {
                                Directory.CreateDirectory(folderPath);
                            }

                            File.WriteAllText(filePath, layoutItem);
                            break;

                        case LatestLayoutType.Listing_Footer_Html:

                            string layoutFooter;

                            if (_layoutMode == LayoutModeType.Simple) {
                                if (settings.Contains(ArticleConstants.SETTING_HTML_FOOTER)) {
                                    layoutFooter = settings[ArticleConstants.SETTING_HTML_FOOTER].ToString();
                                }
                                else
                                {
                                    layoutFooter = ArticleConstants.DEFAULT_HTML_FOOTER;
                                }
                            }
                            else
                            {
                                if (settings.Contains(ArticleConstants.SETTING_HTML_FOOTER_ADVANCED)) {
                                    layoutFooter = settings[ArticleConstants.SETTING_HTML_FOOTER_ADVANCED].ToString();
                                }
                                else
                                {
                                    layoutFooter = ArticleConstants.DEFAULT_HTML_FOOTER_ADVANCED;
                                }
                            }

                            if (!Directory.Exists(folderPath)) {
                                Directory.CreateDirectory(folderPath);
                            }

                            File.WriteAllText(filePath, layoutFooter);
                            break;

                        case LatestLayoutType.Listing_Empty_Html:

                            string noArticles = ArticleConstants.SETTING_HTML_NO_ARTICLES;
                            if (settings.Contains(ArticleConstants.SETTING_HTML_NO_ARTICLES)) {
                                noArticles = settings[ArticleConstants.SETTING_HTML_NO_ARTICLES].ToString();
                            }
                            noArticles = "<div id='NoRecords' class='Normal'>" + noArticles + "</div>";

                            if (!Directory.Exists(folderPath)) {
                                Directory.CreateDirectory(folderPath);
                            }

                            File.WriteAllText(filePath, noArticles);
                            break;

                    }

                }

                objLayout.Template = File.ReadAllText(filePath);
                objLayout.Tokens = objLayout.Template.Split(delimiter);

                DataCache.SetCache(cacheKey, objLayout, new CacheDependency(filePath));

            }

            return objLayout;

        }

        public void UpdateLayout(LatestLayoutType type , int moduleID , string content){

            string folderPath = HttpContext.Current.Server.MapPath(@"~\DesktopModules\GcDesign-NewsArticles - LatestArticles\Templates\" + moduleID.ToString());
            string filePath = HttpContext.Current.Server.MapPath(@"~\DesktopModules\GcDesign-NewsArticles - LatestArticles\Templates\" + moduleID.ToString() + @"\" + type.ToString().Replace("_", "."));

            if(!Directory.Exists(folderPath)) {
                Directory.CreateDirectory(folderPath);
            }

            File.WriteAllText(filePath, content);

            string cacheKey = "LatestArticles-" + moduleID.ToString() + "-" + type.ToString();
            DataCache.RemoveCache(cacheKey);

        }

#endregion
    }
}