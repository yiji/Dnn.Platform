using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Common.Utilities;

namespace GcDesign.NewsArticles
{
    public class HandoutInfo
    {
        #region " private Members "

        private int _handoutID = Null.NullInteger;
        private int _moduleID = Null.NullInteger;
        private int _userID = Null.NullInteger;
        private string _name = Null.NullString;
        private string _description = Null.NullString;

        private List<HandoutArticle> _articles;

        #endregion

        #region " public Properties "

        public int HandoutID
        {
            get
            {
                return _handoutID;
            }
            set
            {
                _handoutID = value;
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

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public List<HandoutArticle> Articles
        {
            get
            {
                return _articles;
            }
            set
            {
                _articles = value;
            }
        }

        #endregion
    }
}