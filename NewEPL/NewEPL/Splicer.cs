using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NewEPL {
    class Splicer : Control {
        public static readonly DependencyProperty TypeProperty;

        static Splicer() {
            TypeProperty = DependencyProperty.Register("Type", typeof(bool), typeof(Image9), new UIPropertyMetadata(null));
        }

        public bool Type {
            get { return (bool)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
    }
}
