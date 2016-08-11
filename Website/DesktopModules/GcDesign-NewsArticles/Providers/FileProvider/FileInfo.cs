using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Entities.Portals;

namespace GcDesign.NewsArticles
{
    public class FileInfo
    {
        #region " Private Members "

        int _fileID;
        int _articleID;

        string _title;
        string _fileName;
        string _extension;
        int _size;
        string _contentType;
        string _folder;
        int _sortOrder;
        string _fileGuid;
        string _link;

        #endregion

        #region " Public Properties "

        public int FileID
        {
            get
            {
                return _fileID;
            }
            set
            {
                _fileID = value;
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

        public string FileGuid
        {
            get
            {
                return _fileGuid;
            }
            set
            {
                _fileGuid = value;
            }
        }

        public string Link
        {
            get
            {
                return _link;
            }
            set
            {
                _link = value;
            }
        }

        #endregion

    }
}