using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GcDesign.NewsArticles
{
    public class ArchiveInfo
    {

        #region " Private Methods "

        // local property declarations
        int _day;
        int _month;
        int _year;
        int _count;

        #endregion

        #region " Public Properties "

        public int Day
        {
            get
            {
                return _day;
            }
            set
            {
                _day = value;
            }
        }

        public int Month
        {
            get
            {
                return _month;
            }
            set
            {
                _month = value;
            }
        }


        public int Year
        {
            get
            {
                return _year;
            }
            set
            {
                _year = value;
            }
        }


        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
            }
        }

        #endregion

    }
}