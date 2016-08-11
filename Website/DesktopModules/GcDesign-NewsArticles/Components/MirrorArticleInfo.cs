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
    public class MirrorArticleInfo
    {
        #region " Private Methods "

        int _articleID;
        int _linkedArticleID;
        int _linkedPortalID;
        bool _autoUpdate;
        string _portalName = "";
        int _portalID;

        #endregion

        #region " public Properties "

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

        public int PortalID
        {
            get
            {
                return _portalID;
            }
            set
            {
                _portalID = value;
            }
        }

        public int LinkedArticleID
        {
            get
            {
                return _linkedArticleID;
            }
            set
            {
                _linkedArticleID = value;
            }
        }

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

        public bool AutoUpdate
        {
            get
            {
                return _autoUpdate;
            }
            set
            {
                _autoUpdate = value;
            }
        }

        public  string PortalName
        {
            get
            {
                if (_portalName == "")
                {
                    PortalController objPortalController = new PortalController();
                    PortalInfo objPortal = objPortalController.GetPortal(LinkedPortalID);

                    if (objPortal != null)
                    {
                        _portalName = objPortal.PortalName;

                        PortalAliasController o = new PortalAliasController();
                        List<PortalAliasInfo> portalAliases = PortalAliasController.Instance.GetPortalAliasesByPortalId(_linkedPortalID).ToList();

                        if (portalAliases.Count > 0)
                        {
                            _portalName = DotNetNuke.Common.Globals.AddHTTP(((PortalAliasInfo)portalAliases[0]).HTTPAlias);
                        }
                    }
                }
                return _portalName;
            }
        }

        #endregion
    }
}