using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke;
using DotNetNuke.Common.Utilities;
namespace GcDesign.NewsArticles
{
    public class CategoryInfo
    {
        #region " Private Methods "

        // local property declarations
        int _categoryID;
        int _moduleID;
        int _parentID;
        string _name;
        string _nameIndented;
        string _description;
        string _image;
        int _level;
        int _sortOrder;
        bool _inheritSecurity;
        CategorySecurityType _categorySecurityType;

        int _numberOfArticles;
        int _numberOfViews;

        string _metaTitle;
        string _metaDescription;
        string _metaKeywords;


        int _levelParent = Null.NullInteger;

        #endregion

        #region " public Properties "

        public int CategoryID
        {
            get
            {
                return _categoryID;
            }
            set
            {
                _categoryID = value;
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

        public int ParentID
        {
            get
            {
                return _parentID;
            }
            set
            {
                _parentID = value;
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

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public string NameIndented
        {
            get
            {
                if (Level == 1)
                {
                    return Name;
                }
                else
                {
                    if ((Level - 1) * 2 > 0)
                    {
                        string a = new String('.', (Level - 1) * 2);
                        return a + Name;
                    }
                    else
                    {
                        return Name;
                    }
                }
            }
            set
            {
                _nameIndented = value;
            }
        }

        public string Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }

        public int Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
            }
        }

        public int LevelParent
        {
            get
            {
                return _levelParent;
            }
            set
            {
                _levelParent = value;
            }
        }

        public int SortOrder
        {
            get
            {
                return _sortOrder;
            }
            set
            {
                _sortOrder = value;
            }
        }

        public bool InheritSecurity
        {
            get
            {
                return _inheritSecurity;
            }
            set
            {
                _inheritSecurity = value;
            }
        }

        public CategorySecurityType CategorySecurityType
        {
            get
            {
                return _categorySecurityType;
            }
            set
            {
                _categorySecurityType = value;
            }
        }

        public int NumberOfArticles
        {
            get
            {
                return _numberOfArticles;
            }
            set
            {
                _numberOfArticles = value;
            }
        }

        public int NumberOfViews
        {
            get
            {
                return _numberOfViews;
            }
            set
            {
                _numberOfViews = value;
            }
        }

        public string MetaTitle
        {
            get
            {
                return _metaTitle;
            }
            set
            {
                _metaTitle = value;
            }
        }

        public string MetaDescription
        {
            get
            {
                return _metaDescription;
            }
            set
            {
                _metaDescription = value;
            }
        }

        public string MetaKeywords
        {
            get
            {
                return _metaKeywords;
            }
            set
            {
                _metaKeywords = value;
            }
        }

        #endregion

        #region " public Methods "

        public CategoryInfo Clone()
        {
            return (CategoryInfo)MemberwiseClone();
        }

        #endregion
    }
}