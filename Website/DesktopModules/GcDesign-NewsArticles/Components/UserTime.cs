using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace GcDesign.NewsArticles.Components
{
    public class UserTime
    {
        public UserTime()
        {

        }

        public DateTime ConvertToUserTime(DateTime dt, double ClientTimeZone)
        {

            PortalSettings _portalSettings = PortalController.Instance.GetCurrentPortalSettings();

            return dt.AddMinutes(ClientTimeZone);

        }

        public DateTime ConvertToServerTime(DateTime dt, double ClientTimeZone)
        {

            PortalSettings _portalSettings = PortalController.Instance.GetCurrentPortalSettings();

            return dt.AddMinutes(ClientTimeZone * -1);


        }

        public double ClientToServerTimeZoneFactor(int serverTimeZoneOffet)
        {


            UserInfo objUserInfo = UserController.Instance.GetCurrentUserInfo();
            return FromClientToServerFactor(objUserInfo.Profile.TimeZone, serverTimeZoneOffet);


        }

        public double PortalToServerTimeZoneFactor(int serverTimeZoneOffet)
        {

            PortalSettings _portalSettings = PortalController.Instance.GetCurrentPortalSettings();
            return FromClientToServerFactor(_portalSettings.TimeZoneOffset, serverTimeZoneOffet);


        }


        public double ServerToClientTimeZoneFactor()
        {



            UserInfo objUser = UserController.GetCurrentUserInfo();
            PortalSettings _portalSettings = PortalController.Instance.GetCurrentPortalSettings();
            return FromServerToClientFactor(objUser.Profile.TimeZone, _portalSettings.TimeZoneOffset);


        }

        private double FromClientToServerFactor(double Client, double Server)
        {

            return Client - Server;

        }

        private double FromServerToClientFactor(double Client, double Server)
        {

            return Server - Client;

        }
    }
}