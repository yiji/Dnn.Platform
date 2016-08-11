using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GcDesign.NewsArticles
{
    public class EmailTemplateInfo
    {
        #region "Private Members"
        int _templateID;
        int _moduleID;
        string _name;
        string _subject;
        string _template;
        #endregion

        #region "Constructors"
        public EmailTemplateInfo() { }

        public EmailTemplateInfo(int templateID, int moduleID, string name, string subject, string template)
        {
            this.TemplateID = templateID;
            this.ModuleID = moduleID;
            this.Name = name;
            this.Subject = subject;
            this.Template = template;
        }
        #endregion

        #region "public Properties"
        public int TemplateID
        {
            get
            {
                return _templateID;
            }
            set
            {
                _templateID = value;
            }
        }

        public int ModuleID
        {
            get
            {
                return _moduleID;
            }
            set
            {
                _moduleID = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                _subject = value;
            }
        }

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
        #endregion
    }
}