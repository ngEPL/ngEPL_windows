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

        List<MoccaBlockGroup> Eval;

        /// <summary>
        ///  파일 불러오고 저장할 때 사용될 블록 엔트리 리스트
        /// </summary>
        List<BlockTemplate> EntryBlockList = new List<BlockTemplate>();

        public MainWindow() {
            InitializeComponent();

            ToggleList = new List<BlockList>();

            var blockGroupMotion = new List<BlockData>();
            blockGroupMotion.Add(new BlockData() { Template = new BlockMotionMove() { Width = 180 } });
            blockGroupMotion.Add(new BlockData() { Template = new BlockMotionDef() { Width = 260 } });

            var blockGroupEvent = new List<BlockData>();
            blockGroupEvent.Add(new BlockData() { Template = new BlockEventStart() { Width = 180 } });

            var blockGroupControl = new List<BlockData>();
            blockGroupControl.Add(new BlockData() { Template = new BlockControlIf() { Width = 210 } });
            blockGroupControl.Add(new BlockData() { Template = new BlockControlWhile() { Width = 210 } });
            blockGroupControl.Add(new BlockData() { Template = new BlockControlStop() { Width = 180 } });

            var blockGroupSensing = new List<BlockData>();

            var blockGroupOperator = new List<BlockData>();
            blockGroupOperator.Add(new BlockData() { Template = new BlockOperatorPlus() { Width = 100 } });

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
            BlockCanvas.Children.Add(TestPreview);

            TestPreview.Visibility = Visibility.Hidden;


            var parser = new MoccaParser("Resources/middle_lang.mocca", CompileMode.FILE_PASS);
            Eval = parser.Parse().Eval();

        }                                                            

        private void block_Drag(object sender, MouseButtonEventArgs e) {
            var block = sender as BlockTemplate;
            var data = new DataObject(typeof(BlockTemplate), (block.Content as BlockTemplate).Clone());
            DragDrop.DoDragDrop(block, data, DragDropEffects.Copy);
        }

        private void canvas_Drop(object sender, DragEventArgs e) {
            var data = e.Data.GetData(typeof(BlockTemplate)) as BlockTemplate;
            var copy = new BlockTemplate();
            copy.X = e.GetPosition(BlockCanvas).X;
            copy.Y = e.GetPosition(BlockCanvas).Y;
            copy.Content = data;
            BlockCanvas.Children.Add(copy);
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

        public new BlockTemplate Cursor = null;

        /// 이름 수정하기
        private void AAA(MoccaSuite s) {
            Type type = s.GetType();

            if (type == typeof(MoccaCommand)) {
                MoccaCommand suite = (MoccaCommand)s;

                Type blockType = null;
                switch(suite.name) {
                case "def": blockType = typeof(BlockMotionDef); break;
                case "cmd": blockType = typeof(BlockMotionMove); break;
                default: blockType = typeof(BlockMotionMove); break;
                }

                var block = new BlockTemplate();
                block.Content = GetBlockForList(blockType).Clone();
                block.X = 0;
                block.Y = 0;
                Cursor.AddChild(block, Cursor.GetSplicers(1)[0]);

                BlockCanvas.Children.Add(block);
                // 내부 블록을 처리해야 함(연산자같은 것).

                Cursor = block;

            } else if (type == typeof(MoccaLogic)) {
                MoccaLogic suite = (MoccaLogic)s;

                var block = new BlockTemplate();
                block.Content = GetBlockForList(typeof(BlockControlIf)).Clone();
                block.X = 0;
                block.Y = 0;

                foreach (var i in Cursor.GetSplicers(1)) {
                    if (((Splicer)VisualTreeHelper.GetChild(i, 0)).YStack == 1) {
                        Cursor.AddChild(block, i);
                        break;
                    }
                }

                BlockCanvas.Children.Add(block);
                
                Cursor = block;
                for (int i = 0; i < suite.cmd_list.Count; i++) {
                    AAA(suite.cmd_list[i]);
                }

                Cursor = block;

            } else if (type == typeof(MoccaWhile)) {
                MoccaWhile suite = (MoccaWhile)s;

                var block = new BlockTemplate();
                block.Content = GetBlockForList(typeof(BlockControlWhile)).Clone();
                block.X = 0;
                block.Y = 0;

                foreach (var i in Cursor.GetSplicers(1)) {
                    if (((Splicer)VisualTreeHelper.GetChild(i, 0)).YStack == 1) {
                        Cursor.AddChild(block, i);
                        break;
                    }
                }

                BlockCanvas.Children.Add(block);

                Cursor = block;
                for (int i = 0; i < suite.cmd_list.Count; i++) {
                    AAA(suite.cmd_list[i]);
                }

                Cursor = block;

            } else if (type == typeof(MoccaArray)) {
                MoccaArray suite = (MoccaArray)s;

            } else if (type == typeof(MoccaFor)) {
                MoccaFor suite = (MoccaFor)s;

            } ///기타 등등..
        }

        private void BlockRefresh(BlockTemplate parent) {

            var splicers = parent.GetSplicers(1);
            for (int i = 0; i < splicers.Count; i++) {
                var splicer = (Splicer)VisualTreeHelper.GetChild(splicers[i], 0);
                foreach (var j in splicer.BlockChildren) {
                    j.IsResized = true;
                    if (i != splicers.Count - 1 && (parent.Content.GetType() == typeof(BlockControlIf) || parent.Content.GetType() == typeof(BlockControlWhile))) {
                        j.CollideBorder = splicers[i];
                        j.CollideSplicer = splicer;
                         (parent.Content as BlockTemplate).IncreaseHeight(splicer, (j.Content as BlockTemplate).GetTotalHeight(), 0);
                    }
                    BlockRefresh(j);
                }
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            ToggleCheck(Toggle_if, true);
            ToggleCheck(Toggle_for, true);
            ToggleCheck(Toggle_text, true);
            ToggleCheck(Toggle_list, true);
            ToggleCheck(Toggle_microbit, true);
            ToggleCheck(Toggle_arduino, true);

            /// 재귀 구조가 필요할 듯.
            foreach(var i in Eval) {
                /// 엔트리 블록 생성
                var entry = new BlockTemplate();
                entry.Content = GetBlockForList(typeof(BlockEventStart)).Clone();
                entry.X = i.x;
                entry.Y = i.y;
                BlockCanvas.Children.Add(entry);

                EntryBlockList.Add(entry);
                
                Cursor = entry;
                foreach (var j in i.suite) {
                    /// 하위 블록 생성
                    AAA(j);
                }
            }

            UpdateLayout();

            /// 화면 리프레시 하기
            foreach (var i in EntryBlockList) {
                BlockRefresh(i);
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
