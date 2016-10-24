using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NewEPL {
    public class Image9 : Image {
        public static readonly DependencyProperty DefaultWidthProperty;
        public static readonly DependencyProperty DefaultHeightProperty;
        public static readonly DependencyProperty SourceUrlProperty;
        public static readonly DependencyProperty ColorProperty;

        public static readonly DependencyProperty Width0Property;
        public static readonly DependencyProperty Width1Property;
        public static readonly DependencyProperty Width2Property;
        public static readonly DependencyProperty Height0Property;
        public static readonly DependencyProperty Height1Property;
        public static readonly DependencyProperty Height2Property;

        public NinePatch Patch;

        static Image9() {
            DefaultWidthProperty = DependencyProperty.Register("DefaultWidth", typeof(int), typeof(Image9), new UIPropertyMetadata(null));
            DefaultHeightProperty = DependencyProperty.Register("DefaultHeight", typeof(int), typeof(Image9), new UIPropertyMetadata(null));

            Width0Property = DependencyProperty.Register("Width0", typeof(int), typeof(Image9), new UIPropertyMetadata(null));
            Width1Property = DependencyProperty.Register("Width1", typeof(int), typeof(Image9), new UIPropertyMetadata(null));
            Width2Property = DependencyProperty.Register("Width2", typeof(int), typeof(Image9), new UIPropertyMetadata(null));
            Height0Property = DependencyProperty.Register("Height0", typeof(int), typeof(Image9), new UIPropertyMetadata(null));
            Height1Property = DependencyProperty.Register("Height1", typeof(int), typeof(Image9), new UIPropertyMetadata(null));
            Height2Property = DependencyProperty.Register("Height2", typeof(int), typeof(Image9), new UIPropertyMetadata(null));

            SourceUrlProperty = DependencyProperty.Register("Source", typeof(string), typeof(Image9), new UIPropertyMetadata(SourceUrlPropertyChanged));
            ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(Image9), new UIPropertyMetadata(ColorPropertyChanged));
        }

        public int DefaultWidth {
            get { return (int)GetValue(DefaultWidthProperty); }
            set { SetValue(DefaultWidthProperty, value); }
        }

        public int DefaultHeight {
            get { return (int)GetValue(DefaultHeightProperty); }
            set { SetValue(DefaultHeightProperty, value); }
        }

        public int Width0 {
            get { return (int)GetValue(Width0Property); }
            set { SetValue(Width0Property, value); }
        }

        public int Width1 {
            get { return (int)GetValue(Width1Property); }
            set { SetValue(Width1Property, value); }
        }

        public int Width2 {
            get { return (int)GetValue(Width2Property); }
            set { SetValue(Width2Property, value); }
        }

        public int Height0 {
            get { return (int)GetValue(Height0Property); }
            set { SetValue(Height0Property, value); }
        }

        public int Height1 {
            get { return (int)GetValue(Height1Property); }
            set { SetValue(Height1Property, value); }
        }

        public int Height2 {
            get { return (int)GetValue(Height2Property); }
            set { SetValue(Height2Property, value); }
        }

        public string SourceUrl {
            get { return (string)GetValue(SourceUrlProperty); }
            set { SetValue(SourceUrlProperty, value); }
        }

        public Color Color {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        private static void SourceUrlPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            var image = (Image9)obj;
            var path = (string)e.NewValue;

            image.Patch = new NinePatch(path);
            image.Patch.DefaultWidth = image.DefaultWidth;
            image.Patch.DefaultHeight = image.DefaultHeight;
            image.Source = image.Patch.GetPatchedImage(new List<int>() { image.Width0, image.Width1, image.Width2 }, new List<int>() { image.Height0, image.Height1, image.Height2 }, image.Color);
        }

        private static void ColorPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            var image = (Image9)obj;
            var color = (Color)e.NewValue;
            //Image.
        }
    }
}
