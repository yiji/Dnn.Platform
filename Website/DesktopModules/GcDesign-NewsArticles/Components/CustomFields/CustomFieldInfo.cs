using GcDesign.NewsArticles.Components.CustomFields;

namespace GcDesign.NewsArticles.Components.CustomFields
{
    public class CustomFieldInfo
    {
        #region " Private Members "

        int _customFieldID;
        int _moduleID;
        string _name;
        CustomFieldType _fieldType;
        string _fieldElements;
        string _defaultValue;
        string _caption;
        string _captionHelp;
        bool _isRequired;
        bool _isVisible;
        int _sortOrder;
        CustomFieldValidationType _validationType;
        string _regularExpression;
        int _length;

        #endregion

        #region " Public Properties "

        public int CustomFieldID
        {
            get
            {
                return _customFieldID;
            }
            set
            {
                _customFieldID = value;
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

        public string FieldElements
        {
            get
            {
                return _fieldElements;
            }
            set
            {
                _fieldElements = value;
            }
        }

        public CustomFieldType FieldType
        {
            get
            {
                return _fieldType;
            }
            set
            {
                _fieldType = value;
            }
        }

        public string DefaultValue
        {
            get
            {
                return _defaultValue;
            }
            set
            {
                _defaultValue = value;
            }
        }


        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
            }
        }


        public string CaptionHelp
        {
            get
            {
                return _captionHelp;
            }
            set
            {
                _captionHelp = value;
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


        public bool IsRequired
        {
            get
            {
                return _isRequired;
            }
            set
            {
                _isRequired = value;
            }
        }


        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
            }
        }


        public CustomFieldValidationType ValidationType
        {
            get
            {
                return _validationType;
            }
            set
            {
                _validationType = value;
            }
        }


        public int Length
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;
            }
        }


        public string RegularExpression
        {
            get
            {
                return _regularExpression;
            }
            set
            {
                _regularExpression = value;
            }
        }

        #endregion

    }
}