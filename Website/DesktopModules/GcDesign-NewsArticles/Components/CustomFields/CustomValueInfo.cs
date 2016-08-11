using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GcDesign.NewsArticles.Components.CustomFields
{
    public class CustomValueInfo
    {

        #region " Private Members "

        int _customValueID;
        int _articleID;
        int _customFieldID;
        string _customValue;

        #endregion

        #region " Public Properties "

        public int CustomValueID
        {
            get
            {
                return _customValueID;
            }
            set
            {
                _customValueID = value;
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

        public int CustomFieldID
        {
            get
            {
                return _customFieldID;
            }
            set
            {
                _customFieldID = value;
            }
        }

        public string CustomValue
        {
            get
            {
                return _customValue;
            }
            set
            {
                _customValue = value;
            }
        }


        #endregion

    }
}