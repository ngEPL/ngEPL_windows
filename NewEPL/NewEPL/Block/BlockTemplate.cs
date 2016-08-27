using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace NewEPL {
    public class BlockTemplate : UserControl{

        MainWindow Main;

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

        public double DifX = 0;
        public double DifY = 0;

        List<BlockTemplate> BlockChildren = new List<BlockTemplate>();
        BlockTemplate BlockParent = null;

        static BlockTemplate() {
        }

        public static BlockTemplate CreateBlock(BlockTemplate b) {
            var ret = (BlockTemplate)Activator.CreateInstance(b.GetType());
            var thumb = (Thumb)VisualTreeHelper.GetChild((DependencyObject)ret.Content, 0);
            var canvas = (Canvas)VisualTreeHelper.GetChild((DependencyObject)ret.Content, 1);

            ret.Width = b.Width;

            thumb.ApplyTemplate();
            thumb.DragStarted += Thumb_DragStarted;
            thumb.DragDelta += Thumb_DragDelta;
            thumb.DragCompleted += Thumb_DragCompleted;

            var image = (Image9)thumb.Template.FindName("image", thumb);
            ret.UpdateSplicer(image);

            return ret;
        }

        private void MoveBlocks(double x, double y) {
            X = x;
            Y = y;

            foreach (var i in BlockChildren) {
                i.MoveBlocks(i.DifX + x, i.DifY + y);
            }
        }

        private void AddChild(BlockTemplate child, Border border) {
            child.BlockParent = this;
            BlockChildren.Add(child);
            child.MoveBlocks(X + Canvas.GetLeft(border), Y+ Canvas.GetTop(border));
            child.DifX = -(this.X - child.X);
            child.DifY = -(this.Y - child.Y);
        }

        private Splicer GetSplicer(int idx) {
            var canvas = (Canvas)VisualTreeHelper.GetChild((DependencyObject)(Content as BlockTemplate).Content, 1);
            return (Splicer)VisualTreeHelper.GetChild(canvas.Children[idx] as DependencyObject, 0);
        }

        /// <summary>
        /// 임시로 Border를 가져오게 함.
        /// </summary>
        /// <param name="what">0일 때 Type=False만 가져옴. 1일 때 Type=True만 가져옴. -1일 때 모두 가져옴.</param>
        /// <returns></returns>
        private List<Border> GetSplicers(int what) {
            List<Border> ret = new List<Border>();
            var canvas = (Canvas)VisualTreeHelper.GetChild((DependencyObject)(Content as BlockTemplate).Content, 1);

            foreach(var i in canvas.Children) {
                var border = (Border)i;//(Border)VisualTreeHelper.GetChild(i as DependencyObject, 0);
                var splicer = (Splicer)VisualTreeHelper.GetChild(i as DependencyObject, 0);
                if (what == -1) ret.Add(border);
                else if (splicer.Type == Convert.ToBoolean(what)) ret.Add(border);
            }

            return ret;
        }

        private void UpdateSplicer(Image9 image) {
            var canvas = (Canvas)VisualTreeHelper.GetChild((DependencyObject)Content, 1);
            foreach (var i in canvas.Children) {
                var border = i as Border;
                var splicer = (Splicer)VisualTreeHelper.GetChild(i as DependencyObject, 0);
                Canvas.SetLeft(border, (Canvas.GetLeft(border) + (image.Patch.GetImmutableWidth(0) + image.Patch.GetStrectedWidth(image.DefaultWidth)) * 0) * 0.8);
                Canvas.SetTop(border, (Canvas.GetTop(border) + (image.Patch.GetImmutableHeight(splicer.XX) + image.Patch.GetStrectedHeight(image.DefaultHeight)) * splicer.XX) * 0.8);

                if (Double.IsNaN(splicer.Width)) {
                    splicer.Width = Width + splicer.RelativeWidth * 0.8; /// 0.8 -> 배율
                }
            }
        }

        private Rect GetBoundingBox(Border border) {
            var splicer = (Splicer)VisualTreeHelper.GetChild(border, 0);
            return new Rect(X + Canvas.GetLeft(border), Y + Canvas.GetTop(border), splicer.Width, splicer.Height);
        }

        private static void Thumb_DragStarted(object sender, DragStartedEventArgs e) {
            var b = (((sender as Thumb).Parent as Grid).Parent as BlockTemplate).Parent as BlockTemplate;

            /// 이프문 블럭같은 경우 이프문 안쪽에 들어가는 블록이 존재하는데 
            /// 이프문의 zindex가 안쪽의 블록보다 높을 경우 안쪽의 블록을 선택할 수 없는 문제가 있음.
            Canvas.SetZIndex(b, 1000000);

            b.Main = (MainWindow)Window.GetWindow(b);
        }

        private static void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e) {
            var b = (((sender as Thumb).Parent as Grid).Parent as BlockTemplate).Parent as BlockTemplate;

            b.MoveBlocks(b.X + e.HorizontalChange, b.Y + e.VerticalChange);

            foreach(var i in b.Main.canvas.Children) {
                if (i.GetType() != typeof(BlockTemplate)) continue;
                if (i.Equals(b)) continue;

                var splicers = b.GetSplicers(0);
                if (splicers.Count <= 0) continue;

                var other = (BlockTemplate)i;
                foreach(var j in other.GetSplicers(1)) {
                    var thisBorder = splicers[0];
                    var thisSplicer = (Splicer)VisualTreeHelper.GetChild(thisBorder, 0);
                    var otherBorder = (Border)j;
                    var otherSplicer = (Splicer)VisualTreeHelper.GetChild(otherBorder, 0);
                    if (b.GetBoundingBox(thisBorder).IntersectsWith(other.GetBoundingBox(otherBorder))) {
                        Canvas.SetLeft(b.Main.TestPreview, other.X + Canvas.GetLeft(otherBorder));
                        Canvas.SetTop(b.Main.TestPreview, other.Y + Canvas.GetTop(otherBorder));
                        b.Main.TestPreview.Visibility = Visibility.Visible;
                    }
                }
            }
            //var canvas = (b.Parent as Canvas);
            //b.X = Math.Min(Math.Max(0, b.X), canvas.ActualWidth - b.ActualWidth);
            //b.Y = Math.Min(Math.Max(0, b.Y), canvas.ActualHeight - b.ActualHeight); 
        }

        /// 추가해야하는 기능. 겹치는 두 블록 그룹에 자식을 추가할 때 마우스 포인터와 충돌해 있는 블록 그룹에 자식이 붙어야 함.
        private static void Thumb_DragCompleted(object sender, DragCompletedEventArgs e) {
            var b = (((sender as Thumb).Parent as Grid).Parent as BlockTemplate).Parent as BlockTemplate;

            foreach (var i in b.Main.canvas.Children) {
                if (i.GetType() != typeof(BlockTemplate)) continue;
                if (i.Equals(b)) continue;

                var splicers = b.GetSplicers(0);
                if (splicers.Count <= 0) continue;

                var other = (BlockTemplate)i;
                foreach (var j in other.GetSplicers(1)) {
                    var thisBorder = splicers[0];
                    var thisSplicer = (Splicer)VisualTreeHelper.GetChild(thisBorder, 0);
                    var otherBorder = (Border)j;
                    var otherSplicer = (Splicer)VisualTreeHelper.GetChild(otherBorder, 0);
                    if (b.GetBoundingBox(thisBorder).IntersectsWith(other.GetBoundingBox(otherBorder))) {
                        other.AddChild(b, j);
                        b.Main.TestPreview.Visibility = Visibility.Hidden;
                    }
                }
            }
        }
    }
}
