using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using GcDesign.NewsArticles.Components.Types;

namespace GcDesign.NewsArticles
{
    public class ArticleSettings
    {
        #region " Private Members "

        private Hashtable _settings;
        private PortalSettings _portalSettings;
        private ModuleInfo _moduleConfiguration;

        #endregion

        #region " Constructors "

        public ArticleSettings(Hashtable settings, PortalSettings portalSettings, ModuleInfo moduleConfiguration)
        {
            _settings = settings;
            _portalSettings = portalSettings;
            _moduleConfiguration = moduleConfiguration;
        }

        #endregion

        #region " Private Properties "

        public Hashtable Settings
        {
            get
            {
                return _settings;
            }
        }

        #endregion

        #region " Private Methods "

        private bool IsInRoles(string roles)
        {

            // Replacement for core IsInRoles check because it doesn't auto-pass super users.

            if (roles != null)
            {
                HttpContext context = HttpContext.Current;
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();


                foreach (string role in roles.Split(';'))
                {
                    if (role != "" && role != null &&
                            ((context.Request.IsAuthenticated == false &&
                              role == DotNetNuke.Common.Globals.glbRoleUnauthUserName) || role == DotNetNuke.Common.Globals.glbRoleAllUsersName ||
                              objUserInfo.IsInRole(role) == true))
                    {
                        return true;
                    }
                }

            }

            return false;

        }

        #endregion

        #region " public Properties "

        public bool AlwaysShowPageID
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SEO_ALWAYS_SHOW_PAGEID_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.SEO_ALWAYS_SHOW_PAGEID_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public int Author
        {
            get
            {
                if (Settings.Contains(ArticleConstants.AUTHOR_SETTING))
                {
                    if (Numeric.IsNumeric(Settings[ArticleConstants.AUTHOR_SETTING].ToString()))
                    {
                        return Convert.ToInt32(Settings[ArticleConstants.AUTHOR_SETTING].ToString());
                    }
                    else
                    {
                        return Null.NullInteger;
                    }
                }
                else
                {
                    return Null.NullInteger;
                }
            }
        }

        public int AuthorDefault
        {
            get
            {
                if (Settings.Contains(ArticleConstants.AUTHOR_DEFAULT_SETTING))
                {
                    if (Numeric.IsNumeric(Settings[ArticleConstants.AUTHOR_DEFAULT_SETTING].ToString()))
                    {
                        return Convert.ToInt32(Settings[ArticleConstants.AUTHOR_DEFAULT_SETTING].ToString());
                    }
                    else
                    {
                        return Null.NullInteger;
                    }
                }
                else
                {
                    return Null.NullInteger;
                }
            }
        }

        public AuthorSelectType AuthorSelect
        {
            get
            {
                if (Settings.Contains(ArticleConstants.AUTHOR_SELECT_TYPE))
                {
                    return (AuthorSelectType)System.Enum.Parse(typeof(AuthorSelectType), Settings[ArticleConstants.AUTHOR_SELECT_TYPE].ToString());
                }
                else
                {
                    return AuthorSelectType.ByDropdown;
                }
            }
        }

