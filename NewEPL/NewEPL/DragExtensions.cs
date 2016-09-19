using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace NewEPL {
    public static class DragExtensions {
        public static DependencyProperty IgnoreFocusProperty = DependencyProperty.RegisterAttached("IgnoreFocus", typeof(bool), typeof(ListboxExtensions), new UIPropertyMetadata(false, IgnoreFocusChanged));

        public static bool GetIgnoreFocus(DependencyObject dependencyObject) {
            return (bool)dependencyObject.GetValue(IgnoreFocusProperty);
    }

        public static void SetIgnoreFocus(DependencyObject dependencyObject, bool value) {
            dependencyObject.SetValue(IgnoreFocusProperty, value);
        }

        private static void IgnoreFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var newValue = (bool)e.NewValue;
            var oldValue = (bool)e.OldValue;

            var fe = d as FrameworkElement;
            if (fe == null) return;

            if (!newValue || oldValue || fe.IsFocused) return;

            //fe.PreviewMouseDown += Fe_PreviewMouseDown;
            //fe. += Fe_ManipulationStarted;
        }

        private static void Fe_ManipulationDelta(object sender, ManipulationDeltaEventArgs e) {
            throw new NotImplementedException();
        }

        private static void Fe_ManipulationStarted(object sender, ManipulationStartedEventArgs e) {
            throw new NotImplementedException();
        }

        private static void Fe_DragEnter(object sender, DragEventArgs e) {
            if (e.Handled) return;

            e.Handled = true;

            var eventArg = new DragStartedEventArgs(0, 0) {
                RoutedEvent = UIElement.DragEnterEvent,
                Source = sender
            };

            var parent = (FrameworkElement)VisualTreeHelper.GetChild(((FrameworkElement)sender).Parent, 0);
            if (parent != null) parent.RaiseEvent(eventArg);
        }

        private static void Fe_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.Handled) return;

            e.Handled = true;
            var eventArg = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton) {
                RoutedEvent = UIElement.PreviewMouseDownEvent,
                Source = sender
            };

            var parent = (FrameworkElement)VisualTreeHelper.GetChild(((FrameworkElement)sender).Parent, 0);
            if (parent != null) parent.RaiseEvent(eventArg);
        }
    }
}
