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
        public int DefaultWidth = 0, DefaultHeight = 0;
        public List<int> InitWidth = new List<int>(), InitHeight = new List<int>();

        private List<int> SavedWidths = new List<int>(), SavedHeights = new List<int>();

        public NinePatch(ImageSource src) {
            Source = src;

            TopPatches.Clear();
            LeftPatches.Clear();
            BottomPatches.Clear();
            RightPatches.Clear();
            WidthRegions.Clear(); 
            HeightRegions.Clear();

            SavedWidths.Insert(0, 0);
            SavedWidths.Insert(1, 0);
            SavedWidths.Insert(2, 0);

            SavedHeights.Insert(0, 0);
            SavedHeights.Insert(1, 0);
            SavedHeights.Insert(2, 0);

            FindPatchRegion();
        }

        public NinePatch(string path) {
            Source = new BitmapImage(new Uri(path)); 

            TopPatches.Clear();
            LeftPatches.Clear();
            BottomPatches.Clear();
            RightPatches.Clear();
            WidthRegions.Clear();
            HeightRegions.Clear();

            SavedWidths.Insert(0, 0);
            SavedWidths.Insert(1, 0);
            SavedWidths.Insert(2, 0);

            SavedHeights.Insert(0, 0);
            SavedHeights.Insert(1, 0);
            SavedHeights.Insert(2, 0);

            FindPatchRegion();
        }

        public void ClearCache() {
            Cache.Clear();
        }

        public ImageSource GetPatchedImage(List<int> widths, List<int> heights, Color color) {
            var src = (BitmapSource)Original;

            for (int i = 0; i < widths.Count; i++) SavedWidths[i] += widths[i];
            for (int i = 0; i < heights.Count; i++) SavedHeights[i] += heights[i];

            Width = (int)Original.Width;
            Height = (int)Original.Height;

            int sumWidth = 0, sumHeight = 0;

            foreach (var i in SavedWidths) sumWidth += i;
            foreach (var i in SavedHeights) sumHeight += i;

            Width += sumWidth;
            Height += sumHeight;

            int srcWidth = src.PixelWidth;
            int srcHeight = src.PixelHeight;
            int targetWidth = Width;
            int targetHeight = Height;

            targetWidth = Math.Max(srcWidth, targetWidth);
            targetHeight = Math.Max(srcHeight, targetHeight);
            if (srcWidth == targetWidth && srcHeight == targetHeight) {
                return src;
            }

            byte[] srcPixels = new byte[targetHeight * targetWidth * BYTES_PER_PIXEL];
            byte[] pixelsId = new byte[targetHeight * targetWidth * BYTES_PER_PIXEL];

            var xMapping = XMapping(targetWidth, SavedWidths);
            var yMapping = YMapping(targetHeight, SavedHeights);

            int index = 0;
            src.CopyPixels(srcPixels, targetWidth * BYTES_PER_PIXEL, 0);

            for (int row = 0; row < targetHeight; row++) {
                int sourceY = yMapping[row];
                for (int col = 0; col < targetWidth; col++) {
                    int sourceX = xMapping[col];
                    pixelsId[index++] = (byte)(srcPixels[sourceY * targetWidth * 4 + sourceX * 4 + 0] * (color.B / 255.0)); /// b
                    pixelsId[index++] = (byte)(srcPixels[sourceY * targetWidth * 4 + sourceX * 4 + 1] * (color.G / 255.0)); /// g
                    pixelsId[index++] = (byte)(srcPixels[sourceY * targetWidth * 4 + sourceX * 4 + 2] * (color.R / 255.0)); /// r
                    pixelsId[index++] = srcPixels[sourceY * targetWidth * 4 + sourceX * 4 + 3]; /// a
                }
            }

            int stride = BYTES_PER_PIXEL * targetWidth;

            var ret = new WriteableBitmap(targetWidth, targetHeight, 96, 96, PixelFormats.Bgra32, null);//WriteableBitmap.Create(targetWidth, targetHeight, 96, 96, PixelFormats.Bgra32, null, pixelsId, stride);
            ret.WritePixels(new Int32Rect(0, 0, targetWidth, targetHeight), pixelsId, stride, 0);

            return ret;
        }

        public ImageSource GetPatchedImage(int width, int height, Color color) {
            Width = width;
            Height = height;
            if (Cache.ContainsKey(String.Format("{0}x{1}", width, height))) {
                return Cache[String.Format("{0}x{1}", width, height)];
            }

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

            var xMapping = XMapping(targetWidth, targetWidth);
            var yMapping = YMapping(targetHeight, targetHeight);

            int index = 0;
            src.CopyPixels(srcPixels, targetWidth * BYTES_PER_PIXEL, 0);

            for (int row = 0; row < targetHeight; row++) {
                int sourceY = yMapping[row];
                for (int col = 0; col < targetWidth; col++) {
                    int sourceX = xMapping[col];
                    pixelsId[index++] = (byte)(srcPixels[sourceY * targetWidth * 4 + sourceX * 4 + 0] * (color.B / 255.0)); /// b
                    pixelsId[index++] = (byte)(srcPixels[sourceY * targetWidth * 4 + sourceX * 4 + 1] * (color.G / 255.0)); /// g
                    pixelsId[index++] = (byte)(srcPixels[sourceY * targetWidth * 4 + sourceX * 4 + 2] * (color.R / 255.0)); /// r
                    pixelsId[index++] = (byte)(srcPixels[sourceY * targetWidth * 4 + sourceX * 4 + 3]); /// a
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx">몇 번째 영역을 늘릴 것인지 선택하는 인자</param>
        /// <param name="width">늘릴 길이</param>
        /// <param name="totalWidth">이미지의 전체 길이</param>
        /// <returns></returns>
        private List<int> XMapping(int totalWidth, List<int> values) {
            var ret = new List<int>(totalWidth);

            int src = 0;
            int dst = 0;
            int patchIdx = 0;
            while (dst < totalWidth) {
                int loc = TopPatches[patchIdx];
                for (int i = 0; i < patchIdx; i++) loc += values[i];
                if (dst == loc) {
                    for (int i = 0; i < values[patchIdx]; i++) {
                        ret.Insert(dst++, src);
                    }
                    patchIdx = Math.Min(++patchIdx, TopPatches.Count - 1);
                } else {
                    ret.Insert(dst++, src++);
                }

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

        private List<int> YMapping(int totalHeight, List<int> values) {
            var ret = new List<int>(totalHeight);

            int src = 0;
            int dst = 0;
            int patchIdx = 0;
            while(dst < totalHeight) {
                int loc = LeftPatches[patchIdx];
                for (int i = 0; i < patchIdx; i++) loc += values[i];
                if(dst == loc) {
                    for(int i = 0; i < values[patchIdx]; i++) {
                        ret.Insert(dst++, src);
                    }
                    patchIdx = Math.Min(++patchIdx, LeftPatches.Count - 1);
                } else {
                    ret.Insert(dst++, src++);
                }

            }

            return ret;
        }
    }
}
