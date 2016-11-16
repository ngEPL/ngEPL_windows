using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NewEPL {
    class ExtendedTextBox : TextBox, ISplicer {
        public bool Hidden = false;

        public Rect GetBoundingBox(BlockTemplate block, StackPanel panel) {
            var rpp = TranslatePoint(new Point(0, 0), block);
            var rtp = new Point(0, 0);//TranslatePoint(new Point(0, 0), panel);
            return new Rect(block.X + rpp.X + rtp.X, block.Y + rpp.Y + rtp.Y, ActualWidth, ActualHeight);
        }

        public List<BlockTemplate> BlockChildren = new List<BlockTemplate>();
    }
}
