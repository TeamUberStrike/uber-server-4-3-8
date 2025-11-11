using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Yahoo.Yui.Compressor;

namespace UberStrike.Channels.Utils 
{
    public class ScriptCombiner
    {
        private readonly static TimeSpan CACHE_DURATION = TimeSpan.FromDays(30);
        private System.Web.HttpContextBase context;
        private string _ContentType;
        private string _key;
        private string _version;

        public ScriptCombiner(string keyname, string version, string type)
        {
            this._ContentType = type;
            this._key = keyname;
            this._version = version;
        }

        public void ProcessRequest(System.Web.HttpContextBase context)
        {
            this.context = context;
            HttpRequestBase request = context.Request;

            // Read setName, version from query string
            string setName = _key;
            string version = _version;
            string contentType = _ContentType;
            // Decide if browser supports compressed response
            bool isCompressed = this.CanGZip(context.Request);

            // If the set has already been cached, write the response directly from
            // cache. Otherwise generate the response and cache it
            if (!this.WriteFromCache(setName, version, isCompressed, contentType))
            {
                using (MemoryStream memoryStream = new MemoryStream(8092))
                {
                    // Decide regular stream or gzip stream based on 
                    // whether the response can be compressed or not
                    //using (Stream writer = isCompressed ?  (Stream)(new GZipStream
                    // (memoryStream, CompressionMode.Compress)) : memoryStream)
                    using (Stream writer = isCompressed ?
                         (Stream)(new GZipStream(memoryStream, CompressionMode.Compress)) :
                         memoryStream)
                    {
                        // Read the files into one big string
                        StringBuilder allScripts = new StringBuilder();
                        foreach (string fileName in GetScriptFileNames(setName))
                        {
                            allScripts.Append(File.ReadAllText(context.Server.MapPath(fileName)));
                        }
                        // Minify the combined script files and remove comments and white spaces
                        //var minifier = new JavaScriptMinifier();
                        string minified = string.Empty;
                        if (contentType == "text/javascript")
                        {
                            minified = JavaScriptCompressor.Compress(allScripts.ToString());
                        }
                        else if (contentType == "text/css")
                        {
                            minified = CssCompressor.Compress(allScripts.ToString());
                        }

#if DEBUG
                        minified = allScripts.ToString();
#endif
                        byte[] bts = Encoding.UTF8.GetBytes(minified);
                        writer.Write(bts, 0, bts.Length);
                    }

                    // Cache the combined response so that it can be directly written
                    // in subsequent calls
                    byte[] responseBytes = memoryStream.ToArray();
                    context.Cache.Insert(GetCacheKey(setName, version, isCompressed),
                        responseBytes, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                        CACHE_DURATION);

                    // Generate the response
                    this.WriteBytes(responseBytes, isCompressed, contentType);
                }
            }
        }
        private bool WriteFromCache(string setName, string version,
                bool isCompressed, string ContentType)
        {
            byte[] responseBytes = context.Cache[GetCacheKey
                  (setName, version, isCompressed)] as byte[];

            if (responseBytes == null || responseBytes.Length == 0)
                return false;

            this.WriteBytes(responseBytes, isCompressed, ContentType);
            return true;
        }

        private void WriteBytes(byte[] bytes, bool isCompressed, string ContentType)
        {
            HttpResponseBase response = context.Response;
            response.AppendHeader("Content-Length", bytes.Length.ToString());
            response.ContentType = ContentType;
            if (isCompressed)
                response.AppendHeader("Content-Encoding", "gzip");
            else
                response.AppendHeader("Content-Encoding", "utf-8");

            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.Now.Add(CACHE_DURATION));
            context.Response.Cache.SetMaxAge(CACHE_DURATION);
            response.ContentEncoding = Encoding.Unicode;
            response.OutputStream.Write(bytes, 0, bytes.Length);
            response.Flush();
        }

        private bool CanGZip(HttpRequestBase request)
        {
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (!string.IsNullOrEmpty(acceptEncoding) &&
                 (acceptEncoding.Contains("gzip") || acceptEncoding.Contains("deflate")))
                return true;
            return false;
        }

        private string GetCacheKey(string setName, string version, bool isCompressed)
        {
            return "HttpCombiner." + setName + "." + version + "." + isCompressed;
        }

        public bool IsReusable
        {
            get { return true; }
        }

        // private helper method that return an array of file names 
        // inside the text file stored in App_Data folder
        private static string[] GetScriptFileNames(string setName)
        {
            var scripts = new System.Collections.Generic.List<string>();

            string setDefinition =
                        System.Configuration.ConfigurationManager.AppSettings[setName] ?? "";
            string[] fileNames = setDefinition.Split(new char[] { ',' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string fileName in fileNames)
            {
                if (!String.IsNullOrEmpty(fileName))
                    scripts.Add(fileName);
            }
            return scripts.ToArray();
        }
    }
}
