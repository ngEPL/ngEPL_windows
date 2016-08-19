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
        List<BlockList> ToggleList;
        Rectangle TestPreview;

        int TestCount = 0;

        int ZIndexManager = 0;

        public MainWindow() {
            InitializeComponent();

            ToggleList = new List<BlockList>();

            var blocks_if = new List<BlockData>();
            blocks_if.Add(new BlockData() { Size = (int)(210 * 0.8), Res = "pack://siteoforigin:,,,/Resources/block_test_3.png", Tag = BlockType.TEST3 });
            blocks_if.Add(new BlockData() { Size = (int)(210 * 0.8), Res = "pack://siteoforigin:,,,/Resources/block_test_1.png", Tag = BlockType.TEST1 });
            blocks_if.Add(new BlockData() { Size = (int)(210 * 0.8), Res = "pack://siteoforigin:,,,/Resources/block_test_2.png", Tag = BlockType.TEST2 });
            blocks_if.Add(new BlockData() { Size = (int)(82), Res = "pack://siteoforigin:,,,/Resources/btn_dropdown_normal.9.png", Tag = BlockType.TEST2 });

            var blocks_for = new List<BlockData>();
            blocks_for.Add(new BlockData() { Size = (int)(285 * 0.8), Res = "pack://siteoforigin:,,,/Resources/block_test_4.png", Tag = BlockType.TEST4 });

            var blocks_text = new List<BlockData>();

            var blocks_list = new List<BlockData>();

            var blocks_microbit = new List<BlockData>();

            var blocks_arduino = new List<BlockData>();

            ToggleList.Add(new BlockList() { Name = "조건", Source = blocks_if });
            ToggleList.Add(new BlockList() { Name = "반복", Source = blocks_for });
            ToggleList.Add(new BlockList() { Name = "텍스트", Source = blocks_text });
            ToggleList.Add(new BlockList() { Name = "리스트", Source = blocks_list });
            ToggleList.Add(new BlockList() { Name = "마이크로 비트", Source = blocks_microbit });
            ToggleList.Add(new BlockList() { Name = "아두이노", Source = blocks_arduino });

            BlockLists.ItemsSource = ToggleList;

            TestPreview = new Rectangle {
                Fill = Brushes.LightBlue,
                Width = 15,
                Height = 15
            };

            Canvas.SetLeft(TestPreview, 0);
            Canvas.SetTop(TestPreview, 0);
            Canvas.SetZIndex(TestPreview, 10000000);
            canvas.Children.Add(TestPreview);

            TestPreview.Visibility = Visibility.Hidden;
        }

        private void image_Drag(object sender, MouseButtonEventArgs e) {
            var image = e.Source as Image;
            var data = new DataObject(typeof(SourceAndType), new SourceAndType() { Source = image.Source, Type = (BlockType)image.Tag});
            DragDrop.DoDragDrop(image, data, DragDropEffects.Move);
        }

        private void canvas_Drop(object sender, DragEventArgs e) {
            var sat = e.Data.GetData(typeof(SourceAndType)) as SourceAndType;
            var copy = new Block(sat.Source, e.GetPosition(this.canvas).X, e.GetPosition(this.canvas).Y, sat.Source.Width * 0.8, sat.Type);
            copy.DragStarted += Thumb_DragStarted;
            copy.DragDelta += Thumb_DragDelta;
            copy.DragCompleted += Thumb_DragCompleted;
            Canvas.SetZIndex(copy, ZIndexManager++);
            this.canvas.Children.Add(copy);
            copy.TestNum = TestCount++;
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
            ToggleCheck(Toggle_for, true);
            ToggleCheck(Toggle_text, true);
            ToggleCheck(Toggle_list, true);
            ToggleCheck(Toggle_microbit, true);
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

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e) {
            var b = sender as Block;

            /// 이프문 블럭같은 경우 이프문 안쪽에 들어가는 블록이 존재하는데 
            /// 이프문의 zindex가 안쪽의 블록보다 높을 경우 안쪽의 블록을 선택할 수 없는 문제가 있음.
            Canvas.SetZIndex(b, 1000000);
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e) {
            var b = sender as Block;
            bool escaper = false;
            b.MoveBlocks(b.X + e.HorizontalChange, b.Y + e.VerticalChange);

            /// 일단 움직이면 자식에서 없애버림.
            /// 부모가 있다면,
            if (b.Parent != null) {
                var me = new KeyValuePair<bool, string>();
                foreach(var i in b.Parent.Children) {
                    if(i.Value == b) {
                        me = i.Key;
                        break;
                    }
                }
                b.Parent.Children.Remove(me);
                b.Parent = null;
            }

            /// 어디에 붙일건지 표시
            if (b.IsFemale) {
                /// canvas.Children 대신 각 마지막 자식들이 들어가야함. 아니면 검색하면서 자식이 있는 블록은 무시하기.
                foreach (var i in canvas.Children) {
                    if (escaper) break;
                    if (i == b) continue;
                    /// 나중에 캔버스 사용하지 말고 리스트를 따로 만들기.
                    if (i.GetType() != typeof(Block)) continue;
                   
                    var other = i as Block;
                    /// 블록의 어느 위치에 붙일 지는 아직 안만듦, 무조건 아래쪽에 붙는다고 가정하고 만들어짐.
                    foreach (var j in other.Cols) {
                        if (!j.Key.Key) continue;
                        if (other.Children.ContainsKey(j.Key)) continue;
                        if (Block.RealCollider(b, b.GetFemale()).IntersectsWith(Block.RealCollider(other, j.Value))) {
                            Canvas.SetLeft(TestPreview, other.X + j.Value.X);
                            Canvas.SetTop(TestPreview, other.Y + j.Value.Y);
                            TestPreview.Visibility = Visibility.Visible;
                            escaper = true;
                        }
                    }
                } 
            }
        }
        
        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e) { 
            var b = sender as Block;
            bool escaper = false;
            /// 붙이기
            if (b.IsFemale) {
                /// canvas.Children 대신 각 마지막 자식들이 들어가야함. 아니면 검색하면서 자식이 있는 블록은 무시하기.
                foreach(var i in canvas.Children) {
                    if (escaper) break;
                    if (i == b) continue;
                    if (i.GetType() != typeof(Block)) continue;

                    var other = i as Block;

                    foreach (var j in other.Cols) {
                        if (!j.Key.Key) continue;
                        if (other.Children.ContainsKey(j.Key)) continue;
                        if (Block.RealCollider(b, b.GetFemale()).IntersectsWith(Block.RealCollider(other, j.Value))) {
                            other.AddChild(j.Key, b, Block.RealCollider(other, j.Value));
                            TestPreview.Visibility = Visibility.Hidden;
                            escaper = true;
                        } 
                    }
                }
            }
        }
    }
}
