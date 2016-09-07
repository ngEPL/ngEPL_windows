using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NewEPL {
     public class NinePatch {

        private ImageSource Source;

        public ImageSource Original {
            get {
                var src = (BitmapSource)Source;
                var cropped = new CroppedBitmap(src, new Int32Rect(1, 1, (int)src.Width - 2, (int)src.Height - 2));

                MemoryStream stream = new MemoryStream();
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(cropped));
                encoder.Save(stream);

                BitmapImage ret = new BitmapImage();
                ret.BeginInit();
                ret.StreamSource = stream;
                ret.EndInit();

                return ret;
            }
        }

        private Dictionary<string, ImageSource> Cache = new Dictionary<string, ImageSource>();

        private List<int> TopPatches = new List<int>(), LeftPatches = new List<int>(), BottomPatches = new List<int>(), RightPatches = new List<int>();
        private List<int> WidthRegions = new List<int>(), HeightRegions = new List<int>();

        private const int BYTES_PER_PIXEL = 4;

        public int Width, Height;

        public NinePatch(ImageSource src) {
            Source = src;
        }

        public NinePatch(string path) {
            Source = new BitmapImage(new Uri(path));
        }

        public void ClearCache() {
            Cache.Clear();
        }

        public ImageSource GetPatchedImage(int width, int height) {
            Width = width;
            Height = height;
            if (Cache.ContainsKey(String.Format("{0}x{1}", width, height))) {
                return Cache[String.Format("{0}x{1}", width, height)];
            }

            TopPatches.Clear();
            LeftPatches.Clear();
            BottomPatches.Clear();
            RightPatches.Clear();
            WidthRegions.Clear();
            HeightRegions.Clear();

            FindPatchRegion();

            var src = (BitmapSource)Original;

            int srcWidth = src.PixelWidth;
            int srcHeight = src.PixelHeight;
            int targetWidth = width;
            int targetHeight = height;

            targetWidth = Math.Max(srcWidth, targetWidth);
            targetHeight = Math.Max(srcHeight, targetHeight);
            if(srcWidth == targetWidth && srcHeight == targetHeight) {
                return src;
            }

            byte[] srcPixels = new byte[targetHeight * targetWidth * BYTES_PER_PIXEL];
            byte[] pixelsId = new byte[targetHeight * targetWidth * BYTES_PER_PIXEL];

            var xMapping = XMapping(targetWidth - srcWidth, targetWidth);
            var yMapping = YMapping(targetHeight - srcHeight, targetHeight);

            int index = 0;
            src.CopyPixels(srcPixels, targetWidth * BYTES_PER_PIXEL, 0);

            for (int row = 0; row < targetHeight; row++) {
                int sourceY = yMapping[row];
                for (int col = 0; col < targetWidth; col++) {
                    int sourceX = xMapping[col];
                    for (int i = 0; i < 4; i++)
                        pixelsId[index++] = srcPixels[sourceY * targetWidth * 4 + sourceX * 4 + i];
                }
            }   

            int stride = BYTES_PER_PIXEL * targetWidth;

            var ret = new WriteableBitmap(targetWidth, targetHeight, 96, 96, PixelFormats.Bgra32, null);//WriteableBitmap.Create(targetWidth, targetHeight, 96, 96, PixelFormats.Bgra32, null, pixelsId, stride);
            ret.WritePixels(new Int32Rect(0, 0, targetWidth, targetHeight), pixelsId, stride, 0);

            //Cache.Add(String.Format("{0}x{1}", targetWidth, targetHeight), ret);

            return ret;
        }

        public int GetImmutableWidth(int idx) {
            return WidthRegions[idx];
        }

        public int GetImmutableHeight(int idx) {
            return HeightRegions[idx];
        }

        public int GetStrectedWidth(int width) {
            int ret = 0;
            foreach(var i in WidthRegions) {
                ret += i;
            }

            return width - ret;
        }

        public int GetStrectedHeight(int height) {
            int ret = 0;
            foreach (var i in HeightRegions) {
                ret += i;
            }

            return height - ret;
        }

        private void FindPatchRegion() {
            BitmapSource src = Source as BitmapSource;
            byte[] srcPixels = new byte[src.PixelWidth *src.PixelHeight * BYTES_PER_PIXEL];
            src.CopyPixels(srcPixels, (src.PixelWidth * src.Format.BitsPerPixel + 7) / 8, 0);

            // Top
            int count = 0;
            WidthRegions.Add(count);
            for (int x = 1; x < src.PixelWidth - 1; x++) {
                if(srcPixels[0 * src.PixelWidth * BYTES_PER_PIXEL + x * BYTES_PER_PIXEL + 0] == 0 &&
                    srcPixels[0 * src.PixelWidth * BYTES_PER_PIXEL + x * BYTES_PER_PIXEL + 1] == 0 &&
                    srcPixels[0 * src.PixelWidth * BYTES_PER_PIXEL + x * BYTES_PER_PIXEL + 2] == 0 &&
                    srcPixels[0 * src.PixelWidth * BYTES_PER_PIXEL + x * BYTES_PER_PIXEL + 3] == 255) { 
                    TopPatches.Add(x - 1);
                    WidthRegions.Add(count);
                    count = 0;
                } else {
                    count += 1;
                }
            }
            WidthRegions.Add(count);

            count = 0;
            HeightRegions.Add(count);
            // Left
            for (int y = 1; y < src.PixelHeight - 1; y++) {
                if (srcPixels[y * src.PixelWidth * BYTES_PER_PIXEL + 0 * BYTES_PER_PIXEL + 0] == 0 &&
                    srcPixels[y * src.PixelWidth * BYTES_PER_PIXEL + 0 * BYTES_PER_PIXEL + 1] == 0 &&
                    srcPixels[y * src.PixelWidth * BYTES_PER_PIXEL + 0 * BYTES_PER_PIXEL + 2] == 0 &&
                    srcPixels[y * src.PixelWidth * BYTES_PER_PIXEL + 0 * BYTES_PER_PIXEL + 3] == 255) {
                    LeftPatches.Add(y - 1);
                    HeightRegions.Add(count);
                    count = 0;
                } else {
                    count += 1;
                }
            }
            HeightRegions.Add(count);
            // Bottom
            for (int x = 1; x < src.PixelWidth - 1; x++) {
                if (srcPixels[(src.PixelHeight - 1) * src.PixelWidth * BYTES_PER_PIXEL + x * BYTES_PER_PIXEL + 0] == 0 &&
                    srcPixels[(src.PixelHeight - 1) * src.PixelWidth * BYTES_PER_PIXEL + x * BYTES_PER_PIXEL + 1] == 0 &&
                    srcPixels[(src.PixelHeight - 1) * src.PixelWidth * BYTES_PER_PIXEL + x * BYTES_PER_PIXEL + 2] == 0 &&
                    srcPixels[(src.PixelHeight - 1) * src.PixelWidth * BYTES_PER_PIXEL + x * BYTES_PER_PIXEL + 3] == 255) {
                    BottomPatches.Add(x - 1);
                }
            }
            // Right
            for (int y = 1; y < src.PixelHeight - 1; y++) {
                if (srcPixels[y * src.PixelWidth * BYTES_PER_PIXEL + (src.PixelWidth - 1) * BYTES_PER_PIXEL + 0] == 0 &&
                    srcPixels[y * src.PixelWidth * BYTES_PER_PIXEL + (src.PixelWidth - 1) * BYTES_PER_PIXEL + 1] == 0 &&
                    srcPixels[y * src.PixelWidth * BYTES_PER_PIXEL + (src.PixelWidth - 1) * BYTES_PER_PIXEL + 2] == 0 &&
                    srcPixels[y * src.PixelWidth * BYTES_PER_PIXEL + (src.PixelWidth - 1) * BYTES_PER_PIXEL + 3] == 255) {
                    RightPatches.Add(y - 1);
                }
            }
        }

        private List<int> XMapping(int diff, int target) {
            var ret = new List<int>(target);

            int src = 0;
            int dst = 0;
            while (dst < target) {
                int foundIndex = TopPatches.IndexOf(src);
                if (foundIndex != -1) {
                    int repeatCount = (diff / TopPatches.Count) + 1;
                    if (foundIndex < diff % TopPatches.Count) {
                        repeatCount++;
                    }
                    for (int j = 0; j < repeatCount; j++) {
                        ret.Insert(dst++, src);
                    }
                } else {
                    ret.Insert(dst++, src);
                }
                src++;
            }

            return ret;
        }

        private List<int> YMapping(int diff, int target) {
            var ret = new List<int>(target);

            int src = 0;
            int dst = 0;
            while (dst < target) {
                int foundIndex = LeftPatches.IndexOf(src);
                if (foundIndex != -1) {
                    int repeatCount = (diff / LeftPatches.Count) + 1;
                    if (foundIndex < diff % LeftPatches.Count) {
                        repeatCount++;
                    }
                    for (int j = 0; j < repeatCount; j++) {
                        ret.Insert(dst++, src);
                    }
                } else {
                    ret.Insert(dst++, src);
                }
                src++;
            }

            return ret;
        }
    }
}
