using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text.RegularExpressions;

using CookComputing.XmlRpc;

namespace GcDesign.NewsArticles.Tracking
{
    public class PingProxy : XmlRpcClientProtocol
    {
        #region " Public Methods "

        public WeblogsUpdateResponse Ping(string WeblogName, string WeblogURL)
        {
            IWebLogsUpdate proxy = (IWebLogsUpdate)XmlRpcProxyGen.Create(typeof(IWebLogsUpdate));
            return proxy.Ping(WeblogName, WeblogURL);
        }

        public struct WeblogsUpdateResponse
        {
            public bool flerror;
            public string message;
        }

        [XmlRpcUrl("http://rpc.weblogs.com/RPC2")]
        public interface IWebLogsUpdate
        {
            [XmlRpcMethod("weblogUpdates.ping")]
            WeblogsUpdateResponse Ping(string WeblogName, string WeblogURL);
        }

        #endregion
    }
}