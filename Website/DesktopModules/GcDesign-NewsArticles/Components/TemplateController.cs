using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;

namespace GcDesign.NewsArticles
{
    public class TemplateController
    {
        #region " public Methods "

        public TemplateInfo GetTemplate(string name, PortalSettings portalSettings, string template, int tabModuleID)
        {
            // Me.MapPath("Templates/" & Template & "/")
            // Dim pathToTemplate As String = articleModuleBase.TemplatePath

            string pathToTemplate = portalSettings.HomeDirectoryMapPath + @"GcDesign-NewsArticles\Templates\Templates\" + template + @"\";

            string cacheKey = tabModuleID.ToString() + name;
            string cacheKeyXml = tabModuleID.ToString() + name + ".xml";

            TemplateInfo objTemplate = (TemplateInfo)DataCache.GetCache(cacheKey);
            TemplateInfo objTemplateXml = (TemplateInfo)DataCache.GetCache(cacheKeyXml);

            if (objTemplate == null || objTemplateXml == null)
            {
                string delimStr = "[]";
                char[] delimiter = delimStr.ToCharArray();

                objTemplate = new TemplateInfo();

                string path = pathToTemplate + name + ".html";
                string pathXml = pathToTemplate + name + ".xml";

                if (!File.Exists(path))
                {
                    //path = articleModuleBase.MapPath("Templates/Default/") & name & ".html"
                    path = pathToTemplate + @"Standard\" + name + ".html";
                }

                StreamReader sr = new StreamReader(path);
                try
                {
                    objTemplate.Template = sr.ReadToEnd();
                }
                catch
                {
                    objTemplate.Template = "<br>ERROR: UNABLE TO READ '" + name + "' TEMPLATE:";
                }
                finally
                {
                    if (sr != null)
                    {
                        sr.Close();
                    }
                }

                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(pathXml);
                }
                catch
                {
                    // Do Nothing 
                }
                finally
                {
                    objTemplate.Xml = doc;
                }

                objTemplate.Tokens = objTemplate.Template.Split(delimiter);

                DataCache.SetCache(cacheKey, objTemplate, new CacheDependency(path));
                DataCache.SetCache(cacheKeyXml, objTemplate, new CacheDependency(pathXml));
            }

            return objTemplate;

        }

        #endregion
    }
}