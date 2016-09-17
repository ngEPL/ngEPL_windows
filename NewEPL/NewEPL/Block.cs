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
        //public List<Block> Children = new List<Block>();
        public int TestNum = 0;
        
        // 임시 이름 나중에 바꾸기
        // 암/수, 이름, 실제 영역
        public Dictionary<KeyValuePair<bool, string>, Rect> Cols = new Dictionary<KeyValuePair<bool, string>, Rect>();
        public Dictionary<KeyValuePair<bool, string>, Block> Children = new Dictionary<KeyValuePair<bool, string>, Block>();
        public Block Parent = null;

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

        public double Width, Height;
        public double DefaultWidth, DefaultHeight;

        public BlockType Type;

        NinePatch Patch;

        public Block(string path, double x, double y, double width, double height, BlockType type) {
            //Patch = new NinePatch(path.Split('.')[0] + ".9.png");
            //Patch.ClearCache();

            //var patchedImage = null;

            //var t = new ControlTemplate();
            //var f = new FrameworkElementFactory(typeof(Image));
            //f.SetValue(FrameworkElement.WidthProperty, width * 0.8);
            //f.SetValue(FrameworkElement.HeightProperty, height * 0.8);
            //f.SetValue(Image.SourceProperty, patchedImage);
            //t.VisualTree = f;
            //Template = t; 

            //X = x;
            //Y = y;
            
            //Width = patchedImage.Width * 0.8;
            //Height = patchedImage.Height * 0.8;
            //DefaultWidth = width;
            //DefaultHeight = height;

            ////MessageBox.Show(Width.ToString());

            //Type = type;

            //UpdateColSize();
        }
        
        public void MoveBlocks(double x, double y) {
            X = x;
            Y = y;

            foreach (var i in Children) {
                i.Value.MoveBlocks(i.Value.DifX + x, i.Value.DifY + y);
            }
        }

        public void AddChild(KeyValuePair<bool, string> key, Block child, Rect Col) {
            child.Parent = this;
            Children.Add(key, child);
            child.MoveBlocks(Col.X, Col.Y);
            child.DifX = -(this.X - child.X);
            child.DifY = -(this.Y - child.Y);
        }

        public Rect GetFemale() {
            Rect ret = new Rect();

            foreach(var i in Cols) {
                if (!i.Key.Key) ret = i.Value;
            }

            return ret;
        }

        public void Resize(int width, int height) {
            var t = new ControlTemplate();
            var f = new FrameworkElementFactory(typeof(Image));
            f.SetValue(FrameworkElement.WidthProperty, (double)width * 0.8);
            f.SetValue(FrameworkElement.HeightProperty, (double)height * 0.8);
            //f.SetValue(Image.SourceProperty, Patch.GetPatchedImage(width, height));
            t.VisualTree = f;
            Template = t;

            Width = width * 0.8;
            Height = height * 0.8;

            UpdateColSize();
        }

        void UpdateColSize() {
            Cols.Clear();
            switch (Type) {
            case BlockType.TEST1:
                Cols.Add(new KeyValuePair<bool, string>(true, "Down"), new Rect(0, 0 + Height - 5, 40, 20));
                Cols.Add(new KeyValuePair<bool, string>(false, "Up"), new Rect(0, 0 + 5, 40, 20));

                break;
            case BlockType.TEST2:
                Cols.Add(new KeyValuePair<bool, string>(false, "Up"), new Rect(0, 0 + 5, 40, 20));

                break;
            case BlockType.TEST3:
                Cols.Add(new KeyValuePair<bool, string>(true, "Down"), new Rect(0, 0 + Height - 5, 40, 20));

                break;
            case BlockType.TEST4:
                Cols.Add(new KeyValuePair<bool, string>(false, "Up"), new Rect(0, 0 + 5, 40, 20));
                Cols.Add(new KeyValuePair<bool, string>(true, "Mid"), new Rect(30, 0 + 5 + 35, 40, 20));
                Cols.Add(new KeyValuePair<bool, string>(true, "Down"), new Rect(0, 0 + Height - 5, 40, 20));

                break;
            }
        }

        /// 이름 바꾸기
        public static Rect RealCollider(Block b, Rect r) {
            return new Rect(b.X + r.X, b.Y + r.Y, r.Width, r.Height);
        }
    }

    class BlockList {
        public string Category { get; set; }
        public List<BlockData> Source { get; set; }
    }

    class BlockData {
        //public double Width { get; set; }
        //public double Height { get; set; }
        public BlockTemplate Template { get; set; } 
        //public BlockType Tag { get; set; }
    }

    class PathAndType {
        public string Path;
        public BlockType Type;
    }

}
