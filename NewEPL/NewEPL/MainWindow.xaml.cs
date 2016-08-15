using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NewEPL {
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window {
        List<BlockList> toggleList;
        Window dw = null;

        public MainWindow() {
            InitializeComponent();

            toggleList = new List<BlockList>();

            var blocks_if = new List<BlockData>();
            blocks_if.Add(new BlockData() { Size = (int)(210 * 0.8), Res = "pack://siteoforigin:,,,/Resources/block_test_3.png", Tag = BlockType.TEST3 });
            blocks_if.Add(new BlockData() { Size = (int)(210 * 0.8), Res = "pack://siteoforigin:,,,/Resources/block_test_1.png", Tag = BlockType.TEST1 });
            blocks_if.Add(new BlockData() { Size = (int)(210 * 0.8), Res = "pack://siteoforigin:,,,/Resources/block_test_2.png", Tag = BlockType.TEST2 });

            var blocks_for = new List<BlockData>();
            blocks_for.Add(new BlockData() { Size = (int)(285 * 0.8), Res = "pack://siteoforigin:,,,/Resources/block_test_4.png", Tag = BlockType.TEST4 });

            var blocks_text = new List<BlockData>();

            var blocks_list = new List<BlockData>();

            var blocks_microbit = new List<BlockData>();

            var blocks_arduino = new List<BlockData>();

            toggleList.Add(new BlockList() { Name = "조건", Source = blocks_if });
            toggleList.Add(new BlockList() { Name = "반복", Source = blocks_for });
            toggleList.Add(new BlockList() { Name = "텍스트", Source = blocks_text });
            toggleList.Add(new BlockList() { Name = "리스트", Source = blocks_list });
            toggleList.Add(new BlockList() { Name = "마이크로 비트", Source = blocks_microbit });
            toggleList.Add(new BlockList() { Name = "아두이노", Source = blocks_arduino });

            BlockLists.ItemsSource = toggleList;
        }

        void CreateDragDropWindow(Visual e) {
            dw = new Window();
            dw.WindowStyle = WindowStyle.None;
            dw.AllowsTransparency = true;
            dw.AllowDrop = false;
            dw.Background = null;
            dw.IsHitTestVisible = false;
            dw.SizeToContent = SizeToContent.WidthAndHeight;
            dw.Topmost = true;
            dw.ShowInTaskbar = false;

            Rectangle r = new Rectangle();
            r.Width = ((FrameworkElement)e).ActualWidth;
            r.Height = ((FrameworkElement)e).ActualHeight;
            r.Fill = new VisualBrush(e);
            dw.Content = r;


            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            this.dw.Left = w32Mouse.X;
            this.dw.Top = w32Mouse.Y;
            this.dw.Show();

        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point {
            public Int32 X;
            public Int32 Y;
        };

        

        private void image_Drag(object sender, MouseButtonEventArgs e) {
            var image = e.Source as Image;
            var data = new DataObject(typeof(SourceAndType), new SourceAndType() { Source = image.Source, Type = (BlockType)image.Tag});
            DragDrop.DoDragDrop(image, data, DragDropEffects.Move);
        }

        private void canvas_Drop(object sender, DragEventArgs e) {
            var sat = e.Data.GetData(typeof(SourceAndType)) as SourceAndType;
            var copy = new Block(sat.Source, e.GetPosition(this.canvas).X, e.GetPosition(this.canvas).Y, sat.Source.Width * 0.8, sat.Type);
            copy.DragDelta += Thumb_DragDelta;
            copy.DragCompleted += Thumb_DragCompleted;
            this.canvas.Children.Add(copy);
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e) {
            var idx = Int32.Parse((sender as ToggleButton).Tag.ToString());
            (BlockLists.ItemContainerGenerator.ContainerFromIndex(idx) as ListBoxItem).Visibility = Visibility.Visible;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e) {
            var idx = Int32.Parse((sender as ToggleButton).Tag.ToString());
            (BlockLists.ItemContainerGenerator.ContainerFromIndex(idx) as ListBoxItem).Visibility = Visibility.Collapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            ToggleCheck(Toggle_if, true);
            ToggleCheck(Toggle_for, false);
            ToggleCheck(Toggle_text, true);
            ToggleCheck(Toggle_list, false);
            ToggleCheck(Toggle_microbit, false);
            ToggleCheck(Toggle_arduino, true);
        }

        private void ToggleCheck(ToggleButton btn, bool v) {
            if (v) {
                btn.IsChecked = true;
            } else {
                btn.IsChecked = false;
                ToggleButton_Unchecked(btn, null);
            }
        }

        private void Image_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e) {
            var b = sender as Block;
            b.XX += e.HorizontalChange;
            b.YY += e.VerticalChange;
            b.Update();
            //b.MoveBlocks(b.X + e.HorizontalChange, b.Y + e.VerticalChange);
            //b.X += e.HorizontalChange;
            //b.Y += e.VerticalChange;
            //foreach(var i in b.Children) {
            //    i.X += e.HorizontalChange;
            //    i.Y += e.VerticalChange;
            //}
            //string txt = "";
            //foreach(Block i in canvas.Children) {
            //    txt += "Down: " + i.Cols["Down"].X.ToString() + ", " + i.Cols["Down"].Y.ToString() + "\n" + "Up: " + i.Cols["Up"].X.ToString() + ", " + i.Cols["Up"].Y.ToString() + "\n" + "--------------------------------------------------------" + "\n";
            //}
            //test.Content = txt;
            test.Content = b.X.ToString() + ", " + b.Y.ToString();
            //test.Content = (canvas.Children[0] as Block).Cols[new KeyValuePair<bool, string>(true, "Down")][0].X.ToString() + ", " + (canvas.Children[0] as Block).Cols[new KeyValuePair<bool, string>(true, "Down")][0].Y.ToString();
        }
        
        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e) { 
            var b = sender as Block;

            /// 붙이기
            if (b.IsFemale) {
                /// canvas.Children 대신 각 마지막 자식들이 들어가야함. 아니면 검색하면서 자식이 있는 블록은 무시하기.
                foreach(var i in canvas.Children) {
                    if (i == b) continue;

                    var other = i as Block;
                    /// 블록의 어느 위치에 붙일 지는 아직 안만듦, 무조건 아래쪽에 붙는다고 가정하고 만들어짐.
                    foreach (var j in other.Cols) {
                        if (!j.Key.Key) continue;
                        if (b.GetFemale().IntersectsWith(j.Value[0])) {
                            MessageBox.Show("Get!");
                            other.AddChild(b);
                        }
                    }
                }
            }
            //if (b.haveConcave) {
            //    foreach (var i in canvas.Children) {
            //        if (i == b) continue;

            //        var child = i as Block;
            //        //if (!child.haveConvex) continue;
            //        if(child.Cols[new KeyValuePair<bool, string>(true, "Down")].IntersectsWith(b.Cols[new KeyValuePair<bool, string>(false, "Up")])) {
            //            MessageBox.Show("Check!");
            //            child.AddChild(b);
            //        }
            //    }
            //}
        }
    }
}
