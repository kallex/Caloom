using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Xml;
using AaltoGlobalImpact.OIP;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using TheBall.CORE;

namespace TheBall
{
    public static class StorageSupport
    {
        public const string SubscriptionContainer = "subscription";
        public static string InformationTypeKey = "InformationType";
        public static string InformationObjectTypeKey = "InformationObjectType";
        public static string InformationSourcesKey = "InformationSources";
        public static string InformationType_WebTemplateValue = "WebTemplate";
        public static string InformationType_RuntimeWebTemplateValue = "RuntimeWebTemplate";
        public static string InformationType_InformationObjectValue = "InformationObject";
        public static string InformationType_RenderedWebPage = "RenderedWebPage";
        public static string InformationType_GenericContentValue = "GenericContent";

        //public static CloudTableClient CurrTableClient { get; private set; }
        public static CloudStorageAccount CurrStorageAccount { get; private set; }
        public static CloudBlobContainer CurrActiveContainer
        {
            get { return InformationContext.Current.CurrActiveContainer; }
        }
        //public static CloudBlobContainer CurrAnonPublicContainer { get; private set; }
        //public static CloudBlobContainer CurrTemplateContainer { get; private set; }
        public static CloudBlobClient CurrBlobClient
        {
            get { return InformationContext.Current.CurrBlobClient; }
        }
        public const int AccOrGrpPlusIDPathLength = 41;
        private const string ContentFolderName = "Content";
        public const int GuidLength = 36;

        public static Guid ActiveOwnerID
        {
            get { return Guid.Empty; }
        }

        public static void SetBlobInformationType(this CloudBlob blob, string informationTypeValue)
        {
            blob.Attributes.Metadata[InformationTypeKey] = informationTypeValue;
        }

        public static string GetBlobInformationType(this CloudBlob blob)
        {
            FetchMetadataIfMissing(blob);
            return blob.Attributes.Metadata[InformationTypeKey];
        }

        public static void SetBlobInformationObjectType(this CloudBlob blob, string informationObjectType)
        {
            blob.Attributes.Metadata[InformationObjectTypeKey] = informationObjectType;
        }

        public static string GetBlobInformationObjectType(this CloudBlob blob)
        {
            FetchMetadataIfMissing(blob);
            return blob.Attributes.Metadata[InformationObjectTypeKey];
        }

        public static void SetBlobInformationSources(this CloudBlob blob, InformationSourceCollection informationSourceCollection)
        {
            //if(informationSourceCollection == null)
            //{
            //    blob.Attributes.Metadata.Remove(InformationSourcesKey);
            //    return;
            //}
            //string stringValue = informationSourceCollection.SerializeToXml(noFormatting:true);
            //blob.Attributes.Metadata[InformationSourcesKey] = stringValue;
            string blobInformationType = blob.GetBlobInformationType();
            if(blobInformationType == InformationType_GenericContentValue)
                throw new InvalidDataException("Blob type of GenericContent cannot have information sources: " + blob.Name);
            InformationSourceSupport.SetInformationSources(blob.Name, informationSourceCollection);
        }

        public static InformationSourceCollection GetBlobInformationSources(this CloudBlob blob)
        {
            //FetchMetadataIfMissing(blob);
            //string stringValue = blob.Attributes.Metadata[InformationSourcesKey];
            //if (stringValue == null)
            //    return null;
            //return InformationSourceCollection.DeserializeFromXml(stringValue);
            string blobInformationType = blob.GetBlobInformationType();
            if (blobInformationType == InformationType_GenericContentValue)
                throw new InvalidDataException("Blob type of GenericContent cannot have information sources: " + blob.Name);
            var result = InformationSourceSupport.GetInformationSources(blob.Name);
            return result;
        }

        private static void FetchMetadataIfMissing(CloudBlob blob)
        {
            if(blob.Metadata.Count == 0)
            {
                try
                {
                    blob.FetchAttributes();
                    InformationContext.AddStorageTransactionToCurrent();
                } catch(StorageClientException stEx)
                {
                    if (stEx.ErrorCode == StorageErrorCode.BlobNotFound || stEx.ErrorCode == StorageErrorCode.ResourceNotFound)
                        return;
                    throw;
                }
            }
        }


        public static void InitializeWithConnectionString(string connStr, bool debugMode = false)
        {
            ServicePointManager.UseNagleAlgorithm = false;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connStr);
            CurrStorageAccount = storageAccount;
            //CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            //CurrTableClient = tableClient;

            //var activeAnonPublicContainer = blobClient.GetContainerReference("pub");
            //CurrAnonPublicContainer = activeAnonPublicContainer;
            //var activeTemplateContainer = blobClient.GetContainerReference("private-templates");
            //CurrTemplateContainer = activeTemplateContainer;
            QueueSupport.InitializeAfterStorage(debugMode:debugMode);
        }

        /*
        public static void CreatePersonalContainer(TBAccount account)
        {
            CloudBlobContainer container = GetPersonalContainer(account);
            container.CreateIfNotExist();
        }

        public static CloudBlobContainer GetPersonalContainer(TBAccount account)
        {
            return CurrBlobClient.GetContainerReference("acc" + account.ID);
        }*/

        public static CloudBlobContainer ConfigurePrivateTemplateBlobStorage(string connStr, bool deleteBlobs)
        {
            return ConfigureBlobStorageContainer(connStr, deleteBlobs, "private-templates", BlobContainerPublicAccessType.Off);
        }

        public static CloudBlobContainer ConfigureAnonWebBlobStorage(string connString, bool deleteBlobs)
        {
            return ConfigureBlobStorageContainer(connString, deleteBlobs, "anon-webcontainer", BlobContainerPublicAccessType.Blob);
        }

