using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;
using System.Xml;
using System.IO;
using System.Net;

namespace GcDesign.NewsArticles
{
    public class Bitly
    {
        private string loginAccount;
        private string apiKeyForAccount;

        private string submitPath = "http://api.bit.ly/shorten?version=2.0.1&format=xml";
        private int errorStatus = 0;
        private string errorStatusMessage = "";


        // Constructors (overloaded and chained)
        public Bitly():this("bitlyapidemo", "R_0da49e0a9118ff35f52f629d2d71bf07")
        {
        }


        public Bitly(string login , string APIKey ){
            loginAccount = login;
            apiKeyForAccount = APIKey;

            submitPath += "&login=" + loginAccount + "&apiKey=" + apiKeyForAccount;
        }


        // Properties to retrieve error information.
        public  int ErrorCode{
            get{
                return errorStatus;
            }
        }

        public  string ErrorMessage{
            get{
                return errorStatusMessage;
            }
        }


        //Main shorten function which takes in the long URL and returns the bit.ly shortened URL
        public string Shorten(string url ){

            errorStatus = 0;
            errorStatusMessage = "";

            XmlDocument doc;
            doc = buildDocument(url);

            if(doc.DocumentElement != null ){ 

                XmlNode shortenedNode = doc.DocumentElement.SelectSingleNode("results/nodeKeyVal/shortUrl");

                if (shortenedNode != null) {

                    return shortenedNode.InnerText;

                }
                else
                {

                    getErrorCode(doc);

                }
            }
            else
            {

                errorStatus = -1;
                errorStatusMessage = "Unable to connect to bit.ly for shortening of URL";
            }

            return "";

        }


        // Sets error code and message in the situation we receive a response, but there was
        // something wrong with our submission.
        private void getErrorCode(XmlDocument doc){

            XmlNode errorNode  = doc.DocumentElement.SelectSingleNode("errorCode");
            XmlNode errorMessageNode = doc.DocumentElement.SelectSingleNode("errorMessage");

            if (errorNode != null ){

                errorStatus = Convert.ToInt32(errorNode.InnerText);
                errorStatusMessage = errorMessageNode.InnerText;
            }
        }


        // Builds an XmlDocument using the XML returned by bit.ly in response
        // to our URL being submitted
        private XmlDocument buildDocument(string url){

            XmlDocument doc = new XmlDocument();

            try
            {

                // Load the XML response into an XML Document and return it.
                doc.LoadXml(readSource(submitPath + "&longUrl=" + url));
                return doc;
            }
            catch(Exception e){
            return new XmlDocument();
            }

        }


        // Fetches a result from bit.ly provided the URL submitted
        private string readSource(string url ) {
            WebClient client = new WebClient();

            try
            {

                using(StreamReader reader = new StreamReader(client.OpenRead(url))){
                    // Read all of the response
                    return reader.ReadToEnd();
               }
            }
            catch(Exception e)
            {
             throw e;
            }

       }
    }
}