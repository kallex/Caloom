using System.Web;

namespace TheBall
{
    public class MediaFileData
    {
        public string FileName;
        public byte[] FileContent;
        public HttpPostedFile HttpFile;
    }
}