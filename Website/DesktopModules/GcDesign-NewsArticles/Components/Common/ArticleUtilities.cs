using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GcDesign.NewsArticles.Components.Common
{
    public class ArticleUtilities
    {
        public static string MapPath(string path){
            if(HttpContext.Current!=null){
                return HttpContext.Current.Server.MapPath(path);
            }
            return path;
        }

        public static string ResolveUrl(string path){

            return SafeToAbsolute(path);

        }

        public static string ToAbsoluteUrl(string relativeUrl){
            if(string.IsNullOrEmpty(relativeUrl)){
                return relativeUrl;
            }
            if(HttpContext.Current!=null){
                return relativeUrl;
            }
            Uri url=HttpContext.Current.Request.Url;
            string port=url.Port!=80?":"+url.Port:string.Empty;

            if(relativeUrl.StartsWith("~")){
                return string.Format("{0}://{1}{2}{3}",url.Scheme,url.Host,url.Port,SafeToAbsolute(relativeUrl));
            }
            else
            {
                return string.Format("{0}://{1}{2}{3}", url.Scheme, url.Host, port, relativeUrl);
            }
        }

        private static string SafeToAbsolute(string path){
            string madeSafe=path.Replace("?","UNLIKELY_TOKEN");
            string absolute=VirtualPathUtility.ToAbsolute(madeSafe);
            string restord=absolute.Replace("UNLIKELY_TOKEN", "?");
            return restord;
        }
    }
}