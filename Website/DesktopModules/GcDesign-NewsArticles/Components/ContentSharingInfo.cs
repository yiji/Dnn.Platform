using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;

namespace GcDesign.NewsArticles
{
    public class ContentSharingInfo
    {
        #region " Private Methods "

        int _linkedPortalID;
        string _portalTitle;
        int _linkedTabID;
        string _tabTitle;
        int _linkedModuleID;
        string _moduleTitle;

        #endregion

        #region " public Properties "

        public int LinkedPortalID
        {
            get
            {
                return _linkedPortalID;
            }
            set
            {
                _linkedPortalID = value;
            }
        }

        public string PortalTitle
        {
            get
            {
                if (_portalTitle == "")
                {
                    PortalController objPortalController = new PortalController();
                    PortalInfo objPortal = objPortalController.GetPortal(LinkedPortalID);

                    if (objPortal != null)
                    {
                        _portalTitle = objPortal.PortalName;

                        List<PortalAliasInfo> portalAliases = PortalAliasController.Instance.GetPortalAliasesByPortalId(_linkedPortalID).ToList();

                        if (portalAliases.Count > 0)
                        {
                            _portalTitle = DotNetNuke.Common.Globals.AddHTTP(((PortalAliasInfo)portalAliases[0]).HTTPAlias);
                        }
                    }

                }
                return _portalTitle;
            }
        }

        public int LinkedTabID
        {
            get
            {
                return _linkedTabID;
            }
            set
            {
                _linkedTabID = value;
            }
        }

        public string TabTitle
        {
            get
            {
                return _tabTitle;
            }
            set
            {
                _tabTitle = value;
            }
        }

        public int LinkedModuleID
        {
            get
            {
                return _linkedModuleID;
            }
            set
            {
                _linkedModuleID = value;
            }
        }

        public string ModuleTitle
        {
            get
            {
                return _moduleTitle;
            }
            set
            {
                _moduleTitle = value;
            }
        }

        public  string Title
        {
            get
            {
                return PortalTitle + " -> " + _tabTitle + " -> " + _moduleTitle;
            }
        }

        public  string LinkedID
        {
            get
            {
                return _linkedPortalID + "-" + _linkedModuleID;
            }
        }

        #endregion
    }
}