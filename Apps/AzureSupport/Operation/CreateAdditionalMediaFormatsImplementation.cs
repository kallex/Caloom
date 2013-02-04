using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.WindowsAzure.StorageClient;
using TheBall;

namespace AaltoGlobalImpact.OIP
{
    public static class CreateAdditionalMediaFormatsImplementation
    {
        public static Bitmap GetTarget_BitmapData(string masterRelativeLocation)
        {
            try
            {
                CloudBlob blob = StorageSupport.CurrActiveContainer.GetBlobReference(masterRelativeLocation);
                Bitmap bitmap;
                using(Stream bitmapStream = new MemoryStream((int) blob.Properties.Length))
                {
                    blob.DownloadToStream(bitmapStream);
                    bitmapStream.Seek(0, SeekOrigin.Begin);
                    bitmap = new Bitmap(bitmapStream);
                }
                return bitmap;
            } catch
            {
                
            }
            return null;
        }

        public static void ExecuteMethod_CreateImageMediaFormats(string masterRelativeLocation, Bitmap bitmapData)
        {
            if (bitmapData == null)
                return;
            Size[] desiredSizes = new Size[]
                                      {
                                          // Wide screen format
                                          new Size(1280, 720),
                                          new Size(640, 360),
                                          // .. portrait alternatives, but not the biggest ones
                                          new Size(360, 640),
                                          // Standard screen format
                                          new Size(1024, 768),
                                          new Size(800, 600),
                                          new Size(640, 480),
                                          new Size(320, 240),
                                          new Size(160, 120),
                                          // .. portrait alternatives, but not the biggest ones
                                          new Size(480, 640),
                                          new Size(240, 320),
                                          new Size(120, 160),
                                          // Square icon format
                                          new Size(256, 256),
                                          new Size(128, 128),
                                          new Size(64, 64),
                                          new Size(32, 32),
                                      };
            foreach(Size size in desiredSizes)
            {
                string sizedFittingAllInLocation = GetSizedLocation(masterRelativeLocation, size, fittingAllIn: true);
                Bitmap fittingBitmap = ResizeImage(bitmapData, size, true, false, false);
                StoreToBlob(sizedFittingAllInLocation, fittingBitmap);
                string sizedCroppingLocation = GetSizedLocation(masterRelativeLocation, size, fittingAllIn: false);
                Bitmap croppedBitmap = ResizeImage(bitmapData, size, true, false, true);
                StoreToBlob(sizedCroppingLocation, croppedBitmap);
            }
        }

        private static void StoreToBlob(string blobLocation, Bitmap bitmap)
        {
            var blob = StorageSupport.CurrActiveContainer.GetBlobReference(blobLocation);
            using(MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Jpeg);
                stream.Seek(0, SeekOrigin.Begin);
                blob.UploadFromStream(stream);
                Debug.WriteLine("Uploaded media blob: " + blobLocation);
            }
        }

        private static string GetSizedLocation(string masterRelativeLocation, Size size, bool fittingAllIn)
        {
            string masterLocationWithoutExtension = RenderWebSupport.GetLocationWithoutExtension(masterRelativeLocation);
            string formatWithExtension = fittingAllIn
                                             ? String.Format("_{0}x{1}_whole.jpg", size.Width, size.Height)
                                             : String.Format("_{0}x{1}_crop.jpg", size.Width, size.Height);
            return masterLocationWithoutExtension + formatWithExtension;
        }

        private static Bitmap ResizeImage(Bitmap mg, Size newSize, bool maintainRatio = false, bool honorNewSizeWithSorrowBorders = false,
            bool cropInsteadOfFitWithEmpty = false)
        {
            double ratio = 0d;
            double myThumbWidth = 0d;
            double myThumbHeight = 0d;
            int x = 0;
            int y = 0;

            Bitmap bp;

            double widthRatio = (double) mg.Width/newSize.Width;
            double heightRatio = (double) mg.Height/newSize.Height;

            ratio = cropInsteadOfFitWithEmpty ? Math.Min(widthRatio, heightRatio) : Math.Max(widthRatio, heightRatio);

            myThumbHeight = Math.Ceiling(mg.Height / ratio);
            myThumbWidth = Math.Ceiling(mg.Width / ratio);

            //Size thumbSize = new Size((int)myThumbWidth, (int)myThumbHeight);
            Size thumbSize;
            if (maintainRatio)
                thumbSize = new Size((int)myThumbWidth, (int)myThumbHeight);
            else
                thumbSize = newSize;
            if (honorNewSizeWithSorrowBorders)
            {
                bp = new Bitmap(newSize.Width, newSize.Height);
                x = (newSize.Width - thumbSize.Width) / 2;
                y = (newSize.Height - thumbSize.Height);

            }
            else
            {
                bp = new Bitmap(thumbSize.Width, thumbSize.Height);
                x = 0;
                y = 0;
            }

            // Had to add System.Drawing class in front of Graphics ---
            using (System.Drawing.Graphics g = Graphics.FromImage(bp))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                Rectangle rect = new Rectangle(x, y, thumbSize.Width, thumbSize.Height);
                g.DrawImage(mg, rect, 0, 0, mg.Width, mg.Height, GraphicsUnit.Pixel);

            }
            return bp;
        }

        public static object GetTarget_VideoData(string masterRelativeLocation)
        {
            return null;
        }

        public static void ExecuteMethod_CreateVideoMediaFormats(string masterRelativeLocation, object videoData)
        {
            if (videoData == null)
                return;
        }
    }
}