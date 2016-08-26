using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NewEPL {
    class Splicer : Control {
        public static readonly DependencyProperty RelativeWidthProperty;
        public static readonly DependencyProperty TypeProperty;
        public static readonly DependencyProperty XXProperty;

        static Splicer() {
            RelativeWidthProperty = DependencyProperty.Register("RelativeWidth", typeof(int), typeof(Image9), new UIPropertyMetadata(null));
            TypeProperty = DependencyProperty.Register("Type", typeof(bool), typeof(Image9), new UIPropertyMetadata(null));
            XXProperty = DependencyProperty.Register("XX", typeof(int), typeof(Image9), new UIPropertyMetadata(null));
        }

        public int RelativeWidth {
            get { return (int)GetValue(RelativeWidthProperty); }
            set { SetValue(RelativeWidthProperty, value); }
        }

        public bool Type {
            get { return (bool)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public int XX {
            get { return (int)GetValue(XXProperty); }
            set { SetValue(XXProperty, value); }
        }
    }
}
