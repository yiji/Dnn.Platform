using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GcDesign.NewsArticles
{
    public class LayoutInfo
    {
        #region " Private Members "

        string _template;
        string[] _tokens;

        #endregion

        #region " public Properties "

        public string Template
        {
            get
            {
                return _template;
            }
            set
            {
                _template = value;
            }
        }


        public string[] Tokens
        {
            get
            {
                return _tokens;
            }
            set
            {
                _tokens = value;
            }
        }

        #endregion
    }
}