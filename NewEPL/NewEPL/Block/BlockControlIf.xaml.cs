using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NewEPL {
    /// <summary>
    /// BlockTest1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BlockControlIf : BlockTemplate {
        double CalcHeight = 88;
        
        public BlockControlIf() {
            InitializeComponent();
        }

        public override double GetTotalHeight() {
            double ret = ActualHeight;

            foreach (var i in GetSplicers(1)) {
                var splicer = (Splicer)VisualTreeHelper.GetChild(i, 0);

                if (splicer.YStack == 0) continue;

                foreach (var j in splicer.BlockChildren) {
                    ret += (j.Content as BlockTemplate).GetTotalHeight() - 10;
                }
            }

            return ret;
        }

        public override void IncreaseHeight(Splicer what, double height, int lv) {

            /// 나중에 스플라이서마다 이름을 지정할 예정
            var parentBlock = (BlockTemplate)(((what.Parent as Border).Parent as Canvas).Parent as Grid).Parent;
            Splicer parentSplicer = what;

            while(true) {
                if (this.Equals(parentBlock)) {
                    if (parentSplicer.YStack == 1) {
                        if (BlockParent != null) (BlockParent.Content as BlockTemplate).IncreaseHeight(what, height, lv + 1);
                        return;
                    }
                }
                if (parentBlock.BlockParent == null) {
                    break;
                }
                parentSplicer = parentBlock.SplicerParent;
                parentBlock = (BlockTemplate)(((parentBlock.SplicerParent.Parent as Border).Parent as Canvas).Parent as Grid).Parent;
            }
            
            var thumb = GetThumb();
            var image = (Image9)thumb.Template.FindName("image", thumb);

            CalcHeight += height - 10;

            image.Source = image.Patch.GetPatchedImage((int)image.ActualWidth, (int)CalcHeight, image.Color);
           
            UpdateSplicer(image, (int)image.ActualWidth, (int)CalcHeight);
        
            foreach (var i in GetSplicers(1)) {
                var splicer = (Splicer)VisualTreeHelper.GetChild(i as DependencyObject, 0);
                foreach (var j in splicer.BlockChildren) {
                    j.MoveBlocks((Parent as BlockTemplate).X + Canvas.GetLeft(i), (Parent as BlockTemplate).Y + Canvas.GetTop(i));
                    j.DifX = -((Parent as BlockTemplate).X - j.X);
                    j.DifY = -((Parent as BlockTemplate).Y - j.Y);
                }
            }

            if (BlockParent != null) {
                if (lv == 0)
                    (BlockParent.Content as BlockTemplate).IncreaseHeight(what, height - 32, lv + 1);
                else
                    (BlockParent.Content as BlockTemplate).IncreaseHeight(what, height, lv + 1);
            }
        }

        public override void DecreaseHeight(Splicer what, double height, int lv) {

            /// 나중에 스플라이서마다 이름을 지정할 예정
            var parentBlock = (BlockTemplate)(((what.Parent as Border).Parent as Canvas).Parent as Grid).Parent;
            Splicer parentSplicer = what;

            while (true) {
                if (this.Equals(parentBlock)) {
                    if (parentSplicer.YStack == 1) {
                        if (BlockParent != null) (BlockParent.Content as BlockTemplate).DecreaseHeight(what, height, lv + 1);
                        return;
                    }
                }
                if (parentBlock.BlockParent == null) {
                    break;
                }
                parentSplicer = parentBlock.SplicerParent;
                parentBlock = (BlockTemplate)(((parentBlock.SplicerParent.Parent as Border).Parent as Canvas).Parent as Grid).Parent;
            }

            var thumb = GetThumb();
            var image = (Image9)thumb.Template.FindName("image", thumb);

            CalcHeight -= height - 10;

            image.Source = image.Patch.GetPatchedImage((int)image.ActualWidth, Math.Max((int)CalcHeight, 120), image.Color);

            UpdateSplicer(image, (int)image.ActualWidth, Math.Max((int)CalcHeight, 120));

            foreach (var i in GetSplicers(1)) {
                var splicer = (Splicer)VisualTreeHelper.GetChild(i as DependencyObject, 0);
                foreach (var j in splicer.BlockChildren) {
                    j.MoveBlocks((Parent as BlockTemplate).X + Canvas.GetLeft(i), (Parent as BlockTemplate).Y + Canvas.GetTop(i));
                    j.DifX = -((Parent as BlockTemplate).X - j.X);
                    j.DifY = -((Parent as BlockTemplate).Y - j.Y);
                }
            }

            if (BlockParent != null) {
                if (lv == 0)
                    (BlockParent.Content as BlockTemplate).DecreaseHeight(what, height - 32, lv + 1);
                else
                    (BlockParent.Content as BlockTemplate).DecreaseHeight(what, height, lv + 1);
            }
        }
    }
}
