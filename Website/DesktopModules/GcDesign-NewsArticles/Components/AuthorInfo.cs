using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;

namespace GcDesign.NewsArticles
{
    public class AuthorInfo
    {
        #region " Private Members "

        // local property declarations
        string _userName;
        int _userID;
        string _firstName;
        string _lastName;
        string _displayName;
        int _articleCount;

        #endregion

        #region " public Properties "

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
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

        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
            }
        }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public int ArticleCount
        {
            get
            {
                return _articleCount;
            }
            set
            {
                _articleCount = value;
            }
        }

        #endregion
    }
}