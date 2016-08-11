using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GcDesign.NewsArticles
{
    public enum EmailTemplateType
    {
        ArticleSubmission,
        ArticleApproved,
        ArticleUpdateMirrored,
        CommentNotification,
        CommentRequiringApproval,
        CommentApproved,
    }
}