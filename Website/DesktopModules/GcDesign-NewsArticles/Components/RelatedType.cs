using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GcDesign.NewsArticles
{
    public enum RelatedType
    {
        None,
        MatchCategoriesAny,
        MatchCategoriesAll,
        MatchTagsAny,
        MatchTagsAll,
        MatchCategoriesAnyTagsAny,
        MatchCategoriesAllTagsAny,
        MatchCategoriesAnyTagsAll,
    }
}