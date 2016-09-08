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
using Mocca;
using Mocca.DataType;

namespace NewEPL {
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window {
        List<BlockList> ToggleList;
        public Rectangle TestPreview;

        List<MoccaBlockGroup> eval;

        public MainWindow() {
            InitializeComponent();

            ToggleList = new List<BlockList>();

            var blockGroupMotion = new List<BlockData>();
            blockGroupMotion.Add(new BlockData() { Template = new BlockMotionMove() { Width = 180 } });

            var blockGroupEvent = new List<BlockData>();
            blockGroupEvent.Add(new BlockData() { Template = new BlockEventStart() { Width = 180 } });

            var blockGroupControl = new List<BlockData>();
            blockGroupControl.Add(new BlockData() { Template = new BlockControlIf() { Width = 210 } });
            blockGroupControl.Add(new BlockData() { Template = new BlockControlStop() { Width = 180 } });

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


            var parser = new MoccaParser("Resources/middle_lang.mocca", CompileMode.FILE_PASS);
            eval = parser.Parse().Eval();

        }                                                            

        private void block_Drag(object sender, MouseButtonEventArgs e) {
            var block = sender as BlockTemplate;
            var clone = BlockTemplate.CopyBlockContent((BlockTemplate)block.Content);
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

        private BlockTemplate GetBlockForList(Type type) {
            BlockTemplate ret = null;

            bool escaper = false;
            foreach(BlockList i in BlockLists.Items) {
                foreach(var j in i.Source) {
                    if(j.Template.GetType().Equals(type)) {
                        ret = j.Template;
                        escaper = true;
                        break;
                    }
                }
                if (escaper) break;
            }

            return ret;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            ToggleCheck(Toggle_if, true);
            ToggleCheck(Toggle_for, true);
            ToggleCheck(Toggle_text, true);
            ToggleCheck(Toggle_list, true);
            ToggleCheck(Toggle_microbit, true);
            ToggleCheck(Toggle_arduino, true);

            foreach(var i in eval) {
                /// 엔트리 블록 생성
                var entry = new BlockTemplate();
                entry.Content = BlockTemplate.CopyBlockContent(GetBlockForList(typeof(BlockEventStart)));
                entry.X = i.x;
                entry.Y = i.y;
                BlockSpace.Children.Add(entry);

                BlockTemplate block = entry;
                foreach (var j in i.suite) {
                    /// 하위 블록 생성
                    var type = j.GetType();
                    if (type == typeof(MoccaCommand)) {
                        MoccaCommand suite = (MoccaCommand)j;

                        var parent = block;

                        block = new BlockTemplate();
                        block.Content = BlockTemplate.CopyBlockContent(GetBlockForList(typeof(BlockMotionMove)));
                        block.X = 0;
                        block.Y = 0;
                        parent.AddChild(block, parent.GetSplicers(1)[0]);

                        BlockSpace.Children.Add(block);

                    } else if (type == typeof(MoccaLogic)) {
                        MoccaLogic suite = (MoccaLogic)j;


                    } else if (type == typeof(MoccaWhile)) {
                        MoccaWhile suite = (MoccaWhile)j;


                    } else if (type == typeof(MoccaArray)) {
                        MoccaArray suite = (MoccaArray)j;


                    } else if (type == typeof(MoccaFor)) {
                        MoccaFor suite = (MoccaFor)j;


                    } ///기타 등등..
                }
            }
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
