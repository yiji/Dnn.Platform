using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GcDesign.NewsArticles
{
    public class RatingInfo
    {
        #region " Private Members "

        int _ratingID;
        int _articleID;
        int _userID;
        DateTime _createdDate;
        double _rating;

        #endregion

        #region " public Properties "

        public int RatingID
        {
            get
            {
                return _ratingID;
            }
            set
            {
                _ratingID = value;
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

        #endregion
    }
}