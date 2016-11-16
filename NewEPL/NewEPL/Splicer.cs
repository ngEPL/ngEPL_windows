using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NewEPL {
    public class Splicer : Control, ISplicer {
        public static readonly DependencyProperty XProperty;
        public static readonly DependencyProperty YProperty;
        public static readonly DependencyProperty RelativeWidthProperty;
        public static readonly DependencyProperty TypeProperty;
        public static readonly DependencyProperty XStackProperty;
        public static readonly DependencyProperty YStackProperty;

        static Splicer() {
            XProperty = DependencyProperty.Register("X", typeof(double), typeof(Image9), new UIPropertyMetadata(null));
            YProperty = DependencyProperty.Register("Y", typeof(double), typeof(Image9), new UIPropertyMetadata(null));
            RelativeWidthProperty = DependencyProperty.Register("RelativeWidth", typeof(int), typeof(Image9), new UIPropertyMetadata(null));
            TypeProperty = DependencyProperty.Register("Type", typeof(bool), typeof(Image9), new UIPropertyMetadata(null));
            XStackProperty = DependencyProperty.Register("XStack", typeof(int), typeof(Image9), new UIPropertyMetadata(null));
            YStackProperty = DependencyProperty.Register("YStack", typeof(int), typeof(Image9), new UIPropertyMetadata(null));
        }

        public double X {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public double Y {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public int RelativeWidth {
            get { return (int)GetValue(RelativeWidthProperty); }
            set { SetValue(RelativeWidthProperty, value); }
        }

        public bool Type {
            get { return (bool)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public int XStack {
            get { return (int)GetValue(XStackProperty); }
            set { SetValue(XStackProperty, value); }
        }

        public int YStack {
            get { return (int)GetValue(YStackProperty); }
            set { SetValue(YStackProperty, value); }
        }

        public List<BlockTemplate> BlockChildren = new List<BlockTemplate>();
    }
}
