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

        public int ZIndex {
            get {
                return Canvas.GetZIndex(this);
            }
            set {
                Canvas.SetZIndex(this, value);
            }
        }

        public int TempZIndex = 0;

        public BlockTemplate BlockParent = null;
        public Splicer SplicerParent = null;

        public bool IsResized = false;

        static BlockTemplate() {
        }

        public static BlockTemplate CopyBlockContent(BlockTemplate b) {
            var ret = (BlockTemplate)Activator.CreateInstance(b.GetType());
            var thumb = ret.GetThumb();
            var canvas = ret.GetCanvas();

            ret.Width = b.Width;

            thumb.ApplyTemplate();
            thumb.DragStarted += Thumb_DragStarted;
            thumb.DragDelta += Thumb_DragDelta;
            thumb.DragCompleted += Thumb_DragCompleted;

            var image = (Image9)thumb.Template.FindName("image", thumb);
            foreach (var i in canvas.Children) {
                var border = i as Border;
                var splicer = (Splicer)VisualTreeHelper.GetChild(i as DependencyObject, 0);
                Canvas.SetLeft(border, (splicer.X + (image.Patch.GetImmutableWidth(0) + image.Patch.GetStrectedWidth(image.DefaultWidth)) * 0));
                Canvas.SetTop(border, (splicer.Y + (image.Patch.GetImmutableHeight(splicer.YStack) + image.Patch.GetStrectedHeight(image.DefaultHeight)) * splicer.YStack));

                if (Double.IsNaN(splicer.Width)) {
                    splicer.Width = ret.Width + splicer.RelativeWidth; /// 0.8 -> 배율
                }
            }

            b.ZIndex = 0;

            return ret;
        }

        public void MoveBlocks(double x, double y) {
            X = x;
            Y = y;

            foreach (var i in GetSplicers(1)) {
                var splicer = (Splicer)VisualTreeHelper.GetChild(i as DependencyObject, 0);
                foreach (var j in splicer.BlockChildren) {
                    j.MoveBlocks(j.DifX + x, j.DifY + y);
                }
            }
        }

        private void SetZIndex(int zindex) {
            ZIndex = zindex;
            TempZIndex = zindex;

            foreach (var i in GetSplicers(1)) {
                var splicer = (Splicer)VisualTreeHelper.GetChild(i, 0);

                foreach (var j in splicer.BlockChildren) {
                    j.SetZIndex(ZIndex + 1);
                }
            }
        }

        private void SetTempZIndex(int zindex) {
            TempZIndex = ZIndex;
            ZIndex = zindex;

            foreach (var i in GetSplicers(1)) {
                var splicer = (Splicer)VisualTreeHelper.GetChild(i, 0);

                foreach (var j in splicer.BlockChildren) {
                    j.SetTempZIndex(ZIndex + 1);
                }
            }
        }

        public void AddChild(BlockTemplate child, Border border) {
            var splicer = (Splicer)VisualTreeHelper.GetChild(border as DependencyObject, 0);
            (child.Content as BlockTemplate).BlockParent = this;
            (child.Content as BlockTemplate).SplicerParent = splicer;
            splicer.BlockChildren.Add(child);
            child.MoveBlocks(X + Canvas.GetLeft(border), Y + Canvas.GetTop(border));
            child.DifX = -(this.X - child.X);
            child.DifY = -(this.Y - child.Y);
            SetZIndex(ZIndex + 1);
        }

        public Thumb GetThumb() {
            if (this.GetType() == typeof(BlockTemplate))
                return (Thumb)VisualTreeHelper.GetChild((DependencyObject)(Content as BlockTemplate).Content, 0);
            return (Thumb)VisualTreeHelper.GetChild((DependencyObject)Content, 0);
        }

        public Canvas GetCanvas() {
            if (this.GetType() == typeof(BlockTemplate))
                return (Canvas)VisualTreeHelper.GetChild((DependencyObject)(Content as BlockTemplate).Content, 1);
            return (Canvas)VisualTreeHelper.GetChild((DependencyObject)Content, 1);
        }

        public Splicer GetSplicer(int idx) {
            var canvas = (Canvas)VisualTreeHelper.GetChild((DependencyObject)(Content as BlockTemplate).Content, 1);
            return (Splicer)VisualTreeHelper.GetChild(canvas.Children[idx] as DependencyObject, 0);
        }

        /// <summary>
        /// 임시로 Border를 가져오게 함.
        /// </summary>
        /// <param name="what">0일 때 Type=False만 가져옴. 1일 때 Type=True만 가져옴. -1일 때 모두 가져옴.</param>
        /// <returns></returns>
        public List<Border> GetSplicers(int what) {
            List<Border> ret = new List<Border>();
            var canvas = GetCanvas();

            foreach(var i in canvas.Children) {
                var border = (Border)i;//(Border)VisualTreeHelper.GetChild(i as DependencyObject, 0);
                var splicer = (Splicer)VisualTreeHelper.GetChild(i as DependencyObject, 0);
                if (what == -1) ret.Add(border);
                else if (splicer.Type == Convert.ToBoolean(what)) ret.Add(border);
            }

            return ret;
        }

        protected void RemoveChild(BlockTemplate child) {
            foreach(var i in GetSplicers(1)) {
                var splicer = (Splicer)VisualTreeHelper.GetChild(i as DependencyObject, 0);
                foreach(var j in splicer.BlockChildren) {
                    if(j.Equals(child)) {
                        splicer.BlockChildren.Remove(j);
                        return;
                    }
                }
            }
        }

        protected void UpdateSplicer(Image9 image, int width, int height) {
            var canvas = GetCanvas();
            foreach (var i in canvas.Children) {
                var border = i as Border;
                var splicer = (Splicer)VisualTreeHelper.GetChild(i as DependencyObject, 0);
                Canvas.SetLeft(border, (splicer.X + (image.Patch.GetImmutableWidth(0) + image.Patch.GetStrectedWidth(width)) * 0));
                Canvas.SetTop(border, (splicer.Y + (image.Patch.GetImmutableHeight(splicer.YStack) + image.Patch.GetStrectedHeight(height)) * splicer.YStack));

                if (Double.IsNaN(splicer.Width)) {
                    splicer.Width = Width + splicer.RelativeWidth * 0.8; /// 0.8 -> 배율
                }
            }
        }

        private Rect GetBoundingBox(Border border) {
            var splicer = (Splicer)VisualTreeHelper.GetChild(border, 0);
            return new Rect(X + Canvas.GetLeft(border), Y + Canvas.GetTop(border), splicer.Width, splicer.Height);
        }

        // 블록마다 길이 구하는 방식이 다를 수 있음 (IF블록 같은 경우 안쪽 자식 블록은 계산하지 않고 밑 블록만 계산에 넣음)
        public virtual double GetTotalHeight() {
            double ret = ActualHeight;

            foreach(var i in GetSplicers(1)) {
                var splicer = (Splicer)VisualTreeHelper.GetChild(i, 0);

                foreach(var j in splicer.BlockChildren) {
                    ret += (j.Content as BlockTemplate).GetTotalHeight() - 10;
                }
            }

            return ret;
        }

        public virtual BlockTemplate IncreaseSize(Splicer what, double width, double height, int cnt) {
            if(BlockParent != null) {
                (BlockParent.Content as BlockTemplate).IncreaseSize(what, 0, height, cnt + 1);
            }
            return this;
        }

        public virtual BlockTemplate DecreaseSize(Splicer what, double width, double height, int cnt) {
            if (BlockParent != null) {
                (BlockParent.Content as BlockTemplate).DecreaseSize(what, 0, height, cnt + 1);
            }
            return this;
        }

        private static void Thumb_DragStarted(object sender, DragStartedEventArgs e) {
            var b = (((sender as Thumb).Parent as Grid).Parent as BlockTemplate).Parent as BlockTemplate;

            b.SetTempZIndex(99999999);

            b.Main = (MainWindow)Window.GetWindow(b);
        }

        public Border CollideBorder = null;
        public Splicer CollideSplicer = null;

        /// 이미지 늘어나는 기능을 따로 빼기.
        private static void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e) {
            var b = (((sender as Thumb).Parent as Grid).Parent as BlockTemplate).Parent as BlockTemplate;
            bool escaper = false;

            b.MoveBlocks(b.X + e.HorizontalChange, b.Y + e.VerticalChange);

            //var canvas = (b.Parent as Canvas);
            //b.X = Math.Min(Math.Max(0, b.X), canvas.ActualWidth - b.ActualWidth);
            //b.Y = Math.Min(Math.Max(0, b.Y), canvas.ActualHeight - b.ActualHeight);

            //b.MoveBlocks(b.X, b.Y);

            if ((b.Content as BlockTemplate).BlockParent != null) {
                (b.Content as BlockTemplate).BlockParent.RemoveChild(b);
                (b.Content as BlockTemplate).BlockParent = null;
            }

            bool isCollision = false;

            foreach (var i in b.Main.BlockCanvas.Children) {
                if (escaper) break;
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

                    if(otherSplicer.BlockChildren.Count > 0) continue;

                    if (b.GetBoundingBox(thisBorder).IntersectsWith(other.GetBoundingBox(otherBorder))) {
                        Canvas.SetLeft(b.Main.TestPreview, other.X + Canvas.GetLeft(otherBorder));
                        Canvas.SetTop(b.Main.TestPreview, other.Y + Canvas.GetTop(otherBorder));
                        b.Main.TestPreview.Visibility = Visibility.Visible;

                        b.CollideBorder = otherBorder;
                        b.CollideSplicer = otherSplicer;
                        isCollision = true;

                        escaper = true;
                        break;
                    }
                }
            }

            if (!isCollision) {
                if (b.IsResized) {
                    (((b.CollideBorder.Parent as Canvas).Parent as Grid).Parent as BlockTemplate).DecreaseSize(b.CollideSplicer, 0, (b.Content as BlockTemplate).GetTotalHeight(), 0);
                    b.IsResized = false;
                }
            }
        }

        /// 추가해야하는 기능. 겹치는 두 블록 그룹에 자식을 추가할 때 마우스 포인터와 충돌해 있는 블록 그룹에 자식이 붙어야 함.
        private static void Thumb_DragCompleted(object sender, DragCompletedEventArgs e) {
            var b = (((sender as Thumb).Parent as Grid).Parent as BlockTemplate).Parent as BlockTemplate;
            bool escaper = false;

            foreach (var i in b.Main.BlockCanvas.Children) {
                if (escaper) break;
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

                    if (otherSplicer.BlockChildren.Count > 0) continue;

                    if (b.GetBoundingBox(thisBorder).IntersectsWith(other.GetBoundingBox(otherBorder))) {
                        other.AddChild(b, j);
                        b.Main.TestPreview.Visibility = Visibility.Hidden;

                        if (!b.IsResized) {
                            (((otherBorder.Parent as Canvas).Parent as Grid).Parent as BlockTemplate).IncreaseSize(otherSplicer, 0, (b.Content as BlockTemplate).GetTotalHeight(), 0);
                            b.IsResized = true;
                        }

                        escaper = true;
                        break;
                    }
                }
            }

            b.SetZIndex(b.TempZIndex);
        }
    }
}
