using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;

using DotNetNuke;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using GcDesign.NewsArticles.Components.CustomFields;

namespace GcDesign.NewsArticles
{
    public class ArticleInfo
    {
        #region " Private Members "

        // 成员变量
        int _articleID;
        int _authorID;
        int _approverID;
        DateTime _createdDate;
        DateTime _lastUpdate;
        string _title;
        string _summary;
        string _articleText;
        bool _isApproved;
        bool _isDraft;
        int _numberOfViews;
        DateTime _startDate;
        DateTime _endDate;
        int _moduleID;
        bool _isFeatured;
        double _rating;
        int _ratingCount;
        int _lastUpdateID;
        bool _isSecure;
        bool _isNewWindow;

        string _metaTitle;
        string _metaDescription;
        string _metaKeywords;
        string _pageHeadText;
        string _shortUrl;
        string _rssGuid;

        string _imageUrl;
        string _url;

        string _authorEmail;
        string _authorUserName;
        string _authorFirstName;
        string _authorLastName;
        string _authorDisplayName;

        string _lastUpdateEmail;
        string _lastUpdateUserName;
        string _lastUpdateFirstName;
        string _lastUpdateLastName;
        string _lastUpdateDisplayName;

        string _body;
        int _pageCount;
        int _commentCount;
        int _fileCount;
        int _imageCount;
        string _imageUrlResolved;

        Hashtable _customList;

        string _tags;

        UserInfo _approver;

        #endregion

        #region " Private Methods "

        private void InitializePropertyList()
        {
            //添加缓存
            CustomFieldController objCustomFieldController = new CustomFieldController();
            ArrayList objCustomFields = objCustomFieldController.List(ModuleID);

            CustomValueController objCustomValueController = new CustomValueController();
            List<CustomValueInfo> objCustomValues = objCustomValueController.List(ArticleID);

            _customList = new Hashtable();
            foreach (CustomFieldInfo objCustomField in objCustomFields)
            {
                string value = "";
                foreach (CustomValueInfo objCustomValue in objCustomValues)
                {
                    if (objCustomValue.CustomFieldID == objCustomField.CustomFieldID)
                    {
                        value = objCustomValue.CustomValue;
                    }

                }
                _customList.Add(objCustomField.CustomFieldID, value);
            }
        }

        #endregion

        #region " Public Properties "

        public int ArticleID
        {
            get
            {
                return _articleID;
            }
            set
            {
                _articleID = value;
            }
        }

        public int AuthorID
        {
            get
            {
                return _authorID;
            }
            set
            {
                _authorID = value;
            }
        }

        public int ApproverID
        {
            get
            {
                return _approverID;
            }
            set
            {
                _approverID = value;
            }
        }

        public DateTime CreatedDate
        {
            get
            {
                return _createdDate;
            }
            set
            {
                _createdDate = value;
            }
        }

        public DateTime LastUpdate
        {
            get
            {
                return _lastUpdate;
            }
            set
            {
                _lastUpdate = value;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        public string Summary
        {
            get
            {
                return _summary;
            }
            set
            {
                _summary = value;
            }
        }

        public string ArticleText
        {
            get
            {
                return _articleText;
            }
            set
            {
                _articleText = value;
            }
        }

        public bool IsApproved
        {
            get
            {
                return _isApproved;
            }
            set
            {
                _isApproved = value;
            }
        }

        public bool IsDraft
        {
            get
            {
                return _isDraft;
            }
            set
            {
                _isDraft = value;
            }
        }

        public int NumberOfViews
        {
            get
            {
                return _numberOfViews;
            }
            set
            {
                _numberOfViews = value;
            }
        }


        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }

        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
            }
        }

        public int ModuleID
        {
            get
            {
                return _moduleID;
            }
            set
            {
                _moduleID = value;
            }
        }

