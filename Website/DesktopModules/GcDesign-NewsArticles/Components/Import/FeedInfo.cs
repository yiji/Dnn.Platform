using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Common.Utilities;

namespace GcDesign.NewsArticles.Import
{
    public class FeedInfo
    {
        #region " Private Members "

        int _feedID = Null.NullInteger;
        int _moduleID;
        string _title;
        string _url;
        int _userID;
        bool _autoFeature;
        bool _isActive;
        FeedDateMode _dateMode;

        int _autoExpire;
        FeedExpiryType _autoExpireUnit;

        List<CategoryInfo> _categories;

        #endregion

        #region " Private Properties "

        public int FeedID
        {
            get
            {
                return _feedID;
            }
            set
            {
                _feedID = value;
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

        public bool AutoFeature
        {
            get
            {
                return _autoFeature;
            }
            set
            {
                _autoFeature = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
            }
        }

        public FeedDateMode DateMode
        {
            get
            {
                return _dateMode;
            }
            set
            {
                _dateMode = value;
            }
        }

        public int AutoExpire
        {
            get
            {
                return _autoExpire;
            }
            set
            {
                _autoExpire = value;
            }
        }

        public FeedExpiryType AutoExpireUnit
        {
            get
            {
                return _autoExpireUnit;
            }
            set{
                _autoExpireUnit = value;
            }
        }

        public List<CategoryInfo> Categories
        {
            get{
                if (_categories == null) {
                    if (_feedID == Null.NullInteger) {
                        _categories = new List<CategoryInfo>();
                    }
                    else
                    {
                        FeedController objFeedController = new FeedController();
                        _categories = objFeedController.GetFeedCategoryList(_feedID);
                    }
                }
                return _categories;
            }
            set
            {
                _categories = value;
            }
        }

        #endregion
    }
}