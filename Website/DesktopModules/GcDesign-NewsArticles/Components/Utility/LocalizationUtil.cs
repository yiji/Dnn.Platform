using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Xml;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using System.Web.Caching;

namespace GcDesign.NewsArticles.Components.Utility
{
    public class LocalizationUtil
    {
        #region " Private Members "

        private static string _strUseLanguageInUrlDefault = Null.NullString;

        #endregion

        #region " Public Methods "

        private static bool GetHostSettingAsBoolean(string key, bool defaultValue)
        {
            bool retValue = defaultValue;
            try
            {
                string setting = DotNetNuke.Entities.Host.HostSettings.GetHostSetting(key);
                if (!string.IsNullOrEmpty(setting))
                {
                    retValue = (setting.ToUpperInvariant().StartsWith("Y") || setting.ToUpperInvariant() == "TRUE");
                }
            }
            catch (Exception ex)
            {
                //we just want to trap the error as we may not be installed so there will be no Settings
            }
            return retValue;
        }

        private static bool GetPortalSettingAsBoolean(int portalID, string key, bool defaultValue)
        {
            bool retValue = defaultValue;
            try
            {
                string setting = DotNetNuke.Entities.Portals.PortalSettings.GetSiteSetting(portalID, key);
                if (!string.IsNullOrEmpty(setting))
                {
                    retValue = (setting.ToUpperInvariant().StartsWith("Y") || setting.ToUpperInvariant() == "TRUE");
                }
            }
            catch (Exception ex)
            {
                //we just want to trap the error as we may not be installed so there will be no Settings
            }
            return retValue;
        }

        public static bool UseLanguageInUrl()
        {

            string hostSetting = DotNetNuke.Entities.Host.HostSettings.GetHostSetting("EnableUrlLanguage");
            if (hostSetting != "")
            {
                return GetHostSettingAsBoolean("EnableUrlLanguage", true);
            }

            PortalSettings objSettings = PortalController.Instance.GetCurrentPortalSettings();
            string portalSetting = DotNetNuke.Entities.Portals.PortalSettings.GetSiteSetting(objSettings.PortalId, "EnableUrlLanguage");
            if (portalSetting != "")
            {
                return GetPortalSettingAsBoolean(objSettings.PortalId, "EnableUrlLanguage", true);
            }

            if (!File.Exists(HttpContext.Current.Server.MapPath(Localization.ApplicationResourceDirectory + "/Locales.xml")))
            {
                return GetHostSettingAsBoolean("EnableUrlLanguage", true);
            }

            string cacheKey = "";
            PortalSettings objPortalSettings = PortalController.Instance.GetCurrentPortalSettings();
            bool useLanguage = false;

            // check default host setting
            if (string.IsNullOrEmpty(_strUseLanguageInUrlDefault))
            {
                XmlDocument xmldoc = new XmlDocument();
                XmlNode languageInUrl;

                xmldoc.Load(HttpContext.Current.Server.MapPath(Localization.ApplicationResourceDirectory + "/Locales.xml"));
                languageInUrl = xmldoc.SelectSingleNode("//root/languageInUrl");
                if (languageInUrl != null)
                {
                    _strUseLanguageInUrlDefault = languageInUrl.Attributes["enabled"].InnerText;
                }
                else
                {
                    try
                    {
                        int version = Convert.ToInt32(PortalController.Instance.GetCurrentPortalSettings().Version.Replace(".", ""));
                        if (version >= 490)
                        {
                            _strUseLanguageInUrlDefault = "true";
                        }
                        else
                        {
                            _strUseLanguageInUrlDefault = "false";
                        }
                    }
                    catch
                    {
                        _strUseLanguageInUrlDefault = "false";
                    }
                }
            }
            useLanguage = Convert.ToBoolean(_strUseLanguageInUrlDefault);

            //check current portal setting
            string FilePath = HttpContext.Current.Server.MapPath(Localization.ApplicationResourceDirectory + "/Locales.Portal-" + objPortalSettings.PortalId.ToString() + ".xml");
            if (File.Exists(FilePath))
            {
                cacheKey = "dotnetnuke-uselanguageinurl" + objPortalSettings.PortalId.ToString();
                try
                {
                    object o = DataCache.GetCache(cacheKey);
                    if (o == null)
                    {
                        XmlDocument xmlLocales = new XmlDocument();
                        bool bXmlLoaded = false;

                        xmlLocales.Load(FilePath);
                        bXmlLoaded = true;

                        XmlDocument d = new XmlDocument();
                        d.Load(FilePath);

                        if (bXmlLoaded && xmlLocales.SelectSingleNode("//locales/languageInUrl") != null)
                        {
                            useLanguage = Boolean.Parse(xmlLocales.SelectSingleNode("//locales/languageInUrl").Attributes["enabled"].InnerText);
                        }
                        if (Globals.PerformanceSetting != Globals.PerformanceSettings.NoCaching)
                        {
                            CacheDependency dp = new CacheDependency(FilePath);
                            DataCache.SetCache(cacheKey, useLanguage, dp);
                        }
                    }
                    else
                    {
                        useLanguage = Convert.ToBoolean(o);
                    }
                }
                catch (Exception ex)
                {
                }

                return useLanguage;
            }
            else
            {
                return useLanguage;
            }

        }

        #endregion
    }
}