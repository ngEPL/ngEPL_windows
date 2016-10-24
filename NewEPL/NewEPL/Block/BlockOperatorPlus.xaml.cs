using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NewEPL {

    public partial class BlockOperatorPlus : BlockTemplate {
        public BlockOperatorPlus() {
            InitializeComponent();
        }

        protected override void CheckDragAction(BlockTemplate b) {

            bool escaper = false;

            var splicers = b.GetSplicers(1);

            foreach (var i in b.Main.BlockCanvas.Children) {
                if (escaper) break;
                if (i.GetType() != typeof(BlockTemplate)) continue;
                if (i.Equals(b)) continue;
                if (splicers.Count <= 0) continue;

                var other = (BlockTemplate)i;
                StackPanel contentPanel = other.GetContentPanel();
                foreach (var j in contentPanel.Children) {
                    if(j.GetType() == typeof(ExtendedTextBox)) {    
                        ExtendedTextBox textBox = (ExtendedTextBox)j;
                        var boundingBox = textBox.GetBoundingBox(other, contentPanel);
                        if (boundingBox.IntersectsWith(b.GetBoundingBox(splicers[0]))) {
                            Canvas.SetLeft(b.Main.TestPreview, boundingBox.X);
                            Canvas.SetTop(b.Main.TestPreview, boundingBox.Y);
                            b.Main.TestPreview.Visibility = System.Windows.Visibility.Visible;
                            escaper = true;
                            break;
                        }
                    }
                }
            }
        }

        protected override void CheckDropAction(BlockTemplate b) {
            bool escaper = false;

            var splicers = b.GetSplicers(1);

            foreach (var i in b.Main.BlockCanvas.Children) {
                if (escaper) break;
                if (i.GetType() != typeof(BlockTemplate)) continue;
                if (i.Equals(b)) continue;
                if (splicers.Count <= 0) continue;

                var other = (BlockTemplate)i;
                StackPanel contentPanel = other.GetContentPanel();
                foreach (var j in contentPanel.Children) {
                    if (j.GetType() == typeof(ExtendedTextBox)) {
                        ExtendedTextBox textBox = (ExtendedTextBox)j;
                        var boundingBox = textBox.GetBoundingBox(other, contentPanel);
                        if (boundingBox.IntersectsWith(b.GetBoundingBox(splicers[0]))) {
                            b.X = boundingBox.X;
                            b.Y = boundingBox.Y;

                            textBox.Visibility = System.Windows.Visibility.Hidden;
                            textBox.Width = b.ActualWidth;

                            other.SetWidth(b.ActualWidth - 40);
                            other.SetHeight(14);
                            contentPanel.Height += 14;

                            escaper = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}
