using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml;

namespace GcDesign.NewsArticles
{
    public class TemplateInfo
    {
        #region "Private Members"
        string _template;
        string[] _tokens;
        XmlDocument _xml;
        #endregion

        #region "public Properties"

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

        public XmlDocument Xml
        {
            get
            {
                return _xml;
            }
            set
            {
                _xml = value;
            }
        }

        #endregion
    }
}