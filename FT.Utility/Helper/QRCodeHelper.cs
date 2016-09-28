namespace FT.Utility.Helper
{
    using ZXing;
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using ZXing.QrCode.Internal;
    using System.Collections.Generic;
    using ZXing.Common;

    public class QRCodeHelper
    {
        public static Bitmap Create(string content, int width, int height)
        {
            BarcodeWriter writer = new BarcodeWriter();
            Bitmap bit;
            EncodingOptions options = new EncodingOptions();
            //Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();
            //hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            //hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            //hints.Add(EncodeHintType.MARGIN, 0);

            options.Margin = 0;
            options.Width = width;
            options.Height = height;
            writer.Options = options;
            writer.Format = BarcodeFormat.QR_CODE;
            bit = writer.Write(content);
            return bit;
        }

        public static Bitmap Create(string content, Image centralImage)
        {
            MultiFormatWriter writer = new MultiFormatWriter();
            Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();
            hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            hints.Add(EncodeHintType.MARGIN, 0);
            Bitmap bitmap = writer.encode(content, BarcodeFormat.QR_CODE, 300, 300, hints).ToBitmap();
            Image image = centralImage;
            var size = writer.encode(content, BarcodeFormat.QR_CODE, 300, 300);
            int width = Math.Min((int)(((double)size.Width) / 3.5), image.Width);
            int height = Math.Min((int)(((double)size.Height) / 3.5), image.Height);
            int x = (bitmap.Width - width) / 2;
            int y = (bitmap.Height - height) / 2;
            Bitmap bitmap2 = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(bitmap2))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.DrawImage(bitmap, 0, 0);
            }
            Graphics graphics2 = Graphics.FromImage(bitmap2);
            graphics2.FillRectangle(Brushes.White, x, y, width, height);
            graphics2.DrawImage(image, x, y, width, height);
            return bitmap2;
        }

        public static Bitmap Create(string content, string imagePath)
        {
            Image centralImage = Image.FromFile(imagePath);
            return Create(content, centralImage);
        }
    }
}

