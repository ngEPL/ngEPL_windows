using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace NewEPL {
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window {
        List<BlockList> ToggleList;
        public Rectangle TestPreview;

        public MainWindow() {
            InitializeComponent();

            ToggleList = new List<BlockList>();

            var blockGroupMotion = new List<BlockData>();
            blockGroupMotion.Add(new BlockData() { Template = new BlockMotionMove() { Width=210 * 0.8 } });

            var blockGroupEvent = new List<BlockData>();
            blockGroupEvent.Add(new BlockData() { Template = new BlockEventStart() { Width=210 * 0.8 } });

            var blockGroupControl = new List<BlockData>();
            blockGroupControl.Add(new BlockData() { Template = new BlockControlIf() { Width = 280 * 0.8 } });
            blockGroupControl.Add(new BlockData() { Template = new BlockControlStop() { Width = 210 * 0.8 } });

            var blockGroupSensing = new List<BlockData>();

            var blockGroupOperator = new List<BlockData>();
                
            var blockGroupData = new List<BlockData>();

            ToggleList.Add(new BlockList() { Category = "동작", Source = blockGroupMotion });
            ToggleList.Add(new BlockList() { Category = "이벤트", Source = blockGroupEvent });
            ToggleList.Add(new BlockList() { Category = "컨트롤", Source = blockGroupControl });
            ToggleList.Add(new BlockList() { Category = "제어", Source = blockGroupSensing });
            ToggleList.Add(new BlockList() { Category = "연산", Source = blockGroupOperator });
            ToggleList.Add(new BlockList() { Category = "데이터", Source = blockGroupData });

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

        private void block_Drag(object sender, MouseButtonEventArgs e) {
            var block = sender as BlockTemplate;
            var clone = BlockTemplate.CreateBlock((BlockTemplate)block.Content);
            var data = new DataObject(typeof(BlockTemplate), clone);
            DragDrop.DoDragDrop(block, data, DragDropEffects.Copy);
        }

        private void canvas_Drop(object sender, DragEventArgs e) {
            var data = e.Data.GetData(typeof(BlockTemplate)) as BlockTemplate;
            var copy = new BlockTemplate();
            copy.X = e.GetPosition(canvas).X;
            copy.Y = e.GetPosition(canvas).Y;
            copy.Content = data;
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
    }
}
