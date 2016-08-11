using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GcDesign.NewsArticles.Import
{
    public enum FeedExpiryType
    {
        None = -1,
        Minute = 0,
        Hour = 1,
        Day = 2,
        Month = 3,
        Year = 4,
    }
}