using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GcDesign.NewsArticles
{
    [Serializable]
    public class HandoutArticle
    {
        #region " private Members "

        private int _articleID;
        private string _title;
        private string _summary;
        private int _sortOrder;

        #endregion

        #region " public Properties "

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