using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GcDesign.NewsArticles
{
    public class TagInfo : IComparable
    {
        #region " Private Members "

        int _tagID;
        int _moduleID;
        string _name;
        string _nameLowered;
        int _usages;

        #endregion

        #region " public Properties "

        public int TagID
        {
            get
            {
                return _tagID;
            }
            set
            {
                _tagID = value;
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

        public string NameLowered
        {
            get
            {
                return _nameLowered;
            }
            set
            {
                _nameLowered = value;
            }
        }

        public int Usages
        {
            get
            {
                return _usages;
            }
            set
            {
                _usages = value;
            }
        }

        #endregion

        #region " Optional Interfaces "

         int IComparable.CompareTo(object obj)
        {
            if (obj.GetType() != typeof(TagInfo))
            {
                throw new Exception("Object is not TagInfo");
            }

            TagInfo Compare = (TagInfo)obj;
            int result = this.Name.CompareTo(Compare.Name);

            if (result == 0)
            {
                result = this.Name.CompareTo(Compare.Name);
            }
            return result;
        }

        #endregion


    }
}