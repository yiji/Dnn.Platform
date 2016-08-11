using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Common.Utilities;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web.Services;
using Ventrian.ImageResizer;
using System.IO;
using System.Collections.Specialized;

namespace GcDesign.NewsArticles
{
    /// <summary>
    /// ImageHandler 的摘要说明
    /// </summary>
    public class ImageHandler : IHttpHandler
    {

        #region " private Members "

        private int _width = ArticleConstants.DEFAULT_THUMBNAIL_WIDTH;
        private int _height = ArticleConstants.DEFAULT_THUMBNAIL_HEIGHT;
        private string _homeDirectory = Null.NullString;
        private string _fileName = Null.NullString;
        private bool _quality = false;
        private bool _cropped = false;

        #endregion

        #region " private Methods "

        private int GetPhotoHeight(Image objPhoto)
        {

            int width;
            if (objPhoto.Width > _width)
            {
                width = _width;
            }
            else
            {
                width = objPhoto.Width;
            }

            int height = Convert.ToInt32(objPhoto.Height / (objPhoto.Width / width));
            if (height > _height)
            {
                height = _height;
                width = Convert.ToInt32(objPhoto.Width / (objPhoto.Height / height));
            }

            return height;

        }

        private int GetPhotoWidth(Image objPhoto)
        {

            int width;

            if (objPhoto.Width > _width)
            {
                width = _width;
            }
            else
            {
                width = objPhoto.Width;
            }

            int height = Convert.ToInt32(objPhoto.Height / (objPhoto.Width / width));
            if (height > _height)
            {
                height = _height;
                width = Convert.ToInt32(objPhoto.Width / (objPhoto.Height / height));
            }

            return width;

        }

        private void ReadQueryString(HttpContext context)
        {

            if (context.Request["Width"] != null)
            {
                if (Numeric.IsNumeric(context.Request["Width"]))
                {
                    _width = Convert.ToInt32(context.Request["Width"]);
                }
            }

            if (context.Request["Height"] != null)
            {
                if (Numeric.IsNumeric(context.Request["Height"]))
                {
                    _height = Convert.ToInt32(context.Request["Height"]);
                }
            }

            if (context.Request["HomeDirectory"] != null)
            {
                _homeDirectory = context.Server.UrlDecode(context.Request["HomeDirectory"]);
            }

            if (context.Request["FileName"] != null)
            {
                _fileName = context.Server.UrlDecode(context.Request["FileName"]);
            }

            if (context.Request["Q"] != null)
            {
                if (context.Request["Q"] == "1")
                {
                    _quality = true;
                }
            }

            if (context.Request["S"] != null)
            {
                if (context.Request["S"] == "1")
                {
                    _cropped = true;
                }
            }

        }

        #endregion

        #region " Properties "

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region " Event Handlers "

        public void ProcessRequest(HttpContext context)
        {

            //设置返回格式 Set up the response settings
            context.Response.ContentType = "image/jpeg";

            //缓存 Caching 
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.Now.AddDays(30));
            context.Response.Cache.VaryByParams["FileName"] = true;
            context.Response.Cache.VaryByParams["HomeDirectory"] = true;
            context.Response.Cache.VaryByParams["Width"] = true;
            context.Response.Cache.VaryByParams["Height"] = true;
            context.Response.Cache.VaryByParams["s"] = true;
            context.Response.Cache.AppendCacheExtension("max-age=86400");

            ReadQueryString(context);

            string path = "";
            if (_fileName == "placeholder-600.jpg")
            {
                path = "Images/placeholder-600.jpg";
            }
            else
            {
                path = _homeDirectory + "/" + _fileName;
            }

            context.Items.Add("httpcompress.attemptedinstall", "true");

            if (!System.IO.File.Exists(context.Server.MapPath(path)))
            {
                path = path + ".resources";
                if (!System.IO.File.Exists(context.Server.MapPath(path)))
                {
                    return;
                }
            }

            if (!_cropped)
            {
                Image photo = Image.FromFile(context.Server.MapPath(path));

                int width = GetPhotoWidth(photo);
                int height = GetPhotoHeight(photo);

                photo.Dispose();

                _width = width;
                _height = height;

            }

            NameValueCollection objQueryString = new NameValueCollection();

            foreach (string key in context.Request.QueryString.Keys)
            {
                string[] values = context.Request.QueryString.GetValues(key);
                foreach (string value in values)
                {

                    if (key.ToLower() == "width" || key.ToLower() == "height")
                    {
                        if (key.ToLower() == "width")
                        {
                            objQueryString.Add("maxwidth", _width.ToString());
                            objQueryString.Add(key, _width.ToString());
                        }
                        if (key.ToLower() == "height")
                        {
                            objQueryString.Add("maxheight", _height.ToString());
                            objQueryString.Add(key, _height.ToString());
                        }
                    }
                    else
                    {
                        objQueryString.Add(key, value);
                    }
                }
            }

            if (_cropped)
            {
                objQueryString.Add("crop", "auto");
            }

            Bitmap objImage = ImageManager.getBestInstance().BuildImage(context.Server.MapPath(path), objQueryString, new WatermarkSettings(objQueryString));
            if (path.ToLower().EndsWith("jpg"))
            {
                objImage.Save(context.Response.OutputStream, ImageFormat.Jpeg);
            }
            else
            {
                if (path.ToLower().EndsWith("gif"))
                {
                    context.Response.ContentType = "image/gif";
                    ImageOutputSettings ios = new ImageOutputSettings(ImageOutputSettings.GetImageFormatFromPhysicalPath(context.Server.MapPath(path)), objQueryString);
                    ios.SaveImage(context.Response.OutputStream, objImage);
                }
                else
                {
                    if (path.ToLower().EndsWith("png"))
                    {
                        MemoryStream objMemoryStream = new MemoryStream();
                        context.Response.ContentType = "image/png";
                        objImage.Save(objMemoryStream, ImageFormat.Png);
                        objMemoryStream.WriteTo(context.Response.OutputStream);
                    }
                    else
                    {
                        objImage.Save(context.Response.OutputStream, ImageFormat.Jpeg);
                    }
                }
            }

        }

        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            for (int i = 0; i < codecs.Length; i++)
            {
                if (codecs[i].MimeType == mimeType)
                {
                    return codecs[i];
                }
            }

            return null;
        }

        #endregion
    }
}