using System.Windows;
using System.Windows.Controls;

namespace NewEPL {
    public class Image9 : Image{
        public static readonly DependencyProperty DefaultWidthProperty;
        public static readonly DependencyProperty DefaultHeightProperty;
        public static readonly DependencyProperty SourceUrlProperty;

        NinePatch Patch;

        static Image9() {
            DefaultWidthProperty = DependencyProperty.Register("DefaultWidth", typeof(int), typeof(Image9), new UIPropertyMetadata(null));
            DefaultHeightProperty = DependencyProperty.Register("DefaultHeight", typeof(int), typeof(Image9), new UIPropertyMetadata(null));
            SourceUrlProperty = DependencyProperty.Register("Source", typeof(string), typeof(Image9), new UIPropertyMetadata(SourceUrlPropertyChanged));
        }

        public int DefaultWidth {
            get { return (int)GetValue(DefaultWidthProperty); }
            set { SetValue(DefaultWidthProperty, value); }
        }

        public int DefaultHeight {
            get { return (int)GetValue(DefaultHeightProperty); }
            set { SetValue(DefaultHeightProperty, value); }
        }

        public string SourceUrl {
            get { return (string)GetValue(SourceUrlProperty); }
            set { SetValue(SourceUrlProperty, value); }
        }

        private static void SourceUrlPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            var image = (Image9)obj;
            var path = (string)e.NewValue;

            image.Patch = new NinePatch(path);
            image.Source = image.Patch.GetPatchedImage(image.DefaultWidth, image.DefaultHeight);
        }
    }
}
