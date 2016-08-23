using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NewEPL {
    /// <summary>
    /// BlockTest1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BlockTest1 : BlockTemplate {
        public BlockTest1() {
            InitializeComponent();
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e) {
            var b = (((sender as Thumb).Parent as Grid).Parent as BlockTest1).Parent as BlockTemplate;
            Canvas.SetLeft(b, Canvas.GetLeft(b) + e.HorizontalChange);
            Canvas.SetTop(b, Canvas.GetTop(b) + e.VerticalChange);
        }
    }
}
