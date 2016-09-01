using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NewEPL {
    /// <summary>
    /// BlockTest1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BlockTest4 : BlockTemplate {
        public BlockTest4() {
            InitializeComponent();
        }

        public override BlockTemplate Resize(Splicer what, double width, double height) {

            if (BlockParent != null) {
                (BlockParent.Content as BlockTemplate).Resize(what, 0, height);
            }

            /// 나중에 스플라이서마다 이름을 지정할 예정
            var parentBlock = (BlockTemplate)(((what.Parent as Border).Parent as Canvas).Parent as Grid).Parent;
            Splicer parentSplicer = what;

            while(true) {
                if (this.Equals(parentBlock)) {
                    if (parentSplicer.YStack == 1) {
                        return this;
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

            double w = width;   
            double h = height;

            if (Double.IsNaN(width)) {
                w = image.Patch.Width;
            }
            if (Double.IsNaN(height)) {
                h = image.Patch.Height;
            }

            image.Source = image.Patch.GetPatchedImage((int)image.Patch.Width, (int)(ActualHeight * 1.25 + height - 6));

            /// 모든 자식 크기만큼 늘어나게 해야할듯.
            UpdateSplicer(image, (int)image.Patch.Width, (int)(ActualHeight * 1.25 + height - 6));
            
            foreach (var i in GetSplicers(1)) {
                var splicer = (Splicer)VisualTreeHelper.GetChild(i as DependencyObject, 0);
                foreach (var j in splicer.BlockChildren) {
                    j.MoveBlocks((Parent as BlockTemplate).X + Canvas.GetLeft(i), (Parent as BlockTemplate).Y + Canvas.GetTop(i));
                    j.DifX = -((Parent as BlockTemplate).X - j.X);
                    j.DifY = -((Parent as BlockTemplate).Y - j.Y);
                }
            }

            return this;
        }
    }
}