        public bool AuthorUserIDFilter
        {
            get
            {
                if (Settings.Contains(ArticleConstants.AUTHOR_USERID_FILTER_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.AUTHOR_USERID_FILTER_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public string AuthorUserIDParam
        {
            get
            {
                if (Settings.Contains(ArticleConstants.AUTHOR_USERID_PARAM_SETTING))
                {
                    return Settings[ArticleConstants.AUTHOR_USERID_PARAM_SETTING].ToString();
                }
                else
                {
                    return "ID";
                }
            }
        }

        public bool AuthorUsernameFilter
        {
            get
            {
                if (Settings.Contains(ArticleConstants.AUTHOR_USERNAME_FILTER_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.AUTHOR_USERNAME_FILTER_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public string AuthorUsernameParam
        {
            get
            {
                if (Settings.Contains(ArticleConstants.AUTHOR_USERNAME_PARAM_SETTING))
                {
                    return Settings[ArticleConstants.AUTHOR_USERNAME_PARAM_SETTING].ToString();
                }
                else
                {
                    return "Username";
                }
            }
        }

        public bool AuthorLoggedInUserFilter
        {
            get
            {
                if (Settings.Contains(ArticleConstants.AUTHOR_LOGGED_IN_USER_FILTER_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.AUTHOR_LOGGED_IN_USER_FILTER_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public bool ArchiveAuthor
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ARCHIVE_AUTHOR_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ARCHIVE_AUTHOR_SETTING].ToString());
                }
                else
                {
                    return ArticleConstants.ARCHIVE_AUTHOR_SETTING_DEFAULT;
                }
            }
        }

        public bool ArchiveCategories
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ARCHIVE_CATEGORIES_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ARCHIVE_CATEGORIES_SETTING].ToString());
                }
                else
                {
                    return ArticleConstants.ARCHIVE_CATEGORIES_SETTING_DEFAULT;
                }
            }
        }

        public bool ArchiveCurrentArticles
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ARCHIVE_CURRENT_ARTICLES_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ARCHIVE_CURRENT_ARTICLES_SETTING].ToString());
                }
                else
                {
                    return ArticleConstants.ARCHIVE_CURRENT_ARTICLES_SETTING_DEFAULT;
                }
            }
        }

        public bool ArchiveMonth
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ARCHIVE_MONTH_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ARCHIVE_MONTH_SETTING].ToString());
                }
                else
                {
                    return ArticleConstants.ARCHIVE_MONTH_SETTING_DEFAULT;
                }
            }
        }

        public bool BubbleFeatured
        {
            get
            {
                if (Settings.Contains(ArticleConstants.BUBBLE_FEATURED_ARTICLES))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.BUBBLE_FEATURED_ARTICLES].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public bool CategoryBreadcrumb
        {
            get
            {
                if (Settings.Contains(ArticleConstants.CATEGORY_BREADCRUMB_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.CATEGORY_BREADCRUMB_SETTING].ToString());
                }
                else
                {
                    return ArticleConstants.CATEGORY_BREADCRUMB_SETTING_DEFAULT;
                }
            }
        }

        //jiang 文章Breadcrumb
        public bool ArticleBreadcrumb
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ARTICLE_BREADCRUMB_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ARTICLE_BREADCRUMB_SETTING].ToString());
                }
                else
                {
                    return ArticleConstants.ARTICLE_BREADCRUMB_SETTING_DEFAULT;
                }
            }
        }

        public bool CategoryFilterSubmit
        {
            get{
                if (Settings.Contains(ArticleConstants.CATEGORY_FILTER_SUBMIT_SETTING)) {
                    return Convert.ToBoolean(Settings[ArticleConstants.CATEGORY_FILTER_SUBMIT_SETTING].ToString());
                }else{
                    return ArticleConstants.CATEGORY_FILTER_SUBMIT_SETTING_DEFAULT;
                }
            }
        }

        public bool IncludeInPageName
        {
            get
            {
                if (Settings.Contains(ArticleConstants.CATEGORY_NAME_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.CATEGORY_NAME_SETTING].ToString());
                }
                else
                {
                    return ArticleConstants.CATEGORY_NAME_SETTING_DEFAULT;
                }
            }
        }

        public int CategorySelectionHeight
        {
            get
            {
                if (Settings.Contains(ArticleConstants.CATEGORY_SELECTION_HEIGHT_SETTING))
                {
                    return Convert.ToInt32(Settings[ArticleConstants.CATEGORY_SELECTION_HEIGHT_SETTING].ToString());
                }
                else
                {
                    return ArticleConstants.CATEGORY_SELECTION_HEIGHT_DEFAULT;
                }
            }
        }

        public CategorySortType CategorySortType
        {
            get
            {
                if (Settings.Contains(ArticleConstants.CATEGORY_SORT_SETTING))
                {
                    try
                    {
                        return (CategorySortType)System.Enum.Parse(typeof(CategorySortType), Settings[ArticleConstants.CATEGORY_SORT_SETTING].ToString());
                    }
                    catch
                    {
                        return ArticleConstants.CATEGORY_SORT_SETTING_DEFAULT;
                    }
                }
                else
                {
                    return ArticleConstants.CATEGORY_SORT_SETTING_DEFAULT;
                }
            }
        }

        public string CommentAkismetKey
        {
            get
            {
                if (Settings.Contains(ArticleConstants.COMMENT_AKISMET_SETTING))
                {
                    return Settings[ArticleConstants.COMMENT_AKISMET_SETTING].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public bool CommentModeration
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ENABLE_COMMENT_MODERATION_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_COMMENT_MODERATION_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public bool CommentRequireName
        {
            get
            {
                if (Settings.Contains(ArticleConstants.COMMENT_REQUIRE_NAME_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.COMMENT_REQUIRE_NAME_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool CommentRequireEmail
        {
            get
            {
                if (Settings.Contains(ArticleConstants.COMMENT_REQUIRE_EMAIL_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.COMMENT_REQUIRE_EMAIL_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public string ContentSharingPortals
        {
            get
            {
                if (Settings.Contains(ArticleConstants.CONTENT_SHARING_SETTING))
                {
                    return Settings[ArticleConstants.CONTENT_SHARING_SETTING].ToString();
                }
                else
                {
                    return Null.NullString;
                }
            }
        }

        public int DefaultImagesFolder
        {
            get
            {
                if (Settings.Contains(ArticleConstants.DEFAULT_IMAGES_FOLDER_SETTING))
                {
                    if (Numeric.IsNumeric(Settings[ArticleConstants.DEFAULT_IMAGES_FOLDER_SETTING].ToString()))
                    {
                        return Convert.ToInt32(Settings[ArticleConstants.DEFAULT_IMAGES_FOLDER_SETTING].ToString());
                    }
                    else
                    {
                        return Null.NullInteger;
                    }
                }
                else
                {
                    return Null.NullInteger;
                }
            }
        }

        public int DefaultFilesFolder
        {
            get
            {
                if (Settings.Contains(ArticleConstants.DEFAULT_FILES_FOLDER_SETTING))
                {
                    if (Numeric.IsNumeric(Settings[ArticleConstants.DEFAULT_FILES_FOLDER_SETTING].ToString()))
                    {
                        return Convert.ToInt32(Settings[ArticleConstants.DEFAULT_FILES_FOLDER_SETTING].ToString());
                    }
                    else
                    {
                        return Null.NullInteger;
                    }
                }
                else
                {
                    return Null.NullInteger;
                }
            }
        }

        public bool EnableActiveSocialFeed
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ACTIVE_SOCIAL_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ACTIVE_SOCIAL_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public bool JournalIntegration
        {
            get
            {
                if (Settings.Contains(ArticleConstants.JOURNAL_INTEGRATION_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.JOURNAL_INTEGRATION_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool JournalIntegrationGroups
        {
            get
            {
                if (Settings.Contains(ArticleConstants.JOURNAL_INTEGRATION_GROUPS_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.JOURNAL_INTEGRATION_GROUPS_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public string ActiveSocialSubmitKey
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ACTIVE_SOCIAL_SUBMIT_SETTING))
                {
                    return Settings[ArticleConstants.ACTIVE_SOCIAL_SUBMIT_SETTING].ToString();
                }
                else
                {
                    return "9F02B914-F565-4E5A-9194-8423431056CD";
                }
            }
        }

        public string ActiveSocialCommentKey
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ACTIVE_SOCIAL_COMMENT_SETTING))
                {
                    return Settings[ArticleConstants.ACTIVE_SOCIAL_COMMENT_SETTING].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public string ActiveSocialRateKey
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ACTIVE_SOCIAL_RATE_SETTING))
                {
                    return Settings[ArticleConstants.ACTIVE_SOCIAL_RATE_SETTING].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public bool EnableAutoTrackback
        {
            get{
                if (Settings.Contains(ArticleConstants.ENABLE_AUTO_TRACKBACK_SETTING)) {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_AUTO_TRACKBACK_SETTING].ToString());
                }else{
                    return true;
                }
            }
        }

        public bool EnableCoreSearch
        {
            get{
                if (Settings.Contains(ArticleConstants.ENABLE_CORE_SEARCH_SETTING)) {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_CORE_SEARCH_SETTING].ToString());
                }else{
                    return false;
                }
            }
        }

        public bool EnableExternalImages
        {
            get{
                if (Settings.Contains(ArticleConstants.ENABLE_EXTERNAL_IMAGES_SETTING)) {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_EXTERNAL_IMAGES_SETTING].ToString());
                }else{
                    return true;
                }
            }
        }

        public bool EnableNotificationPing
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ENABLE_NOTIFICATION_PING_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_NOTIFICATION_PING_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EnableImagesUpload
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ENABLE_UPLOAD_IMAGES_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_UPLOAD_IMAGES_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool EnablePortalImages
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ENABLE_PORTAL_IMAGES_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_PORTAL_IMAGES_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool EnableRatings
        {
            get
            {
                if (EnableRatingsAnonymous || EnableRatingsAuthenticated)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EnableRatingsAnonymous
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ENABLE_RATINGS_ANONYMOUS_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_RATINGS_ANONYMOUS_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool EnableRatingsAuthenticated
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ENABLE_RATINGS_AUTHENTICATED_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_RATINGS_AUTHENTICATED_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool EnableSmartThinkerStoryFeed
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SMART_THINKER_STORY_FEED_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.SMART_THINKER_STORY_FEED_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public bool EnableSyndication
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ENABLE_SYNDICATION_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_SYNDICATION_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool EnableSyndicationEnclosures
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ENABLE_SYNDICATION_ENCLOSURES_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_SYNDICATION_ENCLOSURES_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool EnableSyndicationHtml
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ENABLE_SYNDICATION_HTML_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_SYNDICATION_HTML_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public bool ExpandSummary
        {
            get
            {
                if (Settings.Contains(ArticleConstants.EXPAND_SUMMARY_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.EXPAND_SUMMARY_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public bool FeaturedOnly
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SHOW_FEATURED_ONLY_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.SHOW_FEATURED_ONLY_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public int[] FilterCategories
        {
            get
            {
                if (Settings.Contains(ArticleConstants.CATEGORIES_SETTING + _moduleConfiguration.TabID.ToString()))
                {
                    if (Settings[ArticleConstants.CATEGORIES_SETTING + _moduleConfiguration.TabID.ToString()].ToString() != Null.NullString && Settings[ArticleConstants.CATEGORIES_SETTING + _moduleConfiguration.TabID.ToString()].ToString() != "-1")
                    {
                        string[] categories = Settings[ArticleConstants.CATEGORIES_SETTING + _moduleConfiguration.TabID.ToString()].ToString().Split(',');
                        List<int> cats = new List<int>();

                        foreach (string category in categories)
                        {
                            if (Numeric.IsNumeric(category))
                            {
                                cats.Add(Convert.ToInt32(category));
                            }
                        }

                        return cats.ToArray();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }


        public int FilterSingleCategory
        {
            get
            {
                if (Settings.Contains(ArticleConstants.CATEGORIES_FILTER_SINGLE_SETTING))
                {
                    if (Numeric.IsNumeric(Settings[ArticleConstants.CATEGORIES_FILTER_SINGLE_SETTING].ToString()))
                    {
                        return Convert.ToInt32(Settings[ArticleConstants.CATEGORIES_FILTER_SINGLE_SETTING]);
                    }
                    else
                    {
                        return Null.NullInteger;
                    }
                }
                else
                {
                    return Null.NullInteger;
                }
            }
        }


        public string ImageJQueryPath
        {
            get
            {
                if (Settings.Contains(ArticleConstants.IMAGE_JQUERY_PATH))
                {
                    return Settings[ArticleConstants.IMAGE_JQUERY_PATH].ToString();
                }
                else
                {
                    return "includes/fancybox/jquery.fancybox.pack.js";
                }
            }
        }

        public ThumbnailType ImageThumbnailType
        {
            get{
                if (Settings.Contains(ArticleConstants.IMAGE_THUMBNAIL_SETTING)) {
                    return (ThumbnailType)System.Enum.Parse(typeof(ThumbnailType), Settings[ArticleConstants.IMAGE_THUMBNAIL_SETTING].ToString());
                }else{
                    return ArticleConstants.DEFAULT_IMAGE_THUMBNAIL;
                }
            }
        }

        public bool IncludeJQuery
        {
            get
            {
                if (Settings.Contains(ArticleConstants.INCLUDE_JQUERY_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.INCLUDE_JQUERY_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public MatchOperatorType MatchCategories
        {
            get
            {
                if (Settings.Contains(ArticleConstants.MATCH_OPERATOR_SETTING))
                {
                    return (MatchOperatorType)System.Enum.Parse(typeof(MatchOperatorType), Settings[ArticleConstants.MATCH_OPERATOR_SETTING].ToString());
                }
                else
                {
                    return MatchOperatorType.MatchAny;
                }
            }
        }

        public int MaxArticles
        {
            get
            {
                if (Settings.Contains(ArticleConstants.MAX_ARTICLES_SETTING))
                {
                    if (Numeric.IsNumeric(Settings[ArticleConstants.MAX_ARTICLES_SETTING].ToString()))
                    {
                        return Convert.ToInt32(Settings[ArticleConstants.MAX_ARTICLES_SETTING].ToString());
                    }
                    else
                    {
                        return Null.NullInteger;
                    }
                }
                else
                {
                    return Null.NullInteger;
                }
            }
        }

        public int MaxAge
        {
            get
            {
                if (Settings.Contains(ArticleConstants.MAX_AGE_SETTING))
                {
                    if (Numeric.IsNumeric(Settings[ArticleConstants.MAX_AGE_SETTING].ToString()))
                    {
                        return Convert.ToInt32(Settings[ArticleConstants.MAX_AGE_SETTING].ToString());
                    }
                    else
                    {
                        return Null.NullInteger;
                    }
                }
                else
                {
                    return Null.NullInteger;
                }
            }
        }

        public int MaxImageHeight
        {
            get
            {
                if (Settings.Contains(ArticleConstants.IMAGE_MAX_HEIGHT_SETTING))
                {
                    if (Numeric.IsNumeric(Settings[ArticleConstants.IMAGE_MAX_HEIGHT_SETTING].ToString()))
                    {
                        return Convert.ToInt32(Settings[ArticleConstants.IMAGE_MAX_HEIGHT_SETTING].ToString());
                    }
                    else
                    {
                        return ArticleConstants.DEFAULT_IMAGE_MAX_HEIGHT;
                    }
                }
                else
                {
                    return ArticleConstants.DEFAULT_IMAGE_MAX_HEIGHT;
                }
            }
        }

        public int MaxImageWidth
        {
            get
            {
                if (Settings.Contains(ArticleConstants.IMAGE_MAX_WIDTH_SETTING))
                {
                    if (Numeric.IsNumeric(Settings[ArticleConstants.IMAGE_MAX_WIDTH_SETTING].ToString()))
                    {
                        return Convert.ToInt32(Settings[ArticleConstants.IMAGE_MAX_WIDTH_SETTING].ToString());
                    }
                    else
                    {
                        return ArticleConstants.DEFAULT_IMAGE_MAX_WIDTH;
                    }
                }
                else
                {
                    return ArticleConstants.DEFAULT_IMAGE_MAX_WIDTH;
                }
            }
        }

        public MenuPositionType MenuPosition
        {
            get
            {
                if (Settings.Contains(ArticleConstants.MENU_POSITION_TYPE))
                {
                    return (MenuPositionType)System.Enum.Parse(typeof(MenuPositionType), Settings[ArticleConstants.MENU_POSITION_TYPE].ToString());
                }
                else
                {
                    return MenuPositionType.Top;
                }
            }
        }

        public bool NotFeaturedOnly
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SHOW_NOT_FEATURED_ONLY_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.SHOW_NOT_FEATURED_ONLY_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public bool NotifyAuthorOnComment
        {
            get
            {
                if (Settings.Contains(ArticleConstants.NOTIFY_COMMENT_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.NOTIFY_COMMENT_SETTING]);
                }
                else
                {
                    return true;
                }
            }
        }

        public bool NotifyAuthorOnApproval
        {
            get
            {
                if (Settings.Contains(ArticleConstants.NOTIFY_APPROVAL_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.NOTIFY_APPROVAL_SETTING]);
                }
                else
                {
                    return true;
                }
            }
        }

        public bool NotifyDefault
        {
            get
            {
                if (Settings.Contains(ArticleConstants.NOTIFY_DEFAULT_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.NOTIFY_DEFAULT_SETTING]);
                }
                else
                {
                    return false;
                }
            }
        }

        public string NotifyEmailOnComment
        {
            get
            {
                if (Settings.Contains(ArticleConstants.NOTIFY_COMMENT_SETTING_EMAIL))
                {
                    return Settings[ArticleConstants.NOTIFY_COMMENT_SETTING_EMAIL].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public bool NotifyApproverForCommentApproval
        {
            get
            {
                if (Settings.Contains(ArticleConstants.NOTIFY_COMMENT_APPROVAL_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.NOTIFY_COMMENT_APPROVAL_SETTING]);
                }
                else
                {
                    return true;
                }
            }
        }

        public string NotifyEmailForCommentApproval
        {
            get
            {
                if (Settings.Contains(ArticleConstants.NOTIFY_COMMENT_APPROVAL_EMAIL_SETTING))
                {
                    return Settings[ArticleConstants.NOTIFY_COMMENT_APPROVAL_EMAIL_SETTING].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public bool NotSecuredOnly
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SHOW_NOT_SECURED_ONLY_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.SHOW_NOT_SECURED_ONLY_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public bool ProcessPostTokens
        {
            get
            {
                if (Settings.Contains(ArticleConstants.PROCESS_POST_TOKENS))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.PROCESS_POST_TOKENS].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool ResizeImages
        {
            get
            {
                if (Settings.Contains(ArticleConstants.IMAGE_RESIZE_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.IMAGE_RESIZE_SETTING].ToString());
                }
                else
                {
                    return ArticleConstants.DEFAULT_IMAGE_RESIZE;
                }
            }
        }

        public bool SecuredOnly
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SHOW_SECURED_ONLY_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.SHOW_SECURED_ONLY_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public string ShortenedID
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SEO_SHORTEN_ID_SETTING))
                {
                    return Settings[ArticleConstants.SEO_SHORTEN_ID_SETTING].ToString();
                }
                else
                {
                    return "ID";
                }
            }
        }

        public string SortBy
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SORT_BY))
                {
                    return Settings[ArticleConstants.SORT_BY].ToString();
                }
                else
                {
                    return ArticleConstants.DEFAULT_SORT_BY;
                }
            }
        }

        public string SortDirection
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SORT_DIRECTION))
                {
                    return Settings[ArticleConstants.SORT_DIRECTION].ToString();
                }
                else
                {
                    return ArticleConstants.DEFAULT_SORT_DIRECTION;
                }
            }
        }

        public int SortDirectionComments
        {
            get{
                if (Settings.Contains(ArticleConstants.COMMENT_SORT_DIRECTION_SETTING)) {
                    return Convert.ToInt32(Settings[ArticleConstants.COMMENT_SORT_DIRECTION_SETTING].ToString());
                }else{
                    return 0;
                }
            }
        }

        public SyndicationLinkType SyndicationLinkType
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SYNDICATION_LINK_TYPE))
                {
                    return (SyndicationLinkType)System.Enum.Parse(typeof(SyndicationLinkType), Settings[ArticleConstants.SYNDICATION_LINK_TYPE].ToString());
                }
                else
                {
                    return SyndicationLinkType.Article;
                }
            }
        }

        public SyndicationEnclosureType SyndicationEnclosureType
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SYNDICATION_ENCLOSURE_TYPE))
                {
                    return (SyndicationEnclosureType)System.Enum.Parse(typeof(SyndicationEnclosureType), Settings[ArticleConstants.SYNDICATION_ENCLOSURE_TYPE].ToString());
                }
                else
                {
                    return SyndicationEnclosureType.Attachment;
                }
            }
        }

        public int SyndicationSummaryLength
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SYNDICATION_SUMMARY_LENGTH))
                {
                    return Convert.ToInt32(Settings[ArticleConstants.SYNDICATION_SUMMARY_LENGTH].ToString());
                }
                else
                {
                    return Null.NullInteger;
                }
            }
        }

        public int SyndicationMaxCount
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SYNDICATION_MAX_COUNT))
                {
                    return Convert.ToInt32(Settings[ArticleConstants.SYNDICATION_MAX_COUNT].ToString());
                }
                else
                {
                    return 50;
                }
            }
        }

        public string SyndicationImagePath
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SYNDICATION_IMAGE_PATH))
                {
                    return Settings[ArticleConstants.SYNDICATION_IMAGE_PATH].ToString();
                }
                else
                {
                    return "~/DesktopModules/GcDesign-NewsArticles/Images/rssbutton.gif";
                }
            }
        }

        public bool UseCanonicalLink
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SEO_USE_CANONICAL_LINK_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.SEO_USE_CANONICAL_LINK_SETTING]);
                }
                else
                {
                    return true;
                }
            }
        }

        public bool ExpandMetaInformation
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SEO_EXPAND_META_INFORMATION_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.SEO_EXPAND_META_INFORMATION_SETTING]);
                }
                else
                {
                    return false;
                }
            }
        }

        public bool UniquePageTitles
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SEO_UNIQUE_PAGE_TITLES_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.SEO_UNIQUE_PAGE_TITLES_SETTING]);
                }
                else
                {
                    return true;
                }
            }
        }

        public bool UseCaptcha
        {
            get
            {
                if (Settings.Contains(ArticleConstants.USE_CAPTCHA_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.USE_CAPTCHA_SETTING]);
                }
                else
                {
                    return false;
                }
            }
        }

        public UrlModeType UrlModeType
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SEO_URL_MODE_SETTING))
                {
                    try
                    {
                        return (UrlModeType)System.Enum.Parse(typeof(UrlModeType), Settings[ArticleConstants.SEO_URL_MODE_SETTING].ToString());
                    }
                    catch
                    {
                        return UrlModeType.Shorterned;
                    }
                }
                else
                {
                    return UrlModeType.Shorterned;
                }
            }
        }

        public bool WatermarkEnabled
        {
            get
            {
                if (_settings.Contains(ArticleConstants.IMAGE_WATERMARK_ENABLED_SETTING))
                {
                    return Convert.ToBoolean(_settings[ArticleConstants.IMAGE_WATERMARK_ENABLED_SETTING].ToString());
                }
                else
                {
                    return ArticleConstants.IMAGE_WATERMARK_ENABLED_SETTING_DEFAULT;
                }
            }
        }

        public string WatermarkText
        {
            get
            {
                if (_settings.Contains(ArticleConstants.IMAGE_WATERMARK_TEXT_SETTING))
                {
                    return _settings[ArticleConstants.IMAGE_WATERMARK_TEXT_SETTING].ToString();
                }
                else
                {
                    return ArticleConstants.IMAGE_WATERMARK_TEXT_SETTING_DEFAULT;
                }
            }
        }

        public string WatermarkImage
        {
            get
            {
                if (_settings.Contains(ArticleConstants.IMAGE_WATERMARK_IMAGE_SETTING))
                {
                    return _settings[ArticleConstants.IMAGE_WATERMARK_IMAGE_SETTING].ToString();
                }
                else
                {
                    return ArticleConstants.IMAGE_WATERMARK_IMAGE_SETTING_DEFAULT;
                }
            }
        }

        public WatermarkPosition WatermarkPosition
        {
            get{
                if (_settings.Contains(ArticleConstants.IMAGE_WATERMARK_IMAGE_POSITION_SETTING)) {
                    try{
                        return (WatermarkPosition)System.Enum.Parse(typeof(WatermarkPosition), _settings[ArticleConstants.IMAGE_WATERMARK_IMAGE_POSITION_SETTING].ToString());
                    }
                    catch
                    {
                        return ArticleConstants.IMAGE_WATERMARK_IMAGE_POSITION_SETTING_DEFAULT;
                    }
                }
                else{
                    return ArticleConstants.IMAGE_WATERMARK_IMAGE_POSITION_SETTING_DEFAULT;
                }
            }
        }

        public int PageSize
        {
            get
            {
                if (Settings.Contains("Number"))
                {
                    if (Convert.ToInt32(Settings["Number"].ToString()) > 0)
                    {
                        return Convert.ToInt32(Settings["Number"].ToString());
                    }
                    else
                    {
                        if (Convert.ToInt32(Settings["Number"].ToString()) == -1)
                        {
                            return 10000;
                        }
                        else
                        {
                            return 10;
                        }
                    }
                }
                else
                {
                    return 10;
                }
            }
        }

        public bool IsCommentsAnonymous
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ENABLE_ANONYMOUS_COMMENTS_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_ANONYMOUS_COMMENTS_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsCommentsEnabled
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ENABLE_COMMENTS_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_COMMENTS_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsCommentModerationEnabled
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ENABLE_COMMENT_MODERATION_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_COMMENT_MODERATION_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsCommentWebsiteHidden
        {
            get
            {
                if (Settings.Contains(ArticleConstants.COMMENT_HIDE_WEBSITE_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.COMMENT_HIDE_WEBSITE_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsRateable
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated)
                {
                    return EnableRatingsAuthenticated;
                }
                else
                {
                    return EnableRatingsAnonymous;
                }
            }
        }

        public bool IsSyndicationEnabled
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ENABLE_SYNDICATION_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_SYNDICATION_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsIncomingTrackbackEnabled
        {
            get
            {
                if (Settings.Contains(ArticleConstants.ENABLE_INCOMING_TRACKBACK_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.ENABLE_INCOMING_TRACKBACK_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool LaunchLinks
        {
            get
            {
                if (Settings.Contains(ArticleConstants.LAUNCH_LINKS))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.LAUNCH_LINKS].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public DisplayType DisplayMode
        {
            get
            {
                if (Settings.Contains(ArticleConstants.DISPLAY_MODE))
                {
                    return (DisplayType)System.Enum.Parse(typeof(DisplayType), Settings[ArticleConstants.DISPLAY_MODE].ToString());
                }
                else
                {
                    return DisplayType.FullName;
                }
            }
        }

        public RelatedType RelatedMode
        {
            get
            {
                if (Settings.Contains(ArticleConstants.RELATED_MODE))
                {
                    return (RelatedType)System.Enum.Parse(typeof(RelatedType), Settings[ArticleConstants.RELATED_MODE].ToString());
                }
                else
                {
                    return RelatedType.MatchCategoriesAnyTagsAny;
                }
            }
        }

        public string Template
        {
            get
            {
                if (Settings.Contains(ArticleConstants.TEMPLATE_SETTING))
                {
                    return Settings[ArticleConstants.TEMPLATE_SETTING].ToString();
                }
                else
                {
                    return "Standard";
                }
            }
        }

        public bool SEOTitleSpecified
        {
            get
            {
                return Settings.Contains(ArticleConstants.SEO_TITLE_SETTING);
            }
        }

        public bool ShowPending
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SHOW_PENDING_SETTING))
                {
                    return Convert.ToBoolean(Settings[ArticleConstants.SHOW_PENDING_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public TitleReplacementType TitleReplacement
        {
            get
            {
                if (Settings.Contains(ArticleConstants.TITLE_REPLACEMENT_TYPE))
                {
                    return (TitleReplacementType)System.Enum.Parse(typeof(TitleReplacementType), Settings[ArticleConstants.TITLE_REPLACEMENT_TYPE].ToString());
                }
                else
                {
                    return TitleReplacementType.Dash;
                }
            }
        }

        public string TextEditorWidth
        {
            get
            {
                if (Settings.Contains(ArticleConstants.TEXT_EDITOR_WIDTH))
                {
                    return Settings[ArticleConstants.TEXT_EDITOR_WIDTH].ToString();
                }
                else
                {
                    return "100%";
                }
            }
        }

        public string TextEditorHeight
        {
            get
            {
                if (Settings.Contains(ArticleConstants.TEXT_EDITOR_HEIGHT))
                {
                    return Settings[ArticleConstants.TEXT_EDITOR_HEIGHT].ToString();
                }
                else
                {
                    return "400";
                }
            }
        }

        public TextEditorModeType TextEditorSummaryMode
        {
            get
            {
                if (Settings.Contains(ArticleConstants.TEXT_EDITOR_SUMMARY_MODE))
                {
                    return (TextEditorModeType)System.Enum.Parse(typeof(TextEditorModeType), Settings[ArticleConstants.TEXT_EDITOR_SUMMARY_MODE].ToString());
                }
                else
                {
                    return TextEditorModeType.Rich;
                }
            }
        }

        public string TextEditorSummaryWidth
        {
            get
            {
                if (Settings.Contains(ArticleConstants.TEXT_EDITOR_SUMMARY_WIDTH))
                {
                    return Settings[ArticleConstants.TEXT_EDITOR_SUMMARY_WIDTH].ToString();
                }
                else
                {
                    return "100%";
                }
            }
        }

        public string TextEditorSummaryHeight
        {
            get
            {
                if (Settings.Contains(ArticleConstants.TEXT_EDITOR_SUMMARY_HEIGHT))
                {
                    return Settings[ArticleConstants.TEXT_EDITOR_SUMMARY_HEIGHT].ToString();
                }
                else
                {
                    return "400";
                }
            }
        }

        public string TwitterName
        {
            get
            {
                if (Settings.Contains(ArticleConstants.TWITTER_USERNAME))
                {
                    return Settings[ArticleConstants.TWITTER_USERNAME].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public string TwitterBitLyLogin
        {
            get
            {
                if (Settings.Contains(ArticleConstants.TWITTER_BITLY_LOGIN))
                {
                    return Settings[ArticleConstants.TWITTER_BITLY_LOGIN].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public string TwitterBitLyAPIKey
        {
            get
            {
                if (Settings.Contains(ArticleConstants.TWITTER_BITLY_API_KEY))
                {
                    return Settings[ArticleConstants.TWITTER_BITLY_API_KEY].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public int ServerTimeZone
        {
            get
            {
                if (Settings.Contains(ArticleConstants.SERVER_TIMEZONE))
                {
                    if (Numeric.IsNumeric(Settings[ArticleConstants.SERVER_TIMEZONE].ToString()))
                    {
                        return Convert.ToInt32(Settings[ArticleConstants.SERVER_TIMEZONE].ToString());
                    }
                    else
                    {
                        return _portalSettings.TimeZoneOffset;
                    }
                }
                else
                {
                    return _portalSettings.TimeZoneOffset;
                }
            }
        }




        public string TemplatePath
        {
            get
            {
                return _portalSettings.HomeDirectoryMapPath + "GcDesign-NewsArticles/Templates/" + Template + "/";
            }
        }

        public string SecureUrl
        {
            get
            {
                if (Settings.Contains(ArticleConstants.PERMISSION_SECURE_URL_SETTING))
                {
                    return Settings[ArticleConstants.PERMISSION_SECURE_URL_SETTING].ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public int RoleGroupID
        {
            get
            {
                if (Settings.Contains(ArticleConstants.PERMISSION_ROLE_GROUP_ID))
                {
                    return Convert.ToInt32(Settings[ArticleConstants.PERMISSION_ROLE_GROUP_ID].ToString());
                }
                else
                {
                    return -1;
                }
            }
        }

        public bool IsApprover
        {
            get
            {
                if (HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (IsAdmin)
                {
                    return true;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_APPROVAL_SETTING))
                {
                    return PortalSecurity.IsInRoles(Settings[ArticleConstants.PERMISSION_APPROVAL_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsCategoriesEnabled
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_CATEGORIES_SETTING))
                {
                    return IsInRoles(Settings[ArticleConstants.PERMISSION_CATEGORIES_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsExcerptEnabled
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_EXCERPT_SETTING))
                {
                    return IsInRoles(Settings[ArticleConstants.PERMISSION_EXCERPT_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsImagesEnabled
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_IMAGE_SETTING))
                {
                    return IsInRoles(Settings[ArticleConstants.PERMISSION_IMAGE_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsFilesEnabled
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_FILE_SETTING))
                {
                    return IsInRoles(Settings[ArticleConstants.PERMISSION_FILE_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsLinkEnabled
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_LINK_SETTING))
                {
                    return IsInRoles(Settings[ArticleConstants.PERMISSION_LINK_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsFeaturedEnabled
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_FEATURE_SETTING))
                {
                    return PortalSecurity.IsInRoles(Settings[ArticleConstants.PERMISSION_FEATURE_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsSecureEnabled
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_SECURE_SETTING))
                {
                    return PortalSecurity.IsInRoles(Settings[ArticleConstants.PERMISSION_SECURE_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsPublishEnabled
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_PUBLISH_SETTING))
                {
                    return PortalSecurity.IsInRoles(Settings[ArticleConstants.PERMISSION_PUBLISH_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsExpiryEnabled
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_EXPIRY_SETTING))
                {
                    return PortalSecurity.IsInRoles(Settings[ArticleConstants.PERMISSION_EXPIRY_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsMetaEnabled
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_META_SETTING))
                {
                    if (Settings[ArticleConstants.PERMISSION_META_SETTING].ToString() == "")
                    {
                        return false;
                    }
                    return PortalSecurity.IsInRoles(Settings[ArticleConstants.PERMISSION_META_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsCustomEnabled
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_CUSTOM_SETTING))
                {
                    if (Settings[ArticleConstants.PERMISSION_CUSTOM_SETTING].ToString() == "")
                    {
                        return false;
                    }
                    return PortalSecurity.IsInRoles(Settings[ArticleConstants.PERMISSION_CUSTOM_SETTING].ToString());
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsAutoApprover
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (IsAdmin)
                {
                    return true;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_AUTO_APPROVAL_SETTING))
                {
                    return PortalSecurity.IsInRoles(Settings[ArticleConstants.PERMISSION_AUTO_APPROVAL_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsAutoApproverComment
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (IsAdmin)
                {
                    return true;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_AUTO_APPROVAL_COMMENT_SETTING))
                {
                    return PortalSecurity.IsInRoles(Settings[ArticleConstants.PERMISSION_AUTO_APPROVAL_COMMENT_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsAutoFeatured
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_AUTO_FEATURE_SETTING))
                {

                    foreach (string role in Settings[ArticleConstants.PERMISSION_AUTO_FEATURE_SETTING].ToString().Split(';'))
                    {
                        if (role != "")
                        {
                            if (UserController.Instance.GetCurrentUserInfo().IsInRole(role))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsAutoSecured
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_AUTO_SECURE_SETTING))
                {

                    foreach (string role in Settings[ArticleConstants.PERMISSION_AUTO_SECURE_SETTING].ToString().Split(';'))
                    {
                        if (role != "")
                        {
                            if (UserController.Instance.GetCurrentUserInfo().IsInRole(role))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsSubmitter
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                if (IsAdmin)
                {
                    return true;
                }

                if (Settings.Contains(ArticleConstants.PERMISSION_SUBMISSION_SETTING))
                {
                    return PortalSecurity.IsInRoles(Settings[ArticleConstants.PERMISSION_SUBMISSION_SETTING].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsAdmin
        {
            get
            {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
                {
                    return false;
                }

                bool blnHasModuleEditPermissions = PortalSecurity.IsInRoles(_moduleConfiguration.AuthorizedEditRoles);

                if (blnHasModuleEditPermissions == false)
                {
                    blnHasModuleEditPermissions = PortalSecurity.IsInRoles(_portalSettings.ActiveTab.AdministratorRoles);
                }

                if (blnHasModuleEditPermissions == false)
                {
                    blnHasModuleEditPermissions = PortalSecurity.IsInRoles(_portalSettings.AdministratorRoleName);
                }

                return blnHasModuleEditPermissions;
            }
        }

        #endregion
    }
}