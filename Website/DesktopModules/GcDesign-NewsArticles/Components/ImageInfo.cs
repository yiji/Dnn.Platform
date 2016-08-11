using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GcDesign.NewsArticles
{
    public class ImageInfo
    {
        #region " Private Members "

        int _imageID;
        int _articleID;

        string _title;

        string _fileName;
        string _extension;
        int _size;
        int _width;
        int _height;
        string _contentType;
        string _folder;
        int _sortOrder;
        string _imageGuid;
        string _description;

        #endregion

        #region " public Properties "

        public int ImageID
        {
            get
            {
                return _imageID;
            }
            set
            {
                _imageID = value;
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

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        public string Extension
        {
            get
            {
                return _extension;
            }
            set
            {
                _extension = value;
            }
        }

        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        public string ContentType
        {
            get
            {
                return _contentType;
            }
            set
            {
                _contentType = value;
            }
        }

        public string Folder
        {
            get
            {
                return _folder;
            }
            set
            {
                _folder = value;
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

        public string ImageGuid
        {
            get
            {
                return _imageGuid;
            }
            set
            {
                _imageGuid = value;
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

        #endregion
    }
}