using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NewEPL {

    enum BlockType {
        TEST1, TEST2, TEST3, TEST4
    }

    class Block : Thumb{
        public List<Block> Children = new List<Block>();
        // 임시 이름 나중에 바꾸기
        // 암/수, 이름, 실제 영역
        public Dictionary<KeyValuePair<bool, string>, List<Rect>> Cols = new Dictionary<KeyValuePair<bool, string>, List<Rect>>();

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

        /// 이름 나중에 수정하기.
        public double XX = 0;
        public double YY = 0;

        public bool IsFemale {
            get {
                bool ret = false;

                foreach (var i in Cols) {
                    if (!i.Key.Key) {
                        ret = true;
                        break;
                    }
                }

                return ret;
            }
        }

        public double Width;

        public double Height;

        BlockType Type;

        public Block(ImageSource src, double x, double y, double width, BlockType type) {
            var t = new ControlTemplate();
            var f = new FrameworkElementFactory(typeof(Image));
            f.SetValue(FrameworkElement.WidthProperty, width);
            f.SetValue(FrameworkElement.HeightProperty, Double.NaN);
            f.SetValue(Image.SourceProperty, src);
            t.VisualTree = f;
            this.Template = t;

            XX = x;
            YY = y;

            X = x;
            Y = y;

            Width = src.Width;
            Height = src.Height;

            Type = type;

            switch(type) {
            case BlockType.TEST1:
                Cols.Add(new KeyValuePair<bool, string>(true, "Down"), new List<Rect>() { new Rect(0, 0 + src.Height - 4, 40, 8), new Rect(0, 0 + src.Height - 4, 40, 8) });
                Cols.Add(new KeyValuePair<bool, string>(false, "Up"), new List<Rect>() { new Rect(0, 0 + 4, 40, 8), new Rect(0, 0 + 4, 40, 8) });

                break;
            case BlockType.TEST2:
                Cols.Add(new KeyValuePair<bool, string>(false, "Up"), new List<Rect>() { new Rect(0, 0 + 4, 40, 8), new Rect(0, 0 + 4, 40, 8) });

                break;
            case BlockType.TEST3:
                Cols.Add(new KeyValuePair<bool, string>(true, "Down"), new List<Rect>() { new Rect(0, 0 + src.Height - 4 - 8, 40, 8), new Rect(0, 0 + src.Height - 4 - 8, 40, 8) });

                break;
            case BlockType.TEST4:
                Cols.Add(new KeyValuePair<bool, string>(false, "Up"), new List<Rect>() { new Rect(0, 0 + src.Height - 4 - 8, 40, 8), new Rect(0, 0 + src.Height - 4 - 8, 40, 8) });
                Cols.Add(new KeyValuePair<bool, string>(true, "Mid"), new List<Rect>() { new Rect(0, 0 + src.Height - 4 - 8 + 40, 40, 8), new Rect(0, 0 + src.Height - 4 - 8 + 40, 40, 8) });
                Cols.Add(new KeyValuePair<bool, string>(true, "Down"), new List<Rect>() { new Rect(0, 0 + 4, 40, 8), new Rect(0, 0 + 4, 40, 8) });
                //Cols.Add("Down", new Rect(x, y + src.Height - 4 - 8, 40, 8));
                //Cols.Add("Up", new Rect(x, y + 4, 40, 8));

                break;
            }

        }
        
        void MoveCols(double x, double y) {
            foreach (var i in Cols.Keys.ToList()) {
                Cols[i][0] = new Rect(Cols[i][1].X + x, Cols[i][1].Y + y, Cols[i][0].Width, Cols[i][0].Height);
            }
        }

        /// 움직일때 뭉쳐서 움직임...
        public void MoveBlocks(double x, double y) {
            XX = x  ;
            YY = y;

            X = XX;
            Y = YY;

            foreach (var i in Children) {
                i.MoveBlocks(x, y);
            }

            MoveCols(x, y);
        }

        public void AddChild(Block child) {
            Children.Add(child);
            child.MoveBlocks(this.X, this.Y + Height - 18);
        }

        public void Update() {
            X += XX;
            Y += YY;
        }

        /// 여기서 콜라이더 위치가 안바뀌어서 문제가 발생하는듯.
        //void SetPosition(double x, double y) {
        //    X = x;
        //    Y = y;
        //    Move

        //    foreach(var i in Children) {
        //        i.SetPosition(x, y + Height - 18);
        //    }
        //}

        public Rect GetFemale() {
            Rect ret = new Rect();

            foreach(var i in Cols) {
                if (!i.Key.Key) ret = i.Value[0];
            }

            return ret;
        }
    }

    class BlockList {
        public string Name { get; set; }
        public List<BlockData> Source { get; set; }
    }

    class BlockData {
        public int Size { get; set; }
        public string Res { get; set; }
        public BlockType Tag { get; set; }
    }

    class SourceAndType {
        public ImageSource Source;
        public BlockType Type;
    }

}
