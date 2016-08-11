using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GcDesign.NewsArticles
{
    public class ArticleConstants
    {
        #region " 常量 Constants "

        //Token Settings
        public const string DEFAULT_TEMPLATE = "Standard";

        // Basic Settings
        public const string EXPAND_SUMMARY_SETTING = "ExpandSummary";

        public const string ENABLE_RATINGS_AUTHENTICATED_SETTING = "EnableRatings";
        public const string ENABLE_RATINGS_ANONYMOUS_SETTING = "EnableAnonymousRatings";

        public const string JOURNAL_INTEGRATION_SETTING = "JournalIntegration";
        public const string JOURNAL_INTEGRATION_GROUPS_SETTING = "JournalIntegrationGroups";

        public const string ACTIVE_SOCIAL_SETTING = "ActiveSocialFeed";
        public const string ACTIVE_SOCIAL_SUBMIT_SETTING = "ActiveSocialFeedSubmit";
        public const string ACTIVE_SOCIAL_COMMENT_SETTING = "ActiveSocialFeedComment";
        public const string ACTIVE_SOCIAL_RATE_SETTING = "ActiveSocialFeedRate";
        public const string AUTHOR_SELECT_TYPE = "AuthorSelectType";
        public const string ENABLE_CORE_SEARCH_SETTING = "EnableCoreSearch";
        public const string ENABLE_SYNDICATION_SETTING = "EnableSyndication";
        public const string ENABLE_SYNDICATION_ENCLOSURES_SETTING = "EnableSyndicationEnclosures";
        public const string ENABLE_SYNDICATION_HTML_SETTING = "EnableSyndicationHtml";
        public const string ENABLE_NOTIFICATION_PING_SETTING = "NotificationPing";
        public const string ENABLE_AUTO_TRACKBACK_SETTING = "AutoTrackback";
        public const string ENABLE_INCOMING_TRACKBACK_SETTING = "IncomingTrackback";
        public const string PAGE_SIZE_SETTING = "Number";
        public const string LAUNCH_LINKS = "LaunchLinks";
        public const string BUBBLE_FEATURED_ARTICLES = "BubbleFeaturedArticles";
        public const string REQUIRE_CATEGORY = "RequireCategory";
        public const string DISPLAY_MODE = "DisplayMode";
        public const string PROCESS_POST_TOKENS = "ProcessPostTokens";
        public const string RELATED_MODE = "RelatedMode";
        public const string TEMPLATE_SETTING = "Template";
        public const string ENABLE_SEO_TITLE_SETTING = "EnableSEOTitle";
        public const string SEO_TITLE_SETTING = "SEOTitle";
        public const string TITLE_REPLACEMENT_TYPE = "TitleReplacementType";
        public const string SMART_THINKER_STORY_FEED_SETTING = "EnableSmartThinkerStoryFeed";
        public const string TEXT_EDITOR_WIDTH = "TextEditorWidth";
        public const string TEXT_EDITOR_HEIGHT = "TextEditorHeight";
        public const string TEXT_EDITOR_SUMMARY_MODE = "TextEditorSummaryMode";
        public const string TEXT_EDITOR_SUMMARY_WIDTH = "TextEditorSummaryWidth";
        public const string TEXT_EDITOR_SUMMARY_HEIGHT = "TextEditorSummaryHeight";
        public const string SERVER_TIMEZONE = "ServerTimeZone";
        public const string SORT_BY = "SortBy";
        public const string SORT_DIRECTION = "SortDirection";
        public const string SYNDICATION_LINK_TYPE = "SyndicationLinkType";
        public const string SYNDICATION_SUMMARY_LENGTH = "SyndicationSummaryLength";
        public const string SYNDICATION_MAX_COUNT = "SyndicationMaxCount";
        public const string SYNDICATION_ENCLOSURE_TYPE = "SyndicationEnclosureType";
        public const string SYNDICATION_IMAGE_PATH = "SyndicationImagePath";
        public const string MENU_POSITION_TYPE = "MenuPositionType";

        // Image Settings
        public const string INCLUDE_JQUERY_SETTING = "IncludeJQuery";
        public const string IMAGE_JQUERY_PATH = "JQueryPath";
        public const string ENABLE_EXTERNAL_IMAGES_SETTING = "EnableImagesExternal";
        public const string ENABLE_UPLOAD_IMAGES_SETTING = "EnableImagesUpload";
        public const string ENABLE_PORTAL_IMAGES_SETTING = "EnableImages";
        public const string DEFAULT_IMAGES_FOLDER_SETTING = "DefaultImagesFolder";
        public const string DEFAULT_FILES_FOLDER_SETTING = "DefaultFilesFolder";
        public const string IMAGE_RESIZE_SETTING = "ResizeImages";
        public const string IMAGE_THUMBNAIL_SETTING = "ImageThumbnailType";
        public const string IMAGE_MAX_WIDTH_SETTING = "ImageMaxWidth";
        public const string IMAGE_MAX_HEIGHT_SETTING = "ImageMaxHeight";

        public const string IMAGE_WATERMARK_ENABLED_SETTING = "ImageWatermarkEnabled";
        public const bool IMAGE_WATERMARK_ENABLED_SETTING_DEFAULT = false;
        public const string IMAGE_WATERMARK_TEXT_SETTING = "ImageWatermarkText";
        public const string IMAGE_WATERMARK_TEXT_SETTING_DEFAULT = "";
        public const string IMAGE_WATERMARK_IMAGE_SETTING = "ImageWatermarkImage";
        public const string IMAGE_WATERMARK_IMAGE_SETTING_DEFAULT = "";
        public const string IMAGE_WATERMARK_IMAGE_POSITION_SETTING = "ImageWatermarkImagePosition";
        public const WatermarkPosition IMAGE_WATERMARK_IMAGE_POSITION_SETTING_DEFAULT = WatermarkPosition.BottomRight;

        public const bool DEFAULT_IMAGE_RESIZE = true;
        public const ThumbnailType DEFAULT_IMAGE_THUMBNAIL  = ThumbnailType.Proportion;
        public const int DEFAULT_IMAGE_MAX_WIDTH = 600;
        public const int DEFAULT_IMAGE_MAX_HEIGHT = 480;
        public const int DEFAULT_THUMBNAIL_HEIGHT = 100;
        public const int DEFAULT_THUMBNAIL_WIDTH = 100;

        // Category Settings
        public const string ARCHIVE_CURRENT_ARTICLES_SETTING = "ArchiveCurrentArticles";
        public const bool ARCHIVE_CURRENT_ARTICLES_SETTING_DEFAULT  = true;
        public const string ARCHIVE_CATEGORIES_SETTING = "ArchiveCategories";
        public const bool ARCHIVE_CATEGORIES_SETTING_DEFAULT = true;
        public const string ARCHIVE_AUTHOR_SETTING = "ArchiveAuthor";
        public const bool ARCHIVE_AUTHOR_SETTING_DEFAULT = true;
        public const string ARCHIVE_MONTH_SETTING = "ArchiveMonth";
        public const bool ARCHIVE_MONTH_SETTING_DEFAULT = true;

        // Category Settings
        public const string DEFAULT_CATEGORIES_SETTING = "DefaultCategories";
        public const string CATEGORY_SELECTION_HEIGHT_SETTING = "CategorySelectionHeight";
        public const int CATEGORY_SELECTION_HEIGHT_DEFAULT = 150;
        public const string CATEGORY_BREADCRUMB_SETTING = "CategoryBreadcrumb";
        public const string ARTICLE_BREADCRUMB_SETTING = "ArticleBreadcrumb";
        public const bool CATEGORY_BREADCRUMB_SETTING_DEFAULT = true;
        public const bool ARTICLE_BREADCRUMB_SETTING_DEFAULT = false;
        public const string CATEGORY_NAME_SETTING = "CategoryName";
        public const bool CATEGORY_NAME_SETTING_DEFAULT = true;
        public const string CATEGORY_FILTER_SUBMIT_SETTING = "CategoryFilterSubmit";
        public const bool CATEGORY_FILTER_SUBMIT_SETTING_DEFAULT = false;
        public const string CATEGORY_SORT_SETTING = "CategorySortType";
        public const CategorySortType CATEGORY_SORT_SETTING_DEFAULT = CategorySortType.SortOrder;

        // Category Security Settings
        public const string PERMISSION_CATEGORY_VIEW_SETTING = "PermissionCategoryView";
        public const string PERMISSION_CATEGORY_SUBMIT_SETTING = "PermissionCategorySubmit";

        // Comment Settings
        public const string ENABLE_COMMENTS_SETTING = "EnableComments";
        public const string ENABLE_ANONYMOUS_COMMENTS_SETTING = "EnableAnonymousComments";
        public const string ENABLE_COMMENT_MODERATION_SETTING = "EnableCommentModeration";
        public const string COMMENT_HIDE_WEBSITE_SETTING = "CommentHideWebsite";
        public const string COMMENT_REQUIRE_NAME_SETTING = "CommentRequireName";
        public const string COMMENT_REQUIRE_EMAIL_SETTING = "CommentRequireEmail";
        public const string USE_CAPTCHA_SETTING = "UseCaptcha";
        public const string NOTIFY_DEFAULT_SETTING = "NotifyDefault";
        public const string COMMENT_SORT_DIRECTION_SETTING = "CommentSortDirection";
        public const string COMMENT_AKISMET_SETTING = "CommentAkismet";

        // Content Sharing Settings
        public const string CONTENT_SHARING_SETTING = "ContentSharingPortals";

        // Filter Settings
        public const string MAX_ARTICLES_SETTING = "MaxArticles";
        public const string MAX_AGE_SETTING = "MaxArticlesAge";
        public const string CATEGORIES_SETTING = "Categories";
        public const string CATEGORIES_FILTER_SINGLE_SETTING = "CategoriesFilterSingle";
        public const string SHOW_PENDING_SETTING = "ShowPending";
        public const string SHOW_FEATURED_ONLY_SETTING = "ShowFeaturedOnly";
        public const string SHOW_NOT_FEATURED_ONLY_SETTING = "ShowNotFeaturedOnly";
        public const string SHOW_SECURED_ONLY_SETTING = "ShowSecuredOnly";
        public const string SHOW_NOT_SECURED_ONLY_SETTING = "ShowNotSecuredOnly";
        public const string AUTHOR_SETTING = "Author";
        public const string AUTHOR_DEFAULT_SETTING = "AuthorDefault";
        public const string MATCH_OPERATOR_SETTING = "MatchOperator";

        public const string AUTHOR_USERID_FILTER_SETTING = "AuthorUserIDFilter";
        public const string AUTHOR_USERID_PARAM_SETTING = "AuthorUserIDParam";
        public const string AUTHOR_USERNAME_FILTER_SETTING = "AuthorUsernameFilter";
        public const string AUTHOR_USERNAME_PARAM_SETTING = "AuthorUsernameParam";
        public const string AUTHOR_LOGGED_IN_USER_FILTER_SETTING = "AuthorLoggedInUserFilter";

        //栏目显示设置
        public const string CATEGORIES_Show_SETTING = "CategoriesShow";
        public const string CATEGORIES_Show_FILTER_SINGLE_SETTING = "CategoriesShowFilterSingle";

        // Security Settings
        public const string PERMISSION_ROLE_GROUP_ID = "RoleGroupIDFilter";
        public const string PERMISSION_SECURE_SETTING = "SecureRoles";
        public const string PERMISSION_SECURE_URL_SETTING = "SecureUrl";
        public const string PERMISSION_AUTO_SECURE_SETTING = "AutoSecureRoles";
        public const string PERMISSION_SUBMISSION_SETTING = "SubmissionRoles";
        public const string PERMISSION_APPROVAL_SETTING = "ApprovalRoles";
        public const string PERMISSION_AUTO_APPROVAL_SETTING = "AutoApprovalRoles";
        public const string PERMISSION_AUTO_APPROVAL_COMMENT_SETTING = "AutoApprovalCommentRoles";
        public const string PERMISSION_FEATURE_SETTING = "FeatureRoles";
        public const string PERMISSION_AUTO_FEATURE_SETTING = "AutoFeatureRoles";

        // Security Form Settings
        public const string PERMISSION_CATEGORIES_SETTING = "PermissionCategoriesRoles";
        public const string PERMISSION_EXCERPT_SETTING = "PermissionExcerptRoles";
        public const string PERMISSION_IMAGE_SETTING = "PermissionImageRoles";
        public const string PERMISSION_FILE_SETTING = "PermissionFileRoles";
        public const string PERMISSION_LINK_SETTING = "PermissionLinkRoles";
        public const string PERMISSION_PUBLISH_SETTING = "PermissionPublishRoles";
        public const string PERMISSION_EXPIRY_SETTING = "PermissionExpiryRoles";
        public const string PERMISSION_META_SETTING = "PermissionMetaRoles";
        public const string PERMISSION_CUSTOM_SETTING = "PermissionCustomRoles";

        // Admin Settings
        public const string PERMISSION_SITE_TEMPLATES_SETTING = "PermissionSiteTemplates";

        // Notification Settings
        public const string NOTIFY_SUBMISSION_SETTING = "NotifySubmission";
        public const string NOTIFY_SUBMISSION_SETTING_EMAIL = "NotifySubmissionEmail";
        public const string NOTIFY_APPROVAL_SETTING = "NotifyApproval";
        public const string NOTIFY_COMMENT_SETTING = "NotifyComment";
        public const string NOTIFY_COMMENT_SETTING_EMAIL = "NotifyCommentEmail";
        public const string NOTIFY_COMMENT_APPROVAL_SETTING = "NotifyCommentApproval";
        public const string NOTIFY_COMMENT_APPROVAL_EMAIL_SETTING = "NotifyCommentApprovalEmail";

        // SEO Settings
        public const string SEO_ALWAYS_SHOW_PAGEID_SETTING = "AlwaysShowPageID";
        public const string SEO_URL_MODE_SETTING = "SEOUrlMode";
        public const string SEO_SHORTEN_ID_SETTING = "SEOShorternID";
        public const string SEO_USE_CANONICAL_LINK_SETTING = "SEOUseCanonicalLink";
        public const string SEO_EXPAND_META_INFORMATION_SETTING = "SEOExpandMetaInformation";
        public const string SEO_UNIQUE_PAGE_TITLES_SETTING = "SEOUniquePageTitles";

        // SEO Settings
        public const string TWITTER_USERNAME = "NA-TwitterUsername";
        public const string TWITTER_BITLY_LOGIN = "NA-BitLyLogin";
        public const string TWITTER_BITLY_API_KEY = "NA-BitLyAPI";

        // Latest Articles 
        public const string LATEST_ARTICLES_TAB_ID = "LatestArticlesTabID";
        public const string LATEST_ARTICLES_MODULE_ID = "LatestArticlesModuleID";
        public const string LATEST_ARTICLES_CATEGORIES = "LatestArticlesCategories";
        public const string LATEST_ARTICLES_CATEGORIES_EXCLUDE = "LatestArticlesCategoriesExclude";
        public const string LATEST_ARTICLES_MATCH_OPERATOR = "LatestArticlesMatchOperator";
        public const string LATEST_ARTICLES_TAGS = "LatestArticlesTags";
        public const string LATEST_ARTICLES_TAGS_MATCH_OPERATOR = "LatestArticlesTagsMatchOperator";
        public const string LATEST_ARTICLES_IDS = "LatestArticlesIDS";
        public const string LATEST_ARTICLES_COUNT = "LatestArticlesCount";
        public const string LATEST_ARTICLES_START_POINT = "LatestArticlesStartPoint";
        public const string LATEST_ARTICLES_MAX_AGE = "LatestArticlesMaxAge";
        public const string LATEST_ARTICLES_START_DATE = "LatestArticlesStartDate";
        public const string LATEST_ARTICLES_SHOW_PENDING = "LatestArticlesShowPending";
        public const string LATEST_ARTICLES_SHOW_RELATED = "LatestArticlesShowRelated";
        public const string LATEST_ARTICLES_FEATURED_ONLY = "LatestArticlesFeaturedOnly";
        public const string LATEST_ARTICLES_NOT_FEATURED_ONLY = "LatestArticlesNotFeaturedOnly";
        public const string LATEST_ARTICLES_SECURED_ONLY = "LatestArticlesSecuredOnly";
        public const string LATEST_ARTICLES_NOT_SECURED_ONLY = "LatestArticlesNotSecuredOnly";
        public const string LATEST_ARTICLES_CUSTOM_FIELD_FILTER = "LatestArticlesCustomFieldFilter";
        public const string LATEST_ARTICLES_CUSTOM_FIELD_VALUE = "LatestArticlesCustomFieldValue";
        public const string LATEST_ARTICLES_LINK_FILTER = "LatestArticlesLinkFilter";
        public const string LATEST_ARTICLES_SORT_BY = "LatestArticlesSortBy";
        public const string LATEST_ARTICLES_SORT_DIRECTION = "LatestArticlesSortDirection";
        public const string LATEST_ARTICLES_ITEMS_PER_ROW = "ItemsPerRow";
        public const int LATEST_ARTICLES_ITEMS_PER_ROW_DEFAULT = 1;
        public const string LATEST_ARTICLES_AUTHOR = "LatestArticlesAuthor";
        public const int LATEST_ARTICLES_AUTHOR_DEFAULT = -1;
        public const string LATEST_ARTICLES_QUERY_STRING_FILTER = "LatestArticlesQueryStringFilter";
        public const bool LATEST_ARTICLES_QUERY_STRING_FILTER_DEFAULT  = false;
        public const string LATEST_ARTICLES_QUERY_STRING_PARAM = "LatestArticlesQueryStringParam";
        public const string LATEST_ARTICLES_QUERY_STRING_PARAM_DEFAULT = "ID";
        public const string LATEST_ARTICLES_USERNAME_FILTER = "LatestArticlesUsernameFilter";
        public const bool LATEST_ARTICLES_USERNAME_FILTER_DEFAULT = false;
        public const string LATEST_ARTICLES_USERNAME_PARAM = "LatestArticlesUsernameParam";
        public const string LATEST_ARTICLES_USERNAME_PARAM_DEFAULT = "Username";
        public const string LATEST_ARTICLES_LOGGED_IN_USER_FILTER = "LatestArticlesLoggedInUserFilter";
        public const bool LATEST_ARTICLES_LOGGED_IN_USER_FILTER_DEFAULT  = false;
        public const string LATEST_ARTICLES_INCLUDE_STYLESHEET = "LatestArticlesIncludeStylesheet";
        public const bool LATEST_ARTICLES_INCLUDE_STYLESHEET_DEFAULT = false;

        public const string LATEST_ENABLE_PAGER = "LatestEnablePager";
        public const bool LATEST_ENABLE_PAGER_DEFAULT  = false;
        public const string LATEST_PAGE_SIZE = "LatestPageSize";
        public const int LATEST_PAGE_SIZE_DEFAULT = 10;

        public const string LATEST_ARTICLES_LAYOUT_MODE = "LayoutMode";
        public const LayoutModeType LATEST_ARTICLES_LAYOUT_MODE_DEFAULT = LayoutModeType.Simple;

        public const string SETTING_HTML_HEADER = "HtmlHeader";
        public const string SETTING_HTML_BODY = "HtmlBody";
        public const string SETTING_HTML_FOOTER = "HtmlFooter";

        public const string SETTING_HTML_HEADER_ADVANCED = "HtmlHeaderAdvanced";
        public const string SETTING_HTML_BODY_ADVANCED = "HtmlBodyAdvanced";
        public const string SETTING_HTML_FOOTER_ADVANCED = "HtmlFooterAdvanced";

        public const string SETTING_HTML_NO_ARTICLES = "HtmlNoArticles";

        public const string DEFAULT_HTML_HEADER = "<table cellpadding=0 cellspacing=4>";
        public const string DEFAULT_HTML_BODY = "<TR><TD>[EDIT]<span class=normal><a href='[LINK]'>[TITLE]</a> by [AUTHORUSERNAME]</span></TD></TR><TR><TD><span class=normal>[SUMMARY]</span></TD></TR>";
        public const string DEFAULT_HTML_FOOTER = "</table>";
        public const string DEFAULT_HTML_HEADER_ADVANCED = "";
        public const string DEFAULT_HTML_BODY_ADVANCED = "[EDIT]<span class=normal><a href='[LINK]'>[TITLE]</a> by [AUTHORUSERNAME]</span><br><span class=normal>[SUMMARY]</span>";
        public const string DEFAULT_HTML_FOOTER_ADVANCED = "";
        public const string DEFAULT_HTML_NO_ARTICLES = "No articles match criteria.";
        public const string DEFAULT_SORT_BY = "StartDate";
        public const string DEFAULT_SORT_DIRECTION = "DESC";

        // Latest Comments 
        public const string LATEST_COMMENTS_TAB_ID = "LatestCommentsTabID";
        public const string LATEST_COMMENTS_MODULE_ID = "LatestCommentsModuleID";
        public const string LATEST_COMMENTS_COUNT = "LatestCommentsCount";

        public const string LATEST_COMMENTS_HTML_HEADER = "HtmlHeaderLatestComments";
        public const string LATEST_COMMENTS_HTML_BODY = "HtmlBodyLatestComments";
        public const string LATEST_COMMENTS_HTML_FOOTER = "HtmlFooterLatestComments";
        public const string LATEST_COMMENTS_HTML_NO_COMMENTS = "HtmlNoCommentsLatestComments";
        public const string LATEST_COMMENTS_INCLUDE_STYLESHEET = "LatestCommentsIncludeStylesheet";

        public const string DEFAULT_LATEST_COMMENTS_HTML_HEADER = "<table cellpadding='0' cellspacing='4'>";
        public const string DEFAULT_LATEST_COMMENTS_HTML_BODY = "<TR><TD><span class=normal><a href='[COMMENTLINK]'>[ARTICLETITLE]</a><br /> [COMMENT:50] by [AUTHOR]</span></TD></TR>";
        public const string DEFAULT_LATEST_COMMENTS_HTML_FOOTER = "</table>";
        public const string DEFAULT_LATEST_COMMENTS_HTML_NO_COMMENTS = "No comments match criteria.";
        public const bool DEFAULT_LATEST_COMMENTS_INCLUDE_STYLESHEET = false;

        // News Archives
        public const string NEWS_ARCHIVES_TAB_ID = "NewsArchivesTabID";
        public const int NEWS_ARCHIVES_TAB_ID_DEFAULT = -1;
        public const string NEWS_ARCHIVES_MODULE_ID = "NewsArchivesModuleID";
        public const int NEWS_ARCHIVES_MODULE_ID_DEFAULT = -1;
        public const string NEWS_ARCHIVES_MODE = "NewsArchivesMode";
        public const ArchiveModeType NEWS_ARCHIVES_MODE_DEFAULT = ArchiveModeType.Date;
        public const string NEWS_ARCHIVES_HIDE_ZERO_CATEGORIES = "NewsArchivesHideZeroCategories";
        public const string NEWS_ARCHIVES_PARENT_CATEGORY = "NewsArchivesParentCategory";
        public const int NEWS_ARCHIVES_PARENT_CATEGORY_DEFAULT = -1;
        public const string NEWS_ARCHIVES_MAX_DEPTH = "NewsArchivesMaxDepth";
        public const int NEWS_ARCHIVES_MAX_DEPTH_DEFAULT = -1;
        public const string NEWS_ARCHIVES_AUTHOR_SORT_BY = "NewsArchivesAuthorSortBy";
        public const AuthorSortByType NEWS_ARCHIVES_AUTHOR_SORT_BY_DEFAULT = AuthorSortByType.DisplayName;
        public const bool NEWS_ARCHIVES_HIDE_ZERO_CATEGORIES_DEFAULT = false;
        public const string NEWS_ARCHIVES_GROUP_BY = "GroupBy";
        public const GroupByType NEWS_ARCHIVES_GROUP_BY_DEFAULT = GroupByType.Month;
        public const string NEWS_ARCHIVES_LAYOUT_MODE = "LayoutMode";
        public const LayoutModeType NEWS_ARCHIVES_LAYOUT_MODE_DEFAULT = LayoutModeType.Simple;
        public const string NEWS_ARCHIVES_ITEMS_PER_ROW = "ItemsPerRow";
        public const int NEWS_ARCHIVES_ITEMS_PER_ROW_DEFAULT = 1;

        public const string NEWS_ARCHIVES_SETTING_HTML_HEADER = "NewsArchivesHtmlHeader";
        public const string NEWS_ARCHIVES_SETTING_HTML_BODY = "NewsArchivesHtmlBody";
        public const string NEWS_ARCHIVES_SETTING_HTML_FOOTER = "NewsArchivesHtmlFooter";

        public const string NEWS_ARCHIVES_SETTING_HTML_HEADER_ADVANCED = "NewsArchivesHtmlHeaderAdvanced";
        public const string NEWS_ARCHIVES_SETTING_HTML_BODY_ADVANCED = "NewsArchivesHtmlBodyAdvanced";
        public const string NEWS_ARCHIVES_SETTING_HTML_FOOTER_ADVANCED = "NewsArchivesHtmlFooterAdvanced";

        public const string NEWS_ARCHIVES_SETTING_CATEGORY_HTML_HEADER = "NewsArchivesCategoryHtmlHeader";
        public const string NEWS_ARCHIVES_SETTING_CATEGORY_HTML_BODY = "NewsArchivesCategoryHtmlBody";
        public const string NEWS_ARCHIVES_SETTING_CATEGORY_HTML_FOOTER = "NewsArchivesCategoryHtmlFooter";

        public const string NEWS_ARCHIVES_SETTING_CATEGORY_HTML_HEADER_ADVANCED = "NewsArchivesCategoryHtmlHeaderAdvanced";
        public const string NEWS_ARCHIVES_SETTING_CATEGORY_HTML_BODY_ADVANCED = "NewsArchivesCategoryHtmlBodyAdvanced";
        public const string NEWS_ARCHIVES_SETTING_CATEGORY_HTML_FOOTER_ADVANCED = "NewsArchivesCategoryHtmlFooterAdvanced";

        public const string NEWS_ARCHIVES_SETTING_AUTHOR_HTML_HEADER = "NewsArchivesAuthorHtmlHeader";
        public const string NEWS_ARCHIVES_SETTING_AUTHOR_HTML_BODY = "NewsArchivesAuthorHtmlBody";
        public const string NEWS_ARCHIVES_SETTING_AUTHOR_HTML_FOOTER = "NewsArchivesAuthorHtmlFooter";

        public const string NEWS_ARCHIVES_SETTING_AUTHOR_HTML_HEADER_ADVANCED = "NewsArchivesAuthorHtmlHeaderAdvanced";
        public const string NEWS_ARCHIVES_SETTING_AUTHOR_HTML_BODY_ADVANCED = "NewsArchivesAuthorHtmlBodyAdvanced";
        public const string NEWS_ARCHIVES_SETTING_AUTHOR_HTML_FOOTER_ADVANCED = "NewsArchivesAuthorHtmlFooterAdvanced";

        public const string NEWS_ARCHIVES_DEFAULT_HTML_HEADER = "<table cellpadding=0 cellspacing=4>";
        public const string NEWS_ARCHIVES_DEFAULT_HTML_BODY = "<TR><TD class=normal><a href='[LINK]'>[MONTH] [YEAR] ([COUNT])</a></TD></TR>";
        public const string NEWS_ARCHIVES_DEFAULT_HTML_FOOTER = "</table>";

        public const string NEWS_ARCHIVES_DEFAULT_HTML_HEADER_ADVANCED = "";
        public const string NEWS_ARCHIVES_DEFAULT_HTML_BODY_ADVANCED = "<span class=normal><a href='[LINK]'>[MONTH] [YEAR] ([COUNT])</a></span>";
        public const string NEWS_ARCHIVES_DEFAULT_HTML_FOOTER_ADVANCED = "";

        public const string NEWS_ARCHIVES_DEFAULT_CATEGORY_HTML_HEADER = "<table cellpadding=0 cellspacing=4>";
        public const string NEWS_ARCHIVES_DEFAULT_CATEGORY_HTML_BODY = "<TR><TD class=normal><a href='[LINK]'>[CATEGORY] ([COUNT])</a></TD></TR>";
        public const string NEWS_ARCHIVES_DEFAULT_CATEGORY_HTML_FOOTER = "</table>";

        public const string NEWS_ARCHIVES_DEFAULT_CATEGORY_HTML_HEADER_ADVANCED = "";
        public const string NEWS_ARCHIVES_DEFAULT_CATEGORY_HTML_BODY_ADVANCED = "<span class=normal><a href='[LINK]'>[CATEGORY] ([COUNT])</a></span>";
        public const string NEWS_ARCHIVES_DEFAULT_CATEGORY_HTML_FOOTER_ADVANCED = "";

        public const string NEWS_ARCHIVES_DEFAULT_AUTHOR_HTML_HEADER = "<table cellpadding=0 cellspacing=4>";
        public const string NEWS_ARCHIVES_DEFAULT_AUTHOR_HTML_BODY = "<TR><TD class=normal><a href='[LINK]'>[AUTHORDISPLAYNAME] ([COUNT])</a></TD></TR>";
        public const string NEWS_ARCHIVES_DEFAULT_AUTHOR_HTML_FOOTER = "</table>";

        public const string NEWS_ARCHIVES_DEFAULT_AUTHOR_HTML_HEADER_ADVANCED = "";
        public const string NEWS_ARCHIVES_DEFAULT_AUTHOR_HTML_BODY_ADVANCED = "<span class=normal><a href='[LINK]'>[AUTHORDISPLAYNAME] ([COUNT])</a></span>";
        public const string NEWS_ARCHIVES_DEFAULT_AUTHOR_HTML_FOOTER_ADVANCED = "";

        // News Search

        public const string NEWS_SEARCH_TAB_ID = "NewsSearchTabID";
        public const string NEWS_SEARCH_MODULE_ID = "NewsSearchModuleID";
        public const string News_Search_Window = "NewsSearchWindow";

        // Caching Constants
        public const string CACHE_CATEGORY_ARTICLE = "NewsCategory_Multi_Cache_";
        public const string CACHE_IMAGE_ARTICLE = "NewsImage_Multi_Cache_";
        public const string CACHE_CATEGORY_ARTICLE_NO_LINK = "NewsCategory_Multi_Cache_NoLink";
        public const string CACHE_CATEGORY_ARTICLE_LATEST = "NewsCategory_Multi_Cache_Latest_";

        // News Article

        public const string News_ARTICLES_TAB_ID = "NewsArticlesTabID";
        public const string NEWS_ARTICLES_MODULE_ID = "NewsArticlesModuleID";

#endregion
    }
}