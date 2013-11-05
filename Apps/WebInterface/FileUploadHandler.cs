/*
 * Major parts of this handler are based on the code by authors/entities below (Kalle Launiala 5th of November 2013).
 * 
 * 
 * jQuery File Upload Plugin C# Example 1.0
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * PHP Version: Copyright 2010, Sebastian Tschan
 * https://blueimp.net
 * 
 * Translated to C# by Shannon Whitley 2012
 * http://whitleymedia.com
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 */


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
//using AaltoGlobalImpact.OIP;
using AzureSupport;
using DotNetOpenAuth.OpenId.RelyingParty;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;
using TheBall.CORE;

namespace WebInterface
{
    public class FileUploadHandler : IHttpHandler
    {
        private const string AuthGroupPrefix = "/auth/grp/";
        private const string AuthAccountPrefix = "/auth/acc/";
        private int AuthGroupPrefixLen;
        private int AuthAccountPrefixLen;
        //private const string AuthFileUpload = "/fileupload/grp/";
        //private int AuthEmailValidationLen;


        /// <summary>
        /// You will need to configure this handler in the web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public FileUploadHandler()
        {
            AuthGroupPrefixLen = AuthGroupPrefix.Length;
            AuthAccountPrefixLen = AuthAccountPrefix.Length;
        }

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            WebSupport.InitializeContextStorage(context.Request);
            try
            {
                if (request.Path.StartsWith(AuthGroupPrefix))
                {
                    //HandleEmailValidation(context);
                }        
            } finally
            {
                InformationContext.ProcessAndClearCurrent();
            }
        }