        public string ImageUrl
        {
            get
            {
                return _imageUrl;
            }
            set
            {
                _imageUrl = value;
            }
        }

        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
            }
        }

        public bool IsFeatured
        {
            get
            {
                return _isFeatured;
            }
            set
            {
                _isFeatured = value;
            }
        }

        public double Rating
        {
            get
            {
                return _rating;
            }
            set
            {
                _rating = value;
            }
        }

        public int RatingCount
        {
            get
            {
                return _ratingCount;
            }
            set
            {
                _ratingCount = value;
            }
        }

        public int LastUpdateID
        {
            get
            {
                return _lastUpdateID;
            }
            set
            {
                _lastUpdateID = value;
            }
        }

        public bool IsSecure
        {
            get
            {
                return _isSecure;
            }
            set
            {
                _isSecure = value;
            }
        }

        public bool IsNewWindow
        {
            get
            {
                return _isNewWindow;
            }
            set
            {
                _isNewWindow = value;
            }
        }


        public string MetaTitle
        {
            get
            {
                return _metaTitle;
            }
            set
            {
                _metaTitle = value;
            }
        }

        public string MetaDescription
        {
            get
            {
                return _metaDescription;
            }
            set
            {
                _metaDescription = value;
            }
        }


        public string MetaKeywords
        {
            get
            {
                return _metaKeywords;
            }
            set
            {
                _metaKeywords = value;
            }
        }

        public string PageHeadText
        {
            get
            {
                return _pageHeadText;
            }
            set
            {
                _pageHeadText = value;
            }
        }


        public string ShortUrl
        {
            get
            {
                return _shortUrl;
            }
            set
            {
                _shortUrl = value;
            }
        }

        public string RssGuid
        {
            get
            {
                return _rssGuid;
            }
            set
            {
                _rssGuid = value;
            }
        }

        public string AuthorUserName
        {
            get
            {
                return _authorUserName;
            }
            set
            {
                _authorUserName = value;
            }
        }


        public string AuthorEmail
        {
            get
            {
                return _authorEmail;
            }
            set
            {
                _authorEmail = value;
            }
        }

        public string AuthorFirstName
        {
            get
            {
                return _authorFirstName;
            }
            set
            {
                _authorFirstName = value;
            }
        }


        public string AuthorLastName
        {
            get
            {
                return _authorLastName;
            }
            set
            {
                _authorLastName = value;
            }
        }


        public string AuthorDisplayName
        {
            get
            {
                return _authorDisplayName;
            }
            set
            {
                _authorDisplayName = value;
            }
        }

        public StatusType Status
        {
            get
            {
                if (IsDraft)
                {
                    return StatusType.Draft;
                }
                if (!IsApproved)
                {
                    return StatusType.AwaitingApproval;
                }
                else
                {
                    return StatusType.Published;
                }
            }
            set
            {
                switch (value)
                {
                    case StatusType.Draft:
                        _isDraft = true;
                        _isApproved = false;
                        break;
                    case StatusType.AwaitingApproval:
                        _isDraft = false;
                        _isApproved = false;
                        break;
                    case StatusType.Published:
                        _isDraft = false;
                        _isApproved = true;
                        break;
                    default:
                        break;
                }
            }
        }

        public string AuthorFullName
        {
            get
            {
                return _authorFirstName + " " + _authorLastName;
            }
        }

        public string LastUpdateUserName
        {
            get
            {
                return _lastUpdateUserName;
            }
            set
            {
                _lastUpdateUserName = value;
            }
        }

        public string LastUpdateEmail
        {
            get
            {
                return _lastUpdateEmail;
            }
            set
            {
                _lastUpdateEmail = value;
            }
        }


        public string LastUpdateFirstName
        {
            get
            {
                return _lastUpdateFirstName;
            }
            set
            {
                _lastUpdateFirstName = value;
            }
        }

        public string LastUpdateLastName
        {
            get
            {
                return _lastUpdateLastName;
            }
            set
            {
                _lastUpdateLastName = value;
            }
        }

        public string LastUpdateDisplayName
        {
            get
            {
                return _lastUpdateDisplayName;
            }
            set
            {
                _lastUpdateDisplayName = value;
            }
        }


        public string LastUpdateFullName
        {
            get
            {
                return _lastUpdateFirstName + " " + _lastUpdateLastName;
            }
        }

        public string Body
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;
            }
        }


        public int PageCount
        {
            get
            {
                return _pageCount;
            }
            set
            {
                _pageCount = value;
            }
        }

        public int CommentCount
        {
            get
            {
                return _commentCount;
            }
            set
            {
                _commentCount = value;
            }
        }

        public int FileCount
        {
            get
            {
                return _fileCount;
            }
            set
            {
                _fileCount = value;
            }
        }


        public int ImageCount
        {
            get
            {
                return _imageCount;
            }
            set
            {
                _imageCount = value;
            }
        }


        public Hashtable CustomList
        {
            get
            {
                if (_customList == null)
                {
                    InitializePropertyList();
                }
                return _customList;
            }
        }

        public string Tags
        {
            get
            {
                return _tags;
            }
            set
            {
                _tags = value;
            }
        }

        public UserInfo Approver(int portalID)
        {
            if (_approver == null && ApproverID != Null.NullInteger)
            {
                UserController objUserController = new UserController();
                _approver = objUserController.GetUser(portalID, ApproverID);
            }
            return _approver;
        }

        public string TitleAlternate
        {
            get
            {
                return "[" + StartDate.Year.ToString() + "-" + StartDate.Month.ToString() + "-" + StartDate.Day.ToString() + "] " + Title;
            }

        }

        #endregion

    }
}