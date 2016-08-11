using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace GcDesign.NewsArticles.Base
{
    public class NewsArticleControlBase : PortalModuleBase  //UserControl,
    {
        #region " Private Members "

        private int _articleID;

        #endregion

        #region " Public Properties "

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

        protected NewsArticleModuleBase ArticleModuleBase
        {
            get
            {
                return (NewsArticleModuleBase)Parent;
            }
        }

        #endregion
    }
}