        #endregion
    }

    public class UploadHandler
    {
        public string script_url { get; set; }
        public string upload_dir { get; set; }
        public string upload_url { get; set; }
        public string param_name { get; set; }
        public string delete_type { get; set; }
        public int max_file_size { get; set; }
        public int min_file_size { get; set; }
        public string accept_file_types { get; set; }
        public int max_number_of_files { get; set; }
        public int max_width { get; set; }
        public int max_height { get; set; }
        public int min_width { get; set; }
        public int min_height { get; set; }
        public bool discard_aborted_uploads { get; set; }
        //TODO: Enable this later if needed.
        //      The orientation code has not been written.
        //public bool orient_images { get; set; }
        Dictionary<string, UploadFileInfo> image_versions = null;
        private UploadHandler upload_handler;

        public class UploadFileInfo
        {
            public string name { get; set; }
            public long size { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public string type { get; set; }
            public string dir { get; set; }
            public string url { get; set; }
            public string thumbnail_url { get; set; }
            public string error { get; set; }
            public string delete_type { get; set; }
            public string delete_url { get; set; }
            public Dictionary<string, UploadFileInfo> image_versions { get; set; }
        }


        public UploadHandler()
        {
            PropertiesInit(null, null);
        }


        public UploadHandler(string path, string url)
        {
            PropertiesInit(path, url);
        }


        private void PropertiesInit(string path, string url)
        {
            this.script_url = url;
            this.upload_dir = path + "/files/";
            this.upload_url = url + "/files/";
            this.param_name = "files";
            // Set the following option to 'POST', if your server does not support
            // DELETE requests. This is a parameter sent to the client:
            this.delete_type = "DELETE";
            // The web.config setting maxRequestLength
            // takes precedence over max_file_size:
            //<system.web>
            //<httpRuntime executionTimeout="240" maxRequestLength="10124" />
            //</system.web>
            this.max_file_size = 10124000;
            this.min_file_size = 1;
            this.accept_file_types = @"^.+\.((jpg)|(gif)|(jpeg)|(png))$";
            // The maximum number of files for the upload directory:
            this.max_number_of_files = -1;
            // Image resolution restrictions:
            this.max_width = -1;
            this.max_height = -1;
            this.min_width = 1;
            this.min_height = 1;
            //Set the following option to false to enable resumable uploads:
            this.discard_aborted_uploads = true;
            //Set to true to rotate images based on EXIF meta data, if available:
            //this.orient_images = false;
            // Uncomment the following version to restrict the size of
            // uploaded images. You can also add additional versions with
            // their own upload directories:
            /*this.image_versions = new Dictionary<string, UploadFileInfo>() {
                {"Large",new UploadFileInfo(){width=250,height=250, dir=this.upload_dir + "Large/", url=this.upload_url + "Large/"}}
                ,{"Thumbnail",new UploadFileInfo(){width=80,height=80, dir=this.upload_dir + "Thumbnail/", url = this.upload_url + "Thumbnail/"}}
            };*/
        }


        public UploadFileInfo FileDeleteUrlSet(UploadFileInfo file)
        {
            file.delete_url = this.script_url + "/Default.aspx?file=" + HttpUtility.UrlEncode(file.name);
            file.delete_type = this.delete_type;
            if (file.delete_type != "DELETE")
            {
                file.delete_url += "&_method=DELETE";
            }


            return file;
        }


        public UploadFileInfo FileObjectGet(string file_name)
        {
            UploadFileInfo file = new UploadFileInfo();
            string file_path = this.upload_dir + file_name;
            if (File.Exists(file_path) && file_name.Length > 0 && file_name.Substring(0, 1) != ".")
            {
                file.name = file_name;
                file.size = new FileInfo(file_path).Length;
                file.url = this.upload_url + HttpUtility.UrlEncode(file_name);
                if (this.image_versions != null)
                {
                    file.image_versions = new Dictionary<string, UploadFileInfo>();
                    foreach (string version in this.image_versions.Keys)
                    {
                        if (File.Exists(this.image_versions[version].dir + file_name))
                        {
                            file.image_versions.Add(version, new UploadFileInfo()
                            {
                                name = file_name
                                ,
                                dir = this.image_versions[version].dir
                                ,
                                url = this.image_versions[version].url + HttpUtility.UrlEncode(file_name)
                            });
                        }
                    }
                }
                if (file.image_versions != null && file.image_versions.ContainsKey("Thumbnail"))
                {
                    file.thumbnail_url = file.image_versions["Thumbnail"].url;
                }
                file = FileDeleteUrlSet(file);
            }
            return file;
        }


        public List<UploadFileInfo> FileObjectsGet()
        {
            List<UploadFileInfo> infoList = new List<UploadFileInfo>();
            foreach (string file in Directory.GetFiles(this.upload_dir))
            {
                infoList.Add(FileObjectGet(Path.GetFileName(file)));
            }
            return infoList;
        }


        public bool ScaledImageCreate(UploadFileInfo file)
        {
            string file_path = this.upload_dir + file.name;
            string new_file_path = file.dir + file.name;
            Image img = Image.FromFile(file_path);
            string fileNameExtension = Path.GetExtension(file_path).ToLower();
            ImageFormat imageType = GetImageType(fileNameExtension);
            if (img == null)
            {
                return false;
            }
            int img_width = img.Width;
            int img_height = img.Height;


            if (img_width < 1 || img_height < 1)
            {
                return false;
            }


            float scale = Math.Min(file.width / (float)img_width, file.height / (float)img_height);


            int new_width = (int)Math.Round(img_width * scale, 0);
            int new_height = (int)Math.Round(img_height * scale, 0);


            Bitmap new_image = new Bitmap(new_width, new_height);
            Graphics g = Graphics.FromImage(new_image);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;


            foreach (PropertyItem pItem in img.PropertyItems)
            {
                new_image.SetPropertyItem(pItem);
            }


            g.DrawImage(img, new Rectangle(0, 0, new_width, new_height));


            img.Dispose();


            new_image.Save(new_file_path, imageType);
            new_image.Dispose();


            return true;
        }


        private static ImageFormat GetImageType(string fileExt)
        {
            switch (fileExt)
            {
                case ".jpg":
                    return ImageFormat.Jpeg;
                case ".gif":
                    return ImageFormat.Gif;
                default: // (png)
                    return ImageFormat.Png;
            }
        }


        public bool Validate(HttpPostedFile uploaded_file, UploadFileInfo file, string error, int index)
        {
            if (error != null)
            {
                file.error = error;
                return false;
            }


            if (String.IsNullOrEmpty(file.name))
            {
                file.error = "missingFileName";
                return false;
            }
            if (file.name.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)
            {
                file.error = "invalidFileName";
                return false;
            }


            if (!Regex.IsMatch(file.name, this.accept_file_types, RegexOptions.Multiline | RegexOptions.IgnoreCase))
            {
                file.error = "acceptFileTypes";
                return false;
            }


            if (this.max_file_size > 0 && (file.size > this.max_file_size))
            {
                file.error = "maxFileSize";
                return false;
            }
            if (this.min_file_size > 1 && (file.size < this.min_file_size))
            {
                file.error = "minFileSize";
                return false;
            }
            if (this.max_number_of_files > 0 && FileObjectsGet().Count >= this.max_number_of_files)
            {
                file.error = "maxNumberOfFiles";
                return false;
            }


            if (File.Exists(this.upload_dir + file.name) && file.size == new FileInfo(this.upload_dir + file.name).Length)
            {
                using (Image img = Image.FromFile(this.upload_dir + file.name))
                {
                    file.width = img.Width;
                    file.height = img.Height;
                    img.Dispose();
                }


                if ((this.max_width > 0 && file.width > this.max_width) ||
                    (this.max_height > 0 && file.height > this.max_height))
                {
                    file.error = "maxResolution";
                    return false;
                }
                if ((this.min_width > 0 && file.width < this.min_width) ||
                    (this.min_height > 0 && file.height < this.min_height))
                {
                    file.error = "minResolution";
                    return false;
                }
            }


            return true;
        }


        public UploadFileInfo FileUploadHandle(HttpPostedFile uploaded_file, string name, long size, string type, string error, int index)
        {
            UploadFileInfo file = new UploadFileInfo();
            file.name = name;
            file.size = size;
            file.type = type;


            if (Validate(uploaded_file, file, error, index))
            {
                string file_path = this.upload_dir + name;
                bool append_file = !this.discard_aborted_uploads && File.Exists(file_path)
                    || file.size > uploaded_file.InputStream.Length;


                // multipart/formdata uploads (POST method uploads)
                if (append_file)
                {
                    using (FileStream fs = File.Open(file_path, FileMode.Append))
                    {
                        uploaded_file.InputStream.CopyTo(fs);
                        fs.Flush();
                    }
                }
                else
                {
                    using (FileStream fs = File.OpenWrite(file_path))
                    {
                        uploaded_file.InputStream.CopyTo(fs);
                        fs.Flush();
                    }


                }




                if (file.size == new FileInfo(file_path).Length)
                {
                    //Validate again for chunked files.
                    if (Validate(uploaded_file, file, error, index))
                    {
                        //if (this.orient_images)
                        //{
                        //    //orient_image(file_path);
                        //}
                        //Create different versions
                        file.url = this.upload_url + HttpUtility.UrlEncode(file.name);
                        file.image_versions = new Dictionary<string, UploadFileInfo>();
                        foreach (string version in this.image_versions.Keys)
                        {
                            file.image_versions.Add(version, new UploadFileInfo()
                            {
                                name = file.name
                                ,
                                dir = this.image_versions[version].dir
                                ,
                                url = this.image_versions[version].url + HttpUtility.UrlEncode(file.name)
                                ,
                                width = this.image_versions[version].width
                                ,
                                height = this.image_versions[version].height
                            });


                            ScaledImageCreate(file.image_versions[version]);
                        }
                        if (file.image_versions != null && file.image_versions.ContainsKey("Thumbnail"))
                        {
                            file.thumbnail_url = file.image_versions["Thumbnail"].url;
                        }
                        file = FileDeleteUrlSet(file);
                    }
                }
                else
                {
                    if (!append_file && this.discard_aborted_uploads)
                    {
                        File.Delete(file_path);
                        file.error = "abort";
                    }


                }
            }

            return file;
        }


        private HttpResponse Response;
        private HttpRequest Request;

        protected void Page_Load(object sender, EventArgs e)
        {
            //upload_handler = new UploadHandler(Server.MapPath("."), FullUrlGet());
            Response.Clear();
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Cache-Control", "no-store, no-cache, must-revalidate");
            Response.AddHeader("Content-Disposition", "inline; filename=\"files.json\"");
            Response.AddHeader("X-Content-Type-Options", "nosniff");
            Response.AddHeader("Access-Control-Allow-Origin", "*");
            Response.AddHeader("Access-Control-Allow-Methods", "OPTIONS, HEAD, GET, POST, PUT, DELETE");
            Response.AddHeader("Access-Control-Allow-Headers", "X-File-Name, X-File-Type, X-File-Size");


            switch (Request.HttpMethod)
            {
                case "OPTIONS":
                    break;
                case "HEAD":
                case "GET":
                    Get();
                    break;
                case "POST":
                    if (Request["_method"] != null && Request["_method"] == "DELETE")
                    {
                        Delete();
                    }
                    else
                    {
                        Post();
                    }
                    break;
                case "DELETE":
                    Delete();
                    break;
                default:
                    Response.Status = "Method Not Allowed";
                    Response.StatusCode = 405;
                    Response.End();
                    break;
            }
        }



        private void Get()
        {
            string file_name = null;
            string json = "";


            if (Request["file"] != null)
            {
                file_name = Path.GetFileName(Request["file"]);
            }


            Response.AddHeader("Content-type", "application/json");


            if (!String.IsNullOrEmpty(file_name))
            {
                json = Json.Serialize<UploadHandler.UploadFileInfo>(upload_handler.FileObjectGet(file_name));
            }
            else
            {
                json = Json.Serialize<List<UploadHandler.UploadFileInfo>>(upload_handler.FileObjectsGet());
            }
            Response.Write(json);
            Response.End();
        }




        private void Post()
        {
            if (Request["_method"] != null && Request["_method"] == "DELETE")
            {
                Delete();
            }


            List<UploadHandler.UploadFileInfo> fileInfoList = new List<UploadHandler.UploadFileInfo>();
            HttpFileCollection upload = Request.Files;


            for (int i = 0; i < upload.Count; i++)
            {
                UploadHandler.UploadFileInfo fileInfo = new UploadHandler.UploadFileInfo();
                HttpPostedFile file = upload[i];
                fileInfo.type = Path.GetExtension(file.FileName).ToLower();
                fileInfo.name = Path.GetFileName(file.FileName);
                fileInfo.size = file.InputStream.Length;
                if (Request.Headers["X-File-Size"] != null)
                {
                    fileInfo.size = long.Parse(Request.Headers["X-File-Size"].ToString());
                }


                fileInfo = upload_handler.FileUploadHandle(file, fileInfo.name, fileInfo.size, fileInfo.type, fileInfo.error, i);
                fileInfoList.Add(fileInfo);
            }
            Response.Clear();
            Response.AddHeader("Vary", "Accept");
            string json = Json.Serialize<List<UploadHandler.UploadFileInfo>>(fileInfoList);
            string redirect = null;
            if (Request["redirect"] != null)
            {
                redirect = Request["Redirect"];
            }
            if (redirect != null)
            {
                Response.AddHeader("Location,", String.Format(redirect, HttpUtility.UrlEncode(json)));
                Response.End();
            }
            if (Request.ServerVariables["HTTP_ACCEPT"] != null && Request.ServerVariables["HTTP_ACCEPT"].ToString().IndexOf("application/json") >= 0)
            {
                Response.AddHeader("Content-type", "application/json");
            }
            else
            {
                Response.AddHeader("Content-type", "text/plain");
            }


            Response.Write(json);
            Response.End();


        }


        private void Delete()
        {
            string file_name = null;
            if (Request["file"] != null)
            {
                file_name = Request["file"];
            }
            string file_path = upload_handler.upload_dir + file_name;
            UploadHandler.UploadFileInfo file = upload_handler.FileObjectGet(file_name);


            bool success = File.Exists(file_path) && file_name.Length > 0 && file_name.Substring(0, 1) != ".";
            if (success)
            {
                success = false;
                File.Delete(file_path);
                success = true;
            }
            if (success)
            {
                //Delete other file versions.
                foreach (string version in file.image_versions.Keys)
                {
                    if (File.Exists(file.image_versions[version].dir + file_name))
                    {
                        File.Delete(file.image_versions[version].dir + file_name);
                    }
                }
            }
            Response.AddHeader("Content-type", "application/json");
            Response.Write(Json.Serialize<bool>(success));
            Response.End();
        }


        private string FullUrlGet()
        {
            string url = Request.Url.AbsoluteUri;
            if (url.LastIndexOf("/") > 1)
            {
                url = url.Substring(0, url.LastIndexOf("/"));
            }




            return url;
        }

    }


    public static class Json
    {
        public static T Deserialise<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                obj = (T)serializer.ReadObject(ms);
                return obj;
            }
        }
        public static string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                return Encoding.Default.GetString(ms.ToArray());
            }
        }
    }

}
