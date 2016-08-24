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

            thumb.ApplyTemplate();

            var image = (Image9)thumb.Template.FindName("image", thumb);

            thumb.DragDelta += Thumb_DragDelta;

            foreach (var i in canvas.Children) {
                var border = i as Border;
                Canvas.SetTop(border, Canvas.GetTop(border) + image.DefaultHeight - 16);
                var splicer = (Splicer)VisualTreeHelper.GetChild(i as DependencyObject, 0);
                //var a = (Image9)thumb.GetTemplateChild();
                //thumb.Template.CHild
                //MessageBox.Show(((Image9)VisualTreeHelper.GetChild(thumb as DependencyObject, 0)).ToString());
            }

            return ret;
        }

        private static void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e) {
            var b = (((sender as Thumb).Parent as Grid).Parent as BlockTemplate).Parent as BlockTemplate;
            b.X += e.HorizontalChange;
            b.Y += e.VerticalChange;
        }
    }
}
