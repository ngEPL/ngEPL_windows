using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace NewEPL {
    public class BlockTemplate : UserControl{

        public double X {
            get {
                return Canvas.GetLeft(this);
            }
            set {
                Canvas.SetLeft(this, value);
            }
        }

        public double Y {
            get {
                return Canvas.GetTop(this);
            }
            set {
                Canvas.SetTop(this, value);
            }
        }

        static BlockTemplate() {
        }

        public static BlockTemplate CreateBlock(BlockTemplate b) {
            var ret = (BlockTemplate)Activator.CreateInstance(b.GetType());
            var thumb = (Thumb)VisualTreeHelper.GetChild((DependencyObject)ret.Content, 0);
            var canvas = (Canvas)VisualTreeHelper.GetChild((DependencyObject)ret.Content, 1);

            ret.Width = b.Width;

            thumb.ApplyTemplate();
            thumb.DragDelta += Thumb_DragDelta;

            var image = (Image9)thumb.Template.FindName("image", thumb);
            foreach (var i in canvas.Children) {
                var border = i as Border;
                var splicer = (Splicer)VisualTreeHelper.GetChild(i as DependencyObject, 0);
                Canvas.SetLeft(border, (Canvas.GetLeft(border) + (image.Patch.GetImmutableWidth(splicer.XX) + image.Patch.GetStrectedWidth(image.DefaultWidth)) * splicer.XX) * 0.8);
                Canvas.SetTop(border, (Canvas.GetTop(border) + (image.Patch.GetImmutableHeight(splicer.XX) + image.Patch.GetStrectedHeight(image.DefaultHeight)) * splicer.XX) * 0.8);

                if (Double.IsNaN(splicer.Width)) {
                    splicer.Width = ret.Width + splicer.RelativeWidth * 0.8; /// 0.8 -> 배율
                }
            }

            return ret;
        }

        private static void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e) {
            var b = (((sender as Thumb).Parent as Grid).Parent as BlockTemplate).Parent as BlockTemplate;
            b.X += e.HorizontalChange;
            b.Y += e.VerticalChange;

            var canvas = (b.Parent as Canvas);
            b.X = Math.Min(Math.Max(0, b.X), canvas.ActualWidth - b.ActualWidth);
            b.Y = Math.Min(Math.Max(0, b.Y), canvas.ActualHeight - b.ActualHeight);
        }
    }
}