        public static CloudBlobContainer ConfigureBlobStorageContainer(string connString, bool deleteBlobs, string containerName, BlobContainerPublicAccessType containerAccessType)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            var anonWebContainer = blobClient.GetContainerReference(containerName);
            anonWebContainer.CreateIfNotExist();
            if (deleteBlobs)
            {
                BlobRequestOptions blobRequestOptions = new BlobRequestOptions();
                blobRequestOptions.UseFlatBlobListing = true;
                foreach (var blob in anonWebContainer.ListBlobs(blobRequestOptions))
                {
                    CloudBlockBlob blockBlob = blob as CloudBlockBlob;
                    if (blockBlob != null)
                        blockBlob.Delete();
                    CloudPageBlob pageBlob = blob as CloudPageBlob;
                    if (pageBlob != null)
                        pageBlob.Delete();
                }
            }
            BlobContainerPermissions permissions = new BlobContainerPermissions();
            permissions.PublicAccess = containerAccessType;
            anonWebContainer.SetPermissions(permissions);
            return anonWebContainer;
        }

        public static string DownloadBlobText(this CloudBlobContainer container,
            string blobPath, bool returnNullIfMissing = false)
        {
            try
            {
                var blob = container.GetBlockBlobReference(blobPath);
                return blob.DownloadText();
            }
            catch (StorageClientException stEx)
            {
                if (returnNullIfMissing && stEx.ErrorCode == StorageErrorCode.BlobNotFound)
                    return null;
                throw;
            }
        }

        public static CloudBlockBlob UploadBlobText(this CloudBlobContainer container,
            string blobPath, string textContent, string blobInformationType = null)
        {
            var blob = container.GetBlockBlobReference(blobPath);
            UploadBlobText(blob, textContent, blobInformationType);
            return blob;
        }

        public static void UploadBlobText(this CloudBlob blob, string textContent, string blobInformationType = null)
        {
            if (blobInformationType == null)
                blobInformationType = InformationType_GenericContentValue;
            blob.Attributes.Properties.ContentType = GetMimeType(Path.GetExtension(blob.Name));
            blob.SetBlobInformationType(blobInformationType);
            blob.UploadText(textContent);
        }

        public static void UploadBlobBinary(this CloudBlobContainer container,
    string blobPath, byte[] binaryContent, string blobInformationType = null)
        {
            if (blobInformationType == null)
                blobInformationType = InformationType_GenericContentValue;
            var blob = container.GetBlockBlobReference(blobPath);
            blob.Attributes.Properties.ContentType = GetMimeType(Path.GetExtension(blobPath));
            blob.SetBlobInformationType(blobInformationType);
            blob.UploadByteArray(binaryContent);
            InformationContext.AddStorageTransactionToCurrent();
        }

        public static void UploadBlobStream(this CloudBlobContainer container,
    string blobPath, Stream streamContent, string blobInformationType = null)
        {
            if (blobInformationType == null)
                blobInformationType = InformationType_GenericContentValue;
            var blob = container.GetBlockBlobReference(blobPath);
            blob.Attributes.Properties.ContentType = GetMimeType(Path.GetExtension(blobPath));
            blob.SetBlobInformationType(blobInformationType);
            blob.UploadFromStream(streamContent);
            InformationContext.AddStorageTransactionToCurrent();
        }

        public static string GetContainerNameFromHostName(string hostName)
        {
            return hostName.Replace(".", "-");
        }

        public static string GetMimeType(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            switch (extension)
            {
                #region Big freaking list of mime types
                // combination of values from Windows 7 Registry and  
                // from C:\Windows\System32\inetsrv\config\applicationHost.config 
                // some added, including .7z and .dat 

                case ".323": return "text/h323";
                case ".3g2": return "video/3gpp2";
                case ".3gp": return "video/3gpp";
                case ".3gp2": return "video/3gpp2";
                case ".3gpp": return "video/3gpp";
                case ".7z": return "application/x-7z-compressed";
                case ".aa": return "audio/audible";
                case ".AAC": return "audio/aac";
                case ".aaf": return "application/octet-stream";
                case ".aax": return "audio/vnd.audible.aax";
                case ".ac3": return "audio/ac3";
                case ".aca": return "application/octet-stream";
                case ".accda": return "application/msaccess.addin";
                case ".accdb": return "application/msaccess";
                case ".accdc": return "application/msaccess.cab";
                case ".accde": return "application/msaccess";
                case ".accdr": return "application/msaccess.runtime";
                case ".accdt": return "application/msaccess";
                case ".accdw": return "application/msaccess.webapplication";
                case ".accft": return "application/msaccess.ftemplate";
                case ".acx": return "application/internet-property-stream";
                case ".AddIn": return "text/xml";
                case ".ade": return "application/msaccess";
                case ".adobebridge": return "application/x-bridge-url";
                case ".adp": return "application/msaccess";
                case ".ADT": return "audio/vnd.dlna.adts";
                case ".ADTS": return "audio/aac";
                case ".afm": return "application/octet-stream";
                case ".ai": return "application/postscript";
                case ".aif": return "audio/x-aiff";
                case ".aifc": return "audio/aiff";
                case ".aiff": return "audio/aiff";
                case ".air": return "application/vnd.adobe.air-application-installer-package+zip";
                case ".amc": return "application/x-mpeg";
                case ".application": return "application/x-ms-application";
                case ".art": return "image/x-jg";
                case ".asa": return "application/xml";
                case ".asax": return "application/xml";
                case ".ascx": return "application/xml";
                case ".asd": return "application/octet-stream";
                case ".asf": return "video/x-ms-asf";
                case ".ashx": return "application/xml";
                case ".asi": return "application/octet-stream";
                case ".asm": return "text/plain";
                case ".asmx": return "application/xml";
                case ".aspx": return "application/xml";
                case ".asr": return "video/x-ms-asf";
                case ".asx": return "video/x-ms-asf";
                case ".atom": return "application/atom+xml";
                case ".au": return "audio/basic";
                case ".avi": return "video/x-msvideo";
                case ".axs": return "application/olescript";
                case ".bas": return "text/plain";
                case ".bcpio": return "application/x-bcpio";
                case ".bin": return "application/octet-stream";
                case ".bmp": return "image/bmp";
                case ".c": return "text/plain";
                case ".cab": return "application/octet-stream";
                case ".caf": return "audio/x-caf";
                case ".calx": return "application/vnd.ms-office.calx";
                case ".cat": return "application/vnd.ms-pki.seccat";
                case ".cc": return "text/plain";
                case ".cd": return "text/plain";
                case ".cdda": return "audio/aiff";
                case ".cdf": return "application/x-cdf";
                case ".cer": return "application/x-x509-ca-cert";
                case ".chm": return "application/octet-stream";
                case ".class": return "application/x-java-applet";
                case ".clp": return "application/x-msclip";
                case ".cmx": return "image/x-cmx";
                case ".cnf": return "text/plain";
                case ".cod": return "image/cis-cod";
                case ".config": return "application/xml";
                case ".contact": return "text/x-ms-contact";
                case ".coverage": return "application/xml";
                case ".cpio": return "application/x-cpio";
                case ".cpp": return "text/plain";
                case ".crd": return "application/x-mscardfile";
                case ".crl": return "application/pkix-crl";
                case ".crt": return "application/x-x509-ca-cert";
                case ".cs": return "text/plain";
                case ".csdproj": return "text/plain";
                case ".csh": return "application/x-csh";
                case ".csproj": return "text/plain";
                case ".css": return "text/css";
                case ".csv": return "application/octet-stream";
                case ".cur": return "application/octet-stream";
                case ".cxx": return "text/plain";
                case ".dat": return "application/octet-stream";
                case ".datasource": return "application/xml";
                case ".dbproj": return "text/plain";
                case ".dcr": return "application/x-director";
                case ".def": return "text/plain";
                case ".deploy": return "application/octet-stream";
                case ".der": return "application/x-x509-ca-cert";
                case ".dgml": return "application/xml";
                case ".dib": return "image/bmp";
                case ".dif": return "video/x-dv";
                case ".dir": return "application/x-director";
                case ".disco": return "text/xml";
                case ".dll": return "application/x-msdownload";
                case ".dll.config": return "text/xml";
                case ".dlm": return "text/dlm";
                case ".doc": return "application/msword";
                case ".docm": return "application/vnd.ms-word.document.macroEnabled.12";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".dot": return "application/msword";
                case ".dotm": return "application/vnd.ms-word.template.macroEnabled.12";
                case ".dotx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                case ".dsp": return "application/octet-stream";
                case ".dsw": return "text/plain";
                case ".dtd": return "text/xml";
                case ".dtsConfig": return "text/xml";
                case ".dv": return "video/x-dv";
                case ".dvi": return "application/x-dvi";
                case ".dwf": return "drawing/x-dwf";
                case ".dwp": return "application/octet-stream";
                case ".dxr": return "application/x-director";
                case ".eml": return "message/rfc822";
                case ".emz": return "application/octet-stream";
                case ".eot": return "application/octet-stream";
                case ".eps": return "application/postscript";
                case ".etl": return "application/etl";
                case ".etx": return "text/x-setext";
                case ".evy": return "application/envoy";
                case ".exe": return "application/octet-stream";
                case ".exe.config": return "text/xml";
                case ".fdf": return "application/vnd.fdf";
                case ".fif": return "application/fractals";
                case ".filters": return "Application/xml";
                case ".fla": return "application/octet-stream";
                case ".flr": return "x-world/x-vrml";
                case ".flv": return "video/x-flv";
                case ".fsscript": return "application/fsharp-script";
                case ".fsx": return "application/fsharp-script";
                case ".generictest": return "application/xml";
                case ".gif": return "image/gif";
                case ".group": return "text/x-ms-group";
                case ".gsm": return "audio/x-gsm";
                case ".gtar": return "application/x-gtar";
                case ".gz": return "application/x-gzip";
                case ".h": return "text/plain";
                case ".hdf": return "application/x-hdf";
                case ".hdml": return "text/x-hdml";
                case ".hhc": return "application/x-oleobject";
                case ".hhk": return "application/octet-stream";
                case ".hhp": return "application/octet-stream";
                case ".hlp": return "application/winhlp";
                case ".hpp": return "text/plain";
                case ".hqx": return "application/mac-binhex40";
                case ".hta": return "application/hta";
                case ".htc": return "text/x-component";
                case ".htm": return "text/html";
                case ".html": return "text/html";
                case ".htt": return "text/webviewhtml";
                case ".hxa": return "application/xml";
                case ".hxc": return "application/xml";
                case ".hxd": return "application/octet-stream";
                case ".hxe": return "application/xml";
                case ".hxf": return "application/xml";
                case ".hxh": return "application/octet-stream";
                case ".hxi": return "application/octet-stream";
                case ".hxk": return "application/xml";
                case ".hxq": return "application/octet-stream";
                case ".hxr": return "application/octet-stream";
                case ".hxs": return "application/octet-stream";
                case ".hxt": return "text/html";
                case ".hxv": return "application/xml";
                case ".hxw": return "application/octet-stream";
                case ".hxx": return "text/plain";
                case ".i": return "text/plain";
                case ".ico": return "image/x-icon";
                case ".ics": return "application/octet-stream";
                case ".idl": return "text/plain";
                case ".ief": return "image/ief";
                case ".iii": return "application/x-iphone";
                case ".inc": return "text/plain";
                case ".inf": return "application/octet-stream";
                case ".inl": return "text/plain";
                case ".ins": return "application/x-internet-signup";
                case ".ipa": return "application/x-itunes-ipa";
                case ".ipg": return "application/x-itunes-ipg";
                case ".ipproj": return "text/plain";
                case ".ipsw": return "application/x-itunes-ipsw";
                case ".iqy": return "text/x-ms-iqy";
                case ".isp": return "application/x-internet-signup";
                case ".ite": return "application/x-itunes-ite";
                case ".itlp": return "application/x-itunes-itlp";
                case ".itms": return "application/x-itunes-itms";
                case ".itpc": return "application/x-itunes-itpc";
                case ".IVF": return "video/x-ivf";
                case ".jar": return "application/java-archive";
                case ".java": return "application/octet-stream";
                case ".jck": return "application/liquidmotion";
                case ".jcz": return "application/liquidmotion";
                case ".jfif": return "image/pjpeg";
                case ".jnlp": return "application/x-java-jnlp-file";
                case ".jpb": return "application/octet-stream";
                case ".jpe": return "image/jpeg";
                case ".jpeg": return "image/jpeg";
                case ".jpg": return "image/jpeg";
                case ".js": return "application/x-javascript";
                case ".json": return "application/json";
                case ".jsx": return "text/jscript";
                case ".jsxbin": return "text/plain";
                case ".latex": return "application/x-latex";
                case ".library-ms": return "application/windows-library+xml";
                case ".lit": return "application/x-ms-reader";
                case ".loadtest": return "application/xml";
                case ".lpk": return "application/octet-stream";
                case ".lsf": return "video/x-la-asf";
                case ".lst": return "text/plain";
                case ".lsx": return "video/x-la-asf";
                case ".lzh": return "application/octet-stream";
                case ".m13": return "application/x-msmediaview";
                case ".m14": return "application/x-msmediaview";
                case ".m1v": return "video/mpeg";
                case ".m2t": return "video/vnd.dlna.mpeg-tts";
                case ".m2ts": return "video/vnd.dlna.mpeg-tts";
                case ".m2v": return "video/mpeg";
                case ".m3u": return "audio/x-mpegurl";
                case ".m3u8": return "audio/x-mpegurl";
                case ".m4a": return "audio/m4a";
                case ".m4b": return "audio/m4b";
                case ".m4p": return "audio/m4p";
                case ".m4r": return "audio/x-m4r";
                case ".m4v": return "video/x-m4v";
                case ".mac": return "image/x-macpaint";
                case ".mak": return "text/plain";
                case ".man": return "application/x-troff-man";
                case ".manifest": return "application/x-ms-manifest";
                case ".map": return "text/plain";
                case ".master": return "application/xml";
                case ".mda": return "application/msaccess";
                case ".mdb": return "application/x-msaccess";
                case ".mde": return "application/msaccess";
                case ".mdp": return "application/octet-stream";
                case ".me": return "application/x-troff-me";
                case ".mfp": return "application/x-shockwave-flash";
                case ".mht": return "message/rfc822";
                case ".mhtml": return "message/rfc822";
                case ".mid": return "audio/mid";
                case ".midi": return "audio/mid";
                case ".mix": return "application/octet-stream";
                case ".mk": return "text/plain";
                case ".mmf": return "application/x-smaf";
                case ".mno": return "text/xml";
                case ".mny": return "application/x-msmoney";
                case ".mod": return "video/mpeg";
                case ".mov": return "video/quicktime";
                case ".movie": return "video/x-sgi-movie";
                case ".mp2": return "video/mpeg";
                case ".mp2v": return "video/mpeg";
                case ".mp3": return "audio/mpeg";
                case ".mp4": return "video/mp4";
                case ".mp4v": return "video/mp4";
                case ".mpa": return "video/mpeg";
                case ".mpe": return "video/mpeg";
                case ".mpeg": return "video/mpeg";
                case ".mpf": return "application/vnd.ms-mediapackage";
                case ".mpg": return "video/mpeg";
                case ".mpp": return "application/vnd.ms-project";
                case ".mpv2": return "video/mpeg";
                case ".mqv": return "video/quicktime";
                case ".ms": return "application/x-troff-ms";
                case ".msi": return "application/octet-stream";
                case ".mso": return "application/octet-stream";
                case ".mts": return "video/vnd.dlna.mpeg-tts";
                case ".mtx": return "application/xml";
                case ".mvb": return "application/x-msmediaview";
                case ".mvc": return "application/x-miva-compiled";
                case ".mxp": return "application/x-mmxp";
                case ".nc": return "application/x-netcdf";
                case ".nsc": return "video/x-ms-asf";
                case ".nws": return "message/rfc822";
                case ".ocx": return "application/octet-stream";
                case ".oda": return "application/oda";
                case ".odc": return "text/x-ms-odc";
                case ".odh": return "text/plain";
                case ".odl": return "text/plain";
                case ".odp": return "application/vnd.oasis.opendocument.presentation";
                case ".ods": return "application/oleobject";
                case ".odt": return "application/vnd.oasis.opendocument.text";
                case ".one": return "application/onenote";
                case ".onea": return "application/onenote";
                case ".onepkg": return "application/onenote";
                case ".onetmp": return "application/onenote";
                case ".onetoc": return "application/onenote";
                case ".onetoc2": return "application/onenote";
                case ".orderedtest": return "application/xml";
                case ".osdx": return "application/opensearchdescription+xml";
                case ".p10": return "application/pkcs10";
                case ".p12": return "application/x-pkcs12";
                case ".p7b": return "application/x-pkcs7-certificates";
                case ".p7c": return "application/pkcs7-mime";
                case ".p7m": return "application/pkcs7-mime";
                case ".p7r": return "application/x-pkcs7-certreqresp";
                case ".p7s": return "application/pkcs7-signature";
                case ".pbm": return "image/x-portable-bitmap";
                case ".pcast": return "application/x-podcast";
                case ".pct": return "image/pict";
                case ".pcx": return "application/octet-stream";
                case ".pcz": return "application/octet-stream";
                case ".pdf": return "application/pdf";
                case ".pfb": return "application/octet-stream";
                case ".pfm": return "application/octet-stream";
                case ".pfx": return "application/x-pkcs12";
                case ".pgm": return "image/x-portable-graymap";
                case ".phtml": return "text/html";
                case ".pic": return "image/pict";
                case ".pict": return "image/pict";
                case ".pkgdef": return "text/plain";
                case ".pkgundef": return "text/plain";
                case ".pko": return "application/vnd.ms-pki.pko";
                case ".pls": return "audio/scpls";
                case ".pma": return "application/x-perfmon";
                case ".pmc": return "application/x-perfmon";
                case ".pml": return "application/x-perfmon";
                case ".pmr": return "application/x-perfmon";
                case ".pmw": return "application/x-perfmon";
                case ".png": return "image/png";
                case ".pnm": return "image/x-portable-anymap";
                case ".pnt": return "image/x-macpaint";
                case ".pntg": return "image/x-macpaint";
                case ".pnz": return "image/png";
                case ".pot": return "application/vnd.ms-powerpoint";
                case ".potm": return "application/vnd.ms-powerpoint.template.macroEnabled.12";
                case ".potx": return "application/vnd.openxmlformats-officedocument.presentationml.template";
                case ".ppa": return "application/vnd.ms-powerpoint";
                case ".ppam": return "application/vnd.ms-powerpoint.addin.macroEnabled.12";
                case ".ppm": return "image/x-portable-pixmap";
                case ".pps": return "application/vnd.ms-powerpoint";
                case ".ppsm": return "application/vnd.ms-powerpoint.slideshow.macroEnabled.12";
                case ".ppsx": return "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
                case ".ppt": return "application/vnd.ms-powerpoint";
                case ".pptm": return "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
                case ".pptx": return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case ".prf": return "application/pics-rules";
                case ".prm": return "application/octet-stream";
                case ".prx": return "application/octet-stream";
                case ".ps": return "application/postscript";
                case ".psc1": return "application/PowerShell";
                case ".psd": return "application/octet-stream";
                case ".psess": return "application/xml";
                case ".psm": return "application/octet-stream";
                case ".psp": return "application/octet-stream";
                case ".pub": return "application/x-mspublisher";
                case ".pwz": return "application/vnd.ms-powerpoint";
                case ".qht": return "text/x-html-insertion";
                case ".qhtm": return "text/x-html-insertion";
                case ".qt": return "video/quicktime";
                case ".qti": return "image/x-quicktime";
                case ".qtif": return "image/x-quicktime";
                case ".qtl": return "application/x-quicktimeplayer";
                case ".qxd": return "application/octet-stream";
                case ".ra": return "audio/x-pn-realaudio";
                case ".ram": return "audio/x-pn-realaudio";
                case ".rar": return "application/octet-stream";
                case ".ras": return "image/x-cmu-raster";
                case ".rat": return "application/rat-file";
                case ".rc": return "text/plain";
                case ".rc2": return "text/plain";
                case ".rct": return "text/plain";
                case ".rdlc": return "application/xml";
                case ".resx": return "application/xml";
                case ".rf": return "image/vnd.rn-realflash";
                case ".rgb": return "image/x-rgb";
                case ".rgs": return "text/plain";
                case ".rm": return "application/vnd.rn-realmedia";
                case ".rmi": return "audio/mid";
                case ".rmp": return "application/vnd.rn-rn_music_package";
                case ".roff": return "application/x-troff";
                case ".rpm": return "audio/x-pn-realaudio-plugin";
                case ".rqy": return "text/x-ms-rqy";
                case ".rtf": return "application/rtf";
                case ".rtx": return "text/richtext";
                case ".ruleset": return "application/xml";
                case ".s": return "text/plain";
                case ".safariextz": return "application/x-safari-safariextz";
                case ".scd": return "application/x-msschedule";
                case ".sct": return "text/scriptlet";
                case ".sd2": return "audio/x-sd2";
                case ".sdp": return "application/sdp";
                case ".sea": return "application/octet-stream";
                case ".searchConnector-ms": return "application/windows-search-connector+xml";
                case ".setpay": return "application/set-payment-initiation";
                case ".setreg": return "application/set-registration-initiation";
                case ".settings": return "application/xml";
                case ".sgimb": return "application/x-sgimb";
                case ".sgml": return "text/sgml";
                case ".sh": return "application/x-sh";
                case ".shar": return "application/x-shar";
                case ".shtml": return "text/html";
                case ".sit": return "application/x-stuffit";
                case ".sitemap": return "application/xml";
                case ".skin": return "application/xml";
                case ".sldm": return "application/vnd.ms-powerpoint.slide.macroEnabled.12";
                case ".sldx": return "application/vnd.openxmlformats-officedocument.presentationml.slide";
                case ".slk": return "application/vnd.ms-excel";
                case ".sln": return "text/plain";
                case ".slupkg-ms": return "application/x-ms-license";
                case ".smd": return "audio/x-smd";
                case ".smi": return "application/octet-stream";
                case ".smx": return "audio/x-smd";
                case ".smz": return "audio/x-smd";
                case ".snd": return "audio/basic";
                case ".snippet": return "application/xml";
                case ".snp": return "application/octet-stream";
                case ".sol": return "text/plain";
                case ".sor": return "text/plain";
                case ".spc": return "application/x-pkcs7-certificates";
                case ".spl": return "application/futuresplash";
                case ".src": return "application/x-wais-source";
                case ".srf": return "text/plain";
                case ".SSISDeploymentManifest": return "text/xml";
                case ".ssm": return "application/streamingmedia";
                case ".sst": return "application/vnd.ms-pki.certstore";
                case ".stl": return "application/vnd.ms-pki.stl";
                case ".sv4cpio": return "application/x-sv4cpio";
                case ".sv4crc": return "application/x-sv4crc";
                case ".svc": return "application/xml";
                case ".swf": return "application/x-shockwave-flash";
                case ".t": return "application/x-troff";
                case ".tar": return "application/x-tar";
                case ".tcl": return "application/x-tcl";
                case ".testrunconfig": return "application/xml";
                case ".testsettings": return "application/xml";
                case ".tex": return "application/x-tex";
                case ".texi": return "application/x-texinfo";
                case ".texinfo": return "application/x-texinfo";
                case ".tgz": return "application/x-compressed";
                case ".thmx": return "application/vnd.ms-officetheme";
                case ".thn": return "application/octet-stream";
                case ".tif": return "image/tiff";
                case ".tiff": return "image/tiff";
                case ".tlh": return "text/plain";
                case ".tli": return "text/plain";
                case ".toc": return "application/octet-stream";
                case ".tr": return "application/x-troff";
                case ".trm": return "application/x-msterminal";
                case ".trx": return "application/xml";
                case ".ts": return "video/vnd.dlna.mpeg-tts";
                case ".tsv": return "text/tab-separated-values";
                case ".ttf": return "application/octet-stream";
                case ".tts": return "video/vnd.dlna.mpeg-tts";
                case ".txt": return "text/plain";
                case ".u32": return "application/octet-stream";
                case ".uls": return "text/iuls";
                case ".user": return "text/plain";
                case ".ustar": return "application/x-ustar";
                case ".vb": return "text/plain";
                case ".vbdproj": return "text/plain";
                case ".vbk": return "video/mpeg";
                case ".vbproj": return "text/plain";
                case ".vbs": return "text/vbscript";
                case ".vcf": return "text/x-vcard";
                case ".vcproj": return "Application/xml";
                case ".vcs": return "text/plain";
                case ".vcxproj": return "Application/xml";
                case ".vddproj": return "text/plain";
                case ".vdp": return "text/plain";
                case ".vdproj": return "text/plain";
                case ".vdx": return "application/vnd.ms-visio.viewer";
                case ".vml": return "text/xml";
                case ".vscontent": return "application/xml";
                case ".vsct": return "text/xml";
                case ".vsd": return "application/vnd.visio";
                case ".vsi": return "application/ms-vsi";
                case ".vsix": return "application/vsix";
                case ".vsixlangpack": return "text/xml";
                case ".vsixmanifest": return "text/xml";
                case ".vsmdi": return "application/xml";
                case ".vspscc": return "text/plain";
                case ".vss": return "application/vnd.visio";
                case ".vsscc": return "text/plain";
                case ".vssettings": return "text/xml";
                case ".vssscc": return "text/plain";
                case ".vst": return "application/vnd.visio";
                case ".vstemplate": return "text/xml";
                case ".vsto": return "application/x-ms-vsto";
                case ".vsw": return "application/vnd.visio";
                case ".vsx": return "application/vnd.visio";
                case ".vtx": return "application/vnd.visio";
                case ".wav": return "audio/wav";
                case ".wave": return "audio/wav";
                case ".wax": return "audio/x-ms-wax";
                case ".wbk": return "application/msword";
                case ".wbmp": return "image/vnd.wap.wbmp";
                case ".wcm": return "application/vnd.ms-works";
                case ".wdb": return "application/vnd.ms-works";
                case ".wdp": return "image/vnd.ms-photo";
                case ".webarchive": return "application/x-safari-webarchive";
                case ".webtest": return "application/xml";
                case ".wiq": return "application/xml";
                case ".wiz": return "application/msword";
                case ".wks": return "application/vnd.ms-works";
                case ".WLMP": return "application/wlmoviemaker";
                case ".wlpginstall": return "application/x-wlpg-detect";
                case ".wlpginstall3": return "application/x-wlpg3-detect";
                case ".wm": return "video/x-ms-wm";
                case ".wma": return "audio/x-ms-wma";
                case ".wmd": return "application/x-ms-wmd";
                case ".WMD": return "application/x-ms-wmd";
                case ".wmf": return "application/x-msmetafile";
                case ".wml": return "text/vnd.wap.wml";
                case ".wmlc": return "application/vnd.wap.wmlc";
                case ".wmls": return "text/vnd.wap.wmlscript";
                case ".wmlsc": return "application/vnd.wap.wmlscriptc";
                case ".wmp": return "video/x-ms-wmp";
                case ".wmv": return "video/x-ms-wmv";
                case ".wmx": return "video/x-ms-wmx";
                case ".wmz": return "application/x-ms-wmz";
                case ".wpl": return "application/vnd.ms-wpl";
                case ".wps": return "application/vnd.ms-works";
                case ".wri": return "application/x-mswrite";
                case ".wrl": return "x-world/x-vrml";
                case ".wrz": return "x-world/x-vrml";
                case ".wsc": return "text/scriptlet";
                case ".wsdl": return "text/xml";
                case ".wvx": return "video/x-ms-wvx";
                case ".x": return "application/directx";
                case ".xaf": return "x-world/x-vrml";
                case ".xaml": return "application/xaml+xml";
                case ".xap": return "application/x-silverlight-app";
                case ".xbap": return "application/x-ms-xbap";
                case ".xbm": return "image/x-xbitmap";
                case ".xdr": return "text/plain";
                case ".xht": return "application/xhtml+xml";
                case ".xhtml": return "application/xhtml+xml";
                case ".xla": return "application/vnd.ms-excel";
                case ".xlam": return "application/vnd.ms-excel.addin.macroEnabled.12";
                case ".xlc": return "application/vnd.ms-excel";
                case ".xld": return "application/vnd.ms-excel";
                case ".xlk": return "application/vnd.ms-excel";
                case ".xll": return "application/vnd.ms-excel";
                case ".xlm": return "application/vnd.ms-excel";
                case ".xls": return "application/vnd.ms-excel";
                case ".xlsb": return "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
                case ".xlsm": return "application/vnd.ms-excel.sheet.macroEnabled.12";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".xlt": return "application/vnd.ms-excel";
                case ".xltm": return "application/vnd.ms-excel.template.macroEnabled.12";
                case ".xltx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
                case ".xlw": return "application/vnd.ms-excel";
                case ".xml": return "text/xml";
                case ".xmta": return "application/xml";
                case ".xof": return "x-world/x-vrml";
                case ".XOML": return "text/plain";
                case ".xpm": return "image/x-xpixmap";
                case ".xps": return "application/vnd.ms-xpsdocument";
                case ".xrm-ms": return "text/xml";
                case ".xsc": return "application/xml";
                case ".xsd": return "text/xml";
                case ".xsf": return "text/xml";
                case ".xsl": return "text/xml";
                case ".xslt": return "text/xml";
                case ".xsn": return "application/octet-stream";
                case ".xss": return "application/xml";
                case ".xtp": return "application/octet-stream";
                case ".xwd": return "image/x-xwindowdump";
                case ".z": return "application/x-compress";
                case ".zip": return "application/x-zip-compressed";

                #endregion

                default:
                    // if you have logging, log: "No mime type is registered for extension: " + extension); 
                    return "application/octet-stream";
            }
        }

        public static void ReconnectMastersAndCollections(this IInformationObject informationObject, bool updateContents)
        {
            VirtualOwner owner = VirtualOwner.FigureOwner(informationObject);
            if (updateContents)
            {
                IBeforeStoreHandler beforeStoreHandler = informationObject as IBeforeStoreHandler;
                if(beforeStoreHandler != null)
                    beforeStoreHandler.PerformBeforeStoreUpdate();
            }
            List<IInformationObject> masterObjects = new List<IInformationObject>();
            List<IInformationObject> masterReferringCollections = new List<IInformationObject>();
            informationObject.FindObjectsFromTree(masterObjects, candidate => candidate.IsIndependentMaster, true);
            informationObject.FindObjectsFromTree(
                masterReferringCollections, candidate =>
                                                {
                                                    IInformationCollection informationCollection =
                                                        candidate as IInformationCollection;
                                                    return informationCollection != null &&
                                                           informationCollection.IsMasterCollection;
                                                }, true);
            foreach(var referenceMaster in masterObjects)
            {
                if (referenceMaster == informationObject)
                    continue;
                FixOwnerLocation(referenceMaster, owner);
                var realMaster = referenceMaster.RetrieveMaster(true);
                SubscribeSupport.SetReferenceSubscriptionToMaster(containerObject: informationObject, masterInstance: realMaster, referenceInstance: referenceMaster);
                if (updateContents)
                {
                    if (referenceMaster.MasterETag != realMaster.ETag)
                    {
                        referenceMaster.UpdateMasterValueTreeFromOtherInstance(realMaster);
                        referenceMaster.MasterETag = realMaster.ETag;
                    }
                }
            }
            foreach(IInformationCollection referringCollection in masterReferringCollections)
            {
                if (referringCollection == informationObject)
                    continue;
                IInformationObject referringObject = (IInformationObject) referringCollection;
                FixOwnerLocation(referringObject, owner);
                string masterLocation = referringCollection.GetMasterLocation();
                // Don't self master
                if (masterLocation == referringObject.RelativeLocation)
                    continue;
                SubscribeSupport.SetCollectionSubscriptionToMaster(containerObject: informationObject,
                                                                   masterCollectionLocation: masterLocation,
                                                                   collectionType: referringCollection.GetType());
                if(updateContents)
                {
                    var masterInstance = referringCollection.GetMasterInstance();
                    informationObject.UpdateCollections(masterInstance: masterInstance);
                }
            }
            if (updateContents)
                StoreInformation(informationObject);
        }

        public static void FixOwnerLocation(IInformationObject informationObject, IContainerOwner owner)
        {
            string ownerLocation = GetBlobOwnerAddress(owner, informationObject.RelativeLocation);
            informationObject.RelativeLocation = ownerLocation;
        }

        public static CloudBlob StoreInformationMasterFirst(this IInformationObject informationObject, IContainerOwner owner, bool reconnectMastersAndCollections)
        {
            IBeforeStoreHandler beforeStoreHandler = informationObject as IBeforeStoreHandler;
            if(beforeStoreHandler != null)
                beforeStoreHandler.PerformBeforeStoreUpdate();
            Dictionary<string, List<IInformationObject>> modifiedMasters =
                informationObject.CollectMasterObjects(candidate => candidate.IsInstanceTreeModified);
            foreach (var key in modifiedMasters.Keys)
            {
                var masters = modifiedMasters[key];
                bool masterSaving = masters.Contains(informationObject);
                if (masterSaving)
                    continue;
                var referenceInstance = modifiedMasters[key].First(); // We take first changed, even though there would be several
                if (referenceInstance == informationObject) // Don't master-manage the currently being saved object
                    continue;
                FixOwnerLocation(referenceInstance, owner);
                // TODO: 
                bool masterDidNotExist;
                var realMaster = referenceInstance.RetrieveMaster(true, out masterDidNotExist);
                SubscribeSupport.SetReferenceSubscriptionToMaster(containerObject: informationObject,
                                                 referenceInstance: referenceInstance, masterInstance: realMaster);
                // Compare MasterEtag for reference vs master - throw Exception on mismatch
                if(masterDidNotExist == false)
                {
                    if (referenceInstance.MasterETag != realMaster.ETag)
                    {
                        WorkerSupport.UpdateContainerFromMaster(informationObject.RelativeLocation, informationObject.GetType().FullName,
                            realMaster.RelativeLocation, realMaster.GetType().FullName);
                        throw new ReferenceOutdatedException(containerObject: informationObject, referenceInstance: referenceInstance, masterInstance: realMaster);
                    }
                    IBeforeStoreHandler referenceStoreHandler = referenceInstance as IBeforeStoreHandler;
                    if(referenceStoreHandler != null)
                        referenceStoreHandler.PerformBeforeStoreUpdate();
                    // Update and store real master, get the up-to-date ETag for reference
                    realMaster.UpdateMasterValueTreeFromOtherInstance(referenceInstance);
                    StorageSupport.StoreInformation(realMaster, owner);
                }
                referenceInstance.MasterETag = realMaster.ETag;
            }
            CloudBlob storedResult = StoreInformation(informationObject, owner);
            if(reconnectMastersAndCollections)
            {
                informationObject.ReconnectMastersAndCollections(true);
            }
            return storedResult;
        }

        public static CloudBlob StoreInformation(this IInformationObject informationObject, IContainerOwner owner = null, bool overwriteIfExists = false)
        {
            string location = owner != null
                                  ? GetBlobOwnerAddress(owner, informationObject.RelativeLocation)
                                  : informationObject.RelativeLocation;
            // Updating the relative location just in case - as there shouldn't be a mismatch - critical for master objects
            informationObject.RelativeLocation = location;
            Type informationObjectType = informationObject.GetType();
            var dataContent = SerializeInformationObjectToBuffer(informationObject);
            //memoryStream.Seek(0, SeekOrigin.Begin);
            CloudBlob blob = CurrActiveContainer.GetBlobReference(location);
            BlobRequestOptions options = new BlobRequestOptions();
            options.RetryPolicy = RetryPolicies.Retry(10, TimeSpan.FromSeconds(3));
            string etag = informationObject.ETag;
            bool isNewBlob = etag == null;
            if (!overwriteIfExists)
            {
                if (etag != null)
                    options.AccessCondition = AccessCondition.IfMatch(etag);
                else
                    options.AccessCondition = AccessCondition.IfNoneMatch("*");
            }
            blob.SetBlobInformationType(InformationType_InformationObjectValue);
            blob.SetBlobInformationObjectType(informationObjectType.FullName);
            blob.UploadByteArray(dataContent, options);
            InformationContext.AddStorageTransactionToCurrent();
            informationObject.ETag = blob.Properties.ETag;
            IAdditionalFormatProvider additionalFormatProvider = informationObject as IAdditionalFormatProvider;
            if(additionalFormatProvider != null)
            {
                var additionalContentToStore = additionalFormatProvider.GetAdditionalContentToStore(informationObject.ETag);
                foreach(var additionalContent in additionalContentToStore)
                {
                    string contentLocation = blob.Name + "." + additionalContent.Extension;
                    CurrActiveContainer.UploadBlobBinary(contentLocation, additionalContent.Content);
                    InformationContext.AddStorageTransactionToCurrent();
                }
            }
            if (isNewBlob)
            {
                informationObject.InitializeDefaultSubscribers(owner);
            }
            informationObject.PostStoringExecute(owner);
            SubscribeSupport.NotifySubscribers(informationObject.RelativeLocation);
            Debug.WriteLine(String.Format("Wrote: {0} ID {1}", informationObject.GetType().Name,
                informationObject.ID));
            InformationContext.Current.ObjectStoredNotification(informationObject, 
                isNewBlob ? InformationContext.ObjectChangeType.N_New : InformationContext.ObjectChangeType.M_Modified);
            return blob;
        }


        public static IInformationObject RetrieveInformation(string relativeLocation, string eTag, IContainerOwner owner)
        {
            string typeName = resolveTypeNameFromRelativeLocation(relativeLocation);
            return RetrieveInformation(relativeLocation, typeName, eTag, owner);
        }

        private static string resolveTypeNameFromRelativeLocation(string relativeLocation)
        {
            string[] partsOfLocation = relativeLocation.Split('/');
            VirtualOwner owner = VirtualOwner.FigureOwner(relativeLocation);
            return partsOfLocation[2] + "." + partsOfLocation[3];
        }

        public static IInformationObject RetrieveInformation(string relativeLocation, string typeName, string eTag = null, IContainerOwner owner = null)
        {
            Type type = Assembly.GetExecutingAssembly().GetType(typeName);
            if(type == null)
                throw new InvalidDataException("Type not found: " + typeName);
            return RetrieveInformation(relativeLocation, type, eTag, owner);
        }

        public static IInformationObject RetrieveInformationWithBlob(string relativeLocation, string typeName, out CloudBlob blob, string eTag = null, IContainerOwner owner = null)
        {
            Type type = Assembly.GetExecutingAssembly().GetType(typeName);
            if (type == null)
                throw new InvalidDataException("Type not found: " + typeName);
            return RetrieveInformationWithBlob(relativeLocation, type, out blob, eTag, owner);
        }


        public static IInformationObject RetrieveInformation(string relativeLocation, Type typeToRetrieve, string eTag = null, IContainerOwner owner = null)
        {
            CloudBlob blob;
            return RetrieveInformationWithBlob(relativeLocation, typeToRetrieve, out blob, eTag, owner);
        }

        public static IInformationObject RetrieveInformationWithBlob(string relativeLocation, Type typeToRetrieve, out CloudBlob blob, string eTag = null, IContainerOwner owner = null)
        {
            if (owner != null)
                relativeLocation = GetBlobOwnerAddress(owner, relativeLocation);
            blob = CurrActiveContainer.GetBlobReference(relativeLocation);
            MemoryStream memoryStream = new MemoryStream();
            string blobEtag = null;
            try
            {
                BlobRequestOptions options = new BlobRequestOptions();
                options.RetryPolicy = RetryPolicies.Retry(10, TimeSpan.FromSeconds(3));
                if (eTag != null)
                    options.AccessCondition = AccessCondition.IfMatch(eTag);
                blob.DownloadToStream(memoryStream, options);
                InformationContext.AddStorageTransactionToCurrent();
                blobEtag = blob.Properties.ETag;
            }
            catch (StorageClientException stEx)
            {
                if (stEx.ErrorCode == StorageErrorCode.BlobNotFound)
                    return null;
                throw;
            }
            //if (memoryStream.Length == 0)
            //    return null;
            memoryStream.Seek(0, SeekOrigin.Begin);
            var informationObject = DeserializeInformationObjectFromStream(typeToRetrieve, memoryStream);
            informationObject.ETag = blobEtag;
            //informationObject.RelativeLocation = blob.Attributes.Metadata["RelativeLocation"];
            informationObject.RelativeLocation = relativeLocation;
            informationObject.SetInstanceTreeValuesAsUnmodified();
            Debug.WriteLine(String.Format("Read: {0} ID {1}", informationObject.GetType().Name,
                informationObject.ID));
            return informationObject;
        }

        private static IInformationObject DeserializeInformationObjectFromStream(Type typeToRetrieve, MemoryStream memoryStream)
        {
            StorageSerializationType storageSerializationType = getStorageSerializationType(typeToRetrieve);
            IInformationObject informationObject = null;
            if (storageSerializationType == StorageSerializationType.XML)
            {
                DataContractSerializer serializer = new DataContractSerializer(typeToRetrieve);
                informationObject = (IInformationObject) serializer.ReadObject(memoryStream);
            }
            else if (storageSerializationType == StorageSerializationType.JSON)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeToRetrieve);
                informationObject = (IInformationObject) serializer.ReadObject(memoryStream);
            }
            else if (storageSerializationType == StorageSerializationType.Binary)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                informationObject = (IInformationObject) binaryFormatter.Deserialize(memoryStream);
            }
            else // if(storageSerializationType == StorageSerializationType.Custom)
            {
                throw new NotSupportedException("Custom or unspecified formatting not supported");
            }
            return informationObject;
        }

        private static byte[] SerializeInformationObjectToBuffer(IInformationObject informationObject)
        {
            Type informationObjectType = informationObject.GetType();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] dataContent = null;
                StorageSerializationType storageSerializationType = getStorageSerializationType(informationObjectType);
                if (storageSerializationType == StorageSerializationType.XML)
                {
                    DataContractSerializer ser = new DataContractSerializer(informationObjectType);
                    using (
                        XmlTextWriter writer = new XmlTextWriter(memoryStream, Encoding.UTF8)
                            {
                                Formatting = Formatting.Indented
                            })
                    {
                        ser.WriteObject(writer, informationObject);
                        writer.Flush();
                        dataContent = memoryStream.ToArray();
                    }
                }
                else if (storageSerializationType == StorageSerializationType.JSON)
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(informationObjectType);
                    serializer.WriteObject(memoryStream, informationObject);
                    dataContent = memoryStream.ToArray();
                } else if (storageSerializationType == StorageSerializationType.Binary)
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, informationObject);
                    dataContent = memoryStream.ToArray();
                } else // if (storageSerializationType == StorageSerializationType.Custom)
                {
                    throw new NotSupportedException("Custom or unspecified formatting not supported");
                }
                return dataContent;
            }
        }



        private static StorageSerializationType getStorageSerializationType(Type type)
        {
            PropertyInfo propInfo = type.GetProperty("ClassStorageSerializationType",
                                                     BindingFlags.Public | BindingFlags.Static);
            if(propInfo == null)
                return StorageSerializationType.XML;
            var propValue = propInfo.GetValue(null);
            if(propValue == null)
                return StorageSerializationType.XML;
            return (StorageSerializationType) propValue;
        }

        public static IInformationObject[] RetrieveInformationObjects(string itemDirectory, Type type)
        {
            var result =
                CurrActiveContainer.GetInformationObjects(itemDirectory, null,  iObj => iObj.GetType() == type).ToArray();
            return result;
            /*
            var blobList = CurrActiveContainer.GetBlobListing(itemDirectory, withMetaData: true).ToArray();
            List<IInformationObject> result = new List<IInformationObject>(blobList.Length);
            foreach(CloudBlockBlob blob in blobList)
            {
                var item = RetrieveInformation(blob.Name, type);
                result.Add(item);
            }
            return result.ToArray();
             * */
        }

        public static string GetBlobOwnerAddress(IContainerOwner owner, string blobAddress)
        {
            string ownerPrefix = owner.ContainerName + "/" + owner.LocationPrefix + "/";
            if (blobAddress.StartsWith("grp/") || blobAddress.StartsWith("acc/") || blobAddress.StartsWith("dev/"))
            {
                if(blobAddress.StartsWith(ownerPrefix))
                    return blobAddress;
                // TODO: NOTE! this allows cross-referencing blobs between owner boundaries in subscriptions
                return blobAddress;
                throw new SecurityException("Invalid reference to blob: " + blobAddress + " by owner prefix: " + owner.LocationPrefix);
            }
            return ownerPrefix + blobAddress;
        }

        public static string DownloadOwnerBlobText(IContainerOwner owner, string blobAddress, bool returnNullIfMissing = false)
        {
            string downloadAddress = GetBlobOwnerAddress(owner, blobAddress);
            return CurrActiveContainer.DownloadBlobText(downloadAddress, returnNullIfMissing);
        }

        public static CloudBlockBlob UploadOwnerBlobText(IContainerOwner owner, string blobAddress, string content, string blobInformationType)
        {
            string uploadAddress = GetBlobOwnerAddress(owner, blobAddress);
            return CurrActiveContainer.UploadBlobText(uploadAddress, content, blobInformationType);
        }

        public static void UploadOwnerBlobBinary(IContainerOwner owner, string blobAddress, byte[] binaryContent, string contentInformationType = null)
        {
            string uploadAddress = GetBlobOwnerAddress(owner, blobAddress);
            CurrActiveContainer.UploadBlobBinary(uploadAddress, binaryContent, contentInformationType);
        }


        public static CloudBlob GetOwnerBlobReference(IContainerOwner containerOwner, string contentPath)
        {
            string blobAddress = GetBlobOwnerAddress(containerOwner, contentPath);
            CloudBlob blob = CurrActiveContainer.GetBlockBlobReference(blobAddress);
            return blob;
        }

        public static void DeleteInformationObject(this IInformationObject informationObject, IContainerOwner owner = null)
        {
            string relativeLocation = informationObject.RelativeLocation;
            if (owner != null)
                relativeLocation = GetBlobOwnerAddress(owner, relativeLocation);
            CloudBlob blob = CurrActiveContainer.GetBlobReference(relativeLocation);
            blob.DeleteAndFireSubscriptions();
            IAdditionalFormatProvider additionalFormatProvider = informationObject as IAdditionalFormatProvider;
            if(additionalFormatProvider != null)
            {
                foreach(var contentExtension in additionalFormatProvider.GetAdditionalFormatExtensions())
                {
                    string contentLocation = blob.Name + "." + contentExtension;
                    CloudBlob contentBlob = CurrActiveContainer.GetBlobReference(contentLocation);
                    contentBlob.DeleteIfExists();
                }

            }
            //TODO: Generic default view deletion
            //DefaultViewSupport.DeleteDefaultView(informationObject);
            informationObject.PostDeleteExecute(owner);
            InformationContext.Current.ObjectStoredNotification(informationObject, InformationContext.ObjectChangeType.D_Deleted);

        }

        public static CloudBlockBlob GetBlob(string containerName, string blobAddress)
        {
            var container = CurrBlobClient.GetContainerReference(containerName);
            return (CloudBlockBlob) container.GetBlob(blobAddress);
        }
        
        public static CloudBlob GetBlob(this CloudBlobContainer container, string blobAddress, IContainerOwner owner = null)
        {
            string relativeLocation = blobAddress;
            if (owner != null)
                relativeLocation = GetBlobOwnerAddress(owner, relativeLocation);
            CloudBlob blob = container.GetBlockBlobReference(relativeLocation);
            return blob;
        }

        public static string GetAccountIDFromLocation(string referenceLocation)
        {
            if (referenceLocation.StartsWith("acc/") == false)
                throw new InvalidDataException("Referencelocation is not account owned: " + referenceLocation);
            return referenceLocation.Substring(4, GuidLength);
        }

        public static string GetGroupIDFromLocation(string referenceLocation)
        {
            if (referenceLocation.StartsWith("grp/") == false)
                throw new InvalidDataException("Referencelocation is not group owned: " + referenceLocation);
            return referenceLocation.Substring(4, GuidLength);
        }


        public static string GetContentRootLocation(string referenceLocation)
        {
            if(referenceLocation.StartsWith("acc/") == false && referenceLocation.StartsWith("grp/") == false && referenceLocation.StartsWith("dev/") == false)
                throw new InvalidDataException("Unable to determine root for reference: " + referenceLocation);
            //var contentRoot = referenceLocation.Substring(0, AccOrGrpPlusIDPathLength) + ContentFolderName + "/";
            var contentRoot = referenceLocation.Substring(0, AccOrGrpPlusIDPathLength);
            return contentRoot;
        }

        public static int DeleteContentsFromOwner(IContainerOwner owner)
        {
            string referenceLocation = GetBlobOwnerAddress(owner, ".");
            return DeleteContentsFromOwner(referenceLocation);
        }

        public static int DeleteContentsFromOwner(string referenceLocation)
        {
            string contentRootLocation = GetContentRootLocation(referenceLocation);
            BlobRequestOptions options = new BlobRequestOptions
                                             {UseFlatBlobListing = true};
            string searchRoot = CurrActiveContainer.Name + "/" + contentRootLocation;
            var blobList = CurrBlobClient.ListBlobsWithPrefix(searchRoot, options);
            int deleteCount = 0;
            foreach(CloudBlob blob in blobList)
            {
                blob.DeleteWithoutFiringSubscriptions();
                deleteCount++;
            }
            return deleteCount;
        }

        public static ResultSegment<IListBlobItem> ListBlobxWithPrefixSegmented(this CloudBlobContainer container,
                                                                              string prefix, int maxresults, ResultContinuation continuationToken, BlobRequestOptions options = null)
        {
            string searchRoot = container.Name + "/" + prefix;
            if (options != null)
                return CurrBlobClient.ListBlobsWithPrefixSegmented(searchRoot, maxresults, continuationToken, options);
            return CurrBlobClient.ListBlobsWithPrefixSegmented(searchRoot, maxresults, continuationToken);
        }

        public static IEnumerable<IListBlobItem> ListBlobsWithPrefix(this IContainerOwner owner, string prefix, BlobRequestOptions options = null)
        {
            if(options == null)
                options = new BlobRequestOptions { UseFlatBlobListing = true};
            string searchRoot = CurrActiveContainer.Name + "/" + owner.ContainerName + "/" + owner.LocationPrefix + "/" + prefix;
            return CurrBlobClient.ListBlobsWithPrefix(searchRoot, options);
        }

        public static IEnumerable<IListBlobItem> ListBlobsWithPrefix(this CloudBlobContainer container, string prefix, BlobRequestOptions options = null)
        {
            string searchRoot = container.Name + "/" + prefix;
            if (options != null)
                return CurrBlobClient.ListBlobsWithPrefix(searchRoot, options);
            return CurrBlobClient.ListBlobsWithPrefix(searchRoot);
        }

        public static string GetOwnerRootAddress(IContainerOwner owner)
        {
            string ownerRootAddress = GetBlobOwnerAddress(owner, "");
            return ownerRootAddress;
        }

        public static int DeleteEntireOwner(IContainerOwner owner)
        {
            if(owner == null)
                throw new ArgumentNullException("owner");
            string ownerRootAddress = GetOwnerRootAddress(owner);
            BlobRequestOptions options = new BlobRequestOptions {UseFlatBlobListing = true, BlobListingDetails = BlobListingDetails.Metadata};
            string storageLevelOwnerRootAddress = CurrActiveContainer.Name + "/" + ownerRootAddress;
            var blobs = CurrBlobClient.ListBlobsWithPrefix(storageLevelOwnerRootAddress, options);
            int deletedCount = 0;
            foreach(CloudBlob blob in blobs)
            {
                blob.DeleteWithoutFiringSubscriptions();
                deletedCount++;
            }
            return deletedCount;
        }


        public static int DeleteBlobsFromOwnerTarget(IContainerOwner owner, string targetLocation)
        {
            string rootAddress = CurrActiveContainer.Name + "/" + GetBlobOwnerAddress(owner, targetLocation);
            BlobRequestOptions options = new BlobRequestOptions { UseFlatBlobListing = true, BlobListingDetails = BlobListingDetails.Metadata };
            var blobs = CurrBlobClient.ListBlobsWithPrefix(rootAddress, options);
            int deletedCount = 0;
            foreach (CloudBlob blob in blobs)
            {
                blob.DeleteWithoutFiringSubscriptions();
                deletedCount++;
            }
            return deletedCount;
        }

        public static bool CanContainExternalMetadata(this CloudBlob blob)
        {
            string blobInformationType = blob.GetBlobInformationType();
            if (blobInformationType == null)
                return false;
            if (blobInformationType == InformationType_GenericContentValue)
                return false;
            return true;
        }

        public static bool IsStoredInActiveContainer(this CloudBlob blob)
        {
            return blob.Container.Name == CurrActiveContainer.Name;
        }

        public static void DeleteWithoutFiringSubscriptions(this CloudBlob blob)
        {
            if (blob.IsStoredInActiveContainer() && blob.CanContainExternalMetadata())
            {
                SubscribeSupport.DeleteSubscriptions(blob.Name);
                InformationSourceSupport.DeleteInformationSources(blob.Name);
            }
            blob.DeleteIfExists();
            InformationContext.AddStorageTransactionToCurrent();
        }

        public static void DeleteAndFireSubscriptions(this CloudBlob blob)
        {
            if (blob.IsStoredInActiveContainer() && blob.CanContainExternalMetadata())
            {
                SubscribeSupport.DeleteAfterFiringSubscriptions(targetLocation: blob.Name);
                InformationSourceSupport.DeleteInformationSources(targetLocation: blob.Name);
            }
            blob.DeleteIfExists();
            InformationContext.AddStorageTransactionToCurrent();
        }

        public static void DeleteBlob(string blobPath)
        {
            Console.WriteLine("Deleting: " + blobPath);
            CloudBlob blob = CurrActiveContainer.GetBlobReference(blobPath);
            blob.DeleteIfExists();
            InformationContext.AddStorageTransactionToCurrent();
        }

        public static string GetLocationParentDirectory(string location)
        {
            int lastIndexOfSeparator = location.LastIndexOf('/');
            int lastPositionToInclude = lastIndexOfSeparator + 1; // keep the separator
            return location.Substring(0, lastPositionToInclude);
        }

        public static CloudBlob GetInformationObjectBlobWithProperties(IInformationObject informationObject)
        {
            CloudBlob blob = CurrActiveContainer.GetBlob(informationObject.RelativeLocation);
            blob.FetchAttributes();
            InformationContext.AddStorageTransactionToCurrent();
            return blob;
        }

        public static IEnumerable<IListBlobItem> GetBlobListing(this CloudBlobContainer container, string directoryLocation, bool withMetaData = false)
        {
            string storageListingPrefix = container.Name + "/" + directoryLocation;
            BlobRequestOptions options = new BlobRequestOptions() { UseFlatBlobListing = true, BlobListingDetails = withMetaData ? BlobListingDetails.Metadata : BlobListingDetails.None };
            InformationContext.AddStorageTransactionToCurrent();
            return CurrBlobClient.ListBlobsWithPrefix(storageListingPrefix, options);
        }

        public static IEnumerable<CloudBlockBlob> GetFilteredBlobListing(this CloudBlobContainer container, string directoryLocation, Predicate<CloudBlockBlob> filterIfFalse)
        {
            var blobListing = container.GetBlobListing(directoryLocation, true);
            return blobListing.Cast<CloudBlockBlob>().Where(blob => filterIfFalse(blob));
        }
        
        public static IEnumerable<IInformationObject> GetInformationObjects(this CloudBlobContainer container, string directoryLocation, Predicate<string> filterByFullName = null, Predicate<IInformationObject> filterIfFalse = null)
        {
            var informationObjectBlobs =
                GetFilteredBlobListing(container, directoryLocation,
                                       blob =>
                                           {
                                               bool passFilterByName = false;
                                               if (filterByFullName != null)
                                                   passFilterByName =
                                                       filterByFullName(blob.Name);
                                               else
                                                   passFilterByName = true;
                                               if (passFilterByName == false)
                                                   return false;
                                               return blob.GetBlobInformationType() ==
                                                      StorageSupport.InformationType_InformationObjectValue;
                                           });
            if (filterByFullName != null)
                informationObjectBlobs = informationObjectBlobs.Where(blob => filterByFullName(blob.Name));
            var result = informationObjectBlobs.Select(blob =>
                                                           {
                                                               string informationObjectLocation = blob.Name;
                                                               string informationObjectType =
                                                                   blob.GetBlobInformationObjectType();
                                                               IInformationObject iObj = null;
                                                               try
                                                               {
                                                                   Debug.WriteLine("Loading type: " +
                                                                                   informationObjectType + " from " +
                                                                                   informationObjectLocation);
                                                                   iObj = RetrieveInformation(
                                                                       informationObjectLocation, informationObjectType);
                                                               }
                                                               catch (InvalidDataException dataException)
                                                               {
                                                                   Debug.WriteLine("Failed due to invalid data");
                                                                   // Old types and such cause this, ignore and allow returning null.
                                                               }
                                                               return iObj;
                                                           }).Where(iObj => iObj != null);
            if (filterIfFalse != null)
                result = result.Where(iObj => filterIfFalse(iObj));
            return result;
        }

        public static IEnumerable<IListBlobItem> GetContentBlobListing(IContainerOwner owner, string contentType)
        {
            string contentListingPrefix = GetBlobOwnerAddress(owner, contentType);
            string storageListingPrefix = CurrActiveContainer.Name + "/" + contentListingPrefix;
            BlobRequestOptions options = new BlobRequestOptions() {UseFlatBlobListing = true, BlobListingDetails = BlobListingDetails.Metadata};
            InformationContext.AddStorageTransactionToCurrent();
            return CurrBlobClient.ListBlobsWithPrefix(storageListingPrefix, options);
        }

        public static bool AcquireLogicalLockByCreatingBlob(string lockLocation, out string lockEtag)
        {
            CloudBlob blob = CurrActiveContainer.GetBlobReference(lockLocation);
            DateTime created = DateTime.UtcNow;
            blob.Metadata.Add("LockCreated", created.ToString("s"));
            string blobContent = "LockCreated: " + created.ToString("s");
            BlobRequestOptions options = new BlobRequestOptions();
            options.RetryPolicy = RetryPolicies.Retry(10, TimeSpan.FromSeconds(3));
            options.AccessCondition = AccessCondition.IfNoneMatch("*");
            try
            {
                Debug.WriteLine("Trying to aqruire lock: " + lockLocation);
                blob.UploadText(blobContent, Encoding.UTF8, options);
                InformationContext.AddStorageTransactionToCurrent();
                Debug.WriteLine("Success!");
            }
            catch
            {
                Debug.WriteLine("FAILED!");
                lockEtag = null;
                return false;
            }
            lockEtag = blob.Properties.ETag;
            return true;
        }

        public static bool ReleaseLogicalLockByDeletingBlob(string lockLocation, string lockEtag)
        {
            CloudBlob blob = CurrActiveContainer.GetBlobReference(lockLocation);
            BlobRequestOptions options = new BlobRequestOptions();
            options.RetryPolicy = RetryPolicies.Retry(10, TimeSpan.FromSeconds(3));
            if (lockEtag != null)
                options.AccessCondition = AccessCondition.IfMatch(lockEtag);
            try
            {
                Debug.WriteLine("Trying to release lock: " + lockLocation);
                blob.Delete(options);
                InformationContext.AddStorageTransactionToCurrent();
                Debug.WriteLine("Success!");
            }
            catch
            {
                Debug.WriteLine("FAILED!");
                return false;
            }
            return true;
        }

        public static IInformationObject RetrieveInformationObjectFromDefaultLocation(string contentDomain, string contentTypeName, string contentObjectId, IContainerOwner owner)
        {
            string contentFullTypeName = contentDomain + "." + contentTypeName;
            string contentLocation = contentDomain + "/" + contentTypeName + "/" + contentObjectId;
            return RetrieveInformation(contentLocation, contentFullTypeName, null, owner);
        }

        public static string[] GetLogicalDirectories(IContainerOwner owner, string rootLocation)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");
            var blobListing =
                ListBlobsWithPrefix(owner, rootLocation,
                                                        new BlobRequestOptions {UseFlatBlobListing = false}).ToArray();
            string[] directories = blobListing.Where(item => item is CloudBlobDirectory)
                                              .Select(
                                                  item => item.Uri.LocalPath.Substring(item.Parent.Uri.LocalPath.Length))
                                              .ToArray();
            return directories;
        }
    }

    public class ReferenceOutdatedException : Exception
    {
        private IInformationObject containerObject;
        private IInformationObject referenceInstance;
        private IInformationObject masterInstance;

        public ReferenceOutdatedException(IInformationObject containerObject, IInformationObject referenceInstance, IInformationObject masterInstance)
        {
            // TODO: Complete member initialization
            this.containerObject = containerObject;
            this.referenceInstance = referenceInstance;
            this.masterInstance = masterInstance;
        }
    }
}
