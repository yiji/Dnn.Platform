using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GcDesign.NewsArticles
{
    public class PageInfo
    {
        #region " Private Methods "

        // local property declarations
        int _pageID;
        int _articleID;
        string _title;
        string _pageText;
        int _sortOrder;

        #endregion

        #region " Public Properties "

        public int PageID
        {
            get
            {
                return _pageID;
            }
            set
            {
                _pageID = value;
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


        public string PageText
        {
            get
            {
                return _pageText;
            }
            set
            {
                _pageText = value;
            }
        }

        public int SortOrder
        {
            get
            {
                return _sortOrder;
            }
            set
            {
                _sortOrder = value;
            }
        }

        #endregion

    }
}