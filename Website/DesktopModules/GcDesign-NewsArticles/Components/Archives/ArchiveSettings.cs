using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GcDesign.NewsArticles
{
    public class ArchiveSettings
    {
        #region " 私有属性 "

        private Hashtable Settings;

#endregion

#region " Constructors "

        public ArchiveSettings(Hashtable moduleSettings ){
            Settings = moduleSettings;
            }

#endregion

#region " Public Properties "

        public  AuthorSortByType AuthorSortBy{
        get{
            if(Settings.Contains(ArticleConstants.NEWS_ARCHIVES_AUTHOR_SORT_BY)){
            return (AuthorSortByType)System.Enum.Parse(typeof(AuthorSortByType),Settings[ArticleConstants.NEWS_ARCHIVES_AUTHOR_SORT_BY].ToString());
            }
            return ArticleConstants.NEWS_ARCHIVES_AUTHOR_SORT_BY_DEFAULT;
        }
        }

        public bool CategoryHideZeroCategories{
            get{
                if(Settings.Contains(ArticleConstants.NEWS_ARCHIVES_HIDE_ZERO_CATEGORIES)){
                    return Convert.ToBoolean(Settings[ArticleConstants.NEWS_ARCHIVES_HIDE_ZERO_CATEGORIES].ToString());
                }
                return ArticleConstants.NEWS_ARCHIVES_HIDE_ZERO_CATEGORIES_DEFAULT;
            }
        }
        
        public int CategoryMaxDepth{
            get{
                if(Settings.Contains(ArticleConstants.NEWS_ARCHIVES_MAX_DEPTH)){
                    if(Numeric.IsNumeric(Settings[ArticleConstants.NEWS_ARCHIVES_MAX_DEPTH].ToString())){
                        return Convert.ToInt32(Settings[ArticleConstants.NEWS_ARCHIVES_MAX_DEPTH].ToString());
                    }
                }
                return ArticleConstants.NEWS_ARCHIVES_MAX_DEPTH_DEFAULT;
            }
        }

        public int CategoryParent{
            get{
                if(Settings.Contains(ArticleConstants.NEWS_ARCHIVES_PARENT_CATEGORY)){
                    if(Numeric.IsNumeric(Settings[ArticleConstants.NEWS_ARCHIVES_PARENT_CATEGORY].ToString())){
                        return Convert.ToInt32(Settings[ArticleConstants.NEWS_ARCHIVES_PARENT_CATEGORY].ToString());
                    }
                }
                return ArticleConstants.NEWS_ARCHIVES_PARENT_CATEGORY_DEFAULT;
            }
        }

        public GroupByType GroupBy{
            get{
                if(Settings.Contains(ArticleConstants.NEWS_ARCHIVES_GROUP_BY)){
                    return (GroupByType)System.Enum.Parse(typeof(GroupByType), Settings[ArticleConstants.NEWS_ARCHIVES_GROUP_BY].ToString());
                }
                return ArticleConstants.NEWS_ARCHIVES_GROUP_BY_DEFAULT;
            }
        }
        
        public int ItemsPerRow{
            get{
                if(Settings.Contains(ArticleConstants.NEWS_ARCHIVES_ITEMS_PER_ROW)){
                    if(Numeric.IsNumeric(Settings[ArticleConstants.NEWS_ARCHIVES_ITEMS_PER_ROW].ToString())){
                        return Convert.ToInt32(Settings[ArticleConstants.NEWS_ARCHIVES_ITEMS_PER_ROW].ToString());
                    }
                }
                return ArticleConstants.NEWS_ARCHIVES_ITEMS_PER_ROW_DEFAULT;
            }
        }

        public LayoutModeType LayoutMode{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_LAYOUT_MODE)) {
                    return (LayoutModeType)System.Enum.Parse(typeof(LayoutModeType), Settings[ArticleConstants.NEWS_ARCHIVES_LAYOUT_MODE].ToString());
                }
                return ArticleConstants.NEWS_ARCHIVES_LAYOUT_MODE_DEFAULT;
            }
        }

        public ArchiveModeType Mode{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_MODE)) {
                    return (ArchiveModeType)System.Enum.Parse(typeof(ArchiveModeType), Settings[ArticleConstants.NEWS_ARCHIVES_MODE].ToString());
                }
                return ArticleConstants.NEWS_ARCHIVES_MODE_DEFAULT;
            }
        }

        public int ModuleId{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_MODULE_ID)){
                    if (Numeric.IsNumeric(Settings[ArticleConstants.NEWS_ARCHIVES_MODULE_ID].ToString())){
                        return Convert.ToInt32(Settings[ArticleConstants.NEWS_ARCHIVES_MODULE_ID].ToString());
                    }
                }
                return ArticleConstants.NEWS_ARCHIVES_MODULE_ID_DEFAULT;
           }
        }

        public int TabId{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_TAB_ID)) {
                    if (Numeric.IsNumeric(Settings[ArticleConstants.NEWS_ARCHIVES_TAB_ID].ToString())) {
                        return Convert.ToInt32(Settings[ArticleConstants.NEWS_ARCHIVES_TAB_ID].ToString());
                    }
                }
                return ArticleConstants.NEWS_ARCHIVES_TAB_ID_DEFAULT;
            }
        }

        public object TemplateAuthorAdvancedBody{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_BODY_ADVANCED)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_BODY_ADVANCED].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_AUTHOR_HTML_BODY_ADVANCED;
            }
        }

        public object TemplateAuthorAdvancedFooter{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_FOOTER_ADVANCED)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_FOOTER_ADVANCED].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_AUTHOR_HTML_FOOTER_ADVANCED;
}
}

        public object TemplateAuthorAdvancedHeader{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_HEADER_ADVANCED)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_HEADER_ADVANCED].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_AUTHOR_HTML_HEADER_ADVANCED;
            }
        }

        public object TemplateAuthorBody{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_BODY)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_BODY].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_AUTHOR_HTML_BODY;
            }
        }

        public object TemplateAuthorFooter{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_FOOTER)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_FOOTER].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_AUTHOR_HTML_FOOTER;
                }
        }

        public object TemplateAuthorHeader{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_HEADER)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_AUTHOR_HTML_HEADER].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_AUTHOR_HTML_HEADER;
            }
        }

        public object TemplateCategoryAdvancedBody{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_BODY_ADVANCED)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_BODY_ADVANCED].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_CATEGORY_HTML_BODY_ADVANCED;
}
        }

        public object TemplateCategoryAdvancedFooter{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_FOOTER_ADVANCED)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_FOOTER_ADVANCED].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_CATEGORY_HTML_FOOTER_ADVANCED;
            }
        }

        public object TemplateCategoryAdvancedHeader{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_HEADER_ADVANCED)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_HEADER_ADVANCED].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_CATEGORY_HTML_HEADER_ADVANCED;
            }
        }

        public object TemplateCategoryBody{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_BODY)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_BODY].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_CATEGORY_HTML_BODY;
            }
        }

        public object TemplateCategoryFooter{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_FOOTER)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_FOOTER].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_CATEGORY_HTML_FOOTER;
            }
        }

        public object TemplateCategoryHeader{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_HEADER)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_CATEGORY_HTML_HEADER].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_CATEGORY_HTML_HEADER;
            }
        }

        public object TemplateDateAdvancedBody{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_BODY_ADVANCED)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_BODY_ADVANCED].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_HTML_BODY_ADVANCED;
            }
        }

        public object TemplateDateAdvancedFooter{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_FOOTER_ADVANCED)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_FOOTER_ADVANCED].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_HTML_FOOTER_ADVANCED;
            }
        }

        public object TemplateDateAdvancedHeader{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_HEADER_ADVANCED)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_HEADER_ADVANCED].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_HTML_HEADER_ADVANCED;
            }
        }

        public object TemplateDateBody{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_BODY)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_BODY].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_HTML_BODY;
            }
        }

        public object TemplateDateFooter{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_FOOTER)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_FOOTER].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_HTML_FOOTER;
            }
        }

        public object TemplateDateHeader{
            get{
                if (Settings.Contains(ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_HEADER)) {
                    return Settings[ArticleConstants.NEWS_ARCHIVES_SETTING_HTML_HEADER].ToString();
                }
                return ArticleConstants.NEWS_ARCHIVES_DEFAULT_HTML_HEADER;
            }
        }

#endregion
    }
}