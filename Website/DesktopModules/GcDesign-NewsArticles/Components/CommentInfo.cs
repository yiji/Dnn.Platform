using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke;
using DotNetNuke.Common.Utilities;

namespace GcDesign.NewsArticles
{
    public class CommentInfo
    {
        #region " Private Methods "

        // local property declarations
        int _commentID;
        int _articleID;
        int _userID;
        string _comment;
        DateTime _createdDate;
        string _remoteAddress;
        int _type;
        string _trackbackUrl;
        string _trackbackTitle;
        string _trackbackBlogName;
        string _trackbackExcerpt;
        string _anonymousName;
        string _anonymousEmail;
        string _anonymousURL;
        bool _notifyMe = Null.NullBoolean;
        bool _isApproved = Null.NullBoolean;
        int _approvedBy;

        string _authorEmail;
        string _authorUserName;
        string _authorFirstName;
        string _authorLastName;
        string _authorDisplayName;

        #endregion

        #region " public Properties "

        public int CommentID
        {
            get
            {
                return _commentID;
            }
            set
            {
                _commentID = value;
            }
        }

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

        public int UserID
        {
            get
            {
                return _userID;
            }
            set
            {
                _userID = value;
            }
        }


        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
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

        public string RemoteAddress
        {
            get
            {
                return _remoteAddress;
            }
            set
            {
                _remoteAddress = value;
            }
        }

        public int Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public string TrackbackUrl
        {
            get
            {
                return _trackbackUrl;
            }
            set
            {
                _trackbackUrl = value;
            }
        }

        public string TrackbackTitle
        {
            get
            {
                return _trackbackTitle;
            }
            set
            {
                _trackbackTitle = value;
            }
        }

        public string TrackbackBlogName
        {
            get
            {
                return _trackbackBlogName;
            }
            set
            {
                _trackbackBlogName = value;
            }
        }

        public string TrackbackExcerpt
        {
            get
            {
                return _trackbackExcerpt;
            }
            set
            {
                _trackbackExcerpt = value;
            }
        }

        public bool NotifyMe
        {
            get
            {
                return _notifyMe;
            }
            set
            {
                _notifyMe = value;
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

        public int ApprovedBy
        {
            get
            {
                return _approvedBy;
            }
            set
            {
                _approvedBy = value;
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

        public string AnonymousName
        {
            get
            {
                return _anonymousName;
            }
            set
            {
                _anonymousName = value;
            }
        }

        public string AnonymousEmail
        {
            get
            {
                return _anonymousEmail;
            }
            set
            {
                _anonymousEmail = value;
            }
        }

        public string AnonymousURL
        {
            get
            {
                return _anonymousURL;
            }
            set
            {
                _anonymousURL = value;
            }
        }

        #endregion

    }
